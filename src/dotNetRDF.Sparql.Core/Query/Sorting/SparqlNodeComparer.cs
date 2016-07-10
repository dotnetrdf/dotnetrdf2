/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2015 dotNetRDF Project (dotnetrdf-develop@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using VDS.RDF.Nodes;
using VDS.RDF.Specifications;

namespace VDS.RDF.Query.Sorting
{

    /// <summary>
    /// Comparer class for implementing the SPARQL semantics for the relational operators
    /// </summary>
    public class SparqlNodeComparer 
        : IComparer<INode>, IComparer<IValuedNode>
    {
        /// <summary>
        /// Compares two Nodes
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        public virtual int Compare(INode x, INode y)
        {
            //Nulls are less than everything
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            //If the Node Types are different use Node ordering
            if (x.NodeType != y.NodeType) return x.CompareTo(y);

            if (x.NodeType == NodeType.Literal)
            {
                //Do they have supported Data Types?
                String xtype, ytype;
                try
                {
                    xtype = XmlSpecsHelper.GetSupportedDataType(x);
                    ytype = XmlSpecsHelper.GetSupportedDataType(y);
                }
                catch (RdfException)
                {
                    //Can't determine a Data Type for one/both of the Nodes so use Node ordering
                    return x.CompareTo(y);
                }

                if (xtype.Equals(String.Empty) || ytype.Equals(String.Empty))
                {
                    //One/both has an unknown type
                    if (x.Equals(y))
                    {
                        //If RDF Term equality returns true then we return that they are equal
                        return 0;
                    }
                    else
                    {
                        //If RDF Term equality returns false then we error
                        //UNLESS they have the same Datatype
                        throw new RdfQueryException("Unable to determine ordering since one/both arguments has an Unknown Type");
                    }
                }
                else
                {
                    //Both have known types
                    EffectiveNumericType xnumtype = SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(xtype);
                    EffectiveNumericType ynumtype = SparqlSpecsHelper.GetNumericTypeFromDataTypeUri(ytype);
                    EffectiveNumericType numtype = (EffectiveNumericType)Math.Max((int)xnumtype, (int)ynumtype);
                    if (numtype != EffectiveNumericType.NaN)
                    {
                        if (xnumtype == EffectiveNumericType.NaN || ynumtype == EffectiveNumericType.NaN)
                        {
                            //If one is non-numeric then we can't assume non-equality
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments does not return a Number");
                        }

                        //Both are Numeric so use Numeric ordering
                        try
                        {
                            return this.NumericCompare(x, y, numtype);
                        }
                        catch (FormatException)
                        {
                            if (x.Equals(y)) return 0;
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments does not contain a valid value for it's type");
                        }
                        catch (RdfQueryException)
                        {
                            //If this errors try RDF Term equality since 
                            if (x.Equals(y)) return 0;
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments was not a valid numeric");
                        }
                    }
                    else if (xtype.Equals(ytype))
                    {
                        switch (xtype)
                        {
                            case XmlSpecsHelper.XmlSchemaDataTypeDate:
                                return DateCompare(x, y);
                            case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                                return DateTimeCompare(x, y);
                            case XmlSpecsHelper.XmlSchemaDataTypeString:
                                //Both Strings so use Lexical string ordering
                                return Options.DefaultCulture.CompareInfo.Compare(x.Value, y.Value, Options.DefaultComparisonOptions);
                            default:
                                //Use node ordering
                                return x.CompareTo(y);
                        }
                    }
                    else
                    {
                        String commontype = XmlSpecsHelper.GetCompatibleSupportedDataType(xtype, ytype, true);
                        if (commontype.Equals(String.Empty))
                        {
                            //Use Node ordering
                            return x.CompareTo(y);
                        }
                        else
                        {
                            switch (commontype)
                            {
                                case XmlSpecsHelper.XmlSchemaDataTypeDate:
                                    return DateCompare(x, y);
                                case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                                    return DateTimeCompare(x, y);
                                default:
                                    //Use Node ordering
                                    return x.CompareTo(y);
                            }
                        }
                    }
                }
            }
            else
            {
                //If not Literals use Node ordering
                return x.CompareTo(y);
            }
        }

        /// <summary>
        /// Compares two valued Nodes
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        public virtual int Compare(IValuedNode x, IValuedNode y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            //If the Node Types are different use Node ordering
            if (x.NodeType != y.NodeType) return x.CompareTo(y);

            if (x.NodeType == NodeType.Literal)
            {
                //Do they have supported Data Types?
                String xtype, ytype;
                try
                {
                    xtype = XmlSpecsHelper.GetSupportedDataType(x);
                    ytype = XmlSpecsHelper.GetSupportedDataType(y);
                }
                catch (RdfException)
                {
                    //Can't determine a Data Type for one/both of the Nodes so use Node ordering
                    return x.CompareTo(y);
                }

                if (xtype.Equals(String.Empty) || ytype.Equals(String.Empty))
                {
                    //One/both has an unknown type
                    if (x.Equals(y))
                    {
                        //If RDF Term equality returns true then we return that they are equal
                        return 0;
                    }
                    else
                    {
                        //If RDF Term equality returns false then we error
                        //UNLESS they have the same Datatype
                        throw new RdfQueryException("Unable to determine ordering since one/both arguments has an Unknown Type");
                    }
                }
                else
                {
                    //Both have known types
                    EffectiveNumericType xnumtype = x.NumericType;
                    EffectiveNumericType ynumtype = y.NumericType;
                    EffectiveNumericType numtype = (EffectiveNumericType)Math.Max((int)xnumtype, (int)ynumtype);
                    if (numtype != EffectiveNumericType.NaN)
                    {
                        if (xnumtype == EffectiveNumericType.NaN || ynumtype == EffectiveNumericType.NaN)
                        {
                            //If one is non-numeric then we can't assume non-equality
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments does not return a Number");
                        }

                        //Both are Numeric so use Numeric ordering
                        try
                        {
                            return this.NumericCompare(x, y, numtype);
                        }
                        catch (FormatException)
                        {
                            if (x.Equals(y)) return 0;
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments does not contain a valid value for it's type");
                        }
                        catch (RdfQueryException)
                        {
                            //If this errors try RDF Term equality since 
                            if (x.Equals(y)) return 0;
                            throw new RdfQueryException("Unable to determine ordering since one/both arguments was not a valid numeric");
                        }
                    }
                    else if (xtype.Equals(ytype))
                    {
                        switch (xtype)
                        {
                            case XmlSpecsHelper.XmlSchemaDataTypeDate:
                                return DateCompare(x, y);
                            case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                                return DateTimeCompare(x, y);
                            case XmlSpecsHelper.XmlSchemaDataTypeString:
                                //Both Strings so use Lexical string ordering
                                return Options.DefaultCulture.CompareInfo.Compare(x.Value, y.Value, Options.DefaultComparisonOptions);
                            default:
                                //Use node ordering
                                return x.CompareTo(y);
                        }
                    }
                    else
                    {
                        String commontype = XmlSpecsHelper.GetCompatibleSupportedDataType(xtype, ytype, true);
                        if (commontype.Equals(String.Empty))
                        {
                            //Use Node ordering
                            return x.CompareTo(y);
                        }
                        else
                        {
                            switch (commontype)
                            {
                                case XmlSpecsHelper.XmlSchemaDataTypeDate:
                                    return DateCompare(x, y);
                                case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                                    return DateTimeCompare(x, y);
                                default:
                                    //Use Node ordering
                                    return x.CompareTo(y);
                            }
                        }
                    }
                }
            }
            else
            {
                //If not Literals use standard Node ordering
                return x.CompareTo(y);
            }
        }

        /// <summary>
        /// Compares two Nodes for Numeric Ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <param name="type">Numeric Type</param>
        /// <returns></returns>
        protected virtual int NumericCompare(INode x, INode y, EffectiveNumericType type)
        {
            return NumericCompare(x.AsValuedNode(), y.AsValuedNode(), type);
        }

        /// <summary>
        /// Compares two Nodes for Numeric Ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <param name="type">Numeric Type</param>
        /// <returns></returns>
        protected virtual int NumericCompare(IValuedNode x, IValuedNode y, EffectiveNumericType type)
        {
            if (x == null || y == null) throw new RdfQueryException("Cannot evaluate numeric ordering when one or both arguments are Null");
            if (type == EffectiveNumericType.NaN) throw new RdfQueryException("Cannot evaluate numeric ordering when the Numeric Type is NaN");

            switch (type)
            {
                case EffectiveNumericType.Decimal:
                    return x.AsDecimal().CompareTo(y.AsDecimal());
                case EffectiveNumericType.Double:
                    return x.AsDouble().CompareTo(y.AsDouble());
                case EffectiveNumericType.Float:
                    return x.AsFloat().CompareTo(y.AsFloat());
                case EffectiveNumericType.Integer:
                    return x.AsInteger().CompareTo(y.AsInteger());
                default:
                    throw new RdfQueryException("Cannot evaluate numeric equality since of the arguments is not numeric");
            }
        }

        /// <summary>
        /// Compares two Date Times for Date Time ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        protected virtual int DateTimeCompare(INode x, INode y)
        {
            return DateTimeCompare(x.AsValuedNode(), y.AsValuedNode());
        }

        /// <summary>
        /// Compares two Date Times for Date Time ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        protected virtual int DateTimeCompare(IValuedNode x, IValuedNode y)
        {
            if (x == null || y == null) throw new RdfQueryException("Cannot evaluate date time comparison when one or both arguments are Null");
            try
            {
                DateTime c = x.AsDateTime();
                DateTime d = y.AsDateTime();

                switch (c.Kind)
                {
                    case DateTimeKind.Unspecified:
                        if (d.Kind != DateTimeKind.Unspecified) throw new RdfQueryException(
                                "Dates are incomparable, one specifies time zone information while the other does not");
                        break;
                    case DateTimeKind.Local:
                        if (d.Kind == DateTimeKind.Unspecified) throw new RdfQueryException(
                                "Dates are incomparable, one specifies time zone information while the other does not");
                        c = c.ToUniversalTime();
                        if (d.Kind == DateTimeKind.Local) d = d.ToUniversalTime();
                        break;
                    default:
                        if (d.Kind == DateTimeKind.Unspecified) throw new RdfQueryException(
                                "Dates are incomparable, one specifies time zone information while the other does not");
                        if (d.Kind == DateTimeKind.Local) d = d.ToUniversalTime();
                        break;
                }

                // Compare on unspecified/UTC form as appropriate
                return c.CompareTo(d);
            }
            catch (FormatException)
            {
                throw new RdfQueryException("Cannot evaluate date time comparison since one of the arguments does not have a valid lexical value for a Date");
            }
        }

        /// <summary>
        /// Compares two Dates for Date ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        protected virtual int DateCompare(INode x, INode y)
        {
            return DateCompare(x.AsValuedNode(), y.AsValuedNode());
        }

        /// <summary>
        /// Compares two Dates for Date ordering
        /// </summary>
        /// <param name="x">Node</param>
        /// <param name="y">Node</param>
        /// <returns></returns>
        protected virtual int DateCompare(IValuedNode x, IValuedNode y)
        {
            if (x == null || y == null) throw new RdfQueryException("Cannot evaluate date comparison when one or both arguments are Null");
            try
            {
                DateTime c = x.AsDateTime();
                DateTime d = y.AsDateTime();

                switch (c.Kind)
                {
                    case DateTimeKind.Unspecified:
                        break;
                    case DateTimeKind.Local:
                        c = c.ToUniversalTime();
                        if (d.Kind == DateTimeKind.Local) d = d.ToUniversalTime();
                        break;
                    default:
                        if (d.Kind == DateTimeKind.Local) d = d.ToUniversalTime();
                        break;
                }

                // Timezone irrelevant for date comparisons since we don't have any time to normalize to
                // Thus Open World Assumption means we can compare
                // For Local times we normalize to UTC

                // Compare on the Unspecified/UTC form as appropriate
                int res = c.Year.CompareTo(d.Year);
                if (res == 0)
                {
                    res = c.Month.CompareTo(d.Month);
                    if (res == 0)
                    {
                        res = c.Day.CompareTo(d.Day);
                    }
                }
                return res;
            }
            catch (FormatException)
            {
                throw new RdfQueryException("Cannot evaluate date comparison since one of the arguments does not have a valid lexical value for a Date");
            }
        }
    }
}

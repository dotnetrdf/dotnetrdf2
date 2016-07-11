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
using VDS.RDF.Nodes;
using VDS.RDF.Query.Engine;

namespace VDS.RDF.Query.Expressions.Arithmetic
{
    /// <summary>
    /// Class representing Unary Minus expressions (sign of numeric expression is reversed)
    /// </summary>
    public class MinusExpression
        : BaseUnaryExpression
    {
        /// <summary>
        /// Creates a new Unary Minus Expression
        /// </summary>
        /// <param name="expr">Expression to apply the Minus operator to</param>
        public MinusExpression(IExpression expr) 
            : base(expr) { }

        public override IExpression Copy(IExpression argument)
        {
            return new MinusExpression(argument);
        }

        /// <summary>
        /// Calculates the Numeric Value of this Expression as evaluated for a given Binding
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        public override IValuedNode Evaluate(ISolution solution, IExpressionContext context)
        {
            IValuedNode a = this.Argument.Evaluate(solution, context);
            if (a == null) throw new RdfQueryException("Cannot apply unary minus to a null");

            switch (a.NumericType)
            {
                case EffectiveNumericType.Integer:
                    return new LongNode(-1 * a.AsInteger());

                case EffectiveNumericType.Decimal:
                    decimal decvalue = a.AsDecimal();
                    if (decvalue == Decimal.Zero)
                    {
                        return new DecimalNode(Decimal.Zero);
                    }
                    else
                    {
                        return new DecimalNode(-1 * decvalue);
                    }
                case EffectiveNumericType.Float:
                    float fltvalue = a.AsFloat();
                    if (Single.IsNaN(fltvalue))
                    {
                        return new FloatNode(Single.NaN);
                    }
                    else if (Single.IsPositiveInfinity(fltvalue))
                    {
                        return new FloatNode(Single.NegativeInfinity);
                    }
                    else if (Single.IsNegativeInfinity(fltvalue))
                    {
                        return new FloatNode(Single.PositiveInfinity);
                    }
                    else
                    {
                        return new FloatNode(-1.0f * fltvalue);
                    }
                case EffectiveNumericType.Double:
                    double dblvalue = a.AsDouble();
                    if (Double.IsNaN(dblvalue))
                    {
                        return new DoubleNode(Double.NaN);
                    }
                    else if (Double.IsPositiveInfinity(dblvalue))
                    {
                        return new DoubleNode(Double.NegativeInfinity);
                    }
                    else if (Double.IsNegativeInfinity(dblvalue))
                    {
                        return new DoubleNode(Double.PositiveInfinity);
                    }
                    else
                    {
                        return new DoubleNode(-1.0 * dblvalue);
                    }
                default:
                    throw new RdfQueryException("Cannot evalute an Arithmetic Expression when the Numeric Type of the expression cannot be determined");
            }
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return "-";
            }
        }
    }
}

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
using System.Globalization;
using VDS.RDF.Specifications;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// A Valued Node with a Long value
    /// </summary>
    public class LongNode
        : NumericNode
    {
        private long _value;

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="value">Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        public LongNode(long value, String lexicalValue)
            : this(value, lexicalValue, UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger)) { }

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="value">Long value</param>
        /// <param name="lexicalValue">Lexical Value</param>
        /// <param name="datatype">Datatype URI</param>
        public LongNode(long value, String lexicalValue, Uri datatype)
            : base(lexicalValue, datatype, EffectiveNumericType.Integer)
        {
            this._value = value;
        }

        /// <summary>
        /// Creates a new long valued node
        /// </summary>
        /// <param name="value">Long value</param>
        public LongNode(long value)
            : this(value, value.ToString(CultureInfo.InvariantCulture)) { }

        /// <summary>
        /// Gets the long value
        /// </summary>
        /// <returns></returns>
        public override long AsInteger()
        {
            return this._value;
        }

        /// <summary>
        /// Gets the decimal value of the long
        /// </summary>
        /// <returns></returns>
        public override decimal AsDecimal()
        {
            try
            {
                return Convert.ToDecimal(this._value);
            }
            catch
            {
                throw new NodeValueException("Unable to upcast Long to Double");
            }
        }

        /// <summary>
        /// Gets the float value of the long
        /// </summary>
        /// <returns></returns>
        public override float AsFloat()
        {
            try
            {
                return Convert.ToSingle(this._value);
            }
            catch
            {
                throw new NodeValueException("Unable to upcast Long to Float");
            }
        }

        /// <summary>
        /// Gets the double value of the long
        /// </summary>
        /// <returns></returns>
        public override double AsDouble()
        {
            try
            {
                return Convert.ToDouble(this._value);
            }
            catch
            {
                throw new NodeValueException("Unable to upcast Long to Double");
            }
        }
    }
}

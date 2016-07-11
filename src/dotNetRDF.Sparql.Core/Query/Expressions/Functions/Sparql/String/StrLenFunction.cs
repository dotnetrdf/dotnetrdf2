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

using VDS.RDF.Nodes;
using VDS.RDF.Specifications;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.String
{
    /// <summary>
    /// Represents the SPARQL STRLEN Function
    /// </summary>
    public class StrLenFunction
        : XPath.String.BaseUnaryStringFunction
    {
        /// <summary>
        /// Creates a new STRLEN() function
        /// </summary>
        /// <param name="expr">Argument Expression</param>
        public StrLenFunction(IExpression expr)
            : base(expr) { }

        /// <summary>
        /// Determines the Length of the given String Literal
        /// </summary>
        /// <param name="stringLit">String Literal</param>
        /// <returns></returns>
        protected override IValuedNode EvaluateInternal(INode stringLit)
        {
            return new LongNode(stringLit.Value.Length);
        }

        public override IExpression Copy(IExpression argument)
        {
            return new StrLenFunction(argument);
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return SparqlSpecsHelper.SparqlKeywordStrLen;
            }
        }
    }
}

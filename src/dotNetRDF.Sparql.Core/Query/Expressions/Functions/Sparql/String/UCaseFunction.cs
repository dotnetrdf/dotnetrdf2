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
using System.Linq;
using System.Text;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Expressions.Functions.XPath.String;
using VDS.RDF.Specifications;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.String
{
    /// <summary>
    /// Represents the SPARQL UCASE Function
    /// </summary>
    public class UCaseFunction
        : BaseUnaryStringFunction
    {
        /// <summary>
        /// Creates a new UCASE() function
        /// </summary>
        /// <param name="expr">Argument Expression</param>
        public UCaseFunction(IExpression expr)
            : base(expr) { }

        /// <summary>
        /// Converts the given String Literal to upper case
        /// </summary>
        /// <param name="stringLit">String Literal</param>
        /// <returns></returns>
        protected override IValuedNode EvaluateInternal(INode stringLit)
        {
            if (stringLit.HasLanguage) return new StringNode(Options.DefaultCulture.TextInfo.ToUpper(stringLit.Value), stringLit.Language);
            return stringLit.HasDataType ? new StringNode(Options.DefaultCulture.TextInfo.ToUpper(stringLit.Value), stringLit.DataType) : new StringNode(Options.DefaultCulture.TextInfo.ToUpper(stringLit.Value));
        }

        public override IExpression Copy(IExpression argument)
        {
            return new UCaseFunction(argument);
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return SparqlSpecsHelper.SparqlKeywordUCase;
            }
        }
    }
}

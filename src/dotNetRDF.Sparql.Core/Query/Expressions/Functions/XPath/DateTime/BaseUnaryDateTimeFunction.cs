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

namespace VDS.RDF.Query.Expressions.Functions.XPath.DateTime
{
    /// <summary>
    /// Abstract Base Class for functions which are Unary functions applied to Date Time objects in the XPath function library
    /// </summary>
    public abstract class BaseUnaryDateTimeFunction
        : BaseUnaryExpression
    {
        /// <summary>
        /// Creates a new Unary XPath Date Time function
        /// </summary>
        /// <param name="expr"></param>
        protected BaseUnaryDateTimeFunction(IExpression expr)
            : base(expr) { }

        /// <summary>
        /// Gets the numeric value of the function in the given Evaluation Context for the given Binding ID
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        public override IValuedNode Evaluate(ISolution solution, IExpressionContext context)
        {
            IValuedNode temp = this.Argument.Evaluate(solution, context);
            if (temp != null)
            {
                return this.ValueInternal(temp.AsDateTimeOffset());
            }
            throw new RdfQueryException("Unable to evaluate an XPath Date Time function on a null argument");
        }

        /// <summary>
        /// Abstract method which derived classes must implement to generate the actual numeric value for the function
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns></returns>
        protected abstract IValuedNode ValueInternal(DateTimeOffset dateTime);
    }
}

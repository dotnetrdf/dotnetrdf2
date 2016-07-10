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
using VDS.RDF.Query.Engine;
using VDS.RDF.Specifications;

namespace VDS.RDF.Query.Expressions.Functions.XPath.String
{
    /// <summary>
    /// Abstract Base class for XPath NAry String functions, typically these are functions that have an optional second argument and therefore may be unary/binary and thus we have to represent them as NAry functions
    /// </summary>
    public abstract class BaseNAryStringFunction
        : BaseNAryExpression
    {
        /// <summary>
        /// Type validation function for the argument
        /// </summary>
        protected Func<Uri, bool> _argumentTypeValidator;

        /// <summary>
        /// Creates a new XPath Binary String function
        /// </summary>
        /// <param name="stringExpr">Expression</param>
        /// <param name="argExpr">Argument</param>
        /// <param name="argumentTypeValidator">Type validator for the argument</param>
        protected BaseNAryStringFunction(IExpression stringExpr, IExpression argExpr, Func<Uri, bool> argumentTypeValidator)
            : base(MakeArguments(stringExpr, argExpr))
        {
            this._argumentTypeValidator = argumentTypeValidator;
        }

        private static IEnumerable<IExpression> MakeArguments(IExpression stringExpr, IExpression argExpr)
        {
            return argExpr != null ? new IExpression[] {stringExpr, argExpr} : stringExpr.AsEnumerable();
        }

        /// <summary>
        /// Gets the Value of the function as evaluated in the given Context for the given Binding ID
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        public override IValuedNode Evaluate(ISolution solution, IExpressionContext context)
        {
            INode temp = this.Arguments[0].Evaluate(solution, context);
            if (temp == null) throw new RdfQueryException("Unable to evaluate an XPath String function on a null input");
            if (temp.NodeType == NodeType.Literal)
            {
                INode lit = temp;
                if (lit.DataType != null && !lit.DataType.AbsoluteUri.Equals(XmlSpecsHelper.XmlSchemaDataTypeString))
                {
                    throw new RdfQueryException("Unable to evalaute an XPath String function on a non-string typed Literal");
                }
            }
            else
            {
                throw new RdfQueryException("Unable to evaluate an XPath String function on a non-Literal input");
            }

            //Once we've got to here we've established that the First argument is an appropriately typed/untyped Literal
            if (this.Arguments.Count == 1)
            {
                return this.ValueInternal(temp);
            }
            //Need to validate the argument
            INode tempArg = this.Arguments[1].Evaluate(solution, context);
            if (tempArg != null)
            {
                if (tempArg.NodeType != NodeType.Literal) throw new RdfQueryException("Unable to evaluate an XPath String function where the argument is a non-Literal");
                INode litArg = tempArg;
                if (this._argumentTypeValidator(litArg.DataType))
                {
                    return this.ValueInternal(temp, litArg);
                }
                throw new RdfQueryException("Unable to evaluate an XPath String function since the type of the argument is not supported by this function");
            }
            //Null argument permitted so just invoke the non-argument version of the function
            return this.ValueInternal(temp);
        }

        /// <summary>
        /// Gets the Value of the function as applied to the given String Literal
        /// </summary>
        /// <param name="stringLit">Simple/String typed Literal</param>
        /// <returns></returns>
        public abstract IValuedNode ValueInternal(INode stringLit);

        /// <summary>
        /// Gets the Value of the function as applied to the given String Literal and Argument
        /// </summary>
        /// <param name="stringLit">Simple/String typed Literal</param>
        /// <param name="arg">Argument</param>
        /// <returns></returns>
        public abstract IValuedNode ValueInternal(INode stringLit, INode arg);
    }
}
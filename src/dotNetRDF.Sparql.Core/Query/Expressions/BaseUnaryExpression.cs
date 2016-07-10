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
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Query.Expressions
{
    /// <summary>
    /// Abstract base class for Unary Expressions
    /// </summary>
    public abstract class BaseUnaryExpression 
        : IUnaryExpression
    {
        /// <summary>
        /// Creates a new Base Unary Expression
        /// </summary>
        /// <param name="argument">Argument</param>
        protected BaseUnaryExpression(IExpression argument)
        {
            this.Argument = argument;
        }

        /// <summary>
        /// The sub-expression of this Expression
        /// </summary>
        public IExpression Argument { get; private set; }

        public virtual IExpression Copy()
        {
            return Copy(this.Argument.Copy());
        }

        public abstract IExpression Copy(IExpression argument);

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <param name="solution">Solution</param>
        /// <param name="context">Evaluation Context</param>
        /// <returns></returns>
        public abstract IValuedNode Evaluate(ISolution solution, IExpressionContext context);

        public bool Equals(IExpression other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            if (!(other is IUnaryExpression)) return false;
            if (!this.Functor.Equals(other.Functor)) return false;

            IUnaryExpression expr = (IUnaryExpression) other;
            return this.Argument.Equals(expr.Argument);
        }

        /// <summary>
        /// Gets the String representation of the Expression
        /// </summary>
        /// <returns></returns>
        public sealed override string ToString()
        {
            return ToString(new AlgebraFormatter());
        }

        public string ToString(IAlgebraFormatter formatter)
        {
            String f = SparqlSpecsHelper.IsFunctionKeyword11(this.Functor) ? this.Functor.ToLowerInvariant() : String.Format("<{0}>", formatter.FormatUri(this.Functor));
            return String.Format("{0}({1})", f, this.Argument.ToString(formatter));
        }

        public string ToPrefixString()
        {
            return ToPrefixString(new AlgebraFormatter());
        }

        public string ToPrefixString(IAlgebraFormatter formatter)
        {
            String f = SparqlSpecsHelper.IsFunctionKeyword11(this.Functor) ? this.Functor.ToLowerInvariant() : String.Format("<{0}>", formatter.FormatUri(this.Functor));
            return String.Format("({0} {1})", f, this.Argument.ToPrefixString(formatter));
        }

        /// <summary>
        /// Gets an enumeration of all the Variables used in this expression
        /// </summary>
        public virtual IEnumerable<string> Variables
        {
            get
            {
                return this.Argument.Variables;
            }
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public abstract String Functor
        {
            get;
        }

        /// <summary>
        /// Gets whether an expression can safely be evaluated in parallel
        /// </summary>
        public bool CanParallelise
        {
            get
            {
                // Assume that if the argument can parallelise so can we
                return this.Argument.CanParallelise;
            }
        }

        public bool IsDeterministic
        {
            get
            {
                // Assume that if the argument is deterministic then we are too
                return this.Argument.IsDeterministic;
            }
        }

        public bool IsConstant
        {
            get
            {
                // Assume that if we are deterministic and the argument is constant then we will be constant
                return this.IsDeterministic && this.Argument.IsConstant;
            }
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(Object other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            if (!(other is BaseUnaryExpression)) return false;

            return this.Equals((BaseUnaryExpression) other);
        }

        public override int GetHashCode()
        {
            return Tools.CombineHashCodes(this.Functor, this.Argument);
        }
    }
}
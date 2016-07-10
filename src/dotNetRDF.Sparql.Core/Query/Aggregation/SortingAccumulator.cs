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
using VDS.RDF.Query.Expressions;

namespace VDS.RDF.Query.Aggregation
{
    /// <summary>
    /// An accumulator that accumulates a single value decided which value to keep based on a given comparer
    /// </summary>
    public class SortingAccumulator
        : BaseExpressionAccumulator
    {
        public SortingAccumulator(IExpression expr, IComparer<IValuedNode> comparer)
            : this(expr, comparer, null) { }

        public SortingAccumulator(IExpression expr, IComparer<IValuedNode> comparer, IValuedNode initialValue)
            : base(expr, initialValue)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            this.Comparer = comparer;
        }

        public IComparer<IValuedNode> Comparer { get; private set; }

        public override bool Equals(IAccumulator other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            if (!(other is SortingAccumulator)) return false;

            SortingAccumulator sorter = (SortingAccumulator) other;
            // TODO Will need the standard comparers to override Equals() appropriately
            return this.Expression.Equals(sorter.Expression) && this.Comparer.Equals(sorter.Comparer);
        }

        protected internal override void Accumulate(IValuedNode value)
        {
            // If the new value exceeds the existing value (according to the comparer then we keep the new value)
            if (this.Comparer.Compare(this.AccumulatedResult, value) > 0)
            {
                this.AccumulatedResult = value;
            }
        }
    }
}

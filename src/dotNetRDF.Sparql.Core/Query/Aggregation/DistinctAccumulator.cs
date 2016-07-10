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

using System.Collections.Generic;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Aggregation
{
    /// <summary>
    /// A decorator around other accumulators which only accumulates distinct values
    /// </summary>
    public class DistinctAccumulator
        : BaseExpressionAccumulator
    {
        private readonly ISet<IValuedNode> _values = new HashSet<IValuedNode>();

        public DistinctAccumulator(BaseExpressionAccumulator accumulator)
            : this(accumulator, null) {}

        public DistinctAccumulator(BaseExpressionAccumulator accumulator, IValuedNode initialValue)
            : base(accumulator.Expression, initialValue)
        {
            this.InnerAccumulator = accumulator;
        }

        public BaseExpressionAccumulator InnerAccumulator { get; private set; }

        public override bool Equals(IAccumulator other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            if (!(other is DistinctAccumulator)) return false;

            DistinctAccumulator distinct = (DistinctAccumulator) other;
            return this.InnerAccumulator.Equals(distinct.InnerAccumulator);
        }

        protected internal override void Accumulate(IValuedNode value)
        {
            // Check if we've already seen this item
            if (!this._values.Add(value)) return;

            this.InnerAccumulator.Accumulate(value);
        }

        public override IValuedNode AccumulatedResult
        {
            get { return this.InnerAccumulator.AccumulatedResult; }
            protected internal set { this.InnerAccumulator.AccumulatedResult = value; }
        }
    }
}
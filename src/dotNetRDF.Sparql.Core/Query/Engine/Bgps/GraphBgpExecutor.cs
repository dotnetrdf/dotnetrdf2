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
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Engine.Bgps
{
    /// <summary>
    /// A BGP executor over a single <see cref="IGraph"/> instance
    /// </summary>
    /// <remarks>
    /// When used only BGPs that target the default graph identified by the special node <see cref="Quad.DefaultGraphNode"/> will match, any other graph will return empty results
    /// </remarks>
    public class GraphBgpExecutor
        : BaseBgpExecutor
    {
        public GraphBgpExecutor(IGraph g)
        {
            if (g == null) throw new ArgumentNullException("g");
            this.Graph = g;
        }

        public IGraph Graph { get; private set; }

        protected override IEnumerable<Quad> Find(INode g, INode s, INode p, INode o)
        {
            return Quad.DefaultGraphNode.Equals(g) ? this.Graph.Find(s, p, o).AsQuads(g) : Enumerable.Empty<Quad>();
        }
    }
}
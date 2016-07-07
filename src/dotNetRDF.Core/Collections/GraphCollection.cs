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
using System.Linq;
using VDS.Common.Collections;
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;

namespace VDS.RDF.Collections
{
    /// <summary>
    /// Basic implementation of a Graph Collection
    /// </summary>
    public class GraphCollection 
        : BaseGraphCollection
    {
        /// <summary>
        /// Dictionary of Graph names to Graphs
        /// </summary>
        protected MultiDictionary<INode, IGraph> _graphs;

        /// <summary>
        /// Creates a new Graph Collection
        /// </summary>
        public GraphCollection()
        {
            this._graphs = new MultiDictionary<INode, IGraph>(n => n.GetHashCode(), false, Comparer<INode>.Default, MultiDictionaryMode.AVL);
        }

        /// <summary>
        /// Checks whether the Graph with the given Uri exists in this Graph Collection
        /// </summary>
        /// <param name="graphName">Graph name to test</param>
        /// <returns></returns>
        public override bool ContainsKey(INode graphName)
        {
            return this._graphs.ContainsKey(graphName);
        }

        /// <summary>
        /// Adds a Graph to the Collection
        /// </summary>
        /// <param name="graphName">Graph to add to</param>
        /// <param name="g">Graph to add</param>
        public override void Add(INode graphName, IGraph g)
        {
            if (this._graphs.ContainsKey(graphName))
            {
                // Add to the existing Graph
                this._graphs[graphName].Assert(g.Triples);
            }
            else
            {
                //Safe to add a new Graph
                this._graphs.Add(graphName, g);
            }
        }

        /// <summary>
        /// Removes a Graph from the Collection
        /// </summary>
        /// <param name="graphName">Name of the Graph to remove</param>
        public override bool Remove(INode graphName)
        {
            IGraph g;
            if (!this._graphs.TryGetValue(graphName, out g)) return false;
            if (!this._graphs.Remove(graphName)) return false;
            return true;
        }

        /// <summary>
        /// Clears the graphs from the collection
        /// </summary>
        public override void Clear()
        {
            this._graphs.Clear();
        } 

        /// <summary>
        /// Gets the number of Graphs in the Collection
        /// </summary>
        public override int Count
        {
            get
            {
                return this._graphs.Count;
            }
        }

        /// <summary>
        /// Provides access to the names of the Graphs in the Collection
        /// </summary>
        public override ICollection<INode> Keys
        {
            get
            {
                return this._graphs.Keys;
            }
        }

        /// <summary>
        /// Gets the graphs in the collection
        /// </summary>
        public override ICollection<IGraph> Values
        {
            get 
            {
                return this._graphs.Values; 
            }
        }

        /// <summary>
        /// Gets a Graph from the Collection
        /// </summary>
        /// <param name="graphName">Graph name</param>
        /// <returns></returns>
        public override IGraph this[INode graphName]
        {
            get 
            {
                IGraph g;
                if (this._graphs.TryGetValue(graphName, out g))
                {
                    return g;
                }
                throw new RdfException("The Graph with the given name does not exist in this Graph Collection");
            }
            set
            {
                this.Add(graphName, value);
            }
        }

        /// <summary>
        /// Gets the Enumerator for the Collection
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<KeyValuePair<INode, IGraph>> GetEnumerator()
        {
            return this._graphs.GetEnumerator();
        }

        /// <summary>
        /// Disposes of the Graph Collection
        /// </summary>
        public override void Dispose()
        {
            //No unmanaged resources to dispose of
        }
    }
}

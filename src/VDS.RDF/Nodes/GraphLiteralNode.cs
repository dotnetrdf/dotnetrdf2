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
using System.Text;
using VDS.RDF.Graphs;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Abstract Base Class for Graph Literal Nodes
    /// </summary>
    public abstract class BaseGraphLiteralNode
        : BaseNode, IEquatable<BaseGraphLiteralNode>, IComparable<BaseGraphLiteralNode>, IValuedNode
    {
        /// <summary>
        /// Creates a new Graph Literal Node in the given Graph which represents the given Subgraph
        /// </summary>
        /// <param name="g">Graph this node is in</param>
        /// <param name="subgraph">Sub Graph this node represents</param>
        protected internal BaseGraphLiteralNode(IGraph subgraph)
            : base(NodeType.GraphLiteral)
        {
            this.SubGraph = subgraph;

            //Compute Hash Code
            this._hashcode = Tools.CreateHashCode(this);
        }

        /// <summary>
        /// Creates a new Graph Literal Node whose value is an empty Subgraph
        /// </summary>
        /// <param name="g">Graph this node is in</param>
        protected internal BaseGraphLiteralNode()
            : this(new Graph()) { }

        /// <summary>
        /// Gets the Subgraph that this Node represents
        /// </summary>
        public override IGraph SubGraph { get; protected set; }

        /// <summary>
        /// Implementation of the Equals method for Graph Literal Nodes.  Graph Literals are considered Equal if their respective Subgraphs are equal
        /// </summary>
        /// <param name="obj">Object to compare the Node with</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj is INode)
            {
                return this.Equals((INode)obj);
            }
            else
            {
                //Can only be equal to other Nodes
                return false;
            }
        }

        /// <summary>
        /// Implementation of the Equals method for Graph Literal Nodes.  Graph Literals are considered Equal if their respective Subgraphs are equal
        /// </summary>
        /// <param name="other">Object to compare the Node with</param>
        /// <returns></returns>
        public override bool Equals(INode other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            //Can only be equal to a Graph Literal Node
            return other.NodeType == NodeType.GraphLiteral && EqualityHelper.AreGraphLiteralsEqual(this, other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Graph Literal Node
        /// </summary>
        /// <param name="other">Graph Literal Node</param>
        /// <returns></returns>
        public bool Equals(BaseGraphLiteralNode other)
        {
            return this.Equals((INode)other);
        }

        /// <summary>
        /// Implementation of ToString for Graph Literals which produces a String representation of the Subgraph in N3 style syntax
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            //Use N3 Style notation for Graph Literal string representation
            output.Append("{");

            //Add all the Triples in the Subgraph
            foreach (Triple t in this.SubGraph.Triples)
            {
                output.Append(t.ToString());
            }

            output.Append("}");
            return output.ToString();
        }

        /// <summary>
        /// Implementation of CompareTo for Graph Literals
        /// </summary>
        /// <param name="other">Node to compare to</param>
        /// <returns></returns>
        /// <remarks>
        /// Graph Literal Nodes are greater than Blank Nodes, Uri Nodes, Literal Nodes and Nulls
        /// </remarks>
        public override int CompareTo(INode other)
        {
            if (ReferenceEquals(this, other)) return 0;

            if (other == null)
            {
                //Everything is greater than a null
                //Return a 1 to indicate this
                return 1;
            }
            if (other.NodeType != NodeType.GraphLiteral)
            {
                //Graph Literal Nodes are greater than Blank, Variable, Uri and Literal Nodes
                //Return a 1 to indicate this
                return 1;
            }
            if (other.NodeType == NodeType.GraphLiteral)
            {
                return ComparisonHelper.CompareGraphLiterals(this, (INode)other);
            }
            //Anything else is Greater Than us
            return -1;
        }

        /// <summary>
        /// Returns an Integer indicating the Ordering of this Node compared to another Node
        /// </summary>
        /// <param name="other">Node to test against</param>
        /// <returns></returns>
        public int CompareTo(BaseGraphLiteralNode other)
        {
            return this.CompareTo((INode)other);
        }

        #region IValuedNode Members

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a string
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to an integer
        /// </summary>
        /// <returns></returns>
        public long AsInteger()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a decimal
        /// </summary>
        /// <returns></returns>
        public decimal AsDecimal()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a float
        /// </summary>
        /// <returns></returns>
        public float AsFloat()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a double
        /// </summary>
        /// <returns></returns>
        public double AsDouble()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a boolean
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a date time
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a date time
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset AsDateTimeOffset()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Throws an error as Graph Literals cannot be cast to a time span
        /// </summary>
        /// <returns></returns>
        public TimeSpan AsTimeSpan()
        {
            throw new NodeValueException("Unable to cast a Graph Literal Node to a type");
        }

        /// <summary>
        /// Gets the URI of the datatype this valued node represents as a String
        /// </summary>
        public String EffectiveType
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the numeric type of the node
        /// </summary>
        public EffectiveNumericType NumericType
        {
            get 
            {
                return EffectiveNumericType.NaN; 
            }
        }

        #endregion
    }

    /// <summary>
    /// Class for representing Graph Literal Nodes which are supported in highly expressive RDF syntaxes like Notation 3
    /// </summary>
    public class GraphLiteralNode 
        : BaseGraphLiteralNode, IEquatable<GraphLiteralNode>, IComparable<GraphLiteralNode>
    {
        /// <summary>
        /// Creates a new Graph Literal Node whose value is an empty Subgraph
        /// </summary>
        /// <param name="g">Graph this node is in</param>
        /// <param name="subgraph">Sub-graph this node represents</param>
        public GraphLiteralNode(IGraph subgraph)
            : base(subgraph) { }

        /// <summary>
        /// Implementation of Compare To for Graph Literal Nodes
        /// </summary>
        /// <param name="other">Graph Literal Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Simply invokes the more general implementation of this method
        /// </remarks>
        public int CompareTo(GraphLiteralNode other)
        {
            return this.CompareTo((INode)other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Graph Literal Node
        /// </summary>
        /// <param name="other">Graph Literal Node</param>
        /// <returns></returns>
        public bool Equals(GraphLiteralNode other)
        {
            return base.Equals((INode)other);
        }

    }
}

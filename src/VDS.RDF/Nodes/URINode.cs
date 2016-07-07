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

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Abstract Base Class for URI Nodes
    /// </summary>
    public abstract class BaseUriNode
        : BaseNode, IEquatable<BaseUriNode>, IComparable<BaseUriNode>, IValuedNode
    {
        /// <summary>
        /// Internal Only Constructor for URI Nodes
        /// </summary>
        /// <param name="uri">URI</param>
        protected internal BaseUriNode(Uri uri)
            : base(NodeType.Uri)
        {
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.Uri = uri;

            //Compute Hash Code
            this._hashcode = Tools.CreateHashCode(this);
        }

        /// <summary>
        /// Gets the Uri for this Node
        /// </summary>
        public override Uri Uri { get; protected set; }

        /// <summary>
        /// Implementation of Equality for Uri Nodes
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns></returns>
        /// <remarks>
        /// URI Nodes are considered equal if their various segments are equivalent based on URI comparison rules, see <see cref="EqualityHelper.AreUrisEqual()" />
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is INode)
            {
                return this.Equals((INode) obj);
            }
            //Can only be equal to other Nodes
            return false;
        }

        /// <summary>
        /// Implementation of Equality for Uri Nodes
        /// </summary>
        /// <param name="other">Object to compare with</param>
        /// <returns></returns>
        /// <remarks>
        /// URI Nodes are considered equal if the string form of their URIs match using Ordinal string comparison
        /// </remarks>
        public override bool Equals(INode other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            //Can only be equal to UriNodes
            return other.NodeType == NodeType.Uri && EqualityHelper.AreUrisEqual(this.Uri, other.Uri);
        }

        /// <summary>
        /// Determines whether this Node is equal to a URI Node
        /// </summary>
        /// <param name="other">URI Node</param>
        /// <returns></returns>
        public bool Equals(BaseUriNode other)
        {
            return this.Equals((INode) other);
        }

        /// <summary>
        /// Gets a String representation of a Uri as a plain text Uri
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Implementation of Compare To for Uri Nodes
        /// </summary>
        /// <param name="other">Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Uri Nodes are greater than Blank Nodes and Nulls, they are less than Literal Nodes and Graph Literal Nodes.
        /// <br /><br />
        /// Uri Nodes are ordered based upon lexical ordering of the string value of their URIs
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
            if (other.NodeType == NodeType.Blank || other.NodeType == NodeType.Variable)
            {
                //URI Nodes are greater than Blank and Variable Nodes
                //Return a 1 to indicate this
                return 1;
            }
            if (other.NodeType == NodeType.Uri)
            {
                //Return the result of CompareTo using the URI comparison helper
                return ComparisonHelper.CompareUris(this.Uri, other.Uri);
            }
            //Anything else is considered greater than a URI Node
            //Return -1 to indicate this
            return -1;
        }

        /// <summary>
        /// Returns an Integer indicating the Ordering of this Node compared to another Node
        /// </summary>
        /// <param name="other">Node to test against</param>
        /// <returns></returns>
        public int CompareTo(BaseUriNode other)
        {
            return this.CompareTo((INode) other);
        }

        #region IValuedNode Members

        /// <summary>
        /// Gets the value of the node as a string
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            return this.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to numerics
        /// </summary>
        /// <returns></returns>
        public long AsInteger()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to numerics
        /// </summary>
        /// <returns></returns>
        public decimal AsDecimal()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to numerics
        /// </summary>
        /// <returns></returns>
        public float AsFloat()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to numerics
        /// </summary>
        /// <returns></returns>
        public double AsDouble()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to a boolean
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to a date time
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to a date time
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset AsDateTimeOffset()
        {
            throw new NodeValueException("Cannot cast a URI to a type");
        }

        /// <summary>
        /// Throws an error as URIs cannot be cast to a time span
        /// </summary>
        /// <returns></returns>
        public TimeSpan AsTimeSpan()
        {
            throw new NodeValueException("Cannot case a URI to a type");
        }

        /// <summary>
        /// Gets the URI of the datatype this valued node represents as a String
        /// </summary>
        public String EffectiveType
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Gets the numeric type of the expression
        /// </summary>
        public EffectiveNumericType NumericType
        {
            get { return EffectiveNumericType.NaN; }
        }

        #endregion
    }

    /// <summary>
    /// Class for representing URI Nodes
    /// </summary>
    public class UriNode
        : BaseUriNode, IEquatable<UriNode>, IComparable<UriNode>
    {
        /// <summary>
        /// Internal Only Constructor for URI Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="uri">URI for the Node</param>
        public UriNode(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Implementation of Compare To for URI Nodes
        /// </summary>
        /// <param name="other">URI Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Simply invokes the more general implementation of this method
        /// </remarks>
        public int CompareTo(UriNode other)
        {
            return base.CompareTo((INode) other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a URI Node
        /// </summary>
        /// <param name="other">URI Node</param>
        /// <returns></returns>
        public bool Equals(UriNode other)
        {
            return base.Equals((INode) other);
        }
    }
}
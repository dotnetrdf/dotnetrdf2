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
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using VDS.RDF.Query;
using VDS.RDF.Query.Expressions;
using VDS.RDF.Nodes;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Abstract Base Class for Blank Nodes
    /// </summary>
#if !SILVERLIGHT
    [Serializable,XmlRoot(ElementName="bnode")]
#endif
    public abstract class BaseBlankNode
        : BaseNode, IEquatable<BaseBlankNode>, IComparable<BaseBlankNode>, IValuedNode
    {
        /// <summary>
        /// Internal Only Constructor for Blank Nodes
        /// </summary>
        /// <param name="g">Graph this Node belongs to</param>
        protected internal BaseBlankNode(Guid id)
            : base(NodeType.Blank)
        {
            this.AnonID = id;
            //Compute Hash Code
            this._hashcode = Tools.CreateHashCode(this);
        }

#if !SILVERLIGHT

    /// <summary>
    /// Constructor for deserialization usage only
    /// </summary>
        protected BaseBlankNode()
            : base(NodeType.Blank) { }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">Serialization Information</param>
        /// <param name="context">Streaming Context</param>
        protected BaseBlankNode(SerializationInfo info, StreamingContext context)
            : this((Guid)info.GetValue("id", typeof(Guid))) { }

#endif

        public override Guid AnonID { get; protected set; }

        /// <summary>
        /// Implementation of Equals for Blank Nodes
        /// </summary>
        /// <param name="obj">Object to compare with the Blank Node</param>
        /// <returns></returns>
        /// <remarks>
        /// Blank Nodes are considered equal if their internal IDs match precisely and they originate from the same Graph
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj is INode)
            {
                return this.Equals((INode) obj);
            }
            //Can only be equal to things which are Nodes
            return false;
        }

        /// <summary>
        /// Implementation of Equals for Blank Nodes
        /// </summary>
        /// <param name="other">Object to compare with the Blank Node</param>
        /// <returns></returns>
        /// <remarks>
        /// Blank Nodes are considered equal if their Anon IDs match precisely and they originate from the same Factory
        /// </remarks>
        public override bool Equals(INode other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            //Can only be equal to Blank Nodes
            return other.NodeType == NodeType.Blank && EqualityHelper.AreBlankNodesEqual(this, other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Blank Node
        /// </summary>
        /// <param name="other">Blank Node</param>
        /// <returns></returns>
        public bool Equals(BaseBlankNode other)
        {
            return EqualityHelper.AreBlankNodesEqual(this, other);
        }

        /// <summary>
        /// Returns an Integer indicating the Ordering of this Node compared to another Node
        /// </summary>
        /// <param name="other">Node to test against</param>
        /// <returns></returns>
        public override int CompareTo(INode other)
        {
            if (ReferenceEquals(this, other)) return 0;

            if (other == null)
            {
                //Blank Nodes are considered greater than nulls
                //So we return a 1 to indicate we're greater than it
                return 1;
            }
            if (other.NodeType == NodeType.Variable)
            {
                //Blank Nodes are considered greater than Variables
                return 1;
            }
            if (other.NodeType == NodeType.Blank)
            {
                //Order Blank Nodes lexically by their ID
                return ComparisonHelper.CompareBlankNodes(this, other);
            }
            //Anything else is greater than a Blank Node
            //So we return a -1 to indicate we are less than the other Node
            return -1;
        }

        /// <summary>
        /// Returns an Integer indicating the Ordering of this Node compared to another Node
        /// </summary>
        /// <param name="other">Node to test against</param>
        /// <returns></returns>
        public int CompareTo(BaseBlankNode other)
        {
            return ComparisonHelper.CompareBlankNodes(this, other);
        }

        /// <summary>
        /// Returns a string representation of this Blank Node in QName form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "_:" + this.AnonID;
        }

#if !SILVERLIGHT

        #region ISerializable Members

        /// <summary>
        /// Gets the data for serialization
        /// </summary>
        /// <param name="info">Serialization Information</param>
        /// <param name="context">Streaming Context</param>
        public sealed override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", this.AnonID);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Reads the data for XML deserialization
        /// </summary>
        /// <param name="reader">XML Reader</param>
        public sealed override void ReadXml(XmlReader reader)
        {
            this.AnonID = new Guid(reader.ReadElementContentAsString());
            //Compute Hash Code
            this._hashcode = Tools.CreateHashCode(this);
        }

        /// <summary>
        /// Writes the data for XML serialization
        /// </summary>
        /// <param name="writer">XML Writer</param>
        public sealed override void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this.AnonID.ToString());
        }

        #endregion

#endif

        #region IValuedNode Members

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a String
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to an integer
        /// </summary>
        /// <returns></returns>
        public long AsInteger()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a decimal
        /// </summary>
        /// <returns></returns>
        public decimal AsDecimal()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a float
        /// </summary>
        /// <returns></returns>
        public float AsFloat()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a double
        /// </summary>
        /// <returns></returns>
        public double AsDouble()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a boolean
        /// </summary>
        /// <returns></returns>
        public bool AsBoolean()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a date time
        /// </summary>
        /// <returns></returns>
        public DateTime AsDateTime()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be cast to a date time offset
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset AsDateTimeOffset()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Throws an error as a Blank Node cannot be case to a time span
        /// </summary>
        /// <returns></returns>
        public TimeSpan AsTimeSpan()
        {
            throw new NodeValueException("Unable to cast a Blank Node to a type");
        }

        /// <summary>
        /// Gets the URI of the datatype this valued node represents as a String
        /// </summary>
        public String EffectiveType
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Gets the Numeric Type of the Node
        /// </summary>
        public EffectiveNumericType NumericType
        {
            get { return EffectiveNumericType.NaN; }
        }

        #endregion
    }

    /// <summary>
    /// Class for representing Blank RDF Nodes
    /// </summary>
#if !SILVERLIGHT
    [Serializable,XmlRoot(ElementName = "bnode")]
#endif
    public class BlankNode
        : BaseBlankNode, IEquatable<BlankNode>, IComparable<BlankNode>
    {
        /// <summary>
        /// Internal Only Constructor for Blank Nodes
        /// </summary>
        /// <param name="id">ID</param>
        public BlankNode(Guid id)
            : base(id)
        {
        }

#if !SILVERLIGHT

    /// <summary>
    /// Constructor for deserialization usage only
    /// </summary>
        protected BlankNode()
            : base()
        { }
        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">Serialization Information</param>
        /// <param name="context">Streaming Context</param>
        protected BlankNode(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif

        /// <summary>
        /// Implementation of Compare To for Blank Nodes
        /// </summary>
        /// <param name="other">Blank Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Simply invokes the more general implementation of this method
        /// </remarks>
        public int CompareTo(BlankNode other)
        {
            return this.CompareTo((INode) other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Blank Node
        /// </summary>
        /// <param name="other">Blank Node</param>
        /// <returns></returns>
        public bool Equals(BlankNode other)
        {
            return base.Equals((INode) other);
        }
    }
}
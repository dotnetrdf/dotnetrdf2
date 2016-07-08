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
using System.IO;
using VDS.RDF.Graphs;
using VDS.RDF.Namespaces;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF
{
    /// <summary>
    /// Interface to be implemented by RDF Writers which generate RDF Concrete Syntax
    /// </summary>
    /// <remarks>
    /// This interface provides methods for writing a single graph or a graph store, when the target syntax and the input type do not align the following behaviour is expected to be honoured:
    /// <ul>
    /// <li>If the target syntax is a graph format and a <see cref="IGraphStore"/> is given then only the unnamed default graph should be serialized, all other graphs are ignored</li>
    /// <li>If the target syntax is a dataset format and a <see cref="IGraph"/> is given then treat the given graph as the unnamed default graph and serialize only that</li>
    /// </ul>
    /// </remarks>
    public interface IRdfWriter
    {
        /// <summary>
        /// Method for saving a graph to a concrete RDF syntax via some arbitrary <see cref="TextWriter" />
        /// </summary>
        /// <param name="g">Graph</param>
        /// <param name="output">The text writer to write to</param>
        /// <exception cref="RdfException">Thrown if the RDF in the graph is not representable by the writer</exception>
        /// <exception cref="IOException">Thrown if the writer is unable to write to the underlying storage of the <see cref="TextWriter">TextWriter</see> specified in the <paramref name="output"/> parameter</exception>
        /// <remarks>
        /// If this writer is for a dataset syntax then treat the graph as if it were the unnamed default graph of the dataset and serialize as such
        /// </remarks>
        void Save(IGraph g, TextWriter output);

        /// <summary>
        /// Method for saving a graph store to a concrete RDF syntax via some arbitrary <see cref="TextWriter"/>
        /// </summary>
        /// <param name="graphStore">Graph Store</param>
        /// <param name="output">Text writer to write to</param>
        /// <exception cref="RdfException">Thrown if the RDF in the graph store is not representable by the writer</exception>
        /// <exception cref="IOException">Thrown if the writer is unable to write to the underlying storage of the <see cref="TextWriter">TextWriter</see> specified in the <paramref name="output"/> parameter</exception>
        /// <remarks>
        /// If this writer is for a graph syntax then only serialize the unnamed default graph of the store and ignore any other graphs
        /// </remarks>
        void Save(IGraphStore graphStore, TextWriter output);

        /// <summary>
        /// Event which writers can raise to indicate possible ambiguities or issues in the syntax they are producing
        /// </summary>
        event RdfWriterWarning Warning;
    }
}

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Interface for Writers that Support Pretty Printing
    /// </summary>
    public interface IPrettyPrintingWriter
    {
        /// <summary>
        /// Gets/Sets whether Pretty Printing Mode should be used
        /// </summary>
        bool PrettyPrintMode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that Support engaging High Speed Write Mode for some Graphs
    /// </summary>
    public interface IHighSpeedWriter
    {
        /// <summary>
        /// Gets/Sets whether the Writer can use High Speed Write Mode if the Graph is deemed suitable for this
        /// </summary>
        bool HighSpeedModePermitted
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that support varying levels of Syntax Compression
    /// </summary>
    public interface ICompressingWriter
    {
        /// <summary>
        /// Gets/Sets the Compression Level that the Writer is using
        /// </summary>
        /// <remarks>Compression Level is an arbitrary figure that the Writer can interpret as it wants, implementations of this interface should state in the XML Comments for this property what the different values mean.  The Standard Compression levels provided by the <see cref="WriterCompressionLevel">WriterCompressionLevel</see> enumeration are intended as guides and Writers may interpret these as they desire.</remarks>
        int CompressionLevel
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that support use of DTDs to compress output
    /// </summary>
    public interface IDtdWriter
    {
        /// <summary>
        /// Gets/Sets whether DTDs can be used
        /// </summary>
        bool UseDtd
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that can use attributes (e.g. XML or HTML based writers) which allows you to control whether the writer will choose to use attributes to encode data which could otherwise be expressed as elements
    /// </summary>
    public interface IAttributeWriter
    {
        /// <summary>
        /// Gets/Sets whether literal objects can be compressed as attributes
        /// </summary>
        bool UseAttributes
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that support the use of Namespaces and allows a set of Default Namespaces to be defined
    /// </summary>
    public interface INamespaceWriter
    {
        /// <summary>
        /// Gets/Sets the Default Namespaces used for writing
        /// </summary>
        INamespaceMapper DefaultNamespaces
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that support multi-threaded writing
    /// </summary>
    [Obsolete("Multi-threaded writing is unstable and is being removed from the library", true)]
    public interface IMultiThreadedWriter
    {
        /// <summary>
        /// Gets/Sets whether multi-threading is used
        /// </summary>
        bool UseMultiThreadedWriting
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Writers that generate HTML
    /// </summary>
    public interface IHtmlWriter
    {
        /// <summary>
        /// Gets/Sets a Stylesheet file used to format the HTML
        /// </summary>
        String Stylesheet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the anchor tags used to display the URIs of URI Nodes
        /// </summary>
        String CssClassUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Blank Node IDs
        /// </summary>
        String CssClassBlankNode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Literals
        /// </summary>
        String CssClassLiteral
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the anchor tags used to display Literal datatypes
        /// </summary>
        String CssClassDatatype
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the span tags used to display Literal language specifiers
        /// </summary>
        String CssClassLangSpec
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the CSS class used for the div tags used to group chunks of markup into a box
        /// </summary>
        String CssClassBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets a Prefix that is applied to all href attributes
        /// </summary>
        String UriPrefix
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for writers which use formatters from the Formatting namespace
    /// </summary>
    public interface IFormatterBasedWriter
    {
        /// <summary>
        /// Gets the Type for the Triple Formatter this writer uses
        /// </summary>
        /// <remarks>
        /// This should be the type descriptor for a type that implements <see cref="ITripleFormatter">ITripleFormatter</see>
        /// </remarks>
        Type TripleFormatterType
        {
            get;
        }

        // TODO Add a QuadFormatterType property
    }


}

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
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;

namespace VDS.RDF
{
    /// <summary>
    /// Interface for RDF handlers
    /// </summary>
    /// <remarks>
    /// While originally designed primarily as a low level parsing sink API this has proven to be much more useful and generalisable and is used throughout the API in other ways e.g. processing SPARQL CONSTRUCT results.
    /// </remarks>
    public interface IRdfHandler 
        : INodeFactory
    {
        /// <summary>
        /// Start the Handling of RDF
        /// </summary>
        /// <exception cref="RdfParseException">May be thrown if the Handler is already in use and the implementation is not thread-safe</exception>
        void StartRdf();

        /// <summary>
        /// End the Handling of RDF
        /// </summary>
        /// <param name="ok">Whether parsing finished without error</param>
        void EndRdf(bool ok);

        /// <summary>
        /// Handles a Namespace Definition
        /// </summary>
        /// <param name="prefix">Namespace Prefix</param>
        /// <param name="namespaceUri">Namespace URI</param>
        /// <returns>Should return <strong>true</strong> if parsing should continue or <strong>false</strong> if it should be terminated</returns>
        bool HandleNamespace(String prefix, Uri namespaceUri);

        /// <summary>
        /// Handles a Base URI Definition
        /// </summary>
        /// <param name="baseUri">Base URI</param>
        /// <returns>Should return <strong>true</strong> if parsing should continue or <strong>false</strong> if it should be terminated</returns>
        bool HandleBaseUri(Uri baseUri);

        /// <summary>
        /// Handles a Triple
        /// </summary>
        /// <param name="t">Triple</param>
        /// <returns>Should return <strong>true</strong> if parsing should continue or <strong>false</strong> if it should be terminated</returns>
        bool HandleTriple(Triple t);

        /// <summary>
        /// Handles a Quad
        /// </summary>
        /// <param name="q">Quad</param>
        /// <returns>Should return <strong>true</strong> if parsing should continue or <strong>false</strong> if it should be terminated</returns>
        bool HandleQuad(Quad q);

        /// <summary>
        /// Gets whether the Handler will always handle all data (i.e. won't terminate parsing early)
        /// </summary>
        bool AcceptsAll
        {
            get;
        }
    }
}

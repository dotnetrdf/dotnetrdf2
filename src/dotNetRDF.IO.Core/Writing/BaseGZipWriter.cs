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

#if !NO_COMPRESSION

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using VDS.RDF.Graphs;

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Abstract base class for RDF writers that generate GZipped output
    /// </summary>
    /// <remarks>
    /// <para>
    /// While the normal witers can be used with GZip streams directly this class just abstracts the wrapping of file/stream output into a GZip stream if it is not already passed as such
    /// </para>
    /// </remarks>
    public abstract class BaseGZipWriter
        : IRdfWriter
    {
        private IRdfWriter _writer;

        /// <summary>
        /// Creates a new GZipped writer
        /// </summary>
        /// <param name="writer">Underlying writer</param>
        protected BaseGZipWriter(IRdfWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            this._writer = writer;
            this._writer.Warning += this.RaiseWarning;
        }

        /// <summary>
        /// Saves a Graph as GZipped output
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="output">Writer to save to</param>
        public void Save(IGraph g, TextWriter output)
        {
            if (g == null) throw new RdfOutputException("Cannot write RDF from a null Graph");

            if (output is StreamWriter)
            {
                //Check for inner GZipStream and re-wrap if required
                StreamWriter streamOutput = (StreamWriter)output;
                if (streamOutput.BaseStream is GZipStream)
                {
                    this._writer.Save(g, streamOutput);
                }
                else
                {
                    streamOutput = new StreamWriter(new GZipStream(streamOutput.BaseStream, CompressionMode.Compress));
                    this._writer.Save(g, streamOutput);
                }
            }
            else
            {
                // TODO Provide an adaptor to write GZipped content to any TextWriter
                throw new RdfOutputException("GZipped Output can only be written to StreamWriter instances");
            }
        }

        public void Save(IGraphStore graphStore, TextWriter output)
        {
            if (graphStore == null) throw new RdfOutputException("Cannot write RDF from a null Graph Store");

            if (output is StreamWriter)
            {
                //Check for inner GZipStream and re-wrap if required
                StreamWriter streamOutput = (StreamWriter)output;
                if (streamOutput.BaseStream is GZipStream)
                {
                    this._writer.Save(graphStore, streamOutput);
                }
                else
                {
                    streamOutput = new StreamWriter(new GZipStream(streamOutput.BaseStream, CompressionMode.Compress));
                    this._writer.Save(graphStore, streamOutput);
                }
            }
            else
            {
                // TODO Provide an adaptor to write GZipped content to any TextWriter
                throw new RdfOutputException("GZipped Output can only be written to StreamWriter instances");
            }
        }

        /// <summary>
        /// Helper method for raising warning events
        /// </summary>
        /// <param name="message">Warning message</param>
        private void RaiseWarning(String message)
        {
            RdfWriterWarning d = this.Warning;
            if (d != null) d(message);
        }

        /// <summary>
        /// Event which is raised if non-fatal errors occur writing RDF output
        /// </summary>
        public event RdfWriterWarning Warning;

        /// <summary>
        /// Gets the description of the writer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "GZipped " + this._writer.ToString();
        }
    }

    /// <summary>
    /// Writer for GZipped NTriples
    /// </summary>
    public class GZippedNTriplesWriter
        : BaseGZipWriter
    {
        /// <summary>
        /// Creates a new GZipped NTriples writer
        /// </summary>
        public GZippedNTriplesWriter()
            : base(new NTriplesWriter()) { }
    }

    /// <summary>
    /// Writer for GZipped Turtle
    /// </summary>
    public class GZippedTurtleWriter
        : BaseGZipWriter
    {
        /// <summary>
        /// Creates a new GZipped Turtle writer
        /// </summary>
        public GZippedTurtleWriter()
            : base(new CompressingTurtleWriter()) { }
    }

    /// <summary>
    /// Writer for GZipped Notation 3
    /// </summary>
    public class GZippedNotation3Writer
        : BaseGZipWriter
    {
        /// <summary>
        /// Creates a new GZipped Notation 3 writer
        /// </summary>
        public GZippedNotation3Writer()
            : base(new Notation3Writer()) { }
    }

    public class GZippedNQuadsWriter
        : BaseGZipWriter
    {
        public GZippedNQuadsWriter()
            : base(new NQuadsWriter()) { }
    }

    public class GZippedTriGWriter
        : BaseGZipWriter
    {
        public GZippedTriGWriter()
            : base(new TriGWriter()) { }
    }

    
    public class GZippedCsvWriter
        : BaseGZipWriter
    {
        public GZippedCsvWriter()
            : base(new CsvWriter()) { }
    }

    public class GZippedTsvWriter
        : BaseGZipWriter
    {
        public GZippedTsvWriter()
            : base(new TsvWriter()) { }
    }
}

#endif
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
using System.Text;
using System.IO;
using VDS.RDF.Graphs;
using VDS.RDF.Namespaces;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Specifications;
using VDS.RDF.Writing.Contexts;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Class for generating Notation 3 Concrete RDF Syntax which provides varying levels of Syntax Compression
    /// </summary>
    /// <threadsafety instance="true">Designed to be Thread Safe - should be able to call the Save() method from multiple threads on different Graphs without issue</threadsafety>
    public class Notation3Writer 
        : BaseGraphWriter, IPrettyPrintingWriter, IHighSpeedWriter, ICompressingWriter, INamespaceWriter, IFormatterBasedWriter
    {
        private bool _prettyprint = true;
        private bool _allowHiSpeed = true;
        private int _compressionLevel = WriterCompressionLevel.Default;
        private INamespaceMapper _defaultNamespaces = new NamespaceMapper();

        /// <summary>
        /// Creates a new Notation 3 Writer which uses the Default Compression Level
        /// </summary>
        public Notation3Writer()
        {

        }

        /// <summary>
        /// Creates a new Notation 3 Writer which uses the given Compression Level
        /// </summary>
        /// <param name="compressionLevel">Desired Compression Level</param>
        /// <remarks>See Remarks for this classes <see cref="Notation3Writer.CompressionLevel">CompressionLevel</see> property to see what effect different compression levels have</remarks>
        public Notation3Writer(int compressionLevel)
        {
            this._compressionLevel = compressionLevel;
        }

        /// <summary>
        /// Gets/Sets whether Pretty Printing is used
        /// </summary>
        public bool PrettyPrintMode
        {
            get
            {
                return this._prettyprint;
            }
            set
            {
                this._prettyprint = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether High Speed Write Mode should be allowed
        /// </summary>
        public bool HighSpeedModePermitted
        {
            get
            {
                return this._allowHiSpeed;
            }
            set
            {
                this._allowHiSpeed = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Compression Level to be used
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the Compression Level is set to <see cref="WriterCompressionLevel.None">None</see> then High Speed mode will always be used regardless of the input Graph and the <see cref="Notation3Writer.HighSpeedModePermitted">HighSpeedMorePermitted</see> property.
        /// </para>
        /// <para>
        /// If the Compression Level is set to <see cref="WriterCompressionLevel.Minimal">Minimal</see> or above then full Predicate Object lists will be used for Triples.
        /// </para>
        /// <para>
        /// If the Compression Level is set to <see cref="WriterCompressionLevel.More">More</see> or above then Blank Node Collections and Collection syntax will be used if the Graph contains Triples that can be compressed in that way.</para>
        /// </remarks>
        public int CompressionLevel
        {
            get
            {
                return this._compressionLevel;
            }
            set
            {
                this._compressionLevel = value;
            }
        }

        /// <summary>
        /// Gets/Sets the Default Namespaces that are always available
        /// </summary>
        public INamespaceMapper DefaultNamespaces
        {
            get
            {
                return this._defaultNamespaces;
            }
            set
            {
                this._defaultNamespaces = value;
            }
        }

        /// <summary>
        /// Gets the type of the Triple Formatter used by this writer
        /// </summary>
        public Type TripleFormatterType
        {
            get
            {
                return typeof(Notation3Formatter);
            }
        }

        /// <summary>
        /// Saves a Graph to the given Stream using Notation 3 Syntax
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="output">Stream to save to</param>
        public override void Save(IGraph g, TextWriter output)
        {
            try
            {
                g.Namespaces.Import(this._defaultNamespaces);
                CompressingTurtleWriterContext context = new CompressingTurtleWriterContext(g, output, this._compressionLevel, this._prettyprint, this._allowHiSpeed);
                context.NodeFormatter = new Notation3Formatter(g);
                this.GenerateOutput(context);

                output.Close();
            }
            finally
            {
                output.CloseQuietly();
            }
        }

        /// <summary>
        /// Generates the Notation 3 Syntax for the Graph
        /// </summary>
        private void GenerateOutput(CompressingTurtleWriterContext context)
        {
            //Create the Header
            //Base Directive
            // TODO Provide a way to set Base Uri for writers
            //if (context.Graph.BaseUri != null)
            //{
            //    context.Output.WriteLine("@base <" + context.UriFormatter.FormatUri(context.Graph.BaseUri) + ">.");
            //    context.Output.WriteLine();
            //}
            //Prefix Directives
            foreach (String prefix in context.Graph.Namespaces.Prefixes)
            {
                if (TurtleSpecsHelper.IsValidQName(prefix + ":"))
                {
                    if (!prefix.Equals(String.Empty))
                    {
                        context.Output.WriteLine("@prefix " + prefix + ": <" + context.UriFormatter.FormatUri(context.Graph.Namespaces.GetNamespaceUri(prefix)) + ">.");
                    }
                    else
                    {
                        context.Output.WriteLine("@prefix : <" + context.UriFormatter.FormatUri(context.Graph.Namespaces.GetNamespaceUri(String.Empty)) + ">.");
                    }
                }
            }
            context.Output.WriteLine();

            //Decide on the Write Mode to use
            bool hiSpeed = false;
            double subjNodes = context.Graph.Triples.Select(t => t.Subject).Distinct().Count();
            double triples = context.Graph.Count;
            if ((subjNodes / triples) > 0.75) hiSpeed = true;

            if (context.CompressionLevel == WriterCompressionLevel.None || (hiSpeed && context.HighSpeedModePermitted))
            {
                this.RaiseWarning("High Speed Write Mode in use - minimal syntax compression will be used");
                context.CompressionLevel = WriterCompressionLevel.Minimal;
                context.NodeFormatter = new UncompressedNotation3Formatter();

                foreach (Triple t in context.Graph.Triples)
                {
                    context.Output.WriteLine(this.GenerateTripleOutput(context, t));
                }
            }
            else
            {
                if (context.CompressionLevel >= WriterCompressionLevel.More)
                {
                    CompressionHelper.FindCollections(context);
                }

                //Get the Triples as a Sorted List
                List<Triple> ts = context.Graph.Triples.Where(t => !context.TriplesDone.Contains(t)).ToList();
                ts.Sort(new FullTripleComparer(new FastNodeComparer()));

                //Variables we need to track our writing
                INode lastSubj, lastPred;
                lastSubj = lastPred = null;
                int subjIndent = 0, predIndent = 0;
                String temp;

                for (int i = 0; i < ts.Count; i++)
                {
                    Triple t = ts[i];

                    if (lastSubj == null || !t.Subject.Equals(lastSubj))
                    {
                        //Terminate previous Triples
                        if (lastSubj != null) context.Output.WriteLine(".");

                        //Start a new set of Triples
                        temp = this.GenerateNodeOutput(context, t.Subject, QuadSegment.Subject, 0);
                        context.Output.Write(temp);
                        context.Output.Write(" ");
                        if (temp.Contains('\n'))
                        {
                            subjIndent = temp.Split('\n').Last().Length + 1;
                        }
                        else
                        {
                            subjIndent = temp.Length + 1;
                        }
                        lastSubj = t.Subject;

                        //Write the first Predicate
                        temp = this.GenerateNodeOutput(context, t.Predicate, QuadSegment.Predicate, subjIndent);
                        context.Output.Write(temp);
                        context.Output.Write(" ");
                        predIndent = temp.Length + 1;
                        lastPred = t.Predicate;
                    }
                    else if (lastPred == null || !t.Predicate.Equals(lastPred))
                    {
                        //Terminate previous Predicate Object list
                        context.Output.WriteLine(";");

                        if (context.PrettyPrint) context.Output.Write(new String(' ', subjIndent));

                        //Write the next Predicate
                        temp = this.GenerateNodeOutput(context, t.Predicate, QuadSegment.Predicate, subjIndent);
                        context.Output.Write(temp);
                        context.Output.Write(" ");
                        predIndent = temp.Length + 1;
                        lastPred = t.Predicate;
                    }
                    else
                    {
                        //Continue Object List
                        context.Output.WriteLine(",");

                        if (context.PrettyPrint) context.Output.Write(new String(' ', subjIndent + predIndent));
                    }

                    //Write the Object
                    context.Output.Write(this.GenerateNodeOutput(context, t.Object, QuadSegment.Object, subjIndent + predIndent));
                }

                //Terminate Triples
                if (ts.Count > 0) context.Output.WriteLine(".");

                return;
            }

        }

        /// <summary>
        /// Generates Output for Triples as a single "s p o." Triple
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="t">Triple to output</param>
        /// <returns></returns>
        /// <remarks>Used only in High Speed Write Mode</remarks>
        private String GenerateTripleOutput(CompressingTurtleWriterContext context, Triple t)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(this.GenerateNodeOutput(context, t.Subject, QuadSegment.Subject, 0));
            temp.Append(' ');
            temp.Append(this.GenerateNodeOutput(context, t.Predicate, QuadSegment.Predicate, 0));
            temp.Append(' ');
            temp.Append(this.GenerateNodeOutput(context, t.Object, QuadSegment.Object, 0));
            temp.Append('.');

            return temp.ToString();
        }

        /// <summary>
        /// Generates Output for Nodes in Notation 3 syntax
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="n">Node to generate output for</param>
        /// <param name="segment">Segment of the Triple being output</param>
        /// <param name="indent">Indent to use for pretty printing</param>
        /// <returns></returns>
        private String GenerateNodeOutput(CompressingTurtleWriterContext context, INode n, QuadSegment segment, int indent)
        {
            StringBuilder output = new StringBuilder();

            switch (n.NodeType)
            {
                case NodeType.Blank:
                    if (context.Collections.ContainsKey(n))
                    {
                        output.Append(this.GenerateCollectionOutput(context, context.Collections[n], indent));
                    }
                    else
                    {
                        return context.NodeFormatter.Format(n, segment);
                    }
                    break;

                case NodeType.GraphLiteral:
                    if (segment == QuadSegment.Predicate) throw new RdfOutputException(WriterErrorMessages.GraphLiteralPredicatesUnserializable("Notation 3"));

                    output.Append("{");

                    StringBuilder temp = new StringBuilder();
                    CompressingTurtleWriterContext subcontext = new CompressingTurtleWriterContext(n.SubGraph, new System.IO.StringWriter(temp));
                    subcontext.NodeFormatter = context.NodeFormatter;

                    //Write Triples 1 at a Time on a single line
                    foreach (Triple t in subcontext.Graph.Triples) 
                    {
                        output.Append(this.GenerateNodeOutput(subcontext, t.Subject, QuadSegment.Subject, 0));
                        output.Append(" ");
                        output.Append(this.GenerateNodeOutput(subcontext, t.Predicate, QuadSegment.Predicate, 0));
                        output.Append(" ");
                        output.Append(this.GenerateNodeOutput(subcontext, t.Object, QuadSegment.Object, 0));
                        output.Append(". ");
                    }

                    output.Append("}");
                    break;

                case NodeType.Literal:
                    if (segment == QuadSegment.Predicate) throw new RdfOutputException(WriterErrorMessages.LiteralPredicatesUnserializable("Notation 3"));
                    return context.NodeFormatter.Format(n, segment);

                case NodeType.Uri:
                    return context.NodeFormatter.Format(n, segment);

                case NodeType.Variable:
                    return context.NodeFormatter.Format(n, segment);

                default:
                    throw new RdfOutputException(WriterErrorMessages.UnknownNodeTypeUnserializable("Notation 3"));
            }

            return output.ToString();
        }

        /// <summary>
        /// Internal Helper method which converts a Collection into Notation 3 Syntax
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="c">Collection to convert</param>
        /// <param name="indent">Indent to use for pretty printing</param>
        /// <returns></returns>
        private String GenerateCollectionOutput(CompressingTurtleWriterContext context, OutputRdfCollection c, int indent)
        {
            StringBuilder output = new StringBuilder();
            bool first = true;

            if (!c.IsExplicit)
            {
                output.Append('(');

                while (c.Triples.Count > 0)
                {
                    if (context.PrettyPrint && !first) output.Append(new String(' ', indent));
                    first = false;
                    output.Append(this.GenerateNodeOutput(context, c.Triples.First().Object, QuadSegment.Object, indent));
                    c.Triples.RemoveAt(0);
                    if (c.Triples.Count > 0)
                    {
                        output.Append(' ');
                    }
                }

                output.Append(')');
            }
            else
            {
                if (c.Triples.Count == 0)
                {
                    //Empty Collection
                    //Can represent as a single Blank Node []
                    output.Append("[]");
                }
                else
                {
                    output.Append('[');

                    while (c.Triples.Count > 0)
                    {
                        if (context.PrettyPrint && !first) output.Append(new String(' ', indent));
                        first = false;
                        String temp = this.GenerateNodeOutput(context, c.Triples.First().Predicate, QuadSegment.Predicate, indent);
                        output.Append(temp);
                        output.Append(' ');
                        int addIndent;
                        if (temp.Contains('\n'))
                        {
                            addIndent = temp.Split('\n').Last().Length;
                        }
                        else
                        {
                            addIndent = temp.Length;
                        }
                        output.Append(this.GenerateNodeOutput(context, c.Triples.First().Object, QuadSegment.Object, indent + 2 + addIndent));
                        c.Triples.RemoveAt(0);

                        if (c.Triples.Count > 0)
                        {
                            output.AppendLine(" ; ");
                            output.Append(' ');
                        }
                    }

                    output.Append(']');
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Gets the String representation of the writer which is a description of the syntax it produces
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Notation 3";
        }
    }
}

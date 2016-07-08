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
using System.Linq;
using System.Reflection;
using VDS.RDF.Graphs;
using VDS.RDF.Namespaces;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Parsing.Handlers
{
    /// <summary>
    /// A RDF Handler which writes the handled Triples out to a <see cref="TextWriter">TextWriter</see> using a provided <see cref="ITripleFormatter">ITripleFormatter</see>
    /// </summary>
    public class WriteThroughHandler
        : BaseRdfHandler
    {
        private Type _formatterType;
        private ITripleFormatter _tripleFormatter;
        private IQuadFormatter _quadFormatter;
        private INamespaceFormatter _nsFormatter;
        private IBaseUriFormatter _uriFormatter;
        private TextWriter _writer;
        private bool _closeOnEnd = true;
        private INamespaceMapper _formattingMapper = new QNameOutputMapper();
        private int _written = 0;

        private const int FlushInterval = 50000;

        /// <summary>
        /// Creates a new Write-Through Handler
        /// </summary>
        /// <param name="formatter">Triple Formatter to use</param>
        /// <param name="writer">Text Writer to write to</param>
        /// <param name="closeOnEnd">Whether to close the writer at the end of RDF handling</param>
        public WriteThroughHandler(ITripleFormatter formatter, TextWriter writer, bool closeOnEnd)
        {
            if (writer == null) throw new ArgumentNullException("writer", "Cannot use a null TextWriter with the Write Through Handler");
            if (formatter != null)
            {
                this._tripleFormatter = formatter;
            }
            else
            {
                this._tripleFormatter = new NTriplesFormatter();
            }
            this.InitFormatters();
            this._writer = writer;
            this._closeOnEnd = closeOnEnd;
        }

        /// <summary>
        /// Creates a new Write-Through Handler
        /// </summary>
        /// <param name="formatter">Triple Formatter to use</param>
        /// <param name="writer">Text Writer to write to</param>
        public WriteThroughHandler(ITripleFormatter formatter, TextWriter writer)
            : this(formatter, writer, true) { }

        /// <summary>
        /// Creates a new Write-Through Handler
        /// </summary>
        /// <param name="formatterType">Type of the formatter to create</param>
        /// <param name="writer">Text Writer to write to</param>
        /// <param name="closeOnEnd">Whether to close the writer at the end of RDF handling</param>
        public WriteThroughHandler(Type formatterType, TextWriter writer, bool closeOnEnd)
        {
            if (writer == null) throw new ArgumentNullException("writer", "Cannot use a null TextWriter with the Write Through Handler");
            if (formatterType == null) throw new ArgumentNullException("formatterType", "Cannot use a null formatter type");
            this._formatterType = formatterType;
            this._writer = writer;
            this._closeOnEnd = closeOnEnd;
        }

        /// <summary>
        /// Creates a new Write-Through Handler
        /// </summary>
        /// <param name="formatterType">Type of the formatter to create</param>
        /// <param name="writer">Text Writer to write to</param>
        public WriteThroughHandler(Type formatterType, TextWriter writer)
            : this(formatterType, writer, true) { }

        private void InitFormatters()
        {
            this._quadFormatter = this._tripleFormatter as IQuadFormatter;
            this._nsFormatter = this._tripleFormatter as INamespaceFormatter;
            this._uriFormatter = this._tripleFormatter as IBaseUriFormatter;
        }

        /// <summary>
        /// Starts RDF Handling instantiating a Triple Formatter if necessary
        /// </summary>
        protected override void StartRdfInternal()
        {
            if (this._closeOnEnd && this._writer == null) throw new RdfParseException("Cannot use this WriteThroughHandler as an RDF Handler for parsing as you set closeOnEnd to true and you have already used this Handler and so the provided TextWriter was closed");

            if (this._formatterType != null)
            {
                this._tripleFormatter = null;
                this._formattingMapper = new QNameOutputMapper();

                //Instantiate a new Formatter
                ConstructorInfo[] cs = this._formatterType.GetTypeInfo().GetConstructors();
                Type qnameMapperType = typeof(QNameOutputMapper);
                Type nsMapperType = typeof(INamespaceMapper);
                foreach (ConstructorInfo c in cs.OrderByDescending(c => c.GetParameters().Count()))
                {
                    ParameterInfo[] ps = c.GetParameters();
                    try
                    {
                        if (ps.Length == 1)
                        {
                            if (ps[0].ParameterType.Equals(qnameMapperType))
                            {
                                this._tripleFormatter = Activator.CreateInstance(this._formatterType, new Object[] { this._formattingMapper }) as ITripleFormatter;
                            }
                            else if (ps[0].ParameterType.Equals(nsMapperType))
                            {
                                this._tripleFormatter = Activator.CreateInstance(this._formatterType, new Object[] { this._formattingMapper }) as ITripleFormatter;
                            }
                        }
                        else if (ps.Length == 0)
                        {
                            this._tripleFormatter = Activator.CreateInstance(this._formatterType) as ITripleFormatter;
                        }

                        if (this._tripleFormatter != null) break;
                    }
                    catch
                    {
                        //Suppress errors since we'll throw later if necessary
                    }
                }

                //If we get out here and the formatter is null then we throw an error
                if (this._tripleFormatter == null) throw new RdfParseException("Unable to instantiate a ITripleFormatter from the given Formatter Type " + this._formatterType.FullName);
                this.InitFormatters();
            }

            if (this._tripleFormatter is IGraphFormatter)
            {
                this._writer.WriteLine(((IGraphFormatter)this._tripleFormatter).FormatGraphHeader(this._formattingMapper));
            }
            this._written = 0;
        }

        /// <summary>
        /// Ends RDF Handling closing the <see cref="TextWriter">TextWriter</see> being used if the setting is enabled
        /// </summary>
        /// <param name="ok">Indicates whether parsing completed without error</param>
        protected override void EndRdfInternal(bool ok)
        {
            if (this._tripleFormatter is IGraphFormatter)
            {
                this._writer.WriteLine(((IGraphFormatter)this._tripleFormatter).FormatGraphFooter());
            }
            if (this._closeOnEnd)
            {
                this._writer = null;
            }
        }

        /// <summary>
        /// Handles Namespace Declarations passing them to the underlying formatter if applicable
        /// </summary>
        /// <param name="prefix">Namespace Prefix</param>
        /// <param name="namespaceUri">Namespace URI</param>
        /// <returns></returns>
        protected override bool HandleNamespaceInternal(string prefix, Uri namespaceUri)
        {
            if (this._formattingMapper != null)
            {
                this._formattingMapper.AddNamespace(prefix, namespaceUri);
            }

            if (this._nsFormatter != null)
            {
                this._writer.WriteLine(this._nsFormatter.FormatNamespace(prefix, namespaceUri));
            }

            return true;
        }

        /// <summary>
        /// Handles Base URI Declarations passing them to the underlying formatter if applicable
        /// </summary>
        /// <param name="baseUri">Base URI</param>
        /// <returns></returns>
        protected override bool HandleBaseUriInternal(Uri baseUri)
        {
            if (this._uriFormatter != null)
            {
                this._writer.WriteLine(this._uriFormatter.FormatBaseUri(baseUri));
            }

            return true;
        }

        /// <summary>
        /// Handles Triples by writing them using the underlying formatter
        /// </summary>
        /// <param name="t">Triple</param>
        /// <returns></returns>
        protected override bool HandleTripleInternal(Triple t)
        {
            this._written++;
            this._writer.WriteLine(this._tripleFormatter.Format(t));
            if (this._written >= FlushInterval)
            {
                this._written = 0;
                this._writer.Flush();
            }
            return true;
        }

        protected override bool HandleQuadInternal(Quad q)
        {
            if (this._quadFormatter != null)
            {
                this._written++;
                this._writer.WriteLine(this._quadFormatter.Format(q));
                if (this._written >= FlushInterval)
                {
                    this._written = 0;
                    this._writer.Flush();
                }
                return true;
            }
            else
            {
                throw new RdfParseException("Formatter " + this._tripleFormatter.GetType().Name + " does not support formatting Quads");
            }
        }

        /// <summary>
        /// Gets that the Handler accepts all Triples
        /// </summary>
        public override bool AcceptsAll
        {
            get 
            {
                return true;
            }
        }
    }
}

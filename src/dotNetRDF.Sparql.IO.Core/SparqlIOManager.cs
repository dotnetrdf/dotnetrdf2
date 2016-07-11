﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Writing;

namespace VDS.RDF
{
    /// <summary>
    /// Manager for SPARQL IO Operations
    /// </summary>
    public static class SparqlIOManager
    {
        #region Constants

        /// <summary>
        /// MIME Types for SPARQL Result Sets
        /// </summary>
        internal static string[] SparqlResults = {"application/sparql-results+xml", "application/sparql-results+json"};

        /// <summary>
        /// MIME Types for SPARQL Results XML
        /// </summary>
        internal static string[] SparqlResultsXml = {"application/sparql-results+xml"};

        /// <summary>
        /// MIME Types for SPARQL Results JSON
        /// </summary>
        internal static string[] SparqlResultsJson = {"application/sparql-results+json"};

        /// <summary>
        /// MIME Types for SPARQL Boolean Result
        /// </summary>
        internal static string[] SparqlResultsBoolean = {"text/boolean"};

        /// <summary>
        /// Default File Extension for SPARQL Queries
        /// </summary>
        public const String DefaultSparqlQueryExtension = "rq";

        /// <summary>
        /// Default File Extension for SPARQL Updates
        /// </summary>
        public const String DefaultSparqlUpdateExtension = "ru";

        /// <summary>
        /// Default File Extension for SPARQL XML Results Format
        /// </summary>
        public const String DefaultSparqlXmlExtension = "srx";

        /// <summary>
        /// Default File Extension for SPARQL JSON Results Format
        /// </summary>
        public const String DefaultSparqlJsonExtension = "srj";

        #endregion

        /// <summary>
        /// List of MIME Type Definition
        /// </summary>
        private static List<SparqlMimeTypeDefinition> _mimeTypes;

        /// <summary>
        /// Whether MIME Type Definitions have been initialised
        /// </summary>
        private static bool _init = false;

        private static readonly Object _initLock = new Object();

        /// <summary>
        /// Initialises the MIME Type definitions
        /// </summary>
        private static void Init()
        {
            lock (_initLock)
            {
                if (!_init)
                {
                    _mimeTypes = new List<SparqlMimeTypeDefinition>();

                    //Define SPARQL Results XML
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("SPARQL Results XML", IOManager.W3CFormatsNamespace + "SPARQL_Results_XML", SparqlResultsXml, new String[] {DefaultSparqlXmlExtension}, null, null, typeof (SparqlXmlParser), null, null, typeof (SparqlXmlWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GZipped SPARQL Results XML", SparqlResultsXml, new String[] {DefaultSparqlXmlExtension + "." + DefaultGZipExtension}, null, null, typeof (GZippedSparqlXmlParser), null, null, typeof (GZippedSparqlXmlWriter)));
#endif

                    //Define SPARQL Results JSON
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("SPARQL Results JSON", IOManager.W3CFormatsNamespace + "SPARQL_Results_JSON", SparqlResultsJson, new String[] {DefaultSparqlJsonExtension, DefaultJsonExtension}, null, null, typeof (SparqlJsonParser), null, null, typeof (SparqlJsonWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GZipped SPARQL Results JSON", SparqlResultsJson, new String[] {DefaultSparqlJsonExtension + "." + DefaultGZipExtension, DefaultJsonExtension + "." + DefaultGZipExtension}, null, null, typeof (GZippedSparqlJsonParser), null, null, typeof (GZippedSparqlJsonWriter)));
#endif

                    //Define SPARQL Boolean
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("SPARQL Boolean Result", SparqlResultsBoolean, Enumerable.Empty<String>(), null, null, typeof (SparqlBooleanParser), null, null, null));

                    //Define CSV
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("CSV", Csv, new String[] {DefaultCsvExtension}, null, null, typeof (SparqlCsvParser), typeof (CsvWriter), typeof (CsvStoreWriter), typeof (SparqlCsvWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GZipped SPARQL CSV", Csv, new String[] {DefaultCsvExtension + "." + DefaultGZipExtension}, null, null, typeof (GZippedSparqlCsvParser), null, null, typeof (GZippedSparqlCsvWriter)));
#endif

                    //Define TSV
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("TSV", Tsv, new String[] {DefaultTsvExtension}, null, null, typeof (SparqlTsvParser), typeof (TsvWriter), typeof (TsvStoreWriter), typeof (SparqlTsvWriter)));
#if !NO_COMPRESSION
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GZipped TSV", Tsv, new String[] {DefaultTsvExtension + "." + DefaultGZipExtension}, null, null, typeof (GZippedSparqlTsvParser), null, null, typeof (GZippedSparqlTsvWriter)));
#endif

                    //Define HTML
#if !NO_HTMLAGILITYPACK
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("HTML", IOManager.W3CFormatsNamespace + "RDFa", Html, new String[] {DefaultHtmlExtension, DefaultXHtmlExtension, ".htm"}, typeof (RdfAParser), null, null, typeof (HtmlWriter), null, typeof (SparqlHtmlWriter)));
#endif
#if !NO_COMPRESSION
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GZipped HTML", Html, new String[] {DefaultHtmlExtension + "." + DefaultGZipExtension, DefaultXHtmlExtension + "." + DefaultGZipExtension, ".htm." + DefaultGZipExtension}, typeof (GZippedRdfAParser), null, null, typeof (GZippedRdfAWriter), null, null));
#endif

                    //Define GraphViz DOT
                    _mimeTypes.Add(new SparqlMimeTypeDefinition("GraphViz DOT", new String[] {"text/vnd.graphviz"}, new String[] {".gv", ".dot"}, null, null, null, typeof (GraphVizWriter), null, null));

                    //Define SPARQL Query
                    SparqlMimeTypeDefinition qDef = new SparqlMimeTypeDefinition("SPARQL Query", new String[] {SparqlQuery}, new String[] {DefaultSparqlQueryExtension});
                    qDef.SetObjectParserType<SparqlQuery>(typeof (SparqlQueryParser));
                    _mimeTypes.Add(qDef);

                    //Define SPARQL Update
                    SparqlMimeTypeDefinition uDef = new SparqlMimeTypeDefinition("SPARQL Update", new String[] {SparqlUpdate}, new String[] {DefaultSparqlUpdateExtension});
                    uDef.SetObjectParserType<SparqlUpdateCommandSet>(typeof (SparqlUpdateParser));
                    _mimeTypes.Add(uDef);

                    _init = true;
                }
            }
        }

        public IEnumerable<SparqlMimeTypeDefinition> Definitions
        {
            get
            {
                if (!_init) Init();

                return _mimeTypes;
            }
        }

        /// <summary>
        /// Registers a parser as the default SPARQL Rsults Parser for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="parser">SPARQL Results Parser</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterParser(ISparqlResultsReader parser, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a parser without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<SparqlMimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (SparqlMimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.SparqlResultsParserType = parser.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                SparqlMimeTypeDefinition newDef = new SparqlMimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.SparqlResultsParserType = parser.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Registers a writer as the default SPARQL Results Writer for all the given MIME types and updates relevant definitions to include the MIME types and file extensions
        /// </summary>
        /// <param name="writer">SPARQL Results Writer</param>
        /// <param name="mimeTypes">MIME Types</param>
        /// <param name="fileExtensions">File Extensions</param>
        public static void RegisterWriter(ISparqlResultsWriter writer, IEnumerable<String> mimeTypes, IEnumerable<String> fileExtensions)
        {
            if (!_init) Init();

            if (!mimeTypes.Any()) throw new RdfException("Cannot register a writer without specifying at least 1 MIME Type");

            //Get any existing defintions that are to be altered
            IEnumerable<SparqlMimeTypeDefinition> existing = GetDefinitions(mimeTypes);
            foreach (SparqlMimeTypeDefinition def in existing)
            {
                foreach (String type in mimeTypes)
                {
                    def.AddMimeType(type);
                }
                foreach (String ext in fileExtensions)
                {
                    def.AddFileExtension(ext);
                }
                def.SparqlResultsWriterType = writer.GetType();
            }

            //Create any new defintions
            IEnumerable<String> newTypes = mimeTypes.Where(t => !GetDefinitions(t).Any());
            if (newTypes.Any())
            {
                SparqlMimeTypeDefinition newDef = new SparqlMimeTypeDefinition(String.Empty, newTypes, fileExtensions);
                newDef.SparqlResultsWriterType = writer.GetType();
                AddDefinition(newDef);
            }
        }

        /// <summary>
        /// Selects a SPARQL Parser based on the MIME types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <param name="allowPlainTextResults">Whether to allow for plain text results</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParser(IEnumerable<String> ctypes, bool allowPlainTextResults)
        {
            foreach (SparqlMimeTypeDefinition definition in IOManager.GetDefinitions(ctypes))
            {
                if (definition.CanParseSparqlResults)
                {
                    ISparqlResultsReader parser = definition.GetSparqlResultsParser();
                    IOManager.ApplyParserOptions(parser);
                    return parser;
                }
            }

            if (allowPlainTextResults && (ctypes.Contains("text/plain") || ctypes.Contains("text/boolean")))
            {
                ISparqlResultsReader bParser = new SparqlBooleanParser();
                IOManager.ApplyParserOptions(bParser);
                return bParser;
            }
            else
            {
                String types = (ctypes == null) ? String.Empty : String.Join(",", ctypes.ToArray());
                throw new RdfParserSelectionException("The Library does not contain any Parsers for SPARQL Results in any of the following MIME Types: " + types);
            }
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsReader">ISparqlResultsReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParser(String contentType)
        {
            return IOManager.GetSparqlParser(contentType.AsEnumerable(), false);
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsReader">ISparqlResultsReader</see> based on the HTTP Content-Type header from a HTTP Response
        /// </summary>
        /// <param name="contentType">Value of the HTTP Content-Type Header</param>
        /// <param name="allowPlainTextResults">Whether you allow Sparql Boolean results in text/plain format (Boolean results in text/boolean are handled properly but text/plain results can be conflated with CONSTRUCT/DESCRIBE results in NTriples format)</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParser(String contentType, bool allowPlainTextResults)
        {
            return IOManager.GetSparqlParser(contentType.AsEnumerable(), allowPlainTextResults);
        }

        /// <summary>
        /// Selects a <see cref="ISparqlResultsReader"/> based on the file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        public static ISparqlResultsReader GetSparqlParserByFileExtension(String fileExt)
        {
            if (fileExt == null) throw new ArgumentNullException("fileExt", "File Extension cannot be null");

            foreach (SparqlMimeTypeDefinition def in IOManager.GetDefinitionsByFileExtension(fileExt))
            {
                if (def.CanParseSparqlResults)
                {
                    ISparqlResultsReader parser = def.GetSparqlResultsParser();
                    IOManager.ApplyParserOptions(parser);
                    return parser;
                }
            }

            throw new RdfParserSelectionException("The Library does not contain a Parser for SPARQL Results associated with the file extension '" + fileExt + "'");
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the given MIME Types
        /// </summary>
        /// <param name="ctypes">MIME Types</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// Global options pertaining to writers will be applied to the selected writer
        /// </para>
        /// </remarks>
        public static ISparqlResultsWriter GetSparqlWriter(IEnumerable<String> ctypes)
        {
            String temp;
            return GetSparqlWriter(ctypes, out temp);
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="ctypes">String array of accepted Content Types</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// Global options pertaining to writers will be applied to the selected writer
        /// </para>
        /// </remarks>
        public static ISparqlResultsWriter GetSparqlWriter(IEnumerable<String> ctypes, out String contentType)
        {
            foreach (SparqlMimeTypeDefinition definition in IOManager.GetDefinitions(ctypes))
            {
                if (definition.CanWriteSparqlResults)
                {
                    contentType = definition.CanonicalMimeType;
                    ISparqlResultsWriter writer = definition.GetSparqlResultsWriter();
                    IOManager.ApplyWriterOptions(writer);
                    return writer;
                }
            }

            //Default to SPARQL XML Output
            contentType = IOManager.SparqlResultsXml[0];
            ISparqlResultsWriter defaultWriter = new SparqlXmlWriter();
            IOManager.ApplyWriterOptions(defaultWriter);
            return defaultWriter;
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <param name="contentType">The Content Type header that should be sent in the Response to the Request</param>
        /// <returns>A Writer for a Content Type the client accepts and the Content Type that should be sent to the client</returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// Global options pertaining to writers will be applied to the selected writer
        /// </para>
        /// </remarks>
        public static ISparqlResultsWriter GetSparqlWriter(String acceptHeader, out String contentType)
        {
            String[] ctypes;

            //Parse Accept Header into a String Array
            acceptHeader = acceptHeader.Trim();
            if (acceptHeader.Contains(","))
            {
                ctypes = acceptHeader.Split(',');
            }
            else
            {
                ctypes = new String[] {acceptHeader};
            }

            return GetSparqlWriter(ctypes, out contentType);
        }

        /// <summary>
        /// Selects an appropriate <see cref="ISparqlResultsWriter">ISparqlResultsWriter</see> based on the HTTP Accept header form a HTTP Request
        /// </summary>
        /// <param name="acceptHeader">Value of the HTTP Accept Header</param>
        /// <returns>A Writer for a Content Type the client accepts</returns>
        /// <remarks>
        /// <para>
        /// This method does not take account of any quality/charset preference parameters included in the Accept Header
        /// </para>
        /// <para>
        /// Global options pertaining to writers will be applied to the selected writer
        /// </para>
        /// </remarks>
        public static ISparqlResultsWriter GetSparqlWriter(String acceptHeader)
        {
            String temp;
            return GetSparqlWriter(acceptHeader, out temp);
        }

        /// <summary>
        /// Selects a <see cref="ISparqlResultsWriter"/> based on a file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <returns></returns>
        public static ISparqlResultsWriter GetSparqlWriterByFileExtension(String fileExt)
        {
            String temp;
            return IOManager.GetSparqlWriterByFileExtension(fileExt, out temp);
        }

        /// <summary>
        /// Selects a <see cref="ISparqlResultsWriter"/> based on a file extension
        /// </summary>
        /// <param name="fileExt">File Extension</param>
        /// <param name="contentType">Content Type of the selected writer</param>
        /// <returns></returns>
        public static ISparqlResultsWriter GetSparqlWriterByFileExtension(String fileExt, out String contentType)
        {
            if (fileExt == null) throw new ArgumentNullException("fileExt", "File Extension cannot be null");

            foreach (SparqlMimeTypeDefinition def in IOManager.GetDefinitionsByFileExtension(fileExt))
            {
                if (def.CanWriteSparqlResults)
                {
                    ISparqlResultsWriter writer = def.GetSparqlResultsWriter();
                    IOManager.ApplyWriterOptions(writer);
                    contentType = def.CanonicalMimeType;
                    return writer;
                }
            }

            throw new RdfWriterSelectionException("Unable to select a SPARQL Results Writer, no writers are associated with the file extension '" + fileExt + "'");
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for querying Sparql Endpoints where the response will be a SPARQL Result Set format
        /// </summary>
        /// <returns></returns>
        public static String HttpSparqlAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (SparqlMimeTypeDefinition definition in SparqlIOManager.Definitions)
                {
                    if (definition.CanParseSparqlResults)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);

                return output.ToString();
            }
        }

        /// <summary>
        /// Builds the String for the HTTP Accept Header that should be used for making HTTP Requests where the returned data may be RDF or a SPARQL Result Set
        /// </summary>
        /// <returns></returns>
        public static String HttpRdfOrSparqlAcceptHeader
        {
            get
            {
                if (!_init) Init();

                StringBuilder output = new StringBuilder();

                foreach (MimeTypeDefinition definition in IOManager.Definitions)
                {
                    if (definition.CanParseRdf || definition.CanParseSparqlResults)
                    {
                        output.Append(String.Join(",", definition.MimeTypes.ToArray()));
                        output.Append(',');
                    }
                }
                foreach (SparqlMimeTypeDefinition definition in SparqlIOManager.Definitions)
                {
                }
                if (output[output.Length - 1] == ',') output.Remove(output.Length - 1, 1);
                output.Append(",*/*;q=0.5");

                return output.ToString();
            }
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for SPARQL Results
        /// </summary>
        public static IEnumerable<String> SupportedSparqlMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in IOManager.Definitions
                        where definition.CanParseSparqlResults
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        /// <summary>
        /// Gets the Enumeration of supported MIME Types for RDF Graphs or SPARQL Results
        /// </summary>
        public static IEnumerable<String> SupportedRdfOrSparqlMimeTypes
        {
            get
            {
                if (!_init) Init();

                return (from definition in IOManager.Definitions
                        where definition.CanParseRdf || definition.CanParseSparqlResults
                        from mimeType in definition.MimeTypes
                        select mimeType);
            }
        }

        /// <summary>
        /// Generates a Filename Filter that can be used with any .Net application and includes a user dictated subset of the formats that dotNetRDF is aware of
        /// </summary>
        /// <param name="rdf">Allow RDF formats (e.g. Turtle)</param>
        /// <param name="sparqlResults">Allow SPARQL Results formats (e.g. SPARQL Results XML)</param>
        /// <param name="sparqlQuery">Allow SPARQL Query (i.e. .rq files)</param>
        /// <param name="sparqlUpdate">Allow SPARQL Update (i.e. .ru files)</param>
        /// <param name="allFiles">Allow All Files (i.e. */*)</param>
        /// <returns></returns>
        public static String GetFilenameFilter(bool rdf, bool rdfDatasets, bool sparqlResults, bool sparqlQuery, bool sparqlUpdate, bool allFiles)
        {
            if (!_init) Init();

            String filter = String.Empty;
            List<String> exts = new List<string>();

            foreach (MimeTypeDefinition def in IOManager.Definitions)
            {
                if ((rdf && (def.CanParseRdf || def.CanWriteRdf))
                    || (rdfDatasets && (def.CanParseRdfDatasets || def.CanWriteRdfDatasets))
                    || (sparqlResults && (def.CanParseSparqlResults || def.CanWriteSparqlResults))
                    || (sparqlQuery && def.CanParseObject<SparqlQuery>())
                    || (sparqlUpdate && def.CanParseObject<SparqlUpdateCommandSet>()))
                {
                    exts.AddRange(def.FileExtensions);
                    filter += def.SyntaxName + " Files|*." + String.Join(";*.", def.FileExtensions.ToArray()) + "|";
                }
            }
            //Add an All Supported Formats option as first option
            filter = "All Supported Files|*." + String.Join(";*.", exts.ToArray()) + "|" + filter;

            if (allFiles)
            {
                filter += "All Files|*.*";
            }
            else
            {
                filter = filter.Substring(0, filter.Length - 1);
            }

            return filter;
        }
    }
}
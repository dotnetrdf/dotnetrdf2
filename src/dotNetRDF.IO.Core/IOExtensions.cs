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
using System.IO;
using System.Linq;
using System.Text;
using VDS.RDF.Graphs;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;

namespace VDS.RDF
{
    /// <summary>
    /// Static class containing extension methods related to IO operations
    /// </summary>
    public static class IOExtensions
    {
        /// <summary>
        /// Ensures a parser profile is available, returns default empty profile if null is specified
        /// </summary>
        /// <param name="profile">Parser Profile (possibly null)</param>
        /// <returns>Non-null parser profile</returns>
        public static IParserProfile EnsureParserProfile(this IParserProfile profile)
        {
            return profile ?? new ParserProfile();
        }

        /// <summary>
        /// Checks whether a given <see cref="TextReader"/> has a desired character encoding and invokes a callback if it is not
        /// </summary>
        /// <param name="input">Text Reader</param>
        /// <param name="desired">Desired encoding</param>
        /// <param name="callback">Callback method</param>
        public static void CheckEncoding(this TextReader input, Encoding desired, Action<String> callback)
        {
            // We can't detect encoding on non-StreamReader instances without touching the stream which may not be safe to do
            if (!(input is StreamReader)) return;
            if (callback == null) return;

            // Issue a Warning if the Encoding of the Stream is not UTF-8
            StreamReader streamInput = (StreamReader)input;
            if (!streamInput.CurrentEncoding.Equals(desired))
            {
                callback(string.Format("Expected Input Stream to be encoded as UTF-8 but got a Stream encoded as {0} - Please be aware that parsing errors may occur as a result", streamInput.CurrentEncoding.GetEncodingName()));
            }
        }

        /// <summary>
        /// Gets the encoding name
        /// </summary>
        /// <param name="encoding">Encoding</param>
        /// <returns>Encoding name</returns>
        /// <remarks>
        /// Required because in their infinite wisdom the people who defined the Silverlight profile omitted the <see cref="Encoding.EncodingName"/> property from the <see cref="Encoding"/> class
        /// </remarks>
        public static String GetEncodingName(this Encoding encoding)
        {
#if !SILVERLIGHT
            return encoding.EncodingName;
#else
            return encoding.GetType().Name;
#endif
        }

        /// <summary>
        /// Method for Loading a Graph from some Concrete RDF Syntax via some arbitrary Input
        /// </summary>
        /// <param name="parser">RDF parser to use</param>
        /// <param name="g">Graph to load RDF into</param>
        /// <param name="input">The reader to read input from</param>
        /// <exception cref="RdfException">Thrown if the Parser tries to output something that is invalid RDF</exception>
        /// <exception cref="Parsing.RdfParseException">Thrown if the Parser cannot Parse the Input</exception>
        /// <exception cref="System.IO.IOException">Thrown if the Parser encounters an IO Error while trying to access/parse the Stream</exception>
        public static void Load(this IRdfReader parser, IGraph g, TextReader input)
        {
            if (ReferenceEquals(parser, null)) throw new ArgumentNullException("parser");
            parser.Load(new GraphHandler(g), input);
        }

        public static void Load(this IRdfReader parser, IGraphStore graphStore, TextReader input)
        {
            if (ReferenceEquals(parser, null)) throw new ArgumentNullException("parser");
            parser.Load(new GraphStoreHandler(graphStore), input);
        }

        public static void Load(this IRdfReader parser, IRdfHandler handler, TextReader input)
        {
            parser.Load(handler, input, new ParserProfile());
        }

#if !NO_FILE
        /// <summary>
        /// Method for Loading a Graph from some Concrete RDF Syntax from a given File
        /// </summary>
        /// <param name="parser">RDF parser to use</param>
        /// <param name="g">Graph to load RDF into</param>
        /// <param name="filename">The Filename of the File to read from</param>
        /// <exception cref="RdfException">Thrown if the Parser tries to output something that is invalid RDF</exception>
        /// <exception cref="Parsing.RdfParseException">Thrown if the Parser cannot Parse the Input</exception>
        /// <exception cref="System.IO.IOException">Thrown if the Parser encounters an IO Error while trying to access/parse the File</exception>
        public static void Load(this IRdfReader parser, IGraph g, String filename)
        {
            if (ReferenceEquals(parser, null)) throw new ArgumentNullException("parser");
            // TODO This should look up the appropriate encoding
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                parser.Load(new GraphHandler(g), new StreamReader(stream));
            }
        }

        public static void Load(this IRdfReader parser, IGraphStore graphStore, String filename)
        {
            if (ReferenceEquals(parser, null)) throw new ArgumentNullException("parser");
            // TODO This should look up appropriate encoding
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                parser.Load(new GraphStoreHandler(graphStore), new StreamReader(stream));
            }
        }

        /// <summary>
        /// Method for Saving a Graph to a Concrete RDF Syntax in a file based format
        /// </summary>
        /// <param name="writer">RDF writer to use</param>
        /// <param name="g">The Graph to Save</param>
        /// <param name="filename">The filename to save the Graph in</param>
        /// <exception cref="RdfException">Thrown if the RDF in the Graph is not representable by the Writer</exception>
        /// <exception cref="IOException">Thrown if the Writer is unable to write to the File</exception>
        public static void Save(this IRdfWriter writer, IGraph g, String filename)
        {
            if (ReferenceEquals(writer, null)) throw new ArgumentNullException("writer");
            // TODO This should lookup the appropriate encoding
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                writer.Save(g, new StreamWriter(stream));
                stream.Flush();
            }
        }
#endif

        /// <summary>
        /// Loads RDF data from a file into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="file">File to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// <para>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="FileLoader">FileLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </para>
        /// <para>
        /// <strong>Note:</strong> FileLoader will assign the Graph a file URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// <para>
        /// If a File URI is assigned it will always be an absolute URI for the file
        /// </para>
        /// </remarks>
        public static void LoadFromFile(this IGraph g, String file, IRdfReader parser)
        {
            FileLoader.Load(g, file, parser);
        }

        /// <summary>
        /// Loads RDF data from a file into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="file">File to load from</param>
        /// <remarks>
        /// <para>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="FileLoader">FileLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </para>
        /// <para>
        /// <strong>Note:</strong> FileLoader will assign the Graph a file URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// <para>
        /// If a File URI is assigned it will always be an absolute URI for the file
        /// </para>
        /// </remarks>
        public static void LoadFromFile(this IGraph g, String file)
        {
            FileLoader.Load(g, file);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Loads RDF data from a URI into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="u">URI to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// <para>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="UriLoader">UriLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </para>
        /// <para>
        /// <strong>Note:</strong> UriLoader will assign the Graph the source URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// </remarks>
        public static void LoadFromUri(this IGraph g, Uri u, IRdfReader parser)
        {
            UriLoader.Load(g, u, parser);
        }

        /// <summary>
        /// Loads RDF data from a URI into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="u">URI to load from</param>
        /// <remarks>
        /// <para>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="UriLoader">UriLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </para>
        /// <para>
        /// <strong>Note:</strong> UriLoader will assign the Graph the source URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// </remarks>
        public static void LoadFromUri(this IGraph g, Uri u)
        {
            UriLoader.Load(g, u);
        }

#endif

        // TODO: Add LoadFromUri extensions that do the loading asychronously for use on Silverlight/Windows Phone 7

        /// <summary>
        /// Loads RDF data from a String into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="data">Data to load</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Parse()</strong> methods from the <see cref="StringParser">StringParser</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromString(this IGraph g, String data, IRdfReader parser)
        {
            StringParser.Parse(g, data, parser);
        }

        /// <summary>
        /// Loads RDF data from a String into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="data">Data to load</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Parse()</strong> methods from the <see cref="StringParser">StringParser</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromString(this IGraph g, String data)
        {
            StringParser.Parse(g, data);
        }

        /// <summary>
        /// Loads RDF data from an Embedded Resource into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="resource">Assembly qualified name of the resource to load from</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="EmbeddedResourceLoader">EmbeddedResourceLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromEmbeddedResource(this IGraph g, String resource)
        {
            EmbeddedResourceLoader.Load(g, resource);
        }

        /// <summary>
        /// Loads RDF data from an Embedded Resource into a Graph
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="resource">Assembly qualified name of the resource to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="EmbeddedResourceLoader">EmbeddedResourceLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromEmbeddedResource(this IGraph g, String resource, IRdfReader parser)
        {
            EmbeddedResourceLoader.Load(g, resource, parser);
        }

        /// <summary>
        /// Saves a Graph to a File
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="file">File to save to</param>
        /// <param name="writer">Writer to use</param>
        public static void SaveToFile(this IGraph g, String file, IRdfWriter writer)
        {
            if (writer == null)
            {
                g.SaveToFile(file);
            }
            else
            {
                writer.Save(g, file);
            }
        }

        /// <summary>
        /// Saves a Graph to a File
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="file">File to save to</param>
        public static void SaveToFile(this IGraph g, String file)
        {
            IRdfWriter writer = IOManager.GetWriterByFileExtension(IOManager.GetTrueFileExtension(file));
            writer.Save(g, file);
        }

        /// <summary>
        /// Loads an RDF dataset from a file into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="file">File to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="FileLoader">FileLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromFile(this IGraphStore store, String file, IRdfReader parser)
        {
            FileLoader.Load(store, file, parser);
        }

        /// <summary>
        /// Loads an RDF dataset from a file into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="file">File to load from</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="FileLoader">FileLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromFile(this IGraphStore store, String file)
        {
            FileLoader.Load(store, file);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Loads an RDF dataset from a URI into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="u">URI to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="UriLoader">UriLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromUri(this IGraphStore store, Uri u, IRdfReader parser)
        {
            UriLoader.Load(store, u, parser);
        }

        /// <summary>
        /// Loads an RDF dataset from a URI into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="u">URI to load from</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="UriLoader">UriLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromUri(this IGraphStore store, Uri u)
        {
            UriLoader.Load(store, u);
        }

#endif

        /// <summary>
        /// Loads an RDF dataset from a String into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="data">Data to load</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>ParseDataset()</strong> methods from the <see cref="StringParser">StringParser</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromString(this IGraphStore store, String data, IRdfReader parser)
        {
            StringParser.ParseDataset(store, data, parser);
        }

        /// <summary>
        /// Loads an RDF dataset from a String into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="data">Data to load</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>ParseDataset()</strong> methods from the <see cref="StringParser">StringParser</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromString(this IGraphStore store, String data)
        {
            StringParser.ParseDataset(store, data);
        }

        /// <summary>
        /// Loads an RDF dataset from an Embedded Resource into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="resource">Assembly Qualified Name of the Embedded Resource to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="EmbeddedResourceLoader">EmbeddedResourceLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromEmbeddedResource(this IGraphStore store, String resource, IRdfReader parser)
        {
            EmbeddedResourceLoader.Load(store, resource, parser);
        }

        /// <summary>
        /// Loads an RDF dataset from an Embedded Resource into a Triple Store
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="resource">Assembly Qualified Name of the Embedded Resource to load from</param>
        /// <remarks>
        /// This is just a shortcut to using the static <strong>Load()</strong> methods from the <see cref="EmbeddedResourceLoader">EmbeddedResourceLoader</see> class located in the <see cref="VDS.RDF.Parsing">Parsing</see> namespace
        /// </remarks>
        public static void LoadFromEmbeddedResource(this IGraphStore store, String resource)
        {
            EmbeddedResourceLoader.Load(store, resource);
        }

#if !NO_FILE

        /// <summary>
        /// Saves a Triple Store to a file
        /// </summary>
        /// <param name="store">Triple Store to save</param>
        /// <param name="file">File to save to</param>
        /// <param name="writer">Writer to use</param>
        public static void SaveToFile(this IGraphStore store, String file, IRdfWriter writer)
        {
            if (writer == null)
            {
                store.SaveToFile(file);
            }
            else
            {
                // TODO Should create stream with appropriate encoding
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    writer.Save(store, new StreamWriter(stream));
                    stream.Flush();
                }
            }
        }

        /// <summary>
        /// Saves a Triple Store to a file
        /// </summary>
        /// <param name="store">Triple Store to save</param>
        /// <param name="file">File to save to</param>
        public static void SaveToFile(this IGraphStore store, String file)
        {
            IRdfWriter writer = IOManager.GetWriterByFileExtension(IOManager.GetTrueFileExtension(file));
            // TODO Should create stream with appropriate encoding
            using (var stream = new FileStream(file, FileMode.Create))
            {
                writer.Save(store, new StreamWriter(new FileStream(file, FileMode.Create)));
                stream.Flush();
            }
        }

#endif
    }
}

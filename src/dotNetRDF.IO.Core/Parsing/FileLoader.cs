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
using System.Linq;
using System.IO;
using VDS.RDF.Graphs;
using VDS.RDF.Parsing.Handlers;

namespace VDS.RDF.Parsing
{
#if !NO_FILE
    /// <summary>
    /// Static Helper Class for loading RDF Files into Graphs/Triple Stores
    /// </summary>
    public static class FileLoader
    {
        /// <summary>
        /// Loads the contents of the given File into a Graph providing the RDF format can be determined
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="filename">File to load from</param>
        /// <remarks>
        /// <para>
        /// The <see cref="FileLoader">FileLoader</see> first attempts to select a RDF Parser by examining the file extension to select the most likely MIME type for the file.  This assumes that the file extension corresponds to one of the recognized file extensions for a RDF format the library supports.  If this suceeds then a parser is chosen and will be used to attempt to parse the input.
        /// </para>
        /// <para>
        /// Should this fail then the contents of the file will be read into a String, the <see cref="StringParser">StringParser</see> is then used to attempt to parse it.  The <see cref="StringParser">StringParser</see> uses some simple rules to guess which format the input is likely to be and chooses a parser based on it's guess.
        /// </para>
        /// <para>
        /// <strong>Note:</strong> FileLoader will assign the Graph a file URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// <para>
        /// If a File URI is assigned it will always be an absolute URI for the file
        /// </para>
        /// </remarks>
        public static void Load(IGraph g, String filename)
        {
            FileLoader.Load(g, filename, null);
        }

        /// <summary>
        /// Loads the contents of the given File into a Graph using the given RDF Parser
        /// </summary>
        /// <param name="g">Graph to load into</param>
        /// <param name="filename">File to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// <para>
        /// <strong>Note:</strong> FileLoader will assign the Graph a file URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// <para>
        /// If a File URI is assigned it will always be an absolute URI for the file
        /// </para>
        /// </remarks>
        public static void Load(IGraph g, String filename, IRdfReader parser)
        {
            if (g == null) throw new RdfParseException("Cannot read RDF into a null Graph");
            if (filename == null) throw new RdfParseException("Cannot read RDF from a null File");
            
            //Unescape the filename in case it originated from a File URI
            filename = Uri.UnescapeDataString(filename);

            if (!File.Exists(filename))
            {
#if SILVERLIGHT
                throw new FileNotFoundException("Cannot read RDF from the File '" + filename + "' since it doesn't exist");
#else
                throw new FileNotFoundException("Cannot read RDF from the File '" + filename + "' since it doesn't exist", filename);
#endif
            }

            FileLoader.Load(new GraphHandler(g), filename, parser);
        }

        /// <summary>
        /// Loads the contents of the given File using a RDF Handler providing the RDF format can be determined
        /// </summary>
        /// <param name="handler">RDF Handler to use</param>
        /// <param name="filename">File to load from</param>
        /// <remarks>
        /// <para>
        /// The <see cref="FileLoader">FileLoader</see> first attempts to select a RDF Parser by examining the file extension to select the most likely MIME type for the file.  This assumes that the file extension corresponds to one of the recognized file extensions for a RDF format the library supports.  If this suceeds then a parser is chosen and will be used to attempt to parse the input.
        /// </para>
        /// <para>
        /// Should this fail then the contents of the file will be read into a String, the <see cref="StringParser">StringParser</see> is then used to attempt to parse it.  The <see cref="StringParser">StringParser</see> uses some simple rules to guess which format the input is likely to be and chooses a parser based on it's guess.
        /// </para>
        /// </remarks>
        public static void Load(IRdfHandler handler, String filename)
        {
            FileLoader.Load(handler, filename, (IRdfReader)null);
        }

        /// <summary>
        /// Loads the contents of the given File using a RDF Handler using the given RDF Parser
        /// </summary>
        /// <param name="handler">RDF Handler to use</param>
        /// <param name="filename">File to load from</param>
        /// <param name="parser">Parser to use</param>
        /// <remarks>
        /// <para>
        /// <strong>Note:</strong> FileLoader will assign the Graph a file URI as it's Base URI unless the Graph already has a Base URI or is non-empty prior to attempting parsing.  Note that any Base URI specified in the RDF contained in the file will override this initial Base URI.  In some cases this may lead to invalid RDF being accepted and generating strange relative URIs, if you encounter this either set a Base URI prior to calling this method or create an instance of the relevant parser and invoke it directly.
        /// </para>
        /// <para>
        /// If a File URI is assigned it will always be an absolute URI for the file
        /// </para>
        /// </remarks>
        public static void Load(IRdfHandler handler, String filename, IRdfReader parser)
        {
            if (handler == null) throw new RdfParseException("Cannot read RDF using a null RDF Handler");
            if (filename == null) throw new RdfParseException("Cannot read RDF from a null File");

            //Try to get a Parser from the File Extension if one isn't explicitly specified
            if (parser == null)
            {
                try
                {
                    String ext = IOManager.GetTrueFileExtension(filename);
                    parser = IOManager.GetParserByFileExtension(ext);
                }
                catch (RdfParserSelectionException)
                {
                    //If error then we couldn't determine MIME Type from the File Extension
                    RaiseWarning("Unable to select a parser by determining MIME Type from the File Extension");
                }
            }

            if (parser == null)
            {
                //Unable to determine format from File Extension
                //Read file in locally and use the StringParser to select a parser
                RaiseWarning("Attempting to select parser based on analysis of the data file, this requires loading the file into memory");
                StreamReader reader = new StreamReader(filename);
                String data = reader.ReadToEnd();
                parser = StringParser.GetParser(data);
                RaiseWarning("Used the StringParser to guess the parser to use - it guessed " + parser.GetType().Name);
                parser.Warning += RaiseWarning;
                parser.Load(handler, new StringReader(data));
            }
            else
            {
                //Parser was selected based on File Extension or one was explicitly specified
                parser.Warning += RaiseWarning;
                parser.Load(handler, new StreamReader(filename));
            }
        }

        /// <summary>
        /// Loads the contents of the given File into a Triple Store providing the RDF dataset format can be determined
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="filename">File to load from</param>
        /// <param name="parser">Parser to use to parse the given file</param>
        /// <remarks>
        /// <para>
        /// If the <paramref name="parser"/> parameter is set to null then the <see cref="FileLoader">FileLoader</see> attempts to select a Store Parser by examining the file extension to select the most likely MIME type for the file.  This assume that the file extension corresponds to one of the recognized file extensions for a RDF dataset format the library supports.  If this suceeds then a parser is chosen and used to parse the input file.
        /// </para>
        /// </remarks>
        public static void Load(IGraphStore store, String filename, IRdfReader parser)
        {
            if (store == null) throw new RdfParseException("Cannot read a RDF Dataset into a null Store");
            FileLoader.Load(new GraphStoreHandler(store), filename, parser);
        }

        /// <summary>
        /// Loads the contents of the given File into a Triple Store providing the RDF dataset format can be determined
        /// </summary>
        /// <param name="store">Triple Store to load into</param>
        /// <param name="filename">File to load from</param>
        /// <remarks>
        /// <para>
        /// The <see cref="FileLoader">FileLoader</see> attempts to select a Store Parser by examining the file extension to select the most likely MIME type for the file.  This assume that the file extension corresponds to one of the recognized file extensions for a RDF dataset format the library supports.  If this suceeds then a parser is chosen and used to parse the input file.
        /// </para>
        /// </remarks>
        public static void Load(IGraphStore store, String filename)
        {
            FileLoader.Load(store, filename, null);
        }

        /// <summary>
        /// Raises warning messages
        /// </summary>
        /// <param name="message">Warning Message</param>
        static void RaiseWarning(String message)
        {
            RdfReaderWarning d = Warning;
            if (d != null)
            {
                d(message);
            }
        }

        /// <summary>
        /// Event which is raised when the parser invoked by the FileLoader detects a non-fatal issue with the RDF syntax
        /// </summary>
        public static event RdfReaderWarning Warning;
    }
#endif

}

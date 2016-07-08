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
using Xunit;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Tokens;
using VDS.RDF.Writing;
using FluentAssertions;

namespace VDS.RDF
{
    public class MimeTypesTests : IDisposable
    {
        public MimeTypesTests()
        {
            IOManager.ResetDefinitions();
        }

        public void Dispose()
        {
            IOManager.ResetDefinitions();
        }

        [Fact]
        public void MimeTypesQuality()
        {
            foreach (MimeTypeDefinition definition in IOManager.Definitions)
            {
                String httpHeader = definition.ToHttpHeader();
                if (definition.MimeTypes.Count() > 1)
                {
                    httpHeader.Should().Contain(",", "Expected header to contain multiple types");
                }
                if (definition.Quality < 1d)
                {
                    String quality = definition.Quality.ToString("g3");
                    httpHeader.Should().Contain("q=" + quality, "Expected q=" + quality + " in header");
                }
                foreach (String mimeType in definition.MimeTypes)
                {
                    httpHeader.Should().Contain(mimeType, "Expected MIME type " + mimeType + " in header");
                }
            }
        }

        [Fact]
        public void MimeTypesGetDefinitionsAll()
        {
            int count = IOManager.Definitions.Count();
            Console.WriteLine(count + " Definitions registered");
#if PORTABLE
            Assert.Equal(16, count);
#else
            Assert.Equal(19, count);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeAny()
        {
            int count = IOManager.GetDefinitions(IOManager.Any).Count();
            Console.WriteLine(count + " Definitions registered");
#if PORTABLE
            Assert.Equal(16, count);
#else
            Assert.Equal(19, count);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNotation3_1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/n3");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(Notation3Parser), d.RdfParserType);
            Assert.Equal(typeof(Notation3Writer), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNotation3Parser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNotation3Writer), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNotation3_2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/rdf+n3");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(Notation3Parser), d.RdfParserType);
            Assert.Equal(typeof(Notation3Writer), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNotation3Parser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNotation3Writer), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtNotation3_1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".n3");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(Notation3Parser), d.RdfParserType);
            Assert.Equal(typeof(Notation3Writer), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtNotation3_2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("n3");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(Notation3Parser), d.RdfParserType);
            Assert.Equal(typeof(Notation3Writer), d.RdfWriterType);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtNotation3_3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".n3.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedNotation3Parser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNotation3Writer), d.RdfWriterType);
        }
#endif

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtNotation3_4()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("n3.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedNotation3Parser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNotation3Writer), d.RdfWriterType);
        }
#endif

        [Fact]
        public void MimeTypesGetDefinitionsByTypeTurtle1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/turtle");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeTurtle2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/x-turtle");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeTurtle3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/turtle");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtTurtle1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".ttl");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtTurtle2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("ttl");
            Assert.Equal(1, defs.Count());

#if !NO_COMPRESSION
            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);
#endif
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtTurtle3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".ttl.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
        }
#endif

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtTurtle4()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("ttl.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
        }
#endif

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNTriples1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/rdf-triples");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNTriples2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/plain");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
#endif
        }
       
        [Fact]
        public void MimeTypesGetDefinitionsByTypeNTriples3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/ntriples");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNTriples4()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/ntriples+turtle");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeNTriples5()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/x-ntriples");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtNTriples1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".nt");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtNTriples2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("nt");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(NTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(NTriplesWriter), d.RdfWriterType);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtNTriples3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".nt.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
        }
#endif

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtNTriples4()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("nt.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedNTriplesParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedNTriplesWriter), d.RdfWriterType);
        }
#endif

        
//#if !NO_HTMLAGILITYPACK
//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeRdfA1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/xhtml+xml");
//            Assert.Equal(2, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);

//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeRdfA2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/html");
//            Assert.Equal(2, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);

//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".html");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("html");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA3()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".html.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA4()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("html.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA5()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".htm");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA6()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("htm");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA7()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".htm.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA8()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("htm.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA9()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".xhtml");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA10()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("xhtml");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(RdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(HtmlWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA11()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".xhtml.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtRdfA12()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("xhtml.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedRdfAParser), d.RdfParserType);
//            Assert.Equal(typeof(GZippedRdfAWriter), d.RdfWriterType);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeSparqlXml1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/sparql-results+xml");
//#if PORTABLE
//            Assert.Equal(1, defs.Count());
//#else
//            Assert.Equal(2, defs.Count());
//#endif

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlXmlWriter), d.SparqlResultsWriterType);

//#if !NO_COMPRESSION
//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedSparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlXmlWriter), d.SparqlResultsWriterType);
//#endif
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlXml1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".srx");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlXmlWriter), d.SparqlResultsWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlXml2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("srx");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlXmlWriter), d.SparqlResultsWriterType);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlXml3()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".srx.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlXmlWriter), d.SparqlResultsWriterType);
//        }
//#endif

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlXml4()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("srx.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlXmlParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlXmlWriter), d.SparqlResultsWriterType);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeSparqlJson1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/sparql-results+json");
//#if PORTABLE
//            Assert.Equal(1, defs.Count());
//#else
//            Assert.Equal(2, defs.Count());
//#endif

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlJsonWriter), d.SparqlResultsWriterType);

//#if !NO_COMPRESSION
//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedSparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlJsonWriter), d.SparqlResultsWriterType);
//#endif
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlJson1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".srj");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlJsonWriter), d.SparqlResultsWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlJson2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("srj");
//            Assert.Equal(1, defs.Count());

//#if !NO_COMPRESSION
//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlJsonWriter), d.SparqlResultsWriterType);
//#endif
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlJson3()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".srj.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlJsonWriter), d.SparqlResultsWriterType);
//        }
//#endif

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlJson4()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("srj.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlJsonParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlJsonWriter), d.SparqlResultsWriterType);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeSparqlCsv1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/csv");
//#if PORTABLE
//            Assert.Equal(1, defs.Count());
//#else
//            Assert.Equal(2, defs.Count());
//#endif

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlCsvWriter), d.SparqlResultsWriterType);

//#if !NO_COMPRESSION
//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedSparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlCsvWriter), d.SparqlResultsWriterType);
//#endif
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeSparqlCsv2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/comma-separated-values");
//#if PORTABLE
//            Assert.Equal(1, defs.Count());
//#else
//            Assert.Equal(2, defs.Count());
//#endif

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlCsvWriter), d.SparqlResultsWriterType);

//#if !NO_COMPRESSION
//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedSparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlCsvWriter), d.SparqlResultsWriterType);
//#endif
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlCsv1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".csv");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlCsvWriter), d.SparqlResultsWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlCsv2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("csv");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlCsvWriter), d.SparqlResultsWriterType);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlCsv3()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".csv.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlCsvWriter), d.SparqlResultsWriterType);
//        }
//#endif

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlCsv4()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("csv.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlCsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlCsvWriter), d.SparqlResultsWriterType);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetDefinitionsByTypeSparqlTsv1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/tab-separated-values");
//#if PORTABLE
//            Assert.Equal(1, defs.Count());
//#else
//            Assert.Equal(2, defs.Count());
//#endif

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlTsvWriter), d.SparqlResultsWriterType);

//#if !NO_COMPRESSION
//            //Check GZipped definition
//            d = defs.Last();
//            Assert.Equal(typeof(GZippedSparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlTsvWriter), d.SparqlResultsWriterType);
//#endif
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlTsv1()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".tsv");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlTsvWriter), d.SparqlResultsWriterType);
//        }

//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlTsv2()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("tsv");
//            Assert.Equal(1, defs.Count());

//            //Check normal definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(SparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(SparqlTsvWriter), d.SparqlResultsWriterType);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlTsv3()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".tsv.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlTsvWriter), d.SparqlResultsWriterType);
//        }
//#endif

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetDefinitionsByExtSparqlTsv4()
//        {
//            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("tsv.gz");
//            Assert.Equal(1, defs.Count());

//            //Check GZipped definition
//            MimeTypeDefinition d = defs.First();
//            Assert.Equal(typeof(GZippedSparqlTsvParser), d.SparqlResultsParserType);
//            Assert.Equal(typeof(GZippedSparqlTsvWriter), d.SparqlResultsWriterType);
//        }
//#endif

        [Fact]
        public void MimeTypesGetDefinitionsByTypeCaseSensitivity1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("TEXT/TURTLE");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeCaseSensitivity2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("TEXT/turtle");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeCaseSensitivity3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("TeXt/TuRtLe");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtCaseSensitivity1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".TTL");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtCaseSensitivity2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".tTl");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeExtraParams1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/turtle; charset=utf-8");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeExtraParams2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/turtle; q=1.0");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(TurtleParser), d.RdfParserType);
            Assert.Equal(typeof(CompressingTurtleWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedTurtleParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedTurtleWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetParserByTypeNTriples1()
        {
            IRdfReader parser = IOManager.GetParser("text/plain");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNTriples2()
        {
            IRdfReader parser = IOManager.GetParser("text/ntriples");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNTriples3()
        {
            IRdfReader parser = IOManager.GetParser("text/ntriples+turtle");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNTriples4()
        {
            IRdfReader parser = IOManager.GetParser("application/rdf-triples");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNTriples5()
        {
            IRdfReader parser = IOManager.GetParser("application/x-ntriples");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeTurtle1()
        {
            IRdfReader parser = IOManager.GetParser("text/turtle");
            Assert.IsType<TurtleParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeTurtle2()
        {
            IRdfReader parser = IOManager.GetParser("application/x-turtle");
            Assert.IsType<TurtleParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeTurtle3()
        {
            IRdfReader parser = IOManager.GetParser("application/turtle");
            Assert.IsType<TurtleParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNotation3_1()
        {
            IRdfReader parser = IOManager.GetParser("text/n3");
            Assert.IsType<Notation3Parser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeNotation3_2()
        {
            IRdfReader parser = IOManager.GetParser("text/rdf+n3");
            Assert.IsType<Notation3Parser>(parser);
        }

   

//#if !NO_HTMLAGILITYPACK
//        [Fact]
//        public void MimeTypesGetParserByTypeRdfA1()
//        {
//            IRdfReader parser = IOManager.GetParser("text/html");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByTypeRdfA2()
//        {
//            IRdfReader parser = IOManager.GetParser("application/xhtml+xml");
//            Assert.IsType<RdfAParser>(parser);
//        }
//#endif

        [Fact]
        public void MimeTypesGetParserByTypeUnknown()
        {
            Assert.Throws<RdfParserSelectionException>(() =>
            {
                IRdfReader reader = IOManager.GetParser("application/unknown");
            });
        }

        [Fact]
        public void MimeTypesGetParserByExtNTriples1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".nt");
            Assert.IsType<NTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNTriples2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("nt");
            Assert.IsType<NTriplesParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtNTriples3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".nt.gz");
            Assert.IsType<GZippedNTriplesParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNTriples4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("nt.gz");
            Assert.IsType<GZippedNTriplesParser>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetParserByExtTurtle1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".ttl");
            Assert.IsType<TurtleParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTurtle2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("ttl");
            Assert.IsType<TurtleParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtTurtle3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".ttl.gz");
            Assert.IsType<GZippedTurtleParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTurtle4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("ttl.gz");
            Assert.IsType<GZippedTurtleParser>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetParserByExtNotation3_1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".n3");
            Assert.IsType<Notation3Parser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNotation3_2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("n3");
            Assert.IsType<Notation3Parser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtNotation3_3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".n3.gz");
            Assert.IsType<GZippedNotation3Parser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNotation3_4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("n3.gz");
            Assert.IsType<GZippedNotation3Parser>(parser);
        }
#endif



//#if !NO_HTMLAGILITYPACK
//        [Fact]
//        public void MimeTypesGetParserByExtRdfA1()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".html");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA2()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("html");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA3()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".html.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA4()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("html.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA5()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".htm");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA6()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("htm");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA7()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".htm.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA8()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("htm.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA9()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".xhtml");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA10()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("xhtml");
//            Assert.IsType<RdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA11()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension(".xhtml.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetParserByExtRdfA12()
//        {
//            IRdfReader parser = IOManager.GetParserByFileExtension("xhtml.gz");
//            Assert.IsType<GZippedRdfAParser>(parser);
//        }
//#endif

        [Fact]
        public void MimeTypesGetWriterByTypeUnknown()
        {
            IRdfWriter writer = IOManager.GetWriter("application/unknown");
            Assert.IsType<CompressingTurtleWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeAny()
        {
            IRdfWriter writer = IOManager.GetWriter(IOManager.Any);
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNTriples1()
        {
            IRdfWriter writer = IOManager.GetWriter("text/plain");
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNTriples2()
        {
            IRdfWriter writer = IOManager.GetWriter("text/ntriples");
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNTriples3()
        {
            IRdfWriter writer = IOManager.GetWriter("text/ntriples+turtle");
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNTriples4()
        {
            IRdfWriter writer = IOManager.GetWriter("application/rdf-triples");
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNTriples5()
        {
            IRdfWriter writer = IOManager.GetWriter("application/x-ntriples");
            Assert.IsType<NTriplesWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeTurtle1()
        {
            IRdfWriter writer = IOManager.GetWriter("text/turtle");
            Assert.IsType<CompressingTurtleWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeTurtle2()
        {
            IRdfWriter writer = IOManager.GetWriter("application/x-turtle");
            Assert.IsType<CompressingTurtleWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeTurtle3()
        {
            IRdfWriter writer = IOManager.GetWriter("application/turtle");
            Assert.IsType<CompressingTurtleWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNotation3_1()
        {
            IRdfWriter writer = IOManager.GetWriter("text/n3");
            Assert.IsType<Notation3Writer>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeNotation3_2()
        {
            IRdfWriter writer = IOManager.GetWriter("text/rdf+n3");
            Assert.IsType<Notation3Writer>(writer);
        }



//#if !NO_HTMLAGILITYPACK
//        [Fact]
//        public void MimeTypesGetWriterByTypeRdfA1()
//        {
//            IRdfWriter writer = IOManager.GetWriter("text/html");
//            Assert.IsType<HtmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByTypeRdfA2()
//        {
//            IRdfWriter writer = IOManager.GetWriter("application/xhtml+xml");
//            Assert.IsType<HtmlWriter>(writer);
//        }
//#endif

        [Fact]
        public void MimeTypesGetWriterByExtNTriples1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".nt");
            Assert.IsType<NTriplesWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNTriples2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("nt");
            Assert.IsType<NTriplesWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtNTriples3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".nt.gz");
            Assert.IsType<GZippedNTriplesWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNTriples4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("nt.gz");
            Assert.IsType<GZippedNTriplesWriter>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetWriterByExtTurtle1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".ttl");
            Assert.IsType<CompressingTurtleWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTurtle2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("ttl");
            Assert.IsType<CompressingTurtleWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtTurtle3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".ttl.gz");
            Assert.IsType<GZippedTurtleWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTurtle4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("ttl.gz");
            Assert.IsType<GZippedTurtleWriter>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetWriterByExtNotation3_1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".n3");
            Assert.IsType<Notation3Writer>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNotation3_2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("n3");
            Assert.IsType<Notation3Writer>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtNotation3_3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".n3.gz");
            Assert.IsType<GZippedNotation3Writer>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNotation3_4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("n3.gz");
            Assert.IsType<GZippedNotation3Writer>(parser);
        }
#endif

        

//#if !NO_HTMLAGILITYPACK
//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA1()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".html");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA2()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("html");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//        [Fact]
//#if !NO_COMPRESSION
//        public void MimeTypesGetWriterByExtRdfA3()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".html.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA4()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("html.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA5()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".htm");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA6()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("htm");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA7()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".htm.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA8()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("htm.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA9()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".xhtml");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA10()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("xhtml");
//            Assert.IsType<HtmlWriter>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA11()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension(".xhtml.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetWriterByExtRdfA12()
//        {
//            IRdfWriter parser = IOManager.GetWriterByFileExtension("xhtml.gz");
//            Assert.IsType<GZippedRdfAWriter>(parser);
//        }
//#endif
//#endif

//        [Test, ExpectedException(typeof(RdfParserSelectionException))]
//        public void MimeTypesGetSparqlParserByTypeUnknown()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("application/unknown");
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlXml1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("application/sparql-results+xml");
//            Assert.IsType<SparqlXmlParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlXml2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("application/xml");
//            Assert.IsType<SparqlXmlParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlJson1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("application/sparql-results+json");
//            Assert.IsType<SparqlJsonParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlJson2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("application/json");
//            Assert.IsType<SparqlJsonParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlCsv1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("text/csv");
//            Assert.IsType<SparqlCsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlCsv2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("text/comma-separated-values");
//            Assert.IsType<SparqlCsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByTypeSparqlTsv1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParser("text/tab-separated-values");
//            Assert.IsType<SparqlTsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlXml1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".srx");
//            Assert.IsType<SparqlXmlParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlXml2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("srx");
//            Assert.IsType<SparqlXmlParser>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlXml3()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".srx.gz");
//            Assert.IsType<GZippedSparqlXmlParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlXml4()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("srx.gz");
//            Assert.IsType<GZippedSparqlXmlParser>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlJson1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".srj");
//            Assert.IsType<SparqlJsonParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlJson2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("srj");
//            Assert.IsType<SparqlJsonParser>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlJson3()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".srj.gz");
//            Assert.IsType<GZippedSparqlJsonParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlJson4()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("srj.gz");
//            Assert.IsType<GZippedSparqlJsonParser>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlTsv1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".tsv");
//            Assert.IsType<SparqlTsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlTsv2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("tsv");
//            Assert.IsType<SparqlTsvParser>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlTsv3()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".tsv.gz");
//            Assert.IsType<GZippedSparqlTsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlTsv4()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("tsv.gz");
//            Assert.IsType<GZippedSparqlTsvParser>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlCsv1()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".csv");
//            Assert.IsType<SparqlCsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlCsv2()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("csv");
//            Assert.IsType<SparqlCsvParser>(parser);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlCsv3()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension(".csv.gz");
//            Assert.IsType<GZippedSparqlCsvParser>(parser);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlParserByExtSparqlCsv4()
//        {
//            ISparqlResultsReader parser = IOManager.GetSparqlParserByFileExtension("csv.gz");
//            Assert.IsType<GZippedSparqlCsvParser>(parser);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeUnknown()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("application/unknown");
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeAny()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter(IOManager.Any);
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlXml1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("application/sparql-results+xml");
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlXml2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("application/xml");
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlJson1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("application/sparql-results+json");
//            Assert.IsType<SparqlJsonWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlJson2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("application/json");
//            Assert.IsType<SparqlJsonWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlCsv1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("text/csv");
//            Assert.IsType<SparqlCsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlCsv2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("text/comma-separated-values");
//            Assert.IsType<SparqlCsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByTypeSparqlTsv1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriter("text/tab-separated-values");
//            Assert.IsType<SparqlTsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlXml1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".srx");
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlXml2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("srx");
//            Assert.IsType<SparqlXmlWriter>(writer);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlXml3()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".srx.gz");
//            Assert.IsType<GZippedSparqlXmlWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlXml4()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("srx.gz");
//            Assert.IsType<GZippedSparqlXmlWriter>(writer);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlJson1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".srj");
//            Assert.IsType<SparqlJsonWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlJson2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("srj");
//            Assert.IsType<SparqlJsonWriter>(writer);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlJson3()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".srj.gz");
//            Assert.IsType<GZippedSparqlJsonWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlJson4()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("srj.gz");
//            Assert.IsType<GZippedSparqlJsonWriter>(writer);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlTsv1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".tsv");
//            Assert.IsType<SparqlTsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlTsv2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("tsv");
//            Assert.IsType<SparqlTsvWriter>(writer);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlTsv3()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".tsv.gz");
//            Assert.IsType<GZippedSparqlTsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlTsv4()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("tsv.gz");
//            Assert.IsType<GZippedSparqlTsvWriter>(writer);
//        }
//#endif

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlCsv1()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".csv");
//            Assert.IsType<SparqlCsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlCsv2()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("csv");
//            Assert.IsType<SparqlCsvWriter>(writer);
//        }

//#if !NO_COMPRESSION
//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlCsv3()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension(".csv.gz");
//            Assert.IsType<GZippedSparqlCsvWriter>(writer);
//        }

//        [Fact]
//        public void MimeTypesGetSparqlWriterByExtSparqlCsv4()
//        {
//            ISparqlResultsWriter writer = IOManager.GetSparqlWriterByFileExtension("csv.gz");
//            Assert.IsType<GZippedSparqlCsvWriter>(writer);
//        }
//#endif

        [Fact]
        public void MimeTypesGetParserByTypeNQuads1()
        {
            IRdfReader parser = IOManager.GetParser("text/x-nquads");
            Assert.IsType<NQuadsParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeTriG1()
        {
            IRdfReader parser = IOManager.GetParser("application/x-trig");
            Assert.IsType<TriGParser>(parser);
        }


        [Fact]
        public void MimeTypesGetParserByExtNQuads1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".nq");
            Assert.IsType<NQuadsParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNQuads2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("nq");
            Assert.IsType<NQuadsParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtNQuads3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".nq.gz");
            Assert.IsType<GZippedNQuadsParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtNQuads4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("nq.gz");
            Assert.IsType<GZippedNQuadsParser>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetParserByExtTriG1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".trig");
            Assert.IsType<TriGParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTriG2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("trig");
            Assert.IsType<TriGParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtTriG3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".trig.gz");
            Assert.IsType<GZippedTriGParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTriG4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("trig.gz");
            Assert.IsType<GZippedTriGParser>(parser);
        }
#endif



        [Fact]
        public void MimeTypesGetWriterByTypeNQuads1()
        {
            IRdfWriter parser = IOManager.GetWriter("text/x-nquads");
            Assert.IsType<NQuadsWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeTriG1()
        {
            IRdfWriter parser = IOManager.GetWriter("application/x-trig");
            Assert.IsType<TriGWriter>(parser);
        }

       

        [Fact]
        public void MimeTypesGetWriterByExtNQuads1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".nq");
            Assert.IsType<NQuadsWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNQuads2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("nq");
            Assert.IsType<NQuadsWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtNQuads3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".nq.gz");
            Assert.IsType<GZippedNQuadsWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtNQuads4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("nq.gz");
            Assert.IsType<GZippedNQuadsWriter>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetWriterByExtTriG1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".trig");
            Assert.IsType<TriGWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTriG2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("trig");
            Assert.IsType<TriGWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtTriG3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".trig.gz");
            Assert.IsType<GZippedTriGWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTriG4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("trig.gz");
            Assert.IsType<GZippedTriGWriter>(parser);
        }
#endif


        [Fact]
        public void MimeTypesApplyWriterOptions1()
        {
            int compressionLevel = IOOptions.DefaultCompressionLevel;
            try
            {
                IOOptions.DefaultCompressionLevel = WriterCompressionLevel.High;
                IRdfWriter writer = IOManager.GetWriter("application/turtle");
                Assert.IsType<CompressingTurtleWriter>(writer);
                Assert.Equal(WriterCompressionLevel.High, ((ICompressingWriter)writer).CompressionLevel);
            }
            finally
            {
                IOOptions.DefaultCompressionLevel = compressionLevel;
            }
        }

        [Fact]
        public void MimeTypesApplyWriterOptions2()
        {
            int compressionLevel = IOOptions.DefaultCompressionLevel;
            try
            {
                IOOptions.DefaultCompressionLevel = WriterCompressionLevel.High;
                IRdfWriter writer = IOManager.GetWriterByFileExtension(".ttl");
                Assert.IsType<CompressingTurtleWriter>(writer);
                Assert.Equal(WriterCompressionLevel.High, ((ICompressingWriter)writer).CompressionLevel);
            }
            finally
            {
                IOOptions.DefaultCompressionLevel = compressionLevel;
            }
        }

        [Fact]
        public void MimeTypesApplyWriterOptions3()
        {
            int compressionLevel = IOOptions.DefaultCompressionLevel;
            try
            {
                IOOptions.DefaultCompressionLevel = WriterCompressionLevel.High;
                IRdfWriter writer = IOManager.GetWriter("application/x-trig");
                Assert.IsType<TriGWriter>(writer);
                Assert.Equal(WriterCompressionLevel.High, ((ICompressingWriter)writer).CompressionLevel);
            }
            finally
            {
                IOOptions.DefaultCompressionLevel = compressionLevel;
            }
        }

        [Fact]
        public void MimeTypesApplyWriterOptions4()
        {
            int compressionLevel = IOOptions.DefaultCompressionLevel;
            try
            {
                IOOptions.DefaultCompressionLevel = WriterCompressionLevel.High;
                IRdfWriter writer = IOManager.GetWriterByFileExtension(".trig");
                Assert.IsType<TriGWriter>(writer);
                Assert.Equal(WriterCompressionLevel.High, ((ICompressingWriter)writer).CompressionLevel);
            }
            finally
            {
                IOOptions.DefaultCompressionLevel = compressionLevel;
            }
        }

        [Fact]
        public void MimeTypesApplyParserOptions1()
        {
            TokenQueueMode queueMode = IOOptions.DefaultTokenQueueMode;
            try
            {
                IOOptions.DefaultTokenQueueMode = TokenQueueMode.AsynchronousBufferDuringParsing;
                IRdfReader parser = IOManager.GetParser("application/turtle");
                Assert.IsType<TurtleParser>(parser);
                Assert.Equal(TokenQueueMode.AsynchronousBufferDuringParsing, ((ITokenisingParser)parser).TokenQueueMode);
            }
            finally
            {
                IOOptions.DefaultTokenQueueMode = queueMode;
            }
        }

        [Fact]
        public void MimeTypesApplyParserOptions2()
        {
            TokenQueueMode queueMode = IOOptions.DefaultTokenQueueMode;
            try
            {
                IOOptions.DefaultTokenQueueMode = TokenQueueMode.AsynchronousBufferDuringParsing;
                IRdfReader parser = IOManager.GetParserByFileExtension(".ttl");
                Assert.IsType<TurtleParser>(parser);
                Assert.Equal(TokenQueueMode.AsynchronousBufferDuringParsing, ((ITokenisingParser)parser).TokenQueueMode);
            }
            finally
            {
                IOOptions.DefaultTokenQueueMode = queueMode;
            }
        }

        [Fact]
        public void MimeTypesApplyParserOptions3()
        {
            TokenQueueMode queueMode = IOOptions.DefaultTokenQueueMode;
            try
            {
                IOOptions.DefaultTokenQueueMode = TokenQueueMode.AsynchronousBufferDuringParsing;
                IRdfReader parser = IOManager.GetParser("text/x-nquads");
                Assert.IsType<NQuadsParser>(parser);
                Assert.Equal(TokenQueueMode.AsynchronousBufferDuringParsing, ((ITokenisingParser)parser).TokenQueueMode);
            }
            finally
            {
                IOOptions.DefaultTokenQueueMode = queueMode;
            }
        }

        [Fact]
        public void MimeTypesApplyParserOptions4()
        {
            TokenQueueMode queueMode = IOOptions.DefaultTokenQueueMode;
            try
            {
                IOOptions.DefaultTokenQueueMode = TokenQueueMode.AsynchronousBufferDuringParsing;
                IRdfReader parser = IOManager.GetParserByFileExtension(".nq");
                Assert.IsType<NQuadsParser>(parser);
                Assert.Equal(TokenQueueMode.AsynchronousBufferDuringParsing, ((ITokenisingParser)parser).TokenQueueMode);
            }
            finally
            {
                IOOptions.DefaultTokenQueueMode = queueMode;
            }
        }

        [Fact]
        public void MimeTypesContentNegotiation1()
        {
            String[] types = new String[] { "application/turtle" , "application/rdf+xml", "text/plain" };
            MimeTypeDefinition def = IOManager.GetDefinitions(types).FirstOrDefault();
            Assert.NotNull(def);
            Assert.Equal(typeof(TurtleParser), def.RdfParserType);
        }



        [Fact]
        public void MimeTypesContentNegotiation3()
        {
            String[] types = new String[] { "text/plain", "application/rdf+xml", "application/turtle" };
            MimeTypeDefinition def = IOManager.GetDefinitions(types).FirstOrDefault();
            Assert.NotNull(def);
            Assert.Equal(typeof(NTriplesParser), def.RdfParserType);
        }

        [Fact]
        public void MimeTypesContentNegotiation4()
        {
            MimeTypeDefinition def = IOManager.GetDefinitions(IOManager.Any).FirstOrDefault();
            Assert.NotNull(def);
        }

       
        private void PrintSelectors(IEnumerable<MimeTypeSelector> selectors)
        {
            foreach (MimeTypeSelector selector in selectors)
            {
                Console.WriteLine(selector.ToString());
            }
        }

        [Fact]
        public void MimeTypesSelectors1()
        {
            String[] types = new String[] { "audio/*; q=0.2", "audio/basic" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.False(selectors[0].IsRange);
            Assert.Equal("audio/basic", selectors[0].Type);
            Assert.True(selectors[1].IsRange);
            Assert.Equal("audio/*", selectors[1].Type);
            Assert.Equal(0.2d, selectors[1].Quality);
        }

        [Fact]
        public void MimeTypesSelectors2()
        {
            String[] types = new String[] { "text/plain; q=0.5", "text/turtle" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.Equal("text/turtle", selectors[0].Type);
            Assert.Equal("text/plain", selectors[1].Type);
            Assert.Equal(0.5d, selectors[1].Quality);
        }

        [Fact]
        public void MimeTypesSelectors3()
        {
            String[] types = new String[] { "text/plain", "text/turtle" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.Equal("text/plain", selectors[0].Type);
            Assert.Equal("text/turtle", selectors[1].Type);
        }

        [Fact]
        public void MimeTypesSelectors4()
        {
            String[] types = new String[] { "text/*", "text/html", "*/*" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.Equal("text/html", selectors[0].Type);
            Assert.Equal("text/*", selectors[1].Type);
            Assert.Equal(IOManager.Any, selectors[2].Type);
        }

        [Fact]
        public void MimeTypesSelectors5()
        {
            String[] types = new String[] { "text/plain; q=0.5", "text/turtle; q=0.5" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.Equal("text/plain", selectors[0].Type);
            Assert.Equal(0.5d, selectors[0].Quality);
            Assert.Equal("text/turtle", selectors[1].Type);
            Assert.Equal(0.5d, selectors[1].Quality);
        }

        [Fact]
        public void MimeTypesSelectors6()
        {
            String[] types = new String[] { "text/turtle; q=0.5", "text/plain; q=0.5" };
            List<MimeTypeSelector> selectors = MimeTypeSelector.CreateSelectors(types).ToList();
            this.PrintSelectors(selectors);

            Assert.Equal("text/turtle", selectors[0].Type);
            Assert.Equal(0.5d, selectors[0].Quality);
            Assert.Equal("text/plain", selectors[1].Type);
            Assert.Equal(0.5d, selectors[1].Quality);
        }

        [Fact]
        public void MimeTypesGetDefinitionByUpperCaseExt()
        {
            foreach (MimeTypeDefinition def in IOManager.Definitions)
            {
                if (!def.HasFileExtensions) continue;
                String ext = def.CanonicalFileExtension.ToUpper();
                MimeTypeDefinition def2 = IOManager.GetDefinitionsByFileExtension(ext).FirstOrDefault();
                Assert.NotNull(def2);
                Assert.Equal(def.SyntaxName, def2.SyntaxName);
                Assert.Equal(def.CanonicalMimeType, def2.CanonicalMimeType);
                Assert.Equal(def.HasFileExtensions, def2.HasFileExtensions);
                Assert.Equal(def.CanonicalFileExtension, def2.CanonicalFileExtension);
                Assert.Equal(def.CanParseRdf, def2.CanParseRdf);
                Assert.Equal(def.CanWriteRdf, def2.CanWriteRdf);
                Assert.Equal(def.RdfParserType, def2.RdfParserType);
                Assert.Equal(def.RdfWriterType, def2.RdfWriterType);
            }
        }
    }
}

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
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;
using VDS.RDF.Specifications;
using VDS.RDF.Writing.Formatting;
using FluentAssertions;

namespace VDS.RDF.Parsing.Suites
{
    public class Turtle11Unofficial
        : BaseRdfParserSuite
    {
        public Turtle11Unofficial()
            : base(new TurtleParser(TurtleSyntax.W3C), new NTriplesParser(), "turtle11-unofficial\\") { }

        [Fact]
        public void ParsingSuiteTurtleW3CUnofficalTests()
        {
            //Run manifests
            this.RunManifest("resources/turtle11-unofficial/manifest.ttl", true);
            this.RunManifest("resources/turtle11-unofficial/manifest-bad.ttl", false);

            this.Count.Should().NotBe(0, "No tests found");

            Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed");
            Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

            this.Failed.Should().Be(0, this.Failed + " Tests failed");
            this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate");
        }
    }

   
    public class Turtle11
        : BaseRdfParserSuite
    {
        public Turtle11()
            : base(new TurtleParser(TurtleSyntax.W3C), new NTriplesParser(), "turtle11\\") { }

        [Fact]
        public void ParsingSuiteTurtleW3C()
        {
            try
            {
                //Need IRI validation on to pass some of the tests
                Options.ValidateIris = true;

                //Nodes for positive and negative tests
                Graph g = new Graph();
                g.Namespaces.AddNamespace("rdft", UriFactory.Create("http://www.w3.org/ns/rdftest#"));
                INode posSyntaxTest = g.CreateUriNode("rdft:TestTurtlePositiveSyntax");
                INode posEvalTest = g.CreateUriNode("rdft:TestTurtleEval");
                INode negSyntaxTest = g.CreateUriNode("rdft:TestTurtleNegativeSyntax");
                INode negEvalTest = g.CreateUriNode("rdft:TestTurtleNegativeEval");

                //Run manifests
                this.RunManifest("resources/turtle11/manifest.ttl", new INode[] { posSyntaxTest, posEvalTest }, new INode[] { negSyntaxTest, negEvalTest });

                this.Count.Should().NotBe(0, "No tests found");

                Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed - " + this.Indeterminate + " Indeterminate");
                Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

                if (this.Failed > 0)
                {
                    if (this.Indeterminate == 0)
                    {
                        Assert.True(false, this.Failed + " Tests failed and " + this.Passed + " Tests Passed");
                    }
                    else
                    {
                        Assert.True(false, this.Failed + " Test failed, " + this.Indeterminate + " Tests are indeterminate and " + this.Passed + " Tests Passed");
                    }
                }
                this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate and " + this.Passed + " Tests Passed");
            }
            finally
            {
                Options.ValidateIris = false;
            }
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames1()
        {
            String input = "AZazÀÖØöø˿ͰͽͿ῿‌‍⁰↏Ⰰ⿯、퟿豈﷏ﷰ�𐀀�:";
            Assert.True(TurtleSpecsHelper.IsValidPrefix(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames2()
        {
            String input = "AZazÀÖØöø˿ͰͽͿ῿‌‍⁰↏Ⰰ⿯、퟿豈﷏ﷰ�𐀀�:o";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames3()
        {
            String input = ":a~b";
            Assert.False(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames4()
        {
            String input = ":a%b";
            Assert.False(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames5()
        {
            String input = @":a\~b";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames6()
        {
            String input = ":a%bb";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames7()
        {
            String input = @":\~";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames8()
        {
            String input = ":%bb";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames9()
        {
            String input = @"p:AZazÀÖØöø˿Ͱͽ΄῾‌‍⁰↉Ⰰ⿕、ퟻ﨎ﷇﷰ￯𐀀󠇯";
            Assert.True(TurtleSpecsHelper.IsValidQName(input, TurtleSyntax.W3C));
        }

        [Fact]
        public void ParsingTurtleW3CComplexPrefixedNames10()
        {
            NTriplesFormatter formatter = new NTriplesFormatter();

            Graph ttl = new Graph();
            ttl.LoadFromFile(@"resources\\turtle11\localName_with_non_leading_extras.ttl");
            Assert.False(ttl.IsEmpty);
            Console.WriteLine("Subject from Turtle: " + ttl.Triples.First().Subject.ToString(formatter));

            Graph nt = new Graph();
            NTriplesParser parser = new NTriplesParser();
            parser.Warning += TestTools.WarningPrinter;
            nt.LoadFromFile(@"resources\\turtle11\localName_with_non_leading_extras.nt", parser);
            Assert.False(nt.IsEmpty);
            Console.WriteLine("Subject from NTriples: " + nt.Triples.First().Subject.ToString(formatter));

            Assert.Equal(ttl.Triples.First().Subject, nt.Triples.First().Subject);
        }

        [Fact]
        public void ParsingTurtleW3CNumericLiterals1()
        {
            String input = "123.E+1";
            Assert.True(TurtleSpecsHelper.IsValidDouble(input));
        }

        [Fact]
        public void ParsingTurtleW3CNumericLiterals2()
        {
            String input = @"@prefix : <http://example.org/> .
:subject :predicate 123.E+1.";
            Graph g = new Graph();
            g.LoadFromString(input, new TurtleParser(TurtleSyntax.W3C));
            Assert.False(g.IsEmpty);
            Assert.Equal(1, g.Count);
        }

        [Fact]
        public void ParsingTurtleW3CLiteralEscapes1()
        {
            Graph g = new Graph();
            g.LoadFromFile(@"resources\\turtle11\literal_with_escaped_BACKSPACE.ttl");
            Assert.False(g.IsEmpty);
            Assert.Equal(1, g.Count);
            Triple t = g.Triples.First();
            Assert.Equal(NodeType.Literal, t.Object.NodeType);
            Assert.Equal(1, t.Object.Value.Length);
        }

        [Fact]
        public void ParsingTurtleW3CComplexLiterals1()
        {
            NTriplesFormatter formatter = new NTriplesFormatter();

            Graph ttl = new Graph();
            ttl.LoadFromFile(@"resources\\turtle11\LITERAL1_ascii_boundaries.ttl");
            Assert.False(ttl.IsEmpty);
            Console.WriteLine("Object from Turtle: " + ttl.Triples.First().Object.ToString(formatter));

            Graph nt = new Graph();
            nt.LoadFromFile(@"resources\\turtle11\LITERAL1_ascii_boundaries.nt");
            Assert.False(nt.IsEmpty);
            Console.WriteLine("Object from NTriples: " + nt.Triples.First().Object.ToString(formatter));

            Assert.Equal(ttl.Triples.First().Object, nt.Triples.First().Object);
        }

        [Fact]
        public void ParsingTurtleW3CComplexLiterals2()
        {
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                g.LoadFromFile(@"resources\\turtle11\turtle-syntax-bad-string-04.ttl");
            });
        }

        // TODO Use a custom handler that captures the Base URI for checking

        [Fact]
        public void ParsingTurtleW3CBaseTurtleStyle1()
        {
            //Dot required
            const string graph = "@base <http://example.org/> .";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));
        }

        [Fact]
        public void ShouldThrowWhenTurtleStyleBaseIsMissingDot()
        {
            //Missing dot
            const string graph = "@base <http://example.org/>";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(g, new StringReader(graph));
            });
        }

        [Fact]
        public void ParsingTurtleW3CBaseTurtleStyle3()
        {
            //@base is case sensitive in Turtle
            const string graph = "@BASE <http://example.org/> .";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(g, new StringReader(graph));
            });
        }

        [Fact]
        public void ParsingTurtleW3CBaseSparqlStyle1()
        {
            //Forbidden dot
            const string graph = "BASE <http://example.org/> .";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(g, new StringReader(graph));
            });
        }

        [Fact]
        public void ShouldSuccessfullyParseValidSparqlStyleW3CBase()
        {
            //No dot required
            const string graph = "BASE <http://example.org/>";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));
        }

        [Fact]
        public void ParsingTurtleW3CBaseSparqlStyle3()
        {
            //No dot required and case insensitive
            const string graph = "BaSe <http://example.org/>";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));
        }

        [Fact]
        public void ParsingTurtleW3CPrefixTurtleStyle1()
        {
            //Dot required
            String graph = "@prefix ex: <http://example.org/> .";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));

            Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
        }

        [Fact]
        public void ShouldThrowWhenTurtleStylePrefixIsMissingDot()
        {
            //Missing dot
            const string graph = "@prefix ex: <http://example.org/>";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {

                this.Parser.Load(g, new StringReader(graph));

                Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
            });
        }

        [Fact]
        public void ParsingTurtleW3CPrefixTurtleStyle3()
        {
            //@prefix is case sensitive in Turtle
            const string graph = "@PREFIX ex: <http://example.org/> .";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(g, new StringReader(graph));

                Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
            });
        }

        [Fact]
        public void ParsingTurtleW3CPrefixSparqlStyle1()
        {
            //Forbidden dot
            const string graph = "PREFIX ex: <http://example.org/> .";
            Graph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {

                this.Parser.Load(g, new StringReader(graph));

                Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
            });
        }

        [Fact]
        public void ShouldSuccessfullyParseValidSparqlStyleW3CPrefix()
        {
            //No dot required
            const string graph = "PREFIX ex: <http://example.org/>";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));

            Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
        }

        [Fact]
        public void ParsingTurtleW3CPrefixSparqlStyle3()
        {
            //No dot required and case insensitive
            String graph = "PrEfIx ex: <http://example.org/>";
            Graph g = new Graph();
            this.Parser.Load(g, new StringReader(graph));

            Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
        }
    }
}

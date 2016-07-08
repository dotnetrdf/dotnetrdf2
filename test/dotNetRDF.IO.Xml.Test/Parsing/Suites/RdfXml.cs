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
using System.IO;
using System.Text;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;
using FluentAssertions;

namespace VDS.RDF.Parsing.Suites
{
#if !NO_XMLDOM
    public class RdfXmlDomParser
        : BaseRdfParserSuite
    {
        public RdfXmlDomParser()
            : base(new RdfXmlParser(RdfXmlParserMode.DOM), new NTriplesParser(), "rdfxml\\")
        {
            this.CheckResults = false;
        }

        [Fact]
        public void ParsingSuiteRdfXmlDOM()
        {
            //Run manifests
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && !f.Contains("error"), true);
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && f.Contains("error"), false);

            this.Count.Should().NotBe(0, because: "No tests found");

            Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed");
            Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

            this.Failed.Should().Be(0, this.Failed + " Tests failed");
            this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate");
        }

        [Fact]
        public void ParsingRdfXmlIdsDom()
        {
            IGraph g = new Graph();
            this.Parser.Load(g, @"resources\rdfxml\xmlbase\test014.rdf");
        }

        [Fact]
        public void ParsingRdfXmlIllegalBaseDom()
        {
            IGraph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(new GraphHandler(g), new StreamReader(new FileStream(@"resources\rdfxml\xmlbase\error001.rdf", FileMode.Open)));
            });
        }
    }
#endif

    public class RdfXmlStreamingParser
        : BaseRdfParserSuite
    {
        public RdfXmlStreamingParser()
            : base(new RdfXmlParser(RdfXmlParserMode.Streaming), new NTriplesParser(), "rdfxml\\")
        {
            this.CheckResults = false;
        }

#if PORTABLE
        private readonly string[] _portableIgnoreTests = new string[]
            {
                "resources\\rdfxml\\amp-in-url\\test001.rdf", // Has a DOCTYPE declaration which cannot be parsed in PCL
                "resources\\rdfxml\\rdf-containers-syntax-vs-schema\\test005.rdf", // Has no root element -- note that this test is not run by the NET40 build either
                "resources\\rdfxml\\xmlbase\\test012.rdf", // Has no root element -- again this test is not run byt the NET40 build
                "resources\\rdfxml\\rdf-charmod-uris\\error001.rdf" // Tests for normalization but there is no support for this in PCL
            };
#endif

        [Fact]
        public void ParsingSuiteRdfXmlStreaming()
        {
#if PORTABLE
            //Run manifests
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && !f.Contains("error") && !_portableIgnoreTests.Contains(f), true);
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && f.Contains("error") && !_portableIgnoreTests.Contains(f), false);
#else
            //Run manifests
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && !f.Contains("error"), true);
            this.RunAllDirectories(f => Path.GetExtension(f).Equals(".rdf") && f.Contains("error"), false);
#endif
            this.Count.Should().NotBe(0,"No tests found");

            Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed");
            Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

            this.Failed.Should().Be(0, this.Failed + " Tests failed");
            this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate");
        }

        [Fact]
        public void ParsingRdfXmlIDsStreaming()
        {
            IGraph g = new Graph();
            this.Parser.Load(g, @"resources\rdfxml\xmlbase\test014.rdf");
        }

        [Fact]
        public void ParsingRdfXmlIllegalBaseStreaming()
        {
            IGraph g = new Graph();
            Assert.Throws<RdfParseException>(() =>
            {
                this.Parser.Load(new GraphHandler(g), new StreamReader(new FileStream(@"resources\rdfxml\xmlbase\error001.rdf", FileMode.Open)));
            });
        }
    }
}

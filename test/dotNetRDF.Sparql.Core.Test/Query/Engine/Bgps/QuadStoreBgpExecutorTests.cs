﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Engine.Bgps
{
    public abstract class AbstractBgpExecutorTests
    {
        private static IGraph CreateGraph()
        {
            IGraph g = new Graph();
            g.Namespaces.AddNamespace(String.Empty, new Uri("http://example.org/"));

            INode s1 = g.CreateUriNode(":subject");
            INode s2 = g.CreateBlankNode();
            INode p = g.CreateUriNode(":predicate");
            INode o1 = g.CreateUriNode(":object");
            INode o2 = g.CreateLiteralNode("test");

            g.Assert(s1, p, o1);
            g.Assert(s1, p, o2);
            g.Assert(s1, p, s2);
            g.Assert(s2, p, o1);
            g.Assert(s2, p, o2);

            return g;
        }

        protected abstract IBgpExecutor CreateExecutor(IGraph g);

        [Fact]
        public void BgpExecutorGround1()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateUriNode(":subject"), g.CreateUriNode(":predicate"), g.CreateUriNode(":object"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(1, results.Count);
            Assert.Equal(0, results.First().Variables.Count());
        }

        [Fact]
        public void BgpExecutorGround2()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateUriNode(":subject"), g.CreateUriNode(":predicate"), g.CreateUriNode(":nosuchthing2"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void BgpExecutorMatch1()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateUriNode(":subject"), g.CreateUriNode(":predicate"), g.CreateVariableNode("o"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(3, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 1));
            Assert.True(results.All(s => s.ContainsVariable("o")));
        }

        [Fact]
        public void BgpExecutorMatch2()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateVariableNode("s"), g.CreateUriNode(":predicate"), g.CreateVariableNode("o"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 2));
            Assert.True(results.All(s => s.ContainsVariable("o") && s.ContainsVariable("s")));
        }

        [Fact]
        public void BgpExecutorMatch3()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateVariableNode("s"), g.CreateUriNode(":predicate"), g.CreateBlankNode());

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 2));
            Assert.True(results.All(s => s.ContainsVariable("s")));
        }

        [Fact]
        public void BgpExecutorChainedMatch1()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateVariableNode("s"), g.CreateUriNode(":predicate"), g.CreateVariableNode("o"));
            Triple search2 = new Triple(g.CreateVariableNode("o"), g.CreateUriNode(":predicate"), g.CreateVariableNode("o2"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 2));
            Assert.True(results.All(s => s.ContainsVariable("o") && s.ContainsVariable("s")));

            results = results.SelectMany(s => executor.Match(search2, s, context)).ToList();
            Assert.Equal(2, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 3));
            Assert.True(results.All(s => s.ContainsVariable("o2")));
        }

        [Fact]
        public void BgpExecutorChainedMatch2()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            INode b = g.CreateBlankNode();
            Triple search = new Triple(g.CreateVariableNode("s"), g.CreateUriNode(":predicate"), b);
            Triple search2 = new Triple(b, g.CreateUriNode(":predicate"), g.CreateVariableNode("o"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 2));
            Assert.True(results.All(s => s.ContainsVariable("s")));

            results = results.SelectMany(s => executor.Match(search2, s, context)).ToList();
            Assert.Equal(2, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 3));
            Assert.True(results.All(s => s.ContainsVariable("o")));
        }

        [Fact]
        public void BgpExecutorChainedMatch3()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateVariableNode("s"), g.CreateUriNode(":predicate"), g.CreateVariableNode("o"));
            Triple search2 = new Triple(g.CreateVariableNode("o"), g.CreateUriNode(":predicate"), g.CreateLiteralNode("nosuchthing"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 2));
            Assert.True(results.All(s => s.ContainsVariable("o") && s.ContainsVariable("s")));

            results = results.SelectMany(s => executor.Match(search2, s, context)).ToList();
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void BgpExecutorDistinctMatch1()
        {
            IGraph g = CreateGraph();
            IBgpExecutor executor = this.CreateExecutor(g);

            Triple search = new Triple(g.CreateVariableNode("a"), g.CreateVariableNode("b"), g.CreateVariableNode("c"));
            Triple search2 = new Triple(g.CreateVariableNode("d"), g.CreateVariableNode("e"), g.CreateVariableNode("f"));

            QueryExecutionContext context = new QueryExecutionContext(Quad.DefaultGraphNode, Quad.DefaultGraphNode.AsEnumerable(), null);
            List<ISolution> results = executor.Match(search, context).ToList();
            Assert.Equal(5, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 3));
            Assert.True(results.All(s => s.ContainsVariable("a") && s.ContainsVariable("b") && s.ContainsVariable("c")));

            results = results.SelectMany(s => executor.Match(search2, s, context)).ToList();
            Assert.Equal(25, results.Count);
            Assert.True(results.All(s => s.Variables.Count() == 6));
            Assert.True(results.All(s => s.ContainsVariable("a") && s.ContainsVariable("b") && s.ContainsVariable("c") && s.ContainsVariable("d") && s.ContainsVariable("e") && s.ContainsVariable("f")));
        }
    }

    public class QuadStoreBgpExecutorTests 
        : AbstractBgpExecutorTests
    {
        private static IQuadStore CreateQuadStore(IGraph g)
        {
            GraphStore gs = new GraphStore();
            gs.Add(g);
            return gs;
        }

        protected override IBgpExecutor CreateExecutor(IGraph g)
        {
            return new QuadStoreBgpExecutor(CreateQuadStore(g));
        }
    }

    public class GraphBgpExecutorTests
        : AbstractBgpExecutorTests
    {
        protected override IBgpExecutor CreateExecutor(IGraph g)
        {
            return new GraphBgpExecutor(g);
        }
    }
}

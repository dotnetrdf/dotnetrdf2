﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using VDS.RDF.Query.Expressions.Aggregates.Sparql;
using VDS.RDF.Query.Expressions.Primary;

namespace VDS.RDF.Query.Expressions.Aggregates
{
    public class AggregateComparisonTests
    {
        private void TestEquals(params IAggregateExpression[] aggs)
        {
            // Should be equal to itself
            foreach (IAggregateExpression agg in aggs)
            {
                Assert.Equal(agg, agg);
                Assert.Equal(agg.GetHashCode(), agg.GetHashCode());
            }

            // Should be equal to all others
            for (int i = 0; i < aggs.Length; i++)
            {
                IAggregateExpression agg = aggs[i];
                for (int j = 0; j < aggs.Length; j++)
                {
                    if (i == j) continue;

                    Assert.Equal(agg, aggs[j]);
                    Assert.Equal(agg.GetHashCode(), aggs[j].GetHashCode());
                }
            }
        }

        private void TestNotEquals(params IAggregateExpression[] aggs)
        {
            // Should be equal to itself
            foreach (IAggregateExpression agg in aggs)
            {
                Assert.Equal(agg, agg);
                Assert.Equal(agg.GetHashCode(), agg.GetHashCode());
            }

            // Should not be equal to all others
            for (int i = 0; i < aggs.Length; i++)
            {
                IAggregateExpression agg = aggs[i];
                for (int j = 0; j < aggs.Length; j++)
                {
                    if (i == j) continue;

                    Assert.NotEqual(agg, aggs[j]);
                }
            }
        }

        [Fact]
        public void AggregateEqualityCount()
        {
            IAggregateExpression[] aggs = new IAggregateExpression[]
                                          {
                                              new CountAggregate(new VariableTerm("x")), 
                                              new CountAggregate(new VariableTerm("x"))
                                          };
            TestEquals(aggs);
            aggs[1] = new CountAggregate(new VariableTerm("y"));
            TestNotEquals(aggs);
        }

        [Fact]
        public void AggregateEqualityCountAll()
        {
            IAggregateExpression[] aggs = new IAggregateExpression[]
                                          {
                                              new CountAllAggregate(), 
                                              new CountAllAggregate()
                                          };
            TestEquals(aggs);
            aggs[1] = new CountAggregate(new VariableTerm("x"));
            TestNotEquals(aggs);
        }

        [Fact]
        public void AggregateEqualityCountDistinct()
        {
            IAggregateExpression[] aggs = new IAggregateExpression[]
                                          {
                                              new CountDistinctAggregate(new VariableTerm("x")), 
                                              new CountDistinctAggregate(new VariableTerm("x"))
                                          };
            TestEquals(aggs);
            aggs[1] = new CountAggregate(new VariableTerm("x"));
            TestNotEquals(aggs);
        }

        [Fact]
        public void AggregateEqualityCountAllDistinct()
        {
            IAggregateExpression[] aggs = new IAggregateExpression[]
                                          {
                                              new CountAllDistinctAggregate(), 
                                              new CountAllDistinctAggregate()
                                          };
            TestEquals(aggs);
            aggs[1] = new CountAggregate(new VariableTerm("x"));
            TestNotEquals(aggs);
        }
    }
}

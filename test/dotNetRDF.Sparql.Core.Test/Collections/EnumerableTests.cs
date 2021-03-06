﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace VDS.RDF.Collections
{
    public class EnumerableTests
    {
        public static void Check<T>(IEnumerable<T> expected, IEnumerable<T> actual) 
            where T : class
        {
            Console.WriteLine("Expected:");
            TestTools.PrintEnumerable(expected, ",");
            Console.WriteLine();
            Console.WriteLine("Actual:");
            TestTools.PrintEnumerable(actual, ",");
            Console.WriteLine();

            Assert.Equal(expected.Count(), actual.Count());

            IEnumerator<T> expectedEnumerator = expected.GetEnumerator();
            IEnumerator<T> actualEnumerator = actual.GetEnumerator();

            int i = 0;
            while (expectedEnumerator.MoveNext())
            {
                T expectedItem = expectedEnumerator.Current;
                i++;
                Assert.True(actualEnumerator.MoveNext(), String.Format("Actual enumerator was exhaused at Item {0} when next Item {1} was expected", i, expectedItem));
                T actualItem = actualEnumerator.Current;

                Assert.True(actualItem.Equals(expectedItem), String.Format("Enumerators mismatched at Item {0}", i));
            }
            Assert.False(actualEnumerator.MoveNext(), "Actual enumerator has additional unexpected items");
        }

        public static void CheckStruct<T>(IEnumerable<T> expected, IEnumerable<T> actual) where T : struct
        {
            Console.WriteLine("Expected:");
            TestTools.PrintEnumerableStruct(expected, ",");
            Console.WriteLine();
            Console.WriteLine("Actual:");
            TestTools.PrintEnumerableStruct(actual, ",");
            Console.WriteLine();

            Assert.Equal(expected.Count(), actual.Count());

            IEnumerator<T> expectedEnumerator = expected.GetEnumerator();
            IEnumerator<T> actualEnumerator = actual.GetEnumerator();

            int i = 0;
            while (expectedEnumerator.MoveNext())
            {
                T expectedItem = expectedEnumerator.Current;
                i++;
                Assert.True(actualEnumerator.MoveNext(), String.Format("Actual enumerator was exhaused at Item {0} when next Item {1} was expected", i, expectedItem));
                T actualItem = actualEnumerator.Current;

                Assert.True(actualItem.Equals(expectedItem), String.Format("Enumerators mismatched at Item {0}", i));
            }
            Assert.False(actualEnumerator.MoveNext(), "Actual enumerator has additional unexpected items");
        }

        private static void Exhaust<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) { }
        }

        public static readonly Object[] SkipAndTakeData =
        {
            new object[] { 1, 1, 1},
            new object[] { 1, 50, 10 },
            new object[] { 1, 10, 10 },
            new object[] { 1, 10, 5 },
            new object[] { 1, 10, 20 },
            new object[] { 1, 100, 50 },
        };

        public static readonly Object[] AddOmitData =
        {
            new object[] { 1, 1, 1},
            new object[] { 1, 10, 1},
            new object[] { 1, 10, 11},
            new object[] { 1, 10, 100},
            new object[] { 1, 100, 50 },
            new object[] { 1, 100, 1000 }
        };

        [Fact]
        public void EnumeratorBeforeFirstElement1()
        {
            IEnumerable<int> data = Enumerable.Range(1, 10);
            IEnumerator<int> enumerator =  new LongTakeEnumerator<int>(data.GetEnumerator(), 1);
            Assert.Throws<InvalidOperationException>(() =>
            {
                int i = enumerator.Current;
            });
        }

        [Fact]
        public void EnumeratorBeforeFirstElement2()
        {
            IEnumerable<String> data = new String[] { "a", "b", "c" };
            IEnumerator<String> enumerator = new LongTakeEnumerator<String>(data.GetEnumerator(), 1);
            Assert.Throws<InvalidOperationException>(() =>
            {
                String i = enumerator.Current;
                Assert.Equal(default(String), i);
            });
        }

        [Fact]
        public void EnumeratorBeforeFirstElement3()
        {
            IEnumerable<int> data = Enumerable.Range(1, 10);
            IEnumerator<int> enumerator = new LongSkipEnumerator<int>(data.GetEnumerator(), 1);
            Assert.Throws<InvalidOperationException>(() =>
            {
                int i = enumerator.Current;
            });
        }

        [Fact]
        public void EnumeratorBeforeFirstElement4()
        {
            IEnumerable<String> data = new String[] { "a", "b", "c" };
            IEnumerator<String> enumerator = new LongSkipEnumerator<String>(data.GetEnumerator(), 1);
            Assert.Throws<InvalidOperationException>(() =>
            {
                String i = enumerator.Current;
                Assert.Equal(default(String), i);
            });
        }

        [Fact]
        public void EnumeratorAfterLastElement1()
        {
            IEnumerable<int> data = Enumerable.Range(1, 10);
            IEnumerator<int> enumerator = new LongTakeEnumerator<int>(data.GetEnumerator(), 1);
            Exhaust(enumerator);
            Assert.Throws<InvalidOperationException>(() =>
            {
                int i = enumerator.Current;
            });
        }

        [Fact]
        public void EnumeratorAfterLastElement2()
        {
            IEnumerable<String> data = new String[] { "a", "b", "c" };
            IEnumerator<String> enumerator = new LongTakeEnumerator<String>(data.GetEnumerator(), 1);
            Exhaust(enumerator);
            Assert.Throws<InvalidOperationException>(() =>
            {
                String i = enumerator.Current;
                Assert.Equal(default(String), i);
            });
        }

        [Fact]
        public void EnumeratorAfterLastElement3()
        {
            IEnumerable<int> data = Enumerable.Range(1, 10);
            IEnumerator<int> enumerator = new LongSkipEnumerator<int>(data.GetEnumerator(), 1);
            Exhaust(enumerator);
            Assert.Throws<InvalidOperationException>(() =>
            {
                int i = enumerator.Current;
            });
        }

        [Fact]
        public void EnumeratorAfterLastElement4()
        {
            IEnumerable<String> data = new String[] { "a", "b", "c" };
            IEnumerator<String> enumerator = new LongSkipEnumerator<String>(data.GetEnumerator(), 1);
            Exhaust(enumerator);
            Assert.Throws<InvalidOperationException>(() =>
            {
                String i = enumerator.Current;
                Assert.Equal(default(String), i);
            });
        }
            
        [Theory, MemberData("SkipAndTakeData")]
        public void LongSkipEnumerable(int start, int count, int skip)
        {
            IEnumerable<int> data = Enumerable.Range(start, count);
            IEnumerable<int> expected = data.Skip(skip);
            IEnumerable<int> actual = new LongSkipEnumerable<int>(data, skip);

            if (skip > count)
            {
                Assert.False(expected.Any());
                Assert.False(actual.Any());
            }
            
            CheckStruct(expected, actual);
        }

        [Theory, MemberData("SkipAndTakeData")]
        public void LongTakeEnumerable(int start, int count, int take)
        {
            IEnumerable<int> data = Enumerable.Range(start, count);
            IEnumerable<int> expected = data.Take(take);
            IEnumerable<int> actual = new LongTakeEnumerable<int>(data, take);

            CheckStruct(expected, actual);
        }

        [Theory, MemberData("AddOmitData")]
        public void AddDistinctEnumerable(int start, int count, int item)
        {
            IEnumerable<int> data = Enumerable.Range(start, count);
            IEnumerable<int> expected = data.Concat(item.AsEnumerable()).Distinct();
            IEnumerable<int> actual = new AddDistinctEnumerable<int>(data, item);

            CheckStruct(expected, actual);
        }

        [Theory, MemberData("AddOmitData")]
        public void OmitAllEnumerable(int start, int count, int item)
        {
            IEnumerable<int> data = Enumerable.Range(start, count);
            IEnumerable<int> expected = data.Where(x => !x.Equals(item));
            IEnumerable<int> actual = new OmitAllEnumerable<int>(data, item);

            CheckStruct(expected, actual);
        }

        [Fact]
        public void AddIfEmptyEnumerable()
        {
            IEnumerable<int> data = Enumerable.Empty<int>();
            IEnumerable<int> actual = data.AddIfEmpty(1);

            Assert.False(data.Any());            
            Assert.True(actual.Any());
            Assert.Equal(1, actual.First());
        }

        [Fact]
        public void TopNEnumerable1()
        {
            IEnumerable<int> data = new int[] {1, 7, 9, 3, 5};
            IEnumerable<int> expected = data.OrderBy(i => i).Take(1);
            IEnumerable<int> actual = data.Top(1);

            TestTools.PrintOrderingComparisonEnumerableStruct(data);

            CheckStruct(expected, actual);
        }

        [Fact]
        public void TopNEnumerable2()
        {
            IEnumerable<int> data = new int[] { 1, 7, 9, 3, 5 };
            IEnumerable<int> expected = data.OrderBy(i => i).Take(3);
            IEnumerable<int> actual = data.Top(3);

            TestTools.PrintOrderingComparisonEnumerableStruct(data);

            CheckStruct(expected, actual);
        }

        [Fact]
        public void TopNEnumerable3()
        {
            IEnumerable<int> data = new int[] { 1, 7, 9, 3, 5 };
            IComparer<int> comparer = new ReversedComparer<int>();
            IEnumerable<int> expected = data.OrderBy(i => i, comparer).Take(3);
            IEnumerable<int> actual = data.Top(comparer, 3);

            CheckStruct(expected, actual);
        }

        [Fact]
        public void TopNEnumerable4()
        {
            IEnumerable<String> data = new String[] { "a", "z", "m", "q", "c", "f" };
            IEnumerable<String> expected = data.OrderBy(i => i).Take(3);
            IEnumerable<String> actual = data.Top(3);

            TestTools.PrintOrderingComparisonEnumerable(data);

            Check(expected, actual);
        }
    }
}

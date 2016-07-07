/*
VDS.Common is licensed under the MIT License

Copyright (c) 2012-2015 Robert Vesse

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace VDS.Common.Collections
{
    [Trait("Category", "Arrays")]
    public abstract class AbstractSparseArrayContractTests
    {
        /// <summary>
        /// Creates a new sparse array of the given length to test
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns></returns>
        public abstract ISparseArray<int> CreateInstance(int length);

        [Fact]
        public void SparseArrayBadInstantiation()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                this.CreateInstance(-1);
            });
        }

        [Fact]
        public void SparseArrayEmpty1()
        {
            ISparseArray<int> array = this.CreateInstance(0);
            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void SparseArrayEmpty2()
        {
            ISparseArray<int> array = this.CreateInstance(0);
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                int i = array[0];
            });
        }

        [Fact]
        public void SparseArrayEmpty3()
        {
            ISparseArray<int> array = this.CreateInstance(0);
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                int i = array[-1];
            });
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayGetSet1(int length)
        {
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Equal(length, array.Length);
            for (int i = 0, j = 1; i < array.Length; i++, j *= 2)
            {
                // Should have default value
                Assert.Equal(default(int), array[i]);

                // Set only powers of 2
                if (i != j) continue;
                array[i] = 1;
                Assert.Equal(1, array[i]);
            }
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayGetSet2(int length)
        {
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                int i = array[-1];
            });
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayGetSet3(int length)
        {
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                int i = array[length];
            });
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayEnumerator1(int length)
        {
            Assert.NotEqual(default(int), 1);

            // Sparsely filled array
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Equal(length, array.Length);
            int[] actualArray = new int[length];
            for (int i = 0, j = 1; i < array.Length; i++, j *= 2)
            {
                // Should have default value
                Assert.Equal(default(int), array[i]);

                // Set only powers of 2
                if (i != j) continue;
                array[i] = 1;
                actualArray[i] = 1;
                Assert.Equal(1, array[i]);
            }

            IEnumerator<int> sparsEnumerator = array.GetEnumerator();
            IEnumerator actualEnumerator = actualArray.GetEnumerator();

            int index = -1;
            while (actualEnumerator.MoveNext())
            {
                index++;
                Assert.True(sparsEnumerator.MoveNext(), "Unable to move next at index " + index);
                sparsEnumerator.Current.Should().Be((int)actualEnumerator.Current, because:"Incorrect value at index " + index);
            }
            Assert.Equal(length - 1, index);
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayEnumerator2(int length)
        {
            Assert.NotEqual(default(int), 1);

            // Completely sparse array i.e. no actual data
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Equal(length, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                // Should have default value
                Assert.Equal(default(int), array[i]);
            }

            IEnumerator<int> enumerator = array.GetEnumerator();

            int index = -1;
            while (enumerator.MoveNext())
            {
                index++;
                enumerator.Current.Should().Be(default(int), because:"Incorrect value at index " + index);
            }
            Assert.Equal(length - 1, index);
        }

        [Theory, InlineData(1),
         InlineData(10),
         InlineData(50),
         InlineData(100),
         InlineData(250),
         InlineData(500),
         InlineData(1000),
         InlineData(10000)]
        public void SparseArrayEnumerator3(int length)
        {
            Assert.NotEqual(default(int), 1);

            // Completely filled array
            ISparseArray<int> array = this.CreateInstance(length);
            Assert.Equal(length, array.Length);
            int[] actualArray = new int[length];
            for (int i = 0; i < array.Length; i++)
            {
                // Should have default value
                Assert.Equal(default(int), array[i]);

                // Set all entries
                array[i] = 1;
                actualArray[i] = 1;
                Assert.Equal(1, array[i]);
            }

            IEnumerator<int> sparsEnumerator = array.GetEnumerator();
            IEnumerator actualEnumerator = actualArray.GetEnumerator();

            int index = -1;
            while (actualEnumerator.MoveNext())
            {
                index++;
                Assert.True(sparsEnumerator.MoveNext(), "Unable to move next at index " + index);
                sparsEnumerator.Current.Should().Be((int)actualEnumerator.Current, because:"Incorrect value at index " + index);
            }
            Assert.Equal(length - 1, index);
        }
    }

    [Trait("Category", "Arrays")]
    public class LinkedSparseArrayTests
        : AbstractSparseArrayContractTests
    {
        public override ISparseArray<int> CreateInstance(int length)
        {
            return new LinkedSparseArray<int>(length);
        }
    }

    public abstract class BlockSparseArrayTests
        : AbstractSparseArrayContractTests
    {
        public BlockSparseArrayTests(int blockSize)
        {
            this.BlockSize = blockSize;
        }

        private int BlockSize { get; set; }

        public override ISparseArray<int> CreateInstance(int length)
        {
            return new BlockSparseArray<int>(length, this.BlockSize);
        }
    }

    [Trait("Category", "Arrays")]
    public class BlockSparseArrayTests1 : BlockSparseArrayTests
    {
        public BlockSparseArrayTests1() : base(1) { }
    }

    [Trait("Category", "Arrays")]
    public class BlockSparseArrayTests10 : BlockSparseArrayTests
    {
        public BlockSparseArrayTests10() : base(10) { }
    }

    [Trait("Category", "Arrays")]
    public class BlockSparseArrayTests250 : BlockSparseArrayTests
    {
        public BlockSparseArrayTests250() : base(250) { }
    }

    [Trait("Category", "Arrays")]
    public class BlockSparseArrayTests1000 : BlockSparseArrayTests
    {
        public BlockSparseArrayTests1000() : base(1000) { }
    }

    [Trait("Category", "Arrays")]
    public class BinarySparseArrayTests
        : AbstractSparseArrayContractTests
    {
        public override ISparseArray<int> CreateInstance(int length)
        {
            return new BinarySparseArray<int>(length);
        }
    }
}
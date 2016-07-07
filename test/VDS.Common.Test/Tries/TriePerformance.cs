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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace VDS.Common.Tries
{
    public class TriePerformance
    {
        private static List<String> GenerateWords(int numWords, int minLength, int maxLength)
        {
            char[] pool = Enumerable.Range('a', 26).Select(c => (char)c).ToArray();
            return GenerateWords(numWords, pool, minLength, maxLength);
        }

        private static List<string> GenerateWords(int numWords, char[] pool, int minLength, int maxLength)
        {
            List<String> words = new List<string>();

            Random random = new Random();
            while (words.Count < numWords)
            {
                StringBuilder builder = new StringBuilder();

                // Select word length
                int wordLength = random.Next(minLength, maxLength + 1);

                // Fill word with random characters from the pool
                while (builder.Length < wordLength)
                {
                    builder.Append(pool[random.Next(pool.Length)]);
                }
                words.Add(builder.ToString());
            }

            return words;
        }

        private static List<TrieFiller<String, char>> CreateFillers(List<List<String>> threadWords, ITrie<String, char, String> trie)
        {
            List<TrieFiller<String, char>> fillers = new List<TrieFiller<String, char>>();
            foreach (List<String> threadWordList in threadWords)
            {
                fillers.Add(new TrieFiller<string, char>(trie, threadWordList));
            }
            return fillers;
        }

        private static List<List<string>> CreateThreadWordLists(List<string> words, int numThreads, int repeatsPerWord, int threadsPerWord, bool requireDistinctThreads)
        {
            if (threadsPerWord > numThreads) throw new ArgumentException("threadsPerWord must be <= numThreads", "threadsPerWord");

            List<List<String>> threadWords = new List<List<string>>();
            for (int i = 0; i < numThreads; i++)
            {
                threadWords.Add(new List<string>());
            }

            Random random = new Random();
            foreach (String word in words)
            {
                for (int i = 0; i < repeatsPerWord; i++)
                {
                    if (threadsPerWord == numThreads && requireDistinctThreads)
                    {
                        // Add to all threads
                        foreach (List<String> threadWordList in threadWords)
                        {
                            threadWordList.Add(word);
                        }
                        continue;
                    }

                    // Add to one thread at random
                    int thread = random.Next(threadWords.Count);
                    threadWords[thread].Add(word);

                    if (threadsPerWord == 1) continue;

                    // At to further threads at random
                    if (!requireDistinctThreads)
                    {
                        // Threads are not required to be distinct so add to any threads at random
                        int added = 1;
                        while (added < threadsPerWord)
                        {
                            thread = random.Next(threadWords.Count);
                            threadWords[thread].Add(word);
                            added++;
                        }
                    }
                    else
                    {
                        // Threads are required to be distinct so track which threads we have added to
                        HashSet<int> selectedThreads = new HashSet<int>();
                        selectedThreads.Add(thread);
                        while (selectedThreads.Count < threadsPerWord)
                        {
                            thread = random.Next(threadWords.Count);
                            if (!selectedThreads.Add(thread)) continue;
                            threadWords[thread].Add(word);
                        }
                    }
                }
            }
            return threadWords;
        }

        private static void RunFillers(List<TrieFiller<String, char>> fillers)
        {
            // Launch threads
            List<Thread> threads = new List<Thread>();
            foreach (TrieFiller<String, char> filler in fillers)
            {
                ThreadStart start = new ThreadStart(filler.Run);
                Thread t = new Thread(start);
                t.Start();
                threads.Add(t);
            }

            // Wait for threads to finish
            while (threads.Count > 0)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    TrieFiller<String, char> filler = fillers[i];
                    if (filler.IsFinished) threads.RemoveAt(i);
                    i--;
                }
                Thread.Sleep(50);
            }

            // Report Performance
            foreach (TrieFiller<String, char> filler in fillers)
            {
                Console.Write(filler.HasError ? "[Error]" : "[Success]");
                Console.WriteLine(" Added {0} objects and Read {1} in {2}", filler.Added, filler.Read, filler.Elapsed);
            }
        }

        [Theory]
        [InlineData(100, 15, 100, 1, 1, 1, true),
         InlineData(100, 15, 100, 2, 1, 1, true),
         InlineData(100, 15, 100, 4, 1, 2, true),
         InlineData(100, 15, 100, 4, 1, 4, true),
         InlineData(100, 15, 100, 4, 2, 2, true),
         InlineData(100, 15, 100, 4, 2, 4, true),
         InlineData(100, 15, 100, 4, 8, 2, true),
         InlineData(100, 15, 100, 4, 8, 4, true),
         InlineData(1000, 15, 100, 2, 1, 1, true),
         InlineData(1000, 15, 100, 4, 1, 2, true),
         InlineData(1000, 15, 100, 4, 1, 4, true),
         InlineData(100, 15, 100, 2, 1, 1, false),
         InlineData(100, 15, 100, 4, 1, 2, false),
         InlineData(100, 15, 100, 4, 1, 4, false),
         InlineData(1000, 15, 100, 2, 1, 1, false),
         InlineData(1000, 15, 100, 4, 1, 2, false),
         InlineData(1000, 15, 100, 4, 1, 4, false),
         InlineData(1000, 15, 100, 2, 8, 1, false),
         InlineData(1000, 15, 100, 4, 8, 2, false),
         InlineData(1000, 15, 100, 4, 8, 4, false)]
        public void TrieMultiThreadedPerformance(int numWords, int minLength, int maxLength, int numThreads, int repeatsPerWord, int threadsPerWord, bool requireDistinct)
        {
            List<String> words = GenerateWords(numWords, minLength, maxLength);
            List<List<String>> threadWords = CreateThreadWordLists(words, numThreads, repeatsPerWord, threadsPerWord, requireDistinct);

            Console.WriteLine("StringTrie:");
            List<TrieFiller<String, char>> fillers = CreateFillers(threadWords, new StringTrie<string>());
            RunFillers(fillers);
            Console.WriteLine();
            Console.WriteLine("SparseStringTrie:");
            fillers = CreateFillers(threadWords, new SparseStringTrie<string>());
            RunFillers(fillers);
        }
    }

    class TrieFiller<TKey, TKeyBit>
        where TKey : class
    {
        private readonly ITrie<TKey, TKeyBit, TKey> _trie;
        private readonly Queue<TKey> _keysToInsert = new Queue<TKey>(); 
        private readonly Queue<TKey> _keysToRead = new Queue<TKey>();
        private readonly Stopwatch _timer = new Stopwatch();

        public TrieFiller(ITrie<TKey, TKeyBit, TKey> trie, IEnumerable<TKey> keysToInsert)
        {
            this._trie = trie;
            foreach (TKey key in keysToInsert)
            {
                this._keysToInsert.Enqueue(key);
            }
        }

        public void Run()
        {
            this.Error = null;
            this.IsFinished = false;
            this.Added = 0;
            this.Read = 0;
            this._timer.Stop();
            this._timer.Reset();
            this._timer.Start();

            try
            {
                while (this._keysToInsert.Count > 0)
                {
                    TKey key = this._keysToInsert.Dequeue();
                    this._trie.Add(key, key);
                    this.Added++;
                    this._keysToRead.Enqueue(key);
                }
                while (this._keysToRead.Count > 0)
                {
                    TKey key = this._keysToRead.Dequeue();
                    TKey value = this._trie[key];
                    this.Read++;
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            finally
            {
                this._timer.Stop();
                this.IsFinished = true;
            }
        }

        public bool HasError { get { return this.Error != null; } }

        public Exception Error { get; private set; }

        public int Added { get; private set; }

        public int Read { get; private set; }

        public TimeSpan Elapsed { get { return this._timer.Elapsed; } }

        public bool IsFinished { get; private set; }
    }
}

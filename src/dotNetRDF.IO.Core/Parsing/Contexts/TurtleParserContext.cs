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
using VDS.RDF.Graphs;
using VDS.RDF.Parsing.Tokens;
using VDS.RDF.Specifications;

namespace VDS.RDF.Parsing.Contexts
{
    /// <summary>
    /// Parser Context for Turtle parsing
    /// </summary>
    public class TurtleParserContext
        : TokenisingParserContext
    {
        private readonly TurtleSyntax _syntax = TurtleSyntax.W3C;

        /// <summary>
        /// Creates a new Turtle Parser Context with default settings
        /// </summary>
        /// <param name="handler">RDF Handler</param>
        /// <param name="tokeniser">Tokeniser to use</param>
        /// <param name="syntax">Turtle Syntax</param>
        public TurtleParserContext(IRdfHandler handler, ITokeniser tokeniser, TurtleSyntax syntax, IParserProfile profile)
            : this(handler, tokeniser, syntax, TokenQueueMode.SynchronousBufferDuringParsing, false, false, profile) { }

        /// <summary>
        /// Creates a new Turtle Parser Context with custom settings
        /// </summary>
        /// <param name="handler">RDF Handler</param>
        /// <param name="tokeniser">Tokeniser to use</param>
        /// <param name="syntax">Turtle Syntax</param>
        /// <param name="queueMode">Tokeniser Queue Mode</param>
        public TurtleParserContext(IRdfHandler handler, ITokeniser tokeniser, TurtleSyntax syntax, TokenQueueMode queueMode, IParserProfile profile)
            : this(handler, tokeniser, syntax, queueMode, false, false, profile) { }

        /// <summary>
        /// Creates a new Turtle Parser Context with custom settings
        /// </summary>
        /// <param name="handler">RDF Handler</param>
        /// <param name="tokeniser">Tokeniser to use</param>
        /// <param name="syntax">Turtle Syntax</param>
        /// <param name="traceParsing">Whether to trace parsing</param>
        /// <param name="traceTokeniser">Whether to trace tokenisation</param>
        public TurtleParserContext(IRdfHandler handler, ITokeniser tokeniser, TurtleSyntax syntax, bool traceParsing, bool traceTokeniser, IParserProfile profile)
            : this(handler, tokeniser, syntax, TokenQueueMode.SynchronousBufferDuringParsing, traceParsing, traceTokeniser, profile) { }

        /// <summary>
        /// Creates a new Turtle Parser Context with custom settings
        /// </summary>
        /// <param name="handler">RDF Handler</param>
        /// <param name="tokeniser">Tokeniser to use</param>
        /// <param name="syntax">Turtle Syntax</param>
        /// <param name="queueMode">Tokeniser Queue Mode</param>
        /// <param name="traceParsing">Whether to trace parsing</param>
        /// <param name="traceTokeniser">Whether to trace tokenisation</param>
        public TurtleParserContext(IRdfHandler handler, ITokeniser tokeniser, TurtleSyntax syntax, TokenQueueMode queueMode, bool traceParsing, bool traceTokeniser, IParserProfile profile)
            : base(handler, tokeniser, queueMode, traceParsing, traceTokeniser, profile)
        {
            this._syntax = syntax;
        }

        /// <summary>
        /// Gets the Turtle Syntax being used
        /// </summary>
        public TurtleSyntax Syntax
        {
            get
            {
                return this._syntax;
            }
        }

        /// <summary>
        /// Function for unescaping QNames
        /// </summary>
        public Func<String, String> QNameUnescapeFunction
        {
            get
            {
                switch (this._syntax)
                {
                    case TurtleSyntax.W3C:
                        return TurtleSpecsHelper.UnescapeQName;
                    default:
                        return s => s;
                }
            }
        }
    }
}

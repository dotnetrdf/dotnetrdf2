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
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Query.Paths
{
    /// <summary>
    /// Abstract Base Class for Unary Path operators
    /// </summary>
    public abstract class BaseUnaryPath 
        : IPath
    {
        /// <summary>
        /// Path
        /// </summary>
        protected IPath _path;

        /// <summary>
        /// Creates a new Unary Path
        /// </summary>
        /// <param name="path">Path</param>
        protected BaseUnaryPath(IPath path)
        {
            this._path = path;
        }

        /// <summary>
        /// Gets the Inner Path
        /// </summary>
        public IPath Path
        {
            get
            {
                return this._path;
            }
        }

        public abstract bool IsTerminal { get; }

        public abstract bool IsFixedLength { get; }

        /// <summary>
        /// Gets the String representation of the Path
        /// </summary>
        /// <returns></returns>
        public sealed override String ToString()
        {
            return ToString(new AlgebraFormatter());
        }

        public abstract String ToString(IAlgebraFormatter formatter);
    }
}

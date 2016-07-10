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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using VDS.RDF.Attributes;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("dotNetRDF.IO.Json")]
[assembly: AssemblyDescription("JSON IO APIs for dotNetRDF")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Visual Design Studios")]
[assembly: AssemblyProduct("dotNetRDF.IO.Json")]
[assembly: AssemblyCopyright("Copyright © dotNetRDF Project 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("84020dd1-34b8-4e69-a229-b5886625a207")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.9.0.0")]
[assembly: AssemblyVersion("2.0.0.*")]
[assembly: AssemblyFileVersion("2.0.0.0")]

[assembly: RdfIO(SyntaxName = "RDF/JSON", Encoding = "utf-8", CanonicalMimeType = "application/rdf+json", MimeTypes = new string[]{ "application/json", "text/json" }, CanonicalFileExtension = ".rj", ParserType = typeof(RdfJsonParser), WriterType = typeof(RdfJsonWriter))]
[assembly: RdfIO(SyntaxName = "GZipped RDF/JSON", Encoding = "utf-8", CanonicalMimeType = "application/rdf+json", MimeTypes = new string[] { "application/json", "text/json" }, CanonicalFileExtension = ".rj.gz", ParserType = typeof(GZippedRdfJsonParser), WriterType = typeof(GZippedRdfJsonWriter))]


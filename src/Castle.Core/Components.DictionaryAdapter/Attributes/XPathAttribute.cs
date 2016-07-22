// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class XPathAttribute : Attribute
	{
		private readonly CompiledXPath getPath;
		private readonly CompiledXPath setPath;

		public XPathAttribute(string path)
		{
			if (path == null)
				throw Error.ArgumentNull("path");

			this.getPath = XPathCompiler.Compile(path);
			this.setPath = this.getPath;
		}

		public XPathAttribute(string get, string set)
		{
			if (get == null)
				throw Error.ArgumentNull("get");
			if (set == null)
				throw Error.ArgumentNull("set");

			this.getPath = XPathCompiler.Compile(get);
			this.setPath = XPathCompiler.Compile(set);
		}

		public CompiledXPath GetPath
		{
			get { return getPath; }
		}

		public CompiledXPath SetPath
		{
			get { return setPath; }
		}

		public bool Nullable { get; set; }
	}
}
#endif

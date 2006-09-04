// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook
{
	using System;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// 
	/// </summary>
	public sealed class CompilationContext
	{
		private ArrayList _sources = new ArrayList();
		private Hashtable _properties = new Hashtable();

		public CompilationContext(TextReader source)
		{
			_sources.Add(source);
		}

		public TextReader[] Sources
		{
			get { return (TextReader[]) _sources.ToArray( typeof(TextReader) ); }
		}

		public object this [String name]
		{
			get { return _properties[name]; }
			set { _properties[name] = value; }
		}
	}
}

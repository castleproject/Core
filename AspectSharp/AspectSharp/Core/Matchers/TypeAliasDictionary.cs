 // Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Core.Matchers
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for TypeAliasDictionary.
	/// </summary>
	public sealed class TypeAliasDictionary
	{
		private static readonly TypeAliasDictionary _instance = new TypeAliasDictionary();

		private Hashtable _map = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);

		private TypeAliasDictionary()
		{
			_map["int"] = typeof (int).FullName;
			_map["string"] = typeof (string).FullName;
			_map["float"] = typeof (float).FullName;
			_map["double"] = typeof (double).FullName;
			_map["byte"] = typeof (byte).FullName;
			_map["long"] = typeof (long).FullName;
			_map["short"] = typeof (Int16).FullName;
			_map["int32"] = typeof (Int32).FullName;
			_map["int64"] = typeof (Int64).FullName;
			_map["single"] = typeof (Single).FullName;
			_map["single"] = typeof (Single).FullName;
		}

		public String this[String name]
		{
			get { return _map[name] as String; }
		}

		public static TypeAliasDictionary Instance
		{
			get { return _instance; }
		}
	}
}
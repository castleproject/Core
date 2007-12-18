// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using System;
using System.Text;

namespace Castle.MonoRail.Views.Brail
{
	using System.Collections;
	using Castle.MonoRail.Framework;

	public class BooViewEngineOptions
	{
		bool debug;
		bool batchCompile;
		bool saveToDisk;
		string commonScriptsDirectory = "CommonScripts";
		string saveDirectory = "Brail_Generated_Code";
	    readonly IList assembliesToReference = new ArrayList();
	    readonly IList namespacesToImport = new ArrayList();

		public bool Debug
		{
			get { return debug; }
			set { debug = value; }
		}

		public bool SaveToDisk
		{
			get { return saveToDisk; }
			set { saveToDisk = value; }
		}

		public bool BatchCompile
		{
			get { return batchCompile; }
			set { batchCompile = value; }
		}

		public string CommonScriptsDirectory
		{
			get { return commonScriptsDirectory; }
			set { commonScriptsDirectory = value; }
		}

		public string SaveDirectory
		{
			get { return saveDirectory; }
			set { saveDirectory = value; }
		}

		public IList AssembliesToReference
		{
			get { return assembliesToReference; }
		}

		public IList NamespacesToImport
		{
			get { return namespacesToImport; }
		}

		public BooViewEngineOptions()
		{
			AssembliesToReference.Add(typeof(BooViewEngineOptions).Assembly);//Brail's assembly
			AssembliesToReference.Add(typeof(Controller).Assembly);//MonoRail.Framework's assembly
            AssembliesToReference.Add(typeof(Boo.Lang.Extensions.AssertMacro).Assembly);//Boo.Lang.Extensions assembly
		}
	}
}

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

namespace Castle.CastleOnRails.Generator.Generators
{
	using System;
	using System.IO;
	using System.Collections;


	/// <summary>
	/// 
	/// </summary>
	public class ProjectGenerator : IGenerator
	{
		public ProjectGenerator()
		{
		}

		#region IGenerator Members

		public bool Accept(String name, IDictionary options, TextWriter writer)
		{
			if (!"project".Equals(name))
			{
				return false;
			}
			else if (options.Count == 1)
			{
				writer.WriteLine("Creates a new VS.Net 2003 project structure");
				writer.WriteLine("");
				writer.WriteLine("name     : Project name");
				writer.WriteLine("outdir   : Target directory (must exists)");
				writer.WriteLine("windsor  : [Optional] Enable WindsorContainer Integration");
				writer.WriteLine("view     : [Optional] aspnet|nvelocity (defaults to nvelocity)");
				writer.WriteLine("lang     : [Optional] c#|vb.net (defaults to c#)");
				writer.WriteLine("");
				writer.WriteLine("Example:");
				writer.WriteLine("");
				writer.WriteLine("> generator project name:My.CoR.Project outdir:c:\temp");
				writer.WriteLine("");

				return false;
			}
			else if (!options.Contains("outdir"))
			{
				writer.WriteLine("outdir must be specified");
			}
			else if (!options.Contains("name"))
			{
				writer.WriteLine("name must be specified");
			}
			else 
			{
				DirectoryInfo info = new DirectoryInfo(options["outdir"] as String);

				if (!info.Exists)
				{
					// info.Create(); // Is it safe to use it?
					writer.WriteLine("Error: The specified outdir does not exists.");
				}
			}

			return true;
		}

		public void Execute(String name, IDictionary options, TextWriter writer)
		{
			// Steps to create the project:

			// 1. Create a controllers and views Directory
			// 2. Create a proper web.config
			// 3. Create a build file?
			// 4. Create the sln and the proper csproj (references to assemblies)
		}

		#endregion
	}
}

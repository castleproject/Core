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

namespace Castle.MonoRail.Generator
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Generator.Generators;


	/// <summary>
	/// 
	/// </summary>
	public abstract class App
	{
		public static void Main()
		{
			IGenerator[] generators = CreateGenerators();

			ShowHeader();

			String[] args = Environment.GetCommandLineArgs();

			if (args.Length == 1)
			{
				ShowUsage();
				return;
			}
			else
			{
				IDictionary dic = BuildDictionary(args);
//				bool found = false;

				foreach(IGenerator generator in generators)
				{
					if (generator.Accept(args[1], dic, Console.Out))
					{
//						found = true;
						generator.Execute(dic, Console.Out);
						break;
					}
				}

//				if (!found)
//				{
//					Console.Out.WriteLine("No generator found for '{0}'", args[1]);
//				}
			}
		}

		private static IDictionary BuildDictionary(String[] args)
		{
			HybridDictionary dic = new HybridDictionary(true);

			for(int i=1; i < args.Length; i++)
			{
				String key = args[i];
				String value = String.Empty;

				int index = key.IndexOf(':');

				if (index != -1)
				{
					key = args[i].Substring(0, index);
					value = args[i].Substring(index+1);
					value = NormalizeValue(value);
				}

				dic[key] = value;
			}

			return dic;
		}

		private static void ShowUsage()
		{
			Console.WriteLine("Usage: generator action options");
			Console.WriteLine("");
			Console.WriteLine("Actions:");
			Console.WriteLine("  project");
			Console.WriteLine("  controller");
			Console.WriteLine("");
			Console.WriteLine("Try 'generator action' for more information about the action");
		}

		private static void ShowHeader()
		{
			Console.WriteLine("MonoRail Generator - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() );
			Console.WriteLine("Released under Apache Software License 2.0");
			Console.WriteLine("Copyright (c) 2004-2006 the original author/authors");
			Console.WriteLine("http://www.castleproject.org/");
			Console.WriteLine("");
			Console.WriteLine("");
		}

		private static IGenerator[] CreateGenerators()
		{
			return new IGenerator[] { new ProjectGenerator(), new ControllerGenerator() };
		}

		private static string NormalizeValue(string value)
		{
			// TODO: remove quotes if present
			return value;
		}
	}
}

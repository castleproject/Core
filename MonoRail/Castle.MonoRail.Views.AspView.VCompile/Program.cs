// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.VCompile
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Xml;

	using AspView;
	using Compiler;
	using Compiler.Adapters;
	using Compiler.Factories;

	public class VCompile
	{
		static Arguments arguments;

		static void Main()
		{
			try
			{
				ParseCommandLineArguments();
				SetDefaultsForArguments();
				ValidateEnvironment();

				Console.WriteLine("Compiling [" + arguments.SiteRoot + "] ...");

				AspViewEngineOptions options = InitializeConfig();

				ICompilationContext compilationContext = CreateCompilationContext(options.CompilerOptions.TemporarySourceFilesDirectory);

				OfflineCompiler compiler = new OfflineCompiler(
					new CSharpCodeProviderAdapterFactory(),
					new PreProcessor(),
					compilationContext, options.CompilerOptions, new DefaultFileSystemAdapter());

				string path = compiler.Execute();

				Console.WriteLine("[{0}] compiled into [{1}].", arguments.SiteRoot, path);
			}
			catch (ConfigurationErrorsException ce)
			{
				PrintHelpMessage(ce.Message);
				Environment.Exit(-2);
			}
			catch (ArgumentException ae)
			{
				PrintHelpMessage(ae.Message);
				Environment.Exit(-3);
			}
			catch (ApplicationException ae)
			{
				PrintHelpMessage(ae.Message);
				Environment.Exit(-4);
			}
			catch (Exception ex)
			{
				PrintHelpMessage("Could not compile." + Environment.NewLine + ex);
				Environment.Exit(-1);
			}

		}

		static ICompilationContext CreateCompilationContext(string temporarySourceFilesDirectory)
		{
			return new CompilationContext(
				new DirectoryInfo(Path.Combine(arguments.SiteRoot, "bin")),
				new DirectoryInfo(arguments.SiteRoot),
				new DirectoryInfo(arguments.ViewPathRoot),
				new DirectoryInfo(temporarySourceFilesDirectory));
		}

		static void ValidateEnvironment()
		{
			if (Directory.Exists(arguments.SiteRoot) == false)
			{
				throw new ArgumentException(string.Format("The site root path '{0}' does not exist", arguments.SiteRoot));
			}

			if (Directory.Exists(arguments.ViewPathRoot) == false)
			{
				throw new ArgumentException(string.Format("The views folder path '{0}' does not exist", arguments.ViewPathRoot));
			}
		}

		static void SetDefaultsForArguments()
		{
			if (arguments.SiteRoot == null)
			{
				DirectoryInfo runningDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
				DirectoryInfo siteRootDirectoryInfo = runningDirectoryInfo.Parent;
				if (siteRootDirectoryInfo == null)
				{
					throw new ApplicationException("Site root was not specified and could not be guessed");
				}

				arguments.SiteRoot = siteRootDirectoryInfo.FullName;
			}

			if (arguments.ViewPathRoot == null)
			{
				arguments.ViewPathRoot = Path.Combine(arguments.SiteRoot, "views");
			}
			else if (Path.IsPathRooted(arguments.ViewPathRoot) == false)
			{
				arguments.ViewPathRoot = Path.Combine(arguments.SiteRoot, arguments.ViewPathRoot);
			}
		}

		static string GetArgumentValueFrom(string[] parts)
		{
			string[] valueParts = new string[parts.Length - 1];
			Array.Copy(parts, 1, valueParts, 0, valueParts.Length);
			return string.Join(":", valueParts).Trim().Trim('"');
		}

		static void ParseCommandLineArguments()
		{
			arguments = new Arguments();

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length == 1)
				return;
			for (int i = 1; i < args.Length; ++i)
			{
				string[] parts = args[i].Split(':');

				switch (parts[0].ToLowerInvariant())
				{
					case "/w":
					case "/wait":
						arguments.Wait = true;
						Console.ReadLine();
						break;
					case "/r":
					case "/siteroot":
						if (parts.Length == 1)
							throw new ArgumentException("Missing site root - the '/r' argument was given without a value");
						arguments.SiteRoot = Path.GetFullPath(GetArgumentValueFrom(parts));
						break;
					case "/v":
					case "/viewpathroot":
						if (parts.Length == 1)
							throw new ArgumentException("Missing view path root - the '/v' argument was given without a value");
						arguments.ViewPathRoot = GetArgumentValueFrom(parts);
						break;
					default:
						throw new ArgumentException(string.Format("Unknown argument [{0}]", parts[0]));
				}
			}
		}

		private static void PrintHelpMessage(string message)
		{
			if (message != null)
			{
				Console.WriteLine("message from the compiler:");
				Console.WriteLine(message);
			}
			Console.WriteLine(@"
usage: vcompile [options]

valid options:
    /siteRoot:SITEPATH      -> will compile the site at SITEPATH directory
                               defaults to the current directory's parent
    /r:SITEPATH             -> same as /siteroot:SITEPATH

    /viewPathRoot:VIEWSPATH -> will read views from VIEWSPATHROOT
                               defaults to 'Views' folder under the site root
    /v:VIEWSPATH            -> same as /viewPathRoot:VIEWSPATH    

    /wait                   -> will wait for 'Enter' to give the user a chance
                               to attach a debugger
    /w                      -> same as /wait

examples:
    vcompile /r:C:\Dev\Sites\MySite    
        will compile the site at 'C:\Dev\Sites\MySite' directory, reading
        view templates from 'C:\Dev\Sites\MySite\Views'

    vcompile /v:Plugins\Plugin1\Views
        assuming current directory is 'C:\Dev\Sites\MySite\Bin',
        will compile the site at 'C:\Dev\Sites\MySite' directory, reading
        view templates from 'C:\Dev\Sites\MySite\Plugins\Plugin1\Views'

    vcompile /r:C:\Dev\Sites\MySite /v:C:\Plugins\Plugin1\Views
        will compile the site at 'C:\Dev\Sites\MySite' directory, reading
        view templates from 'C:\Plugins\Plugin1\Views'
");

		}

		private static AspViewEngineOptions InitializeConfig()
		{
			AspViewEngineOptions options =
				InitializeConfig("aspView") ??
				InitializeConfig("aspview") ??
				new AspViewEngineOptions();

			Console.WriteLine(options.CompilerOptions.Debug ? "Compiling in DEBUG mode" : "");

			return options;
		}

		private static AspViewEngineOptions InitializeConfig(string configName)
		{
			string path = Path.Combine(arguments.SiteRoot, "web.config");
			if (!File.Exists(path))
			{
				Console.WriteLine("Cannot locate web.config" + Environment.NewLine +
								  "VCompile should run from the bin directory of the website");
				Environment.Exit(1);
			}
			XmlNode aspViewNode;
			using (XmlTextReader reader = new XmlTextReader(path))
			{
				reader.Namespaces = false;
				XmlDocument xml = new XmlDocument();
				xml.Load(reader);
				aspViewNode = xml.SelectSingleNode("/configuration/" + configName);
			}

			if (aspViewNode == null)
			{
				return null;
			}

			AspViewConfigurationSection section = new AspViewConfigurationSection();
			return (AspViewEngineOptions)section.Create(null, null, aspViewNode);
		}
	}
}
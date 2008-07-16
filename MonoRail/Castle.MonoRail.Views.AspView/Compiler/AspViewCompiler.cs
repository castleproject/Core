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

namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System;
	using System.Text;
	using System.IO;
	using System.Collections.Generic;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
	using System.Reflection;
	using System.Configuration;
	using System.Security;
	using PreCompilationSteps;

	public class AspViewCompiler
	{
		#region members
		private CompilerParameters parameters;
		readonly AspViewCompilerOptions options;
		private static readonly ReferencedAssembly[] defaultAddedReferences = null;
		private static readonly string defaultSiteRoot = AppDomain.CurrentDomain.BaseDirectory;
		const string AssemblyAttributeAllowPartiallyTrustedCallers = "[assembly: System.Security.AllowPartiallyTrustedCallers]";
		const string AllowPartiallyTrustedCallersFileName = "AllowPartiallyTrustedCallers.generated.cs";

		private string pathToAssembly = null;
		private Assembly assembly = null;

		#endregion

		#region c'tor
		public AspViewCompiler(AspViewCompilerOptions options)
		{
			Resolve.Initialize(options.CustomProviders);
			this.options = options;
			InitializeCompilerParameters();
		}
		#endregion

		#region properties
		public string PathToAssembly { get { return pathToAssembly; } }
		public Assembly Assembly { get { return assembly; } }
		#endregion

		public void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files)
		{
			IEnumerable<IPreCompilationStep> preCompilationSteps =
				Resolve.PreCompilationStepsProvider.GetSteps();

			foreach (SourceFile file in files)
				foreach (IPreCompilationStep step in preCompilationSteps)
					step.Process(file);
		}

		public void CompileSite()
		{
			CompileSite(defaultSiteRoot);
		}

		public void CompileSite(string siteRoot)
		{
			CompileSite(siteRoot, defaultAddedReferences);
		}

		public void CompileSite(string siteRoot, ReferencedAssembly[] references)
		{
			List<SourceFile> files = GetSourceFiles(siteRoot ?? defaultSiteRoot);
			if (files.Count == 0)
				return;

			ApplyPreCompilationStepsOn(files);

			string targetDirectory = Path.Combine(siteRoot, "Bin");

			string targetTemporarySourceFilesDirectory = GetTargetTemporarySourceFilesDirectory(targetDirectory);

			if (options.KeepTemporarySourceFiles)
			{
				DeleteFilesIn(targetTemporarySourceFilesDirectory);

				foreach (SourceFile file in files)
					SaveFile(file, targetTemporarySourceFilesDirectory);

				if (options.AllowPartiallyTrustedCallers)
					SaveAllowPartiallyTrustedCallersFileTo(targetTemporarySourceFilesDirectory);
			}

			if (!parameters.GenerateInMemory)
				parameters.OutputAssembly = Path.Combine(targetDirectory, "CompiledViews.dll");

			List<ReferencedAssembly> actualReferences = new List<ReferencedAssembly>();
			if (options.References != null)
				actualReferences.AddRange(options.References);
			if (references != null)
				actualReferences.AddRange(references);

			foreach (ReferencedAssembly reference in actualReferences)
			{
				string assemblyName = reference.Name;
				if (reference.Source == ReferencedAssembly.AssemblySource.BinDirectory)
					assemblyName = Path.Combine(targetDirectory, assemblyName);
				parameters.CompilerOptions += " /r:\"" + assemblyName + "\"";
			}

			CompilerResults results;
			CodeDomProvider codeProvider;
			try
			{
				codeProvider = CodeDomProvider.GetCompilerInfo("csharp").CreateProvider();
			}
			catch (SecurityException)
			{
				codeProvider = new CSharpCodeProvider();
			}
			catch (ConfigurationException)
			{
				codeProvider = new CSharpCodeProvider();
			}

			if (options.KeepTemporarySourceFiles)
			{
				results = codeProvider.CompileAssemblyFromFile(parameters, Directory.GetFiles(targetTemporarySourceFilesDirectory, "*.cs", SearchOption.TopDirectoryOnly));
			}
			else
			{
				string[] sources = GetSourcesFrom(files);

				results = codeProvider.CompileAssemblyFromSource(parameters, sources);
			}

			ThrowIfErrorsIn(results, files);

			if (options.AutoRecompilation)
				assembly = results.CompiledAssembly;
			else
				pathToAssembly = results.PathToAssembly;

		}

		public static List<SourceFile> GetSourceFiles(string siteRoot)
		{
			List<SourceFile> files = new List<SourceFile>();
			string viewsDirectory = Path.Combine(siteRoot, "Views");

			if (!Directory.Exists(viewsDirectory))
				throw new Exception(string.Format("Could not find views folder [{0}]", viewsDirectory));

			string[] fileNames = Directory.GetFiles(viewsDirectory, "*.aspx", SearchOption.AllDirectories);
			foreach (string fileName in fileNames)
			{
				SourceFile file = new SourceFile();
				file.ViewName = fileName.Replace(viewsDirectory, "");
				file.ClassName = AspViewEngine.GetClassName(file.ViewName);
				file.ViewSource = ReadFile(fileName);
				file.RenderBody = file.ViewSource;
				files.Add(file);
			}
			return files;
		}

		#region helpers

		private string[] GetSourcesFrom(ICollection<SourceFile> files)
		{
			List<string> sources=new List<string>(files.Count);
			foreach (SourceFile file in files)
				sources.Add(file.ConcreteClass);

			if (options.AllowPartiallyTrustedCallers)
			{
				sources.Add(AssemblyAttributeAllowPartiallyTrustedCallers);
			}

			return sources.ToArray();
		}

		private string GetTargetTemporarySourceFilesDirectory(string targetDirectory)
		{
			string targetTemporarySourceFilesDirectory = options.TemporarySourceFilesDirectory;
			if (!Path.IsPathRooted(targetTemporarySourceFilesDirectory))
				targetTemporarySourceFilesDirectory = Path.Combine(targetDirectory, targetTemporarySourceFilesDirectory);
			if (!Directory.Exists(targetTemporarySourceFilesDirectory))
				Directory.CreateDirectory(targetTemporarySourceFilesDirectory);
			return targetTemporarySourceFilesDirectory;
		}

		#region IO
		private static string ReadFile(string fileName)
		{
			return File.ReadAllText(fileName);
		}

		private static void SaveFile(SourceFile file, string targetDirectory)
		{
			string fileName = Path.Combine(targetDirectory, file.FileName);
			File.WriteAllText(fileName, file.ConcreteClass, Encoding.UTF8); 
		}

		private static void DeleteFilesIn(string directory)
		{
			foreach (string fileName in Directory.GetFiles(directory, "*.cs", SearchOption.TopDirectoryOnly))
			{
				File.Delete(fileName);
			}
		}

		private static void SaveAllowPartiallyTrustedCallersFileTo(string directory)
		{
			string fileName = Path.Combine(directory, AllowPartiallyTrustedCallersFileName);

			string content = @"// This file was generated by a tool
// Casle.MonoRail.Views.AspView compiler, version
" + AssemblyAttributeAllowPartiallyTrustedCallers;

			File.WriteAllText(fileName, content);
		}
		#endregion

		#region Compiler related
		private void InitializeCompilerParameters()
		{
			parameters = new CompilerParameters();
			parameters.GenerateInMemory = options.AutoRecompilation;
			parameters.GenerateExecutable = false;
			parameters.IncludeDebugInformation = options.Debug;
		}

		private void ThrowIfErrorsIn(CompilerResults results, IEnumerable<SourceFile> files)
		{
			if (results.Errors.Count > 0)
			{
				StringBuilder message = new StringBuilder();
				CodeDomProvider cSharpCodeProvider;
				try
				{
					cSharpCodeProvider = CodeDomProvider.GetCompilerInfo("csharp").CreateProvider();
				}
				catch (SecurityException)
				{
					cSharpCodeProvider = new CSharpCodeProvider();
				}
				catch (ConfigurationException)
				{
					cSharpCodeProvider = new CSharpCodeProvider();
				}

				try
				{
					foreach (SourceFile file in files)
					{
						CompilerResults result = cSharpCodeProvider.CompileAssemblyFromSource(parameters, file.ConcreteClass);
						if (result.Errors.Count > 0)
							foreach (CompilerError err in result.Errors)
								message.AppendLine(string.Format(@"
On '{0}' (class name: {1}) Line {2}, Column {3}, {4} {5}:
{6}
========================================",
								file.ViewName,
								file.ClassName,
								err.Line,
								err.Column,
								err.IsWarning ? "Warning" : "Error",
								err.ErrorNumber,
								err.ErrorText));
					}
				}
				finally
				{
					cSharpCodeProvider.Dispose();
				}
				throw new Exception("Error while compiling views: " + message);
			}
		}
		#endregion

		#endregion

	}
}

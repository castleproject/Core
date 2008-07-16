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
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;

	using Adapters;
	using Factories;

	public class OfflineCompiler : AbstractCompiler
	{
		const string AllowPartiallyTrustedCallersFileName = "AllowPartiallyTrustedCallers.generated.cs";
		readonly IFileSystemAdapter fileSystem;

		public OfflineCompiler(ICodeProviderAdapterFactory codeProviderAdapterFactory, IPreProcessor preProcessor, ICompilationContext context, AspViewCompilerOptions options, IFileSystemAdapter fileSystem)
			: base(codeProviderAdapterFactory, preProcessor, context, options)
		{
			this.fileSystem = fileSystem;
			parameters.GenerateInMemory = false;
			parameters.OutputAssembly = Path.Combine(context.BinFolder.FullName, "CompiledViews.dll");
		}

		public string Execute()
		{
			CompilerResults compilerResults = InternalExecute();
			if (compilerResults == null)
				return null;

			return compilerResults.PathToAssembly;
		}

		protected override void AfterPreCompilation(List<SourceFile> files)
		{
			if (options.KeepTemporarySourceFiles)
				KeepTemporarySourceFiles(files);
		}

		protected override CompilerResults GetResultsFrom(List<SourceFile> files)
		{
			string pdbFileName = parameters.OutputAssembly.Substring(0, parameters.OutputAssembly.Length - 3) + "pdb";
			fileSystem.Delete(parameters.OutputAssembly);
			fileSystem.Delete(pdbFileName);

			if (options.KeepTemporarySourceFiles)
			{
				string[] sourceFiles = files.ConvertAll<string>(SourceFileToFileName).ToArray();

				return codeProvider.CompileAssemblyFromFile(parameters, sourceFiles);
			}
			string[] sources = files.ConvertAll<string>(SourceFileToSource).ToArray();

			return codeProvider.CompileAssemblyFromSource(parameters, sources);
		}

		void KeepTemporarySourceFiles(List<SourceFile> files)
		{
				SetupTemporarySourceFilesDirectory();

				foreach (SourceFile file in files)
					fileSystem.Save(file.FileName, file.ConcreteClass, context.TemporarySourceFilesDirectory);

				if (options.AllowPartiallyTrustedCallers)
				{
					fileSystem.Save(
						AllowPartiallyTrustedCallersFileName,
						GetAllowPartiallyTrustedCallersFileContent(),
						context.TemporarySourceFilesDirectory);
				}
		}
		void SetupTemporarySourceFilesDirectory()
		{
			if (fileSystem.Exists(context.TemporarySourceFilesDirectory) == false)
			{
				fileSystem.Create(context.TemporarySourceFilesDirectory);
				return;
			}

			fileSystem.ClearSourceFilesFrom(context.TemporarySourceFilesDirectory);
		}

	}
}

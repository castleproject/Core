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

namespace Castle.Rook.Compiler
{
	using System;
	using System.IO;
	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services;
	using Castle.Rook.Compiler.Services.Passes;

	public delegate void PassInfoHandler(
		object sender, ICompilerPass pass, CompilationUnit unit, IErrorReport errorService);


	public class Compiler
	{
		private CompilerContainer container;

		public event PassInfoHandler PrePassExecution;

		public event PassInfoHandler PostPassExecution;

		public Compiler() : this(new CompilerContainer())
		{
		}

		public Compiler(CompilerContainer container)
		{
			this.container = container;
		}

		public ICompilerOptionsSet OptionsSet
		{
			get { return (ICompilerOptionsSet) container[ typeof(ICompilerOptionsSet) ]; }
		}

		public bool Compile( String textContent )
		{
			return Compile( new StringReader(textContent) );
		}

		public bool Compile( FileInfo fileInfo )
		{
			using(StreamReader reader = new StreamReader(fileInfo.FullName))
			{
				return Compile( reader );
			}
		}

		public bool Compile( CompilationUnit unit, FileInfo fileInfo )
		{
			using(StreamReader reader = new StreamReader(fileInfo.FullName))
			{
				return Compile( unit, reader );
			}
		}

		public bool Compile( String[] files )
		{
			CompilationUnit cunit = new CompilationUnit();

			foreach( String file in files )
			{
				if (!Compile(cunit, new FileInfo(file)))
				{
					return false;
				}
			}

			return RunPasses(cunit);
		}

		public bool Compile( TextReader reader )
		{
			CompilationUnit cunit = new CompilationUnit();

			if (Compile(cunit, reader))
			{
				return RunPasses(cunit);
			}

			return false;
		}

		public bool Compile( CompilationUnit cunit, TextReader reader )
		{
			container.ParserService.Parse(cunit, reader);

			return !container.ErrorReportService.HasErrors;
		}

		private bool RunPasses(CompilationUnit cunit)
		{
			if (!container.ErrorReportService.HasErrors)
			{
				ICompilerPass builderSkeleton = 
					container[ typeof(CreateBuilderSkeleton) ] as ICompilerPass;

				ExecutePass( builderSkeleton, cunit );
			}

			if (!container.ErrorReportService.HasErrors)
			{
				ICompilerPass scopePass = 
					container[ typeof(ScopePass) ] as ICompilerPass;

				ExecutePass( scopePass, cunit );
			}

			if (!container.ErrorReportService.HasErrors)
			{
				ICompilerPass typeResPass = 
					container[ typeof(TypeResolutionPass) ] as ICompilerPass;

				ExecutePass( typeResPass, cunit );
			}

			if (!container.ErrorReportService.HasErrors)
			{
				ICompilerPass emission = 
					container[ typeof(Emission) ] as ICompilerPass;

				emission.ExecutePass(cunit);
			}

			return !container.ErrorReportService.HasErrors;
		}

		private void ExecutePass(ICompilerPass pass, CompilationUnit unit)
		{
			if (PrePassExecution != null)
			{
				PrePassExecution(this, pass, unit, container.ErrorReportService);
			}

			pass.ExecutePass(unit);

			if (PostPassExecution != null)
			{
				PostPassExecution(this, pass, unit, container.ErrorReportService);
			}
		}
	}
}

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

namespace Castle.Rook.Compiler
{
	using Castle.MicroKernel;

	using Castle.Rook.Compiler.Services;
	using Castle.Rook.Compiler.Services.Default;
	using Castle.Rook.Compiler.Services.Passes;
	using Castle.Rook.Compiler.TypeSystem;

	public class CompilerContainer : DefaultKernel
	{
		public CompilerContainer()
		{
			AddServices();
		}

		public virtual void AddServices()
		{
			this.AddComponent("options", typeof(ICompilerOptionsSet), typeof(DefaultCompilerOptionsSet) );
			this.AddComponent("type.container", typeof(ITypeContainer), typeof(DefaultTypeContainer) );
			this.AddComponent("name.resolver", typeof(INameResolver), typeof(DefaultNameResolver) );
			this.AddComponent("ident", typeof(IIdentifierNameService), typeof(IdentifierNameService) );
			this.AddComponent("parser", typeof(IParser), typeof(RookParser) );
			this.AddComponent("error", typeof(IErrorReport), typeof(ErrorReport) );

			// Passes

			this.AddComponent("builder.skeleton.pass", typeof(CreateBuilderSkeleton) );
			this.AddComponent("emission.pass", typeof(Emission) );
			this.AddComponent("scope.pass", typeof(ScopePass) );
			this.AddComponent("typeresolution.pass", typeof(TypeResolutionPass) );
		}

		public IErrorReport ErrorReportService
		{
			get { return this[typeof(IErrorReport)] as IErrorReport; }
		}

		public IParser ParserService
		{
			get { return this[typeof(IParser)] as IParser; }
		}
	}
}
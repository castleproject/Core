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

namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System;
	using System.CodeDom.Compiler;

	public class DefaultCodeProviderAdapter : ICodeProviderAdapter, IDisposable
	{
		readonly CodeDomProvider codeProvider;

		public DefaultCodeProviderAdapter(CodeDomProvider codeProvider)
		{
			this.codeProvider = codeProvider;
		}

		public CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources)
		{
			return codeProvider.CompileAssemblyFromSource(parameters, sources);

		}

		public CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames)
		{
			return codeProvider.CompileAssemblyFromFile(parameters, fileNames);
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		public void Dispose()
		{
			if (codeProvider != null)
			{
				codeProvider.Dispose();
			}
		}
	}
}
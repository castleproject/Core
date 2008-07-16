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

namespace Castle.MonoRail.Views.AspView.Compiler.Factories
{
	using System.CodeDom.Compiler;
	using System.Configuration;
	using System.Security;
	using Microsoft.CSharp;

	using Adapters;

	public class CSharpCodeProviderAdapterFactory : ICodeProviderAdapterFactory
	{
		public ICodeProviderAdapter GetAdapter()
		{
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

			return new DefaultCodeProviderAdapter(codeProvider);
		}
	}
}

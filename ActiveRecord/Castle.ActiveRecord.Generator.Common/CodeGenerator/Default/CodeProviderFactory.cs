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

namespace Castle.ActiveRecord.Generator.CodeGenerator.Default
{
	using System;
	using System.CodeDom.Compiler;

	using Castle.ActiveRecord.Generator.CodeGenerator;


	public class CodeProviderFactory : ICodeProviderFactory	
	{
		public CodeProviderInfo[] GetAvailableProviders()
		{
			CodeProviderInfo[] providers = new CodeProviderInfo[2];
			
			providers[0] = new CodeProviderInfo( "C#", typeof(Microsoft.CSharp.CSharpCodeProvider) );
			providers[1] = new CodeProviderInfo( "VB.Net", typeof(Microsoft.VisualBasic.VBCodeProvider) );
//			providers[2] = new CodeProviderInfo( "JScript", typeof(Microsoft.JScript.JScriptCodeProvider, Microsoft.JScript) );
//			providers[3] = new CodeProviderInfo( "VJ#", typeof(Microsoft.VJSharp.VJSharpCodeProvider, VJSharpCodeProvider) );
			
			return providers;
		}

		public CodeDomProvider GetProvider(CodeProviderInfo info)
		{
			return (CodeDomProvider) Activator.CreateInstance(info.CodeProvider);
		}
	}
}

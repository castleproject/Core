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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for EasyRuntimeConstructor.
	/// </summary>
	public class EasyRuntimeConstructor : EasyConstructor
	{
		public EasyRuntimeConstructor(AbstractEasyType maintype, params ArgumentReference[] arguments )
		{
			Type[] args = ArgumentsUtil.InitializeAndConvert( arguments );
			
			_builder = maintype.TypeBuilder.DefineConstructor( 
				MethodAttributes.SpecialName|MethodAttributes.Public|MethodAttributes.HideBySig, 
				CallingConventions.Standard, 
				args );

			_builder.SetImplementationFlags(MethodImplAttributes.Runtime|MethodImplAttributes.Managed);
		}

		public override void Generate()
		{
		}

		public override void EnsureValidCodeBlock()
		{
		}
	}
}

using AspectSharp.Lang.AST.Visitors;
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

namespace AspectSharp.Lang.Steps.Types
{
	using System;
	using System.Reflection;
	using System.Collections;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.Steps.Types.Resolution;

	/// <summary>
	/// Summary description for ResolveTypesStep.
	/// </summary>
	public class ResolveTypesStep : AbstractVisitorStep
	{
		private TypeManager _typeManager;
		private IDictionary _mixinKey2TypeReference;
		private IDictionary _interceptorKey2TypeReference;

		public ResolveTypesStep()
		{
			_typeManager = new TypeManager();
			_typeManager.InspectAppDomainAssemblies();
			
			_mixinKey2TypeReference = new Hashtable();
			_interceptorKey2TypeReference = new Hashtable();
		}

		public override void Process(Context context, EngineConfiguration conf)
		{
			Init(context);
			Visit(conf);

			if (!context.HasErrors)
			{
				base.Process(context, conf);
			}
		}

		#region AbstractVisitorStep overrides
	
		public override void OnAssemblyReference(AssemblyReference assemblyReference)
		{
			try
			{
				Assembly assembly = Assembly.LoadWithPartialName(assemblyReference.AssemblyName);
				assemblyReference.ResolvedAssembly = assembly;
			}
			catch(Exception ex)
			{
				Context.RaiseErrorEvent( 
					assemblyReference.LexicalInfo, 
					String.Format("Could not load assembly '{0}'. Reason '{1}'", 
					assemblyReference.AssemblyName, ex.Message) );
			}
		}

		public override void OnTypeReferenceDefinition(TypeReference type)
		{
			if (type.TargetType == TargetTypeEnum.Type)
			{
				base.OnTypeReferenceDefinition (type);

				if (type.AssemblyReference == null)
				{
					type.ResolvedType = _typeManager.ResolveType( type.TypeName );
				}
				else
				{
					type.ResolvedType = LoadType( 
						type.TypeName, 
						type.AssemblyReference.ResolvedAssembly.FullName, 
						type.LexicalInfo );
				}
			}
		}
	
		public override void OnImportDirective(ImportDirective import)
		{
			base.OnImportDirective (import);

			if (import.AssemblyReference != null)
			{
				_typeManager.AddAssembly( import.AssemblyReference.ResolvedAssembly );
			}
		}
	
		public override void OnInterceptorEntryDefinition(InterceptorEntryDefinition interceptor)
		{
			base.OnInterceptorEntryDefinition (interceptor);
			_interceptorKey2TypeReference[interceptor.Key] = interceptor.TypeReference;
		}
	
		public override void OnMixinEntryDefinition(MixinEntryDefinition mixin)
		{
			base.OnMixinEntryDefinition (mixin);
			_mixinKey2TypeReference[mixin.Key] = mixin.TypeReference;
		}

		public override void OnMixinDefinition(MixinDefinition mixin)
		{
			if (mixin.TypeReference.TargetType == TargetTypeEnum.Type)
			{
				base.OnMixinDefinition(mixin);
			}
			else
			{
				mixin.TypeReference = 
					_mixinKey2TypeReference[ mixin.TypeReference.LinkRef ] as TypeReference;
			}
		}

		public override void OnInterceptorDefinition(InterceptorDefinition interceptor)
		{
			if (interceptor.TypeReference.TargetType == TargetTypeEnum.Type)
			{
				base.OnInterceptorDefinition(interceptor);
			}
			else
			{
				interceptor.TypeReference = 
					_interceptorKey2TypeReference[ interceptor.TypeReference.LinkRef ] as TypeReference;
			}
		}

		#endregion

		private Type LoadType( String typeName, String assemblyName, LexicalInfo info )
		{
			String qualifiedName = String.Format("{0}, {1}", typeName, assemblyName);

			try
			{
				return Type.GetType( qualifiedName, true, false );
			}
			catch(Exception)
			{
				Context.RaiseErrorEvent( info, "Could not load type specified " + qualifiedName );
			}

			return null;
		}
	}
}

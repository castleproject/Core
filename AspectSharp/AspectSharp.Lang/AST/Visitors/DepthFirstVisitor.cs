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

namespace AspectSharp.Lang.AST.Visitors
{
	using System;

	/// <summary>
	/// Summary description for AbstractDepthFirstVisitor.
	/// </summary>
	public class DepthFirstVisitor : IVisitor
	{
		public void Visit(INode node)
		{
			node.Accept(this);
		}

		public void Visit(INode[] nodes)
		{
			foreach(INode node in nodes)
			{
				Visit(node);
			}
		}

		#region Depth Control Members

		protected virtual bool EnterEngineConfiguration(EngineConfiguration conf)
		{
			return true;
		}

		protected virtual void LeaveEngineConfiguration(EngineConfiguration conf)
		{
		}

		protected virtual bool EnterAspectDefinition(AspectDefinition aspect)
		{
			return true;
		}

		protected virtual void LeaveAspectDefinition(AspectDefinition aspect)
		{
		}

		#endregion

		#region IVisitor Members

		public virtual void OnEngineConfiguration(EngineConfiguration conf)
		{
			if (EnterEngineConfiguration(conf))
			{
				conf.Imports.Accept(this);
				conf.Mixins.Accept(this);
				conf.Interceptors.Accept(this);
				conf.Aspects.Accept(this);
				
				LeaveEngineConfiguration(conf);
			}
		}

		public virtual void OnImportDirective(ImportDirective import)
		{
			if (import.AssemblyReference != null)
			{
				OnAssemblyReference(import.AssemblyReference);
			}
		}

		public virtual void OnGlobalInterceptorDeclaration(NodeCollectionBase dict)
		{
			Visit(dict.ToNodeArray());
		}

		public virtual void OnGlobalMixinDeclaration(NodeCollectionBase dict)
		{
			Visit(dict.ToNodeArray());
		}


		public virtual void OnExcludedTypes(TypeReferenceCollection types)
		{
			if (types.Count != 0)
			{
				Visit(types.ToNodeArray());
			}
		}

		public virtual void OnAspectDefinition(AspectDefinition aspect)
		{
			if (EnterAspectDefinition(aspect))
			{
				OnTargetTypeDefinition(aspect.TargetType);
				aspect.Mixins.Accept(this);
				aspect.PointCuts.Accept(this);

				LeaveAspectDefinition(aspect);
			}
		}

		public virtual void OnTargetNamespace(String nameSpace)
		{
		}
		
		public virtual void OnTargetTypeDefinition(TargetTypeDefinition targetType)
		{
			if (targetType.TargetStrategy == TargetStrategyEnum.SingleType)
			{
				OnTypeReferenceDefinition( targetType.SingleType );
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Assignable)
			{
				OnTypeReferenceDefinition( targetType.AssignType );
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Namespace)
			{
				OnTargetNamespace( targetType.NamespaceRoot );
				OnExcludedTypes( targetType.Excludes );
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Custom)
			{
				OnTypeReferenceDefinition( targetType.CustomMatcherType );
			}
		}

		public virtual void OnPointCutDefinition(PointCutDefinition pointcut)
		{
			pointcut.Advices.Accept(this);
		}

		public virtual void OnTypeReferenceDefinition(TypeReference type)
		{
			if (type.AssemblyReference != null)
			{
				OnAssemblyReference(type.AssemblyReference);
			}
		}

		public virtual void OnMixinDefinition(MixinDefinition mixin)
		{
			OnTypeReferenceDefinition(mixin.TypeReference);
		}

		public virtual void OnInterceptorDefinition(InterceptorDefinition interceptor)
		{
			OnTypeReferenceDefinition(interceptor.TypeReference);
		}

		public virtual void OnMixinEntryDefinition(MixinEntryDefinition mixin)
		{
			OnTypeReferenceDefinition(mixin.TypeReference);
		}

		public virtual void OnInterceptorEntryDefinition(InterceptorEntryDefinition interceptor)
		{
			OnTypeReferenceDefinition(interceptor.TypeReference);
		}

		public virtual void OnAssemblyReference(AssemblyReference assemblyReference)
		{
			
		}

		#endregion
	}
}

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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;

	using Castle.MicroKernel.Exceptions;
	using Castle.Core;

	/// <summary>
	/// Used during a component request, passed along to the whole process.
	/// This allow some data to be passed along the process, which is used 
	/// to detected cycled dependency graphs and now it's also being used
	/// to provide arguments to components.
	/// </summary>
	[Serializable]
	public sealed class CreationContext : MarshalByRefObject, ISubDependencyResolver
	{
		public readonly static CreationContext Empty = new CreationContext(new DependencyModel[0]);

		/// <summary>
		/// 
		/// </summary>
		private readonly IHandler handler;

		private readonly IDictionary additionalArguments;

		/// <summary>
		/// Holds the scoped dependencies being resolved. 
		/// If a dependency appears twice on the same scope, we'd have a cycle.
		/// </summary>
		private readonly DependencyModelCollection dependencies;

		#if DOTNET2
		
		private readonly Type[] genericArguments;
		
		#endif

		public CreationContext(IHandler handler, IDictionary additionalArguments)
		{
			this.handler = handler;
			this.additionalArguments = additionalArguments;
			dependencies = new DependencyModelCollection();
		}

		public CreationContext(IHandler handler, CreationContext parentContext)
		{
			this.handler = handler;
			additionalArguments = parentContext.additionalArguments;
			dependencies = new DependencyModelCollection(parentContext.Dependencies);
		}

		public CreationContext(DependencyModel[] dependencies)
		{
			this.handler = null;
			this.dependencies = new DependencyModelCollection(dependencies);
		}

		public CreationContext(IHandler handler, DependencyModelCollection dependencies)
			: this(handler, (IDictionary)null)
		{
			this.dependencies = new DependencyModelCollection(dependencies);
		}

		#if DOTNET2

		public CreationContext(IHandler handler, Type typeToExtractGenericArguments, IDictionary additionalArguments)
			: this(handler, additionalArguments)
		{
			genericArguments = ExtractGenericArguments(typeToExtractGenericArguments);
		}

		public CreationContext(IHandler handler, Type typeToExtractGenericArguments, 
		                       CreationContext parentContext) : this(handler, parentContext.Dependencies)
		{
			additionalArguments = parentContext.additionalArguments;
			genericArguments = ExtractGenericArguments(typeToExtractGenericArguments);
		}

		#endif

		#region ISubDependencyResolver 

		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, 
		                      ComponentModel model, DependencyModel dependency)
		{
			if (additionalArguments != null)
			{
				return additionalArguments[dependency.DependencyKey];
			}
			
			return null;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, 
		                       ComponentModel model, DependencyModel dependency)
		{
			if (dependency.DependencyKey == null) return false;
			
			if (additionalArguments != null)
			{
				return additionalArguments.Contains(dependency.DependencyKey);
			}
			
			return false;
		}

		#endregion
		
		public bool HasAdditionalParameters
		{
			get { return additionalArguments != null && additionalArguments.Count != 0; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public IHandler Handler
		{
			get { return handler; }
		}

		#region Cycle detection related members

		/// <summary>
		/// Track dependencies and guards against circular dependencies.
		/// </summary>
		/// <returns>A dependency key that can be used to remove the dependency if it was resolved correctly.</returns>
		public DependencyModel TrackDependency(MemberInfo info, DependencyModel dependencyModel)
		{
			if (dependencies.Contains(dependencyModel))
			{
				StringBuilder sb = new StringBuilder("A cycle was detected when trying to resolve a dependency. ");
				
				sb.Append("The dependency graph that resulted in a cycle is:");

				foreach(DependencyModel key in dependencies)
				{
					DependencyModelExtended extendedInfo = key as DependencyModelExtended;
					
					if (extendedInfo != null)
					{
						sb.AppendFormat("\r\n - {0} for {1} in type {2}",
							key.ToString(), extendedInfo.Info, extendedInfo.Info.DeclaringType);
					}
					else
					{
						sb.AppendFormat("\r\n - {0}", key.ToString());
					}
				}

				sb.AppendFormat("\r\n + {0} for {1} in {2}\r\n",
				                dependencyModel, info, info.DeclaringType);

				throw new CircularDependecyException(sb.ToString());
			}

			DependencyModelExtended trackingKey = new DependencyModelExtended(dependencyModel, info);
			dependencies.Add(trackingKey);
			return trackingKey;
		}

		/// <summary>
		/// Removes a dependency that was resolved successfully.
		/// </summary>
		public void UntrackDependency(DependencyModel model)
		{
			dependencies.Remove(model);
		}

		public DependencyModelCollection Dependencies
		{
			get { return dependencies; }
		}

		/// <summary>
		/// Extends <see cref="DependencyModel"/> adding <see cref="MemberInfo"/>
		/// information. This is only useful to provide detailed information 
		/// on exceptions.
		/// </summary>
		[Serializable]
		internal class DependencyModelExtended : DependencyModel
		{
			private readonly MemberInfo info;

			public DependencyModelExtended(DependencyModel inner, MemberInfo info)
				:
				base(inner.DependencyType, inner.DependencyKey, inner.TargetType, inner.IsOptional)
			{
				this.info = info;
			}

			public MemberInfo Info
			{
				get { return info; }
			}
		}

		#endregion

		#if DOTNET2

		public Type[] GenericArguments
		{
			get { return genericArguments; }
		}

		private static Type[] ExtractGenericArguments(Type typeToExtractGenericArguments)
		{
			return typeToExtractGenericArguments.GetGenericArguments();
		}

		#endif
	}
}
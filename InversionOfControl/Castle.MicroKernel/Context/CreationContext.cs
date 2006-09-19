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
	using System.Reflection;
	using System.Text;

	using Castle.MicroKernel.Exceptions;
	using Castle.Core;

	
	[Serializable]
	public sealed class CreationContext : MarshalByRefObject
	{
		public readonly static CreationContext Empty = new CreationContext(new DependencyModel[0]);

		private readonly DependencyModelCollection dependencies;

#if DOTNET2
		private readonly Type[] arguments;
#endif

		public CreationContext(DependencyModelCollection dependencies)
		{
			this.dependencies = new DependencyModelCollection(dependencies);
		}
		
		public CreationContext(DependencyModel[] dependencies)
		{
			this.dependencies = new DependencyModelCollection(dependencies);
		}

		/// <summary>
		/// Track dependencies and guards against circular dependencies.
		/// </summary>
		/// <returns>A dependency key that can be used to remove the dependency if it was resolved correctly.</returns>
		public DependencyModel TrackDependency(MemberInfo info, DependencyModel dependencyModel)
		{
			if (dependencies.Contains(dependencyModel))
			{
				StringBuilder sb = new StringBuilder("A cycle was detected when trying to create a service. ");
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
		public void RemoveDependencyTracking(DependencyModel model)
		{
			dependencies.Remove(model);
		}

		public DependencyModelCollection Dependencies
		{
			get { return dependencies; }
		}

#if DOTNET2
		
		public CreationContext(DependencyModel[] dependencies, Type target)
		{
			this.dependencies = new DependencyModelCollection(dependencies);
			arguments = ExtractGenericArguments(target);
		}

		public Type[] GenericArguments
		{
			get { return arguments; }
		}

		private static Type[] ExtractGenericArguments(Type target)
		{
			return target.GetGenericArguments();
		}

#endif

		[Serializable]
		internal class DependencyModelExtended : DependencyModel
		{
			private readonly MemberInfo info;

			public DependencyModelExtended(DependencyModel inner, MemberInfo info) : 
				base(inner.DependencyType, inner.DependencyKey, inner.TargetType, inner.IsOptional)
			{
				this.info = info;
			}

			public MemberInfo Info
			{
				get { return info; }
			}
		}
	}
}
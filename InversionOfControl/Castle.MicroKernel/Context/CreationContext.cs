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

	
	[Serializable]
	public sealed class CreationContext : MarshalByRefObject
	{
		public readonly static CreationContext Empty = new CreationContext(new DependencyModel[0]);
		
		private readonly ArrayList dependencies;
	    
		#if DOTNET2
		private readonly Type[] arguments;
		#endif

        public CreationContext(ICollection dependencies)
        {
            this.dependencies = new ArrayList(dependencies);
        }

		/// <summary>
		/// Track dependencies and guards against circular dependencies.
		/// </summary>
		/// <returns>A dependency key that can be used to remove the dependency if it was resolved correctly.</returns>
        public object TrackDependency(MemberInfo info, DependencyModel dependencyModel)
        {
            if (dependencies.Contains(dependencyModel))
            {
                StringBuilder sb = new StringBuilder("A cycle was detected when trying to create a service. ");
                sb.Append("The dependency graph that resulted in a cycle is:");
                
            	foreach (DependencyKey key in dependencies)
                {
                    sb.AppendFormat("\r\n - {0} for {1} in type {2}", 
                                    key.Dependency, key.Info, key.Info.DeclaringType);
                }
            	
                sb.AppendFormat("\r\n + {0} for {1} in {2}\r\n", 
                                dependencyModel, info, info.DeclaringType);
                
            	throw new CircularDependecyException(sb.ToString());
            }

			object trackingKey = new DependencyKey(dependencyModel, info);
			dependencies.Add(trackingKey);
			return trackingKey;
        }

		/// <summary>
		/// Removes a dependency that was resolved successfully.
		/// </summary>
		public void RemoveDependencyTracking(object key)
		{
			dependencies.Remove(key);
		}

	    public ICollection Dependencies
	    {
	        get { return dependencies; }
	    }

		#if DOTNET2
		
		public CreationContext(ICollection dependencies, Type target)
		{
            this.dependencies = new ArrayList(dependencies);
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
		 internal class DependencyKey : MarshalByRefObject
	    {
            DependencyModel dependencyModel;
            MemberInfo info;

            public DependencyKey(DependencyModel model, MemberInfo service)
	        {
	            dependencyModel = model;
	            info = service;
	        }

			public DependencyModel Dependency
			{
				get { return dependencyModel; }
			}

			public MemberInfo Info
			{
				get { return info; }
			}


            public override bool Equals(object obj)
            {
				if ( Object.ReferenceEquals(this, obj))
					return true;
				return dependencyModel.Equals(obj);
            }

            public override int GetHashCode()
            {
                return dependencyModel.GetHashCode();
            }
	    }
	}
}

// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Castle.Core;
	using Castle.MicroKernel.Exceptions;

	internal class DependencyTrackingScope : IDisposable
	{
		private readonly DependencyModel dependencyTrackingKey;
		private readonly DependencyModelCollection dependencies;

		public DependencyTrackingScope(CreationContext creationContext, ComponentModel model, MemberInfo info,
		                               DependencyModel dependencyModel)
		{
		    if (dependencyModel.TargetType == typeof (IKernel)) 
                return;

		    this.dependencies = creationContext.Dependencies;

		    // We track dependencies in order to detect cycled graphs
		    // This prevents a stack overflow
		    this.dependencyTrackingKey = TrackDependency(model, info, dependencyModel);
		}

		public void Dispose()
		{
			// if the dependency were being tracked, and we reached the dispose...
			if (dependencies != null && dependencyTrackingKey != null)
			{
				// ...then the dependency was resolved successfully, we can stop tracking it.
				UntrackDependency(dependencyTrackingKey);
			}
		}

		private DependencyModelExtended TrackDependency(ComponentModel model, MemberInfo info, DependencyModel dependencyModel)
		{
			DependencyModelExtended trackingKey = new DependencyModelExtended(model, dependencyModel, info);

			if (dependencies.Contains(trackingKey))
			{
				StringBuilder sb = new StringBuilder("A cycle was detected when trying to resolve a dependency. ");

				sb.Append("The dependency graph that resulted in a cycle is:");

				foreach(DependencyModel key in dependencies)
				{
					DependencyModelExtended extendedInfo = key as DependencyModelExtended;

					if (extendedInfo != null)
					{
						sb.AppendFormat("\r\n - {0} for {1} in type {2}",
						                key, extendedInfo.Info, extendedInfo.Info.DeclaringType);
					}
					else
					{
						sb.AppendFormat("\r\n - {0}", key);
					}
				}

				sb.AppendFormat("\r\n + {0} for {1} in {2}\r\n",
				                dependencyModel, info, info.DeclaringType);

				throw new CircularDependencyException(sb.ToString());
			}

			dependencies.Add(trackingKey);

			return trackingKey;
		}

		private void UntrackDependency(DependencyModel model)
		{
			dependencies.Remove(model);
		}

		#region DependencyModelExtended

		/// <summary>
		/// Extends <see cref="DependencyModel"/> adding <see cref="MemberInfo"/> and <see cref="ComponentModel"/>
		/// information. Th MemberInfo is only useful to provide detailed information 
		/// on exceptions. 
		/// The ComponentModel is required so we can get resolve an object that takes as a parameter itself, but
		/// with difference model. (See IoC 51 for the details)
		/// </summary>
		[Serializable]
		internal class DependencyModelExtended : DependencyModel
		{
			private readonly ComponentModel model;
			private readonly MemberInfo info;

			public DependencyModelExtended(ComponentModel model, DependencyModel inner, MemberInfo info)
				:
					base(inner.DependencyType, inner.DependencyKey, inner.TargetType, inner.IsOptional)
			{
				this.model = model;
				this.info = info;
			}

			public MemberInfo Info
			{
				get { return info; }
			}

			public override bool Equals(object obj)
			{
				DependencyModelExtended other = obj as DependencyModelExtended;
				if (other == null)
					return false;
				return other.Info == this.Info &&
				       other.model == model &&
				       base.Equals(other);
			}

			public override int GetHashCode()
			{
				int infoHash = 37 ^ Info.GetHashCode();
				return base.GetHashCode() + infoHash;
			}
		}

		#endregion
	}
}
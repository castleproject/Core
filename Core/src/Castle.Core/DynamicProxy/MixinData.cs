// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;

	public class MixinData
	{
		private readonly Dictionary<Type, int> mixinPositions = new Dictionary<Type, int>();
		private readonly List<object> mixinsImpl = new List<object>();

		/// <summary>
		///   Because we need to cache the types based on the mixed in mixins, we do the following here:
		///   - Get all the mixin interfaces
		///   - Sort them by full name
		///   - Return them by position
		/// 
		/// The idea is to have reproducible behavior for the case that mixins are registered in different orders.
		/// This method is here because it is required 
		/// </summary>
		public MixinData(IEnumerable<object> mixinInstances)
		{
			if (mixinInstances != null)
			{
				var sortedMixedInterfaceTypes = new List<Type>();
				var interface2Mixin = new Dictionary<Type, object>();

				foreach (var mixin in mixinInstances)
				{
					var mixinInterfaces = mixin.GetType().GetInterfaces();

					foreach (var inter in mixinInterfaces)
					{
						sortedMixedInterfaceTypes.Add(inter);

						if (interface2Mixin.ContainsKey(inter))
						{
							var message = string.Format(
								"The list of mixins contains two mixins implementing the same interface '{0}': {1} and {2}. An interface cannot be added by more than one mixin.",
								inter.FullName,
								interface2Mixin[inter].GetType().Name,
								mixin.GetType().Name);
							throw new ArgumentException(message, "mixinInstances");
						}

						interface2Mixin[inter] = mixin;
					}
				}
				sortedMixedInterfaceTypes.Sort((x, y) => x.FullName.CompareTo(y.FullName));

				for (var i = 0; i < sortedMixedInterfaceTypes.Count; i++)
				{
					var mixinInterface = sortedMixedInterfaceTypes[i];
					var mixin = interface2Mixin[mixinInterface];

					mixinPositions[mixinInterface] = i;
					mixinsImpl.Add(mixin);
				}
			}
		}

		public IEnumerable<Type> MixinInterfaces
		{
			get { return mixinPositions.Keys; }
		}

		public IEnumerable<object> Mixins
		{
			get { return mixinsImpl; }
		}

		public bool ContainsMixin(Type mixinInterfaceType)
		{
			return mixinPositions.ContainsKey(mixinInterfaceType);
		}

		// For two MixinData objects being regarded equal, only the sorted mixin types are considered, not the actual instances.
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var other = obj as MixinData;
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			if (mixinsImpl.Count != other.mixinsImpl.Count)
			{
				return false;
			}

			for (var i = 0; i < mixinsImpl.Count; ++i)
			{
				if (mixinsImpl[i].GetType() != other.mixinsImpl[i].GetType())
				{
					return false;
				}
			}

			return true;
		}

		// For two MixinData objects being regarded equal, only the mixin types are considered, not the actual instances.
		public override int GetHashCode()
		{
			var hashCode = 0;
			foreach (var mixinImplementation in mixinsImpl)
			{
				hashCode = 29*hashCode + mixinImplementation.GetType().GetHashCode();
			}

			return hashCode;
		}

		public object GetMixinInstance(Type mixinInterfaceType)
		{
			return mixinsImpl[mixinPositions[mixinInterfaceType]];
		}

		public int GetMixinPosition(Type mixinInterfaceType)
		{
			return mixinPositions[mixinInterfaceType];
		}
	}
}

// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections.Generic;

namespace Castle.DynamicProxy
{
	public class MixinData
	{
		private readonly List<object> mixinsImpl = new List<object>();
		private readonly Dictionary<Type, int> mixinPositions = new Dictionary<Type, int>();

		/// <summary>
		/// Because we need to cache the types based on the mixed in mixins, we do the following here:
		///  - Get all the mixin interfaces
		///  - Sort them by full name
		///  - Return them by position
		/// 
		/// The idea is to have reproducable behavior for the case that mixins are registered in different orders.
		/// This method is here because it is required 
		/// </summary>
		public MixinData(IEnumerable<object> mixinInstances)
		{
			if (mixinInstances != null)
			{
				List<Type> sortedMixedInterfaceTypes = new List<Type>();
				Dictionary<Type, object> interface2Mixin = new Dictionary<Type, object>();

				foreach (object mixin in mixinInstances)
				{
					Type[] mixinInterfaces = mixin.GetType().GetInterfaces();

					foreach (Type inter in mixinInterfaces)
					{
						sortedMixedInterfaceTypes.Add(inter);

						if (interface2Mixin.ContainsKey(inter))
						{
							string message = string.Format(
									"The list of mixins contains two mixins implementing the same interface '{0}': {1} and {2}. An interface cannot be added by more than one mixin.",
									inter.FullName,
									interface2Mixin[inter].GetType().Name,
									mixin.GetType().Name);
							throw new ArgumentException(message, "mixinInstances");
						}

						interface2Mixin[inter] = mixin;
					}
				}
				sortedMixedInterfaceTypes.Sort(
						delegate (Type x, Type y) { return x.FullName.CompareTo (y.FullName); });

				for (int i = 0; i < sortedMixedInterfaceTypes.Count; i++)
				{
					Type mixinInterface = sortedMixedInterfaceTypes[i];
					object mixin = interface2Mixin[mixinInterface];

					mixinPositions[mixinInterface] = i;
					mixinsImpl.Add (mixin);
				}
			}
		}

		public IEnumerable<object> Mixins
		{
			get { return mixinsImpl; }
		}

		public IEnumerable<Type> MixinInterfaces
		{
			get { return mixinPositions.Keys; }
		}

		public int GetMixinPosition(Type mixinInterfaceType)
		{
			return mixinPositions[mixinInterfaceType];
		}

		public bool ContainsMixin(Type mixinInterfaceType)
		{
			return mixinPositions.ContainsKey(mixinInterfaceType);
		}

		public object GetMixinInstance(Type mixinInterfaceType)
		{
			return mixinsImpl[mixinPositions[mixinInterfaceType]];
		}

		// For two MixinData objects being regarded equal, only the sorted mixin types are considered, not the actual instances.
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			MixinData other = obj as MixinData;
			if (object.ReferenceEquals(other, null))
				return false;

			if (mixinsImpl.Count != other.mixinsImpl.Count)
				return false;

			for (int i = 0; i < mixinsImpl.Count; ++i)
			{
				if (mixinsImpl[i].GetType() != other.mixinsImpl[i].GetType())
					return false;
			}

			return true;
		}

		// For two MixinData objects being regarded equal, only the mixin types are considered, not the actual instances.
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (object mixinImplementation in mixinsImpl)
				hashCode = 29*hashCode + mixinImplementation.GetType().GetHashCode();

			return hashCode;
		}
	}
}
// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	internal sealed class InterfaceAttributeUtil
	{
		private readonly Aged<Type>[] types; // in order from most to least derived
		private readonly Dictionary<Type, Aged<object>> singletons;
		private readonly List<object> results;

		private int index;

		private Type CurrentType
		{
			get { return types[index].Value; }
		}

		private int CurrentAge
		{
			get { return types[index].Age; }
		}

		private bool IsMostDerivedType
		{
			get { return index == 0; }
		}

		public static object[] GetAttributes(Type type, bool inherit)
		{
			if (type.IsInterface == false)
				throw new ArgumentOutOfRangeException("type");

			var attributes = type.GetCustomAttributes(false);
			var baseTypes  = type.GetInterfaces();

			if (baseTypes.Length == 0 || !inherit)
				return attributes;

			return new InterfaceAttributeUtil(type, baseTypes)
				.GetAttributes(attributes);
		}

		private InterfaceAttributeUtil(Type derivedType, Type[] baseTypes)
		{
			types      = CollectTypes(derivedType, baseTypes);
			singletons = new Dictionary<Type, Aged<object>>();
			results    = new List<object>();
		}

		private Aged<Type>[] CollectTypes(Type derivedType, Type[] baseTypes)
		{
			var ages = new Dictionary<Type, int>();
			int age;

			ages[derivedType] = 0;

			foreach (var baseType in baseTypes)
				if (ShouldConsiderType(baseType))
					ages[baseType] = 1;

			foreach (var baseType in baseTypes)
				if (ages.ContainsKey(baseType))
					foreach (var type in baseType.GetInterfaces())
						if (ages.TryGetValue(type, out age))
							ages[type] = ++age;

			return ages
				.Select (a => new Aged<Type>(a.Key, a.Value))
				.OrderBy(t => t.Age)
				.ToArray();
		}

		private object[] GetAttributes(object[] attributes)
		{
			for (index = types.Length - 1; index > 0; index--)
				ProcessType(CurrentType.GetCustomAttributes(false));

			ProcessType(attributes);

			CollectSingletons();
			return results.ToArray();
		}

		private void ProcessType(object[] attributes)
		{
			foreach (var attribute in attributes)
			{
				var attributeType  = attribute.GetType();
				var attributeUsage = attributeType.GetAttributeUsage();

				if (IsMostDerivedType || attributeUsage.Inherited)
				{
					if (attributeUsage.AllowMultiple)
						results.Add(attribute);
					else
						AddSingleton(attribute, attributeType);
				}
			}
		}

		private void AddSingleton(object attribute, Type attributeType)
		{
			Aged<object> singleton;
			if (singletons.TryGetValue(attributeType, out singleton))
			{
				if (singleton.Age == CurrentAge)
				{
					if (singleton.Value == ConflictMarker)
						return; // already in conflict
					else
						attribute = ConflictMarker;
				}
			}

			singletons[attributeType] = MakeAged(attribute);
		}

		private void CollectSingletons()
		{
			foreach (var entry in singletons)
			{
				var attribute = entry.Value.Value;

				if (attribute == ConflictMarker)
					HandleAttributeConflict(entry.Key);
				else
					results.Add(attribute);
			}
		}

		private void HandleAttributeConflict(Type attributeType)
		{
			var message = string.Format
			(
				"Cannot determine inherited attributes for interface type {0}.  " +
				"Conflicting attributes of type {1} exist in the inheritance graph.",
				CurrentType  .FullName,
				attributeType.FullName
			);

			throw new InvalidOperationException(message);
		}

		private static bool ShouldConsiderType(Type type)
		{
			var ns = type.Namespace;
			return ns != "Castle.Components.DictionaryAdapter"
				&& ns != "System.ComponentModel";
		}

		private Aged<T> MakeAged<T>(T value)
		{
			return new Aged<T>(value, CurrentAge);
		}

		[DebuggerDisplay("{Value}, Age: {Age}")]
		private sealed class Aged<T>
		{
			public readonly T   Value;
			public readonly int Age;

			public Aged(T value, int age)
			{
				Value = value;
				Age   = age;
			}
		}

		private static readonly object
			ConflictMarker = new object();
	}
}

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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Generators;

	public class ProxyElementContributor
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private readonly KeyValuePair<Type, ITypeContributor> mapping;
		private readonly bool onlyProxyVirtual;
		private readonly InterfaceMapping map;

		private readonly IDictionary<PropertyInfo, PropertyToGenerate> properties = new Dictionary<PropertyInfo, PropertyToGenerate>();
		private readonly IDictionary<EventInfo, EventToGenerate> events = new Dictionary<EventInfo, EventToGenerate>();
		private readonly IDictionary<MethodInfo, MethodToGenerate> methods = new Dictionary<MethodInfo, MethodToGenerate>();

		public ProxyElementContributor(KeyValuePair<Type, ITypeContributor> mapping, bool onlyProxyVirtual, InterfaceMapping map)
		{
			this.mapping = mapping;
			this.onlyProxyVirtual = onlyProxyVirtual;
			this.map = map;
		}

		public IEnumerable<MethodToGenerate> Methods
		{
			get { return methods.Values; }
		}

		public IEnumerable<MethodInfo> MethodInfos
		{
			get { return methods.Keys; }
		}

		public IEnumerable<PropertyToGenerate> Properties
		{
			get { return properties.Values; }
		}

		public IEnumerable<EventToGenerate> Events
		{
			get { return events.Values; }
		}

		public void Collect(IProxyGenerationHook hook)
		{
			CollectProperties(hook);
			CollectEvents(hook);
			// Methods go last, because properties and events have methods too (getters/setters add/remove)
			// and we don't want to get duplicates, so we collect property and event methods first
			// then we collect methods, and add only these that aren't there yet
			CollectMethods(hook);
		}

		private void CollectProperties(IProxyGenerationHook hook)
		{
			PropertyInfo[] propertiesFound = mapping.Key.GetProperties(Flags);
			foreach (PropertyInfo property in propertiesFound)
			{
				AddProperty(property, hook);
			}
		}

		private void CollectEvents(IProxyGenerationHook hook)
		{
			EventInfo[] eventsFound = mapping.Key.GetEvents(Flags);
			foreach (EventInfo @event in eventsFound)
			{
				AddEvent(@event, hook);
			}
		}

		private void CollectMethods(IProxyGenerationHook hook)
		{
			MethodInfo[] methodsFound = MethodFinder.GetAllInstanceMethods(mapping.Key, Flags);
			foreach (MethodInfo method in methodsFound)
			{
				AddMethod(method, hook, true);
			}
		}

		private void AddProperty(PropertyInfo property, IProxyGenerationHook hook)
		{
			MethodToGenerate getter = null;
			MethodToGenerate setter = null;

			if (property.CanRead)
			{
				MethodInfo getMethod = property.GetGetMethod(true);
				getter = AddMethod(getMethod, hook, false);
			}

			if (property.CanWrite)
			{
				MethodInfo setMethod = property.GetSetMethod(true);
				setter = AddMethod(setMethod, hook, false);
			}

			if (setter==null && getter == null)
			{
				return;
			}

			IEnumerable<Attribute> nonInheritableAttributes = GetNonInheritableAttributes(property);
			properties[property] = new PropertyToGenerate(property.Name,
			                                              property.PropertyType,
			                                              getter,
			                                              setter,
			                                              PropertyAttributes.None,
			                                              nonInheritableAttributes);
		}

		private void AddEvent(EventInfo @event, IProxyGenerationHook hook)
		{
			MethodInfo addMethod = @event.GetAddMethod(true);
			MethodInfo removeMethod = @event.GetRemoveMethod(true);
			MethodToGenerate adder = null;
			MethodToGenerate remover = null;

			if (addMethod != null)
			{
				adder = AddMethod(addMethod, hook, false);
			}

			if (removeMethod != null)
			{
				remover = AddMethod(removeMethod, hook, false);
			}

			if (adder == null && remover == null) return;

			events[@event] = new EventToGenerate(@event.Name,
			                                     @event.EventHandlerType,
			                                     adder,
			                                     remover,
			                                     EventAttributes.None);
		}

		private MethodToGenerate AddMethod(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			// This is here, so that we don't add multiple times property getters/setters and event add/remove methods
			if (methods.ContainsKey(method))
			{
				return null;
			}

			if (!IsAccessible(method))
			{
				return null;
			}

			if (IsVirtuallyImplementedInterfaceMethod(method))
			{
				return null;
			}

			if(!(AcceptMethod(method, onlyProxyVirtual, hook)))
			{
				return null;
			}

			ITypeContributor target = mapping.Value;
			var methodToGenerate = new MethodToGenerate(method, isStandalone, target);
			methods[method] = methodToGenerate;
			return methodToGenerate;
		}

		private bool IsVirtuallyImplementedInterfaceMethod(MethodInfo method)
		{
			var index = Array.IndexOf(map.InterfaceMethods, method);
			return index != -1 && map.TargetMethods[index].IsFinal == false;
		}

		/// <summary>
		/// Checks if the method is public or protected.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		private bool IsAccessible(MethodBase method)
		{
#if SILVERLIGHT
			return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
#else

			if (method.IsPublic
			    || method.IsFamily
			    || method.IsFamilyAndAssembly
			    || method.IsFamilyOrAssembly)
			{
				return true;
			}

			if (InternalsHelper.IsInternalToDynamicProxy(method.DeclaringType.Assembly) && method.IsAssembly)
			{
				return true;
			}

			return false;
#endif
		}

		/// <summary>
		/// Performs some basic screening and invokes the <see cref="IProxyGenerationHook"/>
		/// to select methods.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="onlyVirtuals"></param>
		/// <param name="hook"></param>
		/// <returns></returns>
		private bool AcceptMethod(MethodInfo method, bool onlyVirtuals, IProxyGenerationHook hook)
		{
			// we can never intercept a sealed (final) method
			if (method.IsFinal)
				return false;

			bool isInternalsAndNotVisibleToDynamicProxy = InternalsHelper.IsInternal(method);
			if (isInternalsAndNotVisibleToDynamicProxy)
			{
				isInternalsAndNotVisibleToDynamicProxy = InternalsHelper.IsInternalToDynamicProxy(method.DeclaringType.Assembly) ==
				                                         false;
			}

			if (isInternalsAndNotVisibleToDynamicProxy)
				return false;

			if (onlyVirtuals && !method.IsVirtual)
			{
#if SILVERLIGHT
				if (method.DeclaringType != typeof(object))
#else
				if (method.DeclaringType != typeof (object) && method.DeclaringType != typeof (MarshalByRefObject))
#endif
				{
					hook.NonVirtualMemberNotification(mapping.Key, method);
				}

				return false;
			}

			//can only proxy methods that are public or protected (or internals that have already been checked above)
			if ((method.IsPublic || method.IsFamily || method.IsAssembly || method.IsFamilyOrAssembly) == false)
				return false;

			if (method.DeclaringType == typeof (object))
			{
				return false;
			}

#if !SILVERLIGHT
			if (method.DeclaringType == typeof (MarshalByRefObject))
			{
				return false;
			}
#endif
			return hook.ShouldInterceptMethod(mapping.Key, method);
		}

		private IEnumerable<Attribute> GetNonInheritableAttributes(ICustomAttributeProvider propertyInfo)
		{
			object[] attrs = propertyInfo.GetCustomAttributes(false);

			foreach (Attribute attribute in attrs)
			{
				if (ShouldSkipAttributeReplication(attribute)) continue;

				yield return attribute;
			}
		}

		/// <summary>
		/// Attributes should be replicated if they are non-inheritable,
		/// but there are some special cases where the attributes means
		/// something to the CLR, where they should be skipped.
		/// </summary>
		private bool ShouldSkipAttributeReplication(Attribute attribute)
		{
			if (SpecialCaseAttributThatShouldNotBeReplicated(attribute))
				return true;

			object[] attrs = attribute.GetType().GetCustomAttributes(typeof (AttributeUsageAttribute), true);

			if (attrs.Length != 0)
			{
				var usage = (AttributeUsageAttribute) attrs[0];

				return usage.Inherited;
			}

			return true;
		}

		private static bool SpecialCaseAttributThatShouldNotBeReplicated(Attribute attribute)
		{
			return AttributesToAvoidReplicating.Contains(attribute.GetType());
		}
	}
}
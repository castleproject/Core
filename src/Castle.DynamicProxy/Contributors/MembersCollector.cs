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
	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;

	public abstract class MembersCollector
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		protected readonly bool onlyProxyVirtual;
		protected readonly InterfaceMapping map;
		private ILogger logger = NullLogger.Instance;
		private IList<MethodInfo> checkedMethods = new List<MethodInfo>();
		private readonly IDictionary<PropertyInfo, PropertyToGenerate> properties = new Dictionary<PropertyInfo, PropertyToGenerate>();
		private readonly IDictionary<EventInfo, EventToGenerate> events = new Dictionary<EventInfo, EventToGenerate>();
		private readonly IDictionary<MethodInfo, MethodToGenerate> methodsToProxy = new Dictionary<MethodInfo, MethodToGenerate>();
		private readonly Type type;

		protected readonly ITypeContributor contributor;

		protected MembersCollector(Type type, ITypeContributor contributor, bool onlyProxyVirtual, InterfaceMapping map)
		{
			this.type = type;
			this.contributor = contributor;
			this.onlyProxyVirtual = onlyProxyVirtual;
			this.map = map;
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public IEnumerable<MethodToGenerate> Methods
		{
			get { return methodsToProxy.Values; }
		}

		public IEnumerable<PropertyToGenerate> Properties
		{
			get { return properties.Values; }
		}

		public IEnumerable<EventToGenerate> Events
		{
			get { return events.Values; }
		}

		public void CollectMembersToProxy(IProxyGenerationHook hook)
		{
			if(checkedMethods==null)// this method was already called!
			{
				throw new InvalidOperationException("Can't call CollectMembersToProxy twice");
			}
			CollectProperties(hook);
			CollectEvents(hook);
			// Methods go last, because properties and events have methods too (getters/setters add/remove)
			// and we don't want to get duplicates, so we collect property and event methods first
			// then we collect methods, and add only these that aren't there yet
			CollectMethods(hook);

			checkedMethods = null; // this is ugly, should have a boolean flag for this or something
		}

		private void CollectProperties(IProxyGenerationHook hook)
		{
			PropertyInfo[] propertiesFound = type.GetProperties(Flags);
			foreach (PropertyInfo property in propertiesFound)
			{
				AddProperty(property, hook);
			}
		}

		private void CollectEvents(IProxyGenerationHook hook)
		{
			EventInfo[] eventsFound = type.GetEvents(Flags);
			foreach (EventInfo @event in eventsFound)
			{
				AddEvent(@event, hook);
			}
		}

		private void CollectMethods(IProxyGenerationHook hook)
		{
			MethodInfo[] methodsFound = MethodFinder.GetAllInstanceMethods(type, Flags);
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

			var nonInheritableAttributes = AttributeUtil.GetNonInheritableAttributes(property);
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
			if (checkedMethods.Contains(method))
			{
				return null;
			}
			checkedMethods.Add(method);

			if (methodsToProxy.ContainsKey(method))
			{
				return null;
			}
			var methodToGenerate = GetMethodToGenerate(method, hook, isStandalone);
			if (methodToGenerate != null)
			{
				methodsToProxy[method] = methodToGenerate;
			}

			return methodToGenerate;
		}

		protected abstract MethodToGenerate GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone);

		protected virtual MethodInfo GetMethodOnTarget(MethodInfo method)
		{
			int index = Array.IndexOf(map.InterfaceMethods, method);
			if (index == -1)
			{
				return null;
			}

			return map.TargetMethods[index];
		}

		/// <summary>
		/// Checks if the method is public or protected.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		protected bool IsAccessible(MethodBase method)
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

			// Explicitly implemented interface method on class
			if (method.IsPrivate && method.IsFinal)
			{
				Logger.Debug("Excluded explicitly implemented interface method {0} on type {1} because it cannot be intercepted.",
					method.Name, method.DeclaringType.FullName);
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
		protected bool AcceptMethod(MethodInfo method, bool onlyVirtuals, IProxyGenerationHook hook)
		{
			// we can never intercept a sealed (final) method
			if (method.IsFinal)
			{
				Logger.Debug("Excluded sealed method {0} on {1} because it cannot be intercepted.", method.Name, method.DeclaringType.FullName);
				return false;
			}

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
				if (method.DeclaringType != typeof(object) && method.DeclaringType != typeof(MarshalByRefObject))
#endif
				{
					Logger.Debug("Excluded non-virtual method {0} on {1} because it cannot be intercepted.", method.Name, method.DeclaringType.FullName);
					hook.NonVirtualMemberNotification(type, method);
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
			return hook.ShouldInterceptMethod(type, method);
		}
	}
}
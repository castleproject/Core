// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;

	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;

	internal abstract class MembersCollector
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private ILogger logger = NullLogger.Instance;

		protected readonly Type type;

		protected MembersCollector(Type type)
		{
			this.type = type;
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public virtual void CollectMembersToProxy(IProxyGenerationHook hook, IMembersCollectorSink sink)
		{
			var checkedMethods = new HashSet<MethodInfo>();

			CollectProperties();
			CollectEvents();
			// Methods go last, because properties and events have methods too (getters/setters add/remove)
			// and we don't want to get duplicates, so we collect property and event methods first
			// then we collect methods, and add only these that aren't there yet
			CollectMethods();

			void CollectProperties()
			{
				var propertiesFound = type.GetProperties(Flags);
				foreach (var property in propertiesFound)
				{
					AddProperty(property);
				}
			}

			void CollectEvents()
			{
				var eventsFound = type.GetEvents(Flags);
				foreach (var @event in eventsFound)
				{
					AddEvent(@event);
				}
			}

			void CollectMethods()
			{
				var methodsFound = MethodFinder.GetAllInstanceMethods(type, Flags);
				foreach (var method in methodsFound)
				{
					AddMethod(method, true);
				}
			}

			void AddProperty(PropertyInfo property)
			{
				MetaMethod getter = null;
				MetaMethod setter = null;

				if (property.CanRead)
				{
					var getMethod = property.GetGetMethod(true);
					getter = AddMethod(getMethod, false);
				}

				if (property.CanWrite)
				{
					var setMethod = property.GetSetMethod(true);
					setter = AddMethod(setMethod, false);
				}

				if (setter == null && getter == null)
				{
					return;
				}

				var nonInheritableAttributes = property.GetNonInheritableAttributes();
				var arguments = property.GetIndexParameters();

				sink.Add(new MetaProperty(property.Name,
				                          property.PropertyType,
				                          property.DeclaringType,
				                          getter,
				                          setter,
				                          nonInheritableAttributes.Select(a => a.Builder),
				                          arguments.Select(a => a.ParameterType).ToArray()));
			}

			void AddEvent(EventInfo @event)
			{
				var addMethod = @event.GetAddMethod(true);
				var removeMethod = @event.GetRemoveMethod(true);
				MetaMethod adder = null;
				MetaMethod remover = null;

				if (addMethod != null)
				{
					adder = AddMethod(addMethod, false);
				}

				if (removeMethod != null)
				{
					remover = AddMethod(removeMethod, false);
				}

				if (adder == null && remover == null)
				{
					return;
				}

				sink.Add(new MetaEvent(@event.Name,
				                       @event.DeclaringType, @event.EventHandlerType, adder, remover, EventAttributes.None));
			}

			MetaMethod AddMethod(MethodInfo method, bool isStandalone)
			{
				if (checkedMethods.Add(method) == false)
				{
					return null;
				}

				var methodToGenerate = GetMethodToGenerate(method, hook, isStandalone);
				if (methodToGenerate != null)
				{
					sink.Add(methodToGenerate);
				}

				return methodToGenerate;
			}
		}

		protected abstract MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone);

		/// <summary>
		///   Performs some basic screening and invokes the <see cref = "IProxyGenerationHook" />
		///   to select methods.
		/// </summary>
		protected bool AcceptMethod(MethodInfo method, bool onlyVirtuals, IProxyGenerationHook hook)
		{
			return AcceptMethodPreScreen(method, onlyVirtuals, hook) && hook.ShouldInterceptMethod(type, method);
		}

		/// <summary>
		///   Performs some basic screening to filter out non-interceptable methods.
		/// </summary>
		/// <remarks>
		///   The <paramref name="hook"/> will get invoked for non-interceptable method notification only;
		///   it does not get asked whether or not to intercept the <paramref name="method"/>.
		/// </remarks>
		protected bool AcceptMethodPreScreen(MethodInfo method, bool onlyVirtuals, IProxyGenerationHook hook)
		{
			if (IsInternalAndNotVisibleToDynamicProxy(method))
			{
				return false;
			}

			var isOverridable = method.IsVirtual && !method.IsFinal;
			if (onlyVirtuals && !isOverridable)
			{
				if (method.DeclaringType != typeof(MarshalByRefObject) &&
					method.IsGetType() == false &&
					method.IsMemberwiseClone() == false)
				{
					Logger.DebugFormat("Excluded non-overridable method {0} on {1} because it cannot be intercepted.", method.Name,
					                   method.DeclaringType.FullName);
					hook.NonProxyableMemberNotification(type, method);
				}
				return false;
			}

			// we can never intercept a sealed (final) method
			if (method.IsFinal)
			{
				Logger.DebugFormat("Excluded sealed method {0} on {1} because it cannot be intercepted.", method.Name,
				                   method.DeclaringType.FullName);
				return false;
			}

			//can only proxy methods that are public or protected (or internals that have already been checked above)
			if ((method.IsPublic || method.IsFamily || method.IsAssembly || method.IsFamilyOrAssembly || method.IsFamilyAndAssembly) == false)
			{
				return false;
			}

			if (method.DeclaringType == typeof(MarshalByRefObject))
			{
				return false;
			}

			if (method.IsFinalizer())
			{
				return false;
			}

			return true;
		}

		private static bool IsInternalAndNotVisibleToDynamicProxy(MethodInfo method)
		{
			return ProxyUtil.IsInternal(method) &&
				   ProxyUtil.AreInternalsVisibleToDynamicProxy(method.DeclaringType.Assembly) == false;
		}
	}
}
// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
// limitations under the License.;

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ServiceModel;
	using Castle.Core;
	using Castle.MicroKernel;

	internal static class WcfUtils
	{
		public static bool IsHosted(IWcfServiceModel serviceModel)
		{
			return serviceModel.IsHosted;
		}

		public static IEnumerable<T> FindDependencies<T>(IDictionary dependencies)
		{
			return FindDependencies<T>(dependencies, null);
		}

		public static IEnumerable<T> FindDependencies<T>(IDictionary dependencies, 
			                                             Predicate<T> test)
		{
			foreach (object dependency in dependencies.Values)
			{
				if (dependency is T)
				{
					T candidate = (T)dependency;

					if (test == null || test(candidate))
					{
						yield return candidate;
					}
				}
				else if (dependency is IEnumerable<T>)
				{
					foreach (T item in (IEnumerable<T>)dependency)
					{
						yield return item;
					}
				}
			}
		}

		public static ICollection<IHandler> FindBehaviors<T>(IKernel kernel, WcfBehaviorScope scope)
		{
			List<IHandler> handlers = new List<IHandler>();
			foreach (IHandler handler in kernel.GetAssignableHandlers(typeof(T)))
			{
				ComponentModel model = handler.ComponentModel;
				if (model.Configuration != null)
				{
					string scopeAttrib = model.Configuration.Attributes[WcfConstants.BehaviorScopeKey];
					if (string.IsNullOrEmpty(scopeAttrib) ||
						scopeAttrib.Equals(scope.ToString(), StringComparison.InvariantCultureIgnoreCase))
					{
						handlers.Add(handler);
					}
				}
			}
			return handlers;
		}

		public static void AddBehaviorDependencies<T>(IKernel kernel, WcfBehaviorScope scope, ComponentModel model)
		{
			foreach (IHandler handler in FindBehaviors<T>(kernel, scope))
			{
				AddBehaviorDependency(null, handler.ComponentModel.Service, model);
			}
		}

		public static void AddBehaviorDependency(string dependencyKey, Type serviceType, ComponentModel model)
		{
			model.Dependencies.Add(new DependencyModel(DependencyType.Service, dependencyKey, serviceType, false));
		}

		public static bool IsCommunicationObjectReady(ICommunicationObject comm)
		{
			switch (comm.State)
			{
				case CommunicationState.Closed:
				case CommunicationState.Closing:
				case CommunicationState.Faulted:
					return false;
			}
			return true;
		}

		public static void ReleaseCommunicationObject(ICommunicationObject comm)
		{
			if (comm != null)
			{
				if (comm.State != CommunicationState.Faulted)
				{
					try
					{
						comm.Close();
					}
					catch (CommunicationException)
					{
						comm.Abort();
					}
					catch (TimeoutException)
					{
						comm.Abort();
					}
					catch
					{
						comm.Abort();
						throw;
					}
				}
				else
				{
					comm.Abort();
				}
			}
		}
	}
}

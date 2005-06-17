// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Summary description for MDefaultServer.
	/// </summary>
	public class MDefaultServer : MarshalByRefObject, MServer
	{
		protected MRegistry registry;

		public MDefaultServer()
		{
			// TODO: Allow customisation of MRegistry

			registry = new Default.MDefaultRegistry(this);

			SetupRegistry();
		}

		private void SetupRegistry()
		{
			RegisterManagedObject(registry, MConstants.REGISTRY_NAME);
		}

		#region MServer Members

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public Object Instantiate(String typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}

			Type targetType = Type.GetType(typeName, true, true);

			object instance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
				targetType.Assembly.FullName, targetType.FullName);

			return instance;
		}

		/// <summary>
		/// Instantiates the specified type using the server domain.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public Object Instantiate(String assemblyName, String typeName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}

			object instance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
				assemblyName, typeName);

			return instance;
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public ManagedInstance CreateManagedObject(String typeName, ManagedObjectName name)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			Object instance = Instantiate(typeName);
			return RegisterManagedObject(instance, name);
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <param name="typeName"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public ManagedInstance CreateManagedObject(String assemblyName, String typeName, ManagedObjectName name)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			Object instance = Instantiate(assemblyName, typeName);
			return RegisterManagedObject(instance, name);
		}


		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		/// <returns></returns>
        public ManagedInstance RegisterManagedObject(Object instance, ManagedObjectName name)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			return registry.RegisterManagedObject(instance, name);
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ManagedInstance GetManagedInstance(ManagedObjectName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			return registry.GetManagedInstance(name);
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		public void UnregisterManagedObject(ManagedObjectName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			registry.UnregisterManagedObject(name);
		}

		/// <summary>
		/// Invokes an action in managed object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="action"></param>
		/// <param name="args"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		public Object Invoke(ManagedObjectName name, String action, Object[] args, Type[] signature)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			return registry.Invoke(name, action, args, signature);
		}

		/// <summary>
		/// Returns the info (attributes and operations) about the specified object.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		public ManagementInfo GetManagementInfo(ManagedObjectName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			
			return registry.GetManagementInfo(name);
		}

		/// <summary>
		/// Gets an attribute value of the specified managed object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		public Object GetAttribute(ManagedObjectName name, String attributeName)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (attributeName == null)
			{
				throw new ArgumentNullException("attributeName");
			}

			return registry.GetAttributeValue(name, attributeName);
		}

		/// <summary>
		/// Sets an attribute value of the specified managed object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		public void SetAttribute(ManagedObjectName name, String attributeName, Object attributeValue)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (attributeName == null)
			{
				throw new ArgumentNullException("attributeName");
			}
			
			registry.SetAttributeValue(name, attributeName, attributeValue);
		}

		/// <summary>
		/// Returns an array of registered domains.
		/// </summary>
		/// <returns>a list of domains</returns>
		public String[] GetDomains()
		{
			return registry.GetDomains();
		}

		/// <summary>
		/// Queries the registerd components.
		/// </summary>
		/// <returns></returns>
		public ManagedObjectName[] Query(ManagedObjectName query)
		{
			return registry.Query(query);
		}

		#endregion
	}
}

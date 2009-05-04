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

namespace Castle.ManagementExtensions
{
	using System;

	/// <summary>
	/// Summary description for MServer.
	/// </summary>
	public interface MServer : MServerConnection
	{
		/// <summary>
		/// Instantiates the specified type using the server domain.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		Object Instantiate(String typeName);

		/// <summary>
		/// Instantiates the specified type using the server domain.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		Object Instantiate(String assemblyName, String typeName);

		/// <summary>
		/// Instantiates the specified managed object.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		ManagedInstance CreateManagedObject(String typeName, ManagedObjectName name);

		/// <summary>
		/// Instantiates the specified managed object.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		ManagedInstance CreateManagedObject(String assemblyName, String typeName, ManagedObjectName name);

		/// <summary>
		/// Registers the specified managed object instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		ManagedInstance RegisterManagedObject(Object instance, ManagedObjectName name);
		
		/// <summary>
		/// Returns a <see cref="ManagedInstance"/> representing 
		/// a managed object instance.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		ManagedInstance GetManagedInstance(ManagedObjectName name);

		/// <summary>
		/// Unregister a managed object from the domain.
		/// </summary>
		/// <param name="name"></param>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		void UnregisterManagedObject(ManagedObjectName name);

		/// <summary>
		/// Invokes an action in managed object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="action"></param>
		/// <param name="args"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		Object Invoke(ManagedObjectName name, String action, Object[] args, Type[] signature);

		/// <summary>
		/// Returns the info (attributes and operations) about the specified object.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		ManagementInfo GetManagementInfo(ManagedObjectName name);

		/// <summary>
		/// Gets an attribute value of the specified managed object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		Object GetAttribute(ManagedObjectName name, String attributeName);

		/// <summary>
		/// Sets an attribute value of the specified managed object.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		/// <exception cref="InvalidDomainException">If domain name is not found.</exception>
		void SetAttribute(ManagedObjectName name, String attributeName, Object attributeValue);

		/// <summary>
		/// Returns an array of registered domains.
		/// </summary>
		/// <returns>a list of domains</returns>
		// String[] GetDomains();

		/// <summary>
		/// Queries the registerd components.
		/// </summary>
		/// <returns></returns>
		ManagedObjectName[] Query(ManagedObjectName query);
	}
}

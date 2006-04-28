// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using System;

	using Castle.MicroKernel.Facilities;
	
	using NHibernate;

	/// <summary>
	/// Dictates the contract for possible different approach 
	/// of session factories obtention.
	/// </summary>
	/// <remarks>
	/// Inspired on Cuyahoga project
	/// </remarks>
	public interface ISessionFactoryResolver
	{
		/// <summary>
		/// Invoked by the facility while the configuration 
		/// node are being interpreted.
		/// </summary>
		/// <param name="alias">
		/// The alias associated with the session factory on the configuration node
		/// </param>
		/// <param name="componentKey">
		/// The component key associated with the session factory on the kernel
		/// </param>
		void RegisterAliasComponentIdMapping(String alias, String componentKey);

		/// <summary>
		/// Implementors should return a session factory 
		/// instance for the specified alias configured previously.
		/// </summary>
		/// <param name="alias">
		/// The alias associated with the session factory on the configuration node
		/// </param>
		/// <returns>
		/// A session factory instance
		/// </returns>
		/// <exception cref="FacilityException">
		/// If the alias is not associated with a session factory
		/// </exception>
		ISessionFactory GetSessionFactory(String alias);
	}
}

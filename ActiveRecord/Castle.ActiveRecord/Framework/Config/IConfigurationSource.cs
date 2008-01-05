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
// limitations under the License.

namespace Castle.ActiveRecord.Framework
{
	using System;
	using Castle.Core.Configuration;

	/// <summary>
	/// Abstracts the source of configuration for the framework.
	/// </summary>
	public interface IConfigurationSource
	{
		/// <summary>
		/// Implementors should return the type that implements
		/// the interface <see cref="IThreadScopeInfo"/>
		/// </summary>
		Type ThreadScopeInfoImplementation { get; }

		/// <summary>
		/// Implementors should return the type that implements 
		/// the interface <see cref="ISessionFactoryHolder"/>
		/// </summary>
		Type SessionFactoryHolderImplementation { get; }

		/// <summary>
		/// Implementors should return the type that implements
		/// the interface NHibernate.Cfg.INamingStrategy
		/// </summary>
		Type NamingStrategyImplementation { get; }

		/// <summary>
		/// Implementors should return an <see cref="IConfiguration"/> 
		/// instance
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IConfiguration GetConfiguration(Type type);

		/// <summary>
		/// Gets a value indicating whether this <see cref="IConfigurationSource"/> produce debug information
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		bool Debug { get; }

		/// <summary>
		/// Gets a value indicating whether the entities should be lazy by default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if entities should be lazy by default; otherwise, <c>false</c>.
		/// </value>
		bool IsLazyByDefault { get; }

		/// <summary>
		/// Gets a value indicating whether table names are assumed plural by default. 
		/// </summary>
		/// <value>
		/// 	<c>true</c> if table names should be pluralized by default; otherwise, <c>false</c>.
		/// </value>
		bool PluralizeTableNames { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the models should be verified against the db schema on Initialisation.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if models should be verified; otherwise, <c>false</c>.
		/// </value>
		bool VerifyModelsAgainstDBSchema { get; }

	}
}

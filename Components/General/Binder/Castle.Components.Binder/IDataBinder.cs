// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Binder
{
	using System;

	public delegate void BinderHandler(object instance, String prefix, Node node);

	/// <summary>
	/// Defines the contract for a data binder implementation approach.
	/// </summary>
	public interface IDataBinder
	{
		bool CanBindParameter(Type desiredType, String paramName, CompositeNode treeRoot);

		bool CanBindObject(Type targetType, String prefix, CompositeNode treeRoot);

		object BindParameter(Type targetType, String paramName, CompositeNode treeRoot);
		
		/// <summary>
		/// Create an instance of the specified type and binds the properties that
		/// are available on the datasource.
		/// </summary>
		/// <param name="targetType">The target type. Can be an array</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="treeRoot">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		object BindObject(Type targetType, String prefix, CompositeNode treeRoot);

		/// <summary>
		/// Create an instance of the specified type and binds the properties that
		/// are available on the datasource respecting the white and black list
		/// </summary>
		/// <param name="targetType">The target type. Can be an array</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="excludedProperties">A list of comma separated values specifing the properties that should be ignored</param>
		/// <param name="allowedProperties">A list of comma separated values specifing the properties that should not be ignored</param>
		/// <param name="treeRoot">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		object BindObject(Type targetType, String prefix, String excludedProperties, String allowedProperties, CompositeNode treeRoot);

		/// <summary>
		/// Binds the properties that are available on the datasource to the specified object instance.
		/// </summary>
		/// <param name="instance">The target instance.</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="treeRoot">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		void BindObjectInstance(object instance, String prefix, CompositeNode treeRoot);

		/// <summary>
		/// Binds the properties that
		/// are available on the datasource respecting the white and black list
		/// </summary>
		/// <param name="instance">The target type.</param>
		/// <param name="prefix">The obligatory prefix that distinguishes it on the datasource</param>
		/// <param name="excludedProperties">A list of comma separated values specifing the properties that should be ignored</param>
		/// <param name="allowedProperties">A list of comma separated values specifing the properties that should not be ignored</param>
		/// <param name="treeRoot">A hierarchycal representation of flat data</param>
		/// <returns>an instance of the specified target type</returns>
		void BindObjectInstance(object instance, String prefix, String excludedProperties, String allowedProperties, CompositeNode treeRoot);

		/// <summary>
		/// Represents the databind errors
		/// </summary>
		ErrorList ErrorList { get; }

		/// <summary>
		/// Exposes the <see cref="IBinderTranslator"/> implementation
		/// if one was provided
		/// </summary>
		IBinderTranslator Translator { get; set; }
		
		/// <summary>
		/// Exposes the <see cref="IConverter"/> implementation
		/// </summary>
		IConverter Converter { get; set; }
		
		/// <summary>
		/// Invoked before the data binder implementation starts to
		/// work on a class instance
		/// </summary>
		event BinderHandler OnBeforeBinding;
		
		/// <summary>
		/// Invoked after the data binder implementation starts to
		/// work on a class instance
		/// </summary>
		event BinderHandler OnAfterBinding;
	}
}
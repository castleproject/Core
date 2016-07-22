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


namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
#if FEATURE_DICTIONARYADAPTER_XML
	using System.Collections.Specialized;
	using System.Xml.XPath;
#endif

	/// <summary>
	/// Defines the contract for building typed dictionary adapters.
	/// </summary>
	public interface IDictionaryAdapterFactory
	{
		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		T GetAdapter<T>(IDictionary dictionary);

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		object GetAdapter(Type type, IDictionary dictionary);

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="IDictionary"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="dictionary">The underlying source of properties.</param>
		/// <param name="descriptor">The property descriptor.</param>
		/// <returns>An implementation of the typed interface bound to the dictionary.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		object GetAdapter(Type type, IDictionary dictionary, PropertyDescriptor descriptor);

#if FEATURE_DICTIONARYADAPTER_XML
		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		T GetAdapter<T>(NameValueCollection nameValues);

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="NameValueCollection"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="nameValues">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the namedValues.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		object GetAdapter(Type type, NameValueCollection nameValues);

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="System.Xml.XmlNode"/>.
		/// </summary>
		/// <typeparam name="T">The typed interface.</typeparam>
		/// <param name="xmlNode">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the <see cref="System.Xml.XmlNode"/>.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		T GetAdapter<T>(System.Xml.XmlNode xmlNode);

		/// <summary>
		/// Gets a typed adapter bound to the <see cref="System.Xml.XmlNode"/>.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="xmlNode">The underlying source of properties.</param>
		/// <returns>An implementation of the typed interface bound to the <see cref="System.Xml.XmlNode"/>.</returns>
		/// <remarks>
		/// The type represented by T must be an interface with properties.
		/// </remarks>
		object GetAdapter(Type type, System.Xml.XmlNode xmlNode);
#endif

		/// <summary>
		/// Gets the <see cref="DictionaryAdapterMeta"/> associated with the type.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <returns>The adapter meta-data.</returns>
		DictionaryAdapterMeta GetAdapterMeta(Type type);

		/// <summary>
		/// Gets the <see cref="DictionaryAdapterMeta"/> associated with the type.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="descriptor">The property descriptor.</param>
		/// <returns>The adapter meta-data.</returns>
		DictionaryAdapterMeta GetAdapterMeta(Type type, PropertyDescriptor descriptor);

		/// <summary>
		/// Gets the <see cref="DictionaryAdapterMeta"/> associated with the type.
		/// </summary>
		/// <param name="type">The typed interface.</param>
		/// <param name="other">Another <see cref="DictionaryAdapterMeta"/> from which to copy behaviors.</param>
		/// <returns>The adapter meta-data.</returns>
		DictionaryAdapterMeta GetAdapterMeta(Type type, DictionaryAdapterMeta other);
	}
}

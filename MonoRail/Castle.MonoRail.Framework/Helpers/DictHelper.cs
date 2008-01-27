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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Helper used to create <see cref="IDictionary"/> instances
	/// </summary>
	public class DictHelper : AbstractHelper
	{
		#region MonoRailDictionary inner class
		/// <summary>
		/// Helper (for the Helper) used to create <see cref="IDictionary"/> instances.
		/// 
		/// </summary>
		public class MonoRailDictionary : HybridDictionary
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="MonoRailDictionary"/> class.
			/// Forces case Insensitivty.
			/// </summary>
			public MonoRailDictionary() : base(true) { }


			// Removed. See explaination in outer class.
			//public MonoRailDictionary N<T>(string key, T value)
			//{
			//    this[key] = value.ToString();
			//    return (this);
			//}

			/// <summary>
			/// Adds the specified key &amp; value to the collection.
			/// </summary>
			/// <remarks>Usuable in placed where the generic version is not available.</remarks>
			/// <param name="key">The key.</param>
			/// <param name="value">The value.</param>
			/// <returns>itself, to allow for chaning.</returns>
			public MonoRailDictionary N(string key, object value)
			{
				this[key] = value.ToString();
				return (this);
			}

			/// <summary>
			/// Adds the specified key to the collection.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <returns>itself, to allow for chaning.</returns>
			public MonoRailDictionary N(string key)
			{
				this[key] = "";
				return (this);
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DictHelper"/> class.
		/// </summary>
		public DictHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DictHelper"/> class.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public DictHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		/// <summary>
		/// Creates an <see cref="IDictionary"/> with entries
		/// infered from the arguments. 
		/// </summary>
		/// <example>
		/// <code>
		/// helper.CreateDict( "style=display: none;", "selected" )
		/// </code></example>
		/// <param name="args"></param>
		/// <returns>an <see cref="IDictionary"/></returns>
		public MonoRailDictionary CreateDict(params String[] args)
		{
			return Create(args);
		}

		/// <summary>
		/// Creates a dictionary from specified arguments.
		/// </summary>
		/// <remarks> Returns a MonoRailDictionary object, 
		/// which is an IDictionary like previous versions of this method.
		/// </remarks>
		/// <example>
		/// <code>
		/// DictHelper.Create( "style=display: none;", "selected" )
		/// </code></example>
		/// <param name="args">The arguments.</param>
		/// <returns>an <see cref="IDictionary"/></returns>
		public static MonoRailDictionary Create(params String[] args)
		{
			MonoRailDictionary dict = new MonoRailDictionary();
			foreach (String arg in args)
			{
				int pos = arg.IndexOf('=');

				if (pos == -1)
				{
					dict[arg] = "";
				}
				else
				{
					dict[arg.Substring(0, pos)] = arg.Substring(pos + 1);
				}
			}
			return dict;
		}

		///// <summary>
		///// Create a new collections and adds the specified key &amp; value.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="key">The key.</param>
		///// <param name="value">The value.</param>
		///// <returns></returns>
		//
		// Generic version can't be used in an nVelocity template, 
		// and the non-generic, object version (below) would need a 
		// different name to avoid conflict, so I figured I'd
		// just get rid of it, even thought it's slightly faster.
		//public MonoRailDictionary N<T>(string key, T value)
		//{
		//    MonoRailDictionary helper = new MonoRailDictionary();
		//    helper[key] = value.ToString();
		//    return helper;
		//}

		/// <summary>
		/// Creates an <see cref="IDictionary"/> and adds the 
		/// specified key &amp; value to the collection.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns>an <see cref="IDictionary"/></returns>
		public MonoRailDictionary N(string name, object value)
		{
			return CreateN(name, value);
		}

		/// <summary>
		/// Creates an <see cref="IDictionary"/> and adds the 
		/// specified key &amp; value to the collection.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>an <see cref="IDictionary"/></returns>
		public static MonoRailDictionary CreateN(string key, object value)
		{
			MonoRailDictionary helper = new MonoRailDictionary();
			helper[key] = value.ToString();
			return helper;
		}

		/// <summary>
		/// Creates a dictionary froms a name value collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		public IDictionary FromNameValueCollection(NameValueCollection collection)
		{
			IDictionary dict = new MonoRailDictionary();

			foreach(string key in collection.AllKeys)
			{
				if (key != null)
				{
					dict[key] = collection.GetValues(key);
				}
			}
			
			return dict;
		}
	}
}

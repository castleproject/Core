// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization; 

	/// <summary>
	/// Default implementation of IContext.
	/// </summary>
	public class DefaultContext : IContext
	{
		private static Hidden HIDDEN_MAKER = new Hidden();

		private IDictionary m_contextData;
		private IContext m_parent;
		private bool m_readOnly;

		/// <summary>
		/// Create a Context with specified data and parent.
		/// </summary>
		/// <param name="contextData">the context data</param>
		/// <param name="parent">the parent Context (may be null)</param>
		public DefaultContext( IDictionary contextData, IContext parent )
		{
			m_parent = parent;
			m_contextData = contextData;
		}

		/// <summary>
		/// Create a Context with specified data.
		/// </summary>
		/// <param name="contextData">the context data</param>
		public DefaultContext( IDictionary contextData ) : this( contextData, null )
		{
		}

		/// <summary>
		/// Create a Context with specified parent.
		/// </summary>
		/// <param name="parent">the parent Context (may be null)</param>
		public DefaultContext( IContext parent ) 
			: this( Hashtable.Synchronized( new Hashtable() ), parent )
		{
		}

		/// <summary>
		/// Create a Context with no parent.
		/// </summary>
		public DefaultContext() : this( (IContext) null )
		{
		}

		#region IContext Members

		/// <summary>
		/// Retrieve an item from the Context.
		/// </summary>
		/// <param name="key">the key of item</param>
		/// <returns>the item stored in context</returns>
		/// <exception cref="ContextException">if item not present</exception>
		public object this[ object key ]
		{
			get 
			{
				object data = m_contextData[key];

				if( null != data )
				{
					if( data is Hidden )
					{
						// Always fail.
						string message = String.Format("Unable to locate {0}", key);
						throw new ContextException( message );
					}

					if( data is IResolvable )
					{
						return ( (IResolvable)data ).Resolve( this );
					}

					return data;
				}

				// If data was null, check the parent
				if( null == m_parent )
				{
					// There was no parent, and no data
					string message = String.Format("Unable to resolve context key {0}", key);
					throw new ContextException( message );
				}

				return m_parent[ key ];
			}
		}

		#endregion

		[Serializable]
			private sealed class Hidden
		{
		}

		/// <summary>
		/// Helper method fo adding items to Context.
		/// </summary>
		/// <param name="key">the items key</param>
		/// <param name="value">the item</param>
		/// <exception cref="ContextException">if context is read only</exception>
		public void Put( object key, object value )
		{
			CheckWriteable();
			if( null == value )
			{
				m_contextData.Remove( key );
			}
			else
			{
				m_contextData[ key ] = value;
			}
		}

		/// <summary>
		/// Hides the item in the context.
		/// After Hide(key) has been called, a Get(key)
		/// will always fail, even if the parent context
		/// has such a mapping.
		/// </summary>
		/// <param name="key">the items key</param>
		/// <exception cref="ContextException">if context is read only</exception>
		public void Hide( object key )
		{
			CheckWriteable();
			m_contextData[ key ] = HIDDEN_MAKER;
		}

		/// <summary>
		/// Utility method to retrieve context data.
		/// </summary>
		/// <returns>the context data</returns>
		protected IDictionary GetContextData()
		{
			return m_contextData;
		}

		/// <summary>
		/// Get parent context if any.
		/// </summary>
		/// <returns>the parent Context (may be null)</returns>
		protected IContext Parent
		{
			get
			{
				return m_parent;
			}
		}

		/// <summary>
		/// Make the context read-only.
		/// Any attempt to write to the context via Add()
		/// will result in an IllegalStateException.
		/// </summary>
		public void MakeReadOnly()
		{
			m_readOnly = true;
		}

		/// <summary>
		/// Utility method to check if context is writeable and if not throw exception.
		/// </summary>
		/// <exception cref="ContextException">if context is read only</exception>
		protected void CheckWriteable()
		{
			if( m_readOnly )
			{
				string message = "Context is read only and can not be modified";
				throw new ContextException( message );
			}
		}
	}
}

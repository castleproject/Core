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

namespace NVelocity.Context
{
	using System;
	using System.Collections;
	using NVelocity.App.Events;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util.Introspection;

	/// <summary>  This adapter class is the container for all context types for internal
	/// use.  The AST now uses this class rather than the app-level Context
	/// interface to allow flexibility in the future.
	/// *
	/// Currently, we have two context interfaces which must be supported :
	/// <ul>
	/// <li> Context : used for application/template data access</li>
	/// <li> InternalHousekeepingContext : used for internal housekeeping and caching</li>
	/// <li> InternalWrapperContext : used for getting root cache context and other
	/// such.</li>
	/// <li> InternalEventContext : for event handling.</li>
	/// </ul>
	/// *
	/// This class implements the two interfaces to ensure that all methods are
	/// supported.  When adding to the interfaces, or adding more context
	/// functionality, the interface is the primary definition, so alter that first
	/// and then all classes as necessary.  As of this writing, this would be
	/// the only class affected by changes to InternalContext
	/// *
	/// This class ensures that an InternalContextBase is available for internal
	/// use.  If an application constructs their own Context-implementing
	/// object w/o sub-classing AbstractContext, it may be that support for
	/// InternalContext is not available.  Therefore, InternalContextAdapter will
	/// create an InternalContextBase if necessary for this support.  Note that
	/// if this is necessary, internal information such as node-cache data will be
	/// lost from use to use of the context.  This may or may not be important,
	/// depending upon application.
	///
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: InternalContextAdapterImpl.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	public class InternalContextAdapterImpl : IInternalContextAdapter
	{
		///
		/// <summary>  the user data Context that we are wrapping
		/// </summary>
		internal IContext context = null;

		///
		/// <summary>  the ICB we are wrapping.  We may need to make one
		/// if the user data context implementation doesn't
		/// support one.  The default AbstractContext-derived
		/// VelocityContext does, and it's recommended that
		/// people derive new contexts from AbstractContext
		/// rather than piecing things together
		/// </summary>
		internal IInternalHousekeepingContext internalHousekeepingContext = null;

		/// <summary>  The InternalEventContext that we are wrapping.  If
		/// the context passed to us doesn't support it, no
		/// biggie.  We don't make it for them - since its a
		/// user context thing, nothing gained by making one
		/// for them now
		/// </summary>
		internal IInternalEventContext internalEventContext = null;

		/// <summary>  CTOR takes a Context and wraps it, delegating all 'data' calls
		/// to it.
		///
		/// For support of internal contexts, it will create an InternalContextBase
		/// if need be.
		/// </summary>
		public InternalContextAdapterImpl(IContext c)
		{
			context = c;

			if (c is IInternalHousekeepingContext)
			{
				internalHousekeepingContext = (IInternalHousekeepingContext) context;
			}
			else
			{
				internalHousekeepingContext = new InternalContextBase();
			}

			IInternalEventContext internalEventContext = context as IInternalEventContext;
			if (internalEventContext != null)
			{
				this.internalEventContext = internalEventContext;
			}
		}

		public String CurrentTemplateName
		{
			get { return internalHousekeepingContext.CurrentTemplateName; }
		}

		public Object[] TemplateNameStack
		{
			get { return internalHousekeepingContext.TemplateNameStack; }
		}

		public Resource CurrentResource
		{
			get { return internalHousekeepingContext.CurrentResource; }

			set { internalHousekeepingContext.CurrentResource = value; }
		}

		public Object[] Keys
		{
			get { return context.Keys; }
		}

		ICollection IDictionary.Keys
		{
			get { return context.Keys; }
		}

		public ICollection Values
		{
			get
			{
				object[] keys = Keys;
				object[] values = new object[keys.Length];

				for(int i = 0; i < values.Length; i++)
				{
					values[i] = Get(keys[i].ToString());
				}

				return values;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public object this[object key]
		{
			get { return Get(key.ToString()); }
			set { Put(key.ToString(), value); }
		}

		public IContext InternalUserContext
		{
			get { return context; }
		}

		public IInternalContextAdapter BaseContext
		{
			get { return this; }
		}

		public EventCartridge EventCartridge
		{
			get
			{
				if (internalEventContext != null)
				{
					return internalEventContext.EventCartridge;
				}

				return null;
			}
		}

		/* --- InternalHousekeepingContext interface methods --- */

		public void PushCurrentTemplateName(String s)
		{
			internalHousekeepingContext.PushCurrentTemplateName(s);
		}

		public void PopCurrentTemplateName()
		{
			internalHousekeepingContext.PopCurrentTemplateName();
		}

		public IntrospectionCacheData ICacheGet(Object key)
		{
			return internalHousekeepingContext.ICacheGet(key);
		}

		public void ICachePut(Object key, IntrospectionCacheData o)
		{
			internalHousekeepingContext.ICachePut(key, o);
		}


		/* ---  Context interface methods --- */

		public Object Put(String key, Object value)
		{
			return context.Put(key, value);
		}

		public Object Get(String key)
		{
			return context.Get(key);
		}

		public bool ContainsKey(Object key)
		{
			return context.ContainsKey(key);
		}


		public Object Remove(Object key)
		{
			return context.Remove(key);
		}


		/* ---- InternalWrapperContext --- */

		/// <summary>  returns the user data context that
		/// we are wrapping
		/// </summary>
		/// <summary>  Returns the base context that we are
		/// wrapping. Here, its this, but for other thing
		/// like VM related context contortions, it can
		/// be something else
		/// </summary>
		/* -----  InternalEventContext ---- */
		public EventCartridge AttachEventCartridge(EventCartridge eventCartridge)
		{
			if (internalEventContext != null)
			{
				return internalEventContext.AttachEventCartridge(eventCartridge);
			}

			return null;
		}

		void IDictionary.Remove(Object key)
		{
			context.Remove(key);
		}

		public void CopyTo(Array array, int index)
		{
			foreach(object value in Values)
			{
				array.SetValue(value, index++);
			}
		}

		public int Count
		{
			get { return context.Count; }
		}

		public object SyncRoot
		{
			get { return context; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public bool Contains(object key)
		{
			return context.ContainsKey(key);
		}

		public void Add(object key, object value)
		{
			context.Put(key.ToString(), value);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return CreateEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return CreateEnumerator();
		}

		private InternalContextAdapterImplEnumerator CreateEnumerator()
		{
			return new InternalContextAdapterImplEnumerator(context);
		}
	}

	public class InternalContextAdapterImplEnumerator : IDictionaryEnumerator
	{
		private int index = -1;
		private readonly IContext context;
		private readonly object[] keys;

		public InternalContextAdapterImplEnumerator(IContext context)
		{
			this.context = context;
			keys = context.Keys;
		}

		#region IDictionaryEnumerator Members

		public object Key
		{
			get { return keys[index]; }
		}

		public object Value
		{
			get { return context.Get(keys[index].ToString()); }
		}

		public DictionaryEntry Entry
		{
			get { return new DictionaryEntry(Key, Value); }
		}

		#endregion

		#region IEnumerator Members

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get { return Entry; }
		}

		public bool MoveNext()
		{
			return ++index < keys.Length;
		}

		#endregion
	}
}
namespace NVelocity.Context
{
	using System;
	using System.Collections;

	using NVelocity.App.Events;
	using NVelocity.Runtime;
	using NVelocity.Runtime.Directive;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util.Introspection;

	/// <summary>  This is a special, internal-use-only context implementation to be
	/// used for the new Velocimacro implementation.
	/// *
	/// The main distinguishing feature is the management of the VMProxyArg objects
	/// in the put() and get() methods.
	/// *
	/// Further, this context also supports the 'VM local context' mode, where
	/// any get() or put() of references that aren't args to the VM are considered
	/// local to the vm, protecting the global context.
	///
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: VMContext.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	public class VMContext : IInternalContextAdapter
	{
		private void InitBlock()
		{
			vmproxyhash = new Hashtable();
			localcontext = new Hashtable();
		}

		public IContext InternalUserContext
		{
			get { return innerContext.InternalUserContext; }
		}

		public IInternalContextAdapter BaseContext
		{
			get { return innerContext.BaseContext; }
		}

		public Object[] Keys
		{
			get
			{
				//return vmproxyhash.keySet().toArray();
				throw new NotImplementedException();
			}

		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}
		ICollection IDictionary.Keys
		{
			get { throw new NotImplementedException(); } 			
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public object this[ object key ]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public String CurrentTemplateName
		{
			get { return innerContext.CurrentTemplateName; }

		}

		public Object[] TemplateNameStack
		{
			get { return innerContext.TemplateNameStack; }

		}

		public EventCartridge EventCartridge
		{
			get { return innerContext.EventCartridge; }

		}

		public Resource CurrentResource
		{
			get { return innerContext.CurrentResource; }

			set { innerContext.CurrentResource = value; }

		}

		/// <summary>container for our VMProxy Objects
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'vmproxyhash' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		internal Hashtable vmproxyhash;

		/// <summary>container for any local or constant VMProxy items
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'localcontext' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		internal Hashtable localcontext;

		/// <summary>the base context store.  This is the 'global' context
		/// </summary>
		internal IInternalContextAdapter innerContext = null;

		/// <summary>context that we are wrapping
		/// </summary>
		internal IInternalContextAdapter wrappedContext = null;

		/// <summary>support for local context scope feature, where all references are local
		/// </summary>
		private bool localcontextscope = false;

		/// <summary>  CTOR, wraps an ICA
		/// </summary>
		public VMContext(IInternalContextAdapter inner, IRuntimeServices rsvc)
		{
			InitBlock();
			localcontextscope = rsvc.GetBoolean(RuntimeConstants.VM_CONTEXT_LOCALSCOPE, false);

			wrappedContext = inner;
			innerContext = inner.BaseContext;
		}

		/// <summary>  return the inner / user context
		/// </summary>
		/// <summary>  Used to put VMProxyArgs into this context.  It separates
		/// the VMProxyArgs into constant and non-constant types
		/// pulling out the value of the constant types so they can
		/// be modified w/o damaging the VMProxyArg, and leaving the
		/// dynamic ones, as they modify context rather than their own
		/// state
		/// </summary>
		/// <param name="vmpa">VMProxyArg to add
		///
		/// </param>
		public void AddVMProxyArg(VMProxyArg vmpa)
		{
			/*
	    *  ask if it's a constant : if so, get the value and put into the
	    *  local context, otherwise, put the vmpa in our vmproxyhash
	    */

			String key = vmpa.ContextReference;

			if (vmpa.isConstant())
			{
				localcontext[key] = vmpa.getObject(wrappedContext);
			}
			else
			{
				vmproxyhash[key] = vmpa;
			}
		}

		/// <summary>  Impl of the Context.put() method.
		/// *
		/// </summary>
		/// <param name="key">name of item to set
		/// </param>
		/// <param name="value">object to set to key
		/// </param>
		/// <returns>old stored object
		///
		/// </returns>
		public Object Put(String key, Object value_)
		{
			/*
	    *  first see if this is a vmpa
	    */

			VMProxyArg vmpa = (VMProxyArg) vmproxyhash[key];

			if (vmpa != null)
			{
				return vmpa.setObject(wrappedContext, value_);
			}
			else
			{
				if (localcontextscope)
				{
					/*
		    *  if we have localcontextscope mode, then just 
		    *  put in the local context
		    */

					return localcontext[key] = value_;
				}
				else
				{
					/*
		    *  ok, how about the local context?
		    */

					if (localcontext.ContainsKey(key))
					{
						return localcontext[key] = value_;
					}
					else
					{
						/*
						* otherwise, let them push it into the 'global' context
						*/

						return innerContext.Put(key, value_);
					}
				}
			}
		}

		/// <summary>  Impl of the Context.gut() method.
		/// *
		/// </summary>
		/// <param name="key">name of item to get
		/// </param>
		/// <returns> stored object or null
		///
		/// </returns>
		public Object Get(String key)
		{
			/*
	    * first, see if it's a VMPA
	    */

			Object o = null;

			VMProxyArg vmpa = (VMProxyArg) vmproxyhash[key];

			if (vmpa != null)
			{
				o = vmpa.getObject(wrappedContext);
			}
			else
			{
				if (localcontextscope)
				{
					/*
		    * if we have localcontextscope mode, then just 
		    * put in the local context
		    */

					o = localcontext[key];
				}
				else
				{
					/*
		    *  try the local context
		    */

					o = localcontext[key];

					if (o == null)
					{
						/*
						* last chance
						*/

						o = innerContext.Get(key);
					}
				}
			}

			return o;
		}

		/// <summary>  not yet impl
		/// </summary>
		public bool ContainsKey(Object key)
		{
			return false;
		}

		/// <summary>  impl badly
		/// </summary>
		/// <summary>  impl badly
		/// </summary>
		public Object Remove(Object key)
		{
			Object o = vmproxyhash[key];
			vmproxyhash.Remove(key);
			return o;
		}

		void IDictionary.Remove(Object key)
		{
			this.Remove(key);
		}

		public void PushCurrentTemplateName(String s)
		{
			innerContext.PushCurrentTemplateName(s);
		}

		public void PopCurrentTemplateName()
		{
			innerContext.PopCurrentTemplateName();
		}


		public IntrospectionCacheData ICacheGet(Object key)
		{
			return innerContext.ICacheGet(key);
		}

		public void ICachePut(Object key, IntrospectionCacheData o)
		{
			innerContext.ICachePut(key, o);
		}

		public EventCartridge AttachEventCartridge(EventCartridge ec)
		{
			return innerContext.AttachEventCartridge(ec);
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public bool Contains( object key )
		{
			throw new NotImplementedException();
		}

		public void Add( object key, object value )
		{
			throw new NotImplementedException();
		}

		public void Clear( )
		{
			throw new NotImplementedException();
		}

		public IDictionaryEnumerator GetEnumerator( )
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator( )
		{
			throw new NotImplementedException();
		}
	}
}

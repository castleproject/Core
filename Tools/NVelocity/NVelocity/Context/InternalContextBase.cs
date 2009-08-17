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
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using NVelocity.App.Events;
	using NVelocity.Runtime.Resource;
	using NVelocity.Util.Introspection;

	/// <summary>  class to encapsulate the 'stuff' for internal operation of velocity.
	/// We use the context as a thread-safe storage : we take advantage of the
	/// fact that it's a visitor  of sorts  to all nodes (that matter) of the
	/// AST during init() and render().
	/// Currently, it carries the template name for namespace
	/// support, as well as node-local context data introspection caching.
	/// *
	/// Note that this is not a public class.  It is for package access only to
	/// keep application code from accessing the internals, as AbstractContext
	/// is derived from this.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: InternalContextBase.cs,v 1.4 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	[Serializable]
	public class InternalContextBase : IInternalHousekeepingContext, IInternalEventContext
	{
		/// <summary>
		/// cache for node/context specific introspection information
		/// </summary>
		private readonly HybridDictionary introspectionCache;

		/// <summary>
		/// Template name stack. The stack top contains the current template name.
		/// </summary>
		private readonly Stack<string> templateNameStack;

		/// <summary>
		/// EventCartridge we are to carry.  Set by application
		/// </summary>
		private EventCartridge eventCartridge = null;

		/// <summary>
		/// Current resource - used for carrying encoding and other
		/// information down into the rendering process
		/// </summary>
		private Resource currentResource = null;

		public InternalContextBase()
		{
			introspectionCache = new HybridDictionary();
			templateNameStack = new Stack<string>();
		}

		public String CurrentTemplateName
		{
			get
			{
				if ((templateNameStack.Count == 0))
				{
					return "<undef>";
				}
				else
				{
					return templateNameStack.Peek();
				}
			}
		}

		public Object[] TemplateNameStack
		{
			get { return templateNameStack.ToArray(); }
		}

		public Resource CurrentResource
		{
			get { return currentResource; }
			set { currentResource = value; }
		}

		public EventCartridge EventCartridge
		{
			get { return eventCartridge; }
		}

		/// <summary>
		/// set the current template name on top of stack
		/// </summary>
		/// <param name="s">current template name</param>
		public void PushCurrentTemplateName(String s)
		{
			templateNameStack.Push(s);
		}

		/// <summary>remove the current template name from stack</summary>
		public void PopCurrentTemplateName()
		{
			templateNameStack.Pop();
		}

		public IntrospectionCacheData ICacheGet(Object key)
		{
			return (IntrospectionCacheData) introspectionCache[key];
		}

		public void ICachePut(Object key, IntrospectionCacheData o)
		{
			introspectionCache[key] = o;
		}

		public EventCartridge AttachEventCartridge(EventCartridge eventCartridge)
		{
			EventCartridge temp = this.eventCartridge;

			this.eventCartridge = eventCartridge;

			return temp;
		}
	}
}
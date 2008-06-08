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

using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime;
using NVelocity.Exception;
using NVelocity.Runtime.Parser;
using NVelocity.App.Events;
using NVelocity.Context;
using NVelocity.Util.Introspection;
using NVelocity;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.IO;
	using System.Text;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using Descriptors;
	using Providers;

	/// <summary>
	/// Pendent
	/// </summary>
	public abstract class AbstractComponentDirective : Directive, IViewRenderer
	{
		private readonly IViewComponentFactory viewComponentFactory;
		private IViewEngine viewEngine;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractComponentDirective"/> class.
		/// </summary>
		/// <param name="viewComponentFactory">The view component factory.</param>
		/// <param name="viewEngine">The view engine instance</param>
		public AbstractComponentDirective(IViewComponentFactory viewComponentFactory, IViewEngine viewEngine)
		{
			this.viewComponentFactory = viewComponentFactory;
			this.viewEngine = viewEngine;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			IEngineContext railsContext = MonoRailHttpHandlerFactory.CurrentEngineContext;
			IViewComponentRegistry registry = railsContext.Services.GetService<IViewComponentFactory>().Registry;
			IViewComponentDescriptorProvider viewDescProvider =
				railsContext.Services.GetService<IViewComponentDescriptorProvider>();
			ICacheProvider cacheProvider = railsContext.Services.CacheProvider;

			INode compNameNode = node.GetChild(0);

			if (compNameNode == null)
			{
				String message = String.Format("You must specify the component name on the #{0} directive", Name);
				throw new ViewComponentException(message);
			}

			string componentName = compNameNode.FirstToken.Image;

			if (componentName == null)
			{
				String message = String.Format("Could not obtain component name from the #{0} directive", Name);
				throw new ViewComponentException(message);
			}

			if (componentName.StartsWith("$"))
			{
				String nodeContent = compNameNode.Literal.Trim('"', '\'');
				SimpleNode inlineNode = runtimeServices.Parse(new StringReader(nodeContent), context.CurrentTemplateName, false);

				inlineNode.Init(context, runtimeServices);
				componentName = (string) Evaluate(inlineNode, context);
			}

			IDictionary componentParams = CreateParameters(context, node);

			Type viewComptype = registry.GetViewComponent(componentName);

			ViewComponentDescriptor descriptor = null;
			CacheKey key = null;

			if (viewComptype != null)
			{
				descriptor = viewDescProvider.Collect(viewComptype);
			}

			bool isOutputtingToCache = false;
			ViewComponentCacheBag bag = null;

			if (descriptor != null && descriptor.IsCacheable)
			{
				key = descriptor.CacheKeyGenerator.Create(componentName, componentParams, railsContext);

				if (key != null)
				{
					ViewComponentCacheBag cachedContent = (ViewComponentCacheBag) cacheProvider.Get(key.ToString());

					if (cachedContent != null)
					{
						// Restore entries

						foreach(KeyValuePair<string, object> pair in cachedContent.ContextEntries)
						{
							context[pair.Key] = pair.Value;
						}

						// Render from cache

						writer.Write(cachedContent.Content);

						return true;
					}

					isOutputtingToCache = true;
					bag = new ViewComponentCacheBag();
				}
			}

			ViewComponent component = viewComponentFactory.Create(componentName);

			if (component == null)
			{
				throw new MonoRailException("ViewComponentFactory returned a null ViewComponent for " + componentName + ". " +
				                            "Please investigate the implementation: " + viewComponentFactory.GetType().FullName);
			}

			ASTDirective directiveNode = (ASTDirective) node;
			IViewRenderer renderer = (IViewRenderer) directiveNode.Directive;

			NVelocityViewContextAdapter contextAdapter = new NVelocityViewContextAdapter(componentName, node, viewEngine, renderer);
			contextAdapter.Context = isOutputtingToCache ? new CacheAwareContext(context, bag) : context;

			ProcessSubSections(component, contextAdapter);

			INode bodyNode = null;

			if (node.ChildrenCount > 0)
			{
				bodyNode = node.GetChild(node.ChildrenCount - 1);
			}

			TextWriter output = isOutputtingToCache ? bag.CacheWriter : writer;

			contextAdapter.BodyNode = bodyNode;
			contextAdapter.ComponentParams = componentParams;
			contextAdapter.TextWriter = output;

			const string ViewComponentContextKey = "viewcomponent";
			object previousComp = context[ViewComponentContextKey];

			try
			{
				context[ViewComponentContextKey] = component;

				component.Init(railsContext, contextAdapter);

				component.Render();

				if (contextAdapter.ViewToRender != null)
				{
					RenderComponentView(context, contextAdapter.ViewToRender, output, contextAdapter);
				}

				if (isOutputtingToCache)
				{
					// Save output

					cacheProvider.Store(key.ToString(), bag);

					// Output to correct writer

					writer.Write(bag.Content);
				}
			}
			finally
			{
				if (previousComp != null)
				{
					context[ViewComponentContextKey] = previousComp;
				}
				else
				{
					context.Remove(ViewComponentContextKey);
				}
			}

			return true;
		}

		public bool RenderComponentView(IInternalContextAdapter context,
		                                String viewToRender, TextWriter writer,
		                                NVelocityViewContextAdapter contextAdapter)
		{
			if (!viewToRender.ToLower().EndsWith(NVelocityViewEngine.TemplateExtension))
			{
				viewToRender = viewToRender + NVelocityViewEngine.TemplateExtension;
			}

			CheckTemplateStack(context);

			String encoding = ExtractEncoding(context);

			Template template = GetTemplate(viewToRender, encoding);

			return RenderView(context, viewToRender, template, writer);
		}

		protected virtual void ProcessSubSections(ViewComponent component, NVelocityViewContextAdapter contextAdapter)
		{
		}

		protected virtual IDictionary CreateParameters(IInternalContextAdapter context, INode node)
		{
			int childrenCount = node.ChildrenCount;

			if (childrenCount > 1)
			{
				INode lastNode = node.GetChild(childrenCount - 1);

				if (lastNode.Type == ParserTreeConstants.BLOCK)
				{
					childrenCount--;
				}

				if (childrenCount > 1)
				{
					IDictionary dict = ProcessFirstParam(node, context, childrenCount);

					if (dict != null)
					{
						return dict;
					}
					else if (childrenCount > 2)
					{
						return ProcessRemainingParams(childrenCount, node, context);
					}
				}
			}

			return new Hashtable(0);
		}

//		protected string ComponentName
//		{
//			get { return componentName; }
//		}
//
//		protected ViewComponent Component
//		{
//			get { return component; }
//		}
//
//		protected NVelocityViewContextAdapter ContextAdapter
//		{
//			get { return contextAdapter; }
//		}

		private IDictionary ProcessRemainingParams(int childrenCount, INode node, IInternalContextAdapter context)
		{
			IDictionary entries = new HybridDictionary(true);

			for(int i = 2; i < childrenCount; i++)
			{
				INode paramNode = node.GetChild(i);

				string nodeContent = paramNode.Literal.TrimStart('"', '\'').TrimEnd('"', '\'');

				string[] parts = nodeContent.Split(new char[] {'='}, 2, StringSplitOptions.RemoveEmptyEntries);

				if (parts.Length == 2 && parts[1].IndexOf("$") != -1)
				{
					SimpleNode inlineNode = runtimeServices.Parse(new StringReader(parts[1]), context.CurrentTemplateName, false);

					inlineNode.Init(context, runtimeServices);

					entries[parts[0]] = Evaluate(inlineNode, context);
				}
				else if (parts.Length == 2)
				{
					entries[parts[0]] = parts[1];
				}
				else
				{
					entries[parts[0]] = String.Empty;
				}
			}

			return entries;
		}

		private object Evaluate(SimpleNode inlineNode, IInternalContextAdapter context)
		{
			ArrayList values = new ArrayList();

			for(int i = 0; i < inlineNode.ChildrenCount; i++)
			{
				INode node = inlineNode.GetChild(i);

				if (node.Type == ParserTreeConstants.TEXT)
				{
					values.Add(((ASTText) node).Text);
				}
				else
				{
					values.Add(node.Value(context));
				}
			}

			if (values.Count == 0)
			{
				return null;
			}
			else if (values.Count == 1)
			{
				return values[0];
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				foreach(object value in values)
				{
					sb.Append(value);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Processes the first param.
		/// first param can either be the literal string 'with' which means the user
		/// is using the syntax #blockcomponent(ComponentName with "param1=value1" "param2=value2")
		/// or it could be a dictionary string like: 
		/// #blockcomponent(ComponentName "#{ param1='value1', param2='value2' }")
		/// anything different than that will throw an exception
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="context">The context.</param>
		/// <param name="childrenCount">The children count.</param>
		/// <returns></returns>
		private IDictionary ProcessFirstParam(INode node, IInternalContextAdapter context, int childrenCount)
		{
			INode compNameNode = node.GetChild(0);
			string componentName = compNameNode.FirstToken.Image;

			INode withNode = node.GetChild(1);

			String withName = withNode.FirstToken.Image;

			if (!"with".Equals(withName))
			{
				IDictionary dict = withNode.Value(context) as IDictionary;

				if (dict != null)
				{
					if (childrenCount > 2)
					{
						String message = String.Format("A #{0} directive with a dictionary " +
						                               "string parameter cannot have extra params - component {0}", componentName);
						throw new ViewComponentException(message);
					}
					return dict;
				}
				else
				{
					String message = String.Format("A #{0} directive with parameters must use " +
					                               "the keyword 'with' - component {0}", componentName);
					throw new ViewComponentException(message);
				}
			}

			return null;
		}

		private bool RenderView(IInternalContextAdapter context,
		                        String viewToRender, Template template, TextWriter writer)
		{
			try
			{
				context.PushCurrentTemplateName(viewToRender);
				((SimpleNode) template.Data).Render(context, writer);
			}
			finally
			{
				context.PopCurrentTemplateName();
			}

			return true;
		}

		private Template GetTemplate(String viewToRender, String encoding)
		{
			return runtimeServices.GetTemplate(viewToRender, encoding);
		}

		private String ExtractEncoding(IInternalContextAdapter context)
		{
			Resource current = context.CurrentResource;

			String encoding;

			if (current != null)
			{
				encoding = current.Encoding;
			}
			else
			{
				encoding = (String) runtimeServices.GetProperty(RuntimeConstants.INPUT_ENCODING);
			}

			return encoding;
		}

		private void CheckTemplateStack(IInternalContextAdapter context)
		{
			Object[] templateStack = context.TemplateNameStack;

			if (templateStack.Length >= runtimeServices.GetInt(RuntimeConstants.PARSE_DIRECTIVE_MAXDEPTH, 20))
			{
				StringBuilder path = new StringBuilder();

				for(int i = 0; i < templateStack.Length; ++i)
				{
					path.Append(" > " + templateStack[i]);
				}

				throw new Exception("Max recursion depth reached (" + templateStack.Length + ")" + " File stack:" + path);
			}
		}
	}

	public class CacheAwareContext : IInternalContextAdapter
	{
		private readonly IInternalContextAdapter context;
		private readonly ViewComponentCacheBag bag;

		public CacheAwareContext(IInternalContextAdapter context, ViewComponentCacheBag bag)
		{
			this.context = context;
			this.bag = bag;
		}

		object IInternalContextAdapter.Remove(object key)
		{
			if (bag.ContextEntries.ContainsKey(key.ToString()))
			{
				bag.ContextEntries.Remove(key.ToString());
			}
			return context.Remove(key);
		}

		public void PushCurrentTemplateName(string s)
		{
			context.PushCurrentTemplateName(s);
		}

		public void PopCurrentTemplateName()
		{
			context.PopCurrentTemplateName();
		}

		public IntrospectionCacheData ICacheGet(object key)
		{
			return context.ICacheGet(key);
		}

		public void ICachePut(object key, IntrospectionCacheData o)
		{
			context.ICachePut(key, o);
		}

		public string CurrentTemplateName
		{
			get { return context.CurrentTemplateName; }
		}

		public object[] TemplateNameStack
		{
			get { return context.TemplateNameStack; }
		}

		public Resource CurrentResource
		{
			get { return context.CurrentResource; }
			set { context.CurrentResource = value; }
		}

		public object Put(string key, object value)
		{
			bag.ContextEntries[key] = value;

			return context.Put(key, value);
		}

		public object Get(string key)
		{
			return context.Get(key);
		}

		public bool ContainsKey(object key)
		{
			return context.ContainsKey(key);
		}

		object IContext.Remove(object key)
		{
			if (bag.ContextEntries.ContainsKey(key.ToString()))
			{
				bag.ContextEntries.Remove(key.ToString());
			}

			return context.Remove(key);
		}

		public IContext InternalUserContext
		{
			get { return context.InternalUserContext; }
		}

		public IInternalContextAdapter BaseContext
		{
			get { return context.BaseContext; }
		}

		public EventCartridge AttachEventCartridge(EventCartridge ec)
		{
			return context.AttachEventCartridge(ec);
		}

		public EventCartridge EventCartridge
		{
			get { return context.EventCartridge; }
		}

		public bool Contains(object key)
		{
			return context.Contains(key);
		}

		public void Add(object key, object value)
		{
			bag.ContextEntries[key.ToString()] = value;
			context.Add(key, value);
		}

		public void Clear()
		{
			context.Clear();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return context.GetEnumerator();
		}

		public void Remove(object key)
		{
			if (bag.ContextEntries.ContainsKey(key.ToString()))
			{
				bag.ContextEntries.Remove(key.ToString());
			}

			((IDictionary) context).Remove(key);
		}

		public object this[object key]
		{
			get { return context[key]; }
			set
			{
				bag.ContextEntries[key.ToString()] = value;
				context[key] = value;
			}
		}

		public ICollection Keys
		{
			get { return ((IDictionary) context).Keys; }
		}

		object[] IContext.Keys
		{
			get { return ((IContext) context).Keys; }
		}

		public ICollection Values
		{
			get { return context.Values; }
		}

		public bool IsReadOnly
		{
			get { return context.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return context.IsFixedSize; }
		}

		public void CopyTo(Array array, int index)
		{
			context.CopyTo(array, index);
		}

		public int Count
		{
			get { return ((ICollection) context).Count; }
		}

		public object SyncRoot
		{
			get { return context.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return context.IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable) context).GetEnumerator();
		}
	}
}

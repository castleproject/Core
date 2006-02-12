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

using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Resource;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime;
using NVelocity.Exception;
using NVelocity.Runtime.Parser;
using IInternalContextAdapter = NVelocity.Context.IInternalContextAdapter;
using Template = NVelocity.Template;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;

	public abstract class AbstractComponentDirective : Directive
	{
		private readonly IViewComponentFactory viewComponentFactory;

		private String componentName;
		private ViewComponent component;

		public AbstractComponentDirective(IViewComponentFactory viewComponentFactory)
		{
			this.viewComponentFactory = viewComponentFactory;
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			INode compNameNode = node.GetChild(0);

			if (compNameNode == null)
			{
				String message = String.Format("You must specify the component name on the #{0} directive", Name);
				throw new RailsException(message);
			}

			componentName = compNameNode.FirstToken.Image;

			if (componentName == null)
			{
				String message = String.Format("Could not obtain component name from the #{0} directive", Name);
				throw new RailsException(message);
			}

			component = viewComponentFactory.Create(componentName);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			INode bodyNode = null;

			IDictionary componentParams = CreateParameters(context, node);

			if (node.ChildrenCount > 0)
			{
				bodyNode = node.GetChild(node.ChildrenCount - 1);
			}

			NVelocityViewContextAdapter contextAdapter =
				new NVelocityViewContextAdapter(
					componentName, context, writer, bodyNode, componentParams);

			IRailsEngineContext railsContext = MonoRailHttpHandler.CurrentContext;

			component.Init(railsContext, contextAdapter);

			component.Render();

			if (contextAdapter.ViewToRender != null)
			{
				return RenderComponentView(context, writer, contextAdapter);
			}

			return true;
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

		private IDictionary ProcessRemainingParams(int childrenCount, INode node, IInternalContextAdapter context)
		{
			ArrayList list = new ArrayList();

			for(int i = 2; i < childrenCount; i++)
			{
				INode paramNode = node.GetChild(i);

				Object value = paramNode.Value(context);

				String stValue = (value as String);

				if (stValue == null)
				{
					String message = String.Format("Could not evaluate parameter {0} to a String for component {1}", i, componentName);
					throw new RailsException(message);
				}

				list.Add(stValue);
			}

			return (new DictHelper()).CreateDict((String[]) list.ToArray(typeof(String)));
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
			INode withNode = node.GetChild(1);

			String withName = withNode.FirstToken.Image;

			if (!"with".Equals(withName))
			{
				IDictionary dict = withNode.Value(context) as IDictionary;

				if (dict != null)
				{
					if (childrenCount > 2)
					{
						String message = String.Format("A #{0} directive with a dictionary string param cannot have extra params - component {0}", componentName, Name);
						throw new RailsException(message);
					}
					return dict;
				}
				else
				{
					String message = String.Format("A #{0} directive with parameters must use the keyword 'with' - component {0}", componentName, Name);
					throw new RailsException(message);
				}
			}

			return null;
		}

		private bool RenderComponentView(IInternalContextAdapter context,
		                                 TextWriter writer, NVelocityViewContextAdapter contextAdapter)
		{
			foreach(DictionaryEntry entry in contextAdapter.ContextVars)
			{
				context.Put(entry.Key.ToString(), entry.Value);
			}

			try
			{
				String viewToRender = contextAdapter.ViewToRender;

				viewToRender = viewToRender + NVelocityViewEngine.TemplateExtension;

				CheckTemplateStack(context);

				String encoding = SetUpEncoding(context);

				Template template = GetTemplate(viewToRender, encoding);

				return RenderView(context, viewToRender, template, writer);
			}
			finally
			{
				// WTF!!
				// this might make sense when contextAdapter was a copy of the context,
				// but not now.
				//foreach(DictionaryEntry entry in contextAdapter.ContextVars)
				//{
				//	context.Remove( entry.Key );
				//}
			}
		}

		private bool RenderView(IInternalContextAdapter context,
		                        String viewToRender, Template template, TextWriter writer)
		{
			try
			{
				context.PushCurrentTemplateName(viewToRender);
				((SimpleNode) template.Data).Render(context, writer);
			}
			catch(Exception e)
			{
				if (e is MethodInvocationException)
				{
					throw;
				}

				return false;
			}
			finally
			{
				context.PopCurrentTemplateName();
			}

			return true;
		}

		private Template GetTemplate(String viewToRender, String encoding)
		{
			try
			{
				return rsvc.GetTemplate(viewToRender, encoding);
			}
			catch(Exception)
			{
				throw;
			}
		}

		private String SetUpEncoding(IInternalContextAdapter context)
		{
			Resource current = context.CurrentResource;

			String encoding = null;

			if (current != null)
			{
				encoding = current.Encoding;
			}
			else
			{
				encoding = (String) rsvc.GetProperty(RuntimeConstants.INPUT_ENCODING);
			}
			return encoding;
		}

		private void CheckTemplateStack(IInternalContextAdapter context)
		{
			Object[] templateStack = context.TemplateNameStack;

			if (templateStack.Length >= rsvc.GetInt(RuntimeConstants.PARSE_DIRECTIVE_MAXDEPTH, 20))
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
}
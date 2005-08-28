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

using INode = NVelocity.Runtime.Parser.Node.INode;
using Template = NVelocity.Template;
using Resource = NVelocity.Runtime.Resource.Resource;
using Directive = NVelocity.Runtime.Directive.Directive;
using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
using RuntimeConstants_Fields = NVelocity.Runtime.RuntimeConstants_Fields;
using RuntimeServices = NVelocity.Runtime.RuntimeServices;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;
using MethodInvocationException = NVelocity.Exception.MethodInvocationException;
using ParserTreeConstants = NVelocity.Runtime.Parser.ParserTreeConstants;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;
	
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Internal;


	public abstract class AbstractComponentDirective : Directive
	{
		private readonly IViewComponentFactory viewComponentFactory;

		private String componentName;
		private ViewComponent component;
		private int bodyNodeIndex = 1;

		public AbstractComponentDirective( IViewComponentFactory viewComponentFactory )
		{
			this.viewComponentFactory = viewComponentFactory;
		}

		public override void init(RuntimeServices rs, InternalContextAdapter context, INode node)
		{
			base.init(rs, context, node);

			INode compNameNode = node.jjtGetChild(0);

			if (compNameNode == null)
			{
				String message = String.Format("You must specify the component name on the #component directive");
				throw new RailsException(message);
			}

			componentName = compNameNode.FirstToken.image;

			if (componentName == null)
			{
				String message = String.Format("Could not obtain component name from the #component directive");
				throw new RailsException(message);
			}

			component = viewComponentFactory.Create( componentName );
		}

		public override bool render(InternalContextAdapter context, TextWriter writer, INode node)
		{
			INode bodyNode = null;

			if (bodyNodeIndex < node.jjtGetNumChildren())
			{
				bodyNode = node.jjtGetChild(bodyNodeIndex);
			}

			NVelocityViewContextAdapter contextAdapter = 
				new NVelocityViewContextAdapter( 
					componentName, context, writer, bodyNode, CreateParameters(context, node) );

			IRailsEngineContext railsContext = (IRailsEngineContext) context.Get("context");

			component.Init( railsContext, contextAdapter );

			component.Render();

			if (contextAdapter.ViewToRender != null)
			{
				return RenderComponentView( context, writer, contextAdapter );
			}

			return true;
		}

		private IDictionary CreateParameters(InternalContextAdapter context, INode node)
		{
			int childrenCount = node.jjtGetNumChildren();

			if (childrenCount > 1)
			{
				bodyNodeIndex = childrenCount - 1;

				INode lastNode = node.jjtGetChild(childrenCount - 1);

				if (lastNode.Type == ParserTreeConstants.JJTBLOCK)
				{
					childrenCount--;
				}

				ArrayList list = new ArrayList();

				for (int i=1; i < childrenCount; i++)
				{
					if (i == 1)
					{
						INode withNode = node.jjtGetChild(1);

						String withName = withNode.FirstToken.image;

						if (!"with".Equals(withName))
						{
							String message = String.Format("A #component directive with parameters must use the keyword 'with' - component {0}", componentName);
							throw new RailsException(message);
						}

						continue;
					}

					INode paramNode = node.jjtGetChild( i );

					Object value = paramNode.Value(context);

					String stValue = (value as String);
					
					if (stValue == null)
					{
						String message = String.Format("Could not evaluate parameter {0} to a String for component {1}", i, componentName);
						throw new RailsException(message);
					}

					list.Add( stValue );
				}

				return (new DictHelper()).CreateDict( (String[]) list.ToArray( typeof(String) ) );
			}

			return new Hashtable(0);
		}

		private bool RenderComponentView(InternalContextAdapter context, 
			TextWriter writer, NVelocityViewContextAdapter contextAdapter)
		{
			String viewToRender = contextAdapter.ViewToRender;

			viewToRender = String.Format("{0}.vm", viewToRender);

			CheckTemplateStack(context);

			String encoding = SetUpEncoding(context);

			Template t = GetTemplate(viewToRender, encoding);

			return RenderView(context, viewToRender, t, writer);
		}

		private bool RenderView(InternalContextAdapter context, 
			String viewToRender, Template template, TextWriter writer)
		{
			try
			{
				context.PushCurrentTemplateName(viewToRender);
				((SimpleNode) template.Data).render(context, writer);
			}
			catch (Exception e)
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
				return rsvc.getTemplate(viewToRender, encoding);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private String SetUpEncoding(InternalContextAdapter context)
		{
			Resource current = context.CurrentResource;
	
			String encoding = null;
	
			if (current != null)
			{
				encoding = current.Encoding;
			}
			else
			{
				encoding = (String) rsvc.getProperty(RuntimeConstants_Fields.INPUT_ENCODING);
			}
			return encoding;
		}

		private void CheckTemplateStack(InternalContextAdapter context)
		{
			Object[] templateStack = context.TemplateNameStack;
	
			if (templateStack.Length >= rsvc.getInt(RuntimeConstants_Fields.PARSE_DIRECTIVE_MAXDEPTH, 20))
			{
				StringBuilder path = new StringBuilder();

				for (int i = 0; i < templateStack.Length; ++i)
				{
					path.Append(" > " + templateStack[i]);
				}

				throw new Exception("Max recursion depth reached (" + templateStack.Length + ")" + " File stack:" + path);
			}
		}
	}
}
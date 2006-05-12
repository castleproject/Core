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

namespace Castle.MonoRail.Framework.Views.StringTemplateView
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Reflection;
	using StringBuilder					= System.Text.StringBuilder;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using StringTemplate				= Antlr.StringTemplate.StringTemplate;
	using StringTemplateGroup			= Antlr.StringTemplate.StringTemplateGroup;
	using StringTemplateGroupInterface	= Antlr.StringTemplate.StringTemplateGroupInterface;
	using StringTemplateLoader			= Antlr.StringTemplate.StringTemplateLoader;
	using StringTemplateException		= Antlr.StringTemplate.StringTemplateException;
	using TemplateLoadException			= Antlr.StringTemplate.TemplateLoadException;
	using HashList						= Antlr.StringTemplate.Collections.HashList;
	using ConfigConstants				= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.ConfigConstants;

	public class ViewComponentStringTemplateGroup : StringTemplateGroup
	{
		protected IViewComponentFactory viewComponentFactory;

		#region Constructors

		public ViewComponentStringTemplateGroup(string name, Type lexerType)
			: base(name, (StringTemplateLoader)null, lexerType)
		{
		}
		
		#endregion

		public IViewComponentFactory ViewComponentFactory
		{
			get 
			{
				if (viewComponentFactory == null)
				{
					IRailsEngineContext ctx = MonoRailHttpHandler.CurrentContext;
					viewComponentFactory = (IViewComponentFactory) ctx.GetService(typeof(IViewComponentFactory));
					if (viewComponentFactory == null)
					{
						throw new RailsException("StringTemplateViewEngine: Could not obtain ViewComponentFactory instance");
					}
				}
				return viewComponentFactory; 
			}
		}

		public override StringTemplate CreateStringTemplate()
		{
			return new ViewComponentStringTemplate();
		}
		
		public override StringTemplate LookupTemplate(StringTemplate enclosingInstance, string name)
		{
			if (name.StartsWith("super."))
			{
				if (superGroup != null)
				{
					int dot = name.IndexOf('.');
					name = name.Substring(dot + 1, (name.Length) - (dot + 1));
					StringTemplate superScopeST =
						superGroup.LookupTemplate(enclosingInstance,name);
					return superScopeST;
				}
				throw new StringTemplateException(Name + " has no super group; invalid template: " + name);
			}

			StringTemplate st = (StringTemplate) templates[name];
			bool foundCachedWrappingTemplate = (st is ViewComponentStringTemplate);
			if ((st == null) || foundCachedWrappingTemplate)
			{
				if (enclosingInstance != null)
				{
					string prefix;
					int index = name.IndexOf('/');
					if (index == -1)
						prefix = name;
					else
						prefix = name.Substring(0, index);

					if ("blockcomponent".Equals(prefix))
					{
						if (st == null)
							st = new ViewComponentStringTemplate(name, this);
						((ViewComponentStringTemplate)st).ViewComponentName = name.Substring(index+1);
					}
					else if ("component".Equals(prefix))
					{
						if (st == null)
							st = new ViewComponentStringTemplate(name, this);
						((ViewComponentStringTemplate)st).ViewComponentName = name.Substring(index+1);
					}
					else if ("capturefor".Equals(prefix))
					{
						if (st == null)
							st = new ViewComponentStringTemplate(name, this);
						((ViewComponentStringTemplate)st).ViewComponentName = "CaptureFor";
					}

					if ((!foundCachedWrappingTemplate) && (st != null))
					{
							templates[name] = st;
					}
				}

				if (st == null && superGroup != null)
				{
					st = superGroup.GetInstanceOf(enclosingInstance, name);
					if (st != null)
					{
						st.Group = this;
					}
				}
				if (st == null)
				{
					templates[name] = NOT_FOUND_ST;
				}
			}
			else if (st == NOT_FOUND_ST)
			{
				return null;
			}
			return st;
		}

		public override StringTemplate GetInstanceOf(StringTemplate enclosingInstance, string name)
		{
			StringTemplate st = LookupTemplate(enclosingInstance, name);
			if (st != null)
			{
				StringTemplate instanceST = st.GetInstanceOf();
				if (st is ViewComponentStringTemplate)
				{
					((ViewComponentStringTemplate)st).ViewComponentName = null;
					((ViewComponentStringTemplate)instanceST).CreateViewComponent();
				}
				return instanceST;
			}
			return null;
		}
	}
}

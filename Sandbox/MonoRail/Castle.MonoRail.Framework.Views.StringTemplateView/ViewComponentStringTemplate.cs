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
	using MethodInfo			= System.Reflection.MethodInfo;
	using ParameterInfo			= System.Reflection.ParameterInfo;
	using StringBuilder			= System.Text.StringBuilder;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using StringTemplateGroup	= Antlr.StringTemplate.StringTemplateGroup;
	using StringTemplate		= Antlr.StringTemplate.StringTemplate;
	using IStringTemplateWriter	= Antlr.StringTemplate.IStringTemplateWriter;
	using FormalArgument		= Antlr.StringTemplate.Language.FormalArgument;
	using ConfigConstants		= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.ConfigConstants;

	public class ViewComponentStringTemplate : StringTemplate
	{
		protected IViewComponentContext viewComponentContext;
		protected ViewComponent viewComponent;
		protected string viewComponentName;

		protected internal ViewComponentStringTemplate() : base()
		{
		}

		public ViewComponentStringTemplate(string name, ViewComponentStringTemplateGroup group)
		{
			this.name = name;
			if (group != null)
			{
				this.group = group;
				this.nativeGroup = group;
			}
		}

		internal string ViewComponentName
		{
			get { return viewComponentName;  }
			set { viewComponentName = value; }
		}

		internal void CreateViewComponent()
		{
			if ((viewComponentName == null) || (viewComponentName.Length < 1))
			{
				throw new RailsException("You must specify the component name with 'blockcomponent' and 'component'.");
			}
			viewComponent = ((ViewComponentStringTemplateGroup)group).ViewComponentFactory.Create(viewComponentName);
			viewComponentContext = new StringTemplateViewContextAdapter(viewComponentName, this);
		}

		override public int Write(IStringTemplateWriter output)
		{
			SetPredefinedAttributes();
			SetDefaultArgumentValues();

			//IRailsEngineContext context = (IRailsEngineContext) GetAttribute(ConfigConstants.CONTEXT_ATTRIB_KEY);
			IRailsEngineContext context = MonoRailHttpHandler.CurrentContext;

			StringWriter writer = new StringWriter();
			StringTemplateViewContextAdapter viewComponentContext = new StringTemplateViewContextAdapter(viewComponentName, this);
			viewComponentContext.TextWriter = writer;


			viewComponent.Init(context, viewComponentContext);
			viewComponent.Render();
			if (viewComponentContext.ViewToRender != null)
			{
				StringTemplate viewST = group.GetEmbeddedInstanceOf(this, viewComponentContext.ViewToRender);
				writer.Write(viewST.ToString());
			}

			if (viewComponentName.Equals("CaptureFor"))
			{
				string keyToSet = (string) GetAttribute("id");
				object valToSet = GetAttribute(keyToSet);
				OutermostEnclosingInstance.RemoveAttribute(keyToSet);
				OutermostEnclosingInstance.SetAttribute(keyToSet, valToSet);
			}

			output.Write(writer.ToString());
			//			if (LintMode)
			//			{
			//				CheckForTrouble();
			//			}
			return 0;
		}
		
		/// <summary>
		/// Make the 'to' template look exactly like the 'from' template
		/// except for the attributes.
		/// </summary>
		override protected void  dup(StringTemplate from, StringTemplate to)
		{
			base.dup(from, to);
			ViewComponentStringTemplate fromST = (ViewComponentStringTemplate) from;
			ViewComponentStringTemplate toST   = (ViewComponentStringTemplate) to;
			toST.viewComponent = fromST.viewComponent;
			toST.viewComponentName = fromST.viewComponentName;
			toST.viewComponentContext = fromST.viewComponentContext;
		}
	}
}

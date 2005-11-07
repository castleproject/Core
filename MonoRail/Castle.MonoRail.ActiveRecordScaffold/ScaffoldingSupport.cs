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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;
	using System.Runtime.CompilerServices;

	using Castle.MonoRail.Framework;

	using Castle.Components.Common.TemplateEngine;
	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;

	/// <summary>
	/// Uses the dynamic action infrastructure to 
	/// add new actions to an existing controller.
	/// </summary>
	/// <remarks>
	/// Provided that a controller uses <see cref="ScaffoldingAttribute"/>
	/// like the following code:
	/// <code>
	/// [Scaffolding( typeof(Account) )]
	/// public class AdminController : Controller
	/// {
	/// }
	/// </code>
	/// Then the following dynamic actions will be added:
	/// <list type="bullet">
	/// <item><term>newAccount</term>
	/// <description>Presents a form to the user fill in order to create the item on the database</description>
	/// </item>
	/// <item><term>createAccount</term>
	/// <description>Take the information submited by the newAccount and creates the item</description>
	/// </item>
	/// <item><term>editAccount</term>
	/// <description>Presents a form to the user fill in order to update the item on the database</description>
	/// </item>
	/// <item><term>updateAccount</term>
	/// <description>Take the information submited by the editAccount and changes the item</description>
	/// </item>
	/// <item><term>listAccount</term>
	/// <description>Presents a paginated list of items saved</description>
	/// </item>
	/// <item><term>confirmAccount</term>
	/// <description>Ask the user if he/she confirms the removal of the item</description>
	/// </item>
	/// <item><term>removeAccount</term>
	/// <description>Attempt to remove the item and presents the results</description>
	/// </item>
	/// </list>
	/// </remarks>
	public class ScaffoldingSupport : IScaffoldingSupport
	{
		private static ITemplateEngine templateEngine = null;

		public void Process(Controller controller)
		{
			InitializeTemplateEngine();

			foreach(ScaffoldingAttribute scaffoldAtt in controller.MetaDescriptor.Scaffoldings)
			{
				String name = scaffoldAtt.Model.Name;

				controller.DynamicActions[ String.Format("new{0}", name) ] = new NewAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("create{0}", name) ] = new CreateAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("edit{0}", name) ] = new EditAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("update{0}", name) ] = new UpdateAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("remove{0}", name) ] = new RemoveAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("confirm{0}", name) ] = new ConfirmRemoveAction( scaffoldAtt.Model, templateEngine ); 
				controller.DynamicActions[ String.Format("list{0}", name) ] = new ListAction( scaffoldAtt.Model, templateEngine ); 
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void InitializeTemplateEngine()
		{
			if (templateEngine == null)
			{
				NVelocityTemplateEngine nvelTemplateEng = new NVelocityTemplateEngine();
#if DEBUG
				nvelTemplateEng.TemplateDir = @"E:\dev\castle\MonoRail\Castle.MonoRail.ActiveRecordScaffold\Templates\";
				nvelTemplateEng.BeginInit();
				nvelTemplateEng.EndInit();
#else
				nvelTemplateEng.AssemblyName = "Castle.MonoRail.ActiveRecordScaffold";
				nvelTemplateEng.BeginInit();
				nvelTemplateEng.EndInit();
#endif

				templateEngine = nvelTemplateEng;
			}
		}
	}
}
// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using NVelocity.Runtime.Directive;

namespace Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives
{
	using System;
	using System.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public class BlockComponentDirective : AbstractComponentDirective
	{
		private readonly IList sectionsCreated = new ArrayList();

		public BlockComponentDirective(IViewComponentFactory viewComponentFactory, IViewEngine viewEngine) : base(viewComponentFactory, viewEngine)
		{
		}

		protected override void ProcessSubSections()
		{
			foreach(SubSectionDirective section in sectionsCreated)
			{
				if (!Component.SupportsSection(section.Name))
				{
					throw new ViewComponentException(
						String.Format("The section '{0}' is not supported by the ViewComponent '{1}'",
							section.Name, ComponentName));
				}

				ContextAdapter.RegisterSection(section);

				section.ContextAdapter = ContextAdapter;
			}
		}

		public override String Name
		{
			get { return "blockcomponent"; }
			set { }
		}

		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}

		public override bool SupportsNestedDirective(String name)
		{
			return true;
		}

		public override Directive CreateNestedDirective(String name)
		{
			SubSectionDirective section = new SubSectionDirective(name);

			sectionsCreated.Add(section);

			return section;
		}
	}
}

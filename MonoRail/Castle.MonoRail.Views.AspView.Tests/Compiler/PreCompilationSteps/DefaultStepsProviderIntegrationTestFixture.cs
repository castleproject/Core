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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.PreCompilationSteps
{
	using AspView.Compiler.PreCompilationSteps;
	using AspView.Compiler;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultStepsProviderIntegrationTestFixture 
	{
		IPreCompilationStepsProvider provider;

		public DefaultStepsProviderIntegrationTestFixture()
		{
			SetUp();
		}

		[SetUp]
		public void SetUp()
		{
			provider = new DefaultPreCompilationStepsProvider();
			Resolve.Initialize(null);
		}

		private void RunSteps(SourceFile file)
		{
			foreach (IPreCompilationStep step in provider.GetSteps())
			{
				step.Process(file);
			}
		}

		[Test]
		public void Proces_WithSiteRootInComponentBody_TransformsTheSiteRoot()
		{
			string source = @"<%@ Page Language=""C#"" %>
<component:Bold>~</component:Bold>";

			SourceFile file = new SourceFile();
			file.ViewName= @"\home\index.aspx";
			file.ViewSource = source;
			file.RenderBody = file.ViewSource;
			
			RunSteps(file);

			#region expected
			string expected = @"using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class  : AspViewBase
	{
		protected override string ViewName { get { return ""\\home\\index.aspx""; } }
		protected override string ViewDirectory { get { return ""\\home""; } }


		public override void Render()
		{
InvokeViewComponent(""Bold"", Bold0_body, null);

		}

		internal void Bold0_body ()
		{
			Output(siteRoot);

		}

	}
}
";
			#endregion

			Assert.AreEqual(expected, file.ConcreteClass);
		}
	}
}

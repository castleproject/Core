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
	using AspView.Compiler;
	using NUnit.Framework;
	using AspView.Compiler.PreCompilationSteps;

	[TestFixture]
	public class RenderConcreteClassStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new RenderConcreteClassStep();
		}

		[Test]
		public void Process_Always_Works()
		{
			file.Imports.Add("No.Such.Import");
			file.Imports.Add("Foo.Bar");
			
			file.ClassName = "view_class";
			file.BaseClassName = "BaseClass";

			file.Properties.Add("name", new ViewProperty("name", "string", null));
			file.Properties.Add("age", new ViewProperty("age", "int", null));
			file.Properties.Add("size", new ViewProperty("size", "int?", "null"));
			file.Properties.Add("user", new ViewProperty("user", "User", "new User()"));

			file.RenderBody = "RenderBody";

			file.ViewName = "view\\class";

			file.ViewComponentSectionHandlers.Add("vc1_body", "vc1_body body");
			file.ViewComponentSectionHandlers.Add("vc1_section1", "vc1_section1 body");
			file.ViewComponentSectionHandlers.Add("vc1_section2", "vc1_section2 body");

			file.EmbededScriptBlocks.Add(@"
private string Text(string name)
{
	return ""<input type='text' name='"" + name + ""' />"";
}
");

			#region expected
			expected =
@"using No.Such.Import;
using Foo.Bar;
namespace CompiledViews
{
	public class view_class : BaseClass
	{
		protected override string ViewName { get { return ""view\\class""; } }
		protected override string ViewDirectory { get { return ""view""; } }

		private string name { get { return (string)GetParameter(""name""); } }
		private int age { get { return (int)GetParameter(""age""); } }
		private int? size { get { return (int?)GetParameter(""size"", null); } }
		private User user { get { return (User)GetParameter(""user"", new User()); } }

		public override void Render()
		{
RenderBody
		}

		internal void vc1_body ()
		{
			vc1_body body
		}

		internal void vc1_section1 ()
		{
			vc1_section1 body
		}

		internal void vc1_section2 ()
		{
			vc1_section2 body
		}

		
		private string Text(string name)
		{
			return ""<input type='text' name='"" + name + ""' />"";
		}

	}
}
";
			#endregion

			step.Process(file);
			Assert.AreEqual(expected, file.ConcreteClass);
		}

	}
}
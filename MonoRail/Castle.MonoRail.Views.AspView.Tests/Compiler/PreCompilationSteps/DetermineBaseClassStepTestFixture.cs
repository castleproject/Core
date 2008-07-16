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
	using NUnit.Framework;

	[TestFixture]
	public class DetermineBaseClassStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new DetermineBaseClassStep();
		}

		private static void AssertPageDirectiveHasBeenRemoved(string viewSource)
		{
			if (Internal.RegularExpressions.PageDirective.IsMatch(viewSource))
				Assert.Fail("Page directive was not removed from view source");
		}

		[Test]
		public void Process_WhenInheritsIsMissing_SetsDefault()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" %>
view content";
			step.Process(file);

			Assert.AreEqual(DetermineBaseClassStep.DefaultBaseClassName, file.BaseClassName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenUsingDefault_SetsDefault()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
view content";
			step.Process(file);

			Assert.AreEqual(DetermineBaseClassStep.DefaultBaseClassName, file.BaseClassName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenUsingDefaultAndTypedView_SetsDefaultAndView()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime<IView>"" %>
view content";
			step.Process(file);

			Assert.AreEqual(DetermineBaseClassStep.DefaultBaseClassName + "<IView>", file.BaseClassName);
			Assert.AreEqual("IView", file.TypedViewName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

    [Test]
    public void Process_WhenUsingDefaultAndGenericTypedView_SetsDefaultAndGenericView()
    {
      file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime<IView<Item>>"" %>
view content";
      step.Process(file);

      Assert.AreEqual(DetermineBaseClassStep.DefaultBaseClassName + "<IView<Item>>", file.BaseClassName);
      Assert.AreEqual("IView<Item>", file.TypedViewName);

      AssertPageDirectiveHasBeenRemoved(file.RenderBody);
    }


		[Test]
		public void Process_WhenUsingClassName_SetsClassName()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClass"" %>
view content";
			step.Process(file);

			Assert.AreEqual("SomeClass", file.BaseClassName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenUsingClassNameAndTypedView_SetsClassNameAndView()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClass<IView>"" %>
view content";
			step.Process(file);

			Assert.AreEqual("SomeClass<IView>", file.BaseClassName);
			Assert.AreEqual("IView", file.TypedViewName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

    [Test]
    public void Process_WhenUsingClassNameAndGenericTypedView_SetsClassNameAndGenericView()
    {
      file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClass<IView<Item>>"" %>
view content";
      step.Process(file);

      Assert.AreEqual("SomeClass<IView<Item>>", file.BaseClassName);
      Assert.AreEqual("IView<Item>", file.TypedViewName);

      AssertPageDirectiveHasBeenRemoved(file.RenderBody);
    }


		[Test]
		public void Process_WhenUsingClassNameAtDesignTime_SetsClassName()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClassAtDesignTime"" %>
view content";
			step.Process(file);

			Assert.AreEqual("SomeClass", file.BaseClassName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenUsingClassNameAtDesignTimeAndTypedView_SetsClassNameAndView()
		{
			file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClassAtDesignTime<IView>"" %>
view content";
			step.Process(file);

			Assert.AreEqual("SomeClass<IView>", file.BaseClassName);
			Assert.AreEqual("IView", file.TypedViewName);

			AssertPageDirectiveHasBeenRemoved(file.RenderBody);
		}

    [Test]
    public void Process_WhenUsingClassNameAtDesignTimeAndGenericTypedView_SetsClassNameAndGenericView()
    {
      file.RenderBody = @"
<%@ Page Language=""C#"" Inherits=""SomeClassAtDesignTime<IView<Item>>"" %>
view content";
      step.Process(file);

      Assert.AreEqual("SomeClass<IView<Item>>", file.BaseClassName);
      Assert.AreEqual("IView<Item>", file.TypedViewName);

      AssertPageDirectiveHasBeenRemoved(file.RenderBody);
    }		


	}
}

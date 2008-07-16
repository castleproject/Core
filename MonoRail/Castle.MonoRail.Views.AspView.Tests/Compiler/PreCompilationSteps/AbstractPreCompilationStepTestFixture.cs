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
	using AspView.Compiler.PreCompilationSteps;
	using NUnit.Framework;

	public abstract class AbstractPreCompilationStepTestFixture
	{
		protected IPreCompilationStep step;
		protected SourceFile file;
		protected string expected;

		public AbstractPreCompilationStepTestFixture()
		{
			SetUp();
		}

		[SetUp]
		public void SetUp()
		{
			CreateSourceFile();
			CreateStep();
		}

		protected abstract void CreateStep();
		protected virtual void CreateSourceFile()
		{
			file = new SourceFile();
		}

		public void AssertStepOutput()
		{
			Assert.AreEqual(expected, file.RenderBody);
		}
	}
}

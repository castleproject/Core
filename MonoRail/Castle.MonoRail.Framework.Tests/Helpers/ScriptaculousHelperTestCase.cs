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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using Castle.MonoRail.Framework.Helpers;
	using NUnit.Framework;

	[TestFixture]
	public class ScriptaculousHelperTestCase
	{
		private ScriptaculousHelper helper = new ScriptaculousHelper();

		[Test]
		public void VisualEffectToggle()
		{
			Assert.AreEqual("Effect.toggle('el1', 'slide', {});", helper.VisualEffect("ToggleSlide", "el1"));
			Assert.AreEqual("Effect.toggle('el1', 'blind', {});", helper.VisualEffect("ToggleBlind", "el1"));
			Assert.AreEqual("Effect.toggle('el1', 'appear', {});", helper.VisualEffect("ToggleAppear", "el1"));
		}

		[Test]
		public void VisualEffect()
		{
			Assert.AreEqual("Effect.Highlight('el1', {});", helper.VisualEffect("Highlight", "el1"));
			Assert.AreEqual("Effect.Fade('el1', {});", helper.VisualEffect("Fade", "el1"));
			Assert.AreEqual("Effect.Shake('el1', {});", helper.VisualEffect("Shake", "el1"));
			Assert.AreEqual("Effect.DropOut('el1', {});", helper.VisualEffect("DropOut", "el1"));
		}
	}
}

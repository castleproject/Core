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
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;

	[TestFixture]
	public class ValidationHelperTestCase
	{
		private ValidationHelper _helper;

		public ValidationHelperTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			_helper = new ValidationHelper();
		}

		[Test, Ignore("We need to mock the controller and the request/context to be able to run this one")]
		public void AutoScriptInstaller()
		{
			Assert.AreEqual("<script type=\"text/javascript\" src=\"vdir/MonoRail/Files/ValidateConfig.rails\"></script>\r\n" +
				"<script type=\"text/javascript\" src=\"vdir/MonoRail/Files/ValidateCore.rails\"></script>\r\n" +
				"<script type=\"text/javascript\" src=\"vdir/MonoRail/Files/ValidateValidators.rails\"></script>\r\n"+
				"<script type=\"text/javascript\" src=\"vdir/MonoRail/Files/ValidateLang.rails\"></script>\r\n", 
				_helper.InstallScripts());
		}

		[Test]
		public void DefaultValidationTriggerFunction()
		{
			Assert.AreEqual("return validateForm( this, false, false, false, false, 0 );", 
				_helper.GetValidationTriggerFunction());
		}

		[Test]
		public void CustomValidationTriggerFunction()
		{
			Assert.AreEqual("return validateForm( document.forms[0], false, false, false, false, 0 );", 
				_helper.GetValidationTriggerFunction("document.forms[0]"));
		}

		[Test]
		public void OverrideDefaults()
		{
			_helper.SetSubmitOptions(true, true, true, 1);

			Assert.AreEqual("return validateForm( this, true, true, true, true, 1 );", 
				_helper.GetValidationTriggerFunction());
		}
	}
}

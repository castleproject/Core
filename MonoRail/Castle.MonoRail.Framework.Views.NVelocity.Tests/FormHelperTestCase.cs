// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void ParamsAreUsedByFormHelper()
		{
			ProcessView_StripRailsExtension("formhelper/UseParamsToFillInputs.rails", "somearg=abc", "otherarg=123");
			AssertSuccess();
			AssertReplyEqualTo("<input type=\"hidden\" id=\"somearg\" name=\"somearg\" value=\"abc\" />\r\n<input type=\"text\" id=\"otherarg\" name=\"otherarg\" value=\"123\" />");
		}
	}
}

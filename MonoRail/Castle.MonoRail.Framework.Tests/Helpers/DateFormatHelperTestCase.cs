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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;

	using Castle.MonoRail.Framework.Helpers;
	
	using NUnit.Framework;


	[TestFixture]
	public class DateFormatHelperTestCase
	{
		DateFormatHelper helper = new DateFormatHelper();

		[Test]
		public void FriendlyFormatWithNoDiff()
		{
			Assert.AreEqual("Just now", helper.FriendlyFormatFromNow( DateTime.Now ));
		}
		
		[Test]
		public void FriendlyFormatWithDiffOfOneSecond()
		{
			Assert.AreEqual("1 second ago", helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds(1) ));
		}

		[Test]
		public void FriendlyFormatWithDiffOfTenSeconds()
		{
			Assert.AreEqual("10 seconds ago", helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds(10) ));
		}

		[Test]
		public void FriendlyFormatWithDiffOfTenMinutes()
		{
			Assert.AreEqual("10 minutes ago", helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes(10) ));
		}

		[Test]
		public void FriendlyFormatWithDiffOf120Minutes()
		{
			Assert.AreEqual("2 hours ago", helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes(120) ));
		}

		[Test]
		public void FriendlyFormatWithDiffOf2Days()
		{
			Assert.AreEqual("2 days ago", helper.FriendlyFormatFromNow( DateTime.Now.AddDays(2) ));
		}
	}
}

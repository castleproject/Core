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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.CastleOnRails.Framework.Internal;
	using Castle.CastleOnRails.Framework.Tests.Controllers;


	[TestFixture]
	public class FilterAttributeTestCase
	{
		[Test]
		public void AttributeInFirstClass()
		{
			TheController controller = new TheController();

			FilterDescriptor[] descs = controller.GetFilters();
			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
		}
	}
}

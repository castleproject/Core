// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.ActiveRecord.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Facilities.ActiveRecord.Tests.Model1;

	/// <summary>
	/// Summary description for Model1TestCase.
	/// </summary>
	[TestFixture]
	public class Model1TestCase
	{
		[Test]
		public void DeleteAll()
		{
			IList orders = Order.DeleteAll();

			Order order = new Order();
			Order.Save( order );

			IList orders = Order.Find();

			Order order2 = Order.FindSingle( 1 );
		}

		[Test]
		public void SaveNew()
		{
			Order order = new Order();
			Order.Save( order );

		}

		[Test]
		public void SaveAndUpdate()
		{
			Order order = new Order();
			Order.Save( order );
			order.Tag = "";
			Order.Update( order );
		}

		[Test]
		public void FindAndFindSingle()
		{
			IList orders = Order.Find();
			Order order2 = Order.FindSingle( 1 );
		}
	}
}

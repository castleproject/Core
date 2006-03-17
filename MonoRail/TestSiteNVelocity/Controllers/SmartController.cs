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

namespace TestSiteNVelocity.Controllers
{
	using System;

	using Castle.MonoRail.Framework;
	using Nullables;

	public class SmartController : SmartDispatcherController
	{
		public void StringMethod(string name)
		{
			RenderText("incoming " + name);
		}

		public void Complex(string strarg, int intarg, String[] strarray)
		{
			RenderText(String.Format("incoming {0} {1} {2}", strarg, intarg, String.Join(",", strarray)));
		}

		public void SimpleBind([DataBind("order")] Order order)
		{
			RenderText(String.Format("incoming {0}", order.ToString()));
		}

		public void ComplexBind([DataBind("order")] Order order, [DataBind("person")] Person person)
		{
			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindExcludePrice([DataBind("order", Exclude="Price")] Order order, [DataBind("person")] Person person)
		{
			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindExcludeName([DataBind("order", Exclude="Name")] Order order, [DataBind("person")] Person person)
		{
			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindWithPrefix([DataBind("order")] Order order, [DataBind("person")] Person person)
		{
			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void FillingBehavior([DataBind("abc")] ClassWithInitializers clazz)
		{
			RenderText(String.Format("incoming {0} {1} {2}", clazz.Name, clazz.Date1.ToShortDateString(), clazz.Date2.ToShortDateString()));
		}

		public void NullableConversion(Nullables.NullableDouble amount)
		{
			RenderText(String.Format("incoming {0} {1}", amount.HasValue, amount.ToString()));
		}

		public void NullableConversion2([DataBind("mov")] Movement movement)
		{
			RenderText(String.Format("incoming {0} {1}", movement.Name, movement.Amount.ToString()));
		}

		public void ArrayBinding([DataBind("user")] User2 user)
		{
			RenderText(user.ToString());
			
			foreach(int id in user.Roles)
			{
				RenderText(" " + id);
			}
			foreach(int id in user.Permissions)
			{
				RenderText(" " + id);
			}
		}
	}

	public class ClassWithInitializers
	{
		private String name;
		private DateTime date1;
		private DateTime date2;

		public ClassWithInitializers()
		{
			name = "hammett";
			date1 = DateTime.Now;
			date2 = DateTime.Now.AddDays(1);
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public DateTime Date1
		{
			get { return date1; }
			set { date1 = value; }
		}

		public DateTime Date2
		{
			get { return date2; }
			set { date2 = value; }
		}
	}

	public class Order
	{
		private String name;
		private int itemCount;
		private float price;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public int ItemCount
		{
			get { return itemCount; }
			set { itemCount = value; }
		}

		public float Price
		{
			get { return price; }
			set { price = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} {1} {2}", name, itemCount, price);
		}
	}

	public class Person
	{
		String id;
		Contact contact;

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public Contact Contact
		{
			get { return contact; }
			set { contact = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", id, contact);
		}
	}

	public class Contact
	{
		String email, phone;

		public Contact()
		{
		}

		public Contact(string email, string phone)
		{
			this.email = email;
			this.phone = phone;
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", email, phone);
		}
	}

	public class Movement
	{
		String name; Nullables.NullableDouble amount;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public NullableDouble Amount
		{
			get { return amount; }
			set { amount = value; }
		}
	}

	public class User2
	{
		String name; int[] roles; int[] permissions;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public int[] Roles
		{
			get { return roles; }
			set { roles = value; }
		}

		public int[] Permissions
		{
			get { return permissions; }
			set { permissions = value; }
		}

		public override string ToString()
		{
			return String.Format("User {0} {1} {2}", name, roles.Length, permissions.Length);
		}
	}
}
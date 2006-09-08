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

	/// <summary>
	/// Same as <see cref="SmartController"/> but using
	/// BindObject methods
	/// </summary>
	public class Smart2Controller : SmartDispatcherController
	{
		public void SimpleBind()
		{
			Order order = (Order) BindObject(typeof(Order), "order");
			RenderText(String.Format("incoming {0}", order.ToString()));
		}

		public void SimpleBindArray()
		{
			Order[] orders = (Order[]) BindObject(typeof(Order[]), "orders");
			
			if (orders == null)
			{
				RenderText("Null array shouldn't be returned by databinder");
			}
			else
			{
				RenderText(String.Format("incoming {0}", orders.Length));
			}
		}

		public void ComplexBind()
		{
			Order order = (Order) BindObject(typeof(Order), "order");
			Person person = (Person) BindObject(typeof(Person), "person");

			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindExcludePrice()
		{
			Order order = (Order) BindObject(ParamStore.Params, typeof(Order), "order", "Price", null);
			Person person = (Person) BindObject(typeof(Person), "person");

			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindExcludeName()
		{
			Order order = (Order) BindObject(ParamStore.Params, typeof(Order), "order", "Name", null);
			Person person = (Person) BindObject(typeof(Person), "person");

			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void ComplexBindWithPrefix()
		{
			Order order = (Order) BindObject(typeof(Order), "order");
			Person person = (Person) BindObject(typeof(Person), "person");

			RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()));
		}

		public void FillingBehavior()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			ClassWithInitializers clazz = (ClassWithInitializers) BindObject(typeof(ClassWithInitializers), "abc");
			
			RenderText(String.Format("incoming {0} {1} {2}", clazz.Name, clazz.Date1.ToShortDateString(), clazz.Date2.ToShortDateString()));
		}

		public void NullableConversion2()
		{
			Movement movement = (Movement) BindObject(typeof(Movement), "mov");
			
			RenderText(String.Format("incoming {0} {1}", movement.Name, movement.Amount.ToString()));
		}

		public void ArrayBinding()
		{
			User2 user = (User2) BindObject(typeof(User2), "user");

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

		public void CalculateUtilizationByDay()
		{
			TimePoint tp1 = (TimePoint) BindObject(typeof(TimePoint), "tp1");
			TimePoint tp2 = (TimePoint) BindObject(typeof(TimePoint), "tp2");
			
			RenderText(tp1.ToString());
			RenderText(tp2.ToString());
		}
	}
}

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
namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
import System
import Castle.MonoRail.Framework

class SmartController(SmartDispatcherController):
	def StringMethod(name as string):
		RenderText("incoming ${name}")
		
	def Complex(strarg as string, intarg as int, strarray as (String)):
		RenderText(String.Format("incoming {0} {1} {2}", strarg, intarg, String.Join(",", strarray)))
	
	def SimpleBind([Castle.MonoRail.Framework.DataBind] order as Order):
		RenderText(String.Format("incoming {0}", order.ToString()))
	
	def ComplexBind([Castle.MonoRail.Framework.DataBind] order as Order, [DataBind()] person as Person):
		RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()))
	
	def ComplexBindWithPrefix([Castle.MonoRail.Framework.DataBind] order as Order, [DataBind(Prefix: "person")] person as Person):
		RenderText(String.Format("incoming {0} {1}", order.ToString(), person.ToString()))
	
	def FillingBehavior([Castle.MonoRail.Framework.DataBind] clazz as ClassWithInitializers):
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us")
		RenderText(String.Format("incoming {0} {1} {2}", clazz.Name, clazz.Date1.ToShortDateString(), clazz.Date2.ToShortDateString()))
	


class ClassWithInitializers:
	private _name as String
	
	private _date1 as date
	
	private _date2 as date
	
	def constructor():
		_name = "hammett"
		_date1 = DateTime.Now
		_date2 = DateTime.Now.AddDays(1)
	
	Name as string:
		get:
			return _name
		set:
			_name = value
	
	Date1 as date:
		get:
			return _date1
		set:
			_date1 = value
	
	Date2 as date:
		get:
			return _date2
		set:
			_date2 = value
	


class Order:
	private _name as String
	
	private _itemCount as int
	
	private _price as single
	
	Name as string:
		get:
			return _name
		set:
			_name = value
	
	ItemCount as int:
		get:
			return _itemCount
		set:
			_itemCount = value
	
	Price as single:
		get:
			return _price
		set:
			_price = value
	
	override def ToString() as string:
		return String.Format("{0} {1} {2}", _name, _itemCount, _price)
	


class Person:
	_id as String
	
	_contact as Contact
	
	Id as string:
		get:
			return _id
		set:
			_id = value
	
	Contact as Contact:
		get:
			return _contact
		set:
			_contact = value
	
	override def ToString() as string:
		return String.Format("{0} {1}", _id, _contact)
	


class Contact:
	_email as String
	_phone as String
	
	Email as string:
		get:
			return _email
		set:
			_email = value
	
	Phone as string:
		get:
			return _phone
		set:
			_phone = value
	
	override def ToString() as string:
		return String.Format("{0} {1}", _email, _phone)

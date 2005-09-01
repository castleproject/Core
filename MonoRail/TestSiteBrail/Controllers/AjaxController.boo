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
import System.Collections
import Castle.MonoRail.Framework
import Castle.MonoRail.Framework.Helpers

class AjaxController(SmartDispatcherController):

	Helper() as AjaxHelper:
		get:
			return cast(AjaxHelper, Helpers["AjaxHelper"])

	def JsFunctions():
		RenderText(Helper.GetJavascriptFunctions())
	
	def LinkToFunction():
		RenderText(Helper.LinkToFunction("<img src='myimg.gid'>", "alert('Ok')"))
	
	def LinkToRemote():
		RenderText(Helper.LinkToRemote("<img src='myimg.gid'>","/controller/action.rails",Hashtable()) )
	
	def BuildFormRemoteTag():
		RenderText(Helper.BuildFormRemoteTag("url",null,null))
		
	def ObserveField():
		RenderText(Helper.ObserveField("myfieldid",2,"/url","elementToBeUpdated", "newcontent"))
		
	def ObserveForm():
		RenderText(Helper.ObserveForm("myfieldid",2,"/url","elementToBeUpdated", "newcontent"))
		
	def Index():
		list = GetList()
		PropertyBag.Add("users", list)
		
	def PeriodInvocation():
		pass
	
	def PeriodInvokeTarget():
		RenderText("Ok")
	
	def AutoCom():
		pass
	
	def NameAutoCompletion(name as string):
		RenderText("<ul class=\"names\"><li class=\"name\">Jisheng Johnny</li><li class=\"name\">John Diana</li><li class=\"name\">Johnathan Maurice</li></ul>")
	
	def AddUserWithAjax(name as string, email as string):
		GetList().Add( User(name,email))
		Index()
		RenderView("/userlist")
	
	def AccountFormValidate(name as string, addressf as string):
		message = ""
		message = "<b>Please, dont forget to enter the name<b>" if name is null or name.Length==0
		message += "<>Please, don't forget to enter the adress<b>" if addressf is null or addressf.Length==0
		RenderText(message)
		
	private def GetList():
		list as IList = Context.Session["list"] as IList
		if list is null:
			list = ArrayList()
			list.Add( User("somefakeuser","fakeemail@server.net"))
			list.Add( User("someotherfakeuser","otheremail@server.net"))
			
			Context.Session["list"] = list
		
		return list
	
	private class User:
		
		[Property(Name)]
		name as string
		
		[Property(Email)]
		email as string
		
		def constructor(name as string, email as string):
			self.name = name
			self.email = email
	
	

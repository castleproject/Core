// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace TestSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	public class PostBackController : SmartDispatcherController
	{
		public PostBackController()
		{
		}

		public void Index()
		{
		}

		public void Hello(string name)
		{
			PropertyBag.Add("name", name);
		}	
		
		public void SayHello(string name)
		{
			RenderText("Hello {0}!", name);
		}
		
		public void CustomArgs()
		{
			
		}
		
		public void TestNullArgs(int i, double d, string s)
		{
			PropertyBag.Add("i", i);
			PropertyBag.Add("d", d);
			PropertyBag.Add("s", s);
			
			CancelView();
		}
	}
}

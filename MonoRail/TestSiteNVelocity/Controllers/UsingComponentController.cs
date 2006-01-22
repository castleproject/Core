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
	using System.Collections;

	using Castle.MonoRail.Framework;


	public class UsingComponentController : Controller
	{
		public void CaptureFor()
		{			
		}

		public void CaptureForAppend()
		{			
		}

		public void CaptureForAppendBefore()
		{			
		}

		public void InlineComponentUsingTemplatedRender()
		{
			PropertyBag.Add("var1", "v1");
			PropertyBag.Add("var2", "v2");
			PropertyBag.Add("fromPropertyBag", "items from property bag");
		}

		public void Index()
		{
		}

		public void Index1()
		{
		}

		public void Index3()
		{
			ArrayList items = new ArrayList();

			items.Add("1");
			items.Add("2");

			PropertyBag.Add("items", items);
		}

		public void Index4()
		{
		}
	
		public void Index5()
		{
		}
	
		public void Index6()
		{
		}

		public void Index7()
		{
		}

		public void Index8()
		{
			ArrayList items = new ArrayList();

			items.Add("1");
			items.Add("2");

			PropertyBag.Add("items", items);
		}

		public void Index9()
		{
		}
	}
}

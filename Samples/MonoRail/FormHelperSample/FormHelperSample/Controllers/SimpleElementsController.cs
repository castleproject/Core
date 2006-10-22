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

namespace FormHelperSample.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using FormHelperSample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class SimpleElementsController : SmartDispatcherController
	{
		public void TextFieldIndex()
		{
			PropertyBag.Add("simplevalue", "hammett");
			PropertyBag.Add("product", new Product(1, "MacBook Pro", 2800.00f, new Supplier(1, "Apple")));
		}
		
		public void TextFieldValueIndex()
		{
			PropertyBag.Add("simplevalue", "hammett");
		}

		public void TextFieldFormatIndex()
		{
			PropertyBag.Add("today", DateTime.Now);
			PropertyBag.Add("value", 10.45f);
			PropertyBag.Add("product", new Product(1, "MacBook Pro", 2800.00f, new Supplier(1, "Apple")));
		}
		
		public void TextAreaIndex()
		{
			PropertyBag.Add("simplevalue", "hammett");
		}
		
		public void PasswordFieldIndex()
		{
			PropertyBag.Add("simplevalue", "hammett");
		}
		
		public void HiddenFieldIndex()
		{
			PropertyBag.Add("simplevalue", "hammett");
		}		
	}
}

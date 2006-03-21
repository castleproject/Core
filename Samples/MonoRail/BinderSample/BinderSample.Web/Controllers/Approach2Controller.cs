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

namespace BinderSample.Web.Controllers
{
	using Castle.MonoRail.Framework;

	using BinderSample.Web.Model;


	[Layout("scaffold")]
	public class Approach2Controller : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag.Add("publishers", Publisher.FindAll());
		}

		public void EditPublisher(int publisherId)
		{
			PropertyBag.Add("publisher", Publisher.Find(publisherId));

			RenderView("EditPublisher");
		}

		public void Update([DataBind("publisher")] Publisher formpublisher,
			[DataBind("book")] Book[] formBooks)
		{
			Publisher publisher = Publisher.Find(formpublisher.Id);
			publisher.Name = formpublisher.Name;

			foreach(Book formBook in formBooks)
			{
				Book book = Book.Find(formBook.Id);

				book.Name = formBook.Name;
				book.Author = formBook.Author;

				book.Save();
			}

			publisher.Save();

			EditPublisher(publisher.Id);
		}
	}
}

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

namespace Castle.Applications.MindDump.Presentation.Controllers
{
	using System;

	using Castle.CastleOnRails.Framework;

	using Castle.Applications.MindDump.Model;
	using Castle.Applications.MindDump.Services;
	using Castle.Applications.MindDump.Adapters;

	[Layout("private")]
	public class MaintenanceController : AbstractSecureController
	{
		private BlogMaintenanceService _maintenanceService;
		private AccountService _accountService;

		public MaintenanceController(AccountService accountService, BlogMaintenanceService maintenanceService)
		{
			_accountService = accountService;
			_maintenanceService = maintenanceService;
		}

		public void NewEntry()
		{
			PropertyBag.Add( "entries", _maintenanceService.ObtainPosts( ObtainCurrentBlog() ) );
		}

		public void SaveNewEntry(String title, String contents)
		{
			Blog blog = ObtainCurrentBlog();

			Post post = _maintenanceService.CreateNewPost( 
				blog, new Post(title, contents, DateTime.Now) );

			Context.Flash["message"] = "Your post was created successfully";

			RenderView("EditEntry");
			EditEntry( post.Id );
		}

		public void EditEntry(long entryid)
		{
			PropertyBag.Add( "entries", _maintenanceService.ObtainPosts( ObtainCurrentBlog() ) );
			PropertyBag.Add( "post", _maintenanceService.ObtainPost( ObtainCurrentBlog(), entryid ) );
		}

		public void UpdateEntry(long entryid, String title, String contents)
		{
			Blog blog = ObtainCurrentBlog();

			_maintenanceService.UpdatePost( 
				blog, entryid, new Post(title, contents) );

			Context.Flash["message"] = "Your post was updated successfully";

			RenderView("EditEntry");
			EditEntry( entryid );
		}

		public void AccountSettings()
		{
			PropertyBag.Add("author", ObtainCurrentAuthor() );
		}

		public void UpdateAccount(String name, String email)
		{
			Author author = ObtainCurrentAuthor();

			author.Name = name;

			_accountService.UpdateAccount(author);

			RenderView("NewEntry");
			NewEntry();
		}

		public void BlogSettings()
		{
			PropertyBag.Add("blog", ObtainCurrentBlog() );
		}

		public void UpdateBlog(String blogname, String blogdesc, String theme)
		{
			Blog blog = ObtainCurrentBlog();

			blog.Name = blogname;
			blog.Description = blogdesc;
			blog.Theme = theme;

			_accountService.UpdateBlog(blog);

			RenderView("NewEntry");
			NewEntry();
		}


		// Private utility members

		private Blog ObtainCurrentBlog()
		{
			Author author = ObtainCurrentAuthor();
			return author.Blogs[0] as Blog;
		}

		private Author ObtainCurrentAuthor()
		{
			return (Context.CurrentUser as PrincipalAuthorAdapter).Author;
		}
	}
}

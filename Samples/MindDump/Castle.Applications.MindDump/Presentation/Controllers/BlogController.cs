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
	using System.Collections;

	using Castle.CastleOnRails.Framework;

	using Castle.Applications.MindDump.Model;
	using Castle.Applications.MindDump.Services;
	using Castle.Applications.MindDump.Presentation.Filters;


	[ControllerDetails("blogs")]
	[Filter(ExecuteEnum.After, typeof(PrintableFilter))]
	public class BlogController : SmartDispatcherController
	{
		private BlogService _blogService;

		public BlogController(BlogService blogService)
		{
			_blogService = blogService;
		}

		public void View()
		{
			Blog blog = _blogService.ObtainBlogByAuthorName( Name );

			LayoutName = blog.Theme;

			IList posts = _blogService.ObtainPosts( blog );

			PropertyBag.Add( "blog", blog );
			PropertyBag.Add( "posts", posts );

			if (posts.Count != 0)
			{
				PropertyBag.Add( "lastpost", posts[ posts.Count - 1 ] );
			}

			RenderView("blogs", "view");
		}

		public void View(long entryid)
		{
			Blog blog = _blogService.ObtainBlogByAuthorName( Name );

			LayoutName = blog.Theme;

			IList posts = _blogService.ObtainPosts( blog );

			PropertyBag.Add( "blog", blog );
			PropertyBag.Add( "posts", posts );
			PropertyBag.Add( "lastpost", _blogService.ObtainPost(blog, entryid) );

			RenderView("blogs", "view");
		}
	}
}

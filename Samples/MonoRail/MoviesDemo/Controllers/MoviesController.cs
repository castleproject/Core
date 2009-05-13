// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace MoviesDemo.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using MoviesDemo.Model;

	[Resource("loc", "MoviesDemo.Resources.Movies")]
	public class MoviesController : AbstractHtmlPageController
	{
		public void List()
		{
			PropertyBag["movies"] = Movie.FindAll();
		}

		public void Edit(int movieId)
		{
			Movie movie = movieId == 0 ? new Movie() : Movie.Find(movieId);

			PropertyBag["movie"] = movie;
		}

		public void View(int movieId)
		{
			PropertyBag["movie"] = Movie.Find(movieId);
		}

		/// <summary>
		/// This is a rather simplistic implementation of a save action.
		/// It's only OK as the Movie class does not have any relation. 
		/// For a more reliable method, check ARDataBindAttribute on 
		/// the documentation 
		/// </summary>
		public void Save([DataBind("movie")] Movie movie)
		{
			if (movie.Id == 0)
			{
				movie.Create();
			}
			else
			{
				movie.Save();
			}

			//Redirect so a refresh doesn't post again
			Response.RedirectToUrl("~/Movies/View.castle?movieId=" + movie.Id);

			CancelView();
		}
	}
}
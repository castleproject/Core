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

namespace AspectSharp.Example
{
	using System;
	using System.IO;
	using System.Collections;

	/// <summary>
	/// Very naive implementation of a request pipeline
	/// </summary>
	public class RequestPipeline
	{
		private Context context;
		private IList contentProviders;
		private IView view;

		public RequestPipeline()
		{
			context = new Context();
			contentProviders = new ArrayList();
		}

		public virtual Context Context
		{
			get { return context; }
			set { context = value; }
		}

		public virtual IView View
		{
			get { return view; }
			set { view = value; }
		}

		public virtual void AddContentProvider(IContentProvider provider)
		{
			contentProviders.Add( provider );
		}

		public virtual void ProcessRequest( TextWriter writer )
		{
			ArrayList fragments = new ArrayList();

			foreach(IContentProvider provider in contentProviders)
			{
				fragments.Add( provider.RetrieveContent( Context ) );
			}

			IContentFragment[] contents = fragments.ToArray( typeof(IContentFragment) ) 
				as IContentFragment[];

			View.Show( contents, writer );
		}
	}
}

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

using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
using Template = NVelocity.Template;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System.IO;

	public class CustomTemplate : Template
	{
		private IViewSource viewSource;

		/// <summary>
		/// gets the named resource as a stream, parses and inits
		/// </summary>
		public override bool Process()
		{
			viewSource = ((ResourceLoaderAdapter) resourceLoader).GetViewSource(Name);

			if (viewSource == null)
			{
				throw new ResourceNotFoundException("Resource could not be located: " + Name);
			}

			return base.Process();
		}

		public bool IsModified
		{
			get
			{
				return viewSource.EnableCache && viewSource.LastUpdated < viewSource.LastModified;
			}
		}

		protected override Stream ObtainStream()
		{
			return viewSource.OpenViewStream();
		}
	}
}

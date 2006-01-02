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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Extends the <see cref="ViewComponent"/> and
	/// provides automatic mapping of the parameters
	/// to public writable properties of the component instance
	/// </summary>
	public class SmartViewComponent : ViewComponent
	{
		public override void Initialize()
		{
			NameValueCollection allParams = new NameValueCollection(Request.Params);
			
			foreach(DictionaryEntry entry in this.ComponentParams)
			{
				allParams[ entry.Key.ToString() ] = entry.ToString();
			}

			DataBinder binder = new DataBinder();

			binder.BindObjectInstance( this, String.Empty, allParams, Request.Files, null );
		}
	}
}

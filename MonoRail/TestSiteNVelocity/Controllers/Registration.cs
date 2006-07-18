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

	[Layout("master")]
	public class Registration : SmartDispatcherController
	{
		public void Index()
		{
		}

		public void List()
		{
			IList customers = new ArrayList();
			customers.Add( "" );

			PropertyBag.Add("customers", customers);
		}

		public void Save(String name, String address, String city, String country, int age)
		{
			// Inserting...

			// Done!

			RenderView( "success" );
		}
		
		public void PostHere( string p1, int p2 )
		{
			RenderText( string.Format( "<p>param1={0}</p><p>param2={1}</p>", p1, p2 ) );
		}
	}
}

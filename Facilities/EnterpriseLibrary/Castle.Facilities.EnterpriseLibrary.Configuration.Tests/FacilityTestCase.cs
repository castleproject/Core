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

namespace Castle.Facilities.EnterpriseLibrary.Configuration.Tests
{
	using System;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Configuration.Sources;

	using NUnit.Framework;

	[TestFixture]
	public class FacilityTestCase
	{
		[Test]
		public void LoadingConfig()
		{
			IWindsorContainer container = new WindsorContainer( new XmlInterpreter(new AppDomainConfigSource()) );
			
			EditorService service = (EditorService) container[ typeof(EditorService) ];

			Assert.AreEqual("Microsoft Sans Serif", service.Data.Name);

			// Container dispose will correctly terminate the components
			// and for a component that came from configuration, this means
			// writing it to the store. We will skip that just to not touch the file 
			// over and over
			// container.Dispose();
		}
	}
}

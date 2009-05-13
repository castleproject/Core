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

namespace Castle.MicroKernel.Tests.Bugs
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class IoC_117
	{
		[Test]
		public void Public_property_with_Protected_setter_causes_Object_Reference_exception()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddComponent<Presenter>();
			kernel.AddComponent<View>();

			try
			{
				Presenter p = (Presenter)kernel.Resolve(typeof(Presenter));
				Assert.IsNotNull(p);
			}
			catch (NullReferenceException)
			{
				Assert.Fail("Should not have thrown a NullReferenceException");
			}
		}
	}

	public class Presenter
	{
		private View view;

		public virtual View View
		{
			get { return view;  }
			protected set { view = value;}
		}
	}

	public class View
	{
	}
}

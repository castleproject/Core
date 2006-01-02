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

namespace Castle.Model.Tests.Resources
{
	using System;

	using NUnit.Framework;
	
	using Castle.Model.Resource;



	[TestFixture]
	public class AssemblyResourceFactoryTestCase
	{
		private AssemblyResourceFactory resFactory = new AssemblyResourceFactory();
		private String AssemblyName = "Castle.Model.Tests";
		private String ResPath = "Resources";

		[Test]
		public void Accept()
		{
			Assert.IsTrue(  resFactory.Accept( new Uri("assembly://something/") ) );
			Assert.IsFalse( resFactory.Accept( new Uri("file://something") ) );
			Assert.IsFalse( resFactory.Accept( new Uri("http://www.castleproject.org") ) );
		}

		[Test]
		public void CreateWithAbsolutePath()
		{
			IResource resource = resFactory.Create( new Uri("assembly://" + AssemblyName + "/" + ResPath + "/file1.txt") );

			Assert.IsNotNull(resource);
			String line = resource.GetStreamReader().ReadLine();
			Assert.AreEqual("Something", line);
		}
	}
}

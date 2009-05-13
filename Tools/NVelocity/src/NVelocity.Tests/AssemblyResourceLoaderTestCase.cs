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

namespace NVelocity
{
	using System.Collections.Generic;
	using App;
	using Commons.Collections;
	using Exception;
	using NUnit.Framework;

	/// <summary>
	/// Test cases for the assembly resource loader.
	/// </summary>
	[TestFixture]
	public class AssemblyResourceLoaderTestCase
	{
		[Test]
		public void AllowsSingleLoaderAssembly()
		{
			ExtendedProperties properties = GetBasicProperties();
			properties.AddProperty("assembly.resource.loader.assembly", "MyTestAssembly");

			new VelocityEngine().Init(properties);
		}

		[Test]
		public void AllowsMultipleLoaderAssemblies()
		{
			ExtendedProperties properties = GetBasicProperties();
			properties.AddProperty("assembly.resource.loader.assembly",
				new List<string> { "MyTestAssembly", "MyTestAssembly2" });

			new VelocityEngine().Init(properties);
		}

		[Test]
		public void ThrowsWhenUnsupportedType()
		{
			ExtendedProperties properties = GetBasicProperties();
			properties.AddProperty("assembly.resource.loader.assembly", 123456);

			try
			{
				new VelocityEngine().Init(properties);
				Assert.Fail();
			}
			catch (VelocityException vex)
			{
				Assert.AreEqual("Expected property 'assembly' to be of type string or List<string>.",
					vex.Message);
			}
		}

		private static ExtendedProperties GetBasicProperties()
		{
			ExtendedProperties properties = new ExtendedProperties();
			properties.AddProperty("resource.loader", "assembly");
			properties.AddProperty("assembly.resource.loader.class",
				"NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader, NVelocity");
			return properties;
		}
	}
}
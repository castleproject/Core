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

namespace Castle.DynamicProxy.Tests
{
	using System.IO;
	using System.Xml.Serialization;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Classes;
	
	using NUnit.Framework;

	[TestFixture]
	public class XmlSerializationTestCase : BasePEVerifyTestCase
	{
		[Test, Ignore("Could not come up with a solution for this")]
		public void ProxyIsXmlSerializable()
		{
			ProxyGenerator gen = new ProxyGenerator();

			ClassToSerialize proxy = (ClassToSerialize) 
				gen.CreateClassProxy(typeof(ClassToSerialize), new StandardInterceptor());

			XmlSerializer serializer = new XmlSerializer(proxy.GetType());
			
			StringWriter writer = new StringWriter();
			
			serializer.Serialize(writer, proxy);
			
			StringReader reader = new StringReader(writer.GetStringBuilder().ToString());

			object newObj = serializer.Deserialize(reader);

			Assert.IsNotNull(newObj);
			Assert.IsInstanceOfType(typeof(ClassToSerialize), newObj);
		}
	}
}

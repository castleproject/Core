// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy.Test
{
	using System;

	using NUnit.Framework;

	using Apache.Avalon.DynamicProxy.Test.Classes;

	/// <summary>
	/// Summary description for SerializableClassTestCase.
	/// </summary>
	[TestFixture]
	public class SerializableClassTestCase
	{
		[Test]
		public void CreateSerializable()
		{
			MySerializableClass myClass = new MySerializableClass();

			ProxyGenerator generator = new ProxyGenerator();
			MySerializableClass proxy = (MySerializableClass) 
				generator.CreateClassProxy( typeof(MySerializableClass), new StandardInvocationHandler(myClass) );

			Assert.IsTrue( proxy.GetType().IsSerializable );
		}
	}
}

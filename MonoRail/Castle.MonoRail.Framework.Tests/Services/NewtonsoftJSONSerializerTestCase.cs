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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System;
	using System.Linq;
	using Framework.Services;
	using JSON;
	using NUnit.Framework;

	[TestFixture]
	public class NewtonsoftJSONSerializerTestCase
	{
		[Test]
		public void Serialize()
		{
			NewtonsoftJSONSerializer serializer = new NewtonsoftJSONSerializer();

			Person p = new Person { Name = "Json", Age = 23};
			Assert.AreEqual("{\"Name\":\"Json\",\"Age\":23}", serializer.Serialize(p));
		}

		[Test]
		public void SerializeWithConverter()
		{
			NewtonsoftJSONSerializer serializer = new NewtonsoftJSONSerializer();

			Person p = new Person { Name = "Json", Age = 23 };

			ValueTypeConverter converter = new ValueTypeConverter();
            Assert.AreEqual("{\"Age\":23}", serializer.Serialize(p, converter));
		}

		class ValueTypeConverter : IJSONConverter
		{
			public void Write(IJSONWriter writer, object value)
			{
				writer.WriteStartObject();
				foreach (var prop in ( from x in value.GetType().GetProperties() where x.PropertyType.IsValueType select x))
				{
					writer.WritePropertyName(prop.Name);
					writer.WriteValue(prop.GetValue(value, null));
				}
				writer.WriteEndObject();
			}

			public bool CanHandle(Type type)
			{
				return type == typeof(Person);
			}

			public object ReadJson(IJSONReader reader, Type objectType)
			{
				throw new NotImplementedException();
			}
		}
	}
}
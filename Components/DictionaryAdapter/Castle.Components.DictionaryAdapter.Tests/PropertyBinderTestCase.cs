// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections;

namespace Castle.Components.DictionaryAdapter.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class PropertyBinderTestCase
	{
		private DictionaryAdapterFactory factory;
		private IDictionary dictionary;

		[SetUp]
		public void SetUp()
		{
			dictionary = new Hashtable();
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void StringToNullableIntBinder_WithAnIntAsAString_ConvertsStringToInt()
		{
			DictionaryAdapterStringToNullableIntBinder stringBinder = new DictionaryAdapterStringToNullableIntBinder();

			int wheels = 3;

			Assert.AreEqual(wheels, ((int?)stringBinder.ConvertFromDictionary(wheels.ToString())).Value);
		}

		[Test]
		public void StringToNullableIntBinder_WithAnStringAsAnInt_ConvertsIntToString()
		{
			DictionaryAdapterStringToNullableIntBinder stringBinder = new DictionaryAdapterStringToNullableIntBinder();

			int wheels = 3;

			Assert.AreEqual(wheels.ToString(), stringBinder.ConvertFromInterface(wheels));
		}

		[Test]
		public void StringToNullableIntBinder_WithANonString_ThrowsException()
		{
			DictionaryAdapterStringToNullableIntBinder stringBinder = new DictionaryAdapterStringToNullableIntBinder();

			try
			{
				stringBinder.ConvertFromDictionary(true);
				Assert.Fail();
			}
			catch(DictionaryAdapterPropertyBindingException e)
			{
			}
			catch(Exception e)
			{
				Assert.Fail();
			}
		}

		[Test]
		public void StringToNullableGuidBinder_WithAGuidAsAString_ConvertsStringToGuid()
		{
			DictionaryAdapterStringToNullableGuidBinder guidBinder = new DictionaryAdapterStringToNullableGuidBinder();

			Guid vin = Guid.NewGuid();

			Assert.AreEqual(vin, ((Guid?)guidBinder.ConvertFromDictionary(vin.ToString())).Value);
		}

		[Test]
		public void StringToNullableGuidBinder_WithAnStringAsAGuid_ConvertsGuidToString()
		{
			DictionaryAdapterStringToNullableGuidBinder guidBinder = new DictionaryAdapterStringToNullableGuidBinder();

			Guid vin = Guid.NewGuid();

			Assert.AreEqual(vin.ToString(), guidBinder.ConvertFromInterface(vin));
		}

		[Test]
		public void ReadStringValue_AsInteger_ReturnsInteger()
		{
			int wheels = 4;
			dictionary["Wheels"] = wheels.ToString();

			Guid vin = Guid.NewGuid();
			dictionary["Vin"] = vin.ToString();

			ICar car = factory.GetAdapter<ICar>(dictionary);

			Assert.AreEqual(wheels, car.Wheels);
			Assert.AreEqual(vin, car.Vin);
		}

		[Test]
		public void WriteIntegerValue_AsString_StoresString()
		{
			int wheels = 3;
			Guid vin = Guid.NewGuid();

			ICar car = factory.GetAdapter<ICar>(dictionary);
			car.Wheels = wheels;
			car.Vin = vin;

			Assert.AreEqual(typeof(string), dictionary["Wheels"].GetType());
			Assert.AreEqual(wheels.ToString(), dictionary["Wheels"]);
			Assert.AreEqual(typeof(string), dictionary["Vin"].GetType());
			Assert.AreEqual(vin.ToString(), dictionary["Vin"]);
		}
	}
	
	public interface ICar
	{
		[DictionaryAdapterPropertyBinder(typeof(DictionaryAdapterStringToNullableIntBinder))]
		int? Wheels { get; set; }

		[DictionaryAdapterPropertyBinder(typeof(DictionaryAdapterStringToNullableGuidBinder))]
		Guid? Vin { get; set; }
	}
}

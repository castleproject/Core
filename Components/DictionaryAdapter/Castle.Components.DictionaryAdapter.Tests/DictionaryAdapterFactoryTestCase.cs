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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using NUnit.Framework;
	using PropertyDescriptor = Castle.Components.DictionaryAdapter.PropertyDescriptor;

	[TestFixture]
	public class DictionaryAdapterFactoryTestCase
	{
		private IDictionary dictionary;
		private DictionaryAdapterFactory factory;

		[SetUp]
		public void SetUp()
		{
			dictionary = new Hashtable();
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void CreateAdapter_NoPrefixPropertiesOnly_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test]
		public void CreateAdapter_AdaptingGenericInterface_Works()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container);
			// create a fake person
			var person = factory.GetAdapter<IPerson>(dictionary);
			container.Item = person;

			Assert.AreSame(person, container.Item);
		}

		[Test]
		public void CreateAdapter_AdaptingGenericInterfaceWithComponent_Works()
		{
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			Assert.IsNotNull(container);
			// create a fake person
			container.Item.Age = 5;
			container.Item.First_Name = "Andre";
			Assert.AreEqual(5, container.Item.Age);
			Assert.AreEqual("Andre", container.Item.First_Name);
		}

		[Test, ExpectedException(typeof(TypeLoadException))]
		public void CreateAdapter_NoPrefixWithMethod_ThrowsException()
		{
			factory.GetAdapter<IPersonWithMethod>(dictionary);
		}

		[Test]
		public void CreateAdapter_PrefixPropertiesOnly_WorksFine()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test]
		public void UpdateAdapter_NoPrefix_UpdatesDictionary()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>(); // I need some friends

			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>)dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapter_Prefix_UpdatesDictionary()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();
			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>)dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapterAndRead_NoPrefix_Matches()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_Prefix_Matches()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_PrefixOverride_Matches()
		{
			var person = factory.GetAdapter<IPersonWithPrefixOverride>(dictionary);
			person.Name = "Craig";

			Assert.AreEqual("Craig", dictionary["Name"]);
		}

		[Test]
		public void UpdateAdapterAndRead_TypePrefix_Matches()
		{
			var person = factory.GetAdapter<IPersonWithTypePrefixOverride>(dictionary);
			person.Height = 72;

			Assert.AreEqual(72, person.Height);
			Assert.AreEqual(72, dictionary["Castle.Components.DictionaryAdapter.Tests.IPersonWithTypePrefixOverride#Height"]);
		}

		[Test]
		public void ReadAdapter_NoPrefixUnitialized_ReturnsDefaults()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void ReadAdapter_PrefixUnitialized_ReturnsDefaults()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void UpdateAdapterAndRead_WithSeveralDifferentOverridesWithDifferentPrefixes_DictionaryKeysHaveCorrectPrefixes()
		{
			var  person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);

			string name = "Ming The Merciless";
			int numberOfFeet = 2;
			int numberOfHeads = 1;
			int numberOfFingers = 3;

			person.Name = name;
			person.NumberOfFeet = numberOfFeet;
			person.HairColor = Color.Green;
			person.EyeColor = Color.Blue;
			person.NumberOfHeads = numberOfHeads;
			person.NumberOfFingers = numberOfFingers;

			string[] keys = new string[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(keys, 0);

			Assert.IsTrue(Array.Exists(keys, key => key == "Name"));
			Assert.IsTrue(Array.Exists(keys, key => key == "NumberOfFeet"));
			Assert.IsTrue(Array.Exists(keys, key => key == "Person_HairColor"));
			Assert.IsTrue(Array.Exists(keys, key => key == "Person2_Eye__Color"));
			Assert.IsTrue(Array.Exists(keys, key => key == "Person2_NumberOfHeads"));
			Assert.IsTrue(Array.Exists(keys, key => key == "NumberOfFingers"));

			Assert.AreEqual(name, person.Name);
			Assert.AreEqual(numberOfFeet, person.NumberOfFeet);
			Assert.AreEqual(Color.Green, person.HairColor);
			Assert.AreEqual(Color.Blue, person.EyeColor);
			Assert.AreEqual(numberOfHeads, person.NumberOfHeads);
			Assert.AreEqual(numberOfFingers, person.NumberOfFingers);
		}

		[Test]
		public void CreateAdapter_WithSubstitionOnProperty_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.First_Name = "Craig";
			Assert.AreEqual("Craig", dictionary["First Name"]);
		}

		[Test]
		public void CreateAdapter_WithSubstitionOnInterface_WorksFine()
		{
			var person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);
			person.Max_Width = 22;
			Assert.AreEqual(22, dictionary["Max Width"]);
		}

		[Test]
		public void CreateAdapter_WithComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var mailing = person.HomeAddress;
			Assert.IsNotNull(mailing);
		}

		[Test]
		public void ReadAdapter_WithComponent_WorksFine()
		{
			dictionary["HomeAddress_Line1"] = "77 Lynwood Dr";
			dictionary["HomeAddress_City"] = "Massapequa";
			dictionary["HomeAddress_State"] = "NY";
			dictionary["HomeAddress_ZipCode"] = "11288";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var home = person.HomeAddress;

			Assert.AreEqual(dictionary["HomeAddress_Line1"], home.Line1);
			Assert.AreEqual(dictionary["HomeAddress_City"], home.City);
			Assert.AreEqual(dictionary["HomeAddress_State"], home.State);
			Assert.AreEqual(dictionary["HomeAddress_ZipCode"], home.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var home = person.HomeAddress;
			home.Line1 = "77 Lynwood Dr";
			home.City = "Massapequa";
			home.State = "NY";
			home.ZipCode = "11288";

			Assert.AreEqual("77 Lynwood Dr", home.Line1);
			Assert.AreEqual("Massapequa", home.City);
			Assert.AreEqual("NY", home.State);
			Assert.AreEqual("11288", home.ZipCode);
		}

		[Test]
		public void ReadAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Line1"] = "139 Dartbrook";
			dictionary["City"] = "Plano";
			dictionary["State"] = "TX";
			dictionary["ZipCode"] = "75062";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var work = person.WorkAddress;

			Assert.AreEqual(dictionary["Line1"], work.Line1);
			Assert.AreEqual(dictionary["City"], work.City);
			Assert.AreEqual(dictionary["State"], work.State);
			Assert.AreEqual(dictionary["ZipCode"], work.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var work = person.WorkAddress;
			work.Line1 = "139 Dartbrook";
			work.City = "Plano";
			work.State = "TX";
			work.ZipCode = "75062";

			Assert.AreEqual("139 Dartbrook", work.Line1);
			Assert.AreEqual("Plano", work.City);
			Assert.AreEqual("TX", work.State);
			Assert.AreEqual("75062", work.ZipCode);
		}

		[Test]
		public void ReadAdapter_WithComponentOverridePrefix_WorksFine()
		{
			dictionary["Billing_Line1"] = "64 Country Rd";
			dictionary["Billing_City"] = "Miami";
			dictionary["Billing_State"] = "FL";
			dictionary["Billing_ZipCode"] = "33101";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var billing = person.BillingAddress;

			Assert.AreEqual(dictionary["Billing_Line1"], billing.Line1);
			Assert.AreEqual(dictionary["Billing_City"], billing.City);
			Assert.AreEqual(dictionary["Billing_State"], billing.State);
			Assert.AreEqual(dictionary["Billing_ZipCode"], billing.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponentOverridePrefix_WorksFine()
		{
			dictionary["Billing_Line1"] = "64 Country Rd";
			dictionary["Billing_City"] = "Miami";
			dictionary["Billing_State"] = "FL";
			dictionary["Billing_ZipCode"] = "33101";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var billing = person.BillingAddress;
			billing.Line1 = "64 Country Rd";
			billing.City = "Miami";
			billing.State = "FL";
			billing.ZipCode = "33101";
		}

		[Test]
		public void CreateAdapter_WithNestedComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var mailing = person.HomeAddress;
			Assert.IsNotNull(mailing.Phone);
		}

		[Test]
		public void ReadAdapter_WithNestedComponent_WorksFine()
		{
			dictionary["HomeAddress_Phone_Number"] = "212-353-1244";
			dictionary["HomeAddress_Phone_Extension"] = "245";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Phone;

			Assert.AreEqual(dictionary["HomeAddress_Phone_Number"], phone.Number);
			Assert.AreEqual(dictionary["HomeAddress_Phone_Extension"], phone.Extension);
		}

		[Test]
		public void UpdateAdapter_WithNestedComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Phone;
			phone.Number = "212-353-1244";
			phone.Extension = "245";

			Assert.AreEqual("212-353-1244", phone.Number);
			Assert.AreEqual("245", phone.Extension);
		}

		[Test]
		public void ReadAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Number"] = "972-324-9821";
			dictionary["Extension"] = "300";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.WorkAddress.Mobile;

			Assert.AreEqual(dictionary["Number"], phone.Number);
			Assert.AreEqual(dictionary["Extension"], phone.Extension);
		}

		[Test]
		public void UpdateAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Mobile;
			phone.Number = "972-324-9821";
			phone.Extension = "300";

			Assert.AreEqual("972-324-9821", phone.Number);
			Assert.AreEqual("300", phone.Extension);
		}

		[Test]
		public void ReadAdapter_WithNestedComponentOverridePrefix_WorksFine()
		{
			dictionary["HomeAddress_Emr_Number"] = "911";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Emergency;

			Assert.AreEqual(dictionary["HomeAddress_Emr_Number"], phone.Number);
			Assert.IsNull(dictionary["HomeAddress_Emr_Extension"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultConversions_WorksFine()
		{
			var now = DateTime.Now;
			var guid = Guid.NewGuid();

			dictionary["Int"] = string.Format("{0}", 22);
			dictionary["Float"] = string.Format("{0}", 98.6);
			dictionary["Double"] = string.Format("{0}", 3.14D);
			dictionary["Decimal"] = string.Format("{0}", 100M);
			dictionary["String"] = "Hello World";
			dictionary["DateTime"] = now.ToShortDateString();
			dictionary["Guid"] = guid.ToString();

			var conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.AreEqual(22, conversions.Int);
			Assert.AreEqual(98.6f, conversions.Float);
			Assert.AreEqual(3.14, conversions.Double);
			Assert.AreEqual(100, conversions.Decimal);
			Assert.AreEqual("Hello World", conversions.String);
			Assert.AreEqual(now.Date, conversions.DateTime.Date);
			Assert.AreEqual(guid, conversions.Guid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultConversions_WorksFine()
		{
			var today = DateTime.Today;
			var guid = Guid.NewGuid();

			var conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.Int = 22;
			conversions.Float = 98.6F;
			conversions.Double = 3.14;
			conversions.Decimal = 100;
			conversions.DateTime = today;
			conversions.Guid = guid;
			conversions.Phone = new Phone("2124751012", "22");

			Assert.AreEqual(string.Format("{0}", 22), dictionary["Int"]);
			Assert.AreEqual(string.Format("{0}", 98.6), dictionary["Float"]);
			Assert.AreEqual(string.Format("{0}", 3.14D), dictionary["Double"]);
			Assert.AreEqual(string.Format("{0}", 100M), dictionary["Decimal"]);
			Assert.AreEqual(today.ToShortDateString(), dictionary["DateTime"]);
			Assert.AreEqual(guid.ToString(), dictionary["Guid"]);
			Assert.AreEqual("2124751012,22", dictionary["Phone"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultNullableConversions_WorksFine()
		{
			DateTime? now = DateTime.Now;
			Guid? guid = Guid.NewGuid();

			dictionary["NullInt"] = string.Format("{0}", 22);
			dictionary["NullFloat"] = string.Format("{0}", 98.6);
			dictionary["NullDouble"] = string.Format("{0}", 3.14D);
			dictionary["NullDecimal"] = string.Format("{0}", 100M);
			dictionary["NullDateTime"] = now.Value.ToShortDateString();
			dictionary["NullGuid"] = guid.ToString();

			var conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.AreEqual(22, conversions.NullInt);
			Assert.AreEqual(98.6f, conversions.NullFloat);
			Assert.AreEqual(3.14, conversions.NullDouble);
			Assert.AreEqual(100, conversions.NullDecimal);
			Assert.AreEqual(now.Value.Date, conversions.NullDateTime.Value.Date);
			Assert.AreEqual(guid, conversions.NullGuid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultNullableConversions_WorksFine()
		{
			DateTime? today = DateTime.Today;
			Guid? guid = Guid.NewGuid();

			var conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.NullInt = 22;
			conversions.NullFloat = 98.6F;
			conversions.NullDouble = 3.14;
			conversions.NullDecimal = 100;
			conversions.NullDateTime = today;
			conversions.NullGuid = guid;

			Assert.AreEqual(string.Format("{0}", 22), dictionary["NullInt"]);
			Assert.AreEqual(string.Format("{0}", 98.6), dictionary["NullFloat"]);
			Assert.AreEqual(string.Format("{0}", 3.14D), dictionary["NullDouble"]);
			Assert.AreEqual(string.Format("{0}", 100M), dictionary["NullDecimal"]);
			Assert.AreEqual(today.Value.ToShortDateString(), dictionary["NullDateTime"]);
			Assert.AreEqual(guid.ToString(), dictionary["NullGuid"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultNullConversions_WorksFine()
		{
			var conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.IsNull(conversions.NullInt);
			Assert.IsNull(conversions.NullFloat);
			Assert.IsNull(conversions.NullDouble);
			Assert.IsNull(conversions.NullDecimal);
			Assert.IsNull(conversions.NullDateTime);
			Assert.IsNull(conversions.NullGuid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultNullConversions_WorksFine()
		{
			var conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.NullInt = null;
			conversions.NullFloat = null;
			conversions.NullDecimal = null;
			conversions.NullDateTime = null;
			conversions.NullGuid = null;

			Assert.IsNull(dictionary["NullInt"]);
			Assert.IsNull(dictionary["NullFloat"]);
			Assert.IsNull(dictionary["NullDouble"]);
			Assert.IsNull(dictionary["NullDecimal"]);
			Assert.IsNull(dictionary["NullDateTime"]);
			Assert.IsNull(dictionary["NullGuid"]);
		}

		[Test]
		public void ReadAdapter_WithStringLists_WorksFine()
		{
			dictionary["Names"] = "Craig,Brenda,Kaitlyn,Lauren,Matthew";
			dictionary["Ages"] = "37,36,5,3,1";

			var lists = factory.GetAdapter<IStringLists>(dictionary);

			var names = lists.Names;
			Assert.IsNotNull(names);
			Assert.AreEqual(5, names.Count);
			Assert.AreEqual("Craig", names[0]);
			Assert.AreEqual("Brenda", names[1]);
			Assert.AreEqual("Kaitlyn", names[2]);
			Assert.AreEqual("Lauren", names[3]);
			Assert.AreEqual("Matthew", names[4]);

			var ages = lists.Ages;
			Assert.AreEqual(5, ages.Count);
			Assert.AreEqual(37, ages[0]);
			Assert.AreEqual(36, ages[1]);
			Assert.AreEqual(5, ages[2]);
			Assert.AreEqual(3, ages[3]);
			Assert.AreEqual(1, ages[4]);
		}

		[Test]
		public void UpdateAdapter_WithStringLists_WorksFine()
		{
			var lists = factory.GetAdapter<IStringLists>(dictionary);

			var names = lists.Names;
			names.Add("Craig");
			names.Add("Brenda");
			names.Add("Kaitlyn");
			names.Add("Lauren");
			names.Add("Matthew");

			Assert.AreEqual("Craig,Brenda,Kaitlyn,Lauren,Matthew", dictionary["Names"]);

			var ages = new List<int>();
			ages.Add(37);
			ages.Add(36);
			ages.Add(5);
			ages.Add(3);
			ages.Add(1);
			lists.Ages = ages;

			Assert.AreEqual("37,36,5,3,1", dictionary["Ages"]);
		}

		#region Safe type names

		[Test]
		public void GetSafeTypeName_NonGenericType_ReturnsTypeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeName(typeof(Version));
			Assert.AreEqual(typeof(Version).Name, name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeName_GenericType_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeName(typeof(List<int>));
			Assert.AreEqual("List_1_System_Int32", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeName_OpenGenericType_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeName(typeof(List<>));
			Assert.AreEqual("List_1", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeName_GenericTypeWithMultipleParameters_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeName(typeof(Dictionary<int, string>));
			Assert.AreEqual("Dictionary_2_System_Int32_System_String", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeName_OpenGenericTypeWithMultipleParameters_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeName(typeof(Dictionary<,>));
			Assert.AreEqual("Dictionary_2", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeFullName_NonGenericType_ReturnsFullName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeFullName(typeof(Version));
			Assert.AreEqual(typeof(Version).FullName, name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeFullName_GenericType_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeFullName(typeof(List<int>));
			Assert.AreEqual("System.Collections.Generic.List_1_System_Int32", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeFullName_OpenGenericType_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeFullName(typeof(List<>));
			Assert.AreEqual("System.Collections.Generic.List_1", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeFullName_GenericTypeWithMultipleParameters_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeFullName(typeof(Dictionary<int, string>));
			Assert.AreEqual("System.Collections.Generic.Dictionary_2_System_Int32_System_String", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		[Test]
		public void GetSafeTypeFullName_OpenGenericTypeWithMultipleParameters_ReturnsSafeName()
		{
			string name = DictionaryAdapterFactory.GetSafeTypeFullName(typeof(Dictionary<,>));
			Assert.AreEqual("System.Collections.Generic.Dictionary_2", name);
			AssemblyName asmName = new AssemblyName(name);
			Assert.AreEqual(name, asmName.Name);
		}

		#endregion

		[Test]
		public void CanObtainDictionaryAdapterMeta()
		{
			var meta = factory.GetAdapter<IPerson>(dictionary) as IDictionaryAdapter;
			Assert.AreSame(dictionary, meta.Dictionary);
			Assert.AreSame(factory, meta.Factory);
			Assert.AreEqual(8, meta.Properties.Count);
		}

		[Test]
		public void DictionaryAdapterMetaIsExplicitImplementation()
		{
			var i = factory.GetAdapter<IEnsureMetaDoesNotConflict>(dictionary);

			i.Dictionary = "Hello";
			i.Properties = 1;

			Assert.AreEqual("Hello", dictionary["Dictionary"]);
			Assert.AreEqual(1, dictionary["Properties"]);

			var meta = i as IDictionaryAdapter;
			Assert.AreSame(dictionary, meta.Dictionary);
			Assert.AreEqual(3, meta.Properties.Count);
		}

		[Test]
		public void CanFetchPropertiesUsingDictionaryAdapterMeta()
		{
			var getter = new CustomGetter();
			var custom = new DictionaryDescriptor().AddGetter(getter);
			var meta = factory.GetAdapter(typeof(IPhone), dictionary, custom) as IDictionaryAdapter;

			Assert.AreEqual(0, getter.PropertiesFetched.Count);

			meta.FetchProperties();

			Assert.AreEqual(2, getter.PropertiesFetched.Count);
			Assert.IsTrue(getter.PropertiesFetched.Contains("Number"));
			Assert.IsTrue(getter.PropertiesFetched.Contains("Extension"));
		}

		[Test]
		public void CanUpgradePropertiesFromReadonlyToReadWrite()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			Assert.IsNotNull(name);

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.AreEqual("Big", name.FirstName);
		}

		[Test]
		public void CanRequestFormattedReadonlyProperties()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			Assert.IsNotNull(name);

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.AreEqual("Big Tex", name.FullName);
		}

		[Test]
		public void WillRaisePropertyChangedEventWhenPropertyChanged()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) => 
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};

			person.Name = "Craig";
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void CanObtainPropertyChangeDetailsWhenPropertyChanged()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				var details = e as PropertyModifiedEventArgs;
				Assert.AreEqual(null, details.OldPropertyValue);
				Assert.AreEqual("Craig", details.NewPropertyValue);
			};

			person.Name = "Craig";
		}

		[Test]
		public void CanCancelPropertyChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanging += (s, e) =>
			{
				e.Cancel = true;
			};

			person.Name = "Craig";
			Assert.AreEqual(null, person.Name);
		}

		[Test]
		public void WillRaisePropertyChangedEventWhenNestedPropertyChanged()
		{
			var notifyCalled = false;
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			container.Item.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};

			container.Item.Name = "Craig";
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void WillPropagatePropertyChangedEventWhenNestedPropertyChanged()
		{
			var notifyCalled = false;
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			container.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreSame(container.Item, s);
				Assert.IsInstanceOf<IPerson>(s);
				Assert.AreEqual("Name", e.PropertyName);
			};

			container.Item.Name = "Craig";
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void WillPropagatePropertyChangedEventWhenBindingListPropertyChanged()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			var person = container.Bindingtems.AddNew();
			person.Name = "Fred Flinstone";
		}

		[Test]
		public void WillStopProgagtingPropertyChangedEventWhenNestedPropertyRemoved()
		{
			var notifyCalled = false;
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			var item = container.Item;

			container.PropertyChanged += (s, e) =>
			{
				notifyCalled = (item == s);
			};

			container.Item = null;

			item.Name = "Craig";
			Assert.IsFalse(notifyCalled);
		}

		[Test]
		public void CanSupressAllPropertyChangedEvents()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};

			using (person.SupressNotificationsSection())
            {
				person.Name = "Craig";            	
            }
			Assert.IsFalse(notifyCalled);
		}

		[Test]
		public void CanResumeAllPropertyChangedEvents()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};

			using (person.SupressNotificationsSection())
			{
				person.Name = "Craig";
			}

			Assert.IsFalse(notifyCalled);
			person.Name = "Fred";
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void CanSupressPropertyChangedEventsForSingleProperty()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};

			person.Max_Width = 10;
			Assert.IsFalse(notifyCalled);
		}

		[Test]
		public void CanEditPropertiesAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.AreEqual("Craig", person.Name);
			person.EndEdit();
			Assert.AreEqual("Craig", person.Name);
		}

		[Test]
		public void CanEditNestedPropertiesAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.EndEdit();
			Assert.AreEqual("600 Tulip Ln.", person.BillingAddress.Line1);
		}

		[Test]
		public void CanEditToplevelAndNestedPropertiesAndAcceptChanges()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			var person = container.Item;
			person.BeginEdit();
			person.Name = "Humpty";
			person.Age = 22;
			container.Phone.Number = "1234";
			container.Phone.Extension = "99";
			person.EndEdit();
			Assert.AreEqual("Humpty", person.Name);
			Assert.AreEqual("1234", container.Phone.Number);
		}

		[Test]
		public void CanEditCollectionPropertiesAndAcceptChanges()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.GenericItems.Add(container.Create<IPerson>());
			container.BeginEdit();
			container.GenericItems.Add(container.Create<IPerson>());
			container.EndEdit();
			Assert.AreEqual(2, container.GenericItems.Count);
		}

		[Test]
		public void CanPerformMultiLevelEditAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.AreEqual("Craig", person.Name);
			person.BeginEdit();
			person.Age = 39;
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(39, person.Age);
			person.EndEdit();
			person.EndEdit();
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(39, person.Age);
		}

		[Test]
		public void WillRaisePropertyChangedEventWhenEditsAreAccepted()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};
			person.BeginEdit();
			person.Name = "Craig";
			person.EndEdit();
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void WillRaisePropertyChangedEventWhenNestedEditsAreAccepted()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BillingAddress.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Line1", e.PropertyName);
			};
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.EndEdit();
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void CanEditPropertiesAndCancelChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.AreEqual("Craig", person.Name);
			person.CancelEdit();
			Assert.IsNull(person.Name);
		}
		
		[Test]
		public void CanEditNestedPropertiesAndCancelChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.CancelEdit();
			Assert.AreEqual("77 Nutmeg Dr.", person.BillingAddress.Line1);
		}

		[Test]
		public void CanEditCollectionPropertiesAndCancelChanges()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.GenericItems.Add(container.Create<IPerson>());
			container.BeginEdit();
			container.GenericItems.Add(container.Create<IPerson>());
			container.CancelEdit();
			Assert.AreEqual(1, container.GenericItems.Count);
		}

		[Test]
		public void CanPerformMultiLevelEditAndCancelAllChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Spyker";
			person.Age = 21;
			person.BeginEdit();
			person.Name = "Craig";
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(21, person.Age);
			person.BeginEdit();
			person.Age = 39;
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(39, person.Age);
			person.CancelEdit();
			person.CancelEdit();
			Assert.AreEqual("Spyker", person.Name);
			Assert.AreEqual(21, person.Age);
		}

		[Test]
		public void CanPerformMultiLevelEditAndCancelInnerChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Spyker";
			person.Age = 21;
			person.BeginEdit();
			person.Name = "Craig";
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(21, person.Age);
			person.BeginEdit();
			person.Age = 39;
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(39, person.Age);
			person.CancelEdit();
			person.EndEdit();
			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(21, person.Age);
		}

		[Test]
		public void WillNotRaisePropertyChangedEventWhenEditsAreCancelled()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Name", e.PropertyName);
			};
			person.BeginEdit();
			person.Name = "Craig";
			person.CancelEdit();
			Assert.IsFalse(notifyCalled);
		}

		[Test]
		public void WillNotRaisePropertyChangedEventWhenNestedEditsAreAccepted()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BillingAddress.PropertyChanged += (s, e) =>
			{
				notifyCalled = true;
				Assert.AreEqual("Line1", e.PropertyName);
			};
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.CancelEdit();
			Assert.IsFalse(notifyCalled);
		}

		[Test]
		public void WillRaisePropertyChangedEventsForReadonlyProperties()
		{
			int notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
					++notifications;
			};

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.AreEqual("Big Tex", name.FullName);
			Assert.AreEqual(2, notifications);
		}

		[Test]
		public void WillRaiseOnePropertyChangedEventsPerReadonlyPropertyWhenEditing()
		{
			int notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
					++notifications;
			};

			name.BeginEdit();
			name.FirstName = "Big";
			name.LastName = "Tex";
			name.EndEdit();

			Assert.AreEqual("Big Tex", name.FullName);
			Assert.AreEqual(1, notifications);
		}

		[Test]
		public void WillNotRaisePropertyChangedEventsForReadonlyPropertyWhenCancelEditing()
		{
			int notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
					++notifications;
			};

			name.BeginEdit();
			name.FirstName = "Big";
			name.LastName = "Tex";
			name.CancelEdit();

			Assert.AreEqual("", name.FullName);
			Assert.AreEqual(0, notifications);
		}

		[Test]
		public void CanInitializeTheDictionaryAdapterWithAttributes()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			var validator = ((IDictionaryAdapter)name).Validator as TestDictionaryValidator;
			Assert.IsNotNull(validator);
		}

		[Test]
		public void CanValidateAndObtainDataErrorInformation()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.FirstName = "Big";
			name.LastName = "Tex";

			Assert.AreEqual("Property FirstName must be at least 10 characters long", name.Error);
		}

		[Test]
		public void CanCreateDictionaryAdapterFromExistingAdapter()
		{
			var name = factory.GetAdapter<IName>(dictionary);
			var person = name.Create<IPerson>();
			Assert.NotNull(person);
			person.Name = "Chuck Norris";
			Assert.AreEqual("Chuck Norris", person.Name);
		}

		[Test]
		public void CanCreateAndInitializeDictionaryAdapterFromExistingAdapter()
		{
			var name = factory.GetAdapter<IName>(dictionary);
			var person = name.Create<IPerson>(p => p.Name = "Chuck Norris");
			Assert.NotNull(person);
			Assert.AreEqual("Chuck Norris", person.Name);
		}

		[Test]
		public void CanGetSimplePropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.AreEqual(5, container.Count);
		}
		
		[Test]
		public void CanGetGuidPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.True(container.Id != new Guid());
		}

		[Test]
		public void CanGetrClassPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container.Address);
		}

		[Test]
		public void CanGetrArrayPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container.Positions);
		}

		[Test]
		public void CanGetGenericCollectionPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container.GenericItems);
		}

		[Test]
		public void CanGetCollectionPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container.Items);
		}

		[Test]
		public void CanGetInterfacePropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.IsNotNull(container.Phone);
		}

		[Test]
		public void CanAddBindingListItemsOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			var person = container.Bindingtems.AddNew();
			Assert.IsNotNull(person);
		}

		[Test]
		public void CanUseDynamicValues()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.Positions = new[] { 2, 4, 6, 8 };
			container.ReducePositions = new DynamicValueDelegate<int>(() => container.Positions.Sum());
			Assert.AreEqual(20, container.ReducePositions.Value);

			container.Positions = new[] { 1, 2, 3, 4 };
			Assert.AreEqual(10, container.ReducePositions.Value);
		}

		[Test]
		public void WillGetNotificedWhenDynamicValueChanges()
		{
			bool notifyCalled = false;
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.ReducePositions = new DynamicValueDelegate<int>(() => container.Positions.Sum());

			container.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "ReducePositions")
				{
					notifyCalled = true;
					Assert.AreEqual(10, ((PropertyModifiedEventArgs)e).NewPropertyValue);
				}
			};

			container.Positions = new[] { 1, 2, 3, 4 };
			Assert.AreEqual(10, container.ReducePositions.Value);
			Assert.IsTrue(notifyCalled);
		}

		[Test]
		public void CanDetermineTheAdaptedInterface()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var type = person.GetType().GetCustomAttributes(
				typeof(DictionaryAdapterAttribute), false).Cast<DictionaryAdapterAttribute>()
				.FirstOrDefault();
			Assert.IsNotNull(type);
			Assert.AreEqual(typeof(IPerson), type.InterfaceType);
		}
	}

	[TestDictionaryValidator]
	public interface IName : IDictionaryNotify, IDictionaryCreate, IDataErrorInfo
	{
		string FirstName { get; }
		string LastName { get; }

		[StringFormat("{0} {1}", "FirstName,LastName")]
		string FullName { get; }
	}

	public enum Color { Red, Green, Blue };

	public interface IMutableName : IName, IEditableObject
	{
		[ValidateStringLengthAtLeast(10)]
		new string FirstName { get; set; }
		new string LastName { get; set; }
	}

	public interface IPhone : IEditableObject, INotifyPropertyChanged, IDataErrorInfo
	{
		string Number { get; set; }
		string Extension { get; set; }
	}

	#region InfrastructureStub

	public abstract class InfrastructureStub : INotifyPropertyChanged, IEditableObject, IDataErrorInfo
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void BeginEdit()
		{
		}

		public void CancelEdit()
		{
		}

		public void EndEdit()
		{
			if (PropertyChanged != null)
			{
			}
		}

		public string Error
		{
			get { return String.Empty; }
		}

		public string this[string columnName]
		{
			get { return String.Empty; }
		}
	}

	#endregion

	public class Phone : InfrastructureStub, IPhone
	{
		private string number;
		private string extension;

		public Phone()
		{
		}

		public Phone(string number, string extension)
		{
			this.number = number;
			this.extension = extension;
		}

		public string Extension
		{
			get { return extension; }
			set { extension = value; }
		}

		public string Number
		{
			get { return number; }
			set { number = value; }
		}
	}

	[PropagateNotifications(false)]
	public interface IAddress : IEditableObject, INotifyPropertyChanged, IDataErrorInfo
	{
		string Line1 { get; set; }
		string Line2 { get; set; }
		string City { get; set; }
		string State { get; set; }
		string ZipCode { get; set; }

		[DictionaryComponent]
		IPhone Phone { get; }

		[DictionaryComponent(NoPrefix = true)]
		IPhone Mobile { get; }

		[DictionaryComponent(Prefix = "Emr_")]
		IPhone Emergency { get; }
	}

	public class Address : InfrastructureStub,  IAddress
	{
		private string line1;
		private string line2;
		private string city;
		private string state;
		private string zipCode;
		private IPhone phone;
		private IPhone mobile;
		private IPhone emergency;

		public string Line1
		{
			get { return line1; }
			set { line1 = value; }
		}

		public string Line2
		{
			get { return line2; }
			set { line2 = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string State
		{
			get { return state; }
			set { state = value; }
		}

		public string ZipCode
		{
			get { return zipCode; }
			set { zipCode = value; }
		}

		public IPhone Phone
		{
			get
			{
				if (phone == null)
				{
					phone = new Phone();
				}
				return phone;
			}
		}

		public IPhone Mobile
		{
			get
			{
				if (mobile == null)
				{
					mobile = new Phone();
				}
				return mobile;
			}
		}

		public IPhone Emergency
		{
			get
			{
				if (emergency == null)
				{
					emergency = new Phone();
				}
				return emergency;
			}
		}
	}

	[MultiLevelEdit]
	public interface IPerson : IEditableObject, IDictionaryNotify, IDataErrorInfo
	{
		string Name { get; set; }

		int Age { get; set; }

		DateTime DOB { get; set; }

		IList<IPerson> Friends { get; set; }

		[DictionaryKeySubstitution("_", " ")]
		string First_Name { get; set; }

		[DictionaryComponent]
		IAddress HomeAddress { get; set; }

		[DictionaryComponent(NoPrefix = true)]
		IAddress WorkAddress { get; set; }

		[DictionaryComponent(Prefix = "Billing_")]
		IAddress BillingAddress { get; set; }
	}

	[OnDemand]
	public interface IItemContainer<TItem> : IDictionaryCreate, IEditableObject, INotifyPropertyChanged
	{
		Guid Id { get; set; }

		TItem Item { get; set; }

		[OnDemand(5)]
		int Count { get; set; }

		Address Address { get; set; }

		IPhone Phone { get; set; }

		int[] Positions { get; set; }

		IDynamicValue<int> ReducePositions { get; set; }

		IList<TItem> GenericItems { get; set; }

		BindingList<TItem> Bindingtems { get; set; }

		IList Items { get; set; }
	}

	public interface IItemContainerWithComponent<TItem> : IDictionaryNotify
	{
		[DictionaryComponent]
		TItem Item { get; set; }
	}

	public interface IPersonWithoutPrefix : IPerson
	{
		int NumberOfFeet { get; set; }
	}

	[DictionaryKeyPrefix("Person_")]
	public interface IPersonWithPrefix : IPersonWithoutPrefix
	{
		Color HairColor { get; set; }
	}

	[DictionaryKeyPrefix("Person2_")]
	public interface IPersonWithPrefixOverride : IPersonWithPrefix
	{
		[DictionaryKey("Eye__Color")]
		Color EyeColor { get; set; }
	}

	[DictionaryTypeKeyPrefix]
	public interface IPersonWithTypePrefixOverride : IPersonWithPrefixOverride
	{
		int Height { get; set; }
	}

	public interface IPersonWithPrefixOverrideFurtherOverride : IPersonWithPrefixOverride
	{
		[DefaultValue(1)]
		int NumberOfHeads { get; set; }
	}

	[DictionaryKeyPrefix("")]
	[DictionaryKeySubstitution("_", " ")]
	public interface IPersonWithDeniedInheritancePrefix : IPersonWithPrefixOverrideFurtherOverride
	{
		int NumberOfFingers { get; set; }

		[SuppressNotifications]
		int Max_Width { get; set; }
	}

	public interface IPersonWithMethod : IPerson
	{
		void Run();
	}

	public interface IConversions
	{
		int Int { get; set; }
		float Float { get; set; }
		double Double { get; set; }
		decimal Decimal { get; set; }
		String String { get; set; }
		DateTime DateTime { get; set; }
		Guid Guid { get; set; }
		int? NullInt { get; set; }
		float? NullFloat { get; set; }
		double? NullDouble { get; set; }
		DateTime? NullDateTime { get; set; }
		Guid? NullGuid { get; set; }
		decimal? NullDecimal { get; set; }
	}

	[StringValues]
	public interface IConversionsToString : IConversions
	{
		[TypeConverter(typeof(PhoneConverter))]
		Phone Phone { get; set; }
	}

	public interface IStringLists
	{
		[StringList]
		IList<string> Names { get; }

		[StringList]
		IList<int> Ages { get; set; }
	}

	public class PhoneConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,
											Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context,
										   CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] fields = ((string)value).Split(new char[] { ',' });
				return new Phone(fields[0], fields[1]);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context,
										 CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((Phone)value).Number + "," + ((Phone)value).Extension;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	public interface IEnsureMetaDoesNotConflict
	{
		string Dictionary { get; set; }

		object Properties { get; set; }

		int FetchProperties { get; set; }
	}

	public class CustomGetter : DictionaryBehaviorAttribute, IDictionaryPropertyGetter
	{
		private List<string> propertiesFetched = new List<string>();

		public IList<string> PropertiesFetched
		{
			get { return propertiesFetched; }
		}

		public void Reset()
		{
			propertiesFetched.Clear();
		}

		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key,
									   object storedValue, PropertyDescriptor property)
		{
			propertiesFetched.Add(key);
			return storedValue;
		}
	}
}

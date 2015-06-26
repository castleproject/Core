// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;

	using CastleTests.Components.DictionaryAdapter.Tests;

	using Xunit;

#if SILVERLIGHT || NETCORE
	using Hashtable = System.Collections.Generic.Dictionary<object, object>;
#endif

#if NETCORE
    internal static class DateTimeExtensions
    {
        public static string ToShortDateString(this DateTime dt)
        {
            return dt.ToString("d");
        }
    }
#endif

	public class DictionaryAdapterFactoryTestCase
	{
		private IDictionary dictionary;
		private DictionaryAdapterFactory factory;

		public DictionaryAdapterFactoryTestCase()
		{
			dictionary = new Hashtable();
			factory = new DictionaryAdapterFactory();
		}

		[Fact]
		public void CreateAdapter_NoPrefixPropertiesOnly_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			Assert.NotNull(person);
		}

		[Fact]
		public void CreateAdapter_AdaptingGenericInterface_Works()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container);
			// create a fake person
			var person = factory.GetAdapter<IPerson>(dictionary);
			container.Item = person;

			Assert.Same(person, container.Item);
		}

		[Fact]
		public void CreateAdapter_AdaptingGenericInterfaceWithComponent_Works()
		{
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			Assert.NotNull(container);
			// create a fake person
			container.Item.Age = 5;
			container.Item.First_Name = "Andre";
			Assert.Equal(5, container.Item.Age);
			Assert.Equal("Andre", container.Item.First_Name);
		}

		[Fact(Skip = "Ignore")]
		public void CreateAdapter_NoPrefixWithMethod_ThrowsException()
		{
			Assert.Throws<TypeLoadException>(() => { factory.GetAdapter<IPersonWithMethod>(dictionary); });
		}

		[Fact]
		public void CreateAdapter_PrefixPropertiesOnly_WorksFine()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			Assert.NotNull(person);
		}

		[Fact]
		public void UpdateAdapter_NoPrefix_UpdatesDictionary()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>(); // I need some friends

			Assert.Equal("Craig", dictionary["Name"]);
			Assert.Equal(37, dictionary["Age"]);
			Assert.Equal(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.Equal(0, ((IList<IPerson>)dictionary["Friends"]).Count);
		}

		[Fact(Skip = "Known issue: DictionaryAdapterFactory.GetAdapter is failing to associate correctly with behaviors.  Appears to be due to lack of ICustomAttributeProvider")]
		public void UpdateAdapter_Prefix_UpdatesDictionary()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();
			Assert.Equal("Craig", dictionary["Name"]);
			Assert.Equal(37, dictionary["Age"]);
			Assert.Equal(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.Equal(0, ((IList<IPerson>)dictionary["Friends"]).Count);
		}

		[Fact]
		public void UpdateAdapterAndRead_NoPrefix_Matches()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.Equal("Craig", person.Name);
			Assert.Equal(37, person.Age);
			Assert.Equal(new DateTime(1970, 7, 19), person.DOB);
			Assert.Equal(0, person.Friends.Count);
		}

		[Fact]
		public void UpdateAdapterAndRead_Prefix_Matches()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.Equal("Craig", person.Name);
			Assert.Equal(37, person.Age);
			Assert.Equal(new DateTime(1970, 7, 19), person.DOB);
			Assert.Equal(0, person.Friends.Count);
		}

		[Fact(Skip = "Known issue: DictionaryAdapterFactory.GetAdapter is failing to associate correctly with behaviors.  Appears to be due to lack of ICustomAttributeProvider")]
		public void UpdateAdapterAndRead_PrefixOverride_Matches()
		{
			var person = factory.GetAdapter<IPersonWithPrefixOverride>(dictionary);
			person.Name = "Craig";

			Assert.Equal("Craig", dictionary["Name"]);
		}

		[Fact]
		public void UpdateAdapterAndRead_TypePrefix_Matches()
		{
			var person = factory.GetAdapter<IPersonWithTypePrefixOverride>(dictionary);
			person.Height = 72;

			Assert.Equal(72, person.Height);
			Assert.Equal(72, dictionary["Castle.Components.DictionaryAdapter.Tests.IPersonWithTypePrefixOverride#Height"]);
		}

		[Fact]
		public void ReadAdapter_NoPrefixUnitialized_ReturnsDefaults()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);

			Assert.Equal(default(string), person.Name);
			Assert.Equal(default(int), person.Age);
			Assert.Equal(default(DateTime), person.DOB);
			Assert.Equal(default(IList<IPerson>), person.Friends);
		}

		[Fact]
		public void ReadAdapter_PrefixUnitialized_ReturnsDefaults()
		{
			var person = factory.GetAdapter<IPersonWithPrefix>(dictionary);

			Assert.Equal(default(string), person.Name);
			Assert.Equal(default(int), person.Age);
			Assert.Equal(default(DateTime), person.DOB);
			Assert.Equal(default(IList<IPerson>), person.Friends);
		}

		[Fact(Skip = "Known issue: DictionaryAdapterFactory.GetAdapter is failing to associate correctly with behaviors.  Appears to be due to lack of ICustomAttributeProvider")]
		public void UpdateAdapterAndRead_WithSeveralDifferentOverridesWithDifferentPrefixes_DictionaryKeysHaveCorrectPrefixes()
		{
			var person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);

			const string name = "Ming The Merciless";
			const int numberOfFeet = 2;
			const int numberOfHeads = 1;
			const int numberOfFingers = 3;

			person.Name = name;
			person.NumberOfFeet = numberOfFeet;
			person.HairColor = Color.Green;
			person.EyeColor = Color.Blue;
			person.NumberOfHeads = numberOfHeads;
			person.NumberOfFingers = numberOfFingers;

			var keys = new string[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(keys, 0);

			Assert.True(keys.Any(key => key == "Name"));
			Assert.True(keys.Any(key => key == "NumberOfFeet"));
			Assert.True(keys.Any(key => key == "Person_HairColor"));
			Assert.True(keys.Any(key => key == "Person2_Eye__Color"));
			Assert.True(keys.Any(key => key == "NumberOfHeads"));
			Assert.True(keys.Any(key => key == "NumberOfFingers"));

			Assert.Equal(name, person.Name);
			Assert.Equal(numberOfFeet, person.NumberOfFeet);
			Assert.Equal(Color.Green, person.HairColor);
			Assert.Equal(Color.Blue, person.EyeColor);
			Assert.Equal(numberOfHeads, person.NumberOfHeads);
			Assert.Equal(numberOfFingers, person.NumberOfFingers);
		}

		[Fact]
		public void CreateAdapter_WithSubstitionOnProperty_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.First_Name = "Craig";
			Assert.Equal("Craig", dictionary["First Name"]);
		}

		[Fact]
		public void CreateAdapter_WithSubstitionOnInterface_WorksFine()
		{
			var person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);
			person.Max_Width = 22;
			Assert.Equal(22, dictionary["Max Width"]);
		}

		[Fact]
		public void CreateAdapter_WithComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var mailing = person.HomeAddress;
			Assert.NotNull(mailing);
		}

		[Fact]
		public void ReadAdapter_WithComponent_WorksFine()
		{
			dictionary["HomeAddress_Line1"] = "77 Lynwood Dr";
			dictionary["HomeAddress_City"] = "Massapequa";
			dictionary["HomeAddress_State"] = "NY";
			dictionary["HomeAddress_ZipCode"] = "11288";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var home = person.HomeAddress;

			Assert.Equal(dictionary["HomeAddress_Line1"], home.Line1);
			Assert.Equal(dictionary["HomeAddress_City"], home.City);
			Assert.Equal(dictionary["HomeAddress_State"], home.State);
			Assert.Equal(dictionary["HomeAddress_ZipCode"], home.ZipCode);
		}

		[Fact]
		public void UpdateAdapter_WithComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var home = person.HomeAddress;
			home.Line1 = "77 Lynwood Dr";
			home.City = "Massapequa";
			home.State = "NY";
			home.ZipCode = "11288";

			Assert.Equal("77 Lynwood Dr", home.Line1);
			Assert.Equal("Massapequa", home.City);
			Assert.Equal("NY", home.State);
			Assert.Equal("11288", home.ZipCode);
		}

		[Fact]
		public void ReadAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Line1"] = "139 Dartbrook";
			dictionary["City"] = "Plano";
			dictionary["State"] = "TX";
			dictionary["ZipCode"] = "75062";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var work = person.WorkAddress;

			Assert.Equal(dictionary["Line1"], work.Line1);
			Assert.Equal(dictionary["City"], work.City);
			Assert.Equal(dictionary["State"], work.State);
			Assert.Equal(dictionary["ZipCode"], work.ZipCode);
		}

		[Fact]
		public void UpdateAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var work = person.WorkAddress;
			work.Line1 = "139 Dartbrook";
			work.City = "Plano";
			work.State = "TX";
			work.ZipCode = "75062";

			Assert.Equal("139 Dartbrook", work.Line1);
			Assert.Equal("Plano", work.City);
			Assert.Equal("TX", work.State);
			Assert.Equal("75062", work.ZipCode);
		}

		[Fact]
		public void ReadAdapter_WithComponentOverridePrefix_WorksFine()
		{
			dictionary["Billing_Line1"] = "64 Country Rd";
			dictionary["Billing_City"] = "Miami";
			dictionary["Billing_State"] = "FL";
			dictionary["Billing_ZipCode"] = "33101";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var billing = person.BillingAddress;

			Assert.Equal(dictionary["Billing_Line1"], billing.Line1);
			Assert.Equal(dictionary["Billing_City"], billing.City);
			Assert.Equal(dictionary["Billing_State"], billing.State);
			Assert.Equal(dictionary["Billing_ZipCode"], billing.ZipCode);
		}

		[Fact]
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

		[Fact]
		public void CreateAdapter_WithNestedComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var mailing = person.HomeAddress;
			Assert.NotNull(mailing.Phone);
		}

		[Fact]
		public void ReadAdapter_WithNestedComponent_WorksFine()
		{
			dictionary["HomeAddress_Phone_Number"] = "212-353-1244";
			dictionary["HomeAddress_Phone_Extension"] = "245";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Phone;

			Assert.Equal(dictionary["HomeAddress_Phone_Number"], phone.Number);
			Assert.Equal(dictionary["HomeAddress_Phone_Extension"], phone.Extension);
		}

		[Fact]
		public void UpdateAdapter_WithNestedComponent_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Phone;
			phone.Number = "212-353-1244";
			phone.Extension = "245";

			Assert.Equal("212-353-1244", phone.Number);
			Assert.Equal("245", phone.Extension);
		}

		[Fact]
		public void ReadAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Number"] = "972-324-9821";
			dictionary["Extension"] = "300";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.WorkAddress.Mobile;

			Assert.Equal(dictionary["Number"], phone.Number);
			Assert.Equal(dictionary["Extension"], phone.Extension);
		}

		[Fact]
		public void UpdateAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Mobile;
			phone.Number = "972-324-9821";
			phone.Extension = "300";

			Assert.Equal("972-324-9821", phone.Number);
			Assert.Equal("300", phone.Extension);
		}

		[Fact]
		public void ReadAdapter_WithNestedComponentOverridePrefix_WorksFine()
		{
			dictionary["HomeAddress_Emr_Number"] = "911";

			var person = factory.GetAdapter<IPerson>(dictionary);
			var phone = person.HomeAddress.Emergency;

			Assert.Equal(dictionary["HomeAddress_Emr_Number"], phone.Number);
			Assert.Null(dictionary["HomeAddress_Emr_Extension"]);
		}

		[Fact]
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
			Assert.Equal(22, conversions.Int);
			Assert.Equal(98.6f, conversions.Float);
			Assert.Equal(3.14, conversions.Double);
			Assert.Equal(100, conversions.Decimal);
			Assert.Equal("Hello World", conversions.String);
			Assert.Equal(now.Date, conversions.DateTime.Date);
			Assert.Equal(guid, conversions.Guid);
		}

#if SILVERLIGHT || NETCORE
		[Fact(Skip = "Conversion of phone is not working. fixme")]
#else
		[Fact]
#endif
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

			Assert.Equal(string.Format("{0}", 22), dictionary["Int"]);
			Assert.Equal(string.Format("{0}", 98.6), dictionary["Float"]);
			Assert.Equal(string.Format("{0}", 3.14D), dictionary["Double"]);
			Assert.Equal(string.Format("{0}", 100M), dictionary["Decimal"]);
#if SILVERLIGHT // SL impl limitation
			Assert.AreEqual(today.ToString(), dictionary["DateTime"]);
#else
			Assert.Equal(today.ToShortDateString(), dictionary["DateTime"]);
#endif
			Assert.Equal(guid.ToString(), dictionary["Guid"]);
			Assert.Equal("2124751012,22", dictionary["Phone"]);
		}

		[Fact]
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
			Assert.Equal(22, conversions.NullInt);
			Assert.Equal(98.6f, conversions.NullFloat);
			Assert.Equal(3.14, conversions.NullDouble);
			Assert.Equal(100, conversions.NullDecimal);
			Assert.Equal(now.Value.Date, conversions.NullDateTime.Value.Date);
			Assert.Equal(guid, conversions.NullGuid);
		}

		[Fact]
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

			Assert.Equal(string.Format("{0}", 22), dictionary["NullInt"]);
			Assert.Equal(string.Format("{0}", 98.6), dictionary["NullFloat"]);
			Assert.Equal(string.Format("{0}", 3.14D), dictionary["NullDouble"]);
			Assert.Equal(string.Format("{0}", 100M), dictionary["NullDecimal"]);
#if SILVERLIGHT // SL impl limitation
			Assert.AreEqual(today.Value.ToString(), dictionary["NullDateTime"]);
#else
			Assert.Equal(today.Value.ToShortDateString(), dictionary["NullDateTime"]);
#endif
			Assert.Equal(guid.ToString(), dictionary["NullGuid"]);
		}

		[Fact]
		public void ReadAdapter_WithDefaultNullConversions_WorksFine()
		{
			var conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.Null(conversions.NullInt);
			Assert.Null(conversions.NullFloat);
			Assert.Null(conversions.NullDouble);
			Assert.Null(conversions.NullDecimal);
			Assert.Null(conversions.NullDateTime);
			Assert.Null(conversions.NullGuid);
		}

		[Fact]
		public void UpdateAdapter_WithDefaultNullConversions_WorksFine()
		{
			var conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.NullInt = null;
			conversions.NullFloat = null;
			conversions.NullDecimal = null;
			conversions.NullDateTime = null;
			conversions.NullGuid = null;

			Assert.Null(dictionary["NullInt"]);
			Assert.Null(dictionary["NullFloat"]);
			Assert.Null(dictionary["NullDouble"]);
			Assert.Null(dictionary["NullDecimal"]);
			Assert.Null(dictionary["NullDateTime"]);
			Assert.Null(dictionary["NullGuid"]);
		}

#if SILVERLIGHT
		[Fact(Skip = "String lists don't seem to work under Silverlight! - fixme")]
#else
		[Fact]
#endif
		public void ReadAdapter_WithStringLists_WorksFine()
		{
			dictionary["Names"] = "Craig,Brenda,Kaitlyn,Lauren,Matthew";
			dictionary["Ages"] = "37,36,5,3,1";

			var lists = factory.GetAdapter<IStringLists>(dictionary);

			var names = lists.Names;
			Assert.NotNull(names);
			Assert.Equal(5, names.Count);
			Assert.Equal("Craig", names[0]);
			Assert.Equal("Brenda", names[1]);
			Assert.Equal("Kaitlyn", names[2]);
			Assert.Equal("Lauren", names[3]);
			Assert.Equal("Matthew", names[4]);

			var ages = lists.Ages;
			Assert.Equal(5, ages.Count);
			Assert.Equal(37, ages[0]);
			Assert.Equal(36, ages[1]);
			Assert.Equal(5, ages[2]);
			Assert.Equal(3, ages[3]);
			Assert.Equal(1, ages[4]);
		}

#if SILVERLIGHT && !NETCORE
		[Fact(Skip = "String lists don't seem to work under Silverlight! - fixme")]
#else
		[Fact]
#endif
		public void UpdateAdapter_WithStringLists_WorksFine()
		{
			var lists = factory.GetAdapter<IStringLists>(dictionary);

			var names = lists.Names;
			names.Add("Craig");
			names.Add("Brenda");
			names.Add("Kaitlyn");
			names.Add("Lauren");
			names.Add("Matthew");

			Assert.Equal("Craig,Brenda,Kaitlyn,Lauren,Matthew", dictionary["Names"]);

			var ages = new List<int>();
			ages.Add(37);
			ages.Add(36);
			ages.Add(5);
			ages.Add(3);
			ages.Add(1);
			lists.Ages = ages;

			Assert.Equal("37,36,5,3,1", dictionary["Ages"]);
		}

		[Fact]
		public void ReadAdapter_WithCompoundKeyPrefix_WorksFine()
		{
			var meta = new Hashtable();
			meta["Id"] = Guid.NewGuid();
			dictionary["Meta"] = meta;

			var person = factory.GetAdapter<IPerson>(dictionary);

			Assert.Equal(meta["Id"], person.Id);
		}

		[Fact]
		public void CanObtainDictionaryAdapterMeta()
		{
			var adapter = factory.GetAdapter<IPerson>(dictionary) as IDictionaryAdapter;
			Assert.Same(dictionary, adapter.This.Dictionary);
			Assert.Same(factory, adapter.This.Factory);
			Assert.Equal(9, adapter.This.Properties.Count);
		}

		[Fact]
		public void DictionaryAdapterMetaIsExplicitImplementation()
		{
			var i = factory.GetAdapter<IEnsureMetaDoesNotConflict>(dictionary);

			i.Dictionary = "Hello";
			i.Properties = 1;

			Assert.Equal("Hello", dictionary["Dictionary"]);
			Assert.Equal(1, dictionary["Properties"]);

			var adapter = i as IDictionaryAdapter;
			Assert.Same(dictionary, adapter.This.Dictionary);
			Assert.Equal(3, adapter.This.Properties.Count);
		}

		[Fact]
		public void CanFetchProperties()
		{
			var getter = new CustomGetter();
			var custom = new PropertyDescriptor().AddBehaviors(getter);
			factory.GetAdapter(typeof(IPhone), dictionary, custom);

			Assert.Equal(1, getter.PropertiesFetched.Count);

			Assert.Equal(1, getter.PropertiesFetched.Count);
			Assert.True(getter.PropertiesFetched.Contains("Number"));
		}

		[Fact]
		public void CanFetchPropertiesOnType()
		{
			var getter = new CustomGetter();
			var custom = new PropertyDescriptor().AddBehaviors(getter);
			factory.GetAdapter(typeof(IPhoneWithFetch), dictionary, custom);

			Assert.Equal(2, getter.PropertiesFetched.Count);
		}

		[Fact]
		public void CanUpgradePropertiesFromReadonlyToReadWrite()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			Assert.NotNull(name);

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.Equal("Big", name.FirstName);
		}

		[Fact]
		public void CanRequestFormattedReadonlyProperties()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			Assert.NotNull(name);

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.Equal("Big Tex", name.FullName);
		}

		[Fact]
		public void WillRaisePropertyChangedEventWhenPropertyChanged()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (!notifyCalled)
				{
					notifyCalled = true;
					Assert.Equal("Name", e.PropertyName);
				}
			};

			person.Name = "Craig";
			Assert.True(notifyCalled);
		}

		[Fact]
		public void CanObtainPropertyChangeDetailsWhenPropertyChanged()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				var details = e as PropertyChangedEventArgsEx;
				if (details != null)
				{
					Assert.Equal(null, details.OldValue);
					Assert.Equal("Craig", details.NewValue);
				}
			};

			person.Name = "Craig";
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void CanCancelPropertyChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanging += (s, e) => { ((PropertyChangingEventArgsEx)e).Cancel = true; };

			person.Name = "Craig";
			Assert.Equal(null, person.Name);
		}
#endif

		[Fact]
		public void WillRaisePropertyChangedEventWhenNestedPropertyChanged()
		{
			var notifyCalled = false;
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			container.Item.PropertyChanged += (s, e) =>
			{
				if (!notifyCalled)
				{
					notifyCalled = true;
					Assert.Equal("Name", e.PropertyName);
				}
			};

			container.Item.Name = "Craig";
			Assert.True(notifyCalled);
		}

		[Fact]
		public void WillNotPropagatePropertyChangedEventWhenNestedPropertyChanged()
		{
			var container = factory.GetAdapter<IItemContainerWithComponent<IPerson>>(dictionary);
			container.PropertyChanged += (s, e) => { Assert.True(false, "Property change event was raised from wrong object."); };

			container.Item.Name = "Craig";
		}

#if !SILVERLIGHT && !NETCORE //no BindingList in Silverlight

		[Fact]
		public void WillPropagatePropertyChangedEventWhenBindingListPropertyChanged()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			var person = container.Bindingtems.AddNew();
			person.Name = "Fred Flinstone";
		}
#endif

		[Fact]
		public void CanSuppressAllPropertyChangedEvents()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (!notifyCalled)
				{
					notifyCalled = true;
					Assert.Equal("Name", e.PropertyName);
				}
			};

			using (person.SuppressNotificationsBlock())
			{
				person.Name = "Craig";
			}
			Assert.False(notifyCalled);
		}

		[Fact]
		public void CanResumeAllPropertyChangedEvents()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (!notifyCalled)
				{
					notifyCalled = true;
					Assert.Equal("Name", e.PropertyName);
				}
			};

			using (person.SuppressNotificationsBlock())
			{
				person.Name = "Craig";
			}

			Assert.False(notifyCalled);
			person.Name = "Fred";
			Assert.True(notifyCalled);
		}

		[Fact]
		public void CanSuppressPropertyChangedEventsForSingleProperty()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (!notifyCalled)
				{
					notifyCalled = true;
					Assert.Equal("Name", e.PropertyName);
				}
			};

			person.Max_Width = 10;
			Assert.False(notifyCalled);
		}

		[Fact]
		public void CanEditPropertiesAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.Equal("Craig", person.Name);
			person.EndEdit();
			Assert.Equal("Craig", person.Name);
		}

		[Fact]
		public void CanEditNestedPropertiesAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.EndEdit();
			Assert.Equal("600 Tulip Ln.", person.BillingAddress.Line1);
		}

		[Fact]
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
			Assert.Equal("Humpty", person.Name);
			Assert.Equal("1234", container.Phone.Number);
		}

		[Fact]
		public void CanEditCollectionPropertiesAndAcceptChanges()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.GenericItems.Add(container.Create<IPerson>());
			container.BeginEdit();
			container.GenericItems.Add(container.Create<IPerson>());
			container.EndEdit();
			Assert.Equal(2, container.GenericItems.Count);
		}

		[Fact]
		public void CanPerformMultiLevelEditAndAcceptChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.Equal("Craig", person.Name);
			person.BeginEdit();
			person.Age = 39;
			Assert.Equal("Craig", person.Name);
			Assert.Equal(39, person.Age);
			person.EndEdit();
			person.EndEdit();
			Assert.Equal("Craig", person.Name);
			Assert.Equal(39, person.Age);
		}

		[Fact]
		public void WillRaisePropertyChangedEventWhenEditsAreAccepted()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Name")
				{
					notifyCalled = true;
				}
			};
			person.BeginEdit();
			person.Name = "Craig";
			person.EndEdit();
			Assert.True(notifyCalled);
		}

		[Fact]
		public void WillRaisePropertyChangedEventWhenNestedEditsAreAccepted()
		{
			var notifyCalled = false;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BillingAddress.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Line1")
				{
					notifyCalled = true;
				}
			};
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.EndEdit();
			Assert.True(notifyCalled);
		}

		[Fact]
		public void CanEditPropertiesAndCancelChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BeginEdit();
			person.Name = "Craig";
			Assert.Equal("Craig", person.Name);
			person.CancelEdit();
			Assert.Null(person.Name);
		}

		[Fact]
		public void CanEditNestedPropertiesAndCancelChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.CancelEdit();
			Assert.Equal("77 Nutmeg Dr.", person.BillingAddress.Line1);
		}

		[Fact]
		public void CanEditCollectionPropertiesAndCancelChanges()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.GenericItems.Add(container.Create<IPerson>());
			container.BeginEdit();
			container.GenericItems.Add(container.Create<IPerson>());
			container.CancelEdit();
			Assert.Equal(1, container.GenericItems.Count);
		}

		[Fact]
		public void CanPerformMultiLevelEditAndCancelAllChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Spyker";
			person.Age = 21;
			person.BeginEdit();
			person.Name = "Craig";
			Assert.Equal("Craig", person.Name);
			Assert.Equal(21, person.Age);
			person.BeginEdit();
			person.Age = 39;
			Assert.Equal("Craig", person.Name);
			Assert.Equal(39, person.Age);
			person.CancelEdit();
			person.CancelEdit();
			Assert.Equal("Spyker", person.Name);
			Assert.Equal(21, person.Age);
		}

		[Fact]
		public void CanPerformMultiLevelEditAndCancelInnerChanges()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Spyker";
			person.Age = 21;
			person.BeginEdit();
			person.Name = "Craig";
			Assert.Equal("Craig", person.Name);
			Assert.Equal(21, person.Age);
			person.BeginEdit();
			person.Age = 39;
			Assert.Equal("Craig", person.Name);
			Assert.Equal(39, person.Age);
			person.CancelEdit();
			person.EndEdit();
			Assert.Equal("Craig", person.Name);
			Assert.Equal(21, person.Age);
		}

		[Fact]
		public void WillNotRaisePropertyChangedEventWhenEditsAreCancelled()
		{
			var notifyCalled = 0;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Name")
				{
					notifyCalled++;
				}
			};
			person.BeginEdit();
			person.Name = "Craig";
			person.CancelEdit();
			Assert.Equal(2, notifyCalled);
		}

		[Fact]
		public void WillRaisePropertyChangedEventWhenNestedEditsAreCancelled()
		{
			var notifyCalled = 0;
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.BillingAddress.Line1 = "77 Nutmeg Dr.";
			person.BillingAddress.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Line1")
				{
					notifyCalled++;
				}
			};
			person.BeginEdit();
			person.BillingAddress.Line1 = "600 Tulip Ln.";
			person.CancelEdit();
			Assert.Equal(2, notifyCalled);
		}

		[Fact]
		public void WillRaisePropertyChangedEventsForReadonlyProperties()
		{
			var notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
				{
					++notifications;
				}
			};

			name.FirstName = "Big";
			name.LastName = "Tex";
			Assert.Equal("Big Tex", name.FullName);
			Assert.Equal(2, notifications);
		}

		[Fact]
		public void WillRaisePropertyChangedEventsForReadonlyPropertyWhenEditing()
		{
			var notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
				{
					++notifications;
				}
			};

			name.BeginEdit();
			name.FirstName = "Big";
			name.LastName = "Tex";
			name.EndEdit();

			Assert.Equal("Big Tex", name.FullName);
			Assert.Equal(2, notifications);
		}

		[Fact]
		public void WillRaisePropertyChangedEventsForReadonlyPropertyWhenCancelEditing()
		{
			var notifications = 0;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "FullName")
				{
					++notifications;
				}
			};

			name.BeginEdit();
			name.FirstName = "Big";
			name.LastName = "Tex";
			name.CancelEdit();

			Assert.Equal("", name.FullName);
			Assert.Equal(3, notifications);
		}

#if !NETCORE
		[Fact]
		public void CanInitializeTheDictionaryAdapterWithAttributes()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			Assert.True(((IDictionaryAdapter)name).Validators.OfType<TestDictionaryValidator>().Any());
		}

		[Fact]
		public void CanValidateAndObtainDataErrorInformation()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.FirstName = "Big";
			name.LastName = "Tex";

			Assert.False(name.IsValid);
			Assert.Equal("Property FirstName must be at least 10 characters long" + Environment.NewLine +
			             "Property LastName must be at least 15 characters long", name.Error);
		}
#endif

		[Fact(Skip = "Lack of ICustomAttributeProvider breaks the ability add arbitrary types as Attributes")]
		public void CanValidateGroupAndObtainDataErrorInformation()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.FirstName = "Big";
			name.LastName = "Tex";

			var groupA = name.ValidateGroups("A");
			Assert.False(groupA.IsValid);
			var groupB = name.ValidateGroups("B");
			Assert.False(groupB.IsValid);
			var groupC = name.ValidateGroups("C");
			Assert.True(groupC.IsValid);

			Assert.Equal("Property FirstName must be at least 10 characters long", groupA.Error);
			Assert.Equal("Property LastName must be at least 15 characters long", groupB.Error);
		}

		[Fact(Skip = "Lack of ICustomAttributeProvider breaks the ability add arbitrary types as Attributes")]
		public void CanChainValidateGroupAndObtainDataErrorInformation()
		{
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.FirstName = "Big";
			name.LastName = "Tex";

			var groupA = name.ValidateGroups("A");
			var groupAandB = groupA.ValidateGroups("B");

			Assert.False(groupAandB.IsValid);
			Assert.Equal("Property FirstName must be at least 10 characters long" + Environment.NewLine +
			             "Property LastName must be at least 15 characters long", groupAandB.Error);
		}

		[Fact]
		public void WillNotifyPropertyChangesOnValidateGroup()
		{
			var notifyCalled = false;
			var name = factory.GetAdapter<IMutableName>(dictionary);
			name.FirstName = "Big";
			name.LastName = "Tex";

			var groupA = name.ValidateGroups("A");
			groupA.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "IsValid")
				{
					notifyCalled = true;
				}
			};

			name.LastName = "Monster";
			Assert.True(notifyCalled);
		}

		[Fact]
		public void CanCreateDictionaryAdapterFromExistingAdapter()
		{
			var name = factory.GetAdapter<IName>(dictionary);
			var person = name.Create<IPerson>();
			Assert.NotNull(person);
			person.Name = "Chuck Norris";
			Assert.Equal("Chuck Norris", person.Name);
		}

		[Fact]
		public void CanCreateAndInitializeDictionaryAdapterFromExistingAdapter()
		{
			var name = factory.GetAdapter<IName>(dictionary);
			var person = name.Create<IPerson>(p => p.Name = "Chuck Norris");
			Assert.NotNull(person);
			Assert.Equal("Chuck Norris", person.Name);
		}

		[Fact]
		public void CanGetSimplePropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.Equal(5, container.Count);
		}

		[Fact]
		public void CanGetGuidPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.Equal(container.Id, new Guid());
		}

		[Fact]
		public void CanGetClassPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container.Address);
		}

		[Fact]
		public void CanGetArrayPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container.Positions);
		}

		[Fact]
		public void CanGetGenericCollectionPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container.GenericItems);
		}

		[Fact]
		public void CanGetCollectionPropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container.Items);
		}

		[Fact]
		public void CanGetInterfacePropertyOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.NotNull(container.Phone);
		}

#if !SILVERLIGHT && !NETCORE //no BindingList in Silverlight

		[Fact]
		public void CanAddBindingListItemsOnDemand()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			var person = container.Bindingtems.AddNew();
			Assert.NotNull(person);
		}
#endif

		[Fact]
		public void WillNotCreateObjectOnDemandWithoutDefaultConstructor()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			Assert.Null(container.EmailAddress);
		}

		[Fact]
		public void CanUseDynamicValues()
		{
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.Positions = new[] { 2, 4, 6, 8 };
			container.ReducePositions = new DynamicValueDelegate<int>(() => container.Positions.Sum());
			Assert.Equal(20, container.ReducePositions.Value);

			container.Positions = new[] { 1, 2, 3, 4 };
			Assert.Equal(10, container.ReducePositions.Value);
		}

		[Fact]
		public void WillGetNotificedWhenDynamicValueChanges()
		{
			var notifyCalled = false;
			var container = factory.GetAdapter<IItemContainer<IPerson>>(dictionary);
			container.ReducePositions = new DynamicValueDelegate<int>(() => container.Positions.Sum());

			container.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "ReducePositions")
				{
					notifyCalled = true;
					Assert.Equal(10, ((PropertyChangedEventArgsEx)e).NewValue);
				}
			};

			container.Positions = new[] { 1, 2, 3, 4 };
			Assert.Equal(10, container.ReducePositions.Value);
			Assert.True(notifyCalled);
		}

		[Fact]
		public void CanGetNewGuidPropertyOnDemand()
		{
			var conversions = factory.GetAdapter<IConversions>(dictionary);
			var guid = conversions.Guid;
			Assert.NotEqual(guid, new Guid());
			Assert.Equal(conversions.Guid, guid);
		}

		[Fact]
		public void CanDetermineTheAdaptedInterface()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			var type = person.GetType().GetTypeInfo().GetCustomAttributes(
				typeof(DictionaryAdapterAttribute), false).Cast<DictionaryAdapterAttribute>()
				.FirstOrDefault();
			Assert.NotNull(type);
			Assert.Equal(typeof(IPerson), type.InterfaceType);
		}

		[Fact]
		public void GetGetDictionaryAdapterMetaData()
		{
			var meta = factory.GetAdapterMeta(typeof(IPerson));
			Assert.NotNull(meta);
			Assert.Equal(typeof(IPerson), meta.Type);
		}

		[Fact]
		public void CanDetermineEqualityBetweenAdapters()
		{
			var container1 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), dictionary,
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));
			var container2 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), new Hashtable(),
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));
			Assert.Equal(container1, container1);
			Assert.NotEqual(container1, container2);

			container1.Id = Guid.NewGuid();
			container2.Id = container1.Id;
			Assert.Equal(container1, container2);
			container2.Id = Guid.NewGuid();
			Assert.NotEqual(container1, container2);
		}

		[Fact]
		public void CanCalculateHashCodeBetweenAdapters()
		{
			var container1 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), dictionary,
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));
			var container2 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), new Hashtable(),
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));

			container1.Id = Guid.NewGuid();
			container2.Id = container1.Id;
			Assert.Equal(container1.GetHashCode(), container2.GetHashCode());
		}

		[Fact]
		public void HashCodesAreImmuatable()
		{
			var container1 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), dictionary,
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));
			var container2 = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), new Hashtable(),
					new PropertyDescriptor().AddBehaviors(new IdEqualityHashCodeStrategy()));
			Assert.NotEqual(container1.GetHashCode(), container2.GetHashCode());
			container1.Id = Guid.NewGuid();
			container2.Id = container1.Id;
			Assert.NotEqual(container1.GetHashCode(), container2.GetHashCode());
		}

		[Fact]
		public void CanSupplyCustomCreationStrategy()
		{
			var container = (IItemContainer<IPerson>)
				factory.GetAdapter(typeof(IItemContainer<IPerson>), dictionary,
					new PropertyDescriptor().AddBehaviors(new CreateHashtableStrategy()));

			Assert.NotNull(container.Address);
			Assert.IsType<Hashtable>(container.This.Dictionary);
		}

		[Fact]
		public void CanObtainTheEffectiveDictionaryKey()
		{
			var person = factory.GetAdapter<IPersonWithPrefixOverride>(dictionary);
			var key = person.GetKey("EyeColor");
			Assert.Equal("Person2_Eye__Color", key);
		}

		[Fact]
		public void CanCoerceDictionaryInterfaces()
		{
			dictionary["FirstName"] = "Charlie";
			dictionary["LastName"] = "Brown";
			var name = factory.GetAdapter<IName>(dictionary);

			var mutableName = name.Coerce<IMutableName>();
			Assert.Equal(name.FirstName, mutableName.FirstName);
			Assert.Equal(name.LastName, mutableName.LastName);

			mutableName.FirstName = "Snoopy";
			mutableName.LastName = "";
			Assert.Equal("Snoopy", mutableName.FirstName);
			Assert.Equal("", mutableName.LastName);
		}

		[Fact]
		public void WillReturnSameInstanceIfCoerceExistingInterface()
		{
			var personWithPrefix = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			var person = ((IDictionaryAdapter)personWithPrefix).Coerce<IPerson>();
			Assert.Same(personWithPrefix, person);
		}

		[Fact]
		public void CanGroupBehaviorsWithBuilders()
		{
			var use = factory.GetAdapter<IUseBehaviorBuilder>(dictionary);
			use.First_Name = "Charlie";

			Assert.Equal(1, dictionary.Count);
			Assert.True(dictionary.Contains("Foo First Name"));
		}

		[Fact]
		public void Can_Remove_Matching_Property()
		{
			var person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Snoopy";
			Assert.True(dictionary.Contains("Name"));
			person.Name = null;
			Assert.False(dictionary.Contains("Name"));
		}

		[Fact]
		public void Can_Remove_Null_Values()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.StringValue = "Pizza";
			Assert.True(dictionary.Contains("StringValue"));
			empty.StringValue = null;
			Assert.False(dictionary.Contains("StringValue"));
		}

		[Fact]
		public void Can_Remove_Empty_Strings()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.StringValue = "Pizza";
			Assert.True(dictionary.Contains("StringValue"));
			empty.StringValue = "";
			Assert.False(dictionary.Contains("StringValue"));
		}

		[Fact]
		public void Can_Remove_Empty_Guids()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.GuidValue = Guid.NewGuid();
			Assert.True(dictionary.Contains("GuidValue"));
			empty.GuidValue = Guid.Empty;
			Assert.False(dictionary.Contains("GuidValue"));
		}

		[Fact]
		public void Can_Remove_Empty_Arrays()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.ArrayValue = new int[] { 1, 2, 3 };
			Assert.True(dictionary.Contains("ArrayValue"));
			empty.ArrayValue = new int[0];
			Assert.False(dictionary.Contains("ArrayValue"));
		}

		[Fact]
		public void Can_Remove_Empty_Collections()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.CollectionValue = new List<double>(new[] { 1.1, 2.2, 3.3 });
			Assert.True(dictionary.Contains("CollectionValue"));
			empty.CollectionValue = new List<double>();
			Assert.False(dictionary.Contains("CollectionValue"));
		}

		[Fact]
		public void Can_Remove_Empty_Nullables()
		{
			var empty = factory.GetAdapter<IEmptyTest>(dictionary);
			empty.NullableValue = 3.0F;
			Assert.True(dictionary.Contains("NullableValue"));
			empty.NullableValue = null;
			Assert.False(dictionary.Contains("NullableValue"));
		}
	}
}
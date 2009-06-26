namespace Castle.Facilities.NHibernateIntegration.Tests.Components
{
	using System;
	using NUnit.Framework;
	using System.Collections.Generic;
	using Util;

	[TestFixture]
	public class ReflectionUtilityTests
	{
		enum MyEnum
		{

		}
		[Test]
		public void Can_get_properties_as_dictionary()
		{
			var blog = new Blog {Name = "osman", Items = new List<BlogItem>() {new BlogItem {}}};
			var dictionary=ReflectionUtility.GetPropertiesDictionary(blog);
			Assert.That(dictionary.ContainsKey("Name"));
			Assert.That(dictionary.ContainsKey("Id"));
			Assert.That(dictionary.ContainsKey("Items"));
			Assert.That(dictionary["Name"],Is.EqualTo("osman"));
		}

		[Test]
		public void SimpleType_returns_true_for_enum_string_datetime_and_primitivetypes_()
		{
			Assert.True(ReflectionUtility.IsSimpleType(typeof(string)));
			Assert.True(ReflectionUtility.IsSimpleType(typeof(DateTime)));
			Assert.True(ReflectionUtility.IsSimpleType(typeof(MyEnum)));
			Assert.True(ReflectionUtility.IsSimpleType(typeof(char)));
		}
	}
}

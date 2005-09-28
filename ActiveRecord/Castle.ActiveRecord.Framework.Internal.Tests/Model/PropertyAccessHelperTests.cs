using System;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using NUnit.Framework;

	[TestFixture]
	public class PropertyAccessHelperTests
	{
		[Test]
		public void Property()
		{
			Assert.AreEqual("property", PropertyAccessHelper.ToString(PropertyAccess.Property));
		}

		[Test]
		public void Field()
		{
			Assert.AreEqual("field", PropertyAccessHelper.ToString(PropertyAccess.Field));
		}

		[Test]
		public void FieldCamelCase()
		{
			Assert.AreEqual("field.camelcase", PropertyAccessHelper.ToString(PropertyAccess.FieldCamelcase));
		}

		[Test]
		public void FieldCamelCaseUnderscore()
		{
			Assert.AreEqual("field.camelcase-underscore", PropertyAccessHelper.ToString(PropertyAccess.FieldCamelcaseUnderscore));
		}

		[Test]
		public void FieldLowerCaseUnderscore()
		{
			Assert.AreEqual("field.lowercase-underscore", PropertyAccessHelper.ToString(PropertyAccess.FieldLowercaseUnderscore));
		}

		[Test]
		public void FieldPascalCase_M_Underscore()
		{
			Assert.AreEqual("field.pascalcase-m-underscore", PropertyAccessHelper.ToString(PropertyAccess.FieldPascalcaseMUnderscore));
		}

		[Test]
		public void NoSetterCamelCase()
		{
			Assert.AreEqual("nosetter.camelcase", PropertyAccessHelper.ToString(PropertyAccess.NosetterCamelcase));
		}

		[Test]
		public void NoSetterCamelCaseUnderscore()
		{
			Assert.AreEqual("nosetter.camelcase-underscore", PropertyAccessHelper.ToString(PropertyAccess.NosetterCamelcaseUnderscore));
		}

		[Test]
		public void NoSetterLowerCaseUnderscore()
		{
			Assert.AreEqual("nosetter.lowercase-underscore", PropertyAccessHelper.ToString(PropertyAccess.NosetterLowercaseUnderscore));
		}

		[Test]
		public void NoSetterPascalCase_M_Underscore()
		{
			Assert.AreEqual("nosetter.pascalcase-m-underscore", PropertyAccessHelper.ToString(PropertyAccess.NosetterPascalcaseMUndersc));
		}
	}
}
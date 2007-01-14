namespace Castle.Components.Validator.Tests.ValidatorTests
{
	using System;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class DateTimeValidatorTestCase
	{
		private DateTimeValidator validator;
		private TestTarget target;

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

			validator = new DateTimeValidator();
			validator.Initialize(typeof(TestTarget).GetProperty("DtField"));
			target = new TestTarget();
		}

		[Test]
		public void InvalidDate()
		{
			Assert.IsFalse(validator.IsValid(target, "some"));
			Assert.IsFalse(validator.IsValid(target, "122"));
			Assert.IsFalse(validator.IsValid(target, "99/99/99"));
			Assert.IsFalse(validator.IsValid(target, "99-99-99"));
		}

		[Test]
		public void ValidDate()
		{
			Assert.IsTrue(validator.IsValid(target, "01/12/2004"));
			Assert.IsTrue(validator.IsValid(target, "07/16/79"));
			Assert.IsTrue(validator.IsValid(target, "2007-01-14T12:05:25"));
		}

		public class TestTarget
		{
			private DateTime dtField;

			public DateTime DtField
			{
				get { return dtField; }
				set { dtField = value; }
			}
		}
	}
}

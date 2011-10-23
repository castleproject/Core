namespace Castle.Components.Validator.Tests
{
	using System.ComponentModel.DataAnnotations;
	using NUnit.Framework;

	[TestFixture]
	public class CachedMetadataValidationRegistryTestCase
	{
		[MetadataType(typeof(PersonMetaData))]
		public class Person
		{
			public string Name {get;set;}
		}

		public class PersonMetaData
		{
			[ValidateNonEmpty][ValidateLength(1,40)]
			public string Name {get;set;}
		}

		[Test]
		public void Should_Validate_Object_Using_Validators_From_Metadata()
		{
			var runner = new ValidatorRunner(new CachedMetadataValidationRegistry());

			var person = new Person();
			Assert.IsFalse(runner.IsValid(person));

			person = new Person{Name = "stan"};
			Assert.IsTrue(runner.IsValid(person));
		}
	}
}
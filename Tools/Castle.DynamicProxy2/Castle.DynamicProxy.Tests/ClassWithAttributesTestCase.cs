namespace Castle.DynamicProxy.Tests
{
	using System.IO;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Classes;
	using NUnit.Framework;

	[TestFixture]
	public class ClassWithAttributesTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods()
		{
			ProxyGenerator generator = new ProxyGenerator();

			AttributedClass instance = (AttributedClass)
				generator.CreateClassProxy(typeof(AttributedClass), new StandardInterceptor());

			object[] attributes = instance.GetType().GetCustomAttributes(typeof(NonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof(NonInheritableAttribute), attributes[0]);

			attributes = instance.GetType().GetMethod("Do1").GetCustomAttributes(typeof(NonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof(NonInheritableAttribute), attributes[0]);
		}

		[Test]
		public void EnsureProxyHasAttributesOnClassAndMethods_ComplexAttributes()
		{
			ProxyGenerator generator = new ProxyGenerator();

			AttributedClass2 instance = (AttributedClass2)
				generator.CreateClassProxy(typeof(AttributedClass2), new StandardInterceptor());

			object[] attributes = instance.GetType().GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof(ComplexNonInheritableAttribute), attributes[0]);
			ComplexNonInheritableAttribute att = (ComplexNonInheritableAttribute) attributes[0];
			// (1, 2, true, "class", FileAccess.Write)
			Assert.AreEqual(1, att.Id);
			Assert.AreEqual(2, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("class", att.Name);
			Assert.AreEqual(FileAccess.Write, att.Access);

			attributes = instance.GetType().GetMethod("Do1").GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof(ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (2, 3, "Do1", Access = FileAccess.ReadWrite)
			Assert.AreEqual(2, att.Id);
			Assert.AreEqual(3, att.Num);
			Assert.AreEqual(false, att.IsSomething);
			Assert.AreEqual("Do1", att.Name);
			Assert.AreEqual(FileAccess.ReadWrite, att.Access);

			attributes = instance.GetType().GetMethod("Do2").GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false);
			Assert.AreEqual(1, attributes.Length);
			Assert.IsInstanceOfType(typeof(ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute)attributes[0];
			// (3, 4, "Do2", IsSomething=true)
			Assert.AreEqual(3, att.Id);
			Assert.AreEqual(4, att.Num);
			Assert.AreEqual(true, att.IsSomething);
			Assert.AreEqual("Do2", att.Name);
		}
	}
}

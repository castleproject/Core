// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Tests.MixinTests
{
	using System;

	using NUnit.Framework;

	using AspectSharp.Builder;
	using AspectSharp.Tests.Classes;

	/// <summary>
	/// Summary description for MixinTestCase.
	/// </summary>
	[TestFixture]
	public class MixinTestCase
	{
		[Test]
		public void MakeAuthorAPersonAndACustomer()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect McBrother for Author " + 
				"   " + 
				"   include DummyCustomer" + 
				"   include DummyPerson" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			Author author = engine.WrapClass(typeof(Author)) as Author;
			Assert.IsNotNull(author);
			Assert.IsNotNull(author as ICustomer);
			Assert.IsNotNull(author as IPerson);

			ICustomer customer = author as ICustomer;
			customer.CustomerId = 10;
			Assert.AreEqual( 10, customer.CustomerId );

			IPerson person = author as IPerson;
			person.Name = "Billy Paul McKinsky";
			Assert.AreEqual( 10, customer.CustomerId );

			Assert.AreEqual( 0, author.BooksPublished );
			author.MoreOneBook();
			Assert.AreEqual( 1, author.BooksPublished );
		}

		[Test]
		public void ImplementLoggerProperty()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect McBrother for Author " + 
				"   " + 
				"   include Loggeable" + 
				"   " + 
				"   pointcut method|property(*)" + 
				"     advice(LogInvocationsInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			Author author = engine.WrapClass(typeof(Author)) as Author;
			Assert.IsNotNull(author);
			Assert.IsNotNull(author as ILoggeable);

			Assert.AreEqual( 0, author.BooksPublished );
			author.MoreOneBook();
			Assert.AreEqual( 1, author.BooksPublished );

			ILoggeable log = author as ILoggeable;
			String messages = log.GetLogMessages();
			// TODO: Correct compare
			Assert.AreEqual( "Invoking get_BooksPublished;Invoking MoreOneBook;Invoking get_BooksPublished;", messages );
		}

		[Test]
		public void MixinMethodsMustBeIntercepted()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect McBrother for Author " + 
				"   " + 
				"   include Loggeable" + 
				"   include DummyPerson" + 
				"   " +
				"   pointcut method|property(*)" + 
				"     advice(LogInvocationsInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			Author author = engine.WrapClass(typeof(Author)) as Author;
			Assert.IsNotNull(author);
			Assert.IsNotNull(author as ILoggeable);
			Assert.IsNotNull(author as IPerson);

			IPerson person = author as IPerson;
			person.Name = "McBilly";
			Assert.AreEqual( "McBilly", person.Name );

			ILoggeable log = author as ILoggeable;
			log.Log("Test");
			String messages = log.GetLogMessages();
			Assert.AreEqual( "Invoking set_Name;Invoking get_Name;Test;", messages );
		}

		[Test]
		public void MixinProxyAware()
		{
			String contents = "import AspectSharp.Tests.Classes in AspectSharp.Tests " + 
				" " + 
				" aspect McBrother for LogEnabledAuthor " + 
				"   " + 
				"   include LogFactoryMixin" + 
				"   " + 
				" end ";

			AspectEngineBuilder builder = new AspectLanguageEngineBuilder(contents);
			AspectEngine engine = builder.Build();

			LogEnabledAuthor author = engine.WrapClass(typeof(LogEnabledAuthor)) as LogEnabledAuthor;
			Assert.IsNotNull(author);
			Assert.IsNotNull(author as ILogEnabled);
			Assert.IsNotNull(author.Logger);
		}
	}
}

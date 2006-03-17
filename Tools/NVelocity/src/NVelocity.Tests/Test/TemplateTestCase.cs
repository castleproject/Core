using ExtendedProperties = Commons.Collections.ExtendedProperties;

namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using NUnit.Framework;
	using NVelocity.App;
	using NVelocity.Runtime;
	using NVelocity.Runtime.Resource;
	using NVelocity.Runtime.Resource.Loader;
	using NVelocity.Test.Provider;

	/// <summary>
	/// Easily add test cases which evaluate templates and check their output.
	///
	/// NOTE:
	/// This class DOES NOT extend RuntimeTestCase because the TemplateTestSuite
	/// already initializes the Velocity runtime and adds the template
	/// test cases. Having this class extend RuntimeTestCase causes the
	/// Runtime to be initialized twice which is not good. I only discovered
	/// this after a couple hours of wondering why all the properties
	/// being setup were ending up as Vectors. At first I thought it
	/// was a problem with the Configuration class, but the Runtime
	/// was being initialized twice: so the first time the property
	/// is seen it's stored as a String, the second time it's seen
	/// the Configuration class makes a Vector with both Strings.
	/// As a result all the getBoolean(property) calls were failing because
	/// the Configurations class was trying to create a Boolean from
	/// a Vector which doesn't really work that well. I have learned
	/// my lesson and now have to add some code to make sure the
	/// Runtime isn't initialized more then once :-)
	/// </summary>
	[TestFixture]
	public class TemplateTestCase : BaseTestCase
	{
		private ExtendedProperties testProperties;

		private TestProvider provider;
		private ArrayList al;
		private Hashtable h;
		private VelocityContext context;
		private VelocityContext context1;
		private VelocityContext context2;
		private ArrayList vec;
		
		private VelocityEngine ve;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public TemplateTestCase()
		{
			try
			{
				ve = new VelocityEngine();

				ExtendedProperties ep = new ExtendedProperties();
				ep.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, TemplateTest.FILE_RESOURCE_LOADER_PATH);

				ep.SetProperty(RuntimeConstants.RUNTIME_LOG_ERROR_STACKTRACE, "true");
				ep.SetProperty(RuntimeConstants.RUNTIME_LOG_WARN_STACKTRACE, "true");
				ep.SetProperty(RuntimeConstants.RUNTIME_LOG_INFO_STACKTRACE, "true");

				ve.Init(ep);

				testProperties = new ExtendedProperties();
				testProperties.Load(new FileStream(TemplateTest.TEST_CASE_PROPERTIES, FileMode.Open, FileAccess.Read));
			}
			catch (Exception e)
			{
				throw new Exception("Cannot setup TemplateTestSuite!");
			}
		}

		/// <summary>
		/// Sets up the test.
		/// </summary>
		[SetUp]
		protected void SetUp()
		{
			provider = new TestProvider();
			al = provider.Customers;
			h = new Hashtable();

			SupportClass.PutElement(h, "Bar", "this is from a hashtable!");
			SupportClass.PutElement(h, "Foo", "this is from a hashtable too!");

			/*
			*  lets set up a vector of objects to test late introspection. See ASTMethod.java
			*/

			vec = new ArrayList();

			vec.Add(new String("string1".ToCharArray()));
			vec.Add(new String("string2".ToCharArray()));

			/*
			*  set up 3 chained contexts, and add our data 
			*  throught the 3 of them.
			*/

			context2 = new VelocityContext();
			context1 = new VelocityContext(context2);
			context = new VelocityContext(context1);

			context.Put("provider", provider);
			context1.Put("name", "jason");
			context2.Put("providers", provider.Customers2);
			context.Put("list", al);
			context1.Put("hashtable", h);
			context2.Put("hashmap", new Hashtable());
			context2.Put("search", provider.Search);
			context.Put("relatedSearches", provider.RelSearches);
			context1.Put("searchResults", provider.RelSearches);
			context2.Put("stringarray", provider.Array);
			context.Put("vector", vec);
			context.Put("mystring", new String("".ToCharArray()));
			context.Put("runtime", new FieldMethodizer("NVelocity.Runtime.RuntimeSingleton"));
			context.Put("fmprov", new FieldMethodizer(provider));
			context.Put("Floog", "floogie woogie");
			context.Put("boolobj", new BoolObj());

			/*
	    *  we want to make sure we test all types of iterative objects
	    *  in #foreach()
	    */

			Object[] oarr = new Object[] {"a", "b", "c", "d"};
			int[] intarr = new int[] {10, 20, 30, 40, 50};

			context.Put("collection", vec);
			context2.Put("iterator", vec.GetEnumerator());
			context1.Put("map", h);
			context.Put("obarr", oarr);
			context.Put("enumerator", vec.GetEnumerator());
			context.Put("intarr", intarr);
		}

		[Test]
		public void CacheProblems()
		{
			VelocityContext context = new VelocityContext();

			context.Put( "AjaxHelper2", new AjaxHelper2() );
			context.Put( "DictHelper", new DictHelper() );

			Template template = ve.GetTemplate(
				GetFileName(null, "dicthelper", TemplateTest.TMPL_FILE_EXT));
			
			StringWriter writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );
		}

		/// <summary>
		/// Adds the template test cases to run to this test suite.  Template test
		/// cases are listed in the <code>TEST_CASE_PROPERTIES</code> file.
		/// </summary>
		[Test]
		public void Test_Run()
		{
			String template;
			Boolean allpass = true;
			Int32 failures = 0;
			for (int i = 1;; i++)
			{
				template = testProperties.GetString(getTemplateTestKey(i));

				if (template != null)
				{
					bool pass = RunTest(template);
					if (!pass)
					{
						Console.Out.Write("Adding TemplateTestCase : " + template + "...");
						Console.Out.WriteLine("FAIL!");
						allpass = false;
					}
				}
				else
				{
					// Assume we're done adding template test cases.
					break;
				}
			}

			if (!allpass)
			{
				Assert.Fail(failures.ToString() + " templates failed");
			}
		}

		/// <summary>
		/// Macro which returns the properties file key for the specified template
		/// test number.
		/// </summary>
		/// <param name="nbr">The template test number to return a property key for.</param>
		/// <returns>The property key.</returns>
		private static String getTemplateTestKey(int nbr)
		{
			return ("test.template." + nbr);
		}

		/// <summary>
		/// Runs the test.
		/// </summary>
		private Boolean RunTest(String baseFileName)
		{
			// run setup before each test so that the context is clean
			SetUp();

			try
			{
				Template template = ve.GetTemplate(GetFileName(null, baseFileName, TemplateTest.TMPL_FILE_EXT));

				AssureResultsDirectoryExists(TemplateTest.RESULT_DIR);

				/* get the file to write to */
				FileStream fos = new FileStream(GetFileName(TemplateTest.RESULT_DIR, baseFileName, TemplateTest.RESULT_FILE_EXT), FileMode.Create);

				StreamWriter writer = new StreamWriter(fos);

				/* process the template */
				template.Merge(context, writer);

				/* close the file */
				writer.Flush();
				writer.Close();

				if (!IsMatch(TemplateTest.RESULT_DIR, TemplateTest.COMPARE_DIR, baseFileName,
				             TemplateTest.RESULT_FILE_EXT, TemplateTest.CMP_FILE_EXT))
				{
					//Fail("Processed template did not match expected output");
					return false;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Test {0} failed", baseFileName);

				throw;
			}

			return true;
		}
	}

}
// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace AspectSharpExample
{
	using System;
	using System.Collections;

	using AspectSharp;
	using AspectSharp.Builder;

	using AspectSharp.Example;
	using AspectSharp.Example.Views;
	using AspectSharp.Example.ContentProviders;

	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		public static void Main()
		{
			// First we're going to execute two content providers and
			// display the contents using the PlainTextView
			SimpleExecution();

			// Now we're going to execute adding a security
			// mixin to content providers and security checking through 
			// interceptors
			MixinAndInterceptorExecution();

			// Altering the Hashtable
			HashtableTest();

			Console.WriteLine("\r\nPress any key to exit." );
			Console.ReadLine();
		}

		private static void SimpleExecution()
		{
			Console.Out.WriteLine( " o0o First execution o0o " );

			RequestPipeline pipeline = new RequestPipeline();
			pipeline.Context["username"] = "Billy Paul Mckinsky";
			pipeline.AddContentProvider( new StaticContentProvider() );
			pipeline.AddContentProvider( new DynamicContentProvider() );
			pipeline.View = new PlainTextView();
			pipeline.ProcessRequest( Console.Out );

			Console.Out.WriteLine();
		}

		private static void MixinAndInterceptorExecution()
		{
			Console.Out.WriteLine( " o0o Within security checking o0o " );

			// For the sake of readability we're keep the aspect code here:
			String contents = 
				" import AspectSharp.Example.Aop.Interceptors " + 
				" import AspectSharp.Example.Aop.Mixins " + 
				" " +
				" aspect sample for [ AspectSharp.Example.ContentProviders ] " + 
				"   include SecurityMixin " + 
				"   " + 
				"   pointcut method(* RetrieveContent())" + 
				"     advice(SecurityCheckInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectLanguageEngineBuilder builder = new AspectLanguageEngineBuilder( contents );
			AspectEngine engine = builder.Build();

			RequestPipeline pipeline = new RequestPipeline();
			pipeline.Context["username"] = "Billy Paul Mckinsky";
			pipeline.AddContentProvider( engine.WrapClass( typeof(StaticContentProvider) ) as IContentProvider );
			pipeline.AddContentProvider( engine.WrapClass( typeof(DynamicContentProvider) ) as IContentProvider );
			pipeline.View = new PlainTextView();
			pipeline.ProcessRequest( Console.Out );

			Console.Out.WriteLine();
		}

		private static void HashtableTest()
		{
			Console.Out.WriteLine( " o0o Changing default hashtable value o0o " );

			// For the sake of readability we're keep the aspect code here:
			String contents = 
				" import System.Collections " + 
				" " +
				" aspect sample for Hashtable " + 
				"   " + 
				"   pointcut propertyread(* Item(*))" + 
				"     advice(HashcodeDefaultValueInterceptor)" + 
				"   end" + 
				"   " + 
				" end ";

			AspectLanguageEngineBuilder builder = new AspectLanguageEngineBuilder( contents );
			AspectEngine engine = builder.Build();

			IDictionary myHashTable = engine.WrapClass( typeof(Hashtable) ) as IDictionary;

			String value = myHashTable["item"] as String;
			Console.Out.WriteLine( "Default value is {0}", value );

			Console.Out.WriteLine();			
		}
	}
}

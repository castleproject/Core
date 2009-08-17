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

using ExtendedProperties=Commons.Collections.ExtendedProperties;

namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using App;
	using Exception;
	using NUnit.Framework;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;
	using Runtime;

	/// <summary>
	/// Test Velocity Introspector
	/// </summary>
	[TestFixture]
	public class ParserTest
	{
		//[Test]
		//public void Test_VelocityCharStream()
		//{
		//  String s1 = "this is a test";
		//  VelocityCharStream vcs = new VelocityCharStream(new StringReader(s1), 1, 1);

		//  String s2 = String.Empty;
		//  try
		//  {
		//    Char c = vcs.ReadChar();
		//    while (true)
		//    {
		//      s2 += c;
		//      c = vcs.ReadChar();
		//    }
		//  }
		//  catch (IOException)
		//  {
		//    // this is expected to happen when the stream has been read
		//  }
		//  Assert.IsTrue(s1.Equals(s2), "read stream did not match source string");
		//}
		[Test]
		public void Test_VelocityTryCharStream()
		{
			String s1 = "this is a test";
			VelocityCharStream vcs = new VelocityCharStream(new StringReader(s1), 1, 1);

			String s2 = String.Empty;

			while(vcs.ReadChar())
			{
				s2 += vcs.CurrentCharacter;
			}
			Assert.IsTrue(s1.Equals(s2), "read stream did not match source string");
		}

		[Test]
		public void Test_Parse()
		{
			VelocityCharStream vcs = GetTemplateStream();
			Parser p = new Parser(vcs);

			SimpleNode root = p.Process();

			String javaNodes = "19,18,9,5,23,56,23,42,23,24,6,18,56,18,9,5,23,56,23,42,23,25,6,18,44,23,5,56,6,18,46,18,43,0";
			String nodes = String.Empty;

			if (root != null)
			{
				Token t = root.FirstToken;
				nodes += t.Kind.ToString();
				while(t != root.LastToken)
				{
					t = t.Next;
					nodes += "," + t.Kind;
				}
			}

			if (!javaNodes.Equals(nodes))
			{
				Console.Out.WriteLine(string.Empty);
				Console.Out.WriteLine(".Net parsed nodes did not match java nodes.");
				Console.Out.WriteLine("java=" + javaNodes);
				Console.Out.WriteLine(".net=" + nodes);
				Assert.Fail(".Net parsed nodes did not match java nodes.");
			}
		}


		[Test]
		public void Test_RuntimeServices()
		{
			IRuntimeServices ri = RuntimeSingleton.RuntimeServices;

			try
			{
				ri.Init();
			}
			catch(Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			ExtendedProperties extendedProperties = ri.Configuration;
		}

		[Test]
		public void Test_Example1()
		{
			String templateFile = "example1.vm";
			try
			{
				/*
				* setup
				*/

				VelocityEngine velocityEngine = new VelocityEngine();

				ExtendedProperties extendedProperties = new ExtendedProperties();
				extendedProperties.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, TemplateTest.FILE_RESOURCE_LOADER_PATH);

				velocityEngine.Init(extendedProperties);

				/*
				*  Make a context object and populate with the data.  This 
				*  is where the Velocity engine gets the data to resolve the
				*  references (ex. $list) in the template
				*/
				VelocityContext context = new VelocityContext();
				context.Put("list", GetNames());

				ExtendedProperties props = new ExtendedProperties();
				props.Add("runtime.log", "nvelocity.log");
				context.Put("props", props);

				/*
				*    get the Template object.  This is the parsed version of your 
				*  template input file.  Note that getTemplate() can throw
				*   ResourceNotFoundException : if it doesn't find the template
				*   ParseErrorException : if there is something wrong with the VTL
				*   Exception : if something else goes wrong (this is generally
				*        indicative of as serious problem...)
				*/
				Template template = null;

				try
				{
					template = velocityEngine.GetTemplate(templateFile);
				}
				catch(ResourceNotFoundException resourceNotFoundException)
				{
					Console.Out.WriteLine("Example : error : cannot find template {0} : \n{1}", templateFile,
					                      resourceNotFoundException.Message);
					Assert.Fail();
				}
				catch(ParseErrorException parseErrorException)
				{
					Console.Out.WriteLine("Example : Syntax error in template {0} : \n{1}", templateFile, parseErrorException);
					Assert.Fail();
				}

				/*
				*  Now have the template engine process your template using the
				*  data placed into the context.  Think of it as a  'merge' 
				*  of the template and the data to produce the output stream.
				*/

				// using Console.Out will send it to the screen
				TextWriter writer = new StringWriter();
				if (template != null)
					template.Merge(context, writer);

				/*
				*  flush and cleanup
				*/

				writer.Flush();
				writer.Close();
			}
			catch(Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		private ArrayList GetNames()
		{
			ArrayList list = new ArrayList();

			list.Add("Billy");
			list.Add("Bob");
			list.Add("Jane");
			list.Add("Sarah");

			return list;
		}

		private VelocityCharStream GetTemplateStream()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("## This is an example velocity template\n");
			sb.Append("\n");
			sb.Append("#set( $this = \"Velocity\")\n");
			sb.Append("\n");
			sb.Append("$this is great!\n");
			sb.Append("\n");
			//TODO: parse fails for this segment for unknown reason
			//	    sb.Append("#foreach( $name in $list )\n");
			//	    sb.Append("row $velocityCount :: $name is great!\n");
			//	    sb.Append("#end\n");
			//	    sb.Append("\n");
			sb.Append("#set( $condition = true)\n");
			sb.Append("\n");
			sb.Append("#if ($condition)\n");
			sb.Append("    The condition is true!\n");
			sb.Append("#else\n");
			sb.Append("    The condition is false!\n");
			sb.Append("#end\n");

			VelocityCharStream vcs = new VelocityCharStream(new StringReader(sb.ToString()), 1, 1);
			return vcs;
		}
	}
}
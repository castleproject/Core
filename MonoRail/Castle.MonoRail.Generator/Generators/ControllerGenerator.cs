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

namespace Castle.MonoRail.Generator.Generators
{
	using System;
	using System.IO;
	using System.Text;
	using System.CodeDom;
	using System.Collections;
	using System.CodeDom.Compiler;

	using Microsoft.CSharp;
	using Microsoft.VisualBasic;

	/// <summary>
	/// 
	/// </summary>
	public class ControllerGenerator : AbstractGenerator, IGenerator
	{
		private bool isNVelocity;
		private bool isSmart;
		private bool isCSharp;
		private DirectoryInfo controllersDir;
		private DirectoryInfo viewsDir;
		private DirectoryInfo testsDir;

		public ControllerGenerator()
		{
		}

		#region IGenerator Members

		public bool Accept(String name, IDictionary options, TextWriter output)
		{
			if ("controller".Equals(name))
			{
				if (options.Values.Count == 1)
				{
					output.WriteLine("Creates a controller and it's view skeletons");
					output.WriteLine("");
					output.WriteLine("name     : Controller name");
					output.WriteLine("actions  : Comma separated list of action names");
					output.WriteLine("ns       : Namespace");
					output.WriteLine("outdir   : Target directory (must exists)");
					output.WriteLine("area     : [Optional] Controller area/section");
					output.WriteLine("smart    : [Optional] Extends SmartDispatcherController (defaults to false)");
					output.WriteLine("view     : [Optional] aspnet|nvelocity (defaults to nvelocity)");
					output.WriteLine("lang     : [Optional] c#|vb.net (defaults to c#)");
					output.WriteLine("");
					output.WriteLine("Examples:");
					output.WriteLine("");
					output.WriteLine(@"> generator controller name:Home actions:Index,About outdir:c:\temp");
					output.WriteLine("");
					output.WriteLine(@"> generator controller name:Account ns:My.Project actions:Save,Authenticate smart view:aspnet outdir:c:\temp");
					output.WriteLine("");
					output.WriteLine(@"> generator controller name:Home actions:Index,About smart lang:vb.net outdir:c:\temp");
					output.WriteLine("");
					output.WriteLine("Remarks:");
					output.WriteLine("");
					output.WriteLine("The outdir must point to a structure created by 'generator project'");
					output.WriteLine("as it relies on the existance of a subdir Controllers and a subdir ");
					output.WriteLine("Views.");
					output.WriteLine("Also note that if you're using Windsor integration, you need to add");
					output.WriteLine("the controller definition to your container:");
					output.WriteLine("");
					output.WriteLine(" container.AddComponent( 'mycontroller', typeof(HomeController) ); ");
					output.WriteLine("");

					return false;
				}
				else if (!options.Contains("actions"))
				{
					output.WriteLine("actions must be specified");

					return false;
				}
				else if (!options.Contains("outdir"))
				{
					output.WriteLine("outdir must be specified");

					return false;
				}
				else if (!options.Contains("name"))
				{
					output.WriteLine("name must be specified");

					return false;
				}
				else 
				{
					DirectoryInfo info = new DirectoryInfo(options["outdir"] as String);

					if (!info.Exists)
					{
						// info.Create(); // Is it safe to use it?
						output.WriteLine("Error: The specified outdir does not exists.");

						return false;
					}
				}

				return true;
			}

			return false;
		}

		public void Execute(IDictionary options, TextWriter output)
		{
			isNVelocity = !(options.Contains("view") && options["view"].Equals("aspnet"));
			isSmart = options.Contains("smart");
			isCSharp = !(options.Contains("lang") && options["lang"].Equals("vb.net"));
			String name = options["name"] as String;
			String area = options["area"] as String;
			String[] actions = (options["actions"] as String).Split(',');
			
			String ns = options["ns"] as String;
			if (ns == null) ns = "WebApplication";

			output.WriteLine("Creating controller source code...");

			/// Steps to generate a controller

			// 1. Resolve directories (controllers, view, tests)

			output.WriteLine("Resolving directories...");
			ResolveDirs(output, options);

			// 2. Generate code (controllers, view, tests)
			// 3. Write them

			output.WriteLine("Generating source code...");
			GenerateCodeAndWrite(name, area, actions, ns, output);


			output.WriteLine("Done!");
		}

		#endregion

		private void ResolveDirs(TextWriter output, IDictionary options)
		{
			DirectoryInfo outdir = new DirectoryInfo(options["outdir"] as String);
			
			try
			{
				controllersDir = new DirectoryInfo(Path.Combine(outdir.FullName, "Controllers"));
				viewsDir = new DirectoryInfo(Path.Combine(outdir.FullName, "views"));
				testsDir = new DirectoryInfo(Path.Combine(outdir.FullName, "../" + outdir.Parent + ".Tests"));

				if (!controllersDir.Exists)
				{
					outdir = new DirectoryInfo( Path.Combine(outdir.FullName, outdir.Name) );

					controllersDir = new DirectoryInfo(Path.Combine(outdir.FullName, "Controllers"));
					viewsDir = new DirectoryInfo(Path.Combine(outdir.FullName, "views"));
					testsDir = new DirectoryInfo(Path.Combine(outdir.FullName, "../" + outdir.Parent + ".Tests"));
				}

				if (!controllersDir.Exists)
				{
					throw new ApplicationException("Could not infer directory structure from the specified 'outdir'");
				}
			}
			catch(Exception ex)
			{
				output.WriteLine(ex.Message);
				throw;
			}
		}

		private void GenerateCodeAndWrite(string name, string area, string[] actions, String ns, TextWriter output)
		{
			CodeDomProvider provider = (isCSharp) ? new CSharpCodeProvider() as CodeDomProvider : new VBCodeProvider() as CodeDomProvider; 
			String fileExtension = (isCSharp) ? ".cs" : ".vb";

			GenerateController(provider, fileExtension, ns, name, area, actions, output);

			GenerateViews(name, actions);

			GenerateTestCase(provider, fileExtension, ns, name, area, actions, output);
		}

		private string Quote(string value)
		{
			return String.Format("\"{0}\"", value);
		}

		private void GenerateController(CodeDomProvider provider, string fileExtension, string ns, string name, 
			string area, string[] actions, TextWriter output)
		{
			CodeNamespace thisNs = GenerateControllerCode(ns, name, area, actions);

			FileInfo controllerFile = new FileInfo( 
				Path.Combine(controllersDir.FullName, name + fileExtension) );

			if (!controllerFile.Exists)
			{
				using (StreamWriter sw = new StreamWriter(controllerFile.FullName, false, Encoding.Default))
				{
					CodeGeneratorOptions opts = new CodeGeneratorOptions();
					opts.BracingStyle = "C";
					provider.CreateGenerator().GenerateCodeFromNamespace(thisNs, sw, opts);
				}
			}
			else
			{
				output.WriteLine("Skipping {0} as the files exists", controllerFile.FullName);
			}
		}

		private CodeNamespace GenerateControllerCode(string ns, string name, string area, string[] actions)
		{
			CodeNamespace thisNs = new CodeNamespace(ns);
			thisNs.Imports.Add(new CodeNamespaceImport("System"));
			thisNs.Imports.Add(new CodeNamespaceImport("Castle.MonoRail.Framework"));
//			thisNs.Comments.Add(new CodeCommentStatement("Ignore the above comment or better, delete it"));
	
			CodeTypeDeclaration controllerType = new CodeTypeDeclaration(name);
			thisNs.Types.Add(controllerType);

			if (area != null)
			{
				controllerType.CustomAttributes.Add( 
					new CodeAttributeDeclaration("ControllerDetails", 
					new CodeAttributeArgument(new CodeSnippetExpression(Quote(StripControllerFrom(name)))), 
					new CodeAttributeArgument("Area", new CodeSnippetExpression(Quote(area)))) );
			}
			else
			{
				controllerType.CustomAttributes.Add( 
					new CodeAttributeDeclaration("ControllerDetails", 
					new CodeAttributeArgument(new CodeSnippetExpression(Quote(StripControllerFrom(name))))) );
			}
	
			if (isSmart)
			{
				controllerType.BaseTypes.Add( "SmartDispatcherController" );
			}
			else
			{
				controllerType.BaseTypes.Add( "Controller" );
			}
	
			foreach(String action in actions)
			{
				CodeMemberMethod actionMethod = new CodeMemberMethod();
				actionMethod.Name = action;
				actionMethod.Attributes = MemberAttributes.Public;

				controllerType.Members.Add(actionMethod);
			}

			return thisNs;
		}

		private void GenerateViews(String name, String[] actions)
		{
			DirectoryInfo viewSubDir = viewsDir.CreateSubdirectory(StripControllerFrom(name));

			foreach(String action in actions)
			{
				Hashtable ctx = new Hashtable();
				ctx.Add("controller", name);
				ctx.Add("action", action);
				ctx.Add("dir", viewSubDir.FullName);

				if (isNVelocity)
				{
					WriteTemplateFile( Path.Combine(viewSubDir.FullName, action + ".vm"), ctx, "newview.vm" );
				}
				else
				{
					WriteTemplateFile( Path.Combine(viewSubDir.FullName, action + ".aspx"), ctx, "newviewaspx.vm" );
				}
			}
		}

		private void GenerateTestCase(CodeDomProvider provider, string extension, string ns, string name, string area, 
			string[] actions, TextWriter output)
		{
			CodeNamespace nsunit = GenerateControllerTestCode(ns, name, area, actions);

			FileInfo controllerFile = new FileInfo( 
				Path.Combine(testsDir.FullName, name + "Tests" + extension) );

			if (!controllerFile.Exists)
			{
				using (StreamWriter sw = new StreamWriter(controllerFile.FullName, false, Encoding.Default))
				{
					CodeGeneratorOptions opts = new CodeGeneratorOptions();
					opts.BracingStyle = "C";
					provider.CreateGenerator().GenerateCodeFromNamespace(nsunit, sw, opts);
				}
			}
			else
			{
				output.WriteLine("Skipping {0} as the files exists", controllerFile.FullName);
			}
		}

		private CodeNamespace GenerateControllerTestCode(string ns, string name, string area, string[] actions)
		{
			String controllerName = StripControllerFrom(name);

			CodeNamespace thisNs = new CodeNamespace(ns + ".Tests");

			thisNs.Imports.Add(new CodeNamespaceImport("System"));
			thisNs.Imports.Add(new CodeNamespaceImport("NUnit.Framework"));
			thisNs.Imports.Add(new CodeNamespaceImport("Castle.MonoRail.TestSupport"));
	
			CodeTypeDeclaration controllerType = new CodeTypeDeclaration(name + "TestCase");
			thisNs.Types.Add(controllerType);

			controllerType.BaseTypes.Add("AbstractMRTestCase");
			controllerType.CustomAttributes.Add( new CodeAttributeDeclaration("TestFixture") );
	
			foreach(String action in actions)
			{
				CodeMemberMethod actionTestMethod = new CodeMemberMethod();
				actionTestMethod.Name = action;
				actionTestMethod.Attributes = MemberAttributes.Public;
				actionTestMethod.CustomAttributes.Add( new CodeAttributeDeclaration("Test") );

				String url = null;

				if (area == null || area.Length == 0)
				{
					url = String.Format("\"{0}/{1}.rails\"", controllerName, action);
				}
				else
				{
					url = String.Format("\"{0}/{1}/{2}.rails\"", area, controllerName, action);
				}
				
				String expected = string.Format("\"View for {0} action {1}\"", name, action);

				CodeMethodInvokeExpression doGetInvoke = new CodeMethodInvokeExpression();
				doGetInvoke.Method = new CodeMethodReferenceExpression(null, "DoGet");
				doGetInvoke.Parameters.Add( new CodeSnippetExpression(url) );
				actionTestMethod.Statements.Add( new CodeExpressionStatement(doGetInvoke) );

				CodeMethodInvokeExpression assertSuccess = new CodeMethodInvokeExpression();
				assertSuccess.Method = new CodeMethodReferenceExpression(null, "AssertSuccess");
				actionTestMethod.Statements.Add( new CodeExpressionStatement(assertSuccess) );

				CodeMethodInvokeExpression assertReply = new CodeMethodInvokeExpression();
				assertReply.Method = new CodeMethodReferenceExpression(null, "AssertReplyContains");
				assertReply.Parameters.Add( new CodeSnippetExpression(expected) );
				actionTestMethod.Statements.Add( new CodeExpressionStatement(assertReply) );

				controllerType.Members.Add(actionTestMethod);
			}

			return thisNs;
		}

		private string StripControllerFrom(string value)
		{
			if (value.EndsWith("controller"))
			{
				return value.Substring(0, value.IndexOf("controller"));
			}
			else if (value.EndsWith("Controller"))
			{
				return value.Substring(0, value.IndexOf("Controller"));
			}

			return value;
		}
	}
}

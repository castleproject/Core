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

namespace NVelocity.Test
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;

	using NUnit.Framework;
	using NVelocity.App;

	[TestFixture]
	public class StringInterpolationTestCase
	{
		[Test]
		public void InterpolationWithEquals()
		{
			VelocityContext c = new VelocityContext();
			c.Put("survey", 1 );
			c.Put("id", 2 );
			c.Put("siteRoot", String.Empty );
			c.Put("AjaxHelper2", new AjaxHelper2());
			c.Put("DictHelper", new DictHelper());
			
			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw, 
				"ContextTest.CaseInsensitive", 
				"$AjaxHelper2.LinkToRemote(\"Remove\", " + 
					"\"${siteRoot}/Participant/DeleteEmail.rails\", " + 
					"$DictHelper.CreateDict( \"failure=error\", \"success=emaillist\", \"with=return 'survey=${survey}&id=${id}'\" ) )");

			Assertion.Assert("Evalutation returned failure", ok);
			Assertion.AssertEquals("Remove /Participant/DeleteEmail.rails return 'survey=1&id=2'", sw.ToString());			
		}
	}

	public class DictHelper
	{
		public IDictionary CreateDict( params String[] args )
		{
			IDictionary dict = new HybridDictionary();

			foreach(String arg in args)
			{
				String[] parts = arg.Split('=');

				if (parts.Length == 1)
				{
					dict[arg] = "";
				}
				else
				{
					dict[ parts[0] ] = String.Join("=", parts, 1, parts.Length - 1);
				}
			}

			return dict;
		}
	}

	public class AjaxHelper2
	{
		public String LinkToRemote(String name, String url, IDictionary options)
		{
			if (options == null) throw new ArgumentNullException("options");

			return name + " " + url + " " + options["with"];
		}
	}
}

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

namespace Castle.MonoRail.Engine.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class FilesTestCase : AbstractCassiniTestCase
	{
		public FilesTestCase()
		{
		}

		[Test]
		public void Ajax()
		{
			string expected = "\r\n\r\n/*  Prototype JavaScript framework";
			string url = "/MonoRail/Files/AjaxScripts.rails";

			Execute(url, expected, url, true, "text/javascript");
		}

		[Test]
		public void Fade()
		{
			string expected = "\r\n// @name      The Fade Anything Technique";
			string url = "/MonoRail/Files/EffectsFatScripts.rails";

			Execute(url, expected, url, true, "text/javascript");
		}

		[Test]
		public void Validation()
		{
			string expected = " \r\n\t\t\t/*************************************";
			string url = "/MonoRail/Files/ValidateCore.rails";

			Execute(url, expected, url, true, "text/javascript");

			expected = " \r\n\t\t\t\tfunction fValConfig()\r\n\t\t\t\t{";
			url = "/MonoRail/Files/ValidateConfig.rails";
			Execute(url, expected, url, true, "text/javascript");

			expected = " \r\n\t\t\t/*< blank basic ******************";
			url = "/MonoRail/Files/ValidateValidators.rails";
			Execute(url, expected, url, true, "text/javascript");

			expected = " \r\n\t\t\t/*--	fValidate US-English language file.";
			url = "/MonoRail/Files/ValidateLang.rails";
			Execute(url, expected, url, true, "text/javascript");
		}
	}
}

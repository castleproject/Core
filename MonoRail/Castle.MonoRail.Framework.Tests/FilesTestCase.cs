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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Net;
	
	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;


	/// <summary>
	/// Ensures MonoRail is serving the static files
	/// </summary>
	[TestFixture]
	public class FilesTestCase : AbstractMRTestCase
	{
		[Test]
		public void Ajax()
		{
			DoGet("MonoRail/Files/AjaxScripts.rails");
			
			AssertSuccess();

			String expected = "/*  Prototype JavaScript framework";

			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);
		}

		[Test]
		public void EffectsFat()
		{
			DoGet("MonoRail/Files/EffectsFatScripts.rails");

			AssertSuccess();

            String expected = "\r\n// @name      The Fade Anything Technique";

			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);
		}

		[Test]
		public void Effects()
		{
			DoGet("MonoRail/Files/Effects2.rails");

			AssertSuccess();

            String expected = "\r\n\r\n// Copyright (c) 2005 Thomas Fuchs (http://script.aculo.us, http://mir.aculo.us)";

			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);
		}

		[Test]
		public void Validation()
		{
			String expected = " \r\n\t\t\t/*************************************";
			DoGet("MonoRail/Files/ValidateCore.rails");
			AssertSuccess();
			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);

			expected = " \r\n\t\t\t\tfunction fValConfig()\r\n\t\t\t\t{";
			DoGet("MonoRail/Files/ValidateConfig.rails");
			AssertSuccess();
            AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);

			expected = " \r\n\t\t\t/*< blank basic ******************";
			DoGet("MonoRail/Files/ValidateValidators.rails");
			AssertSuccess();
			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);

			expected = " \r\n\t\t\t/*--	fValidate US-English language file.";
			DoGet("MonoRail/Files/ValidateLang.rails");
			AssertSuccess();
			AssertContentTypeStartsWith("application/x-javascript");
			AssertHasHeader("Cache-Control");
			AssertReplyStartsWith(expected);
		}
	}
}

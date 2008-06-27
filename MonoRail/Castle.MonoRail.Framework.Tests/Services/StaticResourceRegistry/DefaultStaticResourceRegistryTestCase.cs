// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry
{
	using System;
	using Castle.Core.Resource;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultStaticResourceRegistryTestCase 
	{
		private DefaultStaticResourceRegistry registry;

		[SetUp]
		public void Init()
		{
			registry = new DefaultStaticResourceRegistry();
		}

		[Test]
		public void Exists_CorrectlyMatchesLocationAndVersion()
		{
			registry.RegisterCustomResource("key", null, null, new StaticContentResource("content"), "text/javascript", null);

			Assert.IsTrue(registry.Exists("key", null, null));
			Assert.IsTrue(registry.Exists("Key", null, null));
			Assert.IsTrue(registry.Exists("KEY", null, null));

			Assert.IsFalse(registry.Exists("key2", null, null));
			Assert.IsFalse(registry.Exists("key", "neutral", null));
			Assert.IsFalse(registry.Exists("key", "pt-br", null));
			Assert.IsFalse(registry.Exists("key", null, "1"));
			Assert.IsFalse(registry.Exists("key", null, "1.0"));
		}

		[Test]
		public void GetResource_FetchesCorrectResource()
		{
			registry.RegisterCustomResource("key 1", null, null, new StaticContentResource("content 1"), "text/javascript", null);
			registry.RegisterCustomResource("key 2", null, null, new StaticContentResource("content 2"), "text/javascript", null);

			string mime;
			DateTime? lastMod;
			Assert.AreEqual("content 1", registry.GetResource("key 1", null, null, out mime, out lastMod));
			Assert.AreEqual("text/javascript", mime);
			Assert.AreEqual("content 2", registry.GetResource("key 2", null, null, out mime, out lastMod));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void RegisterAssemblyResource_FetchesCorrectResource()
		{
			registry.RegisterAssemblyResource("key 1", null, null, "Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry.DummyRes", "jsfunctions", "text/javascript", null);
			registry.RegisterAssemblyResource("key 2", null, null, "Castle.MonoRail.Framework.Tests", "Castle.MonoRail.Framework.Tests.Services.StaticResourceRegistry.DummyRes", "jsValidator", "text/javascript", null);

			string mime;
			DateTime? lastMod;
			Assert.AreEqual("Something 1", registry.GetResource("key 1", null, null, out mime, out lastMod));
			Assert.AreEqual("text/javascript", mime);
			Assert.AreEqual("validators", registry.GetResource("key 2", null, null, out mime, out lastMod));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveBehaviourScripts()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual("\r\n/*\r\n   Behaviour v1.1 by Ben Nolan, June 2005.",
				registry.GetResource("BehaviourScripts", null, null, out mime, out lastMod).Substring(0, 48));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveAjaxScripts()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual("\r\n/*  Prototype JavaScript framework, version 1.5.1\r\n *  (c)",
				registry.GetResource("AjaxScripts", null, null, out mime, out lastMod).Substring(0, 60));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveFormHelperScript()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual("\r\n\r\n\r\nfunction monorail_formhelper_numberonly(e, exceptions, forbidalso)\r\n{\r\n\tex",
				registry.GetResource("FormHelperScript", null, null, out mime, out lastMod).Substring(0, 80));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveZebdaScripts()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual(" \r\n\r\n/*\r\n\r\nZebda javascript library, version 0.3.1\r\n http://labs.cavorite.com/ze",
				registry.GetResource("ZebdaScripts", null, null, out mime, out lastMod).Substring(0, 80));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveValidateScripts()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual(" \r\n\t\t\t \r\n\t\t\t/*************************************",
				registry.GetResource("ValidateCore", null, null, out mime, out lastMod).Substring(0, 50));
			Assert.AreEqual("text/javascript", mime);

			Assert.AreEqual(" \r\n\t\t\t/*--\tfValidate US-English language file.\r\n\t\t",
				registry.GetResource("ValidateLang", null, null, out mime, out lastMod).Substring(0, 50));
			Assert.AreEqual("text/javascript", mime);

			Assert.AreEqual(" \r\n\t\t\t/*< blank basic ****************************",
				registry.GetResource("ValidateValidators", null, null, out mime, out lastMod).Substring(0, 50));
			Assert.AreEqual("text/javascript", mime);

			Assert.AreEqual(" \r\n\t\t\t\tfunction fValConfig()\r\n\t\t\t\t{\r\n\t\t\t\t\t/*\tGloba",
				registry.GetResource("ValidateConfig", null, null, out mime, out lastMod).Substring(0, 50));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveEffects2()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual("\r\n\r\n// script.aculo.us scriptaculous.js v1.7.1_beta3, Fri May 25 17:19:41 +0200 ",
				registry.GetResource("Effects2", null, null, out mime, out lastMod).Substring(0, 80));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_CanResolveEffectsFatScripts()
		{
			string mime;
			DateTime? lastMod;
			Assert.AreEqual("\r\n// @name      The Fade Anything Technique\r\n// @namespace http://www.axentric.c",
				registry.GetResource("EffectsFatScripts", null, null, out mime, out lastMod).Substring(0, 80));
			Assert.AreEqual("text/javascript", mime);
		}

		[Test]
		public void DefaultResource_ReturnsLastModified()
		{
			DateTime mod = new DateTime(1979, 07, 16);
			registry.RegisterCustomResource("key", null, null, new StaticContentResource("content"), "text/javascript", mod);

			string mime;
			DateTime? lastMod;
			registry.GetResource("key", null, null, out mime, out lastMod);
			
			Assert.IsTrue(lastMod.HasValue);
			Assert.AreEqual(mod.Year, lastMod.Value.Year);
			Assert.AreEqual(mod.Month, lastMod.Value.Month);
			Assert.AreEqual(mod.Day, lastMod.Value.Day);
		}
	}
}

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

namespace Castle.Applications.PestControl.Tests
{
	using System;

	using Bamboo.Prevalence;

	using NUnit.Framework;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Configuration.Sources;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for AbstractPestControlTestCase.
	/// </summary>
	public abstract class AbstractPestControlTestCase
	{
		protected WindsorContainer _container;
		protected PestControlModel _model;
		protected PrevalenceEngine _engine;

		public AbstractPestControlTestCase()
		{
		}

		[SetUp]
		public void Init()
		{
			_container = new PestControlContainer(new XmlInterpreter(new AppDomainConfigSource()));

			_model = (PestControlModel) _container["pestcontrolModel"];
			_engine = (PrevalenceEngine) _container["prevalenceengine"];
		}

		[TearDown]
		public void End()
		{
			_container.Dispose();
		}

	}
}

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

using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Model.Resource;

namespace Castle.Applications.PestControl.Tests
{
	using System;
	using System.Configuration;

	using Bamboo.Prevalence;

	using NUnit.Framework;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

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
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			XmlInterpreter interpreter = new XmlInterpreter( new ConfigResource() );
			interpreter.ProcessResource(interpreter.Source, store);
			_container = new PestControlContainer(interpreter);

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

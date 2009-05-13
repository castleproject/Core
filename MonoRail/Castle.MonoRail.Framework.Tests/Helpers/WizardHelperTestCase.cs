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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Framework.Helpers;
	using Framework.Routing;
	using Framework.Services;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Test;

	[TestFixture]
	public class WizardHelperTestCase
	{
		#region Setup/Teardown

		[SetUp]
		public void Init()
		{
			helper = new WizardHelper();
			controllerContext = repository.DynamicMock<IControllerContext>();
			engineContext = repository.DynamicMock<IEngineContext>();
			controller = repository.DynamicMock<IController>();
			wizardStepPage = repository.DynamicMock<IWizardStepPage>();
		}

		#endregion

		private const string WizardKey = "wizard.";
		private const string ControllerName = "AddOptionsWizardController";
		private readonly MockRepository repository = new MockRepository();

		private WizardHelper helper;
		private IControllerContext controllerContext;
		private IEngineContext engineContext;
		private IController controller;
		private IWizardStepPage wizardStepPage;

		private void SetupWizardConfiguration(int currentStepIdx)
		{
			IDictionary sessionDictionary = new Dictionary<string, object>();
			IDictionary itemsDictionary = new Dictionary<string, object>();
			IList<string> stepsList = new List<string>();
			stepsList.Add("Step0");
			stepsList.Add("Step1");
			stepsList.Add("Step2");
			sessionDictionary.Add(WizardKey + ControllerName + "currentstepindex", currentStepIdx);
			sessionDictionary.Add(WizardKey + ControllerName + "currentstepname", "Step" + currentStepIdx);
			itemsDictionary.Add(WizardKey + "step.list", stepsList);

			SetupResult.For(engineContext.Session).Return(sessionDictionary);
			SetupResult.For(engineContext.Items).Return(itemsDictionary);
			SetupResult.For(controllerContext.Name).Return(ControllerName);
			
			repository.Replay(engineContext);
			repository.Replay(controllerContext);
			helper.SetContext(engineContext);
			helper.SetController(controller, controllerContext);
			
			helper.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), null);
			helper.CurrentUrl = new UrlInfo("Cars", "MyController", "MyAction", String.Empty, "rails");
		}

		private void SetupWizardSetupPage(int currentStepIdx)
		{
			SetupResult.For(wizardStepPage.WizardControllerContext).Return(controllerContext);
			SetupResult.For(wizardStepPage.ActionName).Return("Step" + currentStepIdx);
			repository.Replay(wizardStepPage);
		}

		private void SetupWizardController(bool useCurrentRouteForRedirects)
		{
			helper.WizardController = repository.DynamicMock<IWizardController>();
			SetupResult.For(helper.WizardController.UseCurrentRouteForRedirects).Return(useCurrentRouteForRedirects);
			repository.Replay(helper.WizardController);

			if (useCurrentRouteForRedirects)
			{
				repository.BackToRecord(controllerContext, BackToRecordOptions.None);
				RouteMatch routeMatch = new RouteMatch();
				routeMatch.AddNamed("manufacturer", "Ford");
				routeMatch.AddNamed("model", "Falcon");
				SetupResult.For(controllerContext.RouteMatch).Return(routeMatch);
				SetupResult.For(controllerContext.AreaName).Return("Cars");
				repository.Replay(controllerContext);

				RoutingEngine routingEngine = new RoutingEngine();
				routingEngine.Add(
					new PatternRoute("/<area>/<manufacturer>/AddOptionsWizard/<model>/[action]")
						.DefaultForController().Is("AddOptionsWizardController")
						.DefaultForAction().Is("start"));
				helper.UrlBuilder = new DefaultUrlBuilder(new StubServerUtility(), routingEngine);
				helper.CurrentUrl = new UrlInfo("Cars", "CarsController", "View", String.Empty, "rails");
				helper.UrlBuilder.UseExtensions = false;
			}
		}

		[Test]
		public void CurrentStepIndex()
		{
			SetupWizardConfiguration(0);
			Assert.AreEqual(0, helper.CurrentStepIndex);

			repository.BackToRecordAll();
			SetupWizardConfiguration(1);
			Assert.AreEqual(1, helper.CurrentStepIndex);

			repository.BackToRecordAll();
			SetupWizardConfiguration(2);
			Assert.AreEqual(2, helper.CurrentStepIndex);
		}

		[Test]
		public void DoesNotHaveNextStepForLastStepOfWizard()
		{
			SetupWizardConfiguration(2);
			Assert.IsFalse(helper.HasNextStep());
		}

		[Test]
		public void DoesNotHavePreviousStepForFirstStepOfWizard()
		{
			SetupWizardConfiguration(0);
			Assert.IsFalse(helper.HasPreviousStep());
		}

		[Test]
		public void HasNextStepForFirstStepOfWizard()
		{
			SetupWizardConfiguration(0);
			Assert.IsTrue(helper.HasNextStep());
		}

		[Test]
		public void HasPreviousStepForLastStepOfWizard()
		{
			SetupWizardConfiguration(2);
			Assert.IsTrue(helper.HasPreviousStep());
		}

		[Test]
		public void LinkToStep()
		{
			SetupWizardConfiguration(0);
			SetupWizardSetupPage(0);
			SetupWizardController(false);

			Assert.AreEqual("<a  href=\"/Cars/AddOptionsWizardController/Step0.rails\">Step0</a>", helper.LinkToStep("Step0", wizardStepPage));
		}

		[Test]
		public void LinkToStepWithId()
		{
			SetupWizardConfiguration(0);
			SetupWizardSetupPage(0);
			SetupWizardController(false);

			Assert.AreEqual("<a  href=\"/Cars/AddOptionsWizardController/Step0.rails?id=Ford\">Step0</a>", helper.LinkToStep("Step0", wizardStepPage, "Ford"));
		}

		[Test]
		public void LinkToStepWithIdAndExtraAttributes()
		{
			SetupWizardConfiguration(0);
			SetupWizardSetupPage(0);
			SetupWizardController(false);
			
			Assert.AreEqual("<a class=\"myclass\"  href=\"/Cars/AddOptionsWizardController/Step0.rails?id=Ford\">Step0</a>", helper.LinkToStep("Step0", wizardStepPage, "Ford", DictHelper.CreateN("class", "myclass")));
		}

		[Test]
		public void LinkToNextStep()
		{
			SetupWizardConfiguration(0);
			SetupWizardSetupPage(0);
			SetupWizardController(false);

			Assert.AreEqual("<a  href=\"/Cars/AddOptionsWizardController/Step1.rails\">Step1</a>", helper.LinkToNext("Step1"));
		}

		[Test]
		public void LinkToPreviousStep()
		{
			SetupWizardConfiguration(2);
			SetupWizardSetupPage(2);
			SetupWizardController(false);

			Assert.AreEqual("<a  href=\"/Cars/AddOptionsWizardController/Step1.rails\">Step1</a>", helper.LinkToPrevious("Step1"));
		}

		[Test]
		public void StepsNames()
		{
			SetupWizardConfiguration(1);
			Assert.AreEqual("Step1", helper.GetStepName(1));
			Assert.AreEqual("Step0", helper.GetStepName(0));
			Assert.AreEqual("Step2", helper.GetStepName(2));
		}

		[Test]
		public void StepsOrder()
		{
			SetupWizardConfiguration(1);
			Assert.AreEqual("Step1", helper.CurrentStepName);
			Assert.AreEqual("Step0", helper.PreviousStepName);
			Assert.AreEqual("Step2", helper.NextStepName);
		}

		[Test]
		public void LinkToPreviousUsingRoute()
		{
			SetupWizardConfiguration(2);
			SetupWizardSetupPage(2);
			SetupWizardController(true);

			Assert.AreEqual("<a  href=\"/Cars/Ford/AddOptionsWizard/Falcon/Step1\">Step1</a>", helper.LinkToPrevious("Step1"));
		}

		[Test]
		public void LinkToNextUsingRoute()
		{
			SetupWizardConfiguration(1);
			SetupWizardSetupPage(1);
			SetupWizardController(true);

			Assert.AreEqual("<a  href=\"/Cars/Ford/AddOptionsWizard/Falcon/Step2\">Step2</a>", helper.LinkToNext("Step2"));
		}

		[Test]
		public void LinkToStepUsingRoute()
		{
			SetupWizardConfiguration(1);
			SetupWizardSetupPage(1);
			SetupWizardController(true);

			Assert.AreEqual("<a  href=\"/Cars/Ford/AddOptionsWizard/Falcon/Step1\">Step1</a>", helper.LinkToStep("Step1", wizardStepPage));
		}
	}
}

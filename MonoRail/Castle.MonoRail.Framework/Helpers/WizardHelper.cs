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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using Internal;

	/// <summary>
	/// Provide useful helpers to be used in a layout view
	/// or in the wizards steps views.
	/// </summary>
	public class WizardHelper : UrlHelper
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardHelper"/> class.
		/// </summary>
		public WizardHelper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public WizardHelper(IEngineContext engineContext) : base(engineContext)
		{
		}

		#endregion

		/// <summary>
		/// Gets/Sets the <see cref="IWizardController"/>.
		/// </summary>
		public IWizardController WizardController { get; set; }

		/// <summary>
		/// Gets the name of the current step.
		/// </summary>
		/// <value>The name of the current step.</value>
		public int CurrentStepIndex
		{
			get { return WizardUtils.GetCurrentStepIndex(Context, Controller, ControllerContext); }
		}

		/// <summary>
		/// Returns the name of the previous step
		/// </summary>
		public string PreviousStepName
		{
			get { return WizardUtils.GetPreviousStepName(Context, Controller, ControllerContext); }
		}

		/// <summary>
		/// Gets the name of the current step.
		/// </summary>
		/// <value>The name of the current step.</value>
		public string CurrentStepName
		{
			get { return WizardUtils.GetCurrentStepName(Context, Controller, ControllerContext); }
		}

		/// <summary>
		/// Returns the name of the next step
		/// </summary>
		public string NextStepName
		{
			get { return WizardUtils.GetNextStepName(Context, Controller, ControllerContext); }
		}

		/// <summary>
		/// Returns <c>true</c> if the current wizard 
		/// flow has a next step.
		/// </summary>
		/// <returns></returns>
		public bool HasNextStep()
		{
			return WizardUtils.HasNextStep(Context, Controller, ControllerContext);
		}

		/// <summary>
		/// Returns <c>true</c> if the current wizard
		/// flow has an accessible previous step.
		/// </summary>
		/// <remarks>
		/// This will only return <c>true</c> if not
		/// the first step
		/// </remarks>
		/// <returns></returns>
		public bool HasPreviousStep()
		{
			return WizardUtils.HasPreviousStep(Context, Controller, ControllerContext);
		}

		/// <summary>
		/// Gets a wizard step name by index.
		/// </summary>
		/// <param name="stepindex">The stepindex.</param>
		/// <returns></returns>
		public string GetStepName(int stepindex)
		{
			return WizardUtils.GetStepName(stepindex, Context, Controller, ControllerContext);
		}

		private string LinkToStep(string linkText, string areaName, string controllerName, string stepName, object id, IDictionary attributes)
		{
			IDictionary parameters = DictHelper.CreateN("controller", controllerName).N("action", stepName);
			if (areaName != null)
			{
				parameters.Add("area", areaName);
			}
			if (WizardController.UseCurrentRouteForRedirects)
			{
				parameters.Add("useCurrentRouteParams", "true");
			}
			if (id != null)
			{
				parameters.Add("querystring", String.Format("id={0}", id));
			}
			return Link(linkText, parameters, attributes);
		}

		#region LinkToStep

		/// <overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates an anchor tag (link) to the specified step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="linkText">The label for the step</param>
		/// <param name="step">The WizardStepPage to link to</param>
		/// <returns></returns>
		public string LinkToStep(string linkText, IWizardStepPage step)
		{
			return LinkToStep(linkText, step, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) to the specified step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="linkText">The label for the step</param>
		/// <param name="step">The WizardStepPage to link to</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <returns></returns>
		public string LinkToStep(string linkText, IWizardStepPage step, object id)
		{
			return LinkToStep(linkText, step, id, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) to the specified step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="linkText">The label for the step</param>
		/// <param name="step">The WizardStepPage to link to</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns></returns>
		public string LinkToStep(string linkText, IWizardStepPage step, object id, IDictionary attributes)
		{
			return LinkToStep(linkText, step.WizardControllerContext.AreaName, step.WizardControllerContext.Name, step.ActionName, id, attributes);
		}

		#endregion

		#region LinkToNext and LinkToPrevious

		/// <overloads>This method has four overloads.</overloads>
		/// <summary>
		/// Creates an anchor tag (link) to the next step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a next step. It's advised 
		/// that you use <see cref="HasNextStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <returns></returns>
		public string LinkToNext(string linkText)
		{
			return LinkToNext(linkText, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) to the next step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a next step. It's advised 
		/// that you use <see cref="HasNextStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns></returns>
		public string LinkToNext(string linkText, IDictionary attributes)
		{
			return LinkToNext(linkText, null, attributes);
		}

		/// <summary>
		/// Creates an anchor tag (link) with an id attribute to the next step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails?Id=id&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a next step. It's advised 
		/// that you use <see cref="HasNextStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <returns></returns>
		public string LinkToNext(string linkText, object id)
		{
			return LinkToNext(linkText, id, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) with an id attribute to the next step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails?Id=id&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a previous step. It's advised 
		/// that you use <see cref="HasNextStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns></returns>
		public string LinkToNext(string linkText, object id, IDictionary attributes)
		{
			return LinkToStep(linkText, ControllerContext.AreaName, ControllerContext.Name, NextStepName, id, attributes);
		}

		/// <overloads>This method has four overloads.</overloads>
		/// <summary>
		/// Creates an anchor tag (link) to the previous step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a previous step. It's advised 
		/// that you use <see cref="HasPreviousStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <returns></returns>
		public string LinkToPrevious(string linkText)
		{
			return LinkToPrevious(linkText, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) to the previous step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a previous step. It's advised 
		/// that you use <see cref="HasPreviousStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns></returns>
		public string LinkToPrevious(string linkText, IDictionary attributes)
		{
			return LinkToPrevious(linkText, null, attributes);
		}

		/// <summary>
		/// Creates an anchor tag (link) with an id attribute to the previous step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails?Id=id&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a previous step. It's advised 
		/// that you use <see cref="HasPreviousStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <returns></returns>
		public string LinkToPrevious(string linkText, object id)
		{
			return LinkToPrevious(linkText, id, null);
		}

		/// <summary>
		/// Creates an anchor tag (link) with an id attribute to the previous step.
		/// <code>
		/// &lt;a href=&quot;/page2.rails?Id=id&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <remarks>
		/// This helper assumes there is a previous step. It's advised 
		/// that you use <see cref="HasPreviousStep"/> before calling this
		/// </remarks>
		/// <param name="linkText">The label for the link</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns></returns>
		public string LinkToPrevious(string linkText, object id, IDictionary attributes)
		{
			return LinkToStep(linkText, ControllerContext.AreaName, ControllerContext.Name, PreviousStepName, id, attributes);
		}

		#endregion
	}
}

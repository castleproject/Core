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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Provide useful helpers to be used in a layout view
	/// or in the wizards steps views.
	/// </summary>
	public class WizardHelper : HtmlHelper
	{
		/// <summary>
		/// Returns <c>true</c> if the current wizard 
		/// flow has a next step.
		/// </summary>
		/// <returns></returns>
		public bool HasNextStep()
		{
			return WizardUtils.HasNextStep(Controller);
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
			return WizardUtils.HasPreviousStep(Controller);
		}

		/// <summary>
		/// Returns the name of the previous step
		/// </summary>
		public String PreviousStepName
		{
			get { return WizardUtils.GetPreviousStepName(Controller); }
		}

		/// <summary>
		/// Returns the name of the next step
		/// </summary>
		public String NextStepName
		{
			get { return WizardUtils.GetNextStepName(Controller); }
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
		public String LinkToStep(String linkText, WizardStepPage step)
		{
			return LinkTo( linkText, step.WizardController.Name, step.ActionName);
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
		public String LinkToStep(String linkText, WizardStepPage step, object id)
		{
			return LinkTo( linkText, step.WizardController.Name, step.ActionName, id);
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
		public String LinkToStep(String linkText, WizardStepPage step, object id, IDictionary attributes)
		{
			return LinkToAttributed( linkText, step.WizardController.Name, step.ActionName, id, attributes);
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
		public String LinkToNext(String linkText)
		{
			return LinkTo( linkText, Controller.Name, NextStepName );
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
		public String LinkToNext(String linkText, IDictionary attributes)
		{
			return LinkToAttributed( linkText, Controller.Name, NextStepName, attributes );
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
		public String LinkToNext(String linkText, object id)
		{
			return LinkTo( linkText, Controller.Name, NextStepName, id );
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
		public String LinkToNext(String linkText, object id, IDictionary attributes)
		{
			return LinkToAttributed( linkText, Controller.Name, NextStepName, id, attributes );
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
		public String LinkToPrevious(String linkText)
		{
			return LinkTo( linkText, Controller.Name, PreviousStepName );
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
		public String LinkToPrevious(String linkText, IDictionary attributes)
		{
			return LinkToAttributed( linkText, Controller.Name, PreviousStepName, attributes );
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
		public String LinkToPrevious(String linkText, object id)
		{
			return LinkTo( linkText, Controller.Name, PreviousStepName, id );
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
		public String LinkToPrevious(String linkText, object id, IDictionary attributes)
		{
			return LinkToAttributed( linkText, Controller.Name, PreviousStepName, id, attributes );
		}

		#endregion
	}
}

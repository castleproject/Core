// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Test
{
	using System.Collections;
	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// Mocks the <see cref="IEmailTemplateService"/> calling 
	/// <see cref="MockRailsEngineContext.AddMailTemplateRendered"/> to register
	/// the calls made to these methods
	/// </summary>
	public class MockEmailTemplateService : IEmailTemplateService
	{
		private readonly MockRailsEngineContext context;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockEmailTemplateService"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public MockEmailTemplateService(MockRailsEngineContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">Name of the template to load.
		/// Will look in <c>Views/mail</c> for that template file.</param>
		/// <param name="parameters">Dictionary with parameters
		/// that you can use on the email template</param>
		/// <param name="doNotApplyLayout">If <c>true</c>, it will skip the layout</param>
		/// <returns>An instance of <see cref="Message"/></returns>
		public Message RenderMailMessage(string templateName, IDictionary parameters, bool doNotApplyLayout)
		{
			context.AddMailTemplateRendered(templateName, parameters);

			return new Message("from", "to", "subject", "body");
		}

		/// <summary>
		/// Renders the mail message.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="doNotApplyLayout">if set to <c>true</c> [do not apply layout].</param>
		/// <returns></returns>
		public Message RenderMailMessage(string templateName, IRailsEngineContext engineContext, IController controller,
		                                 bool doNotApplyLayout)
		{
			context.AddMailTemplateRendered(templateName, controller.PropertyBag);

			return new Message("from", "to", "subject", "body");
		}
	}
}

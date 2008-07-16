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

namespace Castle.MonoRail.Views.AspView
{
    using System;
    using System.Collections.Generic;
    using Framework.Helpers;

    public class ViewAtDesignTime : System.Web.UI.Page
    {
        #region mocking AspViewBase
        public Dictionary<string, object> Properties
        { get { throw new ShouldNotBeImplemented(); } }
        public void OutputSubView(string subViewName)
        { throw new ShouldNotBeImplemented(); }
        public void OutputSubView(string subViewName, IDictionary<string, object> parameters)
        { throw new ShouldNotBeImplemented(); }
        public string ViewContents
        { get { throw new ShouldNotBeImplemented(); } }
        public void Output(string message) { }
        public void Output(string message, params object[] arguments) { }

		public IHelpersAccesor Helpers
		{ get { throw new ShouldNotBeImplemented(); } }
		#region obsolete helper accessors
		[Obsolete("Use Helpers.Ajax instead")]
        public AjaxHelper AjaxHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Dict instead")]
		protected DictHelper DictHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Scriptaculous instead")]
		protected ScriptaculousHelper ScriptaculousHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Effects instead")]
		protected EffectsFatHelper EffectsFatHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Form instead")]
		protected FormHelper FormHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Html instead")]
		protected HtmlHelper HtmlHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Pagination instead")]
		protected PaginationHelper PaginationHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Validation instead")]
		protected ValidationHelper ValidationHelper
        { get { throw new ShouldNotBeImplemented(); } }
		[Obsolete("Use Helpers.Wizard instead")]
		protected WizardHelper WizardHelper
        { get { throw new ShouldNotBeImplemented(); } }
		#endregion
		#endregion

		public class ShouldNotBeImplemented : NotImplementedException
        {
            public ShouldNotBeImplemented() :
                base("This method is a mock for intellisense purposes only. It should not be called in runtime through this class or any of it's successors")
            {}
        }
    }
}

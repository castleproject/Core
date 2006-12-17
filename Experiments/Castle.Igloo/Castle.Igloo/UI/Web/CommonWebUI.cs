#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Castle.Igloo.UIComponents;
using Castle.Windsor;
using Castle.Igloo.Util;

namespace Castle.Igloo.UI.Web
{
    
    public sealed class CommonWebUI
    {

        /// <summary>
        /// Does injection on the WebUI control.
        /// </summary>
        /// <param name="sender">The source of the event, in this case it' a Control.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public static void WebUI_InjectComponent(object sender, EventArgs e)
        {
            //Control control = (Control) sender;

            IWindsorContainer container = ContainerWebAccessorUtil.Container;
            UIComponentRepository repository = container.Resolve<UIComponentRepository>();

            UIComponent uiComponent = repository.ComponentForType(sender.GetType().BaseType);

            uiComponent.Inject(sender);
        }

        /// <summary>
        /// Try to retrieve Action of the WebUI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public static void WebUI_RetrieveAction(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            
            // Try to set the command name on the state object
            // Also see IButtonControl
            Control webControl = null;
            bool findControl = false;
            HttpRequest request = WebUtil.GetCurrentHttpContext().Request;

            foreach (string controllName in request.Form)
            {
                if (controllName != null)
                {
                    string name = controllName;
                    if (name.EndsWith(".x"))
                    {
                        name = name.Substring(0, name.Length - 2);
                    }
                    webControl = control.FindControl(name);
                    if (webControl is IPostBackEventHandler)
                    {
                        findControl = true;
                        break;
                    }
                }
            }
            // Another try
            if (!findControl && request.Form["__EVENTTARGET"] != null)
            {
                webControl = control.FindControl(request.Form["__EVENTTARGET"].Replace(control.UniqueID + ":", ""));
            }

            // The Control.ViewState property is associated with each server control 
            // in your web form 
            // The commandName is in the control.ViewState["CommandName"]
            // wich is protected :-(
            if (webControl != null)
            {
                // Think we can also use the IButtonControl interface which contains the CommandName
                PropertyInfo propertyInfo = typeof(Control).GetProperty("ViewState", BindingFlags.Instance
                    | BindingFlags.IgnoreReturn
                    | BindingFlags.Public
                    | BindingFlags.NonPublic);
                StateBag statebag = propertyInfo.GetValue(webControl, null) as StateBag;

                (control as IView).NavigationContext.Action = statebag["CommandName"] as string;
            }
        }

    }
}

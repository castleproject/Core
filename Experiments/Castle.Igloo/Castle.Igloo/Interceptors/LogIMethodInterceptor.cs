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

using System.Diagnostics;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Castle.Igloo.Controllers;

namespace Castle.Igloo.Interceptors
{

    public class LogIMethodInterceptor : IMethodInterceptor
    {

        #region Membres de IMethodInterceptor

        /// <summary>
        /// Method invoked by the proxy in order to allow
        /// the interceptor to do its work before and after
        /// the actual invocation.
        /// </summary>
        /// <param name="invocation">The invocation holds the details of this interception</param>
        /// <param name="args">The original method arguments</param>
        /// <returns>The return value of this invocation</returns>
        public object Intercept(IMethodInvocation invocation, params object[] args)
        {
            string traceOutput = null;
            object result = null;
            IController controller = invocation.InvocationTarget as IController;

            try
            {
                //Trace.Write("Current view : " + controller.Navigator.NavigationContext.CurrentView + "\n");
                traceOutput = ExtractTraceInfo(invocation, args);
                Trace.Write(traceOutput);
            }
            catch
            {
                //silently fails
            }
            finally
            {
                result = invocation.Proceed(args);
                //Trace.Write("Next view : " + controller.Navigator.NavigationContext.CurrentView + "\n");
            }

            return result;
        }

        #endregion


        /// <summary>
        /// Extracts the trace info.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        private string ExtractTraceInfo(IMethodInvocation invocation, object[] args)
        {
            StringBuilder traceInfo = new StringBuilder();
            MethodInfo methodInfo = invocation.MethodInvocationTarget;

            ParameterInfo[] param = methodInfo.GetParameters();
            traceInfo.Append("Method Name : ");
            traceInfo.Append(methodInfo.Name);
            for (int i = 0; i < args.Length; i++)
            {
                traceInfo.AppendFormat(", Parameter {0}: {1}", param[i].Name, args[i]);
            }
            traceInfo.AppendFormat("\n");
            return traceInfo.ToString();
        }
    }
}

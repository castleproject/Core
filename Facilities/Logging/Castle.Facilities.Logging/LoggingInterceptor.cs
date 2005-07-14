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

namespace Castle.Facilities.Logging
{
	using System;
    using System.Text;

	using Castle.Model;
	using Castle.Model.Interceptor;
	using Castle.Services.Logging;

    [Transient]
	public class LoggingInterceptor : IMethodInterceptor, IOnBehalfAware
	{
		private ComponentModel _target;
        private ILogger _log;

		public LoggingInterceptor(ILoggerFactory factory)
		{
            this._log = factory.Create(this.GetType());
		}

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			_target = target;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
            object result = null;
            String logoutput = null;
            try
            {
                logoutput = ExtractLogInfo(invocation, args);
                
                this._log.Debug(logoutput);

            }
            catch
            {
                //silently fails?
            }
            finally
            {
                result = invocation.Proceed(args);
            }
            return result;
		}

	    private string ExtractLogInfo(IMethodInvocation invocation, object[] args)
	    {
	        StringBuilder loginfo = new StringBuilder();         
	        loginfo.Append("Method Name: ");
	        loginfo.Append(invocation.Method.Name);
	        for (int i = 0; i < args.Length; i++)
	        {
	            loginfo.AppendFormat(", Parameter {0}: {1}", i, args[i]);   
	        }

            return loginfo.ToString();
	    }
	}
}

using Castle.MicroKernel;
using Castle.Model;
using Castle.Services.Logging;
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

	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// 
	/// </summary>
	public class LoggingFacility : AbstractFacility
	{
		public LoggingFacility()
		{
		}

		protected override void Init()
		{
            //Get some config information
            //ideally just a string to a log4net / NLog config file.

            //setup log4net/NLog and get rocking

            Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
            Kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);

            Kernel.AddComponent("logging.logger.default", typeof(ILogger), typeof(NullLogger));
		}

        private void OnComponentModelCreated(ComponentModel model) {
            ////////////////////////
            //for attributal logging
            bool logable;
            logable = false /*= model.Implementation.GetCustomAttributes(typeof(Logable), true).Length > 0*/;

            model.ExtendedProperties["logable"] = logable;

            if(logable)
            {
                //add to loggable things to watch or something
            }

            ///////////////////////////
            //For Constructor Injection
            foreach(DependencyModel d in model.Dependencies)
            {
                if(d.TargetType == typeof(ILogger))
                {
                    //check the config if a logger type is specified
                    //if not give it the default.
                }
            }

        }

        private void OnComponentRegistered(String key, IHandler handler) {
            //attributal logging
            bool logable = (bool) handler.ComponentModel.ExtendedProperties["logable"];
            //don't realy know what to do here
        }

	}
}

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

#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.ActiveRecord.Framework;
using System.Configuration;
using Castle.Model.Configuration;

namespace Castle.ActiveRecord.Tests.Config
{
    [TestFixture]
    public class AspNet2ConfigurationIntegration
    {
        [Test]
        public void GetConnectionStringFromWebConfig()
        {
            IConfigurationSource source =
                ConfigurationSettings.GetConfig("activerecord-asp-net-2.0") as IConfigurationSource;

            IConfiguration config = source.GetConfiguration(typeof(ActiveRecordBase));

            string expected = config.Children["hibernate.connection.connection_string"].Value;
            
            Assert.AreEqual("Test Connection String", expected);
        }
    }
}
#endif
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

namespace Castle.Facilities.SecurityManagement.Tests
{
    using System;
    using System.Security;
    using System.Security.Principal;
    using System.Threading;
    using NUnit.Framework;
    using Castle.Windsor;
	using Castle.MicroKernel.SubSystems.Configuration;

    /// <summary>
	/// Summary description for SecurityTests.
	/// </summary>
	[TestFixture]
	public class SecurityTests
	{
		public SecurityTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        [Test]
        [ExpectedException(typeof(SecurityException))]
        public void DenyMoron()
        {
            
            IWindsorContainer container = this.CreateConfiguredContainer();
            container.AddFacility("security", new SecurityFacility());
            container.AddComponent("test", typeof(SecureComponent));

            SecureComponent s = container["test"] as SecureComponent;

            LoginAsMoron();
            s.DenyMethod();
        }

        [Test]
        public void DenyAdmin()
        {
            
            IWindsorContainer container = this.CreateConfiguredContainer();
            container.AddFacility("security", new SecurityFacility());
            container.AddComponent("test", typeof(SecureComponent));

            SecureComponent s = container["test"] as SecureComponent;
            
            LoginAsAdmin();
            s.DenyMethod();
        }

        [Test]
        [ExpectedException(typeof(SecurityException))]
        public void AllowMoron()
        {
            
            IWindsorContainer container = this.CreateConfiguredContainer();
            container.AddFacility("security", new SecurityFacility());
            container.AddComponent("test", typeof(SecureComponent));

            SecureComponent s = container["test"] as SecureComponent;

            LoginAsMoron();
            s.AllowMethod();
        }

        [Test]
        public void AllowAdmin()
        {
            
            IWindsorContainer container = this.CreateConfiguredContainer();
            container.AddFacility("security", new SecurityFacility());
            container.AddComponent("test", typeof(SecureComponent));

            SecureComponent s = container["test"] as SecureComponent;
            
            LoginAsAdmin();
            s.AllowMethod();
        }

        private void LoginAsAdmin()
        {
            this.Login(new String[] { "admin" });
        }
        private void LoginAsMoron()
        {
            this.Login(new String[] { "moron" });
        }
        private void Login(String[] roles)
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("dru"), roles);
        }

        private IWindsorContainer CreateConfiguredContainer()
        {
            IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());
            return container;
        }
	}
}

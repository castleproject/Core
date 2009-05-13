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

#if !SILVERLIGHT // we do not support xml config on SL

namespace Castle.Windsor.Tests.Proxy
{
	using System;
	using Castle.Core.Interceptor;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	public interface ICalculatorFactory
	{
		ICalcService Create(String id);        
		void Release(ICalcService calculator);
	}

    public interface ICalculatorFactoryCreateWithoutId
    {
        ICalcService Create();        
    }
    
	[TestFixture]
	public class TypedFactoryFacilityTestCase
	{
		[Test]
		public void TypedFactory_WithProxies_WorksFine()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/typedFactory.xml"));

			ICalculatorFactory calcFactory = (ICalculatorFactory) container.Resolve(typeof(ICalculatorFactory));
			Assert.IsNotNull(calcFactory);

			ICalcService calculator = calcFactory.Create("default");
			Assert.IsNotNull(calculator as IProxyTargetAccessor);
			Assert.AreEqual(3, calculator.Sum(1, 2));

			calcFactory.Release(calculator);
		}

        [Test]        
        public void TypedFactory_CreateMethodHasNoId_WorksFine()
		{
			IWindsorContainer container;

            container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/typedFactoryCreateWithoutId.xml"));

            ICalculatorFactoryCreateWithoutId calcFactory = (ICalculatorFactoryCreateWithoutId)container.Resolve(typeof(ICalculatorFactoryCreateWithoutId));
			Assert.IsNotNull(calcFactory);

            ICalcService calculator = calcFactory.Create();
            Assert.IsNotNull(calculator);
			Assert.AreEqual(3, calculator.Sum(1, 2));
		}
	}
}

#endif
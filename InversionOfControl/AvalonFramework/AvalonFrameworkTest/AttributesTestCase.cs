// Copyright 2003-2004 The Apache Software Foundation
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
// limitations under the License.namespace Apache.Avalon.Framework.Test{	using System;	using NUnit.Framework;	using Apache.Avalon.Framework;		[TestFixture]	public class AvalonAttributeTest	{		[Test]		public void Service()		{			Type type = typeof(IDisposable);			AvalonServiceAttribute service = new AvalonServiceAttribute( type );			Assertion.AssertEquals( type, service.ServiceType );		}		[Test]		public void Dependency()		{			string name="test";			Type type = typeof(IDisposable);			AvalonDependencyAttribute dependency = new AvalonDependencyAttribute(type,name,Optional.False);			Assertion.AssertEquals( type, dependency.DependencyType );			Assertion.AssertEquals( name, dependency.Key );			Assertion.AssertEquals( false, dependency.IsOptional );			dependency = new AvalonDependencyAttribute(type,null,Optional.True);			Assertion.AssertEquals( type, dependency.DependencyType );			Assertion.AssertEquals( type.Name, dependency.Key );			Assertion.AssertEquals( true, dependency.IsOptional );		}		[Test]		public void Component()		{			string name="test";			AvalonComponentAttribute component = new AvalonComponentAttribute( name, Lifestyle.Singleton );			Assertion.AssertNotNull( component );			Assertion.AssertEquals( name, component.Name );			Assertion.AssertEquals( Lifestyle.Singleton, component.Lifestyle );		}	}}
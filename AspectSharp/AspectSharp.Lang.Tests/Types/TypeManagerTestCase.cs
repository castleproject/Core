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

namespace AspectSharp.Lang.Tests.Types
{
	using System;
	using System.Reflection;

	using AspectSharp.Lang.Steps.Types.Resolution;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for TypeManagerTestCase.
	/// </summary>
	[TestFixture]
	public class TypeManagerTestCase
	{
		[Test]
		public void InspectAppDomainAssemblies()
		{
			TypeManager manager = new TypeManager();
			manager.InspectAppDomainAssemblies();

			Type stringType = manager.ResolveType( "System.String" );
			Assert.AreEqual( typeof(String), stringType );

			stringType = manager.ResolveType( "String" );
			Assert.AreEqual( typeof(String), stringType );
		}

		[Test]
		public void AddAssembly()
		{
			TypeManager manager = new TypeManager();
			manager.AddAssembly( Assembly.GetAssembly( typeof(TypeManagerTestCase) ) );

			Assert.IsNull( manager.ResolveType( "System.String" ) );
			Assert.AreEqual( typeof(TypeManagerTestCase), manager.ResolveType( "TypeManagerTestCase" ) );
		}
	}
}

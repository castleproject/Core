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

namespace Castle.Rook.Compiler.Tests.TypeContainerTests
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.Compiler.TypeSystem;

	[TestFixture]
	public class TypeContainerTestCase : AbstractContainerTestCase
	{
		[Test]
		public void TypeLookup()
		{
			DefaultTypeContainer typeContainer = (DefaultTypeContainer) 
				container[ typeof(ITypeContainer) ];

			Type consoleType = typeContainer.LookupType( "System::Console" );
			Assert.IsNotNull( consoleType );

			consoleType = typeContainer.LookupType( "System.Console" );
			Assert.IsNotNull( consoleType );

			Type bitArrayType = typeContainer.LookupType( "System.Collections.BitArray" );
			Assert.IsNotNull( bitArrayType );

			bitArrayType = typeContainer.LookupType( "System::Collections::BitArray" );
			Assert.IsNotNull( bitArrayType );

			bitArrayType = typeContainer.LookupType( "System.Collections.BitArray" );
			Assert.IsNotNull( bitArrayType );

			bitArrayType = typeContainer.LookupType( "System.Collections::BitArray" );
			Assert.IsNotNull( bitArrayType );

			bitArrayType = typeContainer.LookupType( "System::Collections.BitArray" );
			Assert.IsNotNull( bitArrayType );
		}
	}
}

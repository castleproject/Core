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

namespace Castle.Rook.Compiler.Tests
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using Castle.Rook.Compiler.TypeGraph;


	[TestFixture]
	public class TypeGraphSpaceTestCase
	{
		[Test]
		public void AddingAssembly()
		{
			TypeGraphSpace graph = new TypeGraphSpace();
			graph.AddAssemblyReference( "mscorlib" );
			graph.AddAssemblyReference( "System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );

			NamespaceGraph ng = graph.GetNamespace("System");
			Assert.IsNotNull( ng );
			Assert.AreEqual( "System", ng.Name );

			ng = graph.GetNamespace("System::Collections::Specialized");
			Assert.IsNotNull( ng );
			Assert.AreEqual( "Specialized", ng.Name );

			ng = graph.GetNamespace("System::Diagnostics");
			Assert.IsNotNull( ng );
			Assert.AreEqual( "Diagnostics", ng.Name );
		}

		[Test]
		public void UsingRequires()
		{
			TypeGraphSpace graph = new TypeGraphSpace();
			graph.AddAssemblyReference( "mscorlib" );
			graph.AddAssemblyReference( "System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );

			TypeGraphSpace sub = new TypeGraphSpace(graph);
			sub.AddRequire( "System" );
			sub.AddRequire( "System::Diagnostics" );

			TypeDefinition type = sub.GetType( "Console" );
			Assert.IsNotNull( type );
			Assert.AreEqual( "Console", type.Name );

			type = sub.GetType( "System::Console" );
			Assert.IsNotNull( type );
			Assert.AreEqual( "Console", type.Name );

			type = sub.GetType( "Debug" );
			Assert.IsNotNull( type );
			Assert.AreEqual( "Debug", type.Name );

			type = sub.GetType( "System::Diagnostics::Debug" );
			Assert.IsNotNull( type );
			Assert.AreEqual( "Debug", type.Name );
		}

		[Test]
		public void CheckAmbiguities()
		{
			TypeGraphSpace graph = new TypeGraphSpace();
			graph.AddAssemblyReference( "mscorlib" );
			graph.AddAssemblyReference( "System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );
			graph.AddAssemblyReference( "Castle.Rook.Compiler.Tests" );

			TypeGraphSpace sub = new TypeGraphSpace(graph);
			sub.AddRequire( "System" );
			sub.AddRequire( "Castle::Rook::Compiler::Tests" );

			Assert.IsFalse( sub.HasAmbiguity("System::Console") ); 

			TypeDefinition type = sub.GetType( "System::Console" );
			Assert.IsNotNull( type );
			Assert.AreEqual( "Console", type.Name );

			Assert.IsTrue( sub.HasAmbiguity("Console") ); 
		}
	}

	/// <summary>
	/// Just to force an ambiguity
	/// </summary>
	public class Console
	{
		
	}
}

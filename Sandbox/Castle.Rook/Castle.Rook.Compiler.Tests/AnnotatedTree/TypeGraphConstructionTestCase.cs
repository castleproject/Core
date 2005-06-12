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

namespace Castle.Rook.Compiler.Tests.AnnotatedTree
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services.Passes;
	using Castle.Rook.Compiler.TypeGraph;


	[TestFixture]
	public class TypeGraphConstructionTestCase : AbstractContainerTestCase
	{
		[Test]
		public void NamespaceAndClasses()
		{
			String contents = 
				"namespace Family::Guy      \r\n" + 
				"							\r\n" + 
				"  class Boat				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"  class Ship				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"end						\r\n" + 
				"							\r\n" + 
				"";

			SourceUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit.CompilationUnit);

			AssertNoErrorOrWarnings();

			TypeGraphSpace graph = unit.SymbolTable.Parent.TypeGraphView;

			TypeDefinition boatType = graph.GetType("Family::Guy::Boat");
			Assert.IsNotNull(boatType);
			Assert.IsNotNull(boatType as TransientType);
			Assert.IsNotNull((boatType as TransientType).TypeDef);
			Assert.AreEqual("Boat", (boatType as TransientType).TypeDef.Name);

			TypeDefinition shipType = graph.GetType("Family::Guy::Ship");
			Assert.IsNotNull(shipType);
			Assert.IsNotNull(shipType as TransientType);
			Assert.IsNotNull((shipType as TransientType).TypeDef);
			Assert.AreEqual("Ship", (shipType as TransientType).TypeDef.Name);
		}

		[Test]
		public void Classes()
		{
			String contents = 
				"							\r\n" + 
				"  class Boat				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"  class Ship				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"";

			SourceUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit.CompilationUnit);

			AssertNoErrorOrWarnings();

			TypeGraphSpace graph = unit.SymbolTable.Parent.TypeGraphView;

			TypeDefinition boatType = graph.GetType("Castle::Rook::Code::Boat");
			Assert.IsNotNull(boatType);
			Assert.IsNotNull(boatType as TransientType);
			Assert.IsNotNull((boatType as TransientType).TypeDef);
			Assert.AreEqual("Boat", (boatType as TransientType).TypeDef.Name);

			TypeDefinition shipType = graph.GetType("Castle::Rook::Code::Ship");
			Assert.IsNotNull(shipType);
			Assert.IsNotNull(shipType as TransientType);
			Assert.IsNotNull((shipType as TransientType).TypeDef);
			Assert.AreEqual("Ship", (shipType as TransientType).TypeDef.Name);
		}

		[Test]
		public void NamespaceClassesAndMethods()
		{
			String contents = 
				"namespace Family::Guy      \r\n" + 
				"							\r\n" + 
				"  class Boat				\r\n" + 
				"							\r\n" + 
				"	 def initialize			\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"	 def save				\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"	 def update				\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"	 def self.find_all		\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"  class Ship				\r\n" + 
				"							\r\n" + 
				"	 def initialize			\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"	 def self.remove_all	\r\n" + 
				"    end	   				\r\n" + 
				"							\r\n" + 
				"  end						\r\n" + 
				"							\r\n" + 
				"end						\r\n" + 
				"							\r\n" + 
				"";

			SourceUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);

			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

			sb.ExecutePass(unit.CompilationUnit);

			AssertNoErrorOrWarnings();

			TypeGraphSpace graph = unit.SymbolTable.Parent.TypeGraphView;

			TypeDefinition boatType = graph.GetType("Family::Guy::Boat");
			Assert.IsNotNull(boatType);
			Assert.IsNotNull(boatType as TransientType);
			Assert.IsNotNull((boatType as TransientType).TypeDef);
			Assert.AreEqual("Boat", (boatType as TransientType).TypeDef.Name);

			TypeDefinition shipType = graph.GetType("Family::Guy::Ship");
			Assert.IsNotNull(shipType);
			Assert.IsNotNull(shipType as TransientType);
			Assert.IsNotNull((shipType as TransientType).TypeDef);
			Assert.AreEqual("Ship", (shipType as TransientType).TypeDef.Name);
		}
	}
}

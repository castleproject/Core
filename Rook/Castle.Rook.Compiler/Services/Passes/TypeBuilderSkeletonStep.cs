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

namespace Castle.Rook.Compiler.Services.Passes
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.TypeSystem;
	using Castle.Rook.Compiler.Visitors;


	public class TypeBuilderSkeletonStep : DepthFirstVisitor
	{
		private readonly ModuleBuilder modBuilder;
		private readonly ITypeContainer container;
		private readonly INameResolver resolver;
		private readonly IErrorReport errorReport;
		private readonly Queue toProcess = new Queue();

		private String currentNamespace;
		private String currentFileName;
		private bool enableErrorReporting;

		public TypeBuilderSkeletonStep(ModuleBuilder modBuilder, ITypeContainer container, 
			INameResolver resolver, IErrorReport errorReport)
		{
			this.modBuilder = modBuilder;
			this.container = container;
			this.resolver = resolver;
			this.errorReport = errorReport;
		}

		public override bool VisitSourceUnit(SourceUnit unit)
		{
			currentFileName = unit.Filename;

			return base.VisitSourceUnit(unit);
		}

		public override bool VisitEnter(NamespaceDescriptor ns)
		{
			currentNamespace = ns.Name;

			return base.VisitEnter(ns);
		}

		public override bool VisitLeave(NamespaceDescriptor ns)
		{
			currentNamespace = null;

			return base.VisitLeave(ns);
		}

		public override bool VisitLeave(TypeDefinitionStatement typeDef)
		{
			if (toProcess.Count != 0)
			{
				ReProcessTypesOnQueue();
			}

			return base.VisitLeave(typeDef);
		}

		public override void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			base.VisitCompilationUnit(compilationUnit);

			while (toProcess.Count != 0)
			{
				int lastCount = toProcess.Count;

				ReProcessTypesOnQueue();

				if (lastCount == toProcess.Count)
				{
					// Something that couldn't be resolved!
					
					enableErrorReporting = true;

					ReProcessTypesOnQueue();

					break;
				}
			}
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			typeDef.Name = BuildTypeName(typeDef);

			if (ResolveBaseType(typeDef))
			{
				CreateAndRegisterTypeBuilder(typeDef);
			}
			else
			{
				// Enqueue

				toProcess.Enqueue( typeDef );
			}

			base.VisitTypeDefinitionStatement(typeDef);

			return true;
		}

		private void CreateAndRegisterTypeBuilder(TypeDefinitionStatement typeDef)
		{
			Type baseType = null;
			IList interfaces = new ArrayList();

			foreach(TypeReference typeRef in typeDef.BaseTypes)
			{
				if (typeRef.ResolvedType.IsClass)
				{
					if (baseType != null)
					{
						// Hey we do not support multiple inheritance

						errorReport.Error( 
							currentFileName, 
							typeDef.Position, 
							"We do not support multiple inheritance.. at least yet" );

						return;
					}
					else
					{
						baseType = typeRef.ResolvedType;
					}
				}
				else if (typeRef.ResolvedType.IsInterface)
				{
					interfaces.Add( typeRef.ResolvedType );
				}
				else
				{
					errorReport.Error( 
						currentFileName, 
						typeDef.Position, 
						"You're trying to extend from a type that is neither a class nor interface" );

					return;
				}
			}

			TypeBuilder typeBuilder = null;

			if (baseType == null)
			{
				typeBuilder = modBuilder.DefineType( typeDef.Name, 
					TypeAttributes.Class|TypeAttributes.Public|TypeAttributes.Serializable );
			}
			else
			{
				typeBuilder = modBuilder.DefineType( typeDef.Name, 
					TypeAttributes.Class|TypeAttributes.Public|TypeAttributes.Serializable, baseType );
			}

			typeDef.ParentType = baseType != null ? baseType : typeof(Object);

			foreach(Type interfaceType in interfaces)
			{
				typeBuilder.AddInterfaceImplementation( interfaceType );
				typeDef.Interfaces.Add( interfaceType );
			}

			typeDef.Builder = typeBuilder;
	
			if (typeBuilder.Namespace == null || typeBuilder.Namespace == String.Empty)
			{
				container.AddUserType( typeBuilder );
			}
			else
			{
				container.GetNamespace( typeBuilder.Namespace ).AddUserType( typeBuilder );
			}
		}

		private bool ResolveBaseType(TypeDefinitionStatement typeDef)
		{
			bool weAreHappy = true;

			foreach(TypeReference typeRef in typeDef.BaseTypes)
			{
				if (!resolver.Resolve( typeRef ))
				{
					if (enableErrorReporting)
					{
						errorReport.Error( "TODO:FILENAME", typeDef.Position, 
							"Could not resolve type {0}. Are you forgetting about " + 
							"a reference to an assembly?", typeRef.TypeName );
					}

					weAreHappy = false;
				}
			}

			return weAreHappy;
		}

		private string BuildTypeName(TypeDefinitionStatement def)
		{
			if (currentNamespace != null)
			{
				return String.Format("{0}.{1}", def.Name);
			}

			return def.Name;
		}

		private bool ReProcessTypesOnQueue()
		{
			object head = null;

			for(;;)
			{
				if (toProcess.Count == 0) break;

				TypeDefinitionStatement typeDef = (TypeDefinitionStatement) toProcess.Dequeue();

				if (head == null)
				{
					head = typeDef; 
				}
				else if (typeDef == head)
				{
					// Back to the first

					toProcess.Enqueue(typeDef);
					break;
				}

				if (ResolveBaseType(typeDef))
				{
					CreateAndRegisterTypeBuilder(typeDef);
				}
				else
				{
					toProcess.Enqueue(typeDef);
				}
			}

			return (toProcess.Count == 0);
		}
	}
}
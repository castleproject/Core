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

namespace Castle.ActiveRecord.Generator.Components.CodeGenerator
{
	using System;
	using System.CodeDom;

	using Castle.ActiveRecord.Generator.Components.Database;


	public class CodeDomGenerator : ICodeDomGenerator
	{
		private INamingService _namingService;

		public CodeDomGenerator(INamingService namingService)
		{
			_namingService = namingService;
		}

		#region ICodeDomGenerator Members

		public CodeTypeDeclaration Generate(IActiveRecordDescriptor descriptor)
		{
			if (descriptor.ClassName == null) return null;

			CodeTypeDeclaration declaration = new CodeTypeDeclaration(descriptor.ClassName);
			declaration.Attributes |= MemberAttributes.Public;

			if (descriptor is ActiveRecordBaseDescriptor)
			{
				
			}
			else
			{
				ActiveRecordDescriptor arDescriptor = (ActiveRecordDescriptor) descriptor;

				if (arDescriptor is ActiveRecordDescriptorSubClass)
				{
					declaration.BaseTypes.Add( new CodeTypeReference( (arDescriptor as ActiveRecordDescriptorSubClass).BaseClass.ClassName ) );
				}
				else
				{
					declaration.BaseTypes.Add( new CodeTypeReference("ActiveRecordBase") );
				}

				AddActiveRecordAttribute(declaration, arDescriptor);

				AddProperties(arDescriptor, declaration);

				AddRelations(arDescriptor, declaration);

				AddOperations(arDescriptor, declaration);
			}

			return declaration;
		}

		#endregion

		private void AddProperties(ActiveRecordDescriptor arDescriptor, CodeTypeDeclaration declaration)
		{
			foreach(ActiveRecordPropertyDescriptor property in arDescriptor.Properties)
			{
				if (!property.Generate) continue;

				CodeMemberProperty memberProperty = CreatePropertyMember(property, declaration);

				AddAppropriateAttribute(memberProperty, property);

				declaration.Members.Add( memberProperty );
			}
		}

		private CodeMemberProperty CreatePropertyMember(ActiveRecordPropertyDescriptor property, CodeTypeDeclaration declaration)
		{
			String targetTypeName = null;

			if (property.PropertyType != null)
			{
				targetTypeName = property.PropertyType.FullName;
			}
			else if (property is ActiveRecordPropertyRelationDescriptor)
			{
				targetTypeName = (property as ActiveRecordPropertyRelationDescriptor).TargetType.ClassName;
			}
			else
			{
				throw new ApplicationException("Could not resolve property type");
			}


			String fieldName = _namingService.CreateFieldName(property.PropertyName);
	
			declaration.Members.Add( new CodeMemberField(targetTypeName, fieldName ) );
	
			CodeMemberProperty memberProperty = new CodeMemberProperty();
			memberProperty.Attributes = MemberAttributes.Public|MemberAttributes.Final;
			memberProperty.Name = property.PropertyName;
			memberProperty.Type = new CodeTypeReference(targetTypeName);
	
			CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression( 
				new CodeThisReferenceExpression(), fieldName );
	
			memberProperty.GetStatements.Add( 
				new CodeMethodReturnStatement( fieldReference ) );
			memberProperty.SetStatements.Add( 
				new CodeAssignStatement(fieldReference, 
				                        new CodeArgumentReferenceExpression("value") ) );
			return memberProperty;
		}

		private void AddAppropriateAttribute(CodeMemberProperty memberProperty, ActiveRecordPropertyDescriptor property)
		{
			bool needExplicitColumnName = (String.Compare(property.ColumnName, property.PropertyName, true) != 0);

			CodeAttributeDeclaration codeAttribute = null;

			if (property is ActiveRecordPrimaryKeyDescriptor)
			{
				codeAttribute = new CodeAttributeDeclaration("PrimaryKey"); 
				codeAttribute.Arguments.Add( new CodeAttributeArgument( 
					new CodeSnippetExpression("PrimaryKeyType.Native") ) );
			
				if (needExplicitColumnName)
				{
					codeAttribute.Arguments.Add( new CodeAttributeArgument(
						new CodeSnippetExpression(Quote(property.ColumnName)) ) );
				}
			}
			else 
			{
				codeAttribute = new CodeAttributeDeclaration("Property"); 

				if (!property.Insert)
				{
					codeAttribute.Arguments.Add( new CodeAttributeArgument("Insert", 
						new CodeSnippetExpression("false") ) );
				}
				if (!property.Update)
				{
					codeAttribute.Arguments.Add( new CodeAttributeArgument("Update", 
						new CodeSnippetExpression("false") ) );
				}

				if (needExplicitColumnName)
				{
					codeAttribute.Arguments.Add( new CodeAttributeArgument("Column",
						new CodeSnippetExpression(Quote(property.ColumnName)) ) );
				}
			}

			memberProperty.CustomAttributes.Add( codeAttribute );
		}

		private void AddActiveRecordAttribute(CodeTypeDeclaration declaration, ActiveRecordDescriptor descriptor)
		{
			CodeAttributeDeclaration att = new CodeAttributeDeclaration("ActiveRecord");

			if (!(descriptor is ActiveRecordDescriptorSubClass))
			{
				att.Arguments.Add( new CodeAttributeArgument( 
					new CodeSnippetExpression(Quote(descriptor.Table.Name)) ) );

				if (descriptor.DiscriminatorField != null && descriptor.DiscriminatorField.Length != 0)
				{
					att.Arguments.Add( new CodeAttributeArgument( "DiscriminatorColumn",
						new CodeSnippetExpression(Quote(descriptor.DiscriminatorField)) ) );
				}
				if (descriptor.DiscriminatorType != null && descriptor.DiscriminatorType.Length != 0)
				{
					att.Arguments.Add( new CodeAttributeArgument( "DiscriminatorType",
						new CodeSnippetExpression(Quote(descriptor.DiscriminatorType)) ) );
				}
			}

			if (descriptor.DiscriminatorValue != null && descriptor.DiscriminatorValue.Length != 0)
			{
				att.Arguments.Add( new CodeAttributeArgument( "DiscriminatorValue",
					new CodeSnippetExpression(Quote(descriptor.DiscriminatorValue)) ) );
			}


			declaration.CustomAttributes.Add( att );
		}

		private void AddRelations(ActiveRecordDescriptor descriptor, CodeTypeDeclaration declaration)
		{
			foreach(ActiveRecordPropertyRelationDescriptor property in descriptor.PropertiesRelations)
			{
				if (!property.Generate) continue;

				CodeAttributeDeclaration att = null;
				CodeMemberProperty memberProperty = CreatePropertyMember(property, declaration);;

				if (property is ActiveRecordBelongsToDescriptor)
				{
					att = new CodeAttributeDeclaration("BelongsTo");

					att.Arguments.Add( new CodeAttributeArgument(new CodeSnippetExpression(Quote(property.ColumnName)) ) );
				}
				else if (property is ActiveRecordHasAndBelongsToManyDescriptor)
				{
					att = new CodeAttributeDeclaration("HasAndBelongsToMany");

					ActiveRecordHasAndBelongsToManyDescriptor hasAndBelongsProp = property as ActiveRecordHasAndBelongsToManyDescriptor;

					att.Arguments.Add( new CodeAttributeArgument(new CodeTypeOfExpression(property.TargetType.ClassName) ) );

//					att.Arguments.Add( new CodeAttributeArgument(new CodeSnippetExpression("RelationType.Bag") ) );

//					att.Arguments.Add( new CodeAttributeArgument("Key", new CodeSnippetExpression(Quote(property.PropertyName)) ) );

					att.Arguments.Add( new CodeAttributeArgument("Table", new CodeSnippetExpression(Quote(property.TargetType.Table.Name)) ) );

					att.Arguments.Add( new CodeAttributeArgument("ColumnRef", new CodeSnippetExpression(Quote(property.ColumnName)) ) );
					
					att.Arguments.Add( new CodeAttributeArgument("ColumnKey", new CodeSnippetExpression(Quote(hasAndBelongsProp.ColumnKey)) ) );
				}
				else if (property is ActiveRecordHasManyDescriptor)
				{
					att = new CodeAttributeDeclaration("HasMany");

					att.Arguments.Add( new CodeAttributeArgument(new CodeTypeOfExpression(property.TargetType.ClassName) ) );

//					att.Arguments.Add( new CodeAttributeArgument(new CodeSnippetExpression("RelationType.Bag") ) );

					att.Arguments.Add( new CodeAttributeArgument("Table", new CodeSnippetExpression(Quote(property.TargetType.Table.Name)) ) );

					att.Arguments.Add( new CodeAttributeArgument("ColumnKey", new CodeSnippetExpression(Quote(property.ColumnName)) ) );
				}
				else if (property is ActiveRecordHasOneDescriptor)
				{
					att = new CodeAttributeDeclaration("HasOne");
				}

				if (property.Cascade != null && property.Cascade != String.Empty)
				{
					att.Arguments.Add( new CodeAttributeArgument("Cascade", new CodeSnippetExpression(Quote(property.Cascade)) ) );
				}
				if (property.Where != null && property.Where != String.Empty)
				{
					att.Arguments.Add( new CodeAttributeArgument("Where", new CodeSnippetExpression(Quote(property.Where)) ) );
				}
				if (property.OrderBy != null && property.OrderBy != String.Empty)
				{
					att.Arguments.Add( new CodeAttributeArgument("OrderBy", new CodeSnippetExpression(Quote(property.OrderBy)) ) );
				}
				if (property.OuterJoin != null && !"auto".Equals(property.OuterJoin))
				{
					att.Arguments.Add( new CodeAttributeArgument("OuterJoin", new CodeSnippetExpression(Quote(property.OuterJoin)) ) );
				}
				if (property.Proxy == true)
				{
					att.Arguments.Add( new CodeAttributeArgument("Proxy", new CodeSnippetExpression("true") ) );
				}
				if (property.Inverse == true)
				{
					att.Arguments.Add( new CodeAttributeArgument("Inverse", new CodeSnippetExpression("true") ) );
				}

				memberProperty.CustomAttributes.Add(att);
				declaration.Members.Add(memberProperty);
			}
		}

		private void AddOperations(ActiveRecordDescriptor descriptor, CodeTypeDeclaration declaration)
		{
			CodeMemberMethod deleteAll = new CodeMemberMethod();
			deleteAll.Name = "DeleteAll";
			deleteAll.Attributes = MemberAttributes.Static|MemberAttributes.Public;
			deleteAll.Statements.Add( new CodeExpressionStatement(
				new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("ActiveRecordBase"), 
					"DeleteAll", new CodeTypeOfExpression(descriptor.ClassName) ) ));
			declaration.Members.Add(deleteAll);

			CodeMemberMethod findAll = new CodeMemberMethod();
			findAll.Name = "FindAll";
			findAll.ReturnType = new CodeTypeReference( descriptor.ClassName, 1 );
			findAll.Attributes = MemberAttributes.Static|MemberAttributes.Public;
			findAll.Statements.Add( new CodeMethodReturnStatement(
				new CodeCastExpression(findAll.ReturnType,
				new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("ActiveRecordBase"), 
				"FindAll", new CodeTypeOfExpression(descriptor.ClassName) ) )));
			declaration.Members.Add(findAll);

			CodeMemberMethod find = new CodeMemberMethod();

			ActiveRecordPrimaryKeyDescriptor primaryKey = descriptor.PrimaryKeyProperty;
			if (primaryKey != null)
			{
				find.Name = "Find";
				find.ReturnType = new CodeTypeReference( descriptor.ClassName );
				find.Attributes = MemberAttributes.Static|MemberAttributes.Public;
				find.Parameters.Add( new CodeParameterDeclarationExpression(primaryKey.PropertyType, primaryKey.PropertyName) );
				find.Statements.Add( new CodeMethodReturnStatement(
					new CodeCastExpression(find.ReturnType,
					new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("ActiveRecordBase"), 
					"FindByPrimaryKey", new CodeTypeOfExpression(descriptor.ClassName), new CodeArgumentReferenceExpression(primaryKey.PropertyName) ) )));
				declaration.Members.Add(find);
			}
		}

		private static String Quote(String value)
		{
			return String.Format("\"{0}\"", value);
		}
	}
}

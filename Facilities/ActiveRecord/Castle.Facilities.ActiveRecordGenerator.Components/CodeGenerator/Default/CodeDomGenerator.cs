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

namespace Castle.Facilities.ActiveRecordGenerator.CodeGenerator.Default
{
	using System;
	using System.CodeDom;

	using Castle.Facilities.ActiveRecordGenerator.Model;


	public class CodeDomGenerator : ICodeDomGenerator
	{
		public CodeDomGenerator()
		{
		}

		#region ICodeGenerator Members

		public CodeCompileUnit Generate(Project project)
		{
			return null;
		}

		public CodeTypeDeclaration Generate(ActiveRecordDescriptor arDescriptor)
		{
			CodeTypeDeclaration declaration = new CodeTypeDeclaration(arDescriptor.ClassName);

			declaration.Attributes |= MemberAttributes.Public;
			declaration.BaseTypes.Add( new CodeTypeReference("ActiveRecordBase") );
			declaration.CustomAttributes.Add( new CodeAttributeDeclaration("ActiveRecord") );
			
			foreach(ActiveRecordPropertyDescriptor property in arDescriptor.Properties)
			{
				if (!property.Generate) continue;

				declaration.Members.Add( new CodeMemberField(property.PropertyType, property.PropertyFieldName) );
				
				CodeMemberProperty memberProperty = new CodeMemberProperty();
				memberProperty.Name = property.PropertyName;
				
				memberProperty.CustomAttributes.Add( 
					new CodeAttributeDeclaration("FieldMapping", 
					new CodeAttributeArgument[]
						{
							new CodeAttributeArgument(new CodeSnippetExpression("\"" + property.ColumnName + "\"") ),
						} ) );

				CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression( 
							new CodeThisReferenceExpression(), property.PropertyFieldName );

				memberProperty.GetStatements.Add( 
					new CodeMethodReturnStatement( fieldReference ) );
				memberProperty.SetStatements.Add( 
					new CodeAssignStatement(fieldReference, 
					new CodeArgumentReferenceExpression("value") ) );
				
				declaration.Members.Add( memberProperty );
			}

			return declaration;
		}

		#endregion
	}
}

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

namespace Castle.ActiveRecord.Generator.Components.Tests
{
	using System;
	using System.Collections;
	using System.Data.OleDb;

	using NUnit.Framework;

	using Castle.ActiveRecord.Generator.Components.Database;

	[TestFixture]
	public class RelationshipBuilderTestCase : AbstractContainerTestCase
	{
		[Test]
		public void BelongsTo()
		{
			InitKernel();
			IRelationshipBuilder relService = ObtainService();

			TableDefinition blogTable;
			TableDefinition postTable;

			BuildBlogPostsStructure(out blogTable, out postTable);

			ActiveRecordDescriptor desc = new ActiveRecordDescriptor("Post");
			ActiveRecordDescriptor targetDesc = new ActiveRecordDescriptor("Blog");

			RelationshipInfo info = new RelationshipInfo( AssociationEnum.BelongsTo, desc, targetDesc );
			info.ParentCol = new ColumnDefinition("blog_id", false, true, true, false, OleDbType.Numeric);

			ActiveRecordPropertyRelationDescriptor propDesc = relService.Build( info );
			Assert.IsNotNull(propDesc);
		}

		private void InitKernel()
		{
			Kernel.AddComponent( "relationsService", typeof(IRelationshipInferenceService), typeof(RelationshipInferenceService) );
			Kernel.AddComponent( "relationBuilder", typeof(IRelationshipBuilder), typeof(RelationshipBuilder) );
			Kernel.AddComponent( "nameService", typeof(INamingService), typeof(NamingService) );
			Kernel.AddComponent( "typeinf", typeof(ITypeInferenceService), typeof(TypeInferenceService) );
		}

		private IRelationshipBuilder ObtainService()
		{
			return Kernel[ typeof(IRelationshipBuilder) ] as IRelationshipBuilder;
		}

		private void BuildBlogPostsStructure(out TableDefinition blogTable, out TableDefinition postTable)
		{
			DatabaseDefinition dbdef = new DatabaseDefinition("alias");

			blogTable = new TableDefinition("blogs", dbdef );
			blogTable.AddColumn( new ColumnDefinition("id", true, false, true, false, OleDbType.Integer) );
			blogTable.AddColumn( new ColumnDefinition("name", false, false, false, false, OleDbType.VarChar) );
	
			postTable = new TableDefinition("posts", dbdef );
			postTable.AddColumn( new ColumnDefinition("id", true, false, true, false, OleDbType.Integer) );
			postTable.AddColumn( new ColumnDefinition("name", false, false, false, false, OleDbType.VarChar) );
			postTable.AddColumn( new ColumnDefinition("blog_id", false, true, false, false, OleDbType.VarChar, blogTable) );
	
			blogTable.AddManyRelation(postTable);
		}
	}
}

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
	using System.Data.OleDb;

	using NUnit.Framework;

	using Castle.ActiveRecord.Generator.Components.Database;


	[TestFixture]
	public class ActiveRecordDescriptorBuilderTestCase : AbstractContainerTestCase
	{
		[Test]
		public void BlogActiveRecord()
		{
			InitKernel();
			IActiveRecordDescriptorBuilder arbuilder = ObtainService();

			TableDefinition blogTable;
			TableDefinition postTable;

			BuildBlogPostsStructure(out blogTable, out postTable);

			BuildContext context = new BuildContext();

			ActiveRecordDescriptor desc = arbuilder.Build( blogTable, context );

			Assert.IsNotNull(desc);

			Assert.AreEqual( "Blog", desc.ClassName );
			Assert.AreEqual( 2, desc.Properties.Count );
			Assert.AreEqual( 1, desc.PropertiesRelations.Count );
		}

		private void InitKernel()
		{
			Kernel.AddComponent( "relationsService", typeof(IRelationshipInferenceService), typeof(RelationshipInferenceService) );
			Kernel.AddComponent( "nameService", typeof(INamingService), typeof(NamingService) );
			Kernel.AddComponent( "typeinf", typeof(ITypeInferenceService), typeof(TypeInferenceService) );
			Kernel.AddComponent( "plain", typeof(IPlainFieldInferenceService), typeof(PlainFieldInferenceService) );
			Kernel.AddComponent( "arbuild", typeof(IActiveRecordDescriptorBuilder), typeof(ActiveRecordDescriptorBuilder) );
		}

		private IActiveRecordDescriptorBuilder ObtainService()
		{
			return Kernel[ typeof(IActiveRecordDescriptorBuilder) ] as IActiveRecordDescriptorBuilder;
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

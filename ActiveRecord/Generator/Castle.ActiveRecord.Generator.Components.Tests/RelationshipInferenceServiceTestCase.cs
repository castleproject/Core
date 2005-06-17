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

	/// <summary>
	/// TODO: Map HasOne association
	/// </summary>
	[TestFixture]
	public class RelationshipInferenceServiceTestCase : AbstractContainerTestCase
	{
		[Test]
		public void BlogHasManyPosts()
		{
			InitKernel();
			IRelationshipInferenceService relService = ObtainService();

			TableDefinition blogTable;
			TableDefinition postTable;

			BuildBlogPostsStructure(out blogTable, out postTable);

			BuildContext context = new BuildContext();

			ActiveRecordDescriptor arDesc = new ActiveRecordDescriptor();

			ActiveRecordPropertyDescriptor[] descs = relService.InferRelations( arDesc, blogTable, context );

			Assert.IsNotNull(descs);
			Assert.AreEqual( 1, descs.Length );

			ActiveRecordHasManyDescriptor desc1 = descs[0] as ActiveRecordHasManyDescriptor;
			Assert.IsNotNull(desc1);
			Assert.IsNotNull(desc1.TargetType);
			Assert.IsNotNull(desc1.PropertyType);

			Assert.AreEqual( "Posts", desc1.PropertyName );
			Assert.AreEqual( "blog_id", desc1.ColumnName );
			Assert.AreEqual( typeof(IList), desc1.PropertyType );

			ActiveRecordDescriptor targetARDescriptor = context.GetNextPendent();
			Assert.AreSame( postTable, targetARDescriptor.Table );
		}

		[Test]
		public void PostBelongsToBlog()
		{
			InitKernel();
			IRelationshipInferenceService relService = ObtainService();

			TableDefinition blogTable;
			TableDefinition postTable;

			BuildBlogPostsStructure(out blogTable, out postTable);

			BuildContext context = new BuildContext();

			ActiveRecordDescriptor arDesc = new ActiveRecordDescriptor();

			ActiveRecordPropertyDescriptor[] descs = relService.InferRelations( arDesc, postTable, context );

			Assert.IsNotNull(descs);
			Assert.AreEqual( 1, descs.Length );

			ActiveRecordBelongsToDescriptor desc1 = descs[0] as ActiveRecordBelongsToDescriptor;
			Assert.IsNotNull(desc1);
			Assert.IsNotNull(desc1.TargetType);
			Assert.IsNull(desc1.PropertyType);

			Assert.AreEqual( "Blog", desc1.PropertyName );
			Assert.AreEqual( "blog_id", desc1.ColumnName );

			ActiveRecordDescriptor targetARDescriptor = context.GetNextPendent();
			Assert.AreSame( blogTable, targetARDescriptor.Table );
		}

		[Test]
		public void SelfReference()
		{
			InitKernel();
			IRelationshipInferenceService relService = ObtainService();

			DatabaseDefinition dbdef = new DatabaseDefinition("alias");

			TableDefinition categoryTable = new TableDefinition("categories", dbdef );
			categoryTable.AddColumn( new ColumnDefinition("id", true, false, true, false, OleDbType.Integer) );
			categoryTable.AddColumn( new ColumnDefinition("name", false, false, false, false, OleDbType.VarChar) );
			categoryTable.AddColumn( new ColumnDefinition("parent_id", false, true, false, false, OleDbType.Integer, categoryTable) );
	
			categoryTable.AddManyRelation(categoryTable);

			BuildContext context = new BuildContext();

			ActiveRecordDescriptor arDesc = new ActiveRecordDescriptor();

			ActiveRecordPropertyDescriptor[] descs = relService.InferRelations( arDesc, categoryTable, context );

			Assert.IsFalse(context.HasPendents);

			Assert.IsNotNull(descs);
			Assert.AreEqual( 2, descs.Length );

			ActiveRecordHasManyDescriptor desc1 = descs[0] as ActiveRecordHasManyDescriptor;
			Assert.IsNotNull(desc1);
			Assert.IsNotNull(desc1.TargetType);
			Assert.IsNotNull(desc1.PropertyType);

			Assert.AreEqual( "Categories", desc1.PropertyName );
			Assert.AreEqual( "parent_id", desc1.ColumnName );
			Assert.AreEqual( typeof(IList), desc1.PropertyType );

			ActiveRecordBelongsToDescriptor desc2 = descs[1] as ActiveRecordBelongsToDescriptor;
			Assert.IsNotNull(desc2);
			Assert.IsNotNull(desc2.TargetType);
			Assert.IsNull(desc2.PropertyType);
			Assert.AreEqual( "Category", desc2.PropertyName );
			Assert.AreEqual( "parent_id", desc2.ColumnName );
		}

		private void InitKernel()
		{
			Kernel.AddComponent( "relationsService", typeof(IRelationshipInferenceService), typeof(RelationshipInferenceService) );
			Kernel.AddComponent( "nameService", typeof(INamingService), typeof(NamingService) );
			Kernel.AddComponent( "typeinf", typeof(ITypeInferenceService), typeof(TypeInferenceService) );
		}

		private IRelationshipInferenceService ObtainService()
		{
			return Kernel[ typeof(IRelationshipInferenceService) ] as IRelationshipInferenceService;
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

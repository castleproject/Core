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
	public class PlainFieldInferenceServiceTestCase : AbstractContainerTestCase
	{
		[Test]
		public void BlogPlainFields()
		{
			Kernel.AddComponent( "plainfields", typeof(IPlainFieldInferenceService), typeof(PlainFieldInferenceService) );
			Kernel.AddComponent( "nameService", typeof(INamingService), typeof(NamingService) );
			Kernel.AddComponent( "typeinf", typeof(ITypeInferenceService), typeof(TypeInferenceService) );

			IPlainFieldInferenceService plainService = Kernel[ typeof(IPlainFieldInferenceService) ] as IPlainFieldInferenceService;

			TableDefinition table = new TableDefinition("blogs", new DatabaseDefinition("alias") );
			table.AddColumn( new ColumnDefinition("id", true, false, true, false, OleDbType.Integer) );
			table.AddColumn( new ColumnDefinition("name", false, false, false, false, OleDbType.VarChar) );
			table.AddColumn( new ColumnDefinition("authorid", false, true, false, false, OleDbType.VarChar) );

			ActiveRecordPropertyDescriptor[] descs = plainService.InferProperties( table );

			Assert.IsNotNull(descs);
			Assert.AreEqual( 2, descs.Length );

			ActiveRecordPropertyDescriptor desc1 = descs[0];
			ActiveRecordPropertyDescriptor desc2 = descs[1];

			Assert.AreEqual( "id", desc1.ColumnName );
			Assert.AreEqual( "Integer", desc1.ColumnTypeName );
			Assert.AreEqual( "Id", desc1.PropertyName );
			Assert.AreEqual( typeof(int), desc1.PropertyType );

			Assert.AreEqual( "name", desc2.ColumnName );
			Assert.AreEqual( "VarChar", desc2.ColumnTypeName );
			Assert.AreEqual( "Name", desc2.PropertyName );
			Assert.AreEqual( typeof(String), desc2.PropertyType );
		}
	}
}

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

namespace Castle.ActiveRecord.Generator.Components
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Generator.Components.Database;


	public class RelationshipInferenceService : IRelationshipInferenceService
	{
		private INamingService _namingService;

		public RelationshipInferenceService(INamingService namingService)
		{
			_namingService = namingService;
		}

		#region IRelationShipInferenceService Members

		public ActiveRecordPropertyRelationDescriptor[] InferRelations(TableDefinition tableDef, BuildContext context)
		{
			ArrayList list = new ArrayList();

			CreateHasManyRelations(tableDef, list, context);

			CreateBelongsToRelations(tableDef, list, context);

			return (ActiveRecordPropertyRelationDescriptor[]) list.ToArray( typeof(ActiveRecordPropertyRelationDescriptor) );
		}

		private void CreateBelongsToRelations(TableDefinition tableDef, IList list, BuildContext context)
		{
			foreach(ColumnDefinition col in tableDef.Columns)
			{
				if (col.RelatedTable != null)
				{
					String propertyName = _namingService.CreateClassName(col.RelatedTable.Name);
					ActiveRecordDescriptor targetType = null;

					if (col.RelatedTable.RelatedDescriptor == null && col.RelatedTable != tableDef)
					{
						col.RelatedTable.RelatedDescriptor = new ActiveRecordDescriptor(col.RelatedTable);

						context.AddPendentDescriptor(col.RelatedTable.RelatedDescriptor);
					}
					
					targetType = col.RelatedTable.RelatedDescriptor;


					ActiveRecordBelongsToDescriptor belongsTo = 
						new ActiveRecordBelongsToDescriptor(col.Name, 
							propertyName, targetType);

					list.Add(belongsTo);
				}
			}
		}

		private void CreateHasManyRelations(TableDefinition tableDef, IList list, BuildContext context)
		{
			foreach(TableDefinition fkTable in tableDef.TablesReferencedByHasRelation)
			{
				String propertyName = _namingService.CreateRelationName(fkTable.Name);
				Type propertyType = typeof(IList);
				ActiveRecordDescriptor targetType = null;
				String colName = null;

				foreach(ColumnDefinition col in fkTable.Columns)
				{
					if (col.RelatedTable == tableDef)
					{
						colName = col.Name;

						if (col.RelatedTable.RelatedDescriptor == null && col.RelatedTable != fkTable)
						{
							col.RelatedTable.RelatedDescriptor = new ActiveRecordDescriptor(fkTable);

							context.AddPendentDescriptor(col.RelatedTable.RelatedDescriptor);
						}
						else if (col.RelatedTable == fkTable)
						{
							targetType = col.RelatedTable.RelatedDescriptor;
						}

						targetType = col.RelatedTable.RelatedDescriptor;

						break;
					}
				}

				// Just to protect ourselves from awkward conditions
				if (colName == null) continue;

				ActiveRecordHasManyDescriptor hasMany = 
					new ActiveRecordHasManyDescriptor(colName, propertyName, propertyType, targetType);
				
				list.Add(hasMany);
			}
		}

		#endregion
	}
}

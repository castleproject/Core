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

		public ActiveRecordPropertyDescriptor[] InferRelations(TableDefinition tableDef, IList typesToBeCreated)
		{
			ArrayList list = new ArrayList();

			CreateHasManyRelations(tableDef, list, typesToBeCreated);

			CreateBelongsToRelations(tableDef, list, typesToBeCreated);

			return (ActiveRecordPropertyDescriptor[]) list.ToArray( typeof(ActiveRecordPropertyDescriptor) );
		}

		private void CreateBelongsToRelations(TableDefinition tableDef, IList list, IList typesToBeCreated)
		{
			foreach(ColumnDefinition col in tableDef.Columns)
			{
				if (col.RelatedTable != null)
				{
					String propertyName = _namingService.CreateClassName(col.RelatedTable.Name);
					ActiveRecordDescriptor propertyType = null;

					if (col.RelatedTable.RelatedDescriptor == null)
					{
						col.RelatedTable.RelatedDescriptor = new ActiveRecordDescriptor(col.RelatedTable);

						typesToBeCreated.Add(col.RelatedTable.RelatedDescriptor);
					}
					else
					{
						propertyType = col.RelatedTable.RelatedDescriptor;
					}

					ActiveRecordBelongsToDescriptor belongsTo = 
						new ActiveRecordBelongsToDescriptor(col.Name, 
							col.Type.ToString(), propertyName, propertyType);

					list.Add(belongsTo);
				}
			}
		}

		private void CreateHasManyRelations(TableDefinition tableDef, IList list, IList typesToBeCreated)
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

						if (col.RelatedTable.RelatedDescriptor == null)
						{
							col.RelatedTable.RelatedDescriptor = new ActiveRecordDescriptor(fkTable);

							typesToBeCreated.Add(col.RelatedTable.RelatedDescriptor);
						}
						else
						{
							targetType = col.RelatedTable.RelatedDescriptor;
						}

						break;
					}
				}

				// Just to protect ourselves from awkward conditions
				if (colName == null) continue;

				ActiveRecordHasManyDescriptor hasMany = 
					new ActiveRecordHasManyDescriptor(colName, propertyName, propertyType, "Bag", targetType);
				
				list.Add(hasMany);
			}
		}

		#endregion
	}
}

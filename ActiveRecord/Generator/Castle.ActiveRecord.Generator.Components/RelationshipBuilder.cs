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


	public class RelationshipBuilder : IRelationshipBuilder
	{
		private INamingService _namingService;

		public RelationshipBuilder(INamingService namingService)
		{
			_namingService = namingService;
		}

		#region IRelationshipBuilder Members

		public ActiveRecordPropertyRelationDescriptor Build(RelationshipInfo info)
		{
			ActiveRecordPropertyRelationDescriptor desc = null;

			if (info.Association == AssociationEnum.BelongsTo)
			{
				desc = CreateBelongsToRelation(info);
			}
			else if (info.Association == AssociationEnum.HasMany)
			{
				desc = CreateHasManyRelation(info);
			}
			else if (info.Association == AssociationEnum.HasAndBelongsToMany)
			{
				desc = CreateHasManyAndBelongsToRelation(info);
			}

			PopulateInfoIntoDescriptor(info, desc);

			return desc;
		}
		
		#endregion

		private ActiveRecordPropertyRelationDescriptor CreateBelongsToRelation(RelationshipInfo info)
		{
			// Validates first
	
			if (info.ParentCol == null)
			{
				throw new ArgumentException("No parent column specified");
			}
	
			String colName = info.ParentCol.Name;
			String propName = _namingService.CreatePropertyName(colName);
	
			return new ActiveRecordBelongsToDescriptor(colName, propName, info.TargetDescriptor);
		}

		private void PopulateInfoIntoDescriptor(RelationshipInfo info, ActiveRecordPropertyRelationDescriptor desc)
		{
			desc.Insert = info.Insert;
			desc.Update = info.Update;
			desc.Proxy = info.UseProxy;

			// TODO: Finish this
		}

		private ActiveRecordPropertyRelationDescriptor CreateHasManyRelation(RelationshipInfo info)
		{
			// Validates first
	
			if (info.ChildCol == null)
			{
				throw new ArgumentException("No column specified");
			}
	
			String colName = info.ChildCol.Name;
			String propName = _namingService.CreateRelationName( info.TargetDescriptor.ClassName );
	
			return new ActiveRecordHasManyDescriptor(colName, propName, typeof(IList), info.TargetDescriptor);
		}

		private ActiveRecordPropertyRelationDescriptor CreateHasManyAndBelongsToRelation(RelationshipInfo info)
		{
			// Validates first
	
			if (info.ParentCol == null)
			{
				throw new ArgumentException("No parent column specified");
			}
			if (info.ChildCol == null)
			{
				throw new ArgumentException("No child column specified");
			}
			if (info.AssociationTable == null)
			{
				throw new ArgumentException("No association table specified");
			}
	
			String colName = info.ParentCol.Name;
			String colKeyName = info.ChildCol.Name;
			String propName = _namingService.CreateRelationName( info.TargetDescriptor.ClassName );
	
			return new ActiveRecordHasAndBelongsToManyDescriptor(colName, 
				info.AssociationTable.Name, propName, info.TargetDescriptor, colKeyName);
		}
	}
}

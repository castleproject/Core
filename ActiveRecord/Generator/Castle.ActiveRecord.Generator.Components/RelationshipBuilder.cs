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
				// Validates first

				if (info.ParentCol == null)
				{
					throw new ArgumentException("No parent column specified");
				}

				String colName = info.ParentCol.Name;
				String propName = _namingService.CreatePropertyName(colName);

				desc = ActiveRecordBelongsToDescriptor(colName, propName, info.TargetDescriptor);
			}

			PopulateInfoIntoDescriptor(info, desc);

			return null;
		}

		#endregion

		private void PopulateInfoIntoDescriptor(RelationshipInfo info, ActiveRecordPropertyRelationDescriptor desc)
		{
			desc.Insert = info.Insert;
			desc.Update = info.Update;
			desc.Proxy = info.UseProxy;

			// TODO: Finish this
		}
	}
}

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

	public enum AssociationEnum
	{
		Undefined,
		HasMany,
		BelongsTo,
		HasAndBelongsToMany
	}

	public interface IRelationshipBuilder
	{
		ActiveRecordPropertyRelationDescriptor Build(RelationshipInfo info);
	}

	public class RelationshipInfo
	{
		private AssociationEnum association;
		private TableDefinition associationTable;
		private ColumnDefinition parentCol;
		private ColumnDefinition childCol;
		private ActiveRecordDescriptor descriptor;
		private ActiveRecordDescriptor targetDescriptor;
		private String where;
		private String orderBy;
		private String outerJoin;
		private bool insert;
		private bool update;
		private bool useProxy;
		private bool invert;
		private bool cascade;

		public RelationshipInfo(AssociationEnum association, ActiveRecordDescriptor descriptor, ActiveRecordDescriptor targetDescriptor)
		{
			if (association == AssociationEnum.Undefined) throw new ArgumentException("association");
			if (descriptor == null) throw new ArgumentNullException("descriptor");
			if (targetDescriptor == null) throw new ArgumentNullException("targetDescriptor");

			this.association = association;
			this.descriptor = descriptor;
			this.targetDescriptor = targetDescriptor;
		}

		public AssociationEnum Association
		{
			get { return association; }
		}

		public ActiveRecordDescriptor Descriptor
		{
			get { return descriptor; }
		}

		public ActiveRecordDescriptor TargetDescriptor
		{
			get { return targetDescriptor; }
		}

		public TableDefinition AssociationTable
		{
			get { return associationTable; }
			set { associationTable = value; }
		}

		public ColumnDefinition ParentCol
		{
			get { return parentCol; }
			set { parentCol = value; }
		}

		public ColumnDefinition ChildCol
		{
			get { return childCol; }
			set { childCol = value; }
		}

		public string Where
		{
			get { return where; }
			set { where = value; }
		}

		public string OrderBy
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

		public string OuterJoin
		{
			get { return outerJoin; }
			set { outerJoin = value; }
		}

		public bool UseProxy
		{
			get { return useProxy; }
			set { useProxy = value; }
		}

		public bool Invert
		{
			get { return invert; }
			set { invert = value; }
		}

		public bool Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}

		public bool Update
		{
			get { return update; }
			set { update = value; }
		}
	}
}

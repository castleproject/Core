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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Iesi.Collections;

	/// <summary>
	/// Traverse the tree checking the semantics of the relation and
	/// association. The goal is to raise clear exceptions if tips of how 
	/// to fix any error.
	/// </summary>
	public class SemanticVerifierVisitor : AbstractDepthFirstVisitor
	{
		private readonly ActiveRecordModelCollection arCollection;
		private ActiveRecordModel currentModel;

		public SemanticVerifierVisitor(ActiveRecordModelCollection arCollection)
		{
			this.arCollection = arCollection;
		}

		public override void VisitModel(ActiveRecordModel model)
		{
			currentModel = model;

			if (model.IsDiscriminatorBase && model.IsJoinedSubClassBase)
			{
				throw new ActiveRecordException( String.Format(
					"Unfortunatelly you can't have a discriminator class " + 
					"and a joined subclass at the same time - check type {0}", model.Type.FullName) );
			}

			if (model.Version != null && model.Timestamp != null)
			{
				throw new ActiveRecordException( String.Format(
					"You can't specify a version and a timestamp properties, only one of them " + 
					"- check type {0}", model.Type.FullName) );
			}

			if (model.IsDiscriminatorSubClass || model.IsJoinedSubClass)
			{
				if (model.Version != null || model.Timestamp != null)
				{
					throw new ActiveRecordException( String.Format(
						"A joined subclass or discriminator subclass can't specify a version or timestamp " + 
						"- check type {0}", model.Type.FullName) );
				}
			}

			if (model.IsJoinedSubClass && model.Key == null)
			{
				throw new ActiveRecordException( String.Format(
					"A joined subclass must specify a key property. Use the JoinedKeyAttribute to denote the shared key. " + 
					"- check type {0}", model.Type.FullName) );
			}

			if (model.IsNestedType)
			{
				if (model.Version != null || model.Timestamp != null)
				{
					throw new ActiveRecordException( String.Format(
						"A nested type is not allowed to have version or timestamped fields " + 
						"- check type {0}", model.Type.FullName) );
				}
			}

			base.VisitModel(model);
		}

		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			if (model.PrimaryKeyAtt.Column == null)
			{
				model.PrimaryKeyAtt.Column = model.Property.Name;
			}

			if (model.PrimaryKeyAtt.Generator == PrimaryKeyType.Foreign)
			{
				// Just a thought, let's see if we are a OneToOne

				if (currentModel.OneToOnes.Count != 0)
				{
					// Yes, set the one to one as param 

					OneToOneModel oneToOne = (OneToOneModel) currentModel.OneToOnes[0];

					String param = "property=" + oneToOne.Property.Name;

					if (model.PrimaryKeyAtt.Params == null)
					{
						model.PrimaryKeyAtt.Params = param;
					}
					else
					{
						model.PrimaryKeyAtt.Params += "," + param;
					}
				}
			}
		}

		public override void VisitProperty(PropertyModel model)
		{
			if (model.PropertyAtt.Column == null)
			{
				model.PropertyAtt.Column = model.Property.Name;
			}
		}

		public override void VisitKey(KeyModel model)
		{
			if (model.JoinedKeyAtt.Column == null)
			{
				model.JoinedKeyAtt.Column = model.Property.Name;
			}
		}

		public override void VisitVersion(VersionModel model)
		{
			if (model.VersionAtt.Column == null)
			{
				model.VersionAtt.Column = model.Property.Name;
			}
		}

		public override void VisitTimestamp(TimestampModel model)
		{
			if (model.TimestampAtt.Column == null)
			{
				model.TimestampAtt.Column = model.Property.Name;
			}
		}

		public override void VisitBelongsTo(BelongsToModel model)
		{
			if (model.BelongsToAtt.Column == null)
			{
				model.BelongsToAtt.Column = model.Property.Name;
			}
			if (model.BelongsToAtt.Type == null)
			{
				model.BelongsToAtt.Type = model.Property.PropertyType;
			}
		}

		public override void VisitHasMany(HasManyModel model)
		{
			model.HasManyAtt.RelationType = GuessRelation(model.Property, model.HasManyAtt.RelationType);

			if (model.HasManyAtt.RelationType == RelationType.IdBag)
			{
				throw new ActiveRecordException( String.Format(
					"You can't use idbags in a many to one association (HasMany) {0}.{1}  ", 
					model.Property.DeclaringType.Name, model.Property.Name) );
			}

			if (model.HasManyAtt.RelationType == RelationType.Map && model.HasManyAtt.Index == null)
			{
				throw new ActiveRecordException( String.Format(
					"A HasMany with type Map requires that you specify an 'Index', use the Index property {0}.{1}  ", 
					model.Property.DeclaringType.Name, model.Property.Name) );
			}

			// Infer table and column based on possible belongs to 
			// on the target class

			String table = model.HasManyAtt.Table;
			String keyColumn = model.HasManyAtt.ColumnKey;

			ActiveRecordModel target = arCollection[ model.HasManyAtt.MapType ];

			if ((table == null || keyColumn == null) && target == null)
			{
				throw new ActiveRecordException( String.Format(
					"ActiveRecord tried to infer details about the relation {0}.{1} but " + 
					"it could not find information about the specified target type {2}", 
					model.Property.DeclaringType.Name, model.Property.Name, model.HasManyAtt.MapType) );
			}

			BelongsToModel targetBtModel = null;

			if (target != null)
			{
				foreach(BelongsToModel btModel in target.BelongsTo)
				{
					if (btModel.Property.PropertyType == model.Property.DeclaringType)
					{
						targetBtModel = btModel; break;
					}
				}
			}

			if ((table == null || keyColumn == null) && targetBtModel == null)
			{
				throw new ActiveRecordException( String.Format(
					"ActiveRecord tried to infer details about the relation {0}.{1} but " + 
					"it could not find a 'BelongsTo' mapped property in the target type {2}", 
					model.Property.DeclaringType.Name, model.Property.Name, model.HasManyAtt.MapType) );
			}

			if (table == null)
			{
				table = target.ActiveRecordAtt.Table;
			}

			if (keyColumn == null)
			{
				keyColumn = targetBtModel.BelongsToAtt.Column;
			}

			model.HasManyAtt.Table = table;
			model.HasManyAtt.ColumnKey = keyColumn;
		}

		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			model.HasManyAtt.RelationType = GuessRelation(model.Property, model.HasManyAtt.RelationType);

			Type otherend = model.HasManyAtt.MapType;

			if (model.HasManyAtt.Table == null)
			{
				throw new ActiveRecordException( String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " + 
					"specify the association table - {0}.{1} " + 
					currentModel.Type.Name, model.Property.Name) );
			}

			if (model.HasManyAtt.ColumnKey == null)
			{
				throw new ActiveRecordException( String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " + 
					"specify the ColumnKey - which is the column that represents the type {0} " + 
					"on the association table - {0}.{1} " + 
					currentModel.Type.Name, model.Property.Name) );
			}

			if (model.HasManyAtt.ColumnRef == null)
			{
				throw new ActiveRecordException( String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " + 
					"specify the ColumnRef - which is the column that represents the other end '{2}' " + 
					"on the association table - {0}.{1} " + 
					currentModel.Type.Name, model.Property.Name, otherend.Name) );
			}

			if (model.HasManyAtt.RelationType == RelationType.IdBag && model.CollectionID == null)
			{
				throw new ActiveRecordException( String.Format(
					"For a many to many association (HasAndBelongsToMany) using IDBag, you need " + 
					"to specify a CollectionIDAttribute giving us more details. " + 
					"{0}.{1} " + 
					currentModel.Type.Name, model.Property.Name, otherend.Name) );
			}

			if (model.HasManyAtt.RelationType == RelationType.Map && model.HasManyAtt.Index == null)
			{
				throw new ActiveRecordException( String.Format(
					"A HasAndBelongsToMany with type Map requires that you specify an 'Index', use the Index property {0}.{1}  ", 
					model.Property.DeclaringType.Name, model.Property.Name) );
			}

			base.VisitHasAndBelongsToMany(model);
		}

		private RelationType GuessRelation(PropertyInfo property, RelationType type)
		{
			if (type == RelationType.Guess)
			{
				if (property.PropertyType == typeof(IList))
				{
					return RelationType.Bag;
				}
				else if (property.PropertyType == typeof(ISet))
				{
					return RelationType.Set;
				}
				else if (property.PropertyType == typeof(IDictionary))
				{
					return RelationType.Map;
				}
				else
				{
					throw new ActiveRecordException( String.Format(
						"Could not guess relation type for property {0}.{1}  ", 
						property.DeclaringType.Name, property.Name) );
				}
			}
			else
			{
				return type;
			}
		}
	}
}

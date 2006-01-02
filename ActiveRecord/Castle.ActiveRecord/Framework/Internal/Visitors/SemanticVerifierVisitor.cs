// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using Nullables;
	using Castle.ActiveRecord;

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

			ThrowIfDoesntHavePrimaryKey(model);

			base.VisitModel(model);
		}

		private static void ThrowIfDoesntHavePrimaryKey(ActiveRecordModel model)
		{
			if (model.IsNestedType)//nested types do not have primary keys
				return;
			//Need to make the check this way because of inheritance, where the current class doesn't have
			//a primary key but the base class does
			ActiveRecordModel tmpModel = model;
			while(tmpModel!=null && tmpModel.Ids.Count==0)
				tmpModel = tmpModel.Parent;
			if(tmpModel==null || tmpModel.Ids.Count==0)
			{
				throw new ActiveRecordException( String.Format(
					"A type must declare a primary key. " + 
						"Check type {0}", model.Type.FullName) );
			}
		}

		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			// check for composite key first
			if( model.Property.PropertyType.GetCustomAttributes( typeof( CompositeKeyAttribute ), false ).Length > 0 )
			{
				MethodInfo eq = model.Property.PropertyType.GetMethod( "Equals", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );
				MethodInfo hc = model.Property.PropertyType.GetMethod( "GetHashCode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );
				
				if( eq == null || hc == null )
				{
					throw new ActiveRecordException( string.Format( "To use type '{0}' as a composite id, you must implement Equals and GetHashCode.", model.Property.PropertyType.Name ) );
				}
				
				if( model.Property.PropertyType.IsSerializable == false )
				{
					throw new ActiveRecordException( string.Format( "To use type '{0}' as a composite id it must be Serializable.", model.Property.PropertyType.Name ) );
				}

				int keyPropAttrCount = 0;
				PropertyInfo[] compositeKeyProps = model.Property.PropertyType.GetProperties();
				foreach( PropertyInfo keyProp in compositeKeyProps )
				{
					if( keyProp.GetCustomAttributes( typeof( KeyPropertyAttribute ), false ).Length > 0 )
					{
						keyPropAttrCount++;
					}
				}
				if( keyPropAttrCount < 2 )
				{
					throw new ActiveRecordException( string.Format( "To use type '{0}' as a composite id it must have two or more properties marked with the [KeyProperty] attribute.", model.Property.PropertyType.Name ) );
				}
			}
			else
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
		}

		public override void VisitProperty(PropertyModel model)
		{
			if (model.PropertyAtt.Column == null)
			{
				model.PropertyAtt.Column = model.Property.Name;
			}

			if (typeof(INullableType).IsAssignableFrom(model.Property.PropertyType))
			{
				model.PropertyAtt.NotNull = false;

				if (model.Property.PropertyType == typeof(NullableBoolean))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableBooleanType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableByte))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableByteType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableChar))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableCharType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableDateTime))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableDateTimeType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableDecimal))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableDecimalType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableDouble))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableDoubleType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableGuid))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableGuidType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableInt16))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableInt16Type, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableInt32))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableInt32Type, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableInt64))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableInt64Type, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableSByte))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableSByteType, Nullables.NHibernate";
				}
				else if (model.Property.PropertyType == typeof(NullableSingle))
				{
					model.PropertyAtt.ColumnType = "Nullables.NHibernate.NullableSingleType, Nullables.NHibernate";
				}
			}
            if (ActiveRecordModel.GetModel(model.Property.PropertyType) != null)
            {
                throw new ActiveRecordException( String.Format(
                    "You can't use [Property] on {0}.{1} because {2} is an active record class, did you mean to use BelongTo?",
                    model.Property.DeclaringType.Name, model.Property.Name, model.Property.PropertyType.FullName));
            }
		}

		public override void VisitField(FieldModel model)
		{
			if (model.FieldAtt.Column == null)
			{
				model.FieldAtt.Column = model.Field.Name;
			}

			if (typeof(INullableType).IsAssignableFrom(model.Field.FieldType))
			{
				model.FieldAtt.NotNull = false;

				if (model.Field.FieldType== typeof(NullableBoolean))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableBooleanType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableByte))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableByteType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableChar))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableCharType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableDateTime))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableDateTimeType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableDecimal))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableDecimalType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableDouble))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableDoubleType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableGuid))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableGuidType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableInt16))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableInt16Type, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableInt32))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableInt32Type, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableInt64))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableInt64Type, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableSByte))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableSByteType, Nullables.NHibernate";
				}
				else if (model.Field.FieldType == typeof(NullableSingle))
				{
					model.FieldAtt.ColumnType = "Nullables.NHibernate.NullableSingleType, Nullables.NHibernate";
				}
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
            if (currentModel.ActiveRecordAtt.Lazy)
            {
                //Assuming that a property must have at least a single accessor
                MethodInfo accessor = model.Property.GetAccessors(true)[0];
                if (!accessor.IsVirtual)
                {
                    throw new ActiveRecordException(
                        string.Format("Property {0} must be virtual because " +
                                      "class {1} support lazy loading [ActiveRecord(Lazy=true)]", 
                                      model.Property.Name,
                                      model.Property.DeclaringType.Name));
                }
            }
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
					if (btModel.BelongsToAtt.Type == model.Property.DeclaringType ||
						btModel.Property.PropertyType == model.Property.DeclaringType)
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

			if (target != null) VisitModel(target);

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

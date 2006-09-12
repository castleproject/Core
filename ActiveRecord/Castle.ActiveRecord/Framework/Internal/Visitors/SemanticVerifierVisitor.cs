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
	using System.Text;

	using Iesi.Collections;
	
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
		private StringBuilder columnPrefix = new StringBuilder();

		public SemanticVerifierVisitor(ActiveRecordModelCollection arCollection)
		{
			this.arCollection = arCollection;
		}

		public override void VisitModel(ActiveRecordModel model)
		{
			ActiveRecordModel savedModel = currentModel;

			try
			{
				currentModel = model;

				if (model.IsDiscriminatorBase && model.IsJoinedSubClassBase)
				{
					throw new ActiveRecordException(String.Format(
						"Unfortunatelly you can't have a discriminator class " +
							"and a joined subclass at the same time - check type {0}", model.Type.FullName));
				}

				if (model.Version != null && model.Timestamp != null)
				{
					throw new ActiveRecordException(String.Format(
						"You can't specify a version and a timestamp properties, only one of them " +
							"- check type {0}", model.Type.FullName));
				}

				if (model.IsDiscriminatorSubClass || model.IsJoinedSubClass)
				{
					if (model.Version != null || model.Timestamp != null)
					{
						throw new ActiveRecordException(String.Format(
							"A joined subclass or discriminator subclass can't specify a version or timestamp " +
								"- check type {0}", model.Type.FullName));
					}
				}

				if (model.IsJoinedSubClass && model.Key == null)
				{
					throw new ActiveRecordException(String.Format(
						"A joined subclass must specify a key property. Use the JoinedKeyAttribute to denote the shared key. " +
							"- check type {0}", model.Type.FullName));
				}

				if (model.IsNestedType)
				{
					if (model.Version != null || model.Timestamp != null)
					{
						throw new ActiveRecordException(String.Format(
							"A nested type is not allowed to have version or timestamped fields " +
								"- check type {0}", model.Type.FullName));
					}
				}

				AssertHasValidKey(model);

				base.VisitModel(model);
			}
			finally
			{
				currentModel = savedModel;
			}
		}

		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			if (model.PrimaryKeyAtt.Column == null)
			{
				model.PrimaryKeyAtt.Column = model.Property.Name;
			}
				
			// Append column prefix
			model.PrimaryKeyAtt.Column = columnPrefix + model.PrimaryKeyAtt.Column;

			if (model.PrimaryKeyAtt.Generator == PrimaryKeyType.Foreign)
			{
				// Let's see if we are an OneToOne

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

		public override void VisitCompositePrimaryKey(CompositeKeyModel model)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

			Type compositeKeyClassType = model.Property.PropertyType;
			
			MethodInfo eq = compositeKeyClassType.GetMethod("Equals", flags);
			MethodInfo hc = compositeKeyClassType.GetMethod("GetHashCode", flags);

			if (eq == null || hc == null)
			{
				throw new ActiveRecordException(String.Format("To use type '{0}' as a composite id, " + 
					"you must implement Equals and GetHashCode.", model.Property.PropertyType.Name));
			}

			if (compositeKeyClassType.IsSerializable == false)
			{
				throw new ActiveRecordException(String.Format("To use type '{0}' as a composite id " + 
					"it must be marked as Serializable.", model.Property.PropertyType.Name));
			}

			int keyPropAttrCount = 0;
			
			PropertyInfo[] compositeKeyProps = compositeKeyClassType.GetProperties();
			
			foreach(PropertyInfo keyProp in compositeKeyProps)
			{
				if (keyProp.GetCustomAttributes(typeof(KeyPropertyAttribute), false).Length > 0)
				{
					keyPropAttrCount++;
				}
			}

			if (keyPropAttrCount < 2)
			{
				throw new ActiveRecordException(String.Format("To use type '{0}' as a composite " + 
					"id it must have two or more properties marked with the [KeyProperty] attribute.", model.Property.PropertyType.Name));
			}
		}

		public override void VisitProperty(PropertyModel model)
		{
			if (model.PropertyAtt.Column == null)
			{
				model.PropertyAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.PropertyAtt.Column = columnPrefix + model.PropertyAtt.Column;

			Type propertyType = model.Property.PropertyType;
			
			if (NHibernateNullablesSupport.IsNHibernateNullableType(propertyType) && 
			    (model.PropertyAtt.ColumnType == null || model.PropertyAtt.ColumnType.Length == 0))
			{
				model.PropertyAtt.NotNull = false;
				model.PropertyAtt.ColumnType = NHibernateNullablesSupport.GetITypeTypeNameForNHibernateNullable(propertyType);
			}

#if DOTNET2
			if (propertyType.IsGenericType && 
			    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
			    String.IsNullOrEmpty(model.PropertyAtt.ColumnType))
			{
				model.PropertyAtt.NotNull = false;
				model.PropertyAtt.ColumnType = ObtainNullableTypeNameForCLRNullable(propertyType);
			}
#endif

			if (ActiveRecordModel.GetModel(propertyType) != null)
			{
				throw new ActiveRecordException(String.Format(
					"You can't use [Property] on {0}.{1} because {2} is an active record class, did you mean to use BelongTo?",
					model.Property.DeclaringType.Name, model.Property.Name, propertyType.FullName));
			}
		}

		public override void VisitField(FieldModel model)
		{
			if (model.FieldAtt.Column == null)
			{
				model.FieldAtt.Column = model.Field.Name;
			}

			// Append column prefix
			model.FieldAtt.Column = columnPrefix + model.FieldAtt.Column;

			Type fieldType = model.Field.FieldType;
			
			if (NHibernateNullablesSupport.IsNHibernateNullableType(fieldType) && 
			    (model.FieldAtt.ColumnType == null || model.FieldAtt.ColumnType.Length == 0))
			{
				model.FieldAtt.NotNull = false;
				model.FieldAtt.ColumnType = NHibernateNullablesSupport.GetITypeTypeNameForNHibernateNullable(fieldType);
			}

#if DOTNET2
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
			    String.IsNullOrEmpty(model.FieldAtt.ColumnType))
			{
				model.FieldAtt.NotNull = false;
				model.FieldAtt.ColumnType = ObtainNullableTypeNameForCLRNullable(fieldType);
			}
#endif
		}

		public override void VisitKey(KeyModel model)
		{
			if (model.JoinedKeyAtt.Column == null)
			{
				model.JoinedKeyAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.JoinedKeyAtt.Column = columnPrefix + model.JoinedKeyAtt.Column;
		}

		public override void VisitVersion(VersionModel model)
		{
			if (model.VersionAtt.Column == null)
			{
				model.VersionAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.VersionAtt.Column = columnPrefix + model.VersionAtt.Column;
		}

		public override void VisitTimestamp(TimestampModel model)
		{
			if (model.TimestampAtt.Column == null)
			{
				model.TimestampAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.TimestampAtt.Column = columnPrefix + model.TimestampAtt.Column;
		}

		public override void VisitBelongsTo(BelongsToModel model)
		{
			if (currentModel.ActiveRecordAtt != null)
			{
				if (currentModel.ActiveRecordAtt.Lazy)
				{
					//Assuming that a property must have at least a single accessor
					MethodInfo accessor = model.Property.GetAccessors(true)[0];
					
					if (!accessor.IsVirtual)
					{
						throw new ActiveRecordException(
							String.Format("Property {0} must be virtual because " +
								"class {1} support lazy loading [ActiveRecord(Lazy=true)]",
							              model.Property.Name, model.Property.DeclaringType.Name));
					}
				}
			}

			if (model.BelongsToAtt.Column == null && model.BelongsToAtt.CompositeKeyColumns == null)
			{
				model.BelongsToAtt.Column = model.Property.Name;
			}

			// Append column prefix
			if (model.BelongsToAtt.Column != null)
			{
				model.BelongsToAtt.Column = columnPrefix + model.BelongsToAtt.Column;
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
				throw new ActiveRecordException(String.Format(
					"You can't use idbags in a many to one association (HasMany) {0}.{1}  ",
					model.Property.DeclaringType.Name, model.Property.Name));
			}

			if (model.HasManyAtt.RelationType == RelationType.Map && model.HasManyAtt.Index == null)
			{
				throw new ActiveRecordException(String.Format(
					"A HasMany with type Map requires that you specify an 'Index', use the Index property {0}.{1}  ",
					model.Property.DeclaringType.Name, model.Property.Name));
			}

			// Infer table and column based on possible belongs to 
			// on the target class

			String table = model.HasManyAtt.Table;
			String keyColumn = model.HasManyAtt.ColumnKey;
			String[] compositeKeyColumnKeys = model.HasManyAtt.CompositeKeyColumnKeys;

			ActiveRecordModel target = arCollection[model.HasManyAtt.MapType];

			if ((table == null || (keyColumn == null && compositeKeyColumnKeys == null)) && target == null)
			{
				throw new ActiveRecordException(String.Format(
					"ActiveRecord tried to infer details about the relation {0}.{1} but " +
						"it could not find information about the specified target type {2}",
					model.Property.DeclaringType.Name, model.Property.Name, model.HasManyAtt.MapType));
			}

			BelongsToModel targetBtModel = null;

			if (target != null)
			{
				foreach(BelongsToModel btModel in target.BelongsTo)
				{
					if (btModel.BelongsToAtt.Type == model.Property.DeclaringType ||
						btModel.Property.PropertyType == model.Property.DeclaringType)
					{
						targetBtModel = btModel;
						break;
					}
				}
			}

			if ((table == null || (keyColumn == null && compositeKeyColumnKeys == null)) && targetBtModel == null)
			{
				throw new ActiveRecordException(String.Format(
					"ActiveRecord tried to infer details about the relation {0}.{1} but " +
						"it could not find a 'BelongsTo' mapped property in the target type {2}",
					model.Property.DeclaringType.Name, model.Property.Name, model.HasManyAtt.MapType));
			}

			if (target != null) VisitModel(target);

			if (table == null)
			{
				table = target.ActiveRecordAtt.Table;
			}

			if (targetBtModel != null)
			{
				if (keyColumn == null && targetBtModel.BelongsToAtt.CompositeKeyColumns == null)
				{
					keyColumn = targetBtModel.BelongsToAtt.Column;
				}
				else
				{
					compositeKeyColumnKeys = targetBtModel.BelongsToAtt.CompositeKeyColumns;
				}
			}

			model.HasManyAtt.Table = table;

			if (keyColumn != null)
			{
				model.HasManyAtt.ColumnKey = keyColumn;
			}
			else
			{
				model.HasManyAtt.CompositeKeyColumnKeys = compositeKeyColumnKeys;
			}
		}

		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			model.HasManyAtt.RelationType = GuessRelation(model.Property, model.HasManyAtt.RelationType);

			Type otherend = model.HasManyAtt.MapType;

			if (model.HasManyAtt.Table == null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " +
						"specify the association table - {0}.{1} ",
						currentModel.Type.Name, model.Property.Name));
			}

			if (model.HasManyAtt.ColumnKey == null && model.HasManyAtt.CompositeKeyColumnKeys == null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " +
						"specify the ColumnKey or CompositeKeyColumnKeys - which is the column(s) that represents the type {0} " +
						"on the association table - {0}.{1} ",
						currentModel.Type.Name, model.Property.Name));
			}

			if (model.HasManyAtt.ColumnKey != null && model.HasManyAtt.CompositeKeyColumnKeys != null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) there should only be " +
						"a ColumnKey or an array of CompositeKeyColumnKeys, not both."));
			}

			if (model.HasManyAtt.CompositeKeyColumnKeys != null && model.HasManyAtt.CompositeKeyColumnKeys.Length < 2)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) with a CompositeKey, " +
						"there must be at least two CompositeKeyColumnKeys  - which are the columns that represent the type {0} " +
						"on the association table - {0}.{1} ",
						currentModel.Type.Name, model.Property.Name));
			}

			if (model.HasManyAtt.ColumnRef == null && model.HasManyAtt.CompositeKeyColumnRefs == null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) we need that you " +
						"specify the ColumnRef or CompositeKeyColumnRefs - which is the column(s) that represents the other end '{2}' " +
						"on the association table - {0}.{1} ",
						currentModel.Type.Name, model.Property.Name, otherend.Name));
			}

			if (model.HasManyAtt.ColumnRef != null && model.HasManyAtt.CompositeKeyColumnRefs != null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) there should only be " +
						"a ColumnRef or an array of CompositeKeyColumnRefs, not both."));
			}

			if (model.HasManyAtt.CompositeKeyColumnRefs != null && model.HasManyAtt.CompositeKeyColumnRefs.Length < 2)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) with a CompositeKey, " +
						"there must be at least two CompositeKeyColumnRefs - which are the columns that represent the other end '{2}' " +
						"on the association table - {0}.{1} ",
						currentModel.Type.Name, model.Property.Name, otherend.Name));
			}

			if (model.HasManyAtt.RelationType == RelationType.IdBag && model.CollectionID == null)
			{
				throw new ActiveRecordException(String.Format(
					"For a many to many association (HasAndBelongsToMany) using IDBag, you need " +
						"to specify a CollectionIDAttribute giving us more details. " +
						"{0}.{1} ",
						currentModel.Type.Name, model.Property.Name));
			}

			if (model.HasManyAtt.RelationType == RelationType.Map && model.HasManyAtt.Index == null)
			{
				throw new ActiveRecordException(String.Format(
					"A HasAndBelongsToMany with type Map requires that you specify an 'Index', use the Index property {0}.{1}  ",
					model.Property.DeclaringType.Name, model.Property.Name));
			}

			base.VisitHasAndBelongsToMany(model);
		}

		public override void VisitOneToOne(OneToOneModel model)
		{
			if (model.OneToOneAtt.MapType == null)
			{
				model.OneToOneAtt.MapType = model.Property.PropertyType;
			}

			base.VisitOneToOne(model);
		}

		public override void VisitNested(NestedModel model)
		{
			if (model.NestedAtt.MapType == null)
			{
				model.NestedAtt.MapType = model.Property.PropertyType;
			}

			if (model.NestedAtt.ColumnPrefix != null)
			{
				columnPrefix.Append(model.NestedAtt.ColumnPrefix);
			}

			base.VisitNested(model);

			if (model.NestedAtt.ColumnPrefix != null)
			{
				columnPrefix.Length -= model.NestedAtt.ColumnPrefix.Length;
			}
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
					throw new ActiveRecordException(String.Format(
						"Could not guess relation type for property {0}.{1}  ",
						property.DeclaringType.Name, property.Name));
				}
			}
			else
			{
				return type;
			}
		}

		private static void AssertHasValidKey(ActiveRecordModel model)
		{
			// Nested types do not have primary keys
			if (model.IsNestedType) return;

			// Need to make the check this way because of inheritance, 
			// where the current class doesn't have
			// a primary key but the base class does
			ActiveRecordModel tmpModel = model;

			while(tmpModel != null && tmpModel.PrimaryKey == null && tmpModel.CompositeKey == null)
			{
				tmpModel = tmpModel.Parent;
			}
			
			if (tmpModel != null && tmpModel.PrimaryKey != null && tmpModel.CompositeKey != null)
			{
				throw new ActiveRecordException(
					String.Format(
						"A type cannot have a primary key and a composite key at the same time. Check type {0}", 
							model.Type.FullName));
			}

			if (tmpModel == null || tmpModel.PrimaryKey == null && tmpModel.CompositeKey == null)
			{
				throw new ActiveRecordException(String.Format(
					"A type must declare a primary key. Check type {0}", model.Type.FullName));
			}
		}

#if DOTNET2
		private static String ObtainNullableTypeNameForCLRNullable(Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return underlyingType.AssemblyQualifiedName;
		}
#endif
	}
}

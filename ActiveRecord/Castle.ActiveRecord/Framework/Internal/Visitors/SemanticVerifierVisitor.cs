// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Iesi.Collections.Generic;
	using System.Collections.Generic;
	using Castle.ActiveRecord;
	using NHibernate.Id;
	using NHibernate.Persister.Entity;

	/// <summary>
	/// Traverse the tree checking the semantics of the relation and
	/// association. The goal is to raise clear exceptions with tips of how 
	/// to fix any error.
	/// It also tries to infer as much information from the class / attribute model as possible so it can
	/// complete the missing information without the user needing to specify it.
	/// </summary>
	public class SemanticVerifierVisitor : AbstractDepthFirstVisitor
	{
		private readonly ActiveRecordModelCollection arCollection;
		private readonly StringBuilder columnPrefix = new StringBuilder();
		private ActiveRecordModel currentModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemanticVerifierVisitor"/> class.
		/// </summary>
		/// <param name="arCollection">The ar collection.</param>
		public SemanticVerifierVisitor(ActiveRecordModelCollection arCollection)
		{
			this.arCollection = arCollection;
		}

		/// <summary>
		/// Visits the model.
		/// </summary>
		/// <remarks>
		/// Check that the model:
		///  - Define only a discriminator or a join subclass, not both
		///  - Doesn't specify version/timestamp property on a joined subclass / discriminator subclass
		///  - Validate that the custom entity persister implements IEntityPersister
		///  - Validate the joined subclasses has a [JoinedKey] to map back to the parent table
		///  - Validate that the class has a PK
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitModel(ActiveRecordModel model)
		{
			ActiveRecordModel savedModel = currentModel;

			try
			{
				currentModel = model;

				//if (model.IsDiscriminatorBase && model.IsJoinedSubClassBase)
				//{
				//    throw new ActiveRecordException(String.Format(
				//                                        "Unfortunatelly you can't have a discriminator class " +
				//                                        "and a joined subclass at the same time - check type {0}", model.Type.FullName));
				//}

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

				if (model.ActiveRecordAtt != null && model.ActiveRecordAtt.Persister != null)
				{
					if (!typeof(IEntityPersister).IsAssignableFrom(model.ActiveRecordAtt.Persister))
					{
						throw new ActiveRecordException(String.Format(
						                                	"The type assigned as a custom persister, does not implement IEntityPersister " +
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

		/// <summary>
		/// Visits the primary key.
		/// </summary>
		/// <remarks>
		/// Infer column name and the reverse property if using [OneToOne]
		/// </remarks>
		/// <param name="model">The model.</param>
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
			else if (model.PrimaryKeyAtt.Generator == PrimaryKeyType.Custom)
			{
				if (model.PrimaryKeyAtt.CustomGenerator == null)
				{
					throw new ActiveRecordException(String.Format(
					                                	"A type defined that its primary key would use a custom generator, " +
					                                	"but apparently forgot to define the custom generator using PrimaryKeyAttribute.CustomGenerator property. " +
					                                	"Check type {0}", currentModel.Type.FullName));
				}

				if (!typeof(IIdentifierGenerator).IsAssignableFrom(model.PrimaryKeyAtt.CustomGenerator))
				{
					throw new ActiveRecordException(
						"The custom generator associated with the PK for the type " + currentModel.Type.FullName +
						"does not implement interface NHibernate.Id.IIdentifierGenerator");
				}
			}
		}

		/// <summary>
		/// Visits the composite primary key.
		/// </summary>
		/// <remarks>
		/// Validate that the composite key type is implementing GetHashCode() and Equals(), is mark serializable.
		/// Validate that the compose key is compose of two or more columns
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitCompositePrimaryKey(CompositeKeyModel model)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

			Type compositeKeyClassType = model.Property.PropertyType;

			MethodInfo eq = null;
			MethodInfo hc = null;

			new List<MethodInfo>(compositeKeyClassType.GetMethods(flags)).ForEach(delegate(MethodInfo method)
					{
						if (method.Name.Equals("Equals"))
						{
							ParameterInfo[] parameters = method.GetParameters();

							if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof(object)))
							{
								eq = method;
							}
						}
						else if (method.Name.Equals("GetHashCode"))
						{
							ParameterInfo[] parameters = method.GetParameters();

							if (parameters.Length == 0)
							{
								hc = method;
							}
						}
					});

			if (eq == null || hc == null)
			{
				throw new ActiveRecordException(String.Format("To use type '{0}' as a composite id, " +
				                                              "you must implement Equals and GetHashCode.",
				                                              model.Property.PropertyType.Name));
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
				                                              "id it must have two or more properties marked with the [KeyProperty] attribute.",
				                                              model.Property.PropertyType.Name));
			}
		}

		/// <summary>
		/// Visits the property.
		/// </summary>
		/// <remarks>
		/// Infer column name and whatever this propery can be null or not
		/// Also catch common mistake of try to use [Property] on an entity, instead of [BelongsTo]
		/// Ensure that joined properties have a joined table.
		/// </remarks>
		/// <param name="model">The model.</param>
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

			if (propertyType.IsGenericType &&
			    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
			    String.IsNullOrEmpty(model.PropertyAtt.ColumnType))
			{
				model.PropertyAtt.NotNull = false;
				model.PropertyAtt.ColumnType = ObtainNullableTypeNameForCLRNullable(propertyType);
			}

			if (ActiveRecordModel.GetModel(propertyType) != null)
			{
				throw new ActiveRecordException(String.Format(
				                                	"You can't use [Property] on {0}.{1} because {2} is an active record class, did you mean to use BelongTo?",
				                                	model.Property.DeclaringType.Name, model.Property.Name, propertyType.FullName));
			}

			JoinedTableModel joinedTable = ObtainJoinedTableIfPresent(model.Property, model.PropertyAtt);
			
			if (joinedTable != null)
			{
				joinedTable.Properties.Add(model);
			}
		}

		/// <summary>
		/// Visits the field.
		/// </summary>
		/// <remarks>
		/// Infer column name and nullablity
		/// </remarks>
		/// <param name="model">The model.</param>
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

			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
			    String.IsNullOrEmpty(model.FieldAtt.ColumnType))
			{
				model.FieldAtt.NotNull = false;
				model.FieldAtt.ColumnType = ObtainNullableTypeNameForCLRNullable(fieldType);
			}

			JoinedTableModel joinedTable = ObtainJoinedTableIfPresent(model.Field, model.FieldAtt);

			if (joinedTable != null)
			{
				joinedTable.Fields.Add(model);
			}
		}

		/// <summary>
		/// Visits the key.
		/// </summary>
		/// <remarks>
		/// Infer column name
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitKey(KeyModel model)
		{
			if (model.JoinedKeyAtt.Column == null)
			{
				model.JoinedKeyAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.JoinedKeyAtt.Column = columnPrefix + model.JoinedKeyAtt.Column;
		}

		/// <summary>
		/// Visits the version.
		/// </summary>
		/// <remarks>
		/// Infer column name
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitVersion(VersionModel model)
		{
			if (model.VersionAtt.Column == null)
			{
				model.VersionAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.VersionAtt.Column = columnPrefix + model.VersionAtt.Column;
		}

		/// <summary>
		/// Visits the timestamp.
		/// </summary>
		/// <remarks>
		/// Infer column name
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitTimestamp(TimestampModel model)
		{
			if (model.TimestampAtt.Column == null)
			{
				model.TimestampAtt.Column = model.Property.Name;
			}

			// Append column prefix
			model.TimestampAtt.Column = columnPrefix + model.TimestampAtt.Column;
		}

		/// <summary>
		/// Visits the belongs to.
		/// </summary>
		/// <remarks>
		/// Infer column name and type
		/// Verify that the property is virtual if the class was marked lazy.
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitBelongsTo(BelongsToModel model)
		{
			if (currentModel.ActiveRecordAtt != null)
			{
				if (currentModel.ActiveRecordAtt.Lazy ||
				    (currentModel.ActiveRecordAtt.LazySpecified == false && ActiveRecordModel.isLazyByDefault))
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

			JoinedTableModel joinedTable = ObtainJoinedTableIfPresent(model.Property, model.BelongsToAtt);

			if (joinedTable != null)
			{
				joinedTable.BelongsTo.Add(model);
			}
		}

		/// <summary>
		/// Visit the has many to any
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHasManyToAny(HasManyToAnyModel model)
		{
			if (model.HasManyToAnyAtt.MapType == null)
				model.HasManyToAnyAtt.MapType = GuessType(null, model.Property.PropertyType);

			model.HasManyToAnyAtt.RelationType = GuessRelation(model.Property, model.HasManyToAnyAtt.RelationType);

			base.VisitHasManyToAny(model);
		}

		/// <summary>
		/// Visits any.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitAny(AnyModel model)
		{
			if (model.AnyAtt.TypeColumn == null)
			{
				model.AnyAtt.TypeColumn = model.Property.Name + "AnyType";
			}

			if (model.AnyAtt.IdColumn == null)
			{
				model.AnyAtt.IdColumn = model.Property.Name + "AnyTypeId";
			}

			JoinedTableModel joinedTable = ObtainJoinedTableIfPresent(model.Property, model.AnyAtt);

			if (joinedTable != null)
			{
				joinedTable.Anys.Add(model);
			}
		}

		/// <summary>
		/// Visits the has many.
		/// </summary>
		/// <remarks>
		/// Guess the type of the relation, if not specified explicitly
		/// Verify that the assoication is valid on [HasMany]
		/// Validate that required information is specified
		/// Infer the other side of the assoication and grab require data from it
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitHasMany(HasManyModel model)
		{
			if (model.HasManyAtt.MapType == null)
				model.HasManyAtt.MapType = GuessType(null, model.Property.PropertyType);

			model.HasManyAtt.RelationType = GuessRelation(model.Property, model.HasManyAtt.RelationType);

			// Guess the details about a map relation if needed
			if (model.HasManyAtt.RelationType == RelationType.Map)
			{
				if (model.HasManyAtt.Table == null || model.HasManyAtt.Table == string.Empty)
				{
					model.HasManyAtt.Table = string.Format("{0}_{1}", model.Property.ReflectedType.Name, model.Property.Name);
				}
				if (model.HasManyAtt.IndexType == null)
				{
					model.HasManyAtt.IndexType = GetIndexTypeFromDictionary(model.Property.PropertyType).Name;
				}
				if (model.HasManyAtt.MapType == null)
				{
					model.HasManyAtt.MapType = GetMapTypeFromDictionary(model.Property.PropertyType);
				}
			}

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

			Type type = model.HasManyAtt.MapType;
			ActiveRecordModel target = arCollection[type];


			if ((table == null || (keyColumn == null && compositeKeyColumnKeys == null)) && target == null)
			{
				throw new ActiveRecordException(String.Format(
				                                	"ActiveRecord tried to infer details about the relation {0}.{1} but " +
				                                	"it could not find information about the specified target type {2}. If you have mapped a Dictionary of value types, please make sure you have specified the Table property.",
				                                	model.Property.DeclaringType.Name, model.Property.Name, type));
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
				                                	model.Property.DeclaringType.Name, model.Property.Name, type));
			}

			if (target != null)
			{
				VisitModel(target);
			}
			else if (model.HasManyAtt.DependentObjects)
			{
				VisitDependentObject(model.DependentObjectModel);
			}

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

		/// <summary>
		/// Visits the has and belongs to many.
		/// </summary>
		/// <remarks>
		/// Verify that a link table was specified
		/// Verify that a key was specified and that it is valid 
		/// Verify that required information was specified
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			if (model.HasManyAtt.MapType == null)
				model.HasManyAtt.MapType = GuessType(null, model.Property.PropertyType);

			model.HasManyAtt.RelationType = GuessRelation(model.Property, model.HasManyAtt.RelationType);

			Type otherend = GuessType(model.HasManyAtt.MapType, model.Property.PropertyType);

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

			if (model.HasManyAtt.RelationType != RelationType.IdBag && 
				model.HasManyAtt.ColumnRef == null && model.HasManyAtt.CompositeKeyColumnRefs == null)
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

		/// <summary>
		/// Visits the one to one.
		/// </summary>
		/// <remarks>
		/// Infer the type on the other side
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitOneToOne(OneToOneModel model)
		{
			if (model.OneToOneAtt.MapType == null)
			{
				model.OneToOneAtt.MapType = model.Property.PropertyType;
			}

			base.VisitOneToOne(model);
		}

		/// <summary>
		/// Visits the nested model
		/// </summary>
		/// <remarks>
		/// Infer the column name and applies and column prefixes specified
		/// </remarks>
		/// <param name="model">The model.</param>
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

			JoinedTableModel joinedTable = ObtainJoinedTableIfPresent(model.Property, model.NestedAtt);

			if (joinedTable != null)
			{
				joinedTable.Components.Add(model);
			}
		}

		/// <summary>
		/// Visits the custom composite user type.
		/// </summary>
		/// <remarks>
		/// Apply any column prefixes specified
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitCompositeUserType(CompositeUserTypeModel model)
		{
			if (model.Attribute.ColumnNames != null)
			{
				for (int index = 0; index < model.Attribute.ColumnNames.Length; ++index)
				{
					// Add column prefix
					model.Attribute.ColumnNames[index] = columnPrefix + model.Attribute.ColumnNames[index];
				}
			}

			base.VisitCompositeUserType(model);
		}

		/// <summary>
		/// Visits the joined table.
		/// </summary>
		/// <remarks>
		/// Infer column name
		/// </remarks>
		/// <param name="model">The model.</param>
		public override void VisitJoinedTable(JoinedTableModel model)
		{
			if (model.JoinedTableAttribute.Column == null)
			{
				model.JoinedTableAttribute.Column = currentModel.PrimaryKey.PrimaryKeyAtt.Column;
			}
		}

		private static RelationType GuessRelation(PropertyInfo property, RelationType type)
		{
			if (type != RelationType.Guess)
				return type;
			Type propertyType = property.PropertyType;

			if (!propertyType.IsInterface)
			{
				throw new ActiveRecordException(String.Format(
					"Type of property {0}.{1} must be an interface (IList, ISet, IDictionary or their generic counter parts). You cannot use ArrayList or List<T> as the property type.",
						property.DeclaringType.Name, property.Name));
			}

			if (propertyType == typeof(IList))
			{
				return RelationType.Bag;
			}
			else if (propertyType == typeof(ISet))
			{
				return RelationType.Set;
			}
			else if (propertyType == typeof(IDictionary))
			{
				return RelationType.Map;
			}
			else if (propertyType.IsGenericType)
			{
				Type genericTypeDefinition = propertyType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(IList<>) ||
				    genericTypeDefinition == typeof(ICollection<>))
				{
					return RelationType.Bag;
				}
				else if (genericTypeDefinition == typeof(ISet<>))
				{
					return RelationType.Set;
				}
				else if (genericTypeDefinition == typeof(IDictionary<,>))
				{
					return RelationType.Map;
				}
			}

			throw new ActiveRecordException(String.Format(
			                                	"Could not guess relation type for property {0}.{1}  ",
			                                	property.DeclaringType.Name, property.Name));
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

		private JoinedTableModel ObtainJoinedTableIfPresent(MemberInfo propertyOrField, WithAccessOptionalTableAttribute access)
		{
			String tableName = access.Table;

			if (tableName == null)
				return null;

			if (currentModel.IsNestedType)
			{
				throw new ActiveRecordException(
						String.Format("{0} {1} references table \"{2}\" which is not allowed on nested types.",
									   propertyOrField is PropertyInfo ? "Property" : "Field", propertyOrField.Name, tableName));
			}

			if (tableName == String.Empty || tableName == currentModel.ActiveRecordAtt.Table)
			{
				access.Table = null;
				return null;
			}

			JoinedTableModel joinedTable = null;

			foreach (JoinedTableModel jtm in currentModel.JoinedTables)
			{
				if (jtm.JoinedTableAttribute.Table == tableName)
				{
					joinedTable = jtm;
					break;
				}
			}

			if (joinedTable == null)
			{
				throw new ActiveRecordException(
						String.Format("{0} {1} references table \"{2}\", which does not have a corresponding [JoinedTable] on the class.",
									   propertyOrField is PropertyInfo ? "Property" : "Field", propertyOrField.Name, tableName));
			}

			return joinedTable;
		}

		private static String ObtainNullableTypeNameForCLRNullable(Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return underlyingType.AssemblyQualifiedName;
		}

		/// <summary>
		/// Gets the index type of a mapped dictionary.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>The index type of a map element</returns>
		public static Type GetIndexTypeFromDictionary(Type propertyType)
		{
			if (propertyType == null)
				throw new ArgumentNullException("propertyType");

			if (propertyType.IsGenericType == false)
				throw new ArgumentException("The specified propertyType {0} is not generic", propertyType.Name);

			if (typeof(IDictionary<,>).IsAssignableFrom(propertyType.GetGenericTypeDefinition()) == false)
			{
				throw new ArgumentException(
					"ActiveRecord tried to infer details about the mapped property {0} but this isn't of the expected IDictionary<,> type.",
					propertyType.Name);
			}

			Type[] arguments = propertyType.GetGenericArguments();
			return arguments[0];
		}

		/// <summary>
		/// Gets the index type of a mapped dictionary.
		/// </summary>
		/// <param name="propertyType">Type of the property.</param>
		/// <returns>The index type of a map element</returns>
		public static Type GetMapTypeFromDictionary(Type propertyType)
		{
			if (propertyType == null)
				throw new ArgumentNullException("propertyType");

			if (propertyType.IsGenericType == false)
				throw new ArgumentException("The specified propertyType {0} is not generic", propertyType.Name);

			if (typeof(IDictionary<,>).IsAssignableFrom(propertyType.GetGenericTypeDefinition()) == false)
			{
				throw new ArgumentException(
					"ActiveRecord tried to infer details about the mapped property {0} but this isn't of the expected IDictionary<,> type.",
					propertyType.Name);
			}

			Type[] arguments = propertyType.GetGenericArguments();
			return arguments[1];
		}
	}
}

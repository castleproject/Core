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
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.Components.Validator;

	/// <summary>
	/// Bulids an <see cref="ActiveRecordModel"/> from a type and does some inital validation.
	/// </summary>
	public class ActiveRecordModelBuilder
	{
		private static readonly BindingFlags DefaultBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public |
																   BindingFlags.Instance | BindingFlags.NonPublic;

		private static readonly BindingFlags FieldDefaultBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public |
																		BindingFlags.NonPublic | BindingFlags.Instance;

		private static readonly IValidatorRegistry validatorRegistry = new CachedValidationRegistry();
		
		private readonly ActiveRecordModelCollection coll = new ActiveRecordModelCollection();
		
		private IModelBuilderExtension extension;

		/// <summary>
		/// Creates a <see cref="ActiveRecordModel"/> from the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public ActiveRecordModel Create(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			if (type.IsDefined(typeof(ActiveRecordSkipAttribute), false)) return null;

			ActiveRecordModel model = new ActiveRecordModel(type);

			coll.Add(model);

			PopulateModel(model, type);

			ActiveRecordBase.Register(type, model);

			return model;
		}

		/// <summary>
		/// Creates the dummy model for the specified type.
		/// This is required for integration with plain NHibernate entities
		/// </summary>
		/// <param name="type">The type.</param>
		public void CreateDummyModelFor(Type type)
		{
			ActiveRecordBase.Register(type, new ActiveRecordModel(type));
		}

		/// <summary>
		/// Sets the extension.
		/// </summary>
		/// <param name="extension">The extension.</param>
		public void SetExtension(IModelBuilderExtension extension)
		{
			this.extension = extension;
		}

		/// <summary>
		/// Gets the models.
		/// </summary>
		/// <value>The models.</value>
		public ActiveRecordModelCollection Models
		{
			get { return coll; }
		}

		/// <summary>
		/// Gets the validator registry used to create the validators
		/// </summary>
		public static IValidatorRegistry ValidatorRegistry
		{
			get { return validatorRegistry; }
		}

		/// <summary>
		/// Populates the model from tye type
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="type">The type.</param>
		private void PopulateModel(ActiveRecordModel model, Type type)
		{
			if (extension != null)
			{
				extension.ProcessClass(type, model);
			}

			ProcessActiveRecordAttribute(type, model);

			ProcessImports(type, model);

			ProcessJoinedBaseAttribute(type, model);

			ProcessJoinedTables(type, model);

			ProcessProperties(type, model);

			ProcessFields(type, model);
		}

		private static void ProcessImports(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes(typeof(ImportAttribute), false);

			foreach (ImportAttribute att in attrs)
			{
				ImportModel im = new ImportModel(att);
				model.Imports.Add(im);
			}
		}

		private static void ProcessJoinedBaseAttribute(Type type, ActiveRecordModel model)
		{
			model.IsJoinedSubClassBase = type.IsDefined(typeof(JoinedBaseAttribute), false);
		}

		private static void PopulateActiveRecordAttribute(ActiveRecordAttribute attribute, ActiveRecordModel model)
		{
			model.ActiveRecordAtt = attribute;

			if (attribute.DiscriminatorColumn != null)
			{
				model.IsDiscriminatorBase = true;

				if (attribute.DiscriminatorValue == null)
				{
					throw new ActiveRecordException(
						String.Format("You must specify a discriminator value for the type {0}", model.Type.FullName));
				}
			}
			else if (attribute.DiscriminatorType != null)
			{
				throw new ActiveRecordException(
					String.Format("The usage of DiscriminatorType for {0} is meaningless", model.Type.FullName));
			}

			if (model.ActiveRecordAtt.Table == null)
			{
				string safename = GetSafeName(model.Type.Name);
				model.ActiveRecordAtt.Table = ActiveRecordModel.pluralizeTableNames
												? Inflector.Pluralize(safename)
												: safename;
			}
		}

		/// <summary>
		/// Remove the generic part from the type name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetSafeName(string name)
		{
			int index = name.IndexOf("`");

			if (index == -1) return name;

			return name.Substring(0, index);
		}

		private void ProcessFields(Type type, ActiveRecordModel model)
		{
			//Check persistent fields of the base class as well
			if (ShouldCheckBase(type))
			{
				ProcessFields(type.BaseType, model);
			}

			FieldInfo[] fields = type.GetFields(FieldDefaultBindingFlags);

			foreach (FieldInfo field in fields)
			{
				if (field.IsDefined(typeof(FieldAttribute), false))
				{
					FieldAttribute fieldAtt = field.GetCustomAttributes(typeof(FieldAttribute), false)[0] as FieldAttribute;

					model.Fields.Add(new FieldModel(field, fieldAtt));
				}

				if (extension != null)
				{
					extension.ProcessField(field, model);
				}
			}
		}

		private static void ProcessJoinedTables(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes(typeof(JoinedTableAttribute), false);

			foreach (JoinedTableAttribute att in attrs)
			{
				JoinedTableModel jtm = new JoinedTableModel(att);
				model.JoinedTables.Add(jtm);
			}
		}

		private void ProcessProperties(Type type, ActiveRecordModel model)
		{
			// Check persistent properties of the base class as well
			if (ShouldCheckBase(type))
			{
				ProcessProperties(type.BaseType, model);
			}

			PropertyInfo[] props = type.GetProperties(DefaultBindingFlags);

			foreach(PropertyInfo prop in props)
			{
				bool isArProperty = false;
				AnyModel anyModel;
				HasManyToAnyModel hasManyToAnyModel;

				if (extension != null)
				{
					extension.ProcessProperty(prop, model);
				}

				object[] valAtts = prop.GetCustomAttributes(typeof(AbstractValidationAttribute), true);

				foreach(AbstractValidationAttribute valAtt in valAtts)
				{
					IValidator validator = valAtt.Build();
					validator.Initialize(validatorRegistry, prop);

					model.Validators.Add(validator);
				}

				foreach(object attribute in prop.GetCustomAttributes(false))
				{
					if (attribute is PrimaryKeyAttribute)
					{
						PrimaryKeyAttribute propAtt = attribute as PrimaryKeyAttribute;
						isArProperty = true;

						// Joined Subclasses must not have PrimaryKey
						if (type.IsDefined(typeof(JoinedBaseAttribute), true) && // JoinedBase in a superclass
							!type.IsDefined(typeof(JoinedBaseAttribute), false)) // but not here
						{
							throw new ActiveRecordException("You can't specify a PrimaryKeyAttribute in a joined subclass. " +
															"Check type " + model.Type.FullName);
						}

						if (prop.PropertyType.IsDefined(typeof(CompositeKeyAttribute), true))
						{
							object[] att = prop.PropertyType.GetCustomAttributes(typeof(CompositeKeyAttribute), true);

							CompositeKeyAttribute cAtt = att[0] as CompositeKeyAttribute;

							model.CompositeKey = new CompositeKeyModel(prop, cAtt);
						}
						else
						{
							if (!propAtt.IsOverride && model.PrimaryKey != null)
							{
								throw new ActiveRecordException("You can't specify more than one PrimaryKeyAttribute in a " +
								                                "class. Check type " + model.Type.FullName);
							}

							model.PrimaryKey = new PrimaryKeyModel(prop, propAtt);
						}
					}
					else if (attribute is CompositeKeyAttribute)
					{
						CompositeKeyAttribute propAtt = attribute as CompositeKeyAttribute;
						isArProperty = true;

						model.CompositeKey = new CompositeKeyModel(prop, propAtt);
					}
					else if (attribute is AnyAttribute)
					{
						AnyAttribute anyAtt = attribute as AnyAttribute;
						isArProperty = true;
						anyModel = new AnyModel(prop, anyAtt);
						model.Anys.Add(anyModel);

						CollectMetaValues(anyModel.MetaValues, prop);
					}
					else if (attribute is PropertyAttribute)
					{
						PropertyAttribute propAtt = attribute as PropertyAttribute;
						isArProperty = true;

						// If this property overrides a base class property remove the old one
						if (propAtt.IsOverride)
						{
							for (int index = 0; index < model.Properties.Count; ++index)
							{
								PropertyModel oldModel = (PropertyModel)model.Properties[index];

								if (oldModel.Property.Name == prop.Name)
								{
									model.Properties.RemoveAt(index);
									break;
								}
							}
						}

						PropertyModel propModel = new PropertyModel(prop, propAtt);
						model.Properties.Add(propModel);
						model.PropertyDictionary[prop.Name] = propModel;
					}
					else if (attribute is NestedAttribute)
					{
						NestedAttribute propAtt = attribute as NestedAttribute;
						isArProperty = true;

						ActiveRecordModel nestedModel = new ActiveRecordModel(prop.PropertyType);

						nestedModel.IsNestedType = true;

						Type nestedType = propAtt.MapType ?? prop.PropertyType;
						nestedModel.IsNestedCompositeType = model.IsNestedCompositeType;
						ProcessProperties(nestedType, nestedModel);
						ProcessFields(nestedType, nestedModel);

						NestedModel nested = new NestedModel(prop, propAtt, nestedModel);
						nestedModel.ParentNested = nested;

						model.Components.Add(nested);
					}
					else if (attribute is NestedParentReferenceAttribute)
					{
						NestedParentReferenceAttribute nestedParentAtt = attribute as NestedParentReferenceAttribute;
						isArProperty = true;

						model.ComponentParent.Add(new NestedParentReferenceModel(prop, nestedParentAtt));
					}
					else if (attribute is JoinedKeyAttribute)
					{
						JoinedKeyAttribute propAtt = attribute as JoinedKeyAttribute;
						isArProperty = true;

						if (model.Key != null)
						{
							throw new ActiveRecordException("You can't specify more than one JoinedKeyAttribute. " +
															"Check type " + model.Type.FullName);
						}

						model.Key = new KeyModel(prop, propAtt);
					}
					else if (attribute is VersionAttribute)
					{
						VersionAttribute propAtt = attribute as VersionAttribute;
						isArProperty = true;

						if (model.Version != null)
						{
							throw new ActiveRecordException("You can't specify more than one VersionAttribute. " +
															"Check type " + model.Type.FullName);
						}

						model.Version = new VersionModel(prop, propAtt);
					}
					else if (attribute is TimestampAttribute)
					{
						TimestampAttribute propAtt = attribute as TimestampAttribute;
						isArProperty = true;

						if (model.Timestamp != null)
						{
							throw new ActiveRecordException("You can't specify more than one TimestampAttribute. " +
															"Check type " + model.Type.FullName);
						}

						model.Timestamp = new TimestampModel(prop, propAtt);
					}
					// Relations
					else if (attribute is OneToOneAttribute)
					{
						OneToOneAttribute propAtt = attribute as OneToOneAttribute;
						isArProperty = true;

						model.OneToOnes.Add(new OneToOneModel(prop, propAtt));
					}
					else if (attribute is BelongsToAttribute)
					{
						BelongsToAttribute propAtt = attribute as BelongsToAttribute;
						isArProperty = true;

						BelongsToModel btModel = new BelongsToModel(prop, propAtt);
						model.BelongsTo.Add(btModel);
						model.BelongsToDictionary[prop.Name] = btModel;

						if (extension != null)
						{
							extension.ProcessBelongsTo(prop, btModel, model);
						}
					}
					// The ordering is important here, HasManyToAny must comes before HasMany!
					else if (attribute is HasManyToAnyAttribute)
					{
						HasManyToAnyAttribute propAtt = attribute as HasManyToAnyAttribute;
						isArProperty = true;

						hasManyToAnyModel = new HasManyToAnyModel(prop, propAtt);
						model.HasManyToAny.Add(hasManyToAnyModel);
						model.HasManyToAnyDictionary[prop.Name] = hasManyToAnyModel;

						CollectMetaValues(hasManyToAnyModel.MetaValues, prop);

						if (extension != null)
						{
							extension.ProcessHasManyToAny(prop, hasManyToAnyModel, model);
						}
					}
					else if (attribute is HasManyAttribute)
					{
						HasManyAttribute propAtt = attribute as HasManyAttribute;
						isArProperty = true;

						HasManyModel hasManyModel = new HasManyModel(prop, propAtt);
						if (propAtt.DependentObjects)
						{
							ActiveRecordModel dependentObjectModel = new ActiveRecordModel(propAtt.MapType);
							dependentObjectModel.IsNestedType = true;
							dependentObjectModel.IsNestedCompositeType = true;
							ProcessProperties(propAtt.MapType, dependentObjectModel);

							hasManyModel.DependentObjectModel = new DependentObjectModel(prop, propAtt, dependentObjectModel);
						}
						model.HasMany.Add(hasManyModel);
						model.HasManyDictionary[prop.Name] = hasManyModel;

						if (extension != null)
						{
							extension.ProcessHasMany(prop, hasManyModel, model);
						}
					}
					else if (attribute is HasAndBelongsToManyAttribute)
					{
						HasAndBelongsToManyAttribute propAtt = attribute as HasAndBelongsToManyAttribute;
						isArProperty = true;

						HasAndBelongsToManyModel habtManyModel = new HasAndBelongsToManyModel(prop, propAtt);
						model.HasAndBelongsToMany.Add(habtManyModel);
						model.HasAndBelongsToManyDictionary[prop.Name] = habtManyModel;

						if (extension != null)
						{
							extension.ProcessHasAndBelongsToMany(prop, habtManyModel, model);
						}
					}
					else if (attribute is Any.MetaValueAttribute)
					{
						if (prop.GetCustomAttributes(typeof(HasManyToAnyAttribute), false).Length == 0 &&
							prop.GetCustomAttributes(typeof(AnyAttribute), false).Length == 0
							)
							throw new ActiveRecordException(
								"You can't specify an Any.MetaValue without specifying the Any or HasManyToAny attribute. " +
								"Check type " + prop.DeclaringType.FullName);
					}
					else if (attribute is CompositeUserTypeAttribute)
					{
						CompositeUserTypeAttribute propAtt = attribute as CompositeUserTypeAttribute;
						isArProperty = true;

						model.CompositeUserType.Add(new CompositeUserTypeModel(prop, propAtt));
					}

					if (attribute is CollectionIDAttribute)
					{
						CollectionIDAttribute propAtt = attribute as CollectionIDAttribute;

						model.CollectionIDs.Add(new CollectionIDModel(prop, propAtt));
					}
					if (attribute is HiloAttribute)
					{
						HiloAttribute propAtt = attribute as HiloAttribute;

						model.Hilos.Add(new HiloModel(prop, propAtt));
					}
				}

				if (!isArProperty)
				{
					model.NotMappedProperties.Add(prop);
				}
			}
		}

		private static void CollectMetaValues(IList<Any.MetaValueAttribute> metaStore, PropertyInfo prop)
		{
			if (metaStore == null)
				throw new ArgumentNullException("metaStore");

			Any.MetaValueAttribute[] metaValues =
				prop.GetCustomAttributes(typeof(Any.MetaValueAttribute), false) as Any.MetaValueAttribute[];

			if (metaValues == null || metaValues.Length == 0)
				return;

			foreach (Any.MetaValueAttribute attribute in metaValues)
			{
				metaStore.Add(attribute);
			}
		}

		private static bool ShouldCheckBase(Type type)
		{
			// Changed as suggested http://support.castleproject.org/jira/browse/AR-40
			bool shouldCheck = IsRootType(type);

			if (shouldCheck) // Perform more checks 
			{
				Type basetype = type.BaseType;

				while (basetype != typeof(object))
				{
					if (basetype.IsDefined(typeof(JoinedBaseAttribute), false)) return false;

					object[] attrs = basetype.GetCustomAttributes(typeof(ActiveRecordAttribute), false);

					if (attrs.Length != 0)
					{
						ActiveRecordAttribute arAttribute = attrs[0] as ActiveRecordAttribute;
						if (arAttribute.DiscriminatorColumn != null)
							return false;
					}
					else
					{
						return true;
					}

					basetype = basetype.BaseType;
				}
			}

			return shouldCheck;
		}

		private static bool IsRootType(Type type)
		{
			bool isRootType = type.BaseType != typeof(object) &&
							  type.BaseType != typeof(ActiveRecordBase) &&
							  type.BaseType != typeof(ActiveRecordValidationBase);
			// && !type.BaseType.IsDefined(typeof(ActiveRecordAttribute), false);

			// generic check
			if (type.BaseType.IsGenericType)
			{
				isRootType = type.BaseType.GetGenericTypeDefinition() != typeof(ActiveRecordBase<>) &&
							 type.BaseType.GetGenericTypeDefinition() != typeof(ActiveRecordValidationBase<>);
			}

			return isRootType;
		}

		private static void ProcessActiveRecordAttribute(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);

			if (attrs.Length == 0)
			{
				throw new ActiveRecordException(
					String.Format("Type {0} is not using the ActiveRecordAttribute, which is obligatory.", type.FullName));
			}

			ActiveRecordAttribute arAttribute = attrs[0] as ActiveRecordAttribute;

			PopulateActiveRecordAttribute(arAttribute, model);
		}
	}
}

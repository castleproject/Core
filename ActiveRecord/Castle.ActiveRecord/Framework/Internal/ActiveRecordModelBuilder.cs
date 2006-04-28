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
	using System.Reflection;
	using System.Collections;

	public class ActiveRecordModelBuilder
	{
		private static readonly BindingFlags DefaultBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
		private static readonly BindingFlags FieldDefaultBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		private readonly ActiveRecordModelCollection coll = new ActiveRecordModelCollection();

		public ActiveRecordModelBuilder()
		{
		}

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

		public ActiveRecordModelCollection Models
		{
			get { return coll; }
		}

		private void PopulateModel(ActiveRecordModel model, Type type)
		{
			ProcessActiveRecordAttribute(type, model);

			ProcessImports(type, model);

			ProcessJoinedBaseAttribute(type, model);

			ProcessProperties(type, model);

			ProcessFields(type, model);
		}

		private void ProcessImports(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes(typeof(ImportAttribute), false);

			foreach(ImportAttribute att in attrs)
			{
				ImportModel im = new ImportModel(att);
				model.Imports.Add(im);
			}
		}

		private void ProcessJoinedBaseAttribute(Type type, ActiveRecordModel model)
		{
			model.IsJoinedSubClassBase = type.IsDefined(typeof(JoinedBaseAttribute), false);
		}

		private void PopulateActiveRecordAttribute(ActiveRecordAttribute attribute, ActiveRecordModel model)
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
				model.ActiveRecordAtt.Table = model.Type.Name;
			}
		}

		private void ProcessFields(Type type, ActiveRecordModel model)
		{
			//Check persistent fields of the base class as well
			if (ShouldCheckBase(type))
			{
				ProcessFields(type.BaseType, model);
			}

			FieldInfo[] fields = type.GetFields(FieldDefaultBindingFlags);

			foreach(FieldInfo field in fields)
			{
				if (field.IsDefined(typeof(FieldAttribute), false))
				{
					FieldAttribute fieldAtt = field.GetCustomAttributes(typeof(FieldAttribute), false)[0] as FieldAttribute;

					model.Fields.Add(new FieldModel(field, fieldAtt));
				}
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
				AnyModel anyModel = null;
				ArrayList anyMetaValues = new ArrayList();

				object[] valAtts = prop.GetCustomAttributes(typeof(AbstractValidationAttribute), true);

				foreach(AbstractValidationAttribute valAtt in valAtts)
				{
					valAtt.Validator.Initialize(prop);

					model.Validators.Add(valAtt.Validator);
				}

				foreach(object attribute in prop.GetCustomAttributes(false))
				{
					if (attribute is PrimaryKeyAttribute)
					{
						PrimaryKeyAttribute propAtt = attribute as PrimaryKeyAttribute;
						isArProperty = true;

						model.Ids.Add(new PrimaryKeyModel(prop, propAtt));
					}
					else if (attribute is AnyAttribute)
					{
						AnyAttribute anyAtt = attribute as AnyAttribute;
						isArProperty = true;
						anyModel = new AnyModel(prop, anyAtt);
						model.Anys.Add(anyModel);
					}
					else if (attribute is Any.MetaValueAttribute)
					{
						Any.MetaValueAttribute meta = attribute as Any.MetaValueAttribute;
						isArProperty = true;
						anyMetaValues.Add(meta);
					}
					else if (attribute is PropertyAttribute)
					{
						PropertyAttribute propAtt = attribute as PropertyAttribute;
						isArProperty = true;

						model.Properties.Add(new PropertyModel(prop, propAtt));
					}
					else if (attribute is NestedAttribute)
					{
						NestedAttribute propAtt = attribute as NestedAttribute;
						isArProperty = true;

						ActiveRecordModel nestedModel = new ActiveRecordModel(prop.PropertyType);

						nestedModel.IsNestedType = true;

						ProcessProperties(prop.PropertyType, nestedModel);

						model.Components.Add(new NestedModel(prop, propAtt, nestedModel));
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

						model.BelongsTo.Add(new BelongsToModel(prop, propAtt));
					}
						//The ordering is important here, HasManyToAny must comes before HasMany!
					else if (attribute is HasManyToAnyAttribute)
					{
						HasManyToAnyAttribute propAtt = attribute as HasManyToAnyAttribute;
						isArProperty = true;

						model.HasManyToAny.Add(new HasManyToAnyModel(prop, propAtt));
					}
					else if (attribute is HasManyAttribute)
					{
						HasManyAttribute propAtt = attribute as HasManyAttribute;
						isArProperty = true;

						model.HasMany.Add(new HasManyModel(prop, propAtt));
					}
					else if (attribute is HasAndBelongsToManyAttribute)
					{
						HasAndBelongsToManyAttribute propAtt = attribute as HasAndBelongsToManyAttribute;
						isArProperty = true;

						model.HasAndBelongsToMany.Add(new HasAndBelongsToManyModel(prop, propAtt));
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

				if (anyMetaValues.Count > 0)
				{
					if (anyModel == null)
					{
						throw new ActiveRecordException("You can't specify a Any.MetaValue without specifying the Any attribute. " +
							"Check type " + prop.DeclaringType.FullName);
					}
					anyModel.MetaValues = anyMetaValues;
				}

				if (!isArProperty)
				{
					model.NotMappedProperties.Add(prop);
				}
			}
		}

		private static bool ShouldCheckBase(Type type)
		{
			// Changed as suggested http://support.castleproject.org/jira/browse/AR-40

			bool shouldCheck = type.BaseType != typeof(object) &&
				type.BaseType != typeof(ActiveRecordBase) &&
				type.BaseType != typeof(ActiveRecordValidationBase); 
				// && !type.BaseType.IsDefined(typeof(ActiveRecordAttribute), false);

			if (shouldCheck) // Perform more checks 
			{
				Type basetype = type.BaseType;
				
				while(basetype != typeof(object))
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

		private void ProcessActiveRecordAttribute(Type type, ActiveRecordModel model)
		{
			object[] attrs = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);

			if (attrs.Length == 0)
			{
				throw new ActiveRecordException(String.Format("Type {0} is not using the ActiveRecordAttribute, which is obligatory.", type.FullName));
			}

			ActiveRecordAttribute arAttribute = attrs[0] as ActiveRecordAttribute;

			PopulateActiveRecordAttribute(arAttribute, model);
		}
	}
}
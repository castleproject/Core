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
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using Castle.ActiveRecord;

	/// <summary>
	/// Traverse the tree emitting proper xml configuration
	/// </summary>
	public class XmlGenerationVisitor : AbstractDepthFirstVisitor
	{
		private StringBuilder xmlBuilder = new StringBuilder();
		private int identLevel = 0;
		private String currentTable;

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			xmlBuilder.Length = 0;
		}

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <value>The XML.</value>
		public String Xml
		{
			get { return xmlBuilder.ToString(); }
		}

		/// <summary>
		/// Creates the XML.
		/// </summary>
		/// <param name="model">The model.</param>
		public void CreateXml(ActiveRecordModel model)
		{
			CreateXmlPI();
			StartMappingNode(model.UseAutoImport);
			Ident();
			VisitModel(model);
			Dedent();
			EndMappingNode();

			if (ActiveRecordModel.isDebug)
			{
				String file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, model.Type.Name + ".hbm.xml");

				System.IO.File.Delete(file);

				using(System.IO.FileStream fs = System.IO.File.OpenWrite(file))
				{
					String xml = Xml;

					byte[] ba = System.Text.ASCIIEncoding.Unicode.GetBytes(xml);

					fs.Write(ba, 0, ba.Length);

					fs.Flush();
				}
			}
		}

		/// <summary>
		/// Visits the model.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitModel(ActiveRecordModel model)
		{
			VisitNodes(model.Imports);
			if (model.IsJoinedSubClass && model.IsDiscriminatorSubClass)
			{
				AppendF("<subclass{0}{1}{2}{3}{4}{5}>",
						MakeAtt("name", MakeTypeName(model.Type)),
						MakeAtt("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
						WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
						MakeAtt("lazy", model.ActiveRecordAtt.Lazy, model.ActiveRecordAtt.LazySpecified),
						WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
						WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert));
				Ident();
				AppendF("<join{0}>",
					MakeAtt("table", model.ActiveRecordAtt.Table),
					WriteIfNonNull("schema", model.ActiveRecordAtt.Schema));
				Ident();
				VisitNode(model.Key);
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				Dedent();
				Append("</join>");
				VisitNodes(model.JoinedTables);
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</subclass>");
			}
			else if (model.IsJoinedSubClass)
			{
				AppendF("<joined-subclass{0}{1}{2}{3}{4}{5}{6}{7}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        MakeAtt("table", model.ActiveRecordAtt.Table),
				        WriteIfNonNull("schema", model.ActiveRecordAtt.Schema),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
				        WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
				        MakeAtt("lazy", model.ActiveRecordAtt.Lazy, model.ActiveRecordAtt.LazySpecified),
				        WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
				        WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert));
				Ident();
				VisitNode(model.Key);
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.Anys);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				VisitNodes(model.JoinedTables);
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</joined-subclass>");
			}
			else if (model.IsDiscriminatorSubClass)
			{
				AppendF("<subclass{0}{1}{2}{3}{4}{5}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        MakeAtt("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
				        MakeAtt("lazy", model.ActiveRecordAtt.Lazy, model.ActiveRecordAtt.LazySpecified),
				        WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
				        WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert));
				Ident();
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				VisitNodes(model.JoinedTables);
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</subclass>");
			}
			else if (model.IsNestedType)
			{
				Ident();
				VisitNodes(model.ComponentParent);
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				VisitNodes(model.CompositeUserType);
				Dedent();
			}
			else
			{
				AppendF("<class{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        MakeAtt("table", model.ActiveRecordAtt.Table),
				        WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
				        WriteIfFalse("mutable", model.ActiveRecordAtt.Mutable),
				        WriteIfNonNull("schema", model.ActiveRecordAtt.Schema),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
				        WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
				        WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert),
				        WriteIfTrue("select-before-update", model.ActiveRecordAtt.SelectBeforeUpdate),
				        ConditionalWrite("polymorphism", model.ActiveRecordAtt.Polymorphism.ToString().ToLower(),
				                         model.ActiveRecordAtt.Polymorphism != Polymorphism.Implicit),
				        WriteIfNonNull("where", model.ActiveRecordAtt.Where),
				        WriteIfNonNull("persister", MakeTypeName(model.ActiveRecordAtt.Persister)),
				        ConditionalWrite("batch-size", model.ActiveRecordAtt.BatchSize.ToString(),
				                         model.ActiveRecordAtt.BatchSize > 1),
				        ConditionalWrite("optimistic-lock", model.ActiveRecordAtt.Locking.ToString().ToLower(),
				                         model.ActiveRecordAtt.Locking != OptimisticLocking.Version),
				        MakeAtt("lazy", model.ActiveRecordAtt.Lazy, model.ActiveRecordAtt.LazySpecified));
				Ident();
				WriteCache(model.ActiveRecordAtt.Cache, model.ActiveRecordAtt.CacheRegion);
				VisitNode(model.PrimaryKey);
				VisitNode(model.CompositeKey);
				WriteDiscriminator(model);
				VisitNode(model.Version);
				VisitNode(model.Timestamp);
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.Anys);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				VisitNodes(model.JoinedTables);
				VisitNodes(model.CompositeUserType);
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</class>");
			}
		}

		/// <summary>
		/// Visits the primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			String unsavedVal = model.PrimaryKeyAtt.UnsavedValue;

			if (unsavedVal == null)
			{
				if (model.Property.PropertyType.IsPrimitive && model.Property.PropertyType != typeof(String))
				{
					unsavedVal = "0";
				}
				else if (model.Property.PropertyType == typeof(Guid))
				{
					unsavedVal = Guid.Empty.ToString();
				}
			}

			AppendF("<id{0}{1}{2}{3}{4}{5}>",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.PrimaryKeyAtt.AccessString),
			        MakeAtt("column", model.PrimaryKeyAtt.Column),
			        MakeTypeAtt(model.Property.PropertyType, model.PrimaryKeyAtt.ColumnType),
			        WriteIfNotZero("length", model.PrimaryKeyAtt.Length),
			        WriteIfNonNull("unsaved-value", unsavedVal));

			Ident();

			String className = null;

			switch(model.PrimaryKeyAtt.Generator)
			{
				case PrimaryKeyType.Identity:
				case PrimaryKeyType.Sequence:
				case PrimaryKeyType.HiLo:
				case PrimaryKeyType.SeqHiLo:
				case PrimaryKeyType.Guid:
				case PrimaryKeyType.Native:
				case PrimaryKeyType.Assigned:
				case PrimaryKeyType.Foreign:
				case PrimaryKeyType.Increment:
					className = model.PrimaryKeyAtt.Generator.ToString().ToLower(CultureInfo.InvariantCulture);
					break;

				case PrimaryKeyType.GuidComb:
					className = "guid.comb";
					break;

				case PrimaryKeyType.UuidHex:
					className = "uuid.hex";
					break;

				case PrimaryKeyType.UuidString:
					className = "uuid.string";
					break;

				case PrimaryKeyType.Counter:
					className = "vm";
					break;

				case PrimaryKeyType.Custom:
					className = MakeTypeName(model.PrimaryKeyAtt.CustomGenerator);
					break;
			}

			AppendF("<generator{0}>", MakeAtt("class", className));

			if (model.PrimaryKeyAtt.SequenceName != null)
			{
				Ident();
				AppendF("<param name=\"sequence\">{0}</param>", model.PrimaryKeyAtt.SequenceName);
				Dedent();
			}
			if (model.PrimaryKeyAtt.Params != null)
			{
				Ident();

				String[] paras = model.PrimaryKeyAtt.Params.Split(',');

				foreach(String param in paras)
				{
					int eqIndex = param.IndexOf("=");
					//need to handle parameters that may contains =, so we will take into account only the first one
					string paramName = param.Substring(0, eqIndex);
					string paramValue = param.Substring(eqIndex+1);
					
					AppendF("<param name=\"{0}\">{1}</param>", paramName, paramValue);
				}

				Dedent();
			}
			AppendF("</generator>");


			Dedent();
			Append("</id>");
		}

		/// <summary>
		/// Visits the composite primary key.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitCompositePrimaryKey(CompositeKeyModel model)
		{
			CompositeKeyAttribute att = model.CompositeKeyAtt;

			string unsavedVal = att.UnsavedValue;

			if (unsavedVal == null)
			{
				unsavedVal = "none";
			}

			AppendF("<composite-id{0}{1}{2}{3}>",
			        MakeAtt("name", model.Property.Name),
			        MakeClassAtt(model.Property.PropertyType),
			        WriteIfNonNull("unsaved-value", unsavedVal),
			        MakeAtt("access", att.AccessString));

			Ident();

			PropertyInfo[] keyProps = model.Property.PropertyType.GetProperties();

			foreach(PropertyInfo keyProp in keyProps)
			{
				KeyPropertyAttribute keyPropAttr = keyProp.GetCustomAttributes(
				                                   	typeof(KeyPropertyAttribute), false)[0] as KeyPropertyAttribute;

				if (keyPropAttr.Column == null)
				{
					keyPropAttr.Column = keyProp.Name;
				}

				AppendF("<key-property{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10} />",
				        MakeAtt("name", keyProp.Name),
				        MakeAtt("access", keyPropAttr.AccessString),
				        MakeAtt("column", keyPropAttr.Column),
				        MakeTypeAtt(keyProp.PropertyType, keyPropAttr.ColumnType),
				        WriteIfNotZero("length", keyPropAttr.Length),
				        WriteIfNonNull("unsaved-value", keyPropAttr.UnsavedValue),
				        WriteIfTrue("not-null", keyPropAttr.NotNull),
				        WriteIfTrue("unique", keyPropAttr.Unique),
				        WriteIfFalse("insert", keyPropAttr.Insert),
				        WriteIfFalse("update", keyPropAttr.Update),
				        WriteIfNonNull("formula", keyPropAttr.Formula));
			}

			Dedent();
			AppendF("</composite-id>");
		}

		/// <summary>
		/// Visits the import.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitImport(ImportModel model)
		{
			AppendF("<import{0}{1} />",
			        MakeAtt("class", MakeTypeName(model.ImportAtt.Type)),
			        MakeAtt("rename", model.ImportAtt.Rename));
		}

		/// <summary>
		/// Visits the property.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitProperty(PropertyModel model)
		{
			PropertyAttribute att = model.PropertyAtt;

			if (AppliesToCurrentTable(att))
			{
				WriteProperty(model.Property.Name, model.Property.PropertyType, att.AccessString,
							  att.ColumnType, att.Insert,
							  att.Update, att.Formula, att.Column,
							  att.Length, att.NotNull, att.Unique, att.UniqueKey, att.SqlType, att.Index, att.Check, att.Default);
			}
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
			JoinedTableAttribute att = model.JoinedTableAttribute;

			using (new TableScope(this, att.Table))
			{
				AppendF("<join{0}{1}{2}{3}>",
							MakeAtt("table", att.Table),
							WriteIfNonNull("schema", att.Schema),
							WriteIfNonNull("fetch", TranslateFetch(att.Fetch)),
							WriteIfTrue("inverse", att.Inverse),
							WriteIfTrue("optional", att.Optional));
				Ident();
				WriteKey(att.Column);
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.Anys);
				VisitNodes(model.Components);
				Dedent();
				Append("</join>");
			}
		}

		/// <summary>
		/// Visits the field.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitField(FieldModel model)
		{
			FieldAttribute att = model.FieldAtt;

			if (AppliesToCurrentTable(att))
			{
				WriteProperty(model.Field.Name, model.Field.FieldType, att.AccessString,
							  att.ColumnType, att.Insert,
							  att.Update, att.Formula, att.Column,
							  att.Length, att.NotNull, att.Unique, att.UniqueKey, att.SqlType, att.Index, att.Check, att.Default);
			}
		}

		/// <summary>
		/// Visits the parent
		/// </summary>
		/// <param name="referenceModel">The reference model.</param>
		public override void VisitNestedParentReference(NestedParentReferenceModel referenceModel)
		{
			AppendF("<parent name=\"{0}\"/>", referenceModel.Property.Name);
		}

		/// <summary>
		/// Visits any.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitAny(AnyModel model)
		{
			if (AppliesToCurrentTable(model.AnyAtt))
			{
				String cascade = TranslateCascadeEnum(model.AnyAtt.Cascade);

				AppendF("<any{0}{1}{2}{3}{4}{5}{6}{7}{8}>",
						MakeAtt("name", model.Property.Name),
						MakeAtt("access", model.AnyAtt.AccessString),
						MakeCustomTypeAtt("id-type", model.AnyAtt.IdType),
						MakeCustomTypeAttIfNotNull("meta-type", model.AnyAtt.MetaType),
						WriteIfFalse("insert", model.AnyAtt.Insert),
						WriteIfFalse("update", model.AnyAtt.Update),
						WriteIfNonNull("index", model.AnyAtt.Index),
						WriteIfNonNull("cascade", cascade),
						WriteIfTrue("not-null", model.AnyAtt.NotNull));
				Ident();
				foreach (Any.MetaValueAttribute meta in model.MetaValues)
				{
					AppendF("<meta-value{0}{1} />",
							MakeAtt("value", meta.Value),
							MakeCustomTypeAtt("class", meta.Class)
						);
				}
				//The ordering is important, apparently
				AppendF("<column{0} />",
						MakeAtt("name", model.AnyAtt.TypeColumn)
					);
				AppendF("<column{0} />",
						MakeAtt("name", model.AnyAtt.IdColumn)
					);
				Dedent();
				Append("</any>");
			}
		}

		/// <summary>
		/// Visits the has many to any.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHasManyToAny(HasManyToAnyModel model)
		{
			HasManyToAnyAttribute att = model.HasManyToAnyAtt;

			Type mapType = GuessType(att.MapType, model.Property.PropertyType);
			WriteCollection(att.Cascade, mapType, att.RelationType, model.Property.Name,
			                model.HasManyToAnyAtt.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey, null, null, null, null, null, model.Configuration, att.Index,
			                att.IndexType,
							att.Cache, att.CacheRegion, att.NotFoundBehaviour, att.Fetch, att.BatchSize, att.CollectionType);
		}

		/// <summary>
		/// Visits the has many to any config.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHasManyToAnyConfig(HasManyToAnyModel.Config model)
		{
			HasManyToAnyAttribute att = model.Parent.HasManyToAnyAtt;
			AppendF("<many-to-any{0}{1}>",
			        MakeCustomTypeAtt("id-type", att.IdType),
			        MakeCustomTypeAttIfNotNull("meta-type", att.MetaType));
			Ident();

			// This is here so the XmlGenerationVisitor will always
			// output the meta-values in consistent order, to aid the tests,
			// MetaValueAttribute implements IComparable
			List<Any.MetaValueAttribute> sortedMetaValues = new List<Any.MetaValueAttribute>(model.Parent.MetaValues);
			sortedMetaValues.Sort();
			foreach(Any.MetaValueAttribute meta in sortedMetaValues)
			{
				AppendF("<meta-value{0}{1} />",
				        MakeAtt("value", meta.Value),
				        MakeCustomTypeAtt("class", meta.Class)
					);
			}

			AppendF("<column{0} />",
			        MakeAtt("name", att.TypeColumn));
			AppendF("<column{0} />",
			        MakeAtt("name", att.IdColumn));
			Dedent();
			AppendF("</many-to-any>");
		}

		/// <summary>
		/// Visits the version.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitVersion(VersionModel model)
		{
			String unsavedValue = model.VersionAtt.UnsavedValue;

			AppendF("<version{0}{1}{2}{3}{4} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.VersionAtt.AccessString),
			        MakeAtt("column", model.VersionAtt.Column),
			        MakeTypeAtt(model.Property.PropertyType, model.VersionAtt.Type),
			        WriteIfNonNull("unsaved-value", unsavedValue));
		}

		/// <summary>
		/// Visits the timestamp.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitTimestamp(TimestampModel model)
		{
			AppendF("<timestamp{0}{1}{2} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.TimestampAtt.AccessString),
			        MakeAtt("column", model.TimestampAtt.Column));
		}

		/// <summary>
		/// Visits the key.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitKey(KeyModel model)
		{
			WriteKey(model.JoinedKeyAtt.Column);
		}

		/// <summary>
		/// Visits the one to one.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitOneToOne(OneToOneModel model)
		{
			String cascade = TranslateCascadeEnum(model.OneToOneAtt.Cascade);

			AppendF("<one-to-one{0}{1}{2}{3}{4}{5}{6}{7} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.OneToOneAtt.AccessString),
			        MakeAtt("class", MakeTypeName(model.OneToOneAtt.MapType)),
			        WriteIfNonNull("cascade", cascade),
			        WriteIfNonNull("property-ref", model.OneToOneAtt.PropertyRef),
					WriteIfNonNull("foreign-key", model.OneToOneAtt.ForeignKey),
					WriteIfNonNull("fetch", TranslateFetch(model.OneToOneAtt.Fetch)),
					WriteIfTrue("constrained", model.OneToOneAtt.Constrained));
		}

		/// <summary>
		/// Visits the belongs to.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitBelongsTo(BelongsToModel model)
		{
			if (!AppliesToCurrentTable(model.BelongsToAtt))
			{
				return;
			}

			String cascade = TranslateCascadeEnum(model.BelongsToAtt.Cascade);
			String fetch = TranslateFetch(model.BelongsToAtt.Fetch);
			String notFoundMode = TranslateNotFoundBehaviourEnum(model.BelongsToAtt.NotFoundBehaviour);

			if (model.BelongsToAtt.Column == null)
			{
				AppendF("<many-to-one{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}>",
				        MakeAtt("name", model.Property.Name),
				        MakeAtt("access", model.BelongsToAtt.AccessString),
				        MakeAtt("class", MakeTypeName(model.BelongsToAtt.Type)),
				        WriteIfTrue("not-null", model.BelongsToAtt.NotNull),
				        WriteIfTrue("unique", model.BelongsToAtt.Unique),
				        WriteIfNonNull("unique-key", model.BelongsToAtt.UniqueKey),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("fetch", fetch),
				        WriteIfFalse("update", model.BelongsToAtt.Update),
				        WriteIfFalse("insert", model.BelongsToAtt.Insert),
						WriteIfNonNull("property-ref", model.BelongsToAtt.PropertyRef),
				        WriteIfNonNull("foreign-key", model.BelongsToAtt.ForeignKey),
				        WriteIfNonNull("not-found", notFoundMode));
				Ident();
				WriteCompositeColumns(model.BelongsToAtt.CompositeKeyColumns);
				Dedent();
				Append("</many-to-one>");
			}
			else
			{
				AppendF("<many-to-one{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13} />",
				        MakeAtt("name", model.Property.Name),
				        MakeAtt("access", model.BelongsToAtt.AccessString),
				        MakeAtt("class", MakeTypeName(model.BelongsToAtt.Type)),
				        MakeAtt("column", model.BelongsToAtt.Column),
				        WriteIfFalse("insert", model.BelongsToAtt.Insert),
				        WriteIfFalse("update", model.BelongsToAtt.Update),
						WriteIfNonNull("property-ref", model.BelongsToAtt.PropertyRef),
				        WriteIfTrue("not-null", model.BelongsToAtt.NotNull),
				        WriteIfTrue("unique", model.BelongsToAtt.Unique),
				        WriteIfNonNull("unique-key", model.BelongsToAtt.UniqueKey),
				        WriteIfNonNull("foreign-key", model.BelongsToAtt.ForeignKey),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("fetch", fetch),
				        WriteIfNonNull("not-found", notFoundMode));
			}
		}

		/// <summary>
		/// Visits the has many.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHasMany(HasManyModel model)
		{
			HasManyAttribute att = model.HasManyAtt;

			Type mapType = GuessType(att.MapType, model.Property.PropertyType);
			WriteCollection(att.Cascade, mapType, att.RelationType, model.Property.Name,
			                model.HasManyAtt.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey, att.CompositeKeyColumnKeys, att.Element, att.ElementType, null, null,
			                model.DependentObjectModel, att.Index, att.IndexType,
							att.Cache, att.CacheRegion, att.NotFoundBehaviour, att.Fetch, att.BatchSize, att.CollectionType);
		}

		/// <summary>
		/// Visits the has and belongs to many.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			HasAndBelongsToManyAttribute att = model.HasManyAtt;

			Type mapType = GuessType(att.MapType, model.Property.PropertyType);
			WriteCollection(att.Cascade, mapType, att.RelationType, model.Property.Name,
			                att.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey, att.CompositeKeyColumnKeys, att.Element, att.ElementType, 
							att.ColumnRef,
							att.CompositeKeyColumnRefs, model.CollectionID, att.Index, att.IndexType, att.Cache, att.CacheRegion, 
							att.NotFoundBehaviour, att.Fetch, att.BatchSize, att.CollectionType);
		}

		/// <summary>
		/// Visits the nested.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitNested(NestedModel model)
		{
			if (model.Model.IsNestedCompositeType)
			{
				AppendF("<nested-composite-element{0}{1}{2}{3}{4}>",
						MakeAtt("name", model.Property.Name),
						WriteIfFalse("update", model.NestedAtt.Update),
						WriteIfFalse("insert", model.NestedAtt.Insert),
						WriteIfNonNull("class", MakeTypeName(model.NestedAtt.MapType)),
						WriteIfNonNull("access", model.NestedAtt.AccessString));

				base.VisitNested(model);

				Append("</nested-composite-element>");
			}
			else if (AppliesToCurrentTable(model.NestedAtt))
			{
				AppendF("<component{0}{1}{2}{3}{4}>",
				        MakeAtt("name", model.Property.Name),
				        WriteIfFalse("update", model.NestedAtt.Update),
				        WriteIfFalse("insert", model.NestedAtt.Insert),
				        WriteIfNonNull("class", MakeTypeName(model.NestedAtt.MapType)),
						WriteIfNonNull("access", model.NestedAtt.AccessString));

				using (new TableScope(this, null))
				{
					base.VisitNested(model);
				}

				Append("</component>");
			}
		}

		/// <summary>
		/// Visits the dependent object.
		/// </summary>
		/// <param name="model">The model</param>
		public override void VisitDependentObject(DependentObjectModel model)
		{
			AppendF("<composite-element{0}>",
			        WriteIfNonNull("class", MakeTypeName(model.HasManyAtt.MapType)));

			base.VisitDependentObject(model);

			Append("</composite-element>");
		}

		/// <summary>
		/// Visits the collection ID.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitCollectionID(CollectionIDModel model)
		{
			AppendF("<collection-id{0}{1}>",
			        MakeAtt("type", model.CollectionIDAtt.ColumnType),
			        MakeAtt("column", model.CollectionIDAtt.Column));
			Ident();
			AppendF("<generator{0}>",
			        MakeAtt("class", model.CollectionIDAtt.Generator.ToString().ToLower()));
			Ident();
			base.VisitCollectionID(model);
			Dedent();
			Append("</generator>");
			Dedent();
			Append("</collection-id>");
		}

		/// <summary>
		/// Visits the hilo.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitHilo(HiloModel model)
		{
			AppendF("<param name=\"table\">{0}</param>", model.HiloAtt.Table);
			AppendF("<param name=\"column\">{0}</param>", model.HiloAtt.Column);
			AppendF("<param name=\"max_lo\">{0}</param>", model.HiloAtt.MaxLo);
		}

		/// <summary>
		/// Visits the custom composite user type.
		/// </summary>
		/// <param name="model">The model.</param>
		public override void VisitCompositeUserType(CompositeUserTypeModel model)
		{
			CompositeUserTypeAttribute attribute = model.Attribute;
			BeginWriteProperty(attribute.AccessString, MakeTypeName(attribute.CompositeType), null, attribute.Insert,
			                   model.Property.Name, model.Property.PropertyType, attribute.Update);

			Ident();

			for (int i = 0; i < attribute.ColumnNames.Length; i++)
			{
				WriteColumn(null, attribute.ColumnNames[i], null, attribute.Length[i], false, null, false, null, null);
			}

			Dedent();

			EndWriteProperty();
		}

		private bool AppliesToCurrentTable(WithAccessOptionalTableAttribute access)
		{
			return access.Table == currentTable;
		}

		private void WriteCollection(ManyRelationCascadeEnum cascadeEnum,
		                             Type targetType, RelationType type, string name,
		                             string accessString, string table, string schema, bool lazy,
		                             bool inverse, string orderBy, string where, string sort,
		                             string columnKey, string[] compositeKeyColumnKeys, string element, string elementType,
		                             string columnRef, string[] compositeKeyColumnRefs,
		                             IVisitable extraModel, string index, string indexType, CacheEnum cache, string cacheregion,
		                             NotFoundBehaviour notFoundBehaviour, FetchEnum fetch, int batchSize, Type collectionType)
		{
			bool extraModelVisited = false;

			String cascade = TranslateCascadeEnum(cascadeEnum);
			String notFoundMode = TranslateNotFoundBehaviourEnum(notFoundBehaviour);
			String fetchString = TranslateFetch(fetch);

			String closingTag = null;

			if (type == RelationType.Guess)
				throw new ActiveRecordException(string.Format("Failed to guess the relation for {0}", name));

			if (type == RelationType.Bag)
			{
				closingTag = "</bag>";

				AppendF("<bag{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}>",
				        MakeAtt("name", name),
				        MakeAtt("access", accessString),
				        WriteIfNonNull("table", table),
				        WriteIfNonNull("schema", schema),
				        MakeAtt("lazy", lazy),
				        WriteIfTrue("inverse", inverse),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("order-by", orderBy),
				        WriteIfNonNull("where", where),
				        WriteIfNonNull("fetch", fetchString),						
                        WriteIfNotOne("batch-size", batchSize),
						WriteIfNonNull("collection-type", MakeTypeName(collectionType)));
			}
			else if (type == RelationType.Set)
			{
				closingTag = "</set>";

				AppendF("<set{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}>",
				        MakeAtt("name", name),
				        MakeAtt("access", accessString),
				        WriteIfNonNull("table", table),
				        WriteIfNonNull("schema", schema),
				        MakeAtt("lazy", lazy),
				        WriteIfTrue("inverse", inverse),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("order-by", orderBy),
				        WriteIfNonNull("where", where),
				        WriteIfNonNull("sort", sort),
                        WriteIfNonNull("fetch", fetchString),
						WriteIfNotOne("batch-size", batchSize),
						WriteIfNonNull("collection-type", MakeTypeName(collectionType)));
			}
			else if (type == RelationType.IdBag)
			{
				closingTag = "</idbag>";

				AppendF("<idbag{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}>",
				        MakeAtt("name", name),
				        MakeAtt("access", accessString),
				        WriteIfNonNull("table", table),
				        WriteIfNonNull("schema", schema),
				        MakeAtt("lazy", lazy),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("order-by", orderBy),
				        WriteIfNonNull("where", where),
                        WriteIfNonNull("fetch", fetchString),
						WriteIfNotOne("batch-size", batchSize),
						WriteIfNonNull("collection-type", MakeTypeName(collectionType)));
				extraModelVisited = true;
				Ident();
				VisitNode(extraModel);
				Dedent();
			}
			else if (type == RelationType.Map)
			{
				closingTag = "</map>";

				AppendF("<map{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}>",
				        MakeAtt("name", name),
				        MakeAtt("access", accessString),
				        WriteIfNonNull("table", table),
				        WriteIfNonNull("schema", schema),
				        MakeAtt("lazy", lazy),
				        WriteIfTrue("inverse", inverse),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("order-by", orderBy),
				        WriteIfNonNull("where", where),
				        WriteIfNonNull("sort", sort),
                        WriteIfNonNull("fetch", fetchString),
						WriteIfNotOne("batch-size", batchSize),
						WriteIfNonNull("collection-type", MakeTypeName(collectionType)));
			}
			else if (type == RelationType.List)
			{
				closingTag = "</list>";
				AppendF("<list{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}>",
				        MakeAtt("name", name),
				        MakeAtt("access", accessString),
				        WriteIfNonNull("table", table),
				        WriteIfNonNull("schema", schema),
				        MakeAtt("lazy", lazy),
				        WriteIfTrue("inverse", inverse),
				        WriteIfNonNull("cascade", cascade),
				        WriteIfNonNull("where", where),
                        WriteIfNonNull("fetch", fetchString),
						WriteIfNotOne("batch-size", batchSize),
						WriteIfNonNull("collection-type", MakeTypeName(collectionType)));
			}


			Ident();

			WriteCache(cache, cacheregion);

			if (columnKey == null)
			{
				Append("<key>");
				Ident();
				WriteCompositeColumns(compositeKeyColumnKeys);
				Dedent();
				Append("</key>");
			}
			else
			{
				WriteKey(columnKey);
			}

			if (type == RelationType.Map || type == RelationType.List)
			{
				WriteIndex(index, indexType);
			}

			if (element != null || elementType != null)
			{
				WriteElement(element, elementType, targetType);
			}
			else if (columnRef == null && compositeKeyColumnRefs == null)
			{
				if (extraModel == null)
				{
					WriteOneToMany(targetType, notFoundMode);
				}
				else if (!extraModelVisited)
				{
					VisitNode(extraModel);
				}
			}
			else
			{
				if (columnRef != null)
				{
					WriteManyToMany(targetType, columnRef, notFoundMode);
				}
				else
				{
					WriteManyToMany(targetType, compositeKeyColumnRefs, notFoundMode);
				}
			}

			Dedent();

			Append(closingTag);
		}

		private static string TranslateNotFoundBehaviourEnum(NotFoundBehaviour notFoundBehaviourEnum)
		{
			switch(notFoundBehaviourEnum)
			{
				case NotFoundBehaviour.Default:
					return null;
				case NotFoundBehaviour.Exception:
					return "exception";
				case NotFoundBehaviour.Ignore:
					return "ignore";
				default:
					return null;
			}
		}

		private static string TranslateCascadeEnum(CascadeEnum cascadeEnum)
		{
			String cascade = null;

			if (cascadeEnum != CascadeEnum.None)
			{
				if (cascadeEnum == CascadeEnum.SaveUpdate)
				{
					cascade = "save-update";
				}
				else
				{
					cascade = cascadeEnum.ToString().ToLower();
				}
			}
			return cascade;
		}

		private static string TranslateCascadeEnum(ManyRelationCascadeEnum cascadeEnum)
		{
			String cascade = null;

			if (cascadeEnum != ManyRelationCascadeEnum.None)
			{
				if (cascadeEnum == ManyRelationCascadeEnum.SaveUpdate)
				{
					cascade = "save-update";
				}
				else if (cascadeEnum == ManyRelationCascadeEnum.AllDeleteOrphan)
				{
					cascade = "all-delete-orphan";
				}
				else
				{
					cascade = cascadeEnum.ToString().ToLower();
				}
			}
			return cascade;
		}

		private static string TranslateCacheEnum(CacheEnum cacheEnum)
		{
			if (cacheEnum == CacheEnum.ReadOnly)
			{
				return "read-only";
			}
			else if (cacheEnum == CacheEnum.ReadWrite)
			{
				return "read-write";
			}
			else
			{
				return "nonstrict-read-write";
			}
		}

		private static string TranslateFetch(FetchEnum fetch)
		{
			switch(fetch)
			{
				case FetchEnum.Select:
					return "select";
				case FetchEnum.Join:
					return "join";
				case FetchEnum.SubSelect:
					return "subselect";
				default:
					return null;
			}
		}

	    private String MakeTypeAtt(Type type, String typeName)
		{
			if (typeName != null)
			{
				return MakeAtt("type", typeName);
			}
			else
			{
				if (type.IsEnum)
				{
					return String.Empty;
				}
				if (type.IsPrimitive || type == typeof(String))
				{
					return MakeAtt("type", type.Name);
				}
				else
				{
					return MakeAtt("type", type.FullName);
				}
			}
		}

		private String MakeClassAtt(Type type)
		{
			if (type.IsEnum) return String.Empty;

			if (type.IsPrimitive)
			{
				return MakeAtt("class", type.Name);
			}
			else
			{
				return MakeAtt("class", MakeTypeName(type));
			}
		}

		private String MakeCustomTypeAttIfNotNull(String attName, Type type)
		{
			if (type == null) return String.Empty;

			return MakeCustomTypeAtt(attName, type);
		}

		private String MakeCustomTypeAtt(String attName, Type type)
		{
			if (type.IsEnum) return String.Empty;

			if (type.IsPrimitive)
			{
				return MakeAtt(attName, type.Name);
			}
			else if (typeof(object).Assembly == type.Assembly)
			{
				return MakeAtt(attName, type.FullName);
			}
			else
			{
				string assemblyName = new AssemblyName(type.Assembly.FullName).Name;
				return MakeAtt(attName, type.FullName + ", " + assemblyName);
			}
		}

		private void WriteElement(string element, string elementType, Type targetType)
		{
			if (element != null)
			{
				AppendF("<element {0} {1}/>",
						MakeAtt("column", element),
						MakeAtt("type", MakeTypeName(targetType)));
			}
			else
			{
				AppendF("<element {0} />",
						MakeAtt("type", elementType));
			}
		}

		private void WriteOneToMany(Type type, String notFoundMode)
		{
			AppendF("<one-to-many{0}{1} />", MakeAtt("class", MakeTypeName(type)), WriteIfNonNull("not-found", notFoundMode));
		}

		private void WriteManyToMany(Type type, String columnRef, String notFoundMode)
		{
			AppendF("<many-to-many{0}{1}{2}/>",
			        MakeAtt("class", MakeTypeName(type)),
			        MakeAtt("column", columnRef),
			        WriteIfNonNull("not-found", notFoundMode));
		}

		private void WriteManyToMany(Type type, String[] compositeKeyColumnRefs, String notFoundMode)
		{
			AppendF("<many-to-many{0}{1}>",
			        MakeAtt("class", MakeTypeName(type)),
			        WriteIfNonNull("not-found", notFoundMode));
			Ident();
			WriteCompositeColumns(compositeKeyColumnRefs);
			Dedent();
			Append("</many-to-many>");
		}

		private void WriteKey(String column)
		{
			AppendF("<key{0} />", MakeAtt("column", column));
		}

		private void WriteCompositeColumns(String[] columns)
		{
			foreach(string column in columns)
			{
				AppendF("<column{0} />", MakeAtt("name", column));
			}
		}

		private void WriteIndex(String column, String type)
		{
			AppendF("<index{0}{1} />", MakeAtt("column", column), WriteIfNonNull("type", type));
		}

		private void WriteCache(CacheEnum cacheEnum, string cacheregion)
		{
			if (cacheEnum != CacheEnum.Undefined)
			{
				AppendF("<cache{1} usage=\"{0}\" />", TranslateCacheEnum(cacheEnum), WriteIfNonNull("region", cacheregion));
			}
		}

		private void WriteDiscriminator(ActiveRecordModel model)
		{
			if (model.IsDiscriminatorBase)
			{
				AppendF("<discriminator{0}{1}{2} />",
						MakeAtt("column", model.ActiveRecordAtt.DiscriminatorColumn),
						WriteIfNonNull("type", model.ActiveRecordAtt.DiscriminatorType),
						WriteIfNonNull("length", model.ActiveRecordAtt.DiscriminatorLength)
						);
			}
		}

		private void WriteProperty(String name, Type propType, String accessString, String columnType,
		                           bool insert, bool update, String formula,
		                           String column, int length, bool notNull, bool unique,
		                           String uniqueKey, String sqlType, String index, String check, String @default)
		{
			BeginWriteProperty(accessString, columnType, formula, insert, name, propType, update);
			
			Ident();

			WriteColumn(check, column, index, length, notNull, sqlType, unique, uniqueKey, @default);
			
			Dedent();

			EndWriteProperty();
		}

		private void EndWriteProperty()
		{
			Append("</property>");
		}

		private void WriteColumn(string check, string column, string index, int length,
		                         bool notNull, string sqlType, bool unique, string uniqueKey, string @default)
		{
			AppendF("<column{0}{1}{2}{3}{4}{5}{6}{7}{8}/>",
			        MakeAtt("name", column),
			        WriteIfNotZero("length", length),
			        WriteIfTrue("not-null", notNull),
			        WriteIfTrue("unique", unique),
			        WriteIfNonNull("unique-key", uniqueKey),
			        WriteIfNonNull("sql-type", sqlType),
			        WriteIfNonNull("index", index),
			        WriteIfNonNull("check", check),
			        WriteIfNonNull("default", @default));
		}

		private void BeginWriteProperty(string accessString, string columnType, string formula, bool insert, string name, Type propType, bool update)
		{
			AppendF("<property{0}{1}{2}{3}{4}{5}>",
			        MakeAtt("name", name),
					WriteIfNonNull("access", accessString),
			        MakeTypeAtt(propType, columnType),
			        WriteIfFalse("insert", insert),
			        WriteIfFalse("update", update),
			        WriteIfNonNull("formula", formula));
		}

		#region Xml generations members

		private void CreateXmlPI()
		{
			Append(Constants.XmlPI);
		}

		private void StartMappingNode(bool useAutoImport)
		{
			AppendF(Constants.XmlHeader,
			        MakeAtt("auto-import", useAutoImport),
			        MakeAtt("default-lazy", ActiveRecordModel.isLazyByDefault));
		}

		private void EndMappingNode()
		{
			Append("</hibernate-mapping>");
		}

		/// <summary>
		/// Create a valid name from a type, without including all the version and public key
		/// information
		/// </summary>
		public static String MakeTypeName(Type type)
		{
			if (type == null) return null;

			string assemblyName = new AssemblyName(type.Assembly.FullName).Name;
			return String.Format("{0}, {1}", type.FullName, assemblyName);
		}

		#endregion

		#region String builder helpers

		private String ConditionalWrite(string attName, string value, bool condition)
		{
			if (condition)
			{
				return MakeAtt(attName, value);
			}
			return String.Empty;
		}

		private String WriteIfNonNull(String attName, String value)
		{
			if (value == null) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfTrue(String attName, bool value)
		{
			if (value == false) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfFalse(String attName, bool value)
		{
			if (value) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfNotZero(String attName, int value)
		{
			if (value == 0) return String.Empty;
			return MakeAtt(attName, value.ToString());
		}

        private String WriteIfNotOne(String attName, int value)
        {
            if (value == 1) return String.Empty;
            return MakeAtt(attName, value.ToString());
        }

		private String MakeAtt(String attName, String value)
		{
			return String.Format(" {0}=\"{1}\"", attName, value);
		}

		private String MakeAtt(String attName, bool value)
		{
			return String.Format(" {0}=\"{1}\"", attName, value.ToString().ToLower());
		}

		private String MakeAtt(String attName, bool value, bool output)
		{
			if (!output)
				return String.Empty;
			return String.Format(" {0}=\"{1}\"", attName, value.ToString().ToLower());
		}

		private void Append(string xml)
		{
			AppendIdentation();
			xmlBuilder.Append(xml);
			xmlBuilder.Append("\r\n");
		}

		private void AppendF(string xml, params object[] args)
		{
			AppendIdentation();
			xmlBuilder.AppendFormat(xml, args);
			xmlBuilder.Append("\r\n");
		}

		private void AppendIdentation()
		{
			for(int i = 0; i < identLevel; i++)
			{
				xmlBuilder.Append("  ");
			}
		}

		private void Ident()
		{
			identLevel++;
		}

		private void Dedent()
		{
			identLevel--;
		}

		#endregion

		#region Nested Class: TableScope

		private class TableScope : IDisposable
		{
			private readonly String currentTable;
			private readonly XmlGenerationVisitor visitor;

			public TableScope(XmlGenerationVisitor visitor, String newTable)
			{
				currentTable = visitor.currentTable;
				visitor.currentTable = newTable;
				this.visitor = visitor;
			}

			public void Dispose()
			{
				visitor.currentTable = currentTable;
			}
		}

		#endregion
	}
}

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
	using System.Text;

	using Castle.ActiveRecord;

	/// <summary>
	/// Traverse the tree emitting proper xml configuration
	/// </summary>
	public class XmlGenerationVisitor : AbstractDepthFirstVisitor
	{
		private StringBuilder xmlBuilder = new StringBuilder();
		private int identLevel = 0;

		public void Reset()
		{
			xmlBuilder.Length = 0;
		}

		public String Xml
		{
			get { return xmlBuilder.ToString(); }
		}

		public void CreateXml(ActiveRecordModel model)
		{
			CreateXmlPI();
			StartMappingNode();
			Ident();
			VisitModel(model);
			Dedent();
			EndMappingNode();

			if (ActiveRecordModel.isDebug) 
			{
				String file = model.Type.Name + ".mapping.xml";

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

		public override void VisitModel(ActiveRecordModel model)
		{
			VisitNodes(model.Imports);
			if (model.IsJoinedSubClass)
			{
				AppendF("<joined-subclass{0}{1}{2}{3}{4}{5}{6}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        MakeAtt("table", model.ActiveRecordAtt.Table),
				        WriteIfNonNull("schema", model.ActiveRecordAtt.Schema),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
                        WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
						MakeAtt("lazy", model.ActiveRecordAtt.Lazy),
						WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
                        WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert));
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
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</joined-subclass>");
			}
			else if (model.IsDiscriminatorSubClass)
			{
				AppendF("<subclass{0}{1}{2}{3}{4}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
				        MakeAtt("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
				        MakeAtt("lazy", model.ActiveRecordAtt.Lazy),
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
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</subclass>");
			}
			else if (model.IsNestedType)
			{
				Ident();
				VisitNodes(model.Fields);
				VisitNodes(model.Properties);
				VisitNodes(model.BelongsTo);
				VisitNodes(model.HasMany);
				VisitNodes(model.HasManyToAny);
				VisitNodes(model.HasAndBelongsToMany);
				VisitNodes(model.Components);
				VisitNodes(model.OneToOnes);
				Dedent();
			}
			else
			{
				AppendF("<class{0}{1}{2}{3}{4}{5}{6}{7}{8}>",
				        MakeAtt("name", MakeTypeName(model.Type)),
				        MakeAtt("table", model.ActiveRecordAtt.Table),
				        WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
				        WriteIfNonNull("schema", model.ActiveRecordAtt.Schema),
				        WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy)),
				        WriteIfNonNull("where", model.ActiveRecordAtt.Where),
				        MakeAtt("lazy", model.ActiveRecordAtt.Lazy),
                        WriteIfTrue("dynamic-update", model.ActiveRecordAtt.DynamicUpdate),
                        WriteIfTrue("dynamic-insert", model.ActiveRecordAtt.DynamicInsert));
				Ident();
				WriteCache(model.ActiveRecordAtt.Cache);
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
				VisitNodes(model.JoinedClasses);
				VisitNodes(model.Classes);
				Dedent();
				Append("</class>");
			}
		}

		public override void VisitPrimaryKey(PrimaryKeyModel model)
		{
			String unsavedVal = model.PrimaryKeyAtt.UnsavedValue;

			if (unsavedVal == null)
			{
				if (model.Property.PropertyType.IsPrimitive && model.Property.PropertyType != typeof(String))
				{
					unsavedVal = "0";
				}
				else if (model.Property.PropertyType != typeof(Guid))
				{
					// Nasty guess, but for 99.98% of situations it will be OK
					unsavedVal = "";
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
					className = model.PrimaryKeyAtt.Generator.ToString().ToLower();
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
					String[] pair = param.Split('=');

					AppendF("<param name=\"{0}\">{1}</param>", pair[0], pair[1]);
				}

				Dedent();
			}
			AppendF("</generator>");
			

			Dedent();
			Append("</id>");
		}

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

		public override void VisitImport(ImportModel model)
		{
			AppendF("<import{0}{1} />",
			        MakeAtt("class", MakeTypeName(model.ImportAtt.Type)),
			        MakeAtt("rename", model.ImportAtt.Rename));
		}

		public override void VisitProperty(PropertyModel model)
		{
			AppendF("<property{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.PropertyAtt.AccessString),
			        MakeAtt("column", model.PropertyAtt.Column),
			        MakeTypeAtt(model.Property.PropertyType, model.PropertyAtt.ColumnType),
			        WriteIfNotZero("length", model.PropertyAtt.Length),
			        WriteIfNonNull("unsaved-value", model.PropertyAtt.UnsavedValue),
			        WriteIfTrue("not-null", model.PropertyAtt.NotNull),
			        WriteIfTrue("unique", model.PropertyAtt.Unique),
			        WriteIfFalse("insert", model.PropertyAtt.Insert),
			        WriteIfFalse("update", model.PropertyAtt.Update),
			        WriteIfNonNull("formula", model.PropertyAtt.Formula));
		}

		public override void VisitAny(AnyModel model)
		{
			String cascade = TranslateCascadeEnum(model.AnyAtt.Cascade);

			AppendF("<any{0}{1}{2}{3}{4}{5}{6}{7}>",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.AnyAtt.AccessString),
			        MakeCustomTypeAtt("id-type", model.AnyAtt.IdType),
			        MakeCustomTypeAtt("meta-type", model.AnyAtt.MetaType),
			        WriteIfFalse("insert", model.AnyAtt.Insert),
			        WriteIfFalse("update", model.AnyAtt.Update),
			        WriteIfNonNull("index", model.AnyAtt.Index),
			        WriteIfNonNull("cascade", cascade));
			Ident();
			foreach(Any.MetaValueAttribute meta in model.MetaValues)
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

		public override void VisitHasManyToAny(HasManyToAnyModel model)
		{
			HasManyToAnyAttribute att = model.HasManyToAnyAtt;

			WriteCollection(att.Cascade, att.MapType, att.RelationType, model.Property.Name,
			                model.HasManyToAnyAtt.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey,null, null, null, null, model.Configuration, att.Index, att.IndexType,
			                att.Cache);
		}

		public override void VisitHasManyToAnyConfig(HasManyToAnyModel.Config model)
		{
			HasManyToAnyAttribute att = model.Parent.HasManyToAnyAtt;
			AppendF("<many-to-any{0}>",
					MakeCustomTypeAtt("id-type", att.IdType),
					MakeCustomTypeAttIfNotNull("meta-type", att.MetaType));
			Ident();
			AppendF("<column{0} />",
					MakeAtt("name", att.TypeColumn));
			AppendF("<column{0} />",
					MakeAtt("name", att.IdColumn));
			Dedent();
			AppendF("</many-to-any>");
		}

		public override void VisitField(FieldModel model)
		{
			AppendF("<property{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10} />",
			        MakeAtt("name", model.Field.Name),
			        MakeAtt("access", model.FieldAtt.AccessString),
			        MakeAtt("column", model.FieldAtt.Column),
			        MakeTypeAtt(model.Field.FieldType, model.FieldAtt.ColumnType),
			        WriteIfNotZero("length", model.FieldAtt.Length),
			        WriteIfNonNull("unsaved-value", model.FieldAtt.UnsavedValue),
			        WriteIfTrue("not-null", model.FieldAtt.NotNull),
			        WriteIfTrue("unique", model.FieldAtt.Unique),
			        WriteIfFalse("insert", model.FieldAtt.Insert),
			        WriteIfFalse("update", model.FieldAtt.Update),
			        WriteIfNonNull("formula", model.FieldAtt.Formula));
		}

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

		public override void VisitTimestamp(TimestampModel model)
		{
			AppendF("<timestamp{0}{1}{2} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.TimestampAtt.AccessString),
			        MakeAtt("column", model.TimestampAtt.Column));
		}

		public override void VisitKey(KeyModel model)
		{
			WriteKey(model.JoinedKeyAtt.Column);
		}

		public override void VisitOneToOne(OneToOneModel model)
		{
			String cascade = TranslateCascadeEnum(model.OneToOneAtt.Cascade);

			AppendF("<one-to-one{0}{1}{2}{3}{4}{5} />",
			        MakeAtt("name", model.Property.Name),
			        MakeAtt("access", model.OneToOneAtt.AccessString),
			        MakeAtt("class", MakeTypeName(model.OneToOneAtt.MapType)),
			        WriteIfNonNull("cascade", cascade),
			        WriteIfNonNull("property-ref", model.OneToOneAtt.PropertyRef),
			        WriteIfNonNull("fetch", TranslateFetch(model.OneToOneAtt.Fetch)),
			        WriteIfTrue("constrained", model.OneToOneAtt.Constrained));
		}

		public override void VisitBelongsTo(BelongsToModel model)
		{
			String cascade = TranslateCascadeEnum(model.BelongsToAtt.Cascade);
			String outerJoin = TranslateOuterJoin(model.BelongsToAtt.OuterJoin);

		    if (model.BelongsToAtt.Column == null)
            {
                AppendF("<many-to-one{0}{1}{2}{3}{4}{5}{6}{7}{8}>",
                        MakeAtt("name", model.Property.Name),
                        MakeAtt("access", model.BelongsToAtt.AccessString),
                        MakeAtt("class", MakeTypeName(model.BelongsToAtt.Type)),
                        WriteIfFalse("insert", model.BelongsToAtt.Insert),
                        WriteIfFalse("update", model.BelongsToAtt.Update),
                        WriteIfTrue("not-null", model.BelongsToAtt.NotNull),
                        WriteIfTrue("unique", model.BelongsToAtt.Unique),
                        WriteIfNonNull("cascade", cascade),
                        WriteIfNonNull("outer-join", outerJoin));
                Ident();
                WriteCompositeColumns(model.BelongsToAtt.CompositeKeyColumns);
                Dedent();
                Append("</many-to-one>");
		    }
		    else
            {
                AppendF("<many-to-one{0}{1}{2}{3}{4}{5}{6}{7}{8}{9} />",
                        MakeAtt("name", model.Property.Name),
                        MakeAtt("access", model.BelongsToAtt.AccessString),
                        MakeAtt("class", MakeTypeName(model.BelongsToAtt.Type)),
                        MakeAtt("column", model.BelongsToAtt.Column),
                        WriteIfFalse("insert", model.BelongsToAtt.Insert),
                        WriteIfFalse("update", model.BelongsToAtt.Update),
                        WriteIfTrue("not-null", model.BelongsToAtt.NotNull),
                        WriteIfTrue("unique", model.BelongsToAtt.Unique),
                        WriteIfNonNull("cascade", cascade),
                        WriteIfNonNull("outer-join", outerJoin));
		    }
		}

		public override void VisitHasMany(HasManyModel model)
		{
			HasManyAttribute att = model.HasManyAtt;

			WriteCollection(att.Cascade, att.MapType, att.RelationType, model.Property.Name,
			                model.HasManyAtt.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey, att.CompositeKeyColumnKeys, att.Element, null, null, null, att.Index, att.IndexType,
			                att.Cache);
		}

		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			HasAndBelongsToManyAttribute att = model.HasManyAtt;

			WriteCollection(att.Cascade, att.MapType, att.RelationType, model.Property.Name,
			                att.AccessString, att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy,
			                att.Where, att.Sort, att.ColumnKey, att.CompositeKeyColumnKeys, null, att.ColumnRef, 
			                att.CompositeKeyColumnRefs, model.CollectionID, att.Index, att.IndexType, att.Cache);
		}

		public override void VisitNested(NestedModel model)
		{
			AppendF("<component{0}{1}{2}{3}>",
			        MakeAtt("name", model.Property.Name),
			        WriteIfFalse("update", model.NestedAtt.Update),
			        WriteIfFalse("insert", model.NestedAtt.Insert),
					WriteIfNonNull("class", MakeTypeName(model.NestedAtt.MapType)));

			base.VisitNested(model);

			Append("</component>");
		}

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

		public override void VisitHilo(HiloModel model)
		{
			AppendF("<param name=\"table\">{0}</param>", model.HiloAtt.Table);
			AppendF("<param name=\"column\">{0}</param>", model.HiloAtt.Column);
			AppendF("<param name=\"max_lo\">{0}</param>", model.HiloAtt.MaxLo);
		}

		private void WriteCollection(ManyRelationCascadeEnum cascadeEnum,
																 Type targetType, RelationType type, string name,
																 string accessString, string table, string schema, bool lazy,
																 bool inverse, string orderBy, string where, string sort,
																 string columnKey, string[] compositeKeyColumnKeys, string element,
																 string columnRef, string[] compositeKeyColumnRefs,
																 IModelNode extraModel, string index, string indexType, CacheEnum cache)
		{
			String cascade = TranslateCascadeEnum(cascadeEnum);

			String closingTag = null;

			if (type == RelationType.Bag)
			{
				closingTag = "</bag>";

				AppendF("<bag{0}{1}{2}{3}{4}{5}{6}{7}{8}>",
								MakeAtt("name", name),
								MakeAtt("access", accessString),
								WriteIfNonNull("table", table),
								WriteIfNonNull("schema", schema),
								MakeAtt("lazy", lazy),
								WriteIfTrue("inverse", inverse),
								WriteIfNonNull("cascade", cascade),
								WriteIfNonNull("order-by", orderBy),
								WriteIfNonNull("where", where));
			}
			else if (type == RelationType.Set)
			{
				closingTag = "</set>";

				AppendF("<set{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}>",
								MakeAtt("name", name),
								MakeAtt("access", accessString),
								WriteIfNonNull("table", table),
								WriteIfNonNull("schema", schema),
								MakeAtt("lazy", lazy),
								WriteIfTrue("inverse", inverse),
								WriteIfNonNull("cascade", cascade),
								WriteIfNonNull("order-by", orderBy),
								WriteIfNonNull("where", where),
								WriteIfNonNull("sort", sort));
			}
			else if (type == RelationType.IdBag)
			{
				closingTag = "</idbag>";

				AppendF("<idbag{0}{1}{2}{3}{4}{5}{6}{7}>",
								MakeAtt("name", name),
								MakeAtt("access", accessString),
								WriteIfNonNull("table", table),
								WriteIfNonNull("schema", schema),
								MakeAtt("lazy", lazy),
								WriteIfNonNull("cascade", cascade),
								WriteIfNonNull("order-by", orderBy),
								WriteIfNonNull("where", where));

				VisitNode(extraModel);
			}
			else if (type == RelationType.Map)
			{
				closingTag = "</map>";

				AppendF("<map{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}>",
								MakeAtt("name", name),
								MakeAtt("access", accessString),
								WriteIfNonNull("table", table),
								WriteIfNonNull("schema", schema),
								MakeAtt("lazy", lazy),
								WriteIfTrue("inverse", inverse),
								WriteIfNonNull("cascade", cascade),
								WriteIfNonNull("order-by", orderBy),
								WriteIfNonNull("where", where),
								WriteIfNonNull("sort", sort));
			}
			else if (type == RelationType.List)
			{
				closingTag = "</list>";
				this.AppendF("<list{0}{1}{2}{3}{4}{5}{6}{7}>", 
				             MakeAtt("name", name), 
				             MakeAtt("access", accessString), 
				             WriteIfNonNull("table", table), 
				             WriteIfNonNull("schema", schema), 
				             MakeAtt("lazy", lazy), 
				             WriteIfTrue("inverse", inverse), 
				             WriteIfNonNull("cascade", cascade), 
				             WriteIfNonNull("where", where));
			}


			Ident();

			WriteCache(cache);
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

			if (element != null)
			{
				WriteElement(element, targetType);
			}
			else if (columnRef == null && compositeKeyColumnRefs == null)
			{
				if (extraModel == null)
				{
					WriteOneToMany(targetType);
				}
				else
				{
					VisitNode(extraModel);
				}
			}
			else
			{
				if (columnRef != null)
				{
					WriteManyToMany(targetType, columnRef);
				}
				else
				{
					WriteManyToMany(targetType, compositeKeyColumnRefs);
				}
			}

			Dedent();

			Append(closingTag);
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
				default:
					return null;
			}
		}
		
		private static string TranslateOuterJoin(OuterJoinEnum ojEnum)
		{
			String outerJoin = null;

			if (ojEnum != OuterJoinEnum.Auto)
			{
				outerJoin = ojEnum.ToString().ToLower();
			}
			
			return outerJoin;
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
			else
			{
				if (typeof(object).Assembly == type.Assembly)
				{
					return MakeAtt(attName, type.FullName);
				}
				string[] parts = type.AssemblyQualifiedName.Split(',');
				string name = string.Join(",", parts, 0, 2);
				return MakeAtt(attName, name);
			}
		}

		private void WriteElement(string element, Type targetType)
		{
			AppendF("<element {0} {1}/>",
					MakeAtt("column", element),
					MakeAtt("type", MakeTypeName(targetType)));
		}

		private void WriteOneToMany(Type type)
		{
			AppendF("<one-to-many{0} />", MakeAtt("class", MakeTypeName(type)));
		}

		private void WriteManyToMany(Type type, String columnRef)
		{
			AppendF("<many-to-many{0}{1} />",
							MakeAtt("class", MakeTypeName(type)),
							MakeAtt("column", columnRef));
		}

		private void WriteManyToMany(Type type, String[] compositeKeyColumnRefs)
		{
			AppendF("<many-to-many{0}>",
							MakeAtt("class", MakeTypeName(type)));
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
			foreach (string column in columns)
			{
				AppendF("<column{0} />", MakeAtt("name", column));
			}
		}

		private void WriteIndex(String column, String type)
		{
			AppendF("<index{0}{1} />", MakeAtt("column", column), WriteIfNonNull("type", type));
		}

		private void WriteCache(CacheEnum cacheEnum)
		{
			if (cacheEnum != CacheEnum.Undefined)
			{
				AppendF("<jcs-cache usage=\"{0}\" />", TranslateCacheEnum(cacheEnum));
			}
		}

		private void WriteDiscriminator(ActiveRecordModel model)
		{
			if (model.IsDiscriminatorBase)
			{
				AppendF("<discriminator{0}{1} />",
				        MakeAtt("column", model.ActiveRecordAtt.DiscriminatorColumn),
				        WriteIfNonNull("type", model.ActiveRecordAtt.DiscriminatorType));
			}
		}

		#region Xml generations members

		private void CreateXmlPI()
		{
			Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
		}

		private void StartMappingNode()
		{
			Append("<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">");
		}

		private void EndMappingNode()
		{
			Append("</hibernate-mapping>");
		}

		private String MakeTypeName(Type type)
		{
			if (type == null) return null;

			return String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}

		#endregion

		#region String builder helpers

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
			if (value == true) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfNotZero(String attName, int value)
		{
			if (value == 0) return String.Empty;
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
	}
}

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
	using System.Text;
	using System.Reflection;


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
		}

		public override void VisitModel(ActiveRecordModel model)
		{
			if (model.IsJoinedSubClass)
			{
				AppendF( "<joined-subclass {0} {1} {2} {3} {4}>",
					MakeAtt("name", MakeTypeName(model.Type)), 
					MakeAtt("table", model.ActiveRecordAtt.Table),
					WriteIfNonNull("schema", model.ActiveRecordAtt.Schema), 
					WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy) ),
					WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue) );
				Ident();
				VisitNode( model.Key );
				VisitNodes( model.Properties );
				VisitNodes( model.BelongsTo  );
				VisitNodes( model.HasMany );
				VisitNodes( model.HasAndBelongsToMany );
				VisitNodes( model.Components );
				VisitNodes( model.OneToOnes );
				VisitNodes( model.JoinedClasses );
				VisitNodes( model.Classes );
				Dedent();
				Append( "</joined-subclass>" );
			}
			else if (model.IsDiscriminatorSubClass)
			{
				AppendF( "<subclass {0} {1} {2}>",
					MakeAtt("name", MakeTypeName(model.Type)), 
					WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy) ),
					MakeAtt("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue) );
				Ident();
				VisitNodes( model.Properties );
				VisitNodes( model.BelongsTo  );
				VisitNodes( model.HasMany );
				VisitNodes( model.HasAndBelongsToMany );
				VisitNodes( model.Components );
				VisitNodes( model.OneToOnes );
				VisitNodes( model.JoinedClasses );
				VisitNodes( model.Classes );
				Dedent();
				Append( "</subclass>" );
			}
			else if (model.IsNestedType)
			{
				Ident();
				VisitNodes( model.Properties );
				VisitNodes( model.BelongsTo  );
				VisitNodes( model.HasMany );
				VisitNodes( model.HasAndBelongsToMany );
				VisitNodes( model.Components );
				VisitNodes( model.OneToOnes );
				Dedent();
			}
			else
			{
				AppendF( "<class {0} {1} {2} {3} {4} {5}>",
					MakeAtt("name", MakeTypeName(model.Type)), 
					MakeAtt("table", model.ActiveRecordAtt.Table),
					WriteIfNonNull("discriminator-value", model.ActiveRecordAtt.DiscriminatorValue),
					WriteIfNonNull("schema", model.ActiveRecordAtt.Schema), 
					WriteIfNonNull("proxy", MakeTypeName(model.ActiveRecordAtt.Proxy) ),
					WriteIfNonNull("where", model.ActiveRecordAtt.Where),
					WriteIfTrue("lazy", model.ActiveRecordAtt.Lazy));
				Ident();
				EnsureOnlyOneKey(model);
				VisitNodes( model.Ids );
				WriteDiscriminator(model);
				VisitNode( model.Version );
				VisitNode( model.Timestamp );
				VisitNodes( model.Properties );
				VisitNodes( model.BelongsTo  );
				VisitNodes( model.HasMany );
				VisitNodes( model.HasAndBelongsToMany );
				VisitNodes( model.Components );
				VisitNodes( model.OneToOnes );
				VisitNodes( model.JoinedClasses );
				VisitNodes( model.Classes );
				Dedent();
				Append( "</class>" );
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

			AppendF("<id {0} {1} {2} {3} {4}>",
				MakeAtt("name", model.Property.Name), 
				MakeAtt("column", model.PrimaryKeyAtt.Column),
				MakeTypeAtt(model.Property, model.PrimaryKeyAtt.ColumnType),
				WriteIfNotZero("length", model.PrimaryKeyAtt.Length), 
				WriteIfNonNull("unsaved-value", unsavedVal));

			Ident();

			if (model.PrimaryKeyAtt.Generator != PrimaryKeyType.None)
			{
				AppendF("<generator {0}>", MakeAtt("class", model.PrimaryKeyAtt.Generator.ToString().ToLower()));
				
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
			}

			Dedent();
			Append( "</id>" );
		}

		public override void VisitProperty(PropertyModel model)
		{
			AppendF("<property {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} />",
				MakeAtt("name", model.Property.Name), 
				MakeAtt("column", model.PropertyAtt.Column),
				MakeTypeAtt(model.Property, model.PropertyAtt.ColumnType),
				WriteIfNotZero("length", model.PropertyAtt.Length), 
				WriteIfNonNull("unsaved-value", model.PropertyAtt.UnsavedValue),
				WriteIfTrue("not-null", model.PropertyAtt.NotNull), 
				WriteIfTrue("unique", model.PropertyAtt.Unique), 
				WriteIfFalse("insert", model.PropertyAtt.Insert), 
				WriteIfFalse("update", model.PropertyAtt.Update),
				WriteIfNonNull("formula", model.PropertyAtt.Formula));
		}

		public override void VisitVersion(VersionModel model)
		{
			AppendF("<version {0} {1} {2} />", 
				MakeAtt("name", model.Property.Name),
				MakeAtt("column", model.VersionAtt.Column),
				MakeTypeAtt(model.Property, model.VersionAtt.Type));
		}

		public override void VisitTimestamp(TimestampModel model)
		{
			AppendF("<timestamp {0} {1} />", 
				MakeAtt("name", model.Property.Name),
				MakeAtt("column", model.TimestampAtt.Column));
		}

		public override void VisitKey(KeyModel model)
		{
			WriteKey(model.JoinedKeyAtt.Column);
		}

		public override void VisitOneToOne(OneToOneModel model)
		{
			String cascade = TranslateCascadeEnum(model.OneToOneAtt.Cascade);

			AppendF("<one-to-one {0} {1} />", 
				MakeAtt("name", model.Property.Name),
				MakeAtt("class", MakeTypeName(model.Property.PropertyType)), 
				WriteIfNonNull("cascade", cascade), 
				WriteIfNonNull("outer-join", TranslateOuterJoin(model.OneToOneAtt.OuterJoin)),
				WriteIfTrue("constrained", model.OneToOneAtt.Constrained));
		}

		public override void VisitBelongsTo(BelongsToModel model)
		{
			String cascade = TranslateCascadeEnum(model.BelongsToAtt.Cascade);
			String outerJoin = TranslateOuterJoin(model.BelongsToAtt.OuterJoin);

			AppendF("<many-to-one {0} {1} {2} {3} {4} {5} {6} {7} {8}/>", 
				MakeAtt("name", model.Property.Name),
				MakeAtt("class", MakeTypeName(model.BelongsToAtt.Type) ), 
				MakeAtt("column", model.BelongsToAtt.Column),
				WriteIfFalse("insert", model.BelongsToAtt.Insert), 
				WriteIfFalse("update", model.BelongsToAtt.Update),
				WriteIfTrue("not-null", model.BelongsToAtt.NotNull), 
				WriteIfTrue("unique", model.BelongsToAtt.Unique), 
				WriteIfNonNull("cascade", cascade), 
				WriteIfNonNull("outer-join", outerJoin) );
		}

		public override void VisitHasMany(HasManyModel model)
		{
			HasManyAttribute att = model.HasManyAtt;

			WriteCollection(att.Cascade, att.MapType, att.RelationType, model.Property.Name, 
				att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy, 
				att.Where, att.Sort, att.ColumnKey, null, null, att.Index, att.IndexType);
		}

		public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
		{
			HasAndBelongsToManyAttribute att = model.HasManyAtt;

			WriteCollection(att.Cascade, att.MapType, att.RelationType, model.Property.Name, 
				att.Table, att.Schema, att.Lazy, att.Inverse, att.OrderBy, 
				att.Where, att.Sort, att.ColumnKey, att.ColumnRef, model.CollectionID, 
				att.Index, att.IndexType);
		}

		public override void VisitNested(NestedModel model)
		{
			AppendF("<component {0} {1} {2}>", 
				MakeAtt("name", model.Property.Name),
				WriteIfFalse("update", model.NestedAtt.Update),
				WriteIfFalse("insert", model.NestedAtt.Insert));
			
			base.VisitNested(model);

			Append("</component>");
		}

		public override void VisitCollectionID(CollectionIDModel model)
		{
			AppendF("<collection-id {0} {1}>", 
				MakeAtt("type", model.CollectionIDAtt.ColumnType),
				MakeAtt("column", model.CollectionIDAtt.Column));
			Ident();
			AppendF("<generator {0}>", 
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

		private void WriteCollection(ManyRelationCascadeEnum cascadeEnum, Type targetType,
			RelationType type, String name, 
			String table, String schema, bool lazy, bool inverse, String orderBy, 
			String where, String sort, String columnKey, String columnRef, CollectionIDModel collectionId,
			String index, String indexType)
		{
			String cascade = TranslateCascadeEnum(cascadeEnum);

			String closingTag = null;
	
			if (type == RelationType.Bag)
			{
				closingTag = "</bag>";

				AppendF("<bag {0} {1} {2} {3} {4} {5} {6} {7} >", 
					MakeAtt("name", name),
					WriteIfNonNull("table", table ), 
					WriteIfNonNull("schema", schema ),
					WriteIfTrue("lazy", lazy), 
					WriteIfTrue("inverse", inverse), 
					WriteIfNonNull("cascade", cascade), 
					WriteIfNonNull("order-by", orderBy),
					WriteIfNonNull("where", where));
			}
			else if (type == RelationType.Set)
			{
				closingTag = "</set>";

				AppendF("<set {0} {1} {2} {3} {4} {5} {6} {7} {8}>", 
					MakeAtt("name", name),
					WriteIfNonNull("table", table ), 
					WriteIfNonNull("schema", schema ),
					WriteIfTrue("lazy", lazy), 
					WriteIfTrue("inverse", inverse), 
					WriteIfNonNull("cascade", cascade), 
					WriteIfNonNull("order-by", orderBy),
					WriteIfNonNull("where", where), 
					WriteIfNonNull("sort", sort));
			}
			else if (type == RelationType.IdBag)
			{
				closingTag = "</idbag>";

				AppendF("<idbag {0} {1} {2} {3} {4} {5} {6}>", 
					MakeAtt("name", name),
					WriteIfNonNull("table", table ), 
					WriteIfNonNull("schema", schema ),
					WriteIfTrue("lazy", lazy), 
					WriteIfNonNull("cascade", cascade), 
					WriteIfNonNull("order-by", orderBy),
					WriteIfNonNull("where", where));

				VisitNode( collectionId );
			}
			else if (type == RelationType.Map)
			{
				closingTag = "</map>";

				AppendF("<map {0} {1} {2} {3} {4} {5} {6} {7} {8}>", 
					MakeAtt("name", name),
					WriteIfNonNull("table", table ), 
					WriteIfNonNull("schema", schema ),
					WriteIfTrue("lazy", lazy), 
					WriteIfTrue("inverse", inverse), 
					WriteIfNonNull("cascade", cascade), 
					WriteIfNonNull("order-by", orderBy),
					WriteIfNonNull("where", where), 
					WriteIfNonNull("sort", sort));
			}
	
			Ident();
			
			WriteKey(columnKey);

			if (type == RelationType.Map)
			{
				WriteIndex(index, indexType);
			}

			if (columnRef == null)
			{
				WriteOneToMany(targetType);
			}
			else
			{
				WriteManyToMany(targetType, columnRef);
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

		private static string TranslateOuterJoin(OuterJoinEnum ojEnum)
		{
			String outerJoin = null;
	
			if (ojEnum != OuterJoinEnum.Auto)
			{
				outerJoin = ojEnum.ToString().ToLower();
			}
			return outerJoin;
		}

		private String MakeTypeAtt(PropertyInfo prop, String typeName)
		{
			if (prop.PropertyType.IsEnum) return String.Empty;

			if (typeName != null)
			{
				return MakeAtt("type", typeName);
			}
			else
			{
				if (prop.PropertyType.IsPrimitive)
				{
					return MakeAtt("type", prop.PropertyType.Name);
				}
				else
				{
					return MakeAtt("type", prop.PropertyType.FullName);
				}
			}
		}

		private void WriteOneToMany(Type type)
		{
			AppendF("<one-to-many {0} />", MakeAtt("class", MakeTypeName(type)));
		}

		private void WriteManyToMany(Type type, String columnRef)
		{
			AppendF("<many-to-many {0} {1} />", 
				MakeAtt("class", MakeTypeName(type)),
				MakeAtt("column", columnRef));
		}

		private void WriteKey(String column)
		{
			AppendF("<key {0} />", MakeAtt("column", column));
		}

		private void WriteIndex(String column, String type)
		{
			AppendF("<index {0} {1} />", MakeAtt("column", column), WriteIfNonNull("type", type));
		}

		private void EnsureOnlyOneKey(ActiveRecordModel model)
		{
			if (model.Ids.Count > 1)
			{
				throw new ActiveRecordException("Composite keys are not supported yet. Type " + model.Type.FullName);
			}
		}

		private void WriteDiscriminator(ActiveRecordModel model)
		{
			if (model.IsDiscriminatorBase)
			{
				AppendF("<discriminator {0} {1} />",
					MakeAtt("column", model.ActiveRecordAtt.DiscriminatorColumn),
					WriteIfNonNull("type", model.ActiveRecordAtt.DiscriminatorType) );
			}
		}

		#region Xml generations members

		private void CreateXmlPI()
		{
			Append( "<?xml version=\"1.0\" encoding=\"utf-16\"?>" );
		}

		private void StartMappingNode()
		{
			Append( "<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " + 
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">" );
		}

		private void EndMappingNode()
		{
			Append( "</hibernate-mapping>" );
		}

		private String MakeTypeName( Type type )
		{
			if (type == null) return null;

			return String.Format( "{0}, {1}", type.FullName, type.Assembly.GetName().Name );
		}

		#endregion

		#region String builder helpers

		private String WriteIfNonNull( String attName, String value )
		{
			if (value == null) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfTrue( String attName, bool value )
		{
			if (value == false) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfFalse( String attName, bool value )
		{
			if (value == true) return String.Empty;
			return MakeAtt(attName, value);
		}

		private String WriteIfNotZero( String attName, int value )
		{
			if (value == 0) return String.Empty;
			return MakeAtt(attName, value.ToString());
		}

		private String MakeAtt( String attName, String value )
		{
			return String.Format( "{0}=\"{1}\"", attName, value );
		}

		private String MakeAtt( String attName, bool value )
		{
			return String.Format( "{0}=\"{1}\"", attName, value.ToString().ToLower() );
		}

		private void Append(string xml)
		{
			AppendIdentation();
			xmlBuilder.Append( xml );
			xmlBuilder.Append( "\r\n" );
		}

		private void AppendF(string xml, params object[] args)
		{
			AppendIdentation();
			xmlBuilder.AppendFormat( xml, args );
			xmlBuilder.Append( "\r\n" );
		}

		private void AppendIdentation()
		{
			for(int i=0; i<identLevel; i++)
			{
				xmlBuilder.Append( "  " );
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

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

namespace Castle.ActiveRecord
{
	using System;
	using System.Configuration;
	using System.Reflection;
	using System.Text;
	using NHibernate;
	using NHibernate.Cfg;


	public class NHibernateMappingEngine
	{
		private static readonly String mappingOpen = "\r\n<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" {0}>";
		private static readonly String mappingClose = "\r\n</hibernate-mapping>";
		private static readonly String classOpen = "\r\n<class name=\"{0}\" {1}>";
		private static readonly String classClose = "\r\n</class>";
		private static readonly String tableAttribute = "table=\"{0}\" ";
		private static readonly String proxyAttribute = "proxy=\"{0}\" ";
		private static readonly String schemaAttribute = "schema=\"{0}\" ";
		private static readonly String idOpen = "\r\n<id {0}>";
		private static readonly String idClose = "\r\n</id>";
		private static readonly String nameAttribute = "name=\"{0}\" ";
		private static readonly String typeAttribute = "type=\"{0}\" ";
		private static readonly String classAttribute = "class=\"{0}\" ";
		private static readonly String generatorOpen = "\r\n<generator class=\"{0}\">";
		private static readonly String generatorClose = "\r\n</generator>";
		private static readonly String propertyOpen = "\r\n<property {0} {1}>";
		private static readonly String propertyClose = "\r\n</property>";
		private static readonly String updateAttribute = "update=\"{0}\" ";
		private static readonly String insertAttribute = "insert=\"{0}\" ";
		private static readonly String formulaAttribute = "formula=\"{0}\" ";
		private static readonly String columnAttribute = "column=\"{0}\" ";
		private static readonly String lengthAttribute = "length=\"{0}\" ";
		private static readonly String notNullAttribute = "not-null=\"{0}\" ";
		private static readonly String oneToOne = "\r\n<one-to-one name=\"{0}\" class=\"{1}\" {2}/>";
		private static readonly String oneToMany = "\r\n<one-to-many class=\"{0}\" />";
		private static readonly String manyToOne = "\r\n<many-to-one {0} {1} />";
		private static readonly String cascadeAttribute = "cascade=\"{0}\" ";
		private static readonly String outerJoinAttribute = "outer-join=\"{0}\" ";
		private static readonly String accessAttribute = "access=\"{0}\" ";
		private static readonly String unsavedValueAttribute = "unsaved-value=\"{0}\" ";
		private static readonly String constrainedAttribute = "constrained=\"{0}\" ";
		private static readonly String mapOpen = "\r\n<map name=\"{0}\" {1}>";
		private static readonly String mapClose = "\r\n</map>";
		private static readonly String listOpen = "\r\n<list name=\"{0}\" {1}>";
		private static readonly String listClose = "\r\n</list>";
		private static readonly String setOpen = "\r\n<set name=\"{0}\" {1}>";
		private static readonly String setClose = "\r\n</set>";
		private static readonly String bagOpen = "\r\n<bag name=\"{0}\" {1}>";
		private static readonly String bagClose = "\r\n</bag>";
		private static readonly String keyTag = "\r\n<key column=\"{0}\"/>";
		private static readonly String elementTag = "\r\n<element column=\"{0}\" class=\"{1}\"/>";
		private static readonly String indexTag = "\r\n<index column=\"{0}\" {1} />";
		private static readonly String lazyAttribute = "lazy=\"{0}\" ";
		private static readonly String inverseAttribute = "inverse=\"{0}\" ";
		private static readonly String sortAttribute = "sort=\"{0}\" ";
		private static readonly String orderByAttribute = "order-by=\"{0}\" ";
		private static readonly String whereAttribute = "where=\"{0}\" ";

		private Configuration _nhibernate;


		public void CreateMapping(Type type, Configuration config)
		{
			if (!type.IsDefined(typeof (ActiveRecordAttribute), true))
			{
				return;
			}

			_nhibernate = config;

			CreateMapping(type);
		}

//		private void RegisterActiveRecords( IConfiguration asm )
//		{
//			Assembly assembly = LoadAssembly( asm.Value );
//			foreach( Type type in assembly.GetTypes() )
//			{
//				if( type.GetCustomAttributes( typeof( ActiveRecordAttribute ), false ) != null )
//				{
//					_nhibernate.AddClass( type );
//					CreateMapping( type );
//				}
//			}
//		}

//		public void Init( IConfiguration engineConfig )
//		{
//			IConfiguration assemblies = engineConfig.Children["assemblies"];
//			if( assemblies == null )
//			{
//				throw new ConfigurationException( "The ActiveRecord facility requires an 'assemblies' configuration." );
//			}
//			_nhibernate = new Configuration();
//
//			foreach( IConfiguration asm in assemblies.Children )
//			{
//				RegisterActiveRecords( asm );
//			}
//		}
//
//		public ISessionFactory SessionFactory
//		{
//			get { return _nhibernate.BuildSessionFactory(); }
//		}

		private void CreateMapping(Type type)
		{
			ActiveRecordAttribute ar = GetActiveRecord(type);
			if (ar != null)
			{
				StringBuilder xml = new StringBuilder(String.Format(mappingOpen, ""));

				string table = (ar.Table == null ? "" : String.Format(tableAttribute, ar.Table));
				string schema = (ar.Schema == null ? "" : String.Format(schemaAttribute, ar.Schema));
				string proxy = (ar.Proxy == null ? "" : String.Format(proxyAttribute, ar.Proxy));

				xml.AppendFormat(classOpen, type.AssemblyQualifiedName, table + schema + proxy);

				AddMappedProperties(xml, type.GetProperties());

				xml.Append(classClose).Append(mappingClose);

				Console.WriteLine("XML Map: \r\n{0}\r\n", xml.ToString());

				_nhibernate.AddXmlString(xml.ToString());
			}
		}

		private void AddMappedProperties(StringBuilder builder, PropertyInfo[] props)
		{
			foreach (PropertyInfo prop in props)
			{
				object[] attributes = prop.GetCustomAttributes(false);
				foreach (object attribute in attributes)
				{
					PrimaryKeyAttribute pk = attribute as PrimaryKeyAttribute;
					if (pk != null)
					{
						AddPrimaryKeyMapping(prop, pk, builder);

						continue;
					}
					PropertyAttribute property = attribute as PropertyAttribute;
					if (property != null)
					{
						AddPropertyMapping(property, prop, builder);

						continue;
					}
					BelongsToAttribute belongs = attribute as BelongsToAttribute;
					if (belongs != null)
					{
						AddManyToOneMapping(prop, belongs, builder);

						continue;
					}
					HasManyAttribute hasmany = attribute as HasManyAttribute;
					if (hasmany != null)
					{
						// TODO: Inspect the return type to infer the 
						// mapping type

						AddBagMapping(prop, hasmany, builder);

//						if (hasmany.Key != null && hasmany.Index == null)
//						{
//							AddMapMapping(prop, hasmany, builder);
//						}
//						else if (hasmany.Index != null && hasmany.Key == null)
//						{
//							AddListMapping(prop, hasmany, builder);
//						}
//						else 
//						{
//							// AddSetMapping(prop, hasmany, builder);
//							AddBagMapping(prop, hasmany, builder);
//						}
						continue;
					}
					HasOneAttribute hasone = attribute as HasOneAttribute;
					if (hasone != null)
					{
						Type otherType = hasone.MapType;
						object[] otherAttributes = otherType.GetCustomAttributes(typeof (BelongsToAttribute), false);
						BelongsToAttribute inverse = null;
						foreach (object o in otherAttributes)
						{
							if (o is BelongsToAttribute)
							{
								inverse = o as BelongsToAttribute;
								break;
							}
						}
						if (inverse != null && inverse.Type == prop.DeclaringType)
						{
							AddOneToOneMapping(prop, hasone, builder);
						}
						// throw exception if no BelongsToAttribute?
						continue;
					}
				}
			}
		}

		private void AddOneToOneMapping(PropertyInfo prop, HasOneAttribute hasone, StringBuilder builder)
		{
			string name = prop.Name;
			string klass = String.Format(classAttribute, prop.PropertyType.Name);
			string cascade = (hasone.Cascade == null ? "" : String.Format(cascadeAttribute, hasone.Cascade));
			string outer = (hasone.OuterJoin == null ? "" : String.Format(outerJoinAttribute, hasone.OuterJoin));
			string constrained = (hasone.Constrained == null ? "" : String.Format(constrainedAttribute, hasone.Constrained));
			builder.AppendFormat(oneToOne, name, klass, cascade + outer + constrained);
		}

		private void AddSetMapping(PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder)
		{
			string name = prop.Name;
			string table = (hasmany.Table == null ? "" : String.Format(tableAttribute, hasmany.Table));
			string schema = (hasmany.Schema == null ? "" : String.Format(schemaAttribute, hasmany.Schema));
			string lazy = (hasmany.Lazy == null ? "" : String.Format(lazyAttribute, hasmany.Lazy));
			string inverse = (hasmany.Inverse == null ? "" : String.Format(inverseAttribute, hasmany.Inverse));
			string cascade = (hasmany.Cascade == null ? "" : String.Format(cascadeAttribute, hasmany.Cascade));
			string sort = (hasmany.Sort == null ? "" : String.Format(sortAttribute, hasmany.Sort));
			string orderBy = (hasmany.OrderBy == null ? "" : String.Format(orderByAttribute, hasmany.OrderBy));
			string where = (hasmany.Where == null ? "" : String.Format(whereAttribute, hasmany.Where));

			builder.AppendFormat(setOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where);

			Type otherType = hasmany.MapType;
			if (hasmany.Key != null)
			{
				PropertyInfo elementProp = otherType.GetProperty(hasmany.Key);
				if (elementProp != null)
				{
					builder.AppendFormat(keyTag, hasmany.Key);
					PropertyInfo indexProp = otherType.GetProperty(hasmany.Index);
					if (indexProp != null)
					{
						string type = String.Format(typeAttribute, indexProp.Name);
						builder.AppendFormat(indexTag, hasmany.Index, type);
					}
					string column = null;
					object[] elementAttributes = elementProp.GetCustomAttributes(false);
					foreach (object attribute in elementAttributes)
					{
						if (attribute is PropertyAttribute)
						{
							column = (attribute as PropertyAttribute).Column;
						}
						else if (attribute is PrimaryKeyAttribute)
						{
							column = (attribute as PrimaryKeyAttribute).Column;
						}
						else if (attribute is BelongsToAttribute)
						{
							column = (attribute as BelongsToAttribute).Column;
						}
					}
					if (column != null)
					{
						builder.AppendFormat(elementTag, column, otherType.Name);
					}
				}
			}
			builder.Append(setClose);
		}

		private void AddListMapping(PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder)
		{
			string name = prop.Name;
			string table = (hasmany.Table == null ? "" : String.Format(tableAttribute, hasmany.Table));
			string schema = (hasmany.Schema == null ? "" : String.Format(schemaAttribute, hasmany.Schema));
			string lazy = (hasmany.Lazy == null ? "" : String.Format(lazyAttribute, hasmany.Lazy));
			string inverse = (hasmany.Inverse == null ? "" : String.Format(inverseAttribute, hasmany.Inverse));
			string cascade = (hasmany.Cascade == null ? "" : String.Format(cascadeAttribute, hasmany.Cascade));
			string sort = (hasmany.Sort == null ? "" : String.Format(sortAttribute, hasmany.Sort));
			string orderBy = (hasmany.OrderBy == null ? "" : String.Format(orderByAttribute, hasmany.OrderBy));
			string where = (hasmany.Where == null ? "" : String.Format(whereAttribute, hasmany.Where));

			builder.AppendFormat(listOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where);

			Type otherType = hasmany.MapType;
			PropertyInfo indexProp = otherType.GetProperty(hasmany.Index);
			if (indexProp != null)
			{
				string type = String.Format(typeAttribute, indexProp.Name);
				builder.AppendFormat(indexTag, hasmany.Index, type);
//				PropertyInfo elementProp = otherType.GetProperty( hasmany.Key );
//				if( elementProp != null )
//				{
//					builder.AppendFormat( keyTag, hasmany.Key );
//				}
				string column = null;
				object[] elementAttributes = indexProp.GetCustomAttributes(false);
				foreach (object attribute in elementAttributes)
				{
					if (attribute is PropertyAttribute)
					{
						column = (attribute as PropertyAttribute).Column;
					}
					else if (attribute is PrimaryKeyAttribute)
					{
						column = (attribute as PrimaryKeyAttribute).Column;
					}
					else if (attribute is BelongsToAttribute)
					{
						column = (attribute as BelongsToAttribute).Column;
					}
				}
				if (column != null)
				{
					builder.AppendFormat(elementTag, column, otherType.Name);
				}
			}
			builder.Append(listClose);
		}

		private void AddMapMapping(PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder)
		{
			string name = prop.Name;
			string table = (hasmany.Table == null ? "" : String.Format(tableAttribute, hasmany.Table));
			string schema = (hasmany.Schema == null ? "" : String.Format(schemaAttribute, hasmany.Schema));
			string lazy = (hasmany.Lazy == null ? "" : String.Format(lazyAttribute, hasmany.Lazy));
			string inverse = (hasmany.Inverse == null ? "" : String.Format(inverseAttribute, hasmany.Inverse));
			string cascade = (hasmany.Cascade == null ? "" : String.Format(cascadeAttribute, hasmany.Cascade));
			string sort = (hasmany.Sort == null ? "" : String.Format(sortAttribute, hasmany.Sort));
			string orderBy = (hasmany.OrderBy == null ? "" : String.Format(orderByAttribute, hasmany.OrderBy));
			string where = (hasmany.Where == null ? "" : String.Format(whereAttribute, hasmany.Where));

			builder.AppendFormat(mapOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where);

			Type otherType = hasmany.MapType;
			PropertyInfo elementProp = otherType.GetProperty(hasmany.Key);
			if (elementProp != null)
			{
				builder.AppendFormat(keyTag, hasmany.Key);
				string column = null;
				object[] elementAttributes = elementProp.GetCustomAttributes(false);
				foreach (object attribute in elementAttributes)
				{
					if (attribute is PropertyAttribute)
					{
						column = (attribute as PropertyAttribute).Column;
					}
					else if (attribute is PrimaryKeyAttribute)
					{
						column = (attribute as PrimaryKeyAttribute).Column;
					}
					else if (attribute is BelongsToAttribute)
					{
						column = (attribute as BelongsToAttribute).Column;
					}
				}
				if (column != null)
				{
					builder.AppendFormat(elementTag, column, otherType.Name);
				}
			}
			builder.Append(mapClose);
		}

		private void AddManyToOneMapping(PropertyInfo prop, BelongsToAttribute belongs, StringBuilder builder)
		{
			string name = String.Format(nameAttribute, prop.Name);
			string klass = String.Format(classAttribute, prop.PropertyType.AssemblyQualifiedName);
			string column = (belongs.Column == null ? "" : String.Format(columnAttribute, belongs.Column));
			string cascade = (belongs.Cascade == null ? "" : String.Format(cascadeAttribute, belongs.Cascade));
			string outer = (belongs.OuterJoin == null ? "" : String.Format(outerJoinAttribute, belongs.OuterJoin));
			string update = (belongs.Update == null ? "" : String.Format(updateAttribute, belongs.Update));
			string insert = (belongs.Insert == null ? "" : String.Format(insertAttribute, belongs.Insert));

			builder.AppendFormat(manyToOne, name, klass + column + cascade + outer + update + insert);
		}

		private void AddPropertyMapping(PropertyAttribute property, PropertyInfo prop, StringBuilder builder)
		{
			string column = (property.Column == null ? "" : String.Format(columnAttribute, property.Column));
			string update = (property.Update == null ? "" : String.Format(updateAttribute, property.Update));
			string insert = (property.Insert == null ? "" : String.Format(insertAttribute, property.Insert));
			string formula = (property.Formula == null ? "" : String.Format(formulaAttribute, property.Formula));
			string length = (property.Length == null ? "" : String.Format(lengthAttribute, property.Length));
			string notNull = (property.NotNull == null ? "" : String.Format(notNullAttribute, property.NotNull));
			string name = String.Format(nameAttribute, prop.Name);
			string type = String.Format(typeAttribute, prop.PropertyType.Name);

			builder.AppendFormat(propertyOpen, name, type + column + update + insert + formula + length + notNull);
			builder.Append(propertyClose);
		}

		private void AddPrimaryKeyMapping(PropertyInfo prop, PrimaryKeyAttribute pk, StringBuilder builder)
		{
			string name = String.Format(nameAttribute, prop.Name);
			string type = String.Format(typeAttribute, prop.PropertyType.Name);
			string column = (pk.Column == null ? "" : String.Format(columnAttribute, pk.Column));
			string unsavedValue = (pk.UnsavedValue == null ? "" : String.Format(unsavedValueAttribute, pk.UnsavedValue));
			string access = (pk.Access == null ? "" : String.Format(accessAttribute, pk.Access));

			builder.AppendFormat(idOpen, name + type + column + unsavedValue + access);

			if (pk.Generator != PrimaryKeyType.None)
			{
				builder.AppendFormat(generatorOpen, pk.Generator.ToString().ToLower());
				builder.Append(generatorClose);
			}

			builder.Append(idClose);
		}

		private ActiveRecordAttribute GetActiveRecord(MemberInfo klass)
		{
			foreach (Attribute attribute in klass.GetCustomAttributes(false))
			{
				if (attribute is ActiveRecordAttribute)
				{
					return attribute as ActiveRecordAttribute;
				}
			}
			return null;
		}
//
//		private Assembly LoadAssembly(string assembly)
//		{
//			try
//			{
//				return Assembly.Load(assembly);
//			}
//			catch (Exception e)
//			{
//				throw new ConfigurationException(String.Format("Assembly '{0}' could not be loaded.", assembly), e);
//			}
//		}
//
		private void AddBagMapping(PropertyInfo prop, HasManyAttribute hasmany, StringBuilder builder)
		{
			string name = prop.Name;
			string table = (hasmany.Table == null ? "" : String.Format(tableAttribute, hasmany.Table));
			string schema = (hasmany.Schema == null ? "" : String.Format(schemaAttribute, hasmany.Schema));
			string lazy = (hasmany.Lazy == null ? "" : String.Format(lazyAttribute, hasmany.Lazy));
			string inverse = (hasmany.Inverse == null ? "" : String.Format(inverseAttribute, hasmany.Inverse));
			string cascade = (hasmany.Cascade == null ? "" : String.Format(cascadeAttribute, hasmany.Cascade));
			string sort = (hasmany.Sort == null ? "" : String.Format(sortAttribute, hasmany.Sort));
			string orderBy = (hasmany.OrderBy == null ? "" : String.Format(orderByAttribute, hasmany.OrderBy));
			string where = (hasmany.Where == null ? "" : String.Format(whereAttribute, hasmany.Where));

			builder.AppendFormat(bagOpen, name, table + schema + lazy + inverse + cascade + sort + orderBy + where);

			Type otherType = hasmany.MapType;
			
			if (hasmany.Key == null)
			{
				throw new ConfigurationException("HasManyAttribute must expose a Key");
			}

//			PropertyInfo elementProp = otherType.GetProperty(hasmany.Key);
			
//			if (elementProp != null)
			{
//				PropertyInfo indexProp = otherType.GetProperty(hasmany.Index);
//				if (indexProp != null)
//				{
//					string type = String.Format(typeAttribute, indexProp.Name);
//					builder.AppendFormat(indexTag, hasmany.Index, type);
//				}

				string column = hasmany.Column == null ? "" : hasmany.Column;

//				object[] elementAttributes = elementProp.GetCustomAttributes(false);
//				foreach (object attribute in elementAttributes)
//				{
//					if (attribute is PropertyAttribute)
//					{
//						column = (attribute as PropertyAttribute).Column;
//						break;
//					}
//					else if (attribute is PrimaryKeyAttribute)
//					{
//						column = (attribute as PrimaryKeyAttribute).Column;
//						break;
//					}
//					else if (attribute is BelongsToAttribute)
//					{
//						column = (attribute as BelongsToAttribute).Column;
//						break;
//					}
//				}

				builder.AppendFormat(keyTag, column);

				// We need to choose from element, one-to-many, many-to-many, composite-element, many-to-any
				// We need to do it wisely
				if (column != null)
				{
					builder.AppendFormat(oneToMany, otherType.AssemblyQualifiedName);
				}
			}
		
			builder.Append(bagClose);
		}
	}
}
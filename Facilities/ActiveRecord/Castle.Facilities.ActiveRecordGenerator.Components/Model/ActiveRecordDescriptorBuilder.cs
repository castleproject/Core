using System.Data.OleDb;
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

namespace Castle.Facilities.ActiveRecordGenerator.Model
{
	using System;

	using Castle.Facilities.ActiveRecordGenerator.CodeGenerator;


	public class ActiveRecordDescriptorBuilder
	{
		private INamingService _naming;

		public ActiveRecordDescriptorBuilder(INamingService naming)
		{
			_naming = naming;
		}

		public ActiveRecordDescriptor Build(TableDefinition tableDef)
		{
			ActiveRecordDescriptor descriptor = new ActiveRecordDescriptor( 
				_naming.CreateClassName(tableDef.Name), tableDef.Name );

			foreach(ColumnDefinition column in tableDef.Columns)
			{
				descriptor.AddProperty( CreateActiveRecordProperty(column) );
			}

			return descriptor;
		}

		protected ActiveRecordPropertyDescriptor CreateActiveRecordProperty(ColumnDefinition column)
		{
			ActiveRecordPropertyDescriptor desc = new ActiveRecordPropertyDescriptor( 
				column.Name, null, 
				_naming.CreatePropertyName(column.Name), 
				_naming.CreateFieldName(column.Name), 
				ConvertOleType(column.Type), column.Nullable );

			return desc;
		}

		private Type ConvertOleType(OleDbType type)
		{
			switch(type)
			{
				case OleDbType.BigInt:
					return typeof(Int64);

				case OleDbType.LongVarBinary:
				case OleDbType.Binary:
					return typeof(Byte);
				
				case OleDbType.Boolean:
					return typeof(Boolean);
				
				case OleDbType.BSTR:
				case OleDbType.Char:
				case OleDbType.VarChar:
				case OleDbType.LongVarChar:
				case OleDbType.LongVarWChar:
					return typeof(String);
				
				case OleDbType.Currency:
				case OleDbType.Decimal:
				case OleDbType.Numeric:
					return typeof(Decimal);
				
				case OleDbType.Date:
				case OleDbType.DBDate:
				case OleDbType.DBTimeStamp:
				case OleDbType.Filetime:
					return typeof(DateTime);
				
				case OleDbType.DBTime:
					return typeof(TimeSpan);
				
				case OleDbType.Double:
					return typeof(Double);
				
				case OleDbType.Guid:
					return typeof(Guid);
				
				case OleDbType.Integer:
					return typeof(Int32);
				
				case OleDbType.PropVariant:
					return typeof(Object);

				case OleDbType.Single:
					return typeof(Single);

				case OleDbType.SmallInt:
					return typeof(Int16);

				case OleDbType.TinyInt:
					return typeof(SByte);

				case OleDbType.UnsignedBigInt:
					return typeof(UInt64);

				case OleDbType.UnsignedInt:
					return typeof(UInt32);

				case OleDbType.UnsignedSmallInt:
					return typeof(UInt16);

				case OleDbType.UnsignedTinyInt:
					return typeof(Byte);

				case OleDbType.VarBinary:
					return typeof(Byte);

				case OleDbType.Variant:
					return typeof(Object);

				case OleDbType.VarNumeric:
					return typeof(Decimal);

				default:
					throw new ArgumentException("OleDbType not supported");
			}
		}
	}
}

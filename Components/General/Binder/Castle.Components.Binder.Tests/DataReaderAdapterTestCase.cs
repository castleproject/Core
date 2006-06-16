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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections;
	using System.Data;
	using Nullables;
	using NUnit.Framework;

	[TestFixture]
	public class DataReaderAdapterTestCase
	{
		[Test]
		public void EmptyReader()
		{
			ArrayList data = new ArrayList();

			MockDataReader reader = new MockDataReader(data, new string[] { "name", "address"});
			
			DataReaderAdapter adapter = new DataReaderAdapter(reader);

			DataBinder binder = new DataBinder();

			Contact contact = (Contact) binder.BindObject(typeof(Contact), "", adapter);

			Assert.IsNotNull(contact);
			Assert.IsNull(contact.Address);
			Assert.IsNull(contact.Name);
		}

		[Test]
		public void SingleRow()
		{
			ArrayList data = new ArrayList();

			data.Add( new object[] { "hammett", "r pereira leite 44" } );

			MockDataReader reader = new MockDataReader(data, new string[] { "name", "address"});
			
			DataReaderAdapter adapter = new DataReaderAdapter(reader);

			DataBinder binder = new DataBinder();

			Contact contact = (Contact) binder.BindObject(typeof(Contact), "", adapter);

			Assert.IsNotNull(contact);
			Assert.AreEqual("hammett", contact.Name);
			Assert.AreEqual("r pereira leite 44", contact.Address);
		}

		[Test]
		public void UsingTypeConverter()
		{
			ArrayList data = new ArrayList();

			data.Add( new object[] { "hammett", "r pereira leite 44", new DateTime(2006,7,16) } );

			MockDataReader reader = new MockDataReader(data, new string[] { "name", "address", "dob" });
			
			DataReaderAdapter adapter = new DataReaderAdapter(reader);

			DataBinder binder = new DataBinder();

			Contact contact = (Contact) binder.BindObject(typeof(Contact), "", adapter);

			Assert.IsNotNull(contact);
			Assert.AreEqual("hammett", contact.Name);
			Assert.AreEqual("r pereira leite 44", contact.Address);
			Assert.IsTrue(contact.DOB.HasValue);
		}

		[Test]
		public void TwoRows()
		{
			ArrayList data = new ArrayList();

			data.Add( new object[] { 26, "hammett", "r pereira leite 44" } );
			data.Add( new object[] { 27, "his girl", "barao bananal 100" } );

			MockDataReader reader = new MockDataReader(data, new string[] { "age", "name", "address"});
			
			DataReaderAdapter adapter = new DataReaderAdapter(reader);

			DataBinder binder = new DataBinder();

			Contact[] contacts = (Contact[]) binder.BindObject(typeof(Contact[]), "", adapter);

			Assert.IsNotNull(contacts);
			Assert.AreEqual(2, contacts.Length);
			
			Assert.AreEqual(26, contacts[0].Age);
			Assert.AreEqual("hammett", contacts[0].Name);
			Assert.AreEqual("r pereira leite 44", contacts[0].Address);

			Assert.AreEqual(27, contacts[1].Age);
			Assert.AreEqual("his girl", contacts[1].Name);
			Assert.AreEqual("barao bananal 100", contacts[1].Address);
		}

		[Test]
		public void UsingTranslator()
		{
			ArrayList data = new ArrayList();

			data.Add( new object[] { 26, "hammett", "r pereira leite 44" } );
			data.Add( new object[] { 27, "his girl", "barao bananal 100" } );

			MockDataReader reader = new MockDataReader(data, new string[] { "idade", "nome", "endereco"});
			
			DataReaderAdapter adapter = new DataReaderAdapter(reader);

			DataBinder binder = new DataBinder(new EnglishToPortugueseTranslator());

			Contact[] contacts = (Contact[]) binder.BindObject(typeof(Contact[]), "", adapter);

			Assert.IsNotNull(contacts);
			Assert.AreEqual(2, contacts.Length);
			
			Assert.AreEqual(26, contacts[0].Age);
			Assert.AreEqual("hammett", contacts[0].Name);
			Assert.AreEqual("r pereira leite 44", contacts[0].Address);

			Assert.AreEqual(27, contacts[1].Age);
			Assert.AreEqual("his girl", contacts[1].Name);
			Assert.AreEqual("barao bananal 100", contacts[1].Address);
		}
	}

	public class EnglishToPortugueseTranslator : IBinderTranslator
	{
		/// <param name="paramName">The property name in the target type</param>
		/// <returns>A name of the source data that should be used to populate the property</returns>
		public String Translate(Type instanceType, String paramName)
		{
			if (paramName == "Age")
			{
				return "idade";
			}
			else if (paramName == "Name")
			{
				return "nome";
			}
			else if (paramName == "Address")
			{
				return "endereco";
			}

			return null;
		}
	}

	public class MockDataReader : IDataReader
	{
		private readonly IList source;
		private readonly string[] fields;
		private int index = -1;

		public MockDataReader(IList source, String[] fields)
		{
			this.source = source;
			this.fields = fields;
		}

		#region IDataReader

		public void Close()
		{
			throw new NotImplementedException();
		}

		public bool NextResult()
		{
			throw new NotImplementedException();
		}

		public bool Read()
		{
			if (index + 1 < source.Count)
			{
				index++;
				return true;
			}
			
			return false;
		}

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		public int Depth
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public int RecordsAffected
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDataRecord

		public string GetName(int i)
		{
			return fields[i];
		}

		public string GetDataTypeName(int i)
		{
			throw new NotImplementedException();
		}

		public Type GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		public object GetValue(int i)
		{
			return ((object[]) source[index])[i];
		}

		public int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public int GetOrdinal(string name)
		{
			return Array.IndexOf(fields, name.ToLower());
		}

		public bool GetBoolean(int i)
		{
			return Convert.ToBoolean( GetValue(i) );
		}

		public byte GetByte(int i)
		{
			return Convert.ToByte( GetValue(i) );
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public char GetChar(int i)
		{
			return Convert.ToChar( GetValue(i) );
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		public short GetInt16(int i)
		{
			return Convert.ToInt16( GetValue(i) );
		}

		public int GetInt32(int i)
		{
			return Convert.ToInt32( GetValue(i) );
		}

		public long GetInt64(int i)
		{
			return Convert.ToInt64( GetValue(i) );
		}

		public float GetFloat(int i)
		{
			return Convert.ToSingle( GetValue(i) );
		}

		public double GetDouble(int i)
		{
			return Convert.ToDouble( GetValue(i) );
		}

		public string GetString(int i)
		{
			return Convert.ToString( GetValue(i) );
		}

		public decimal GetDecimal(int i)
		{
			return Convert.ToDecimal( GetValue(i) );
		}

		public DateTime GetDateTime(int i)
		{
			return Convert.ToDateTime( GetValue(i) );
		}

		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		public bool IsDBNull(int i)
		{
			return GetValue(i) == null;
		}

		public int FieldCount
		{
			get { return fields.Length; }
		}

		public object this[int i]
		{
			get { throw new NotImplementedException(); }
		}

		public object this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	public class Contact
	{
		private int age;
		private String name, address;
		private NullableDateTime dob;

		public int Age
		{
			get { return age; }
			set { age = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		public NullableDateTime DOB
		{
			get { return dob; }
			set { dob = value; }
		}
	}
}

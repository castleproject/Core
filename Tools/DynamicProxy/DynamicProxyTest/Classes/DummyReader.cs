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

namespace Castle.DynamicProxy.Test.Classes
{
	using System;
	using System.Data;


	public class DummyReader : IDataReader
	{
		public DummyReader()
		{
		}

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
			throw new NotImplementedException();
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

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public string GetName(int i)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public int GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

		public bool GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		public byte GetByte(int i)
		{
			throw new NotImplementedException();
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public char GetChar(int i)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public int GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		public long GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		public double GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		public string GetString(int i)
		{
			throw new NotImplementedException();
		}

		public decimal GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		public DateTime GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		public bool IsDBNull(int i)
		{
			throw new NotImplementedException();
		}

		public int FieldCount
		{
			get { throw new NotImplementedException(); }
		}

		public object this[int i]
		{
			get { throw new NotImplementedException(); }
		}

		public object this[string name]
		{
			get { throw new NotImplementedException(); }
		}
	}
}

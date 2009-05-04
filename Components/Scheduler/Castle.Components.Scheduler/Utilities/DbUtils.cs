// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Scheduler.Utilities
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters;
	using System.Runtime.Serialization.Formatters.Binary;

	/// <summary>
	/// Provides utility functions for working with a database.
	/// </summary>
	public static class DbUtils
	{
		private static readonly BinaryFormatter formatter;

		static DbUtils()
		{
			formatter = new BinaryFormatter();
			formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			formatter.FilterLevel = TypeFilterLevel.Low;
			formatter.TypeFormat = FormatterTypeStyle.TypesAlways;
		}

		/// <summary>
		/// Maps a Db value to an object.
		/// <see cref="DBNull.Value"/> is mapped to null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value to map</param>
		/// <returns>The mapped value</returns>
		public static T MapDbValueToObject<T>(object value) where T : class
		{
			if (value == DBNull.Value)
				return null;
			return (T) value;
		}

		/// <summary>
		/// Maps a Db value to a nullable structure.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value to map</param>
		/// <returns>The mapped value</returns>
		public static T? MapDbValueToNullable<T>(object value) where T : struct
		{
			if (value == null || value == DBNull.Value)
				return null;
			return (T?) value;
		}

		/// <summary>
		/// Maps a nullable structure to a Db value.
		/// Null is mapped to <see cref="DBNull.Value" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value to map</param>
		/// <returns>The mapped value</returns>
		public static object MapNullableToDbValue<T>(T? value) where T : struct
		{
			if (!value.HasValue)
				return DBNull.Value;
			return value.Value;
		}

		/// <summary>
		/// Maps an object to a Db value.
		/// Null is mapped to <see cref="DBNull.Value" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value to map</param>
		/// <returns>The mapped value</returns>
		public static object MapObjectToDbValue<T>(T value) where T : class
		{
			if (value == null)
				return DBNull.Value;
			return value;
		}

		/// <summary>
		/// Serializes an object to a byte array for storage in a BLOB.
		/// </summary>
		/// <param name="obj">The object to serialize, possibly null</param>
		/// <returns>The byte array, possibly null</returns>
		public static byte[] SerializeObject(object obj)
		{
			if (obj == null)
				return null;

			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, obj);
			return stream.ToArray();
		}

		/// <summary>
		/// Deserializes an object from a byte array for retrieval from a BLOB.
		/// </summary>
		/// <param name="bytes">The byte array to deserialize, possibly null</param>
		/// <returns>The object, possibly null</returns>
		public static object DeserializeObject(byte[] bytes)
		{
			if (bytes == null)
				return null;

			MemoryStream stream = new MemoryStream(bytes);
			return formatter.Deserialize(stream);
		}
	}
}
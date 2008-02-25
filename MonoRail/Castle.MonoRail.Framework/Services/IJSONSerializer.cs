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

namespace Castle.MonoRail.Framework.Services
{
	using System;

	/// <summary>
	/// Abstracts the underlying JSON writer
	/// </summary>
	public interface IJSONWriter
	{
		/// <summary>
		/// Writes the start object.
		/// </summary>
		void WriteStartObject();
		/// <summary>
		/// Writes the end object.
		/// </summary>
		void WriteEndObject();
		/// <summary>
		/// Writes the start array.
		/// </summary>
		void WriteStartArray();
		/// <summary>
		/// Writes the end array.
		/// </summary>
		void WriteEndArray();
		/// <summary>
		/// Writes the name of the property.
		/// </summary>
		/// <param name="name">The name.</param>
		void WritePropertyName(string name);
		/// <summary>
		/// Writes the end.
		/// </summary>
		void WriteEnd();
		/// <summary>
		/// Writes the null.
		/// </summary>
		void WriteNull();
		/// <summary>
		/// Writes the undefined.
		/// </summary>
		void WriteUndefined();
		/// <summary>
		/// Writes the raw.
		/// </summary>
		/// <param name="javaScript">The java script.</param>
		void WriteRaw(string javaScript);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(object value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(string value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(int value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(long value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(float value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		void WriteValue(bool value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(short value);
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(decimal value);

		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="value">The value.</param>
		void WriteValue(DateTime value);
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IJSONConverter
	{
		/// <summary>
		/// Writes the specified value using the writer.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="value">The value.</param>
		void Write(IJSONWriter writer, object value);

		/// <summary>
		/// Determines whether this instance can handle the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can handle the specified type; otherwise, <c>false</c>.
		/// </returns>
		bool CanHandle(Type type);
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IJSONSerializer
	{
		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The object.</param>
		/// <returns></returns>
		string Serialize(object target);

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The object.</param>
		/// <param name="converters">The converters.</param>
		/// <returns></returns>
		string Serialize(object target, params IJSONConverter[] converters);

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <returns></returns>
		object Deserialize(string jsonString, Type expectedType);

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <returns></returns>
		T Deserialize<T>(string jsonString);

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <returns></returns>
		T[] DeserializeArray<T>(string jsonString);
	}
}

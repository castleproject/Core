using System.Runtime.Serialization;
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

	/// <summary>
	/// Summary description for MySerializableClass.
	/// </summary>
	[Serializable]
	public class MySerializableClass
	{
		protected DateTime current;

		public MySerializableClass()
		{
			current = DateTime.Now;
		}

		public virtual DateTime Current
		{
			get { return current; }
		}

		public virtual double CalculateSumDistanceNow()
		{
			return Math.PI;
		}
	}

	[Serializable]
	public class MySerializableClass2 : MySerializableClass, ISerializable
	{
		public MySerializableClass2() 
		{
		}

		public MySerializableClass2(SerializationInfo info, StreamingContext context) 
		{
			current = (DateTime) info.GetValue("dt", typeof(DateTime));
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dt", current);
		}
	}
}

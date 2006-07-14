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
	
	internal class NHibernateNullablesSupport
	{
		private const String NullableAsm = "Nullables, Version=1.0.2.0, Culture=neutral";
		                     //, PublicKeyToken=154fdcb44c4484fc";

		private const String NullableIType = "Nullables.NHibernate.{0}Type, Nullables.NHibernate";
				
		private static Type tINullableType;
		
		static NHibernateNullablesSupport()
		{
			tINullableType = Type.GetType("Nullables.INullableType, " + NullableAsm, false);
		}
		
		public static bool IsNHibernateNullableType(Type t)
		{
			return tINullableType != null && tINullableType.IsAssignableFrom(t);
		}

		public static String GetITypeTypeNameForNHibernateNullable(Type type)
		{
			if (type == null)
				return null;
			
			bool supported = type.AssemblyQualifiedName.EndsWith(NullableAsm);

			if (!supported)
				throw new ActiveRecordException(String.Format("ActiveRecord does not support Nullable for {0} natively.", type));

			return String.Format(NullableIType, type.Name);
		}
	}
}

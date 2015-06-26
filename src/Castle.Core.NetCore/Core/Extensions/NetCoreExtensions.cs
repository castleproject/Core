// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

using System.Linq;
using System.Reflection.Emit;

namespace System.Reflection
{
	internal static class CoreReflectionExtensions
	{
		public static TypeBuilder GetTypeInfo(this TypeBuilder typeBuilder)
		{
			return typeBuilder;
		}

		// There's no current implementation on NET Core that supports the Binder
		// this class prevents anyone from calling the API with values other than null.
		public class Binder
		{
			private Binder() { }
		}

		public static ConstructorInfo GetConstructor(this Type type, BindingFlags flags, Binder binder, Type[] types, object[] ignored)
		{
			if (ignored != null)
			{
				throw new NotImplementedException("GetConstructor() with ParameterModifiers is unsupported on .NET Core");
			}
			return type.GetConstructors(flags).SingleOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
		}
	}
}

namespace System.ComponentModel
{
	public interface IDataErrorInfo
	{
		string Error
		{
			get;
		}
		string this[string columnName]
		{
			get;
		}
	}
}
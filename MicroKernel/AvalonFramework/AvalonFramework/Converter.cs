// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;

	internal class Converter
	{
		internal static object ChangeType(
			object value, 
			Type type, 
			object defaultValue)
		{
			object result;

			if (type == null)
			{
				result = value;
			}
			else
			{
				try
				{
					result = Convert.ChangeType(value, type); 
				}
				catch (Exception e)
				{
					if (defaultValue == null)
					{
						throw new InvalidCastException("The Convertion failed.", e);
					}
					else
					{
						result = defaultValue;
					}
				}
			}
			return result;
		}
	}
}

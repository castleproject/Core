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

#if SILVERLIGHT
using System;

namespace Castle.Core.Extensions
{
	using System.Collections.Generic;

	public static class SilverlightExtensions
	{
		public static T[] FindAll<T>(this T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<T> list = new List<T>();
			for (int i = 0; i < array.Length; i++)
			{
				if (match(array[i]))
				{
					list.Add(array[i]);
				}
			}
			return list.ToArray();
		}
	}
}

//namespace System.Collections.Specialized
//{
//	using Generic;
//
//	public class NameValueCollection : Dictionary<string, string>
//	{
//		public NameValueCollection() : base(StringComparer.InvariantCultureIgnoreCase)
//		{
//			
//		}
//		public new string this[string key]
//		{
//			get
//			{
//				string result;
//				TryGetValue(key, out result);
//				return result;
//			}
//			set
//			{
//				base[key] = value;
//			}
//		}
//	}
//}

//namespace System.Threading
//{
//	public class ReaderWriterLock
//	{
//		private object _syncObject = new object();
//		
//		public ReaderWriterLock()
//		{
//		}
//
//		public void AcquireWriterLock(int timeout)
//		{
//			Monitor.Enter(_syncObject);
//		}
//
//		public void AcquireReaderLock(int timeout)
//		{
//			Monitor.Enter(_syncObject);
//		}
//
//		public void ReleaseWriterLock()
//		{
//			Monitor.Exit(_syncObject);
//		}
//
//		public void ReleaseLock()
//		{
//			Monitor.Exit(_syncObject);
//		}
//	}
//}

//namespace System.Configuration
//{
//	public class ConfigurationErrorsException : Exception
//	{
//		public ConfigurationErrorsException(string message) : base(message)
//		{
//		}
//
//		public ConfigurationErrorsException(string message, Exception inner) : base(message, inner)
//		{
//		}
//	}
//}

//namespace System.ComponentModel
//{
//	public interface ISupportInitialize
//	{
//		void BeginInit();
//		void EndInit();
//	}
//}

#endif
// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	// OBSOLETE: This has been replaced with IVirtual<T>.

	public interface IRealizable<T> : IRealizableSource
	{
		bool IsReal { get; }
		T    Value  { get; }
	}

	public interface IRealizableSource
	{
		IRealizable<T> AsRealizable<T>();
	}

	public static class RealizableExtensions
	{
		public static IRealizable<T> RequireRealizable<T>(this IRealizableSource obj)
		{
			var realizable = obj.AsRealizable<T>();
			if (realizable == null)
				throw Error.NotRealizable<T>();
			return realizable;
		}
	}
}
#endif

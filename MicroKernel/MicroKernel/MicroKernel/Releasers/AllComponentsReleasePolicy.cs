// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Releasers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for AllComponentsReleasePolicy.
	/// </summary>
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		private IDictionary _instance2Handler = new HybridDictionary();

		public AllComponentsReleasePolicy()
		{
		}

		#region IReleasePolicy Members

		public virtual void Track(object instance, IHandler handler)
		{
			_instance2Handler[instance] = handler;
		}

		public bool HasTrack(object instance)
		{
			return _instance2Handler.Contains(instance);
		}

		public void Release(object instance)
		{
			IHandler handler = (IHandler) _instance2Handler[instance];
			
			if (handler != null)
			{
				_instance2Handler.Remove(instance);

				handler.Release(instance);
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			foreach(DictionaryEntry entry in _instance2Handler)
			{
				object instance = entry.Key;
				IHandler handler = (IHandler) entry.Value;
				handler.Release(instance);
			}

			_instance2Handler.Clear();
		}

		#endregion
	}
}

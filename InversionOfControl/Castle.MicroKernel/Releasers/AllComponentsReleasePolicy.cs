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

namespace Castle.MicroKernel.Releasers
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for AllComponentsReleasePolicy.
	/// </summary>
	[Serializable]
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		private IDictionary instance2Handler = Hashtable.Synchronized(
#if DOTNET2
			new Hashtable(new Util.ReferenceEqualityComparer()));
#else
			new Hashtable(CaseInsensitiveHashCodeProvider.Default, new Util.ReferenceComparer()));
#endif

		public AllComponentsReleasePolicy()
		{
		}

		#region IReleasePolicy Members

		public virtual void Track(object instance, IHandler handler)
		{
			instance2Handler[instance] = handler;
		}

		public bool HasTrack(object instance)
		{
			return instance2Handler.Contains(instance);
		}

		public void Release(object instance)
		{
			IHandler handler = (IHandler) instance2Handler[instance];
			
			if (handler != null)
			{
				instance2Handler.Remove(instance);

				handler.Release(instance);
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			foreach(DictionaryEntry entry in instance2Handler)
			{
				object instance = entry.Key;
				IHandler handler = (IHandler) entry.Value;
				handler.Release(instance);
			}

			instance2Handler.Clear();
		}

		#endregion
	}
}

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

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System.Collections.Generic;
	using System.ServiceModel;

	internal interface IWcfExtensionHelper
	{
		bool AddExtension(object extension);
	}

	internal class WcfExtensionHelper<T> : IWcfExtensionHelper
		where T : IExtensibleObject<T>
	{
		private readonly T owner;

		public WcfExtensionHelper(T owner)
		{
			this.owner = owner;
		}

		public bool AddExtension(object extension)
		{
			if (extension is IExtension<T>)
			{
				IExtension<T> extensionObject = (IExtension<T>)extension;
				owner.Extensions.Add(extensionObject);
				return true;
			}
			return false;
		}
	}

	internal class WcfExtensionHelper<K,T> : IWcfExtensionHelper
		where T : IExtensibleObject<T>
	{
		private readonly KeyedByTypeCollection<K> candidates;

		public WcfExtensionHelper(object candidates)
		{
			this.candidates = (KeyedByTypeCollection<K>)candidates;
		}

		public bool AddExtension(object extension)
		{
			var extensionObject = (IExtension<T>)extension;
			T owner = candidates.Find<T>();

			if (owner != null)
			{
				owner.Extensions.Add(extensionObject);
				return true;
			}

			return false;
		}
	}
}

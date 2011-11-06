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

namespace Castle.Components.DictionaryAdapter
{
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;

	public class DictionaryDescriptor : PropertyDescriptor
	{
		private List<IDictionaryInitializer> initializers;
		private List<IDictionaryMetaInitializer> metaInitializers;

		private static readonly ICollection<IDictionaryInitializer> NoInitializers = new IDictionaryInitializer[0];
		private static readonly ICollection<IDictionaryMetaInitializer> NoMetaInitializers = new IDictionaryMetaInitializer[0];

		public DictionaryDescriptor()
		{
		}

		public DictionaryDescriptor(object[] behaviors)
			: base(behaviors)
		{
		}

		public DictionaryDescriptor(PropertyInfo property, object[] behaviors)
			: base(property, behaviors)
		{
		}

		/// <summary>
		/// Gets the initializers.
		/// </summary>
		/// <value>The initializers.</value>
		public ICollection<IDictionaryInitializer> Initializers
		{
			get { return initializers ?? NoInitializers; }
		}

		/// <summary>
		/// Gets the meta-data initializers.
		/// </summary>
		/// <value>The meta-data initializers.</value>
		public ICollection<IDictionaryMetaInitializer> MetaInitializers
		{
			get { return metaInitializers ?? NoMetaInitializers; }
		}

		/// <summary>
		/// Adds the dictionary initializers.
		/// </summary>
		/// <param name="inits">The initializers.</param>
		public DictionaryDescriptor AddInitializer(params IDictionaryInitializer[] inits)
		{
			return AddInitializers((IEnumerable<IDictionaryInitializer>)inits);
		}

		/// <summary>
		/// Adds the dictionary initializers.
		/// </summary>
		/// <param name="inits">The initializers.</param>
		public DictionaryDescriptor AddInitializers(IEnumerable<IDictionaryInitializer> inits)
		{
			if (inits != null)
			{
				if (initializers == null)
				{
					initializers = new List<IDictionaryInitializer>(inits);
				}
				else
				{
					initializers.AddRange(inits);
				}
			}
			return this;
		}

		/// <summary>
		/// Copies the initializers to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public DictionaryDescriptor CopyInitializers(DictionaryDescriptor other)
		{
			if (initializers != null)
			{
				other.AddInitializers(initializers.Select(init => init.Copy()).OfType<IDictionaryInitializer>());
			}
			return this;
		}

		/// <summary>
		/// Adds the dictionary meta-data initializers.
		/// </summary>
		/// <param name="inits">The meta-data initializers.</param>
		public DictionaryDescriptor AddMetaInitializer(params IDictionaryMetaInitializer[] inits)
		{
			return AddMetaInitializers((IEnumerable<IDictionaryMetaInitializer>)inits);
		}

		/// <summary>
		/// Adds the dictionary meta-data initializers.
		/// </summary>
		/// <param name="inits">The meta-data initializers.</param>
		public DictionaryDescriptor AddMetaInitializers(IEnumerable<IDictionaryMetaInitializer> inits)
		{
			if (inits != null)
			{
				if (metaInitializers == null)
				{
					metaInitializers = new List<IDictionaryMetaInitializer>(inits);
				}
				else
				{
					metaInitializers.AddRange(inits);
				}
			}
			return this;
		}

		/// <summary>
		/// Copies the meta-initializers to the other <see cref="PropertyDescriptor"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public DictionaryDescriptor CopyMetaInitializers(DictionaryDescriptor other)
		{
			if (metaInitializers != null)
			{
				other.AddMetaInitializers(metaInitializers.Select(meta => meta.Copy()).OfType<IDictionaryMetaInitializer>());
			}
			return this;
		}

		public override PropertyDescriptor CopyBehaviors(PropertyDescriptor other)
		{
			if (other is DictionaryDescriptor)
			{
				var otherDict = (DictionaryDescriptor)other;
				CopyMetaInitializers(otherDict).CopyInitializers(otherDict);
			}
			return base.CopyBehaviors(other);
		}

		protected override void InternalAddBehavior(IDictionaryBehavior behavior)
		{
			if (behavior is IDictionaryInitializer)
			{
				AddInitializer((IDictionaryInitializer)behavior);
			}
			if (behavior is IDictionaryMetaInitializer)
			{
				AddMetaInitializer((IDictionaryMetaInitializer)behavior);
			}
			base.InternalAddBehavior(behavior);
		}
	}
}

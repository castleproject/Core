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

namespace Castle.Core
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Collection of <see cref="MethodMetaModel"/>
	/// </summary>
	[Serializable]
	public class MethodMetaModelCollection : ReadOnlyCollectionBase
	{
		private IDictionary methodInfo2Model;

		/// <summary>
		/// Adds the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		public void Add(MethodMetaModel model)
		{
			InnerList.Add(model);
		}

		/// <summary>
		/// Gets the method info2 model.
		/// </summary>
		/// <value>The method info2 model.</value>
		public IDictionary MethodInfo2Model
		{
			get
			{
				if (methodInfo2Model == null) methodInfo2Model = new HybridDictionary();
				
				return methodInfo2Model;
			}
		}
	}
}

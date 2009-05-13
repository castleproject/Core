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

namespace Castle.Core
{
	using System;
	using System.Collections;
    using System.Collections.Generic;
	using System.Collections.ObjectModel;

	/// <summary>
	/// Collection of <see cref="MethodMetaModel"/>
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class MethodMetaModelCollection : Collection<MethodMetaModel>
	{
		private IDictionary methodInfo2Model;

		/// <summary>
		/// Gets the method info2 model.
		/// </summary>
		/// <value>The method info2 model.</value>
		public IDictionary MethodInfo2Model
		{
			get
			{
				if (methodInfo2Model == null) 
					methodInfo2Model = new Dictionary<object, object>();

				return methodInfo2Model;
			}
		}
	}
}

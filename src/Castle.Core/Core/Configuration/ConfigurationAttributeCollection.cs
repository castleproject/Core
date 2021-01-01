﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Configuration
{
#if FEATURE_SERIALIZATION
	using System;
	using System.Runtime.Serialization;
#endif

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class ConfigurationAttributeCollection
		: System.Collections.Specialized.NameValueCollection
	{
		public ConfigurationAttributeCollection()
		{
		}

#if FEATURE_SERIALIZATION
		protected ConfigurationAttributeCollection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif
	}
}
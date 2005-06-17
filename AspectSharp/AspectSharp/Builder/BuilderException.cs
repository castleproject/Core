 // Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Builder
{
	using System;
	using System.Runtime.Serialization;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for BuilderException.
	/// </summary>
	[Serializable]
	public class BuilderException : ApplicationException
	{
		private LexicalInfo _info;

		public BuilderException(String message) : base(message)
		{
		}

		public BuilderException(LexicalInfo info, String message) : base(message)
		{
			_info = info;
		}

		public BuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public LexicalInfo LexicalInfo
		{
			get { return _info; }
		}
	}
}
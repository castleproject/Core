// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;

	/// <summary>
	/// The AvalonStageAttribute allows a component to declare 
	/// a extension dependency.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	public class AvalonStageAttribute : Attribute
	{
		private String m_extensionId;

		/// <summary>
		/// Constructor to initialize the stage's info.
		/// </summary>
		/// <param name="extensionID"></param>
		public AvalonStageAttribute(String extensionID)
		{
			if (extensionID == null || extensionID.Length == 0)
			{
				throw new ArgumentNullException("extensionID", "Stage's extension id can't be null");
			}

			m_extensionId = extensionID;
		}

		/// <summary>
		/// Returns the extension identifier.
		/// </summary>
		public String ExtensionID
		{
			get
			{
				return m_extensionId;
			}
		}
	}
}

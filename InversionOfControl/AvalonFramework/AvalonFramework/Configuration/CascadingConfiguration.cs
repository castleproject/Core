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
	/// Summary description for CascadingConfiguration.
	/// </summary>
	public class CascadingConfiguration : IConfiguration
	{
		/// <summary>
		/// TODO: 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		public CascadingConfiguration(IConfiguration parent, IConfiguration child)
		{
		}

		#region IConfiguration Members

		public string Name
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Name getter implementation
				return null;
			}
		}

		public string Location
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Location getter implementation
				return null;
			}
		}

		public string Value
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Value getter implementation
				return null;
			}
		}

		public string Namespace
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Namespace getter implementation
				return null;
			}
		}

		public string Prefix
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Prefix getter implementation
				return null;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				// TODO:  Add CascadingConfiguration.IsReadOnly getter implementation
				return false;
			}
		}

		public ConfigurationCollection Children
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Children getter implementation
				return null;
			}
		}

		public System.Collections.IDictionary Attributes
		{
			get
			{
				// TODO:  Add CascadingConfiguration.Attributes getter implementation
				return null;
			}
		}

		public IConfiguration GetChild(string child, bool createNew)
		{
			// TODO:  Add CascadingConfiguration.GetChild implementation
			return null;
		}

		public ConfigurationCollection GetChildren(string name)
		{
			// TODO:  Add CascadingConfiguration.GetChildren implementation
			return null;
		}

		public object GetValue(Type type, object defaultValue)
		{
			// TODO:  Add CascadingConfiguration.GetValue implementation
			return null;
		}

		public object GetAttribute(string name, Type type, object defaultValue)
		{
			// TODO:  Add CascadingConfiguration.GetAttribute implementation
			return null;
		}

		public object GetAttribute(string name, object defaultValue)
		{
			// TODO:  Add CascadingConfiguration.GetAttribute implementation
			return null;
		}

		#endregion
	}
}

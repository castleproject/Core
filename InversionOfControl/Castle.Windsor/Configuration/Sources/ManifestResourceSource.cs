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

namespace Castle.Windsor.Configuration.Sources
{
	using System;
	using System.IO;
	using System.Reflection;

	public class ManifestResourceSource : AbstractConfigurationSource
	{
		private StreamReader _reader;

		public ManifestResourceSource(Type type, String resourceName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type", "Null type");
			}

			ExtractResourceStream( type.Assembly, type, resourceName );
		}

		public ManifestResourceSource(Assembly assembly, String resourceName)
		{

			if (assembly == null)
			{
				throw new ArgumentNullException("assembly", "Null assembly");
			}

			ExtractResourceStream( assembly, null, resourceName );
		}

		#region IConfigurationSource Members

		public override TextReader Contents
		{
			get { return _reader; }
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					if (_reader != null) _reader.Close();
				}
				finally
				{
					base.Dispose(disposing);	
				}
			}
		}

		private void ExtractResourceStream(Assembly assembly, Type type, String resourceName)
		{
			if (resourceName == null || resourceName.Length == 0)
			{
				throw new ArgumentException("resourceName", "Null or empty resource name");
			}

			Stream resourceStream = null;

			if (type == null)
			{
				resourceStream = assembly.GetManifestResourceStream( resourceName );
			}
			else
			{
				resourceStream = assembly.GetManifestResourceStream( type, resourceName );
			}

			if (resourceStream == null)
			{
				if (type != null && type.Namespace != String.Empty)
					resourceName = String.Format( "{0}.{1}", type.Namespace, resourceName );
				throw new ArgumentException(String.Format( "Missing resource '{0}' in assembly {1}", resourceName, assembly.FullName ));
			}

			_reader = new StreamReader(resourceStream);
		}
	}
}

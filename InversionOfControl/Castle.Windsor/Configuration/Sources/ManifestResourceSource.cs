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


	public class ManifestResourceSource : IConfigurationSource
	{
		private StreamReader _reader;

		public ManifestResourceSource(Type type, String resourceName)
		{
			if (type == null) throw new ArgumentNullException("true", "Null type");
			if (resourceName == null || resourceName.Length == 0) throw new ArgumentException("resourceName", "Null or empty resource name");

			ExtractResourceStream( type.Assembly, type, resourceName );
		}

		public ManifestResourceSource(Assembly assembly, String resourceName)
		{
			if (assembly == null) throw new ArgumentNullException("assembly", "Null assembly");
			if (resourceName == null || resourceName.Length == 0) throw new ArgumentException("resourceName", "Null or empty resource name");

			ExtractResourceStream( assembly, null, resourceName );
		}

		~ManifestResourceSource()
		{
			Close();
		}

		#region IConfigurationSource Members

		public TextReader Contents
		{
			get { return _reader; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}

		#endregion

		private void ExtractResourceStream(Assembly assembly, Type type, String resourceName)
		{
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

		private void Close()
		{
			if (_reader != null) _reader.Close();
		}
	}
}

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
		private Assembly _assembly;
		private string _resourceName;
		private StreamReader _reader;

		public ManifestResourceSource(Type type, String resourceName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type", "Null type");
			}

			_assembly = type.Assembly;
			_resourceName = resourceName;

			AssertValidResource(type);
		}

		public ManifestResourceSource(Assembly assembly, String resourceName)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly", "Null assembly");
			}

			_assembly = assembly;
			_resourceName = resourceName;

			AssertValidResource(null);
		}

		#region IConfigurationSource Members

		public override TextReader Contents
		{
			get
			{
				if (_reader == null)
				{
					_reader = new StreamReader( ExtractResourceStream() );
				}

				return _reader;
			}
		}

		public override void Close()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}
		}

		#endregion

		/// <summary>
		/// Ensures the resource exists in the manifest.
		/// </summary>
		/// <param name="type">The type to scope the resource.</param>
		private void AssertValidResource(Type type)
		{
			if (_resourceName == null || _resourceName.Length == 0)
			{
				throw new ArgumentException("_resourceName", "Null or empty resource name");
			}
	
			if (type != null && type.Namespace != String.Empty)
				_resourceName = String.Format( "{0}.{1}", type.Namespace, _resourceName );

			string[] resourceNames = _assembly.GetManifestResourceNames();
			for (int i = 0; i < resourceNames.Length; ++i)
			{
				if (resourceNames[i] == _resourceName)
					return;
			}

			throw new ArgumentException(
				String.Format( "Missing resource '{0}' in _assembly {1}",
				_resourceName, _assembly.FullName ) );
		}
	
		/// <summary>
		/// Extracts the <see cref="Stream"/> representing the resource.
		/// </summary>
		/// <returns>The resource <see cref="Stream"/>.</returns>
		private Stream ExtractResourceStream()
		{
			return _assembly.GetManifestResourceStream( _resourceName );
		}
	}
}

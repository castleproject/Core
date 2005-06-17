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


	public class ExternalFileSource : AbstractConfigurationSource
	{
		private FileInfo _info;
		private StreamReader _reader;

		public ExternalFileSource(String fileName)
		{
			if (fileName == null || fileName.Length == 0) throw new ArgumentException("fileName", "Null or empty filename");
			
			_info = new FileInfo(fileName);
			
			if ( !_info.Exists && !Path.IsPathRooted(fileName))
			{
				String newFileName = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, fileName );

				_info = new FileInfo(newFileName);
			}

			if (!_info.Exists)
			{
				throw new ArgumentException("Could not find file '" + fileName + "'");
			}
		}

		#region IConfigurationSource Members

		public override TextReader Contents
		{
			get
			{
				if (_reader == null)
				{
					_reader = File.OpenText(_info.FullName);
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
	}
}

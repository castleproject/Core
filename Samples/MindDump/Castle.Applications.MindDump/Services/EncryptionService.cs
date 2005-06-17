using System;
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

namespace Castle.Applications.MindDump.Services
{
	using System.Text;
	using System.Security.Cryptography;


	public class EncryptionService
	{
		private readonly byte[] _key;
		private readonly byte[] _initVector;

		private DESCryptoServiceProvider _provider;

		public EncryptionService()
		{
			_provider = new DESCryptoServiceProvider();
			_key = Encoding.Default.GetBytes("MyKeyCaS");
			_initVector = Encoding.Default.GetBytes("itVector");
		}

		public string Encrypt(string contents)
		{
			ICryptoTransform transform = _provider.CreateEncryptor(_key, _initVector);

			byte[] bArray = Encoding.Default.GetBytes(contents);
			byte[] bOutput = transform.TransformFinalBlock( bArray, 0, bArray.Length);

			return  Convert.ToBase64String( bOutput );
		}

		public string Decrypt(string contents)
		{
			ICryptoTransform transform = _provider.CreateDecryptor(_key, _initVector);

//			byte[] bArray = Encoding.Default.GetBytes( Convert.FromBase64String( contents) );
			byte[] bArray = Convert.FromBase64String(contents);
			byte[] bOutput = transform.TransformFinalBlock( bArray, 0, bArray.Length );

			return Encoding.Default.GetString( bOutput );
		}
	}
}

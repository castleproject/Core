// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test.Components
{
	using System;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Summary description for AvalonMailService.
	/// </summary>
	[AvalonComponent("mailservice", Lifestyle.Singleton)]
	[AvalonService( typeof(IMailService) )]
	public class AvalonMailService : 
		IMailService, IInitializable, IConfigurable, IDisposable
	{
		public bool initialized;
		public bool configured;
		public bool disposed;

		public AvalonMailService()
		{
		}

		#region IMailService Members

		public void Send(String from, String to, String subject, String message)
		{
		}

		#endregion

		#region IInitializable Members

		public void Initialize()
		{
			initialized = true;
		}

		#endregion

		#region IConfigurable Members

		public void Configure(IConfiguration config)
		{
			configured = true;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			disposed = true;
		}

		#endregion
	}
}

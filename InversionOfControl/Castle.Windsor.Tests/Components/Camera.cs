// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests.Components
{
	using System;

	public interface ICamera
	{
		int Id { get; }
		string Name { get; set; }
		string IPNumber { get; set; }
	}

	public interface ICameraServiceBase
	{
		ICamera Add(String name, string ipNumber);
	}

	public interface ICameraService : ICameraServiceBase
	{
		void Record(ICamera cam);
	}

	public class Camera : ICamera
	{
		private int myId;
		private string myName;
		private string myIPNumber;

		public int Id
		{
			get { return myId; }
			set { myId = value; }
		}

		public string Name
		{
			get { return myName; }
			set { myName = value; }
		}

		public String IPNumber
		{
			get { return myIPNumber; }
			set { myIPNumber = value; }
		}
	}

	public class CameraService : MarshalByRefObject, ICameraService
	{
		public ICamera Add(String name, String ipNumber)
		{
			return new Camera();
		}

		public void Record(ICamera cam)
		{
			Console.WriteLine("Recording...");
		}
	}
}
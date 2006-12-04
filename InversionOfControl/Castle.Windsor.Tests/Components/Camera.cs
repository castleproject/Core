namespace Castle.Windsor.Tests.Components
{
	using System;

	public interface ICamera
	{
		int Id { get; }
		string Name { get; set;}
		string IPNumber { get; set;}
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

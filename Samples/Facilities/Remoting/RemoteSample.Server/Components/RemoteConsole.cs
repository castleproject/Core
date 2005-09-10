namespace RemoteSample.Server.Components
{
	using System;
	using RemoteSample.Components;

	public class RemoteConsole : MarshalByRefObject, IRemoteConsole
	{
		public RemoteConsole()
		{
		}

		#region IRemoteConsole Members

		public void WriteLine(String line)
		{
			Console.WriteLine(line);
		}

		#endregion
	}
}

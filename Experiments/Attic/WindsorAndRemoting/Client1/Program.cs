namespace Client1
{
	using System;
	using System.Windows.Forms;
	using System.Runtime.Remoting;

	static class Program
	{
		[STAThread]
		static void Main()
		{
			RemotingConfiguration.Configure("RemotingTcpConfigClient.config", false);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
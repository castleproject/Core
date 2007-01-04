namespace Client1
{
	using System;
	using System.Runtime.Remoting;
	using System.Windows.Forms;
	using Server;

	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Service1 serv1 = (Service1)
				RemotingServices.Connect(typeof(Service1), "tcp://localhost:2133/serv1");
			
			serv1.DoOperation();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			IService2 serv2 = (IService2)
				RemotingServices.Connect(typeof(IService2), "tcp://localhost:2133/serv2");

			serv2.DoOperation2();
		}
	}
}
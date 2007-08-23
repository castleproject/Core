namespace Castle.NewGenerator.VSNetIntegration
{
	using System.Runtime.InteropServices;
	using Castle.NewGenerator.Core;
	using EnvDTE;

	[Guid("9FF77D9F-E4FC-47EE-8E8B-0079FC2F247D")]
	[ProgId("Castle.TestWizard")]
	[ComDefaultInterface(typeof(IDTWizard))]
	[ComVisible(true)]
	public class VSGeneratorService : IDTWizard, IGeneratorService
	{
		public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams,
		                    ref wizardResult retval)
		{
//			VelocityEngine eng = new VelocityEngine();
//			eng.Init();
//
//			StringWriter writer = new StringWriter();
//
//			eng.Evaluate(new VelocityContext(), writer, "", "my test");

			// MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory);
			// MessageBox.Show(writer.GetStringBuilder().ToString());
		}
	}
}

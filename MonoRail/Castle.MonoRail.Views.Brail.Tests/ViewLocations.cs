namespace Castle.MonoRail.Views.Brail.Tests
{
	using System.IO;

	public class ViewLocations
	{
		public static string TestSiteBrail
		{
			get
			{
				if (Directory.Exists("../../../TestSiteBrail/Views"))
					return "../../../TestSiteBrail";
				return "../../../MonoRail/TestSiteBrail";
			}
		}

		public static string BrailTestsView
		{
			get
			{
				if (Directory.Exists("../../../Castle.MonoRail.Views.Brail.Tests/Views"))
					return "../../../Castle.MonoRail.Views.Brail.Tests";
				return "../../../MonoRail/Castle.MonoRail.Views.Brail.Tests";
			}
		}
	}
}
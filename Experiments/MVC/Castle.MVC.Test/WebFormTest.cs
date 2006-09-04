using NUnit.Extensions.Asp;
using NUnit.Extensions.Asp.AspTester;
using NUnit.Framework;

namespace Castle.MVC.Test
{
	/// <summary>
	/// (If you receive a 401 Access Denied error, check your security settings. 
	/// For example, try enabling anonymous access for the GuestBook virtual directory, 
	/// and make sure the IUSR_machinename user has read access to the directory.)
	/// </summary>
	[TestFixture] 	
	public class WebFormTest : WebFormTestCase 
	{

		[Test] 
		public void TestGoToPage2WithButtonLogin() 
		{ 
			// First, instantiate "Tester" objects: 
			ButtonTester buttonLogin = new ButtonTester("ButtonLogin", CurrentWebForm); 
			LabelTester labelTester = new LabelTester("LabelPreviousView", CurrentWebForm);

			// Second, visit the page being tested: 
			Browser.GetPage("http://localhost/Castle.MVC.Test.Web/Views/index.aspx"); 
			string loginPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();

			buttonLogin.Click(); 
			string currentPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();
			Assert(currentPage, loginPage != currentPage);
			AssertEquals("index", labelTester.Text); 
		} 

		[Test] 
		public void TestGoToPage2ViaLinkButton() 
		{ 
			// First, instantiate "Tester" objects: 
			ButtonTester buttonLogin = new ButtonTester("ButtonLogin", CurrentWebForm); 
			LinkButtonTester link = new LinkButtonTester("LinkToPage2", CurrentWebForm); 
			LabelTester labelTester = new LabelTester("LabelPreviousView", CurrentWebForm);

			// Second, visit the page being tested: 
			Browser.GetPage("http://localhost/Castle.MVC.Test.Web/Views/index.aspx"); 
			string loginPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();

			link.Click(); 
			string currentPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();
			Assert(currentPage, loginPage != currentPage);
			AssertEquals("index", labelTester.Text); 
		} 

		[Test] 
		public void TestGoToPage2ViaButtoninUserControl() 
		{ 
			// First, instantiate "Tester" objects: 
			UserControlTester myUserControl = new UserControlTester("MyUserControl", CurrentWebForm);
			ButtonTester buttonGoToPage2 = new ButtonTester("GoToPage2", myUserControl); 
			LabelTester labelTester = new LabelTester("LabelPreviousView", CurrentWebForm);

			// Second, visit the page being tested: 
			Browser.GetPage("http://localhost/Castle.MVC.Test.Web/Views/index.aspx"); 
			string loginPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();

			buttonGoToPage2.Click(); 
			string currentPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();
			Assert(currentPage, loginPage != currentPage);
			AssertEquals("index", labelTester.Text); 
		} 

		[Test] 
		public void TestGoToPageIndex() 
		{ 
			// First, instantiate "Tester" objects: 
			ButtonTester buttonLogin = new ButtonTester("ButtonLogin", CurrentWebForm); 
			ButtonTester button= new ButtonTester("Button", CurrentWebForm); 

			Browser.GetPage("http://localhost/Castle.MVC.Test.Web/Views/index.aspx"); 
			string loginPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();
			buttonLogin.Click(); 
			button.Click();
			string currentPage = this.Browser.CurrentUrl.AbsoluteUri.ToString();
			Assert(currentPage, loginPage.ToLower() == currentPage.ToLower());
		} 

	}
}

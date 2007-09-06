using NUnit.Framework;
using Castle.MonoRail.TestSupport;
using <%= ModelsNamespace %>;

namespace <%= TestsNamespace %>
{
	/// <summary>
	/// Classe de test du controleur de <%= PluralHumanName %>
	/// </summary>
	[TestFixture]
	public class <%= ControllerName %>ControllerTest : ControllerTestCase
	{		
		
		[Test]
		public void Index()
		{
			DoGet("<%= ControllerFileName %>/index.aspx");
			
			AssertRedirectedTo("/<%= ControllerFileName %>/list.aspx");
		}
		
		[Test]
		public void List()
		{
			DoGet("<%= ControllerFileName %>/list.aspx");
			
			AssertSuccess();
		}
	}
}

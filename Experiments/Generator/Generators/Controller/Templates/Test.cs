using NUnit.Framework;

namespace <%= TestsNamespace %> {
	/// <summary>
	/// <%= ClassName %>Controller test case
	/// </summary>
	[TestFixture]
	public class <%= ClassName %>ControllerTest : ControllerTestCase
	{
		<% for action in Actions: %>
		[Test]
		public void <%= action %>()
		{
			DoGet("<%= ClassName.ToLower() %>/<%= action.ToLower() %>.<%= Extension %>");
			AssertSuccess();
			
			// Assert....
		}
		
		<% end %>
	}
}

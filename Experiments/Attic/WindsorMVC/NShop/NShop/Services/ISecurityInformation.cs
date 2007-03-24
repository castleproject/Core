namespace NShop.Services
{
	public interface ISecurityInformation
	{
		bool HasAuthorizationFor(string action);
	}
}
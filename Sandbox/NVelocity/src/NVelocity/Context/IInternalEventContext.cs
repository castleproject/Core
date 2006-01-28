namespace NVelocity.Context
{
	using NVelocity.App.Events;

	/// <summary>
	/// Interface for event support.  Note that this is a public internal
	/// interface, as it is something that will be accessed from outside
	/// of the .context package.
	/// </summary>
	public interface IInternalEventContext
	{
		EventCartridge EventCartridge { get; }

		EventCartridge AttachEventCartridge(EventCartridge ec);
	}
}
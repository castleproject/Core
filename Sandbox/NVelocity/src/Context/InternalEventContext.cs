using System;
using NVelocity.App.Events;

namespace NVelocity.Context {

    /// <summary>
    /// Interface for event support.  Note that this is a public internal
    /// interface, as it is something that will be accessed from outside
    /// of the .context package.
    /// </summary>
    public interface InternalEventContext {

	EventCartridge EventCartridge {
	    get
		;
	    }

	    EventCartridge AttachEventCartridge(EventCartridge ec);

    }
}

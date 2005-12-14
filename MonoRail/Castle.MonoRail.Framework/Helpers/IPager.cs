using System;
using System.Collections;

namespace Castle.MonoRail.Framework.Helpers
{
	public interface IPager : IEnumerable
	{
		int CurrentIndex { get; }
		int LastIndex { get; }
		int NextIndex { get; }
		int PreviousIndex { get; }
		int FirstIndex { get; }

		bool HasPrevious { get; }
		bool HasNext { get; }
	}
}

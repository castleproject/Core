using System;
using System.Collections;

namespace NVelocity.Util {

    public interface Iterator {

	/// <summary> Move to next element in the array.
	/// </summary>
	/// <returns>The next object in the array.
	/// </returns>
	System.Object next();

	/// <summary> Check to see if there is another element in the array.
	/// </summary>
	/// <returns>Whether there is another element.
	/// </returns>
	bool hasNext();

    }
}

using System;

namespace NVelocity.Util.Introspection {

    /// <summary>  little class to hold 'distance' information
    /// for calling params, as well as determine
    /// specificity
    /// </summary>
    internal class Twonk {
	public int distance;
	public int[] vec;

	public Twonk(int size) {
	    vec = new int[size];
	}

	public virtual int moreSpecific(Twonk other) {
	    if (other.vec.Length != vec.Length)
		return - 1;

	    bool low = false;
	    bool high = false;

	    for (int i = 0; i < vec.Length; i++) {
		if (vec[i] > other.vec[i]) {
		    high = true;
		} else if (vec[i] < other.vec[i]) {
		    low = true;
		}
	    }

	    /*
	    *  this is a 'crossing' - meaning that
	    *  we saw the parameter 'slopes' cross
	    *  this means ambiguity
	    */

	    if (high && low)
		return 0;

	    /*
	    *  we saw that all args were 'high', meaning
	    *  that the other method is more specific so
	    *  we are less
	    */

	    if (high && !low)
		return - 1;

	    /*
	    *  we saw that all points were lower, therefore
	    *  we are more specific
	    */

	    if (!high && low)
		return 1;

	    /*
	    *  the remainder, neither high or low
	    *  means we are the same.  This really can't 
	    *  happen, as it implies the same args, right?
	    */

	    return 1;
	}
    }
}

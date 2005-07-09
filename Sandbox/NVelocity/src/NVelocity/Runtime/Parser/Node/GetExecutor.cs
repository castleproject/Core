namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// Executor that simply tries to execute a get(key)
	/// operation. This will try to find a get(key) method
	/// for any type of object, not just objects that
	/// implement the Map interface as was previously
	/// the case.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	public class GetExecutor : AbstractExecutor
	{
		/// <summary>
		/// Container to hold the 'key' part of get(key).
		/// </summary>
		private Object[] args = new Object[1];

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GetExecutor(RuntimeLogger r, Introspector i, Type c, String key)
		{
			rlog = r;
			args[0] = key;

			// NOTE: changed from get to get to get_Item - assumption is that get would be converted to an indexer in .Net
			// to keep some resembalance to the Java version, look for "Get" and "get" methods as well (both cases for .Net style and java)
			method = i.getMethod(c, "get_Item", args);
			if (method == null)
			{
				method = i.getMethod(c, "Get", args);
				if (method == null)
				{
					method = i.getMethod(c, "get", args);
				}
			}
		}

		/// <summary>
		/// Execute method against context.
		/// </summary>
		public override Object execute(Object o)
		{
			if (method == null)
				return null;

			return method.Invoke(o, args);
		}

//	public override System.Object OLDexecute(System.Object o, InternalContextAdapter context) {
//	    if (method == null)
//		return null;
//
//	    try {
//		return method.Invoke(o, (System.Object[]) args);
//	    } catch (System.Reflection.TargetInvocationException ite) {
//		/*
//		*  the method we invoked threw an exception.
//		*  package and pass it up
//		*/
//		throw new MethodInvocationException("Invocation of method 'get(\"" + args[0] + "\")'" + " in  " + o.GetType() + " threw exception " + ite.GetBaseException().GetType(), ite.GetBaseException(), "get");
//	    } catch (System.ArgumentException iae) {
//		return null;
//	    }
//	}

	}
}
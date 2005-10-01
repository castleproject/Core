using System;
using System.Collections;
using System.Reflection;

namespace Castle.Facilities.Cache
{
	/// <summary>
	/// Description résumée de CacheConfig.
	/// </summary>
	public class CacheConfig
	{
		private IList _methods = new ArrayList();
		private IList _methodName = new ArrayList();

		public void AddMethodName(string value)
		{
			_methodName.Add(value);
		}

		public void AddMethod(MethodInfo method)
		{
			_methods.Add(method);
		}
	
		/// <summary>
		/// A 
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public bool IsMethodCache(MethodInfo method)
		{
			if (_methods.Contains(method)) return true;

			foreach(String methodName in _methodName)
			{
				if (method.Name.Equals(methodName))
				{
					return true;
				}
			}

			return false;
		}
	}
}

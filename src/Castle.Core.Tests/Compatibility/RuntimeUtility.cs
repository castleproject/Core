namespace Castle.Core.Tests.Compatibility
{
	using System;

	public static class RuntimeUtility
	{
		private static bool s_IsMono;
		static RuntimeUtility()
		{
			Type t = Type.GetType("Mono.Runtime");
			s_IsMono = t != null;
		}

		/// <summary>
		/// Detects whether running with Mono.
		/// Recommended by http://www.mono-project.com/docs/faq/technical/
		/// </summary>
		/// <returns>true if running with Mono VM; false otherwise.</returns>
		public static bool IsMono
		{
			get
			{
				return s_IsMono;
			}
		}
	}
}

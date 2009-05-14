// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Castle Project" file="IServiceProviderEx.cs">
//   Copyright 2004-2009 Castle Project - http://www.castleproject.org
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Castle.Core
{
	using System;

	/// <summary>
	/// Increments <c>IServiceProvider</c> with a generic service resolution operation.
	/// </summary>
	public interface IServiceProviderEx : IServiceProvider
	{
		T GetService<T>() where T : class;
	}
}

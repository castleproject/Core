using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Resource;

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	/// <summary>
	/// Resource for a file or an assembly resource
	/// </summary>
	public class FileAssemblyResource:IResource
	{
		/// <summary>
		/// Depending on the resource type, <see cref="AssemblyResource"/> or <see cref="FileResource"/> is decorated.
		/// </summary>
		/// <param name="resource"></param>
		public FileAssemblyResource(string resource)
		{
			if(File.Exists(resource))
			{
				innerResource = new FileResource(resource);
			}
			else
			{
				innerResource = new AssemblyResource(resource);
			}

		}

		private readonly IResource innerResource;
		#region IResource Members

		/// <summary>
		/// Returns an instance of Castle.Core.Resource.IResource created according to the relativePath using itself as the root.
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public IResource CreateRelative(string relativePath)
		{
			return innerResource.CreateRelative(relativePath);
		}

		/// <summary>
		/// Only valid for resources that can be obtained through relative paths
		/// </summary>
		public string FileBasePath
		{
			get { return innerResource.FileBasePath; }
		}

		/// <summary>
		/// Returns a reader for the stream
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public TextReader GetStreamReader(Encoding encoding)
		{
			return innerResource.GetStreamReader(encoding);
		}

		/// <summary>
		/// Returns a reader for the stream
		/// </summary>
		/// <returns></returns>
		public TextReader GetStreamReader()
		{
			return innerResource.GetStreamReader();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes the allocated resources
		/// </summary>
		public void Dispose()
		{
			innerResource.Dispose();
		}

		#endregion
	}
}

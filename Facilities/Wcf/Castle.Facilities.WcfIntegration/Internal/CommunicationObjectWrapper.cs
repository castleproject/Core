
using System.ServiceModel;
using System;

namespace Castle.Facilities.WcfIntegration.Internal
{
	/// <summary>
	/// Simple wrapper for <see cref="ICommunicationObject"/>
	/// </summary>
	internal class CommunicationObjectWrapper : ICommunicationObject
	{
		private readonly ICommunicationObject inner;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inner"></param>
		public CommunicationObjectWrapper(ICommunicationObject inner)
		{
			this.inner = inner;
		}

		/// <summary>
		/// 
		/// </summary>
		public CommunicationState State
		{
			get { return inner.State; }
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Opened
		{
			add { inner.Opened += value; }
			remove { inner.Opened -= value; }
		}
 
		/// <summary>
		/// 
		/// </summary>
 		public event EventHandler Opening
		{
			add { inner.Opening += value; }
			remove { inner.Opening -= value; }			
		}
 
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Closed
		{
			add { inner.Closed += value; }
			remove { inner.Closed -= value; }			
		}

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Closing
		{
			add { inner.Closing += value; }
			remove { inner.Closing -= value; }			
		}

		/// <summary>
		/// 
		/// </summary>
        public event EventHandler Faulted
		{
			add { inner.Faulted += value; }
			remove { inner.Faulted -= value; }			
		}
            
		/// <summary>
		/// 
		/// </summary>
		public void Open()
		{
			inner.Open();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		public void Open(TimeSpan timeout)
		{
			inner.Open(timeout);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return inner.BeginOpen(timeout, callback, state);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginOpen(AsyncCallback callback, object state)
		{
			return inner.BeginOpen(callback, state);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public void EndOpen(IAsyncResult result)
		{
			inner.EndOpen(result);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Close()
		{
			inner.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		public void Close(TimeSpan timeout)
		{
			inner.Close(timeout);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return inner.BeginClose(timeout, callback, state);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginClose(AsyncCallback callback, object state)
		{
			return inner.BeginClose(callback, state);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public void EndClose(IAsyncResult result)
		{
			inner.EndClose(result);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Abort()
		{
			inner.Abort();
		}
	}
}

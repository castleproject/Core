 // Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MicroKernel.Handler
{
    using System;
    using System.Collections;

    using Castle.MicroKernel.Model;

    /// <summary>
    /// Summary description for AbstractHandler.
    /// </summary>
    public abstract class AbstractHandler : IHandler
    {
		private State m_state = State.Valid;
		
		private ArrayList m_instances = new ArrayList();

		private Delegate m_changeStateListener;

		private IKernel m_kernel;

        private IComponentModel m_componentModel;

        private IList m_dependencies = new ArrayList();

        protected Hashtable m_serv2handler = new Hashtable();

        protected ILifestyleManager m_lifestyleManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public AbstractHandler(IComponentModel model)
        {
            AssertUtil.ArgumentNotNull(model, "model");
            m_componentModel = model;
        }

		public IKernel Kernel
		{
			get { return m_kernel; }
		}

		public IList Dependencies
		{
			get { return m_dependencies; }
		}

        #region IHandler Members

        public virtual void Init(IKernel kernel)
        {
            m_kernel = kernel;
        }

        public virtual IComponentModel ComponentModel
        {
            get { return m_componentModel; }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="changeStateDelegate"></param>
		public void AddChangeStateListener( ChangeStateListenerDelegate changeStateDelegate )
		{
			if (m_changeStateListener == null)
			{
				m_changeStateListener = changeStateDelegate;
			}
			else
			{
				m_changeStateListener = Delegate.Combine(m_changeStateListener, changeStateDelegate);
			}
		}

        public virtual object Resolve()
        {
            if (ActualState == State.WaitingDependency)
            {
                throw new HandlerException(
                    "Can't Resolve component. " +
                        "It has dependencies which still to be satisfied.");
            }

            try
            {
                object instance = m_lifestyleManager.Resolve();

                RegisterInstance(instance);

                return instance;
            }
            catch(Exception ex)
            {
                throw new HandlerException("Exception while attempting to instantiate type", ex);
            }
        }

        public virtual void Release(object instance)
        {
            if (IsOwner(instance))
            {
                UnregisterInstance(instance);
                m_lifestyleManager.Release(instance);
            }
        }

        public virtual bool IsOwner(object instance)
        {
            return HasInstance(instance, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual State ActualState
        {
            get { return m_state; }
        }

        #endregion

		#region IDisposable Members

		public void Dispose()
		{
			object[] instances = m_instances.ToArray();

			foreach( object instance in instances )
			{
				Release( instance );
			}

			m_lifestyleManager.Dispose();
		}

		#endregion

		protected virtual void SetNewState( State state )
		{
			m_state = state;

			RaiseChangeStateEvent();
		}

		protected virtual void RaiseChangeStateEvent()
		{
			if ( m_changeStateListener != null )
			{
				ChangeStateListenerDelegate del = (ChangeStateListenerDelegate) m_changeStateListener;
				del( this );
			}
		}

        protected virtual void RegisterInstance(object instance)
        {
            if (!HasInstance(instance, false))
            {
                // WeakReference reference = new WeakReference( instance );
                // m_instances.Add( reference );
                m_instances.Add(instance);
            }
        }

        protected virtual void UnregisterInstance(object instance)
        {
            if (m_instances.Count == 0)
            {
                return;
            }

            HasInstance(instance, true);
        }

        protected virtual bool HasInstance(object instance, bool removeIfFound)
        {
            // foreach( WeakReference reference in m_instances )
            foreach(object storedInstance in m_instances)
            {
                // if (reference.Target == null)
                // {
                //	m_instances.Remove( reference );
                // }

                if (Object.ReferenceEquals(instance, storedInstance /*reference.Target*/))
                {
                    if (removeIfFound)
                    {
                        m_instances.Remove(instance);
                    }

                    return true;
                }
            }

            return false;
        }

	}
}
using System;
using Castle.Igloo.Util;
using Castle.MicroKernel;

namespace Castle.Igloo.Scopes
{
    /// <summary>
    /// Default implementation of the <see cref="IScopedObject"/> interface.
    /// </summary>
    public class DefaultScopedObject : IScopedObject
    {
        private IKernel _kernel = null;
        private string _targetComponentName = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScopedObject"/> class.
        /// </summary>
        public DefaultScopedObject(IKernel kernel, string targetComponentName)
        {
            AssertUtils.ArgumentNotNull(kernel, "BeanFactory must not be null");
            AssertUtils.ArgumentNotNull(targetComponentName, "Target component name must not be null.");

            _kernel = kernel;
            _targetComponentName = targetComponentName;

        }

        #region IScopedObject Members

        /// <summary>
        /// Return the current target object behind this scoped object proxy, in its raw form (as stored in the target scope).
        /// </summary>
        /// <value></value>
        public object TargetObject
        {
            get { return _kernel[_targetComponentName]; }
        }

        /// <summary>
        /// Remove this object from its target scope
        /// </summary>
        public void RemoveFromScope()
        {
            _kernel.ReleaseComponent(_kernel[_targetComponentName]);
        }

        #endregion
    }
}

using Castle.Core;

namespace Castle.MicroKernel.Proxy
{
    /// <summary>
    /// Select the appropriate interecptors based on the application specific
    /// business logic
    /// </summary>
    public interface IModelInterceptorsSelector
    {
        /// <summary>
        /// Select the appropriate intereceptor references.
        /// The intereceptor references aren't neccessarily registered in the model.Intereceptors
        /// </summary>
        /// <param name="model">The model to select the interceptors for</param>
        /// <returns>The intereceptors for this model (in the current context) or a null reference</returns>
        /// <remarks>
        /// If the selector is not interested in modifying the interceptors for this model, it 
        /// should return a null reference and the next selector in line would be executed (or the default
        /// model.Interceptors).
        /// If the selector return a non null value, this is the value that is used, and the model.Interectors are ignored, if this
        /// is not the desirable behavior, you need to merge your interceptors with the ones in model.Interecptors yourself.
        /// </remarks>
        InterceptorReference[] SelectInterceptors(ComponentModel model);

        /// <summary>
        /// Determain whatever the specified has interecptors.
        /// The selector should only return true from this method if it has determained that is
        /// a model that it would likely add interceptors to.
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>Whatever this selector is likely to add intereceptors to the specified model</returns>
        bool HasInterceptors(ComponentModel model);
    }
}
namespace Castle.Facilities.Logging
{
    using System;
    using System.Collections;
    using System.Reflection;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.ModelBuilder;
    using Castle.Model;
    using Castle.Services.Logging;

    /// <summary>
	/// Summary description for LoggingComponentInspector.
	/// </summary>
	public class LoggingComponentInspector : IContributeComponentModelConstruction
	{
        public LoggingComponentInspector(){}

        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            if (UsesLogging( model.Implementation ))
            {
                EnsureRelevantMethodsAreVirtual( model.Service, model.Implementation );

                model.Dependencies.Add( 
                    new DependencyModel( DependencyType.Service, null, typeof(LoggingInterceptor), false ) );

                model.Interceptors.AddFirst(  
                    new InterceptorReference( typeof(LoggingInterceptor) ) );
            }
        }

        private void EnsureRelevantMethodsAreVirtual(Type service, Type implementation)
        {
            if (service.IsInterface) return;

            MethodInfo[] methods = implementation.GetMethods( 
                BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly );

            ArrayList problematicMethods = new ArrayList();

            foreach( MethodInfo method in methods )
            {
                if (!method.IsVirtual && method.IsDefined( typeof(LogAttribute), true ))
                {
                    problematicMethods.Add( method.Name );
                }
            }
			
            if (problematicMethods.Count != 0)
            {
                String[] methodNames = (String[]) problematicMethods.ToArray( typeof(String) );

                String message = String.Format( "The class {0} wants to use logging interception, " + 
                    "however the methods must be marked as virtual in order to do so. Please correct " + 
                    "the following methods: {1}", implementation.FullName, String.Join(", ", methodNames) );

                throw new FacilityException(message);
            }
        }



        private bool UsesLogging(Type type)
        {
            bool result = false;
            MethodInfo[] methods = type.GetMethods( 
                BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly );

            foreach( MethodInfo method in methods )
            {
                if (method.IsDefined( typeof(LogAttribute), true ))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
	}
}

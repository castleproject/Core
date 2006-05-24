This is an example application showing some of the things that I think about currently.
It is not a fully functional framework, but it contains some interesting idea, in my opinion.

This sample requires Web Applications Projects.

Areas of interest:
	 - IController, IView interfaces - this is how the view & controllers are communicating between each other.
	 - DefaultController, Default.aspx are an example that include data binding from NHibernate
	 - FrontController intercepts all incoming requests, load the correct controller and after it finished processing, display the view.
	 - Specifying the view is done via a string the reference its path on the file system.
	 - Sepcifying the controller to handle each request is done on the Windsor configuration
	 
Dependencies:
	- This application depends on the following assemblies:
				Castle.DynamicProxy.dll
				Castle.MicroKernel.dll
				Castle.Model.dll
				Castle.Windsor.dll
				Iesi.Collections.dll
				log4net.dll
				NHibernate.dll
				NHibernate.Generics.dll
				Nullables.dll
				Nullables.NHibernate.dll
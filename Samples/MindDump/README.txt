This sample illustrates the integration between Castle Windsor and Castle on Rails. It depicts how the application overall development can benefit from them.



How to use
==========

The web application was developed using the Cassini web server, instead of IIS (I only have Windows XP Home). It was also developed as the root web application. If you have cassini installed, you can run the application by using:

> cd E:\dev\castle\Samples\PestControl\
> CassiniWebServer.exe "E:\dev\castle\Samples\PestControl\Castle.Applications.PestControl.Web" 8082 /

And access on the browser by using 

http://localhost:8082/default.htm


If you use IIS, create a virtual directory pointing to 

PestControl\Castle.Applications.PestControl.Web




IIS Considerations
==================

This application uses prevalence to keep its data, so the storage directory must be accessible and writable by the ASP.Net User (or system, depends on your configuration).

Also remember to associate the extension 'rails' with the ASP.Net ISAPI on IIS configuration.



Problems?
=========

Please direct any question to our mailing list:

http://www.castleproject.org/home/lists




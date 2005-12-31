This sample illustrates some more complex usages of ActiveRecord within MonoRail. 

Running the sample
==================

First of all edit the web.config to use your database:

        <add key="hibernate.connection.driver_class" value="NHibernate.Driver.SqlClientDriver" />
        <add key="hibernate.dialect"                 value="NHibernate.Dialect.MsSql2000Dialect" />
        <add key="hibernate.connection.provider"     value="NHibernate.Connection.DriverConnectionProvider" />
        <add key="hibernate.connection.connection_string" value="Data Source=.;Initial Catalog=test;Integrated Security=SSPI" />

Or if you have MS SQL Server create a 'test' Database.

Then create the required schema. Check the file 

  BinderSample.Web\schema.sql


Running with Cassini
====================

With cassini it's easy to run and debug the application. Execute:

> CassiniWebServer.exe <full path to>\BinderSample.Web 83 /

Then point your browser to 

  http://localhost:83/


Using the sample
================

Create some publishers and books using Scaffolding to start:

  http://localhost:83/scaffolding/index.rails

Then you can visualize different approaches to edit a publisher and it's books

    * http://localhost/approach1/index.rails
      which uses pure arrays 

and

    * http://localhost/approach2/index.rails
      which uses DataBind and arrays (less effort) 

and

    * http://localhost/approach3/index.rails
      which uses ARDataBind (much less effort) 

Enjoy.

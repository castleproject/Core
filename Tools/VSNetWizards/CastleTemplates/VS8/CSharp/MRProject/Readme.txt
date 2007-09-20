
The MonoRail project is set up and ready to run 
using VS.Net web project support

Some considerations if you are using 
Windsor integration:

Windsor Container Integration
-----------------------------

If you are using the Windsor container integration 
you must remember to register all new controllers on 
the Config/controllers.config file

ViewComponents must be registered as well. Filters 
can be optionally registered on the container.

Connection strings
------------------

See the Config/properties.config

ActiveRecord Integration Facility
---------------------------------

If you have chosen to use this facility, remember
to add a reference to the assembly that contains
the ActiveRecord types (your domain model).

If something needs to be included, check the
Config/facilities.config

NHibernate Integration Facility
-------------------------------

If you have chosen to use this facility, remember
to bring the hbm files or referencing the 
assemblies that contains the NHibernate xml mappings.

If something needs to be included, check the
Config/facilities.config


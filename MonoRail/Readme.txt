
-- Running the test cases

Cassini should be installed on your machine in order to
run the tests. Just use nant:

  > nant -t:net-1.1

to compile and test everything


-- Scaffolding

The scaffolding support tests requires a database. The schema is created automatically. 

You need to change the connection string information on

   MonoRail\TestScaffolding\web.config

  and   

   MonoRail\Castle.MonoRail.ActiveRecordScaffold.Tests\Castle.MonoRail.ActiveRecordScaffold.Tests.dll.config



-- More

More information about MonoRail
can be found at http://www.castleproject.org/


# Castle Scheduler

## Introduction

The Castle Scheduler component offers a lightweight and reusable general-purpose scheduling service that integrates well with most .Net applications. It is similar in purpose to the Java Quartz job scheduling framework but its implementation aims to leverage .Net idioms whenever possible.

The Scheduler service is intentionally kept relatively simple so that multiple implementations may be offered and so that the operation of the service can be easily mocked for testing. The default implementation should suffice for most purposes.

## Features

1. Provides a simple job scheduling service with the ability to create, monitor, update and delete jobs that are to be executed whenever the associated `Trigger` fires.
1. Supports job persistence via database integration or simply managed in-memory by selecting an appropriate Job Store.
1. Supports clustering of job schedulers via shared state in a database. Multiple clusters can co-exist within a common database. Scheduler instances belonging to the same cluster divide the task of running jobs. Race conditions among schedulers are automatically detected and resolved to ensure that at most one instance of any given job is executed at once. In addition, the system automatically detects when running jobs have been orphaned by their host scheduler and takes appropriate action to reschedule them.
1. Detects when a `Trigger` misfires (misses its deadline) and permits various responses.
1. Jobs can be stateful. Parameters and initial state for a job are bundled up into `JobData` objects that are persisted across job executions.
1. The strategy for asynchronously running jobs can be replaced. The default strategy creates instances of `IJob` objects using an `IJobFactory` and asynchronously runs the job's `Execute()` method using the ThreadPool.

## Requirements

The core scheduler depends on `Castle.Core.dll` to provide basic services such as logging. The scheduler project includes optional extensions for use with the Castle Windsor container. Someday it may include other optional extensions that integrate with other projects such as ActiveRecord and Quartz.Net.

## Usage

Usage is fairly straightforward. You get an `IScheduler` service, create some jobs, and eventually they will run. All of the components that make up the scheduler can be managed by an Inversion of Control container such as Castle Windsor, or they may be used stand-alone.

### Configuring a Job Store

#### In-Memory Job Store

The `MemoryJobStore` stores all of its state in memory so there is no configuration needed. Of course, when the scheduling process is shut down, all state will be wiped away.

#### SQL Server Job Store

The `SqlServerJobStore` requires a SQL Server 2000 or newer instance to be configured. Express Edition will work just fine.

This implementation uses stored procedures to access the database in order to comply with local site policies at some workplaces. If this is not to your liking, feel free to contribute your own job store implementation. An alternative database-agnostic implementation based on ActiveRecord is already planned.

Steps for creating the database:

1. Create a new SQL Server Role called `SchedulerRole` and associated it with an appropriate SQL Server Login for your application. The Role will be granted EXECUTE permission to the stored procedures when they are deployed but it otherwise it does not require any special permissions besides being able to connect to the database.
1. Run the schema creation scripts in `src\Castle.Components.Scheduler.Db\SqlServer\Create Scripts`. Caution: Re-running the scripts will drop all of the schedule tables and recreate them.
1. Run each of the stored procedure creation scripts in `src\Castle.Components.Scheduler.Db\SqlServer\Stored Procedures`. Remark: Re-running the scripts will drop and recreate all of the stored procedures.

### Simple Example

Get a scheduler instance, create a job and start the scheduler. This is a bit easier if you are using an Inversion of Control container such as Castle Windsor.

```csharp
IJobStore jobStore = new SqlServerJobStore("[Contrib:My connection string]");

IJobFactory jobFactory = new MyJobFactory();
IJobRunner jobRunner = new DefaultJobRunner(jobFactory);

DefaultScheduler scheduler = new DefaultScheduler(jobStore, jobRunner);
scheduler.Logger = new ConsoleLogger();
scheduler.Initialize();

// Create some initial state information for the job.  (optional)
JobData jobData = new JobData();
jobData.State[Contrib:"Token"] = 1;

// Create a trigger to fire at 2am local time each day.
Trigger trigger = PeriodicTrigger.CreateDailyTrigger(DateTime.Now.Date.ToUniversalTime().AddMinutes(120));

// Create a job specification for my job.
JobSpec jobSpec = new JobSpec("My job.", "A nightly maintenance job.", "MyJob", trigger, jobData);

// Create a job.  If it already exists in the persistent store then automatically update
// its definition to reflect the provided job specification.  This is a good idea when using
// a scheduler cluster because the job is guaranteed to be created exactly once and kept up
// to date without it ever being accidentally deleted by one instance while another instance
// is processing it.
scheduler.CreateJob(jobSpec, CreateJobConflictAction.Update);

// Start the scheduler.
scheduler.Start();
```

The `DefaultJobRunner` also needs a job factory to provide `IJob` instances to execute. The Windsor integration includes a simple job factory that just resolves `IJob` components using the job key as the component id. If you're not using Windsor, you'll need to provide a factory of your own like this:

```csharp
public class MyJobFactory : IJobFactory
{
    public IJob CreateJob(string jobKey)
    {
        if (jobKey == "MyJob")
            return new MyJob();

        throw new InvalidOperationException("Bad job key!");
    }
}
```

Finally we need to define an `IJob`. This example shows a stateful job that writes a token value to the log and increments it each time it runs. Note that if for some reason you don't want your jobs to implement the `IJob` interface or if you want to provide some other fancy mechanism for running them, you can always reimplement `IJobRunner` instead of using the `DefaultJobRunner`.

```csharp
public class MyJob : IJob
{
    public bool Execute(JobExecutionContext context)
    {
        // Use our current state.
        int currentToken = (int) context.JobData.State[Contrib:"Token"];
        context.Logger.InfoFormat("Current token is: '{0}'.", currentToken);

        // Update our state for next time.
        context.JobData.State[Contrib:"Token"] = currentToken + 1;

        // Return true for success!
        return true;
    }
}
```

### Integration with Castle Windsor

The configuration looks something like this:

```xml
<component id="Core.Scheduling.Scheduler"
           service="Castle.Components.Scheduler.IScheduler, Castle.Components.Scheduler"
           type="Castle.Components.Scheduler.DefaultScheduler, Castle.Components.Scheduler" />

<component id="Core.Scheduling.JobStore"
           service="Castle.Components.Scheduler.JobStores.IJobStore, Castle.Components.Scheduler"
           type="Castle.Components.Scheduler.JobStores.SqlServerJobStore, Castle.Components.Scheduler">
  <parameters>
    <connectionString>#{MyConnectionString}</connectionString>
    <clusterName>#{MyClusterName}</clusterName>
  </parameters>
</component>

<component id="Core.Scheduling.JobRunner"
           service="Castle.Components.Scheduler.IJobRunner, Castle.Components.Scheduler"
           type="Castle.Components.Scheduler.DefaultJobRunner, Castle.Components.Scheduler" />

<component id="Core.Scheduling.JobFactory"
           service="Castle.Components.Scheduler.IJobFactory, Castle.Components.Scheduler"
           type="Castle.Components.Scheduler.WindsorExtension.WindsorJobFactory, Castle.Components.Scheduler.WindsorExtension" />
```

The `JobFactory` then automatically resolves jobs by asking Windsor for a registered component whose id is the same as the job key. So for a job with key "MyJob" you should register a component like this:

```xml
<component id="MyJob"
           service="Castle.Components.Scheduler.IJob, Castle.Components.Scheduler"
           type="MyJob, MyAssembly" />
```
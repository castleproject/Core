namespace Castle.DynamicProxy.Tests.BugsReported
{
  using System.Reflection;
  using Castle.DynamicProxy.Tests.Interceptors;
  using NUnit.Framework;

  #region InterClasses

  public interface IGenericInterface<T>
  {
    void GenericMethod(T toto);
  }

  public class Class1 : IGenericInterface<IInterface1>
  {
    public void GenericMethod(IInterface1 toto)
    {
    }
  }

  public class Class2 : IGenericInterface<IInterface2>
  {
    public void GenericMethod(IInterface2 toto)
    {
    }
  }

  public interface IInterface1
  {
  }

  public interface IInterface2
  {
  }

  #endregion

  [TestFixture]
  public class DynProxy179
  {
    [Test]
    public void LoadAssemblyIntoCache_InvalidCacheAfterTwoLoadAssemblyIntoCacheThatContainsGeneric()
    {
      //
      // Step 1 - Save an assembly with 1 generic proxy
      //
      var proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME, "ProxyCache1.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME, "ProxyCache1.dll");
      var proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
      var generator = new ProxyGenerator(proxyBuilder);
      generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface1>), new Class1(), new DoNothingInterceptor());
      proxyGeneratorModuleScope.SaveAssembly();

      //
      // Step 2 - Save another assembly with 1 generic proxy
      // note : to reproduce the problem, must load previously saved assembly (in cache) before saving this assembly.
      //
      proxyGeneratorModuleScope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME + "1", "ProxyCache2.dll", ModuleScope.DEFAULT_ASSEMBLY_NAME + "1", "ProxyCache2.dll");
      proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
      generator = new ProxyGenerator(proxyBuilder);

      Assembly proxyAssembly = Assembly.LoadFrom("ProxyCache1.dll");
      proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

      generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface2>), new Class2(), new DoNothingInterceptor());
      proxyGeneratorModuleScope.SaveAssembly();

      //
      // Step 3 - Load the last proxy assembly and try to create the first generic proxy (created in step 1)
      // note : Creating proxy from step 2 works.
      // exception : Missing method exception, it returns the wrong proxy and the constructor used doesnt match the arguments passed.
      //
      proxyGeneratorModuleScope = new ModuleScope(true);
      proxyBuilder = new DefaultProxyBuilder(proxyGeneratorModuleScope);
      generator = new ProxyGenerator(proxyBuilder);

      proxyAssembly = Assembly.LoadFrom("ProxyCache2.dll");
      proxyGeneratorModuleScope.LoadAssemblyIntoCache(proxyAssembly);

      generator.CreateInterfaceProxyWithTargetInterface(typeof(IGenericInterface<IInterface1>), new Class1(), new DoNothingInterceptor());
    }

  }
}
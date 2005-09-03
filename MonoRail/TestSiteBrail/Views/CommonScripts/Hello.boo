def SayHello(name as string):
	return "Hello, ${name}"

# if this compiles, than we can access referenecd assemblies from the common scripts
def CanAccessTypedFromReferencedAssembly():
	assert typeof(Castle.MonoRail.Engine.RoutingModule) is not null
	
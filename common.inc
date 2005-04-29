<?xml version="1.0"?>

<project name="Common Build Properties">

	<!-- find out where nunit.framework.dll is -->

	<property name="lib.dir"
		value="${path::combine(nant::get-base-directory(), 'lib')}"
		dynamic="true" />
		
	<property name="lib.family.dir"
		value="${path::combine(lib.dir,framework::get-family(framework::get-target-framework()))}"
		dynamic="true" />
	
	<!-- for nant 0.85 rc2 or higher -->
	
	<property name="lib.framework.dir"
		value="${path::combine(lib.family.dir, version::to-string(framework::get-version(framework::get-target-framework())))}"
		dynamic="true" />

    <!-- Uncomment this block for nant backward compat 
	<property name="lib.framework.dir"
		value="${path::combine(lib.family.dir, framework::get-version(framework::get-target-framework()))}"
		dynamic="true" />
    -->
		
	<property name="nunit.framework.dll"
			value="${path::combine(lib.framework.dir, 'nunit.framework.dll')}" />

	<property name="nant.tasks.nunit2"
			value="False"
			unless="${property::exists('nant.tasks.nunit2')}" />

	<echo message="NUnit on ${lib.framework.dir} ${nunit.framework.dll} " />

</project>

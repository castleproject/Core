<?xml version="1.0"?>
<project name="Common Build Properties">

	<!-- Common properties that allow overrides -->

	<property name="strong"		value="STRONG"		unless="${property::exists('strong')}" />
	<property name="debug"		value="false"		unless="${property::exists('debug')}" />
	<property name="optimize"	value="true"		unless="${property::exists('optimize')}" />
	<property name="doc.dir"	value="${base.dir}/docs" />
	<property name="nunit.formatter" value="Plain"	unless="${property::exists('nunit.formatter')}" />
	<property name="os" value="${operating-system::to-string(environment::get-operating-system())}" />
	<property name="isWindows" value="${string::starts-with(os, 'Microsoft Windows')}" />

	<!-- find out where gacutil is -->

	<property name="gacutil.framework.file" value="${framework::get-framework-directory( framework::get-target-framework() )}\gacutil.exe"/>
	<property name="gacutil.sdk.file" value="${framework::get-sdk-directory( framework::get-target-framework() )}\gacutil.exe"/>

	<description>Find "gacutil" and show error message if it was not found.</description>
	<if test="${( not property::exists( 'gacutil.file' )) and file::exists( gacutil.framework.file )}">
		<property name="gacutil.file" value="${gacutil.framework.file}"/>
	</if>
	<if test="${( not property::exists( 'gacutil.file' )) and file::exists( gacutil.sdk.file )}">
		<property name="gacutil.file" value="${gacutil.sdk.file}"/>
	</if>
	<if test="${not property::exists( 'gacutil.file' )}">
		<echo>Could not find "gacutil" to install Castle.MonoRail.TestSupport server to GAC.</echo>
		<echo>To run the tests you have to install it manually: gacutil /i Castle.MonoRail.TestSupport</echo>
	</if>

	<!-- set up caspol.exe location -->

	<property name="caspol.file" value="${framework::get-framework-directory( framework::get-target-framework() )}\caspol.exe"/>

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

    <!-- Uncomment this block for nant backward compatibility
	<property name="lib.framework.dir"
		value="${path::combine(lib.family.dir, framework::get-version(framework::get-target-framework()))}"
		dynamic="true" />
    -->
		
	<property name="nunit.framework.dll"
			value="${path::combine(lib.framework.dir, 'nunit.framework.dll')}" />

	<property name="nant.tasks.nunit2"
			value="False"
			unless="${property::exists('nant.tasks.nunit2')}" />

</project>

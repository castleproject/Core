

\A

#set($woo = "bar")

$woo => bar

The following should print 'as is' : 
$f\oo
\a
"\r"

Now, test the built in directives. Note that $foo isn't in the context :
#set($foo = $foo + 1)
#set(\$foo = $foo + 1)
#if($foo)
#if ( $foo )
#else
#end
#elseif(

Now, a reference not in the context:
\$foo -> $foo
#if($foo)
#if(\$foo)

Put it in :
$foo -> 1
#if(1)
#if($foo)

This isn't in the context, so we get the full monty :
	\$woobie.blagh()

The following two are references :
	$provider.Title = lunatic
	$provider.getTitleMethod() = lunatic

Now, pluggable directives:

\#notadirective
#foreach


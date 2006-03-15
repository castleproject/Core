
--- Schmoo ---

These are not in the context, so they should render as they are here (schmoo).
$foo
\$foo
\\$foo

\#woogie
\\#woogie
\\\#woogie

Now put $foo in the context :
$foo = bar
\$foo =\bar
\\$foo =\\bar

As we increase the number of \'s, we alternate renderings :
bar
$foo
\bar
\$foo
\\bar

--- Pluggable Directives ----

We are doing an #include("test.txt"), starting with 0 '\' preceeding :

--text--
#include("test.txt")
\--text--
\#include("test.txt")
\\--text--

Now, foreach is a PD.  Escape the first one, and then not the second so it
renders.  The third and fourth examples show the single 'unpleasantry' about this.  The \
is only an escape when 'touching' VTL, otherwise, it's just schmoo.

#foreach(

\ first element \ second element \
\ first element \ \ second element \ \
\first element\ \second element\ \

--- Control Structures ----

First should be escaped...
#if(true) hi #end

This isn't.  Note then that it has to render the \\ as a \ because it's stuck to the VTL

\ hi \
\ hi 
And so forth...
\#if(true) hi \#end

\\ hi \\
And more...

#if(true)
	hi
#else
	there
#end

\	hi
\
\#if(true)
	hi
\#else
	there
\#end

\	there
\
\#if(false)
	hi
\#elseif(true)
	there
\#end


#$foo1
\#$foo1
#${foo1}
\#$${foo1}
#C0C0C0
\#C0C0C0
#C0C0C0
\#$C0C0C0
#\$C0C0C0


$(QUERY_STRING{forumid})
\$(QUERY_STRING{forumid})
\\$(QUERY_STRING{forumid})






Now, use the #quietnull example from the global library VM_global_library.vm : 
Now, there should be nothing in the brackets : ><

Where there should be something here : >hello!<


<table>
   <tr><tdi bgcolor=blue>$10.24</td></tr>
   <tr><tdi bgcolor=blue>$15.32</td></tr>
   <tr><tdi bgcolor=blue>$12.15</td></tr>
</table>


Further tests.  The following VMs and non-VM should be properly escaped :
#tablerow
#quietnull
\#notavm
>\<

Now try to define a new quietnull VM :

It should have been rejected, as the default is to not let inlines override existing, and there
should be a message in velocity.log.
Therefore it should still function normally :
>hello!<
><

We should be able to use argless VMs (and directives....)

Hello! I have no args!


And there was a bug where I wasn't getting the params right for the use-instance :


Arg :>$jdom.getRootElement().getChild("properties").getChild("author").getTextTrim()<

String literals should work as you expect :
Arg :>stringliteral<


Test boolean args :

   arg true
      arg false
 - Another fine Velocity Production -



This is a test of the new pedantic mode.

There are a few things you can do in pedantic mode.

Like get the spacing between things first elementsecond element to be really, really tight.

Further, it now binds any \n to the control structures, taking them out of the output.
The hope that this is What You Expect. 
So...

--
pedantic
--

should come out looking like

--
pedantic
--

But pay attention to what follows the #end statement :

1) First, follow with 'stuff' (not sure why you want to do this... but anway...)

--
pedantic
 woogie!
--

should be

--
pedantic
 woogie!
--

2) Whitespace will be eaten if there is a following newline 

--
pedantic
--

should be

--
pedantic
--


-- INLINE STUFF ---

1) respect spaces in the block
>first elementsecond element<
> first element second element<
>first element second element <
> first element  second element <

2) set statement has no output, incuding preceeding whitespace
 first element is first element
 second element is second element

	public void foo( String lala )
	{
  		System.out.println("first element"); 
  		System.out.println("second element"); 
	}

	public void foo( String lala )
	{
	  	System.out.println("first element"); 
	  	System.out.println("second element"); 
		}

Inline set statement :

Here are the prices :  $10.24  $15.32  $12.15 


#!/usr/bin/ruby
#
# Test case for BlueCloth Markdown transforms.
# $Id: 05_Markdown.tests.rb,v 1.1 2005/01/07 23:01:51 alexeyv Exp $
#
# Copyright (c) 2004 The FaerieMUD Consortium.
# 

if !defined?( BlueCloth ) || !defined?( BlueCloth::TestCase )
	basedir = File::dirname( __FILE__ )
	require File::join( basedir, 'bctestcase' )
end


### This test case tests ...
class SubfunctionsTestCase < BlueCloth::TestCase

	### Test email address output
	Emails = %w[
		address@example.com
		foo-list-admin@bar.com
		fu@bar.COM
		baz@ruby-lang.org
		foo-tim-bazzle@bar-hop.co.uk
		littlestar@twinkle.twinkle.band.CO.ZA
		ll@lll.lllll.ll
		Ull@Ulll.Ulllll.ll
		UUUU1@UU1.UU1UUU.UU
		l@ll.ll
		Ull.Ullll@llll.ll
		Ulll-Ull.Ulllll@ll.ll
		1@111.ll
	]
	# I can't see a way to handle IDNs clearly yet, so these will have to wait.
	#	info@öko.de
	#	jemand@büro.de
	#	irgendwo-interreßant@dÅgta.se
	#]

	def test_10_email_address
		printTestHeader "BlueCloth: Inline email address"
		rval = match = nil

		Emails.each {|addr|
			assert_nothing_raised {
				rval = BlueCloth::new( "<#{addr}>" ).to_html
			}

			match = %r{<p><a href="([^\"]+)">[^<]+</a></p>}.match( rval )
			assert_not_nil match, "Match against output #{rval}"
			assert_equal "mailto:#{addr}", decode( match[1] )
		}
	end


	def decode( str )
		str.gsub( /&#(x[a-f0-9]+|\d{3});/i ) {|match|
			code = $1
			debugMsg "Decoding %p" % code

			case code
			when /^x([a-f0-9]+)/i
				debugMsg "  (hex) = %p" % $1.to_i(16).chr
				$1.to_i(16).chr
			when /\d{3}/
				debugMsg "  (oct) = %p" % code.to_i.chr
				code.to_i.chr
			else
				raise "Hmmm... malformed entity %p" % code
			end
		} 
	end



	#################################################################
	###	A U T O - G E N E R A T E D   T E S T S
	#################################################################

	# Parse the data section into a hash of test specifications
	TestSets = {}
	begin
		seenEnd = false
		inMetaSection = true
		inInputSection = true
		section, description, input, output = '', '', '', ''
		linenum = 0

		# Read this file, skipping lines until the __END__ token. Then start
		# reading the tests.
		File::foreach( __FILE__ ) {|line|
			linenum += 1
			if /^__END__/ =~ line then seenEnd = true; next end
			debugMsg "#{linenum}: #{line.chomp}"
			next unless seenEnd

			# Start off in the meta section, which has sections and
			# descriptions.
			if inMetaSection
				
				case line

				# Left angles switch into data section for the current section
				# and description.
				when /^<<</
					inMetaSection = false
					next

				# Section headings look like:
				# ### [Code blocks]
				when /^### \[([^\]]+)\]/
					section = $1.chomp
					TestSets[ section ] ||= {}

				# Descriptions look like:
				# # Para plus code block
				when /^# (.*)/
					description = $1.chomp
					TestSets[ section ][ description ] ||= {
						:line => linenum,
						:sets => [],
					}

				end

			# Data section has input and expected output parts
			else

				case line

				# Right angles terminate a data section, at which point we
				# should have enough data to add a test.
				when /^>>>/
					TestSets[ section ][ description ][:sets] << [ input.chomp, output.chomp ]

					inMetaSection = true
					inInputSection = true
					input = ''; output = ''

				# 3-Dashed divider with text divides input from output
				when /^--- (.+)/
					inInputSection = false

				# Anything else adds to either input or output
				else
					if inInputSection
						input += line
					else
						output += line
					end
				end
			end
		}			
	end

	debugMsg "Test sets: %p" % TestSets

	# Auto-generate tests out of the test specifications
	TestSets.each {|sname, section|

		# Generate a test method for each section
		section.each do |desc, test|
			methname = "test_%03d_%s" %
				[ test[:line], desc.gsub(/\W+/, '_').downcase ]

			# Header
			code = %{
				def #{methname}
					printTestHeader "BlueCloth: #{desc}"
					rval = nil
			}

			# An assertion for each input/output pair
			test[:sets].each {|input, output|
				code << %{
					assert_nothing_raised {
						obj = BlueCloth::new(%p)
						rval = obj.to_html
					}
					assert_equal %p, rval

				} % [ input, output ]
			}

			code << %{
				end
			}


			debugMsg "--- %s [%s]:\n%s\n---\n" % [sname, desc, code]
			eval code
		end

	}

end


__END__

### [Paragraphs and Line Breaks]

# Paragraphs
<<<
This is some stuff that should all be 
put in one paragraph
even though 
it occurs over several lines.

And this is a another
one.
--- Should become:
<p>This is some stuff that should all be 
put in one paragraph
even though 
it occurs over several lines.</p>

<p>And this is a another
one.</p>
>>>

# Line breaks
<<<
Mostly the same kind of thing  
with two spaces at the end  
of each line  
should result in  
line breaks, though.

And this is a another  
one.
--- Should become:
<p>Mostly the same kind of thing<br/>
with two spaces at the end<br/>
of each line<br/>
should result in<br/>
line breaks, though.</p>

<p>And this is a another<br/>
one.</p>
>>>

# Escaping special characters
<<<
The left shift operator, which is written as <<, is often used & greatly admired.
--- Should become:
<p>The left shift operator, which is written as &lt;&lt;, is often used &amp; greatly admired.</p>
>>>

# Preservation of named entities
<<<
The left shift operator, which is written as &lt;&lt;, is often used &amp; greatly admired.
--- Should become:
<p>The left shift operator, which is written as &lt;&lt;, is often used &amp; greatly admired.</p>
>>>

# Preservation of decimal-encoded entities
<<<
The left shift operator, which is written as &#060;&#060;, is often used &#038; greatly admired.
--- Should become:
<p>The left shift operator, which is written as &#060;&#060;, is often used &#038; greatly admired.</p>
>>>

# Preservation of hex-encoded entities
<<<
The left shift operator, which is written as &#x3c;&#x3c;, is often used &#x26; greatly admired.
--- Should become:
<p>The left shift operator, which is written as &#x3c;&#x3c;, is often used &#x26; greatly admired.</p>
>>>

# Inline HTML - table tags
<<<
This is a regular paragraph.

<table>
    <tr>
        <td>Foo</td>
    </tr>
</table>

This is another regular paragraph.
--- Should become:
<p>This is a regular paragraph.</p>

<table>
    <tr>
        <td>Foo</td>
    </tr>
</table>

<p>This is another regular paragraph.</p>
>>>

# Inline HTML - div tags
<<<
This is a regular paragraph.

<div>
   Something
</div>
Something else.
--- Should become:
<p>This is a regular paragraph.</p>

<div>
   Something
</div>

<p>Something else.</p>
>>>


# Inline HTML - Plain HR
<<<
This is a regular paragraph.

<hr />

Something else.
--- Should become:
<p>This is a regular paragraph.</p>

<hr />

<p>Something else.</p>
>>>


# Inline HTML - Fancy HR
<<<
This is a regular paragraph.

<hr class="publishers-mark" id="first-hrule" />

Something else.
--- Should become:
<p>This is a regular paragraph.</p>

<hr class="publishers-mark" id="first-hrule" />

<p>Something else.</p>
>>>


# Inline HTML - Iframe
<<<
This is a regular paragraph.

<iframe src="foo.html" id="foo-frame"></iframe>

Something else.
--- Should become:
<p>This is a regular paragraph.</p>

<iframe src="foo.html" id="foo-frame"></iframe>

<p>Something else.</p>
>>>


# Inline HTML - mathml
<<<
Examples
--------

Now that we have met some of the key players, it is time to see what we can
do. Here are some examples and comments which illustrate the use of the basic
layout and token elements. Consider the expression x2 + 4x + 4 = 0. A basic
MathML presentation encoding for this would be:

<math>
  <mrow>
	<msup>
	  <mi>x</mi>
	  <mn>2</mn>
	</msup>
	<mo>+</mo>
	<mn>4</mn>
	<mi>x</mi>
	<mo>+</mo>
	<mn>4</mn>
	<mo>=</mo>
	<mn>0</mn>
  </mrow>
</math>

This encoding will display as you would expect. However, if we were interested
in reusing this expression in unknown situations, we would likely want to spend
a little more effort analyzing and encoding the logical expression structure.

--- Should become:
<h2>Examples</h2>

<p>Now that we have met some of the key players, it is time to see what we can
do. Here are some examples and comments which illustrate the use of the basic
layout and token elements. Consider the expression x2 + 4x + 4 = 0. A basic
MathML presentation encoding for this would be:</p>

<math>
  <mrow>
    <msup>
      <mi>x</mi>
      <mn>2</mn>
    </msup>
    <mo>+</mo>
    <mn>4</mn>
    <mi>x</mi>
    <mo>+</mo>
    <mn>4</mn>
    <mo>=</mo>
    <mn>0</mn>
  </mrow>
</math>

<p>This encoding will display as you would expect. However, if we were interested
in reusing this expression in unknown situations, we would likely want to spend
a little more effort analyzing and encoding the logical expression structure.</p>
>>>


# Span-level HTML
<<<
This is some stuff with a <span class="foo">spanned bit of text</span> in
it. And <del>this *should* be a bit of deleted text</del> which should be
preserved, and part of it emphasized.
--- Should become:
<p>This is some stuff with a <span class="foo">spanned bit of text</span> in
it. And <del>this <em>should</em> be a bit of deleted text</del> which should be
preserved, and part of it emphasized.</p>
>>>

# Inline HTML (Case-sensitivity)
<<<
This is a regular paragraph.

<TABLE>
    <TR>
        <TD>Foo</TD>
    </TR>
</TABLE>

This is another regular paragraph.
--- Should become:
<p>This is a regular paragraph.</p>

<TABLE>
    <TR>
        <TD>Foo</TD>
    </TR>
</TABLE>

<p>This is another regular paragraph.</p>
>>>

# Span-level HTML (Case-sensitivity)
<<<
This is some stuff with a <SPAN CLASS="foo">spanned bit of text</SPAN> in
it. And <DEL>this *should* be a bit of deleted text</DEL> which should be
preserved, and part of it emphasized.
--- Should become:
<p>This is some stuff with a <SPAN CLASS="foo">spanned bit of text</SPAN> in
it. And <DEL>this <em>should</em> be a bit of deleted text</DEL> which should be
preserved, and part of it emphasized.</p>
>>>



### [Code spans]

# Single backtick
<<<
Making `code` work for you
--- Should become:
<p>Making <code>code</code> work for you</p>
>>>

# Literal backtick with doubling
<<<
Making `` `code` `` work for you
--- Should become:
<p>Making <code>`code`</code> work for you</p>
>>>

# Many repetitions
<<<
Making `````code````` work for you
--- Should become:
<p>Making <code>code</code> work for you</p>
>>>

# Two in a row
<<<
This `thing` should be `two` spans.
--- Should become:
<p>This <code>thing</code> should be <code>two</code> spans.</p>
>>>

# At the beginning of a newline
<<<
I should think that the
`tar` command would be universal.
--- Should become:
<p>I should think that the
<code>tar</code> command would be universal.</p>
>>>

# Entity escaping
<<<
The left angle-bracket (`&lt;`) can also be written as a decimal-encoded
(`&#060;`) or hex-encoded (`&#x3c;`) entity.
--- Should become:
<p>The left angle-bracket (<code>&amp;lt;</code>) can also be written as a decimal-encoded
(<code>&amp;#060;</code>) or hex-encoded (<code>&amp;#x3c;</code>) entity.</p>
>>>

# At the beginning of a document (Bug #525)
<<<
`world` views
--- Should become:
<p><code>world</code> views</p>
>>>




### [Code blocks]

# Para plus code block (literal tab)
<<<
This is a chunk of code:

	some.code > some.other_code

Some stuff.
--- Should become:
<p>This is a chunk of code:</p>

<pre><code>some.code &gt; some.other_code
</code></pre>

<p>Some stuff.</p>
>>>

# Para plus code block (literal tab, no colon)
<<<
This is a chunk of code

	some.code > some.other_code

Some stuff.
--- Should become:
<p>This is a chunk of code</p>

<pre><code>some.code &gt; some.other_code
</code></pre>

<p>Some stuff.</p>
>>>

# Para plus code block (tab-width spaces)
<<<
This is a chunk of code:

    some.code > some.other_code

Some stuff.
--- Should become:
<p>This is a chunk of code:</p>

<pre><code>some.code &gt; some.other_code
</code></pre>

<p>Some stuff.</p>
>>>

# Para plus code block (tab-width spaces, no colon)
<<<
This is a chunk of code

    some.code > some.other_code

Some stuff.
--- Should become:
<p>This is a chunk of code</p>

<pre><code>some.code &gt; some.other_code
</code></pre>

<p>Some stuff.</p>
>>>

# Colon with preceeding space
<<<
A regular paragraph, without a colon. :

    This is a code block.

Some stuff.
--- Should become:
<p>A regular paragraph, without a colon. :</p>

<pre><code>This is a code block.
</code></pre>

<p>Some stuff.</p>
>>>

# Single colon
<<<
:
	
	some.code > some.other_code

Some stuff.
--- Should become:
<p>:</p>

<pre><code>some.code &gt; some.other_code
</code></pre>

<p>Some stuff.</p>
>>>

# Preserve leading whitespace (Bug #541)
<<<
Examples:

          # (Waste character because first line is flush left !!!)
          # Example script1
          x = 1
          x += 1
          puts x

Some stuff.
--- Should become:
<p>Examples:</p>

<pre><code>      # (Waste character because first line is flush left !!!)
      # Example script1
      x = 1
      x += 1
      puts x
</code></pre>

<p>Some stuff.</p>
>>>


### [Horizontal Rules]

# Hrule 1
<<<
* * *
--- Should become:
<hr/>
>>>

# Hrule 2
<<<
***
--- Should become:
<hr/>
>>>

# Hrule 3
<<<
*****
--- Should become:
<hr/>
>>>

# Hrule 4
<<<
- - -
--- Should become:
<hr/>
>>>

# Hrule 5
<<<
---------------------------------------
--- Should become:
<hr/>
>>>


### [Titles]

# setext-style h1
<<<
Title Text
=
--- Should become:
<h1>Title Text</h1>
>>>

<<<
Title Text
===
--- Should become:
<h1>Title Text</h1>
>>>

<<<
Title Text
==========
--- Should become:
<h1>Title Text</h1>
>>>

# setext-style h2
<<<
Title Text
-
--- Should become:
<h2>Title Text</h2>
>>>

<<<
Title Text
---
--- Should become:
<h2>Title Text</h2>
>>>

<<<
Title Text
----------
--- Should become:
<h2>Title Text</h2>
>>>

# ATX-style h1
<<<
# Title Text
--- Should become:
<h1>Title Text</h1>
>>>

<<<
# Title Text #
--- Should become:
<h1>Title Text</h1>
>>>

<<<
# Title Text ###
--- Should become:
<h1>Title Text</h1>
>>>

<<<
# Title Text #####
--- Should become:
<h1>Title Text</h1>
>>>

# ATX-style h2
<<<
## Title Text
--- Should become:
<h2>Title Text</h2>
>>>

<<<
## Title Text #
--- Should become:
<h2>Title Text</h2>
>>>

<<<
## Title Text ###
--- Should become:
<h2>Title Text</h2>
>>>

<<<
## Title Text #####
--- Should become:
<h2>Title Text</h2>
>>>

# ATX-style h3
<<<
### Title Text
--- Should become:
<h3>Title Text</h3>
>>>

<<<
### Title Text #
--- Should become:
<h3>Title Text</h3>
>>>

<<<
### Title Text ###
--- Should become:
<h3>Title Text</h3>
>>>

<<<
### Title Text #####
--- Should become:
<h3>Title Text</h3>
>>>

# ATX-style h4
<<<
#### Title Text
--- Should become:
<h4>Title Text</h4>
>>>

<<<
#### Title Text #
--- Should become:
<h4>Title Text</h4>
>>>

<<<
#### Title Text ###
--- Should become:
<h4>Title Text</h4>
>>>

<<<
#### Title Text #####
--- Should become:
<h4>Title Text</h4>
>>>

# ATX-style h5
<<<
##### Title Text
--- Should become:
<h5>Title Text</h5>
>>>

<<<
##### Title Text #
--- Should become:
<h5>Title Text</h5>
>>>

<<<
##### Title Text ###
--- Should become:
<h5>Title Text</h5>
>>>

<<<
##### Title Text #####
--- Should become:
<h5>Title Text</h5>
>>>

# ATX-style h6
<<<
###### Title Text
--- Should become:
<h6>Title Text</h6>
>>>

<<<
###### Title Text #
--- Should become:
<h6>Title Text</h6>
>>>

<<<
###### Title Text ###
--- Should become:
<h6>Title Text</h6>
>>>

<<<
###### Title Text #####
--- Should become:
<h6>Title Text</h6>
>>>


### [Blockquotes]

# Regular 1-level blockquotes
<<<
> Email-style angle brackets
> are used for blockquotes.
--- Should become:
<blockquote>
    <p>Email-style angle brackets
    are used for blockquotes.</p>
</blockquote>
>>>

# Doubled blockquotes
<<<
> > And, they can be nested.
--- Should become:
<blockquote>
    <blockquote>
        <p>And, they can be nested.</p>
    </blockquote>
</blockquote>
>>>

# Nested blockquotes
<<<
> Email-style angle brackets
> are used for blockquotes.

> > And, they can be nested.
--- Should become:
<blockquote>
    <p>Email-style angle brackets
    are used for blockquotes.</p>
    
    <blockquote>
        <p>And, they can be nested.</p>
    </blockquote>
</blockquote>
>>>

# Lazy blockquotes
<<<
> This is a blockquote with two paragraphs. Lorem ipsum dolor sit amet,
consectetuer adipiscing elit. Aliquam hendrerit mi posuere lectus.
Vestibulum enim wisi, viverra nec, fringilla in, laoreet vitae, risus.

> Donec sit amet nisl. Aliquam semper ipsum sit amet velit. Suspendisse
id sem consectetuer libero luctus adipiscing.
--- Should become:
<blockquote>
    <p>This is a blockquote with two paragraphs. Lorem ipsum dolor sit amet,
    consectetuer adipiscing elit. Aliquam hendrerit mi posuere lectus.
    Vestibulum enim wisi, viverra nec, fringilla in, laoreet vitae, risus.</p>
    
    <p>Donec sit amet nisl. Aliquam semper ipsum sit amet velit. Suspendisse
    id sem consectetuer libero luctus adipiscing.</p>
</blockquote>
>>>


# Blockquotes containing other markdown elements
<<<
> ## This is a header.
> 
> 1.   This is the first list item.
> 2.   This is the second list item.
> 
> Here's some example code:
> 
>     return shell_exec("echo $input | $markdown_script");
--- Should become:
<blockquote>
    <h2>This is a header.</h2>
    
    <ol>
    <li>This is the first list item.</li>
    <li>This is the second list item.</li>
    </ol>
    
    <p>Here's some example code:</p>

<pre><code>return shell_exec("echo $input | $markdown_script");
</code></pre>
</blockquote>
>>>

# Blockquotes with a <pre> section
<<<
> The best approximation of the problem is the following code:
>
> <pre>
> foo + bar; foo.factorize; foo.display
> </pre>
> 
> This should result in an error on any little-endian platform.
--- Should become:
<blockquote>
    <p>The best approximation of the problem is the following code:</p>

<pre>
foo + bar; foo.factorize; foo.display
</pre>
    
    <p>This should result in an error on any little-endian platform.</p>
</blockquote>
>>>



### [Images]

# Inline image with title
<<<
![alt text](/path/img.jpg "Title")
--- Should become:
<p><img src="/path/img.jpg" alt="alt text" title="Title"/></p>
>>>

# Inline image with title (single-quotes)
<<<
![alt text](/path/img.jpg 'Title')
--- Should become:
<p><img src="/path/img.jpg" alt="alt text" title="Title"/></p>
>>>

# Inline image with title (with embedded quotes)
<<<
![alt text](/path/img.jpg 'The "Title" Image')
--- Should become:
<p><img src="/path/img.jpg" alt="alt text" title="The &quot;Title&quot; Image"/></p>
>>>

# Inline image without title
<<<
![alt text](/path/img.jpg)
--- Should become:
<p><img src="/path/img.jpg" alt="alt text"/></p>
>>>

# Inline image with quoted alt text
<<<
![the "alt text"](/path/img.jpg)
--- Should become:
<p><img src="/path/img.jpg" alt="the &quot;alt text&quot;"/></p>
>>>


# Reference image
<<<
![alt text][id]

[id]: /url/to/img.jpg "Title"
--- Should become:
<p><img src="/url/to/img.jpg" alt="alt text" title="Title"/></p>
>>>



### [Emphasis]

# Emphasis (<em>) with asterisks
<<<
Use *single splats* for emphasis.
--- Should become:
<p>Use <em>single splats</em> for emphasis.</p>
>>>

# Emphasis (<em>) with underscores
<<<
Use *underscores* for emphasis.
--- Should become:
<p>Use <em>underscores</em> for emphasis.</p>
>>>

# Strong emphasis (<strong>) with asterisks
<<<
Use **double splats** for more emphasis.
--- Should become:
<p>Use <strong>double splats</strong> for more emphasis.</p>
>>>

# Strong emphasis (<strong>) with underscores
<<<
Use __doubled underscores__ for more emphasis.
--- Should become:
<p>Use <strong>doubled underscores</strong> for more emphasis.</p>
>>>

# Combined emphasis types 1
<<<
Use *single splats* or _single unders_ for normal emphasis.
--- Should become:
<p>Use <em>single splats</em> or <em>single unders</em> for normal emphasis.</p>
>>>

# Combined emphasis types 2
<<<
Use _single unders_ for normal emphasis
or __double them__ for strong emphasis.
--- Should become:
<p>Use <em>single unders</em> for normal emphasis
or <strong>double them</strong> for strong emphasis.</p>
>>>

# Emphasis containing escaped metachars
<<<
You can include literal *\*splats\** by escaping them.
--- Should become:
<p>You can include literal <em>*splats*</em> by escaping them.</p>
>>>

# Two instances of asterisked emphasis on one line
<<<
If there's *two* splatted parts on a *single line* it should still work.
--- Should become:
<p>If there's <em>two</em> splatted parts on a <em>single line</em> it should still work.</p>
>>> 

# Two instances of double asterisked emphasis on one line
<<<
This **doubled** one should **work too**.
--- Should become:
<p>This <strong>doubled</strong> one should <strong>work too</strong>.</p>
>>> 

# Two instances of underscore emphasis on one line
<<<
If there's _two_ underbarred parts on a _single line_ it should still work.
--- Should become:
<p>If there's <em>two</em> underbarred parts on a <em>single line</em> it should still work.</p>
>>> 

# Two instances of doubled underscore emphasis on one line
<<<
This __doubled__ one should __work too__.
--- Should become:
<p>This <strong>doubled</strong> one should <strong>work too</strong>.</p>
>>> 

# Initial emphasis (asterisk)
<<<
*Something* like this should be bold.
--- Should become:
<p><em>Something</em> like this should be bold.</p>
>>>

# Initial emphasis (underscore)
<<<
_Something_ like this should be bold.
--- Should become:
<p><em>Something</em> like this should be bold.</p>
>>>

# Initial strong emphasis (asterisk)
<<<
**Something** like this should be bold.
--- Should become:
<p><strong>Something</strong> like this should be bold.</p>
>>>

# Initial strong emphasis (underscore)
<<<
__Something__ like this should be bold.
--- Should become:
<p><strong>Something</strong> like this should be bold.</p>
>>>

# Partial-word emphasis (Bug #568)
<<<
**E**xtended **TURN**
--- Should become:
<p><strong>E</strong>xtended <strong>TURN</strong></p>
>>>



### [Links]

# Inline link, no title
<<<
An [example](http://url.com/).
--- Should become:
<p>An <a href="http://url.com/">example</a>.</p>
>>>

# Inline link with title
<<<
An [example](http://url.com/ "Check out url.com!").
--- Should become:
<p>An <a href="http://url.com/" title="Check out url.com!">example</a>.</p>
>>>

# Reference-style link, no title
<<<
An [example][ex] reference-style link.

[ex]: http://www.bluefi.com/
--- Should become:
<p>An <a href="http://www.bluefi.com/">example</a> reference-style link.</p>
>>>

# Reference-style link with quoted title
<<<
An [example][ex] reference-style link.

[ex]: http://www.bluefi.com/ "Check out our air."
--- Should become:
<p>An <a href="http://www.bluefi.com/" title="Check out our air.">example</a> reference-style link.</p>
>>>

# Reference-style link with paren title
<<<
An [example][ex] reference-style link.

[ex]: http://www.bluefi.com/ (Check out our air.)
--- Should become:
<p>An <a href="http://www.bluefi.com/" title="Check out our air.">example</a> reference-style link.</p>
>>>

# Reference-style link with one of each (hehe)
<<<
An [example][ex] reference-style link.

[ex]: http://www.bluefi.com/ "Check out our air.)
--- Should become:
<p>An <a href="http://www.bluefi.com/" title="Check out our air.">example</a> reference-style link.</p>
>>>

" <- For syntax highlighting

# Reference-style link with intervening space
<<<
You can split the [linked part] [ex] from
the reference part with a single space.

[ex]: http://www.treefrog.com/ "for some reason"
--- Should become:
<p>You can split the <a href="http://www.treefrog.com/" title="for some reason">linked part</a> from
the reference part with a single space.</p>
>>>

# Reference-style link with intervening space
<<<
You can split the [linked part]
 [ex] from the reference part
with a newline in case your editor wraps it there, I guess.

[ex]: http://www.treefrog.com/
--- Should become:
<p>You can split the <a href="http://www.treefrog.com/">linked part</a> from the reference part
with a newline in case your editor wraps it there, I guess.</p>
>>>

# Reference-style anchors
<<<
I get 10 times more traffic from [Google] [1] than from
[Yahoo] [2] or [MSN] [3].

  [1]: http://google.com/        "Google"
  [2]: http://search.yahoo.com/  "Yahoo Search"
  [3]: http://search.msn.com/    "MSN Search"
--- Should become:
<p>I get 10 times more traffic from <a href="http://google.com/" title="Google">Google</a> than from
<a href="http://search.yahoo.com/" title="Yahoo Search">Yahoo</a> or <a href="http://search.msn.com/" title="MSN Search">MSN</a>.</p>
>>>

# Implicit name-link shortcut anchors
<<<
I get 10 times more traffic from [Google][] than from
[Yahoo][] or [MSN][].

  [google]: http://google.com/        "Google"
  [yahoo]:  http://search.yahoo.com/  "Yahoo Search"
  [msn]:    http://search.msn.com/    "MSN Search"
--- Should become:
<p>I get 10 times more traffic from <a href="http://google.com/" title="Google">Google</a> than from
<a href="http://search.yahoo.com/" title="Yahoo Search">Yahoo</a> or <a href="http://search.msn.com/" title="MSN Search">MSN</a>.</p>
>>>

# Inline anchors
<<<
I get 10 times more traffic from [Google](http://google.com/ "Google")
than from [Yahoo](http://search.yahoo.com/ "Yahoo Search") or
[MSN](http://search.msn.com/ "MSN Search").
--- Should become:
<p>I get 10 times more traffic from <a href="http://google.com/" title="Google">Google</a>
than from <a href="http://search.yahoo.com/" title="Yahoo Search">Yahoo</a> or
<a href="http://search.msn.com/" title="MSN Search">MSN</a>.</p>
>>>

# Graceful fail for unclosed brackets (and bug #524)
<<<
This is just a [bracket opener; it should fail gracefully.
--- Should become:
<p>This is just a [bracket opener; it should fail gracefully.</p>
>>>

# Unresolved reference-style links (Bug #620)
<<<
This is an unresolved [url][1].
--- Should become:
<p>This is an unresolved [url][1].</p>
>>>


### [Auto-links]

# Plain HTTP link
<<<
This is a reference to <http://www.FaerieMUD.org/>. You should follow it.
--- Should become:
<p>This is a reference to <a href="http://www.FaerieMUD.org/">http://www.FaerieMUD.org/</a>. You should follow it.</p>
>>>

# FTP link
<<<
Why not download your very own chandelier from <ftp://ftp.usuc.edu/pub/foof/mir/>?
--- Should become:
<p>Why not download your very own chandelier from <a href="ftp://ftp.usuc.edu/pub/foof/mir/">ftp://ftp.usuc.edu/pub/foof/mir/</a>?</p>
>>>


### [Lists]

# Unordered list
<<<
*   Red
*   Green
*   Blue
--- Should become:
<ul>
<li>Red</li>
<li>Green</li>
<li>Blue</li>
</ul>
>>>

# Unordered list w/alt bullets
<<<
-   Red
-   Green
-   Blue
--- Should become:
<ul>
<li>Red</li>
<li>Green</li>
<li>Blue</li>
</ul>
>>>

# Unordered list w/alt bullets 2
<<<
+   Red
+   Green
+   Blue
--- Should become:
<ul>
<li>Red</li>
<li>Green</li>
<li>Blue</li>
</ul>
>>>

# Unordered list w/mixed bullets
<<<
+   Red
-   Green
*   Blue
--- Should become:
<ul>
<li>Red</li>
<li>Green</li>
<li>Blue</li>
</ul>
>>>

# Ordered list
<<<
1.  Bird
2.  McHale
3.  Parish
--- Should become:
<ol>
<li>Bird</li>
<li>McHale</li>
<li>Parish</li>
</ol>
>>>

# Ordered list, any numbers
<<<
1.  Bird
1.  McHale
1.  Parish
--- Should become:
<ol>
<li>Bird</li>
<li>McHale</li>
<li>Parish</li>
</ol>
>>>

# Ordered list, any numbers 2
<<<
3.  Bird
1.  McHale
8.  Parish
--- Should become:
<ol>
<li>Bird</li>
<li>McHale</li>
<li>Parish</li>
</ol>
>>>

# Hanging indents
<<<
*   Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
    Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
    viverra nec, fringilla in, laoreet vitae, risus.
*   Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
    Suspendisse id sem consectetuer libero luctus adipiscing.
--- Should become:
<ul>
<li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
viverra nec, fringilla in, laoreet vitae, risus.</li>
<li>Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
Suspendisse id sem consectetuer libero luctus adipiscing.</li>
</ul>
>>>

# Lazy indents
<<<
*   Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
viverra nec, fringilla in, laoreet vitae, risus.
*   Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
Suspendisse id sem consectetuer libero luctus adipiscing.
--- Should become:
<ul>
<li>Lorem ipsum dolor sit amet, consectetuer adipiscing elit.
Aliquam hendrerit mi posuere lectus. Vestibulum enim wisi,
viverra nec, fringilla in, laoreet vitae, risus.</li>
<li>Donec sit amet nisl. Aliquam semper ipsum sit amet velit.
Suspendisse id sem consectetuer libero luctus adipiscing.</li>
</ul>
>>>

# Paragraph wrapped list items
<<<
*   Bird

*   Magic
--- Should become:
<ul>
<li><p>Bird</p></li>
<li><p>Magic</p></li>
</ul>
>>>

# Multi-paragraph list items
<<<
1.  This is a list item with two paragraphs. Lorem ipsum dolor
    sit amet, consectetuer adipiscing elit. Aliquam hendrerit
    mi posuere lectus.

    Vestibulum enim wisi, viverra nec, fringilla in, laoreet
    vitae, risus. Donec sit amet nisl. Aliquam semper ipsum
    sit amet velit.

2.  Suspendisse id sem consectetuer libero luctus adipiscing.
--- Should become:
<ol>
<li><p>This is a list item with two paragraphs. Lorem ipsum dolor
sit amet, consectetuer adipiscing elit. Aliquam hendrerit
mi posuere lectus.</p>

<p>Vestibulum enim wisi, viverra nec, fringilla in, laoreet
vitae, risus. Donec sit amet nisl. Aliquam semper ipsum
sit amet velit.</p></li>
<li><p>Suspendisse id sem consectetuer libero luctus adipiscing.</p></li>
</ol>
>>>

# Lazy multi-paragraphs
<<<
*   This is a list item with two paragraphs.

    This is the second paragraph in the list item. You're
only required to indent the first line. Lorem ipsum dolor
sit amet, consectetuer adipiscing elit.

*   Another item in the same list.
--- Should become:
<ul>
<li><p>This is a list item with two paragraphs.</p>

<p>This is the second paragraph in the list item. You're
only required to indent the first line. Lorem ipsum dolor
sit amet, consectetuer adipiscing elit.</p></li>
<li><p>Another item in the same list.</p></li>
</ul>
>>>

# Blockquote in list item
<<<
*   A list item with a blockquote:

    > This is a blockquote
    > inside a list item.
--- Should become:
<ul>
<li><p>A list item with a blockquote:</p>

<blockquote>
    <p>This is a blockquote
    inside a list item.</p>
</blockquote></li>
</ul>
>>>

# Code block in list item
<<<
*   A list item with a code block:

        <code goes here>
--- Should become:
<ul>
<li><p>A list item with a code block:</p>

<pre><code>&lt;code goes here&gt;
</code></pre></li>
</ul>
>>>

# Backslash-escaped number-period-space
<<<
1986\. What a great season.
--- Should become:
<p>1986. What a great season.</p>
>>>


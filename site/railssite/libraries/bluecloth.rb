#!/usr/bin/ruby
# 
# Bluecloth is a Ruby implementation of Markdown, a text-to-HTML conversion
# tool.
# 
# == Synopsis
# 
#   doc = BlueCloth::new "
#     ## Test document ##
#
#     Just a simple test.
#   "
#
#   puts doc.to_html
# 
# == Authors
# 
# * Michael Granger <ged@FaerieMUD.org>
# 
# == Contributors
#
# * Martin Chase <stillflame@FaerieMUD.org> - Peer review, helpful suggestions
# * Florian Gross <flgr@ccan.de> - Filter options, suggestions
#
# == Copyright
#
# Original version:
#   Copyright (c) 2003-2004 John Gruber
#   <http://daringfireball.net/>  
#   All rights reserved.
#
# Ruby port:
#   Copyright (c) 2004 The FaerieMUD Consortium.
# 
# BlueCloth is free software; you can redistribute it and/or modify it under the
# terms of the GNU General Public License as published by the Free Software
# Foundation; either version 2 of the License, or (at your option) any later
# version.
# 
# BlueCloth is distributed in the hope that it will be useful, but WITHOUT ANY
# WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
# A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
# 
# == To-do
#
# * Refactor some of the larger uglier methods that have to do their own
#   brute-force scanning because of lack of Perl features in Ruby's Regexp
#   class. Alternately, could add a dependency on 'pcre' and use most Perl
#   regexps.
#
# * Put the StringScanner in the render state for thread-safety.
#
# == Version
#
#  $Id: bluecloth.rb,v 1.3 2004/05/02 15:56:33 webster132 Exp $
# 

require 'digest/md5'
require 'logger'
require 'strscan'


### BlueCloth is a Ruby implementation of Markdown, a text-to-HTML conversion
### tool.
class BlueCloth < String

	### Exception class for formatting errors.
	class FormatError < RuntimeError

		### Create a new FormatError with the given source +str+ and an optional
		### message about the +specific+ error.
		def initialize( str, specific=nil )
			if specific
				msg = "Bad markdown format near %p: %s" % [ str, specific ]
			else
				msg = "Bad markdown format near %p" % str
			end

			super( msg )
		end
	end


	# Release Version
	Version = '0.0.3'

	# SVN Revision
	SvnRev = %q$Rev: 37 $

	# SVN Id tag
	SvnId = %q$Id: bluecloth.rb,v 1.3 2004/05/02 15:56:33 webster132 Exp $

	# SVN URL
	SvnUrl = %q$URL: svn+ssh://cvs.faeriemud.org/var/svn/BlueCloth/trunk/lib/bluecloth.rb $


	# Rendering state struct. Keeps track of URLs, titles, and HTML blocks
	# midway through a render. I prefer this to the globals of the Perl version
	# because globals make me break out in hives. Or something.
	RenderState = Struct::new( "RenderState", :urls, :titles, :html_blocks, :log )

	# Tab width for #detab! if none is specified
	TabWidth = 4

	# The tag-closing string -- set to '>' for HTML
	EmptyElementSuffix = "/>";

	# Table of MD5 sums for escaped characters
	EscapeTable = {}
	'\\`*_{}[]()#.!'.split(//).each {|char|
		hash = Digest::MD5::hexdigest( char )

		EscapeTable[ char ] = {
 			:md5 => hash,
			:md5re => Regexp::new( hash ),
			:re  => Regexp::new( '\\\\' + Regexp::escape(char) ),
		}
	}


	#################################################################
	###	I N S T A N C E   M E T H O D S
	#################################################################

	### Create a new BlueCloth string.
	def initialize( content="", *restrictions )
		@log = Logger::new( $deferr )
		@log.level = $DEBUG ?
			Logger::DEBUG :
			($VERBOSE ? Logger::INFO : Logger::WARN)
		@scanner = nil

		# Add any restrictions, and set the line-folding attribute to reflect
		# what happens by default.
		restrictions.flatten.each {|r| __send__("#{r}=", true) }
		@fold_lines = true

		super( content )

		@log.debug "String is: %p" % self
	end


	######
	public
	######

	# Filters for controlling what gets output for untrusted input. (But really,
	# you're filtering bad stuff out of untrusted input at submission-time via
	# untainting, aren't you?)
	attr_accessor :filter_html, :filter_styles

	# RedCloth-compatibility accessor. Line-folding is part of Markdown syntax,
	# so this isn't used by anything.
	attr_accessor :fold_lines


	### Render Markdown-formatted text in this string object as HTML and return
	### it. The parameter is for compatibility with RedCloth, and is currently
	### unused, though that may change in the future.
	def to_html( lite=false )

		# Create a StringScanner we can reuse for various lexing tasks
		@scanner = StringScanner::new( '' )

		# Make a structure to carry around stuff that gets placeholdered out of
		# the source.
		rs = RenderState::new( {}, {}, {} )

		# Make a copy of the string with normalized line endings, tabs turned to
		# spaces, and a couple of guaranteed newlines at the end
		text = self.gsub( /\r\n?/, "\n" ).detab
		text += "\n\n"
		@log.debug "Normalized line-endings: %p" % text

		# Filter HTML if we're asked to do so
		if self.filter_html
			text.gsub!( "<", "&lt;" )
			text.gsub!( ">", "&gt;" )
			@log.debug "Filtered HTML: %p" % text
		end

		# Simplify blank lines
		text.gsub!( /^ +$/, '' )
		@log.debug "Tabs -> spaces/blank lines stripped: %p" % text

		# Replace HTML blocks with placeholders
		text = hide_html_blocks( text, rs )
		@log.debug "Hid HTML blocks: %p" % text
		@log.debug "Render state: %p" % rs

		# Strip link definitions, store in render state
		text = strip_link_definitions( text, rs )
		@log.debug "Stripped link definitions: %p" % text
		@log.debug "Render state: %p" % rs

		# Escape meta-characters
		text = escape_special_chars( text )
		@log.debug "Escaped special characters: %p" % text

		# Transform block-level constructs
		text = apply_block_transforms( text, rs )
		@log.debug "After block-level transforms: %p" % text

		# Now swap back in all the escaped characters
		text = unescape_special_chars( text )
		@log.debug "After unescaping special characters: %p" % text

		return text
	end
	

	### Convert tabs in +str+ to spaces.
	def detab( tabwidth=TabWidth )
		copy = self.dup
		copy.detab!( tabwidth )
		return copy
	end


	### Convert tabs to spaces in place and return self if any were converted.
	def detab!( tabwidth=TabWidth )
		newstr = self.split( /\n/ ).collect {|line|
			line.gsub( /(.*?)\t/ ) do
				$1 + ' ' * (tabwidth - $1.length % tabwidth)
			end
		}.join("\n")
		self.replace( newstr )
	end


	#######
	#private
	#######

	### Do block-level transforms on a copy of +str+ using the specified render
	### state +rs+ and return the results.
	def apply_block_transforms( str, rs )
		# Port: This was called '_runBlockGamut' in the original

		@log.debug "Applying block transforms to:\n  %p" % str
		text = transform_headers( str, rs )
		text = transform_hrules( text, rs )
		text = transform_lists( text, rs )
		text = transform_code_blocks( text, rs )
		text = transform_block_quotes( text, rs )
		text = transform_auto_links( text, rs )
		text = hide_html_blocks( text, rs )

		text = form_paragraphs( text, rs )

		@log.debug "Done with block transforms:\n  %p" % text
		return text
	end


	### Apply Markdown span transforms to a copy of the specified +str+ with the
	### given render state +rs+ and return it.
	def apply_span_transforms( str, rs )
		@log.debug "Applying span transforms to:\n  %p" % str

		str = transform_code_spans( str, rs )
		str = encode_html( str )
		str = transform_images( str, rs )
		str = transform_anchors( str, rs )
		str = transform_italic_and_bold( str, rs )

		# Hard breaks
		str.gsub!( / {2,}\n/, "<br#{EmptyElementSuffix}\n" )

		@log.debug "Done with span transforms:\n  %p" % str
		return str
	end


	# The list of tags which are considered block-level constructs and an
	# alternation pattern suitable for use in regexps made from the list
	BlockTags = %w[ p div h[1-6] blockquote pre table dl ol ul script ]
	BlockTagPattern = BlockTags.join('|')

	# Nested blocks:
	# 	<div>
	# 		<div>
	# 		tags for inner block must be indented.
	# 		</div>
	# 	</div>
	StrictBlockRegex = %r{
		^						# Start of line
		<(#{BlockTagPattern})	# Start tag: \2
		\b						# word break
		(.*\n)*?				# Any number of lines, minimal match
		</\1>					# Matching end tag
		[ ]*					# trailing spaces
		(?=\n+|\Z)				# End of line or document
	  }ix

	# More-liberal block-matching
	LooseBlockRegex = %r{
		^						# Start of line
		<(#{BlockTagPattern})	# start tag: \2
		\b						# word break
		(.*\n)*?				# Any number of lines, minimal match
		.*</\1>					# Anything + Matching end tag
		[ ]*					# trailing spaces
		(?=\n+|\Z)				# End of line or document
	  }ix

	# Special case for <hr />.
	HruleBlockRegex = %r{
		(						# $1
			\A\n?				# Start of doc + optional \n
			|					# or
			.*\n\n				# anything + blank line
		)
		(						# save in $2
			[ ]*				# Any spaces
			<hr					# Tag open
			\b					# Word break
			([^<>])*?			# Attributes
			/?>					# Tag close
			(?=\n\n|\Z)			# followed by a blank line or end of document
		)
	  }ix

	### Replace all blocks of HTML in +str+ that start in the left margin with
	### tokens.
	def hide_html_blocks( str, rs )
		@log.debug "Hiding HTML blocks in %p" % str
		
		# Tokenizer proc to pass to gsub
		tokenize = lambda {|match|
			key = Digest::MD5::hexdigest( match )
			rs.html_blocks[ key ] = match
			@log.debug "Replacing %p with %p" %
				[ match, key ]
			"\n\n#{key}\n\n"
		}

		rval = str.dup

		@log.debug "Finding blocks with the strict regex..."
		rval.gsub!( StrictBlockRegex, &tokenize )

		@log.debug "Finding blocks with the loose regex..."
		rval.gsub!( LooseBlockRegex, &tokenize )

		@log.debug "Finding hrules..."
		rval.gsub!( HruleBlockRegex ) {|match| $1 + tokenize[$2] }

		return rval
	end


	# Link defs are in the form: ^[id]: url "optional title"
	LinkRegex = %r{
		^[ ]*\[(.+)\]:		# id = $1
		  [ ]*
		  \n?				# maybe *one* newline
		  [ ]*
		(\S+)				# url = $2
		  [ ]*
		  \n?				# maybe one newline
		  [ ]*
		(?:
			# Titles are delimited by "quotes" or (parens).
			["(]
			(.+?)			# title = $3
			[")]			# Matching ) or "
			[ ]*
		)?	# title is optional
		(?:\n+|\Z)
	  }x

	### Strip link definitions from +str+, storing them in the given RenderState
	### +rs+.
	def strip_link_definitions( str, rs )
		str.gsub( LinkRegex ) {|match|
			id, url, title = $1, $2, $3

			rs.urls[ id.downcase ] = encode_html( url )
			unless title.nil?
				rs.titles[ id.downcase ] = title.gsub( /"/, "&quot;" )
			end
			""
		}
	end


	### Escape special characters in the given +str+
	def escape_special_chars( str )
		@log.debug "  Escaping special characters"
		text = ''

		tokenize_html( str ) {|token, str|
			@log.debug "   Adding %p token %p" % [ token, str ]
			case token

			# Within tags, encode * and _
			when :tag
				text += str.
					gsub( /\*/, EscapeTable['*'][:md5] ).
					gsub( /_/, EscapeTable['_'][:md5] )

			# Encode backslashed stuff in regular text
			when :text
				text += encode_backslash_escapes( str )
			else
				raise TypeError, "Unknown token type %p" % token
			end
		}

		@log.debug "  Text with escapes is now: %p" % text
		return text
	end


	### Swap escaped special characters in a copy of the given +str+ and return
	### it.
	def unescape_special_chars( str )
		EscapeTable.each {|char, hash|
			@log.debug "Unescaping escaped %p with %p" %
				[ char, hash[:md5re] ]
			str.gsub!( hash[:md5re], char )
		}

		return str
	end


	### Return a copy of the given +str+ with any backslashed special character
	### in it replaced with MD5 placeholders.
	def encode_backslash_escapes( str )
		# Make a copy with any double-escaped backslashes encoded
		text = str.gsub( /\\\\/, EscapeTable['\\'][:md5] )
		
		EscapeTable.each_pair {|char, esc|
			next if char == '\\'
			text.gsub!( esc[:re], esc[:md5] )
		}

		return text
	end


	### Transform any Markdown-style horizontal rules in a copy of the specified
	### +str+ and return it.
	def transform_hrules( str, rs )
		@log.debug " Transforming horizontal rules"
		str.gsub( /^( ?[\-\*] ?){3,}$/, "\n<hr#{EmptyElementSuffix}\n" )
	end



	# Pattern to transform lists
	ListRegexp = %r{
		  (?:
			^[ ]{0,#{TabWidth - 1}}	# Indent < tab width
			(\*|\d+\.)						# unordered or ordered ($1)
			[ ]+							# At least one space
		  )
		  (?m:.+?)							# item content (include newlines)
		  (?:
			  \z							# Either EOF
			|								#  or
			  \n{2,}						# Blank line...
			  (?=\S)						# ...followed by non-space
			  (?![ ]* (\*|\d+\.) [ ]+)		# ...but not another item
		  )
	  }x

	### Transform Markdown-style lists in a copy of the specified +str+ and
	### return it.
	def transform_lists( str, rs )
		@log.debug " Transforming lists at %p" % (str[0,100] + '...')

		str.gsub( ListRegexp ) {|list|
			@log.debug "  Found list %p" % list
			list_type = ($1 == '*' ? "ul" : "ol")
			list.gsub!( /\n{2,}/, "\n\n\n" )

			%{<%s>\n%s</%s>\n} % [
				list_type,
				transform_list_items( list, rs ),
				list_type,
			]
		}
	end


	# Pattern for transforming list items
	ListItemRegexp = %r{
		(\n)?							# leading line = $1
		(^[ ]*)							# leading whitespace = $2
		(\*|\d+\.) [ ]+					# list marker = $3
		((?m:.+?)						# list item text   = $4
		(\n{1,2}))
		(?= \n* (\z | \2 (\*|\d+\.) [ ]+))
	  }x

	### Transform list items in a copy of the given +str+ and return it.
	def transform_list_items( str, rs )
		@log.debug " Transforming list items"

		# Trim trailing blank lines
		str = str.sub( /\n{2,}\z/, "\n" )

		str.gsub( ListItemRegexp ) {|line|
			@log.debug "  Found item line %p" % line
			leading_line, item = $1, $4

			if leading_line or /\n{2,}/.match( item )
				@log.debug "   Found leading line or item has a blank"
				item = apply_block_transforms( outdent(item), rs )
			else
				# Recursion for sub-lists
				@log.debug "   Recursing for sublist"
				item = transform_lists( outdent(item), rs ).chomp
				item = apply_span_transforms( item, rs )
			end

			%{<li>%s</li>\n} % item
		}
	end


	# Pattern for matching codeblocks
	CodeBlockRegexp = %r{
		(.?)								# $1 = preceding character
		:\n+								# colon + NL delimiter
		(									# $2 = the code block
		  (?:
			(?:[ ]{#{TabWidth}} | \t)		# a tab or tab-width of spaces
			.*\n+
		  )+
		)
		((?=^[ ]{0,#{TabWidth}}\S)|\Z)		# Lookahead for non-space at
											# line-start, or end of doc
	  }x

	### Transform Markdown-style codeblocks in a copy of the specified +str+ and
	### return it.
	def transform_code_blocks( str, rs )
		@log.debug " Transforming code blocks"

		str.gsub( CodeBlockRegexp ) {|block|
			prevchar, codeblock = $1, $2

			@log.debug "  prevchar = %p" % prevchar

			# Generated the codeblock
			%{%s\n\n<pre><code>%s\n</code></pre>\n\n} % [
				(prevchar.empty? || /\s/ =~ prevchar) ? "" : "#{prevchar}:",
				encode_code( outdent(codeblock), rs ).rstrip,
			]
		}
	end


	# Pattern for matching Markdown blockquote blocks
	BlockQuoteRegexp = %r{
		  (?:
			^[ ]*>[ ]?		# '>' at the start of a line
			  .+\n			# rest of the first line
			(?:.+\n)*		# subsequent consecutive lines
			\n*				# blanks
		  )+
	  }x

	### Transform Markdown-style blockquotes in a copy of the specified +str+
	### and return it.
	def transform_block_quotes( str, rs )
		@log.debug " Transforming block quotes"

		str.gsub( BlockQuoteRegexp ) {|quote|
			@log.debug "Making blockquote from %p" % quote
			quote.gsub!( /^[ ]*>[ ]?/, '' )
			%{<blockquote>\n%s\n</blockquote>\n\n} %
				apply_block_transforms( quote, rs ).
				gsub( /^/, " " * TabWidth )
		}
	end


	AutoAnchorURLRegexp = /<((https?|ftp):[^'">\s]+)>/
	AutoAnchorEmailRegexp = %r{
		<
		(
			[-.\w]+
			\@
			[-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+
		)
		>
	  }x

	### Transform URLs in a copy of the specified +str+ into links and return
	### it.
	def transform_auto_links( str, rs )
		@log.debug " Transforming auto-links"
		str.gsub( AutoAnchorURLRegexp, %{<a href="\\1">\\1</a>}).
			gsub( AutoAnchorEmailRegexp ) {|addr|
			encode_email_address( unescape_special_chars($1) )
		}
	end


	# Encoder functions to turn characters of an email address into encoded
	# entities.
	Encoders = [
		lambda {|char| "&#%03d;" % char},
		lambda {|char| "&#x%X;" % char},
		lambda {|char| char.chr },
	]

	### Transform a copy of the given email +addr+ into an escaped version safer
	### for posting publicly.
	def encode_email_address( addr )

		rval = ''
		("mailto:" + addr).each_byte {|b|
			case b
			when ?:
				rval += ":"
			when ?@
				rval += Encoders[ rand(2) ][ b ]
			else
				r = rand(100)
				rval += (
					r > 90 ? Encoders[2][ b ] :
					r < 45 ? Encoders[1][ b ] :
							 Encoders[0][ b ]
				)
			end
		}

		return %{<a href="%s">%s</a>} % [ rval, rval.sub(/.+?:/, '') ]
	end


	# Regex for matching Setext-style headers
	SetextHeaderRegexp = %r{
		(.+)			# The title text ($1)
		\n
		([\-=])+		# Match a line of = or -. Save only one in $2.
		[ ]*\n+
	   }x

	# Regexp for matching ATX-style headers
	AtxHeaderRegexp = %r{
		^(\#{1,6})	# $1 = string of #'s
		[ ]*
		(.+?)		# $2 = Header text
		[ ]*
		\#*			# optional closing #'s (not counted)
		\n+
	  }x

	### Apply Markdown header transforms to a copy of the given +str+ amd render
	### state +rs+ and return the result.
	def transform_headers( str, rs )
		@log.debug " Transforming headers"

		# Setext-style headers:
		#	  Header 1
		#	  ========
		#  
		#	  Header 2
		#	  --------
		#
		str.
			gsub( SetextHeaderRegexp ) {|m|
				@log.debug "Found setext-style header"
				title, hdrchar = $1, $2
				title = apply_span_transforms( title, rs )

				case hdrchar
				when '='
					%[<h1>#{title}</h1>\n\n]
				when '-'
					%[<h2>#{title}</h2>\n\n]
				else
					title
				end
			}.

			gsub( AtxHeaderRegexp ) {|m|
				@log.debug "Found ATX-style header"
				hdrchars, title = $1, $2
				title = apply_span_transforms( title, rs )

				level = hdrchars.length
				%{<h%d>%s</h%d>\n\n} % [ level, title, level ]
			}
	end


	### Wrap all remaining paragraph-looking text in a copy of +str+ inside <p>
	### tags and return it.
	def form_paragraphs( str, rs )
		@log.debug " Forming paragraphs"
		grafs = str.
			sub( /\A\n+/, '' ).
			sub( /\n+\z/, '' ).
			split( /\n{2,}/ )

		rval = grafs.collect {|graf|

			# Unhashify HTML blocks if this is a placeholder
			if rs.html_blocks.key?( graf )
				rs.html_blocks[ graf ]

			# Otherwise, wrap in <p> tags
			else
				apply_span_transforms(graf, rs).
					sub( /^[ ]*/, '<p>' ) + '</p>'
			end
		}.join( "\n\n" )

		@log.debug " Formed paragraphs: %p" % rval
		return rval
	end


	# Pattern to match the linkid part of an anchor tag for reference-style
	# links.
	RefLinkIdRegex = %r{
		[ ]?					# Optional leading space
		(?:\n[ ]*)?				# Optional newline + spaces
		\[
			(.*?)				# Id = $1
		\]
	  }x

	InlineLinkRegex = %r{
		\(						# Literal paren
			[ ]*				# Zero or more spaces
			(.*?)				# URI = $1
			[ ]*				# Zero or more spaces
			(?:					# 
				([\"\'])		# Opening quote char = $2
				(.*?)			# Title = $3
				\2				# Matching quote char
			)?					# Title is optional
		\)
	  }x

	### Apply Markdown anchor transforms to a copy of the specified +str+ with
	### the given render state +rs+ and return it.
	def transform_anchors( str, rs )
		@log.debug " Transforming anchors"
		@scanner.string = str.dup
		text = ''

		# Scan the whole string
		until @scanner.empty?
		
			if @scanner.scan( /\[/ )
				link = ''; linkid = ''
				depth = 1
				startpos = @scanner.pos
				@log.debug " Found a bracket-open at %d" % startpos

				# Scan the rest of the tag, allowing unlimited nested []s. If
				# the scanner runs out of text before the opening bracket is
				# closed, append the text and return (wasn't a valid anchor).
				while depth.nonzero?
					linktext = @scanner.scan_until( /\]|\[/ )

					if linktext
						@log.debug "  Found a bracket at depth %d: %p" %
							[ depth, linktext ]
						link += linktext

						# Decrement depth for each closing bracket
						depth += ( linktext[-1, 1] == ']' ? -1 : 1 )
						@log.debug "  Depth is now #{depth}"

					# If there's no more brackets, it must not be an anchor, so
					# just abort.
					else
						@log.debug "  Missing closing brace, assuming non-link."
						link += @scanner.rest
						@scanner.terminate
						return text + '[' + link
					end
				end
				link.slice!( -1 ) # Trim final ']'
				@log.debug " Found leading link %p" % link

				# Look for a reference-style second part
				if @scanner.scan( RefLinkIdRegex )
					linkid = @scanner[1]
					linkid = link.dup if linkid.empty?
					linkid.downcase!
					@log.debug "  Found a linkid: %p" % linkid

					# If there's a matching link in the link table, build an
					# anchor tag for it.
					if rs.urls.key?( linkid )
						@log.debug "   Found link key in the link table: %p" %
							rs.urls[linkid]
						url = escape_md( rs.urls[linkid] )

						text += %{<a href="#{url}"}
						if rs.titles.key?(linkid)
							text += %{ title="%s"} % escape_md( rs.titles[linkid] )
						end
						text += %{>#{link}</a>}

					# If the link referred to doesn't exist, just append the raw
					# source to the result
					else
						@log.debug "  Linkid %p not found in link table" % linkid
						@log.debug "  Appending original string instead: %p" %
							@scanner.string[ startpos-1 .. @scanner.pos ]
						text += @scanner.string[ startpos-1 .. @scanner.pos ]
					end

				# ...or for an inline style second part
				elsif @scanner.scan( InlineLinkRegex )
					url = @scanner[1]
					title = @scanner[3]
					@log.debug "  Found an inline link to %p" % url

					text += %{<a href="%s"} % escape_md( url )
					if title
						text += %{ title="%s"} % escape_md( title )
					end
					text += %{>#{link}</a>}

				# No linkid part: just append the first part as-is.
				else
					@log.debug "No linkid, so no anchor. Appending literal text."
					text += @scanner.string[ startpos-1 .. @scanner.pos-1 ]
				end # if linkid

			# Plain text
			else
				@log.debug " Scanning to the next link from %p" % @scanner.rest
				text += @scanner.scan( /[^\[]+/ )
			end

		end # until @scanner.empty?

		return text
	end

	# Pattern to match strong emphasis in Markdown text
	BoldRegexp = %r{ (\*\*|__) (?=\S) (.+?\S) \1 }x

	# Pattern to match normal emphasis in Markdown text
	ItalicRegexp = %r{ (\*|_) (?=\S) (.+?\S) \1 }x

	### Transform italic- and bold-encoded text in a copy of the specified +str+
	### and return it.
	def transform_italic_and_bold( str, rs )
		@log.debug " Transforming italic and bold"

		str.
			gsub( BoldRegexp, %{<strong>\\2</strong>} ).
			gsub( ItalicRegexp, %{<em>\\2</em>} )
	end

	
	### Transform backticked spans into <code> spans.
	def transform_code_spans( str, rs )
		@log.debug " Transforming code spans"

		# Set up the string scanner and just return the string unless there's at
		# least one backtick.
		@scanner.string = str.dup
		unless @scanner.exist?( /`/ )
			@scanner.terminate
			@log.debug "No backticks found for code span in %p" % str
			return str
		end

		@log.debug "Transforming code spans in %p" % str

		# Build the transformed text anew
		text = ''

		# Scan to the end of the string
		until @scanner.empty?

			# Scan up to an opening backtick
			if pre = @scanner.scan_until( /.?(?=`)/m )
				text += pre
				@log.debug "Found backtick at %d after '...%s'" %
					[ @scanner.pos, text[-10, 10] ]

				# Make a pattern to find the end of the span
				opener = @scanner.scan( /`+/ )
				len = opener.length
				closer = Regexp::new( opener )
				@log.debug "Scanning for end of code span with %p" % closer

				# Scan until the end of the closing backtick sequence. Chop the
				# backticks off the resultant string, strip leading and trailing
				# whitespace, and encode any enitites contained in it.
				codespan = @scanner.scan_until( closer ) or
					raise FormatError::new( @scanner.rest[0,20],
						"No %p found before end" % opener )

				@log.debug "Found close of code span at %d: %p" %
					[ @scanner.pos - len, codespan ]
				codespan.slice!( -len, len )
				text += "<code>%s</code>" %
					encode_code( codespan.strip, rs )

			# If there's no more backticks, just append the rest of the string
			# and move the scan pointer to the end
			else
				text += @scanner.rest
				@scanner.terminate
			end
		end

		return text
	end


	# Next, handle inline images:  ![alt text](url "optional title")
	# Don't forget: encode * and _
	InlineImageRegexp = %r{
		(					# Whole match = $1
			!\[ (.*?) \]	# alt text = $2
		  \([ ]* (\S+) [ ]*	# source url = $3
			(				# title = $4
			  (["'])		# quote char = $5
			  .*?
			  \5			# matching quote
			  [ ]*
			)?				# title is optional
		  \)
		)
	  }xs #"


	# Reference-style images
	ReferenceImageRegexp = %r{
		(					# Whole match = $1
			!\[ (.*?) \]	# Alt text = $2
			[ ]?			# Optional space
			(?:\n[ ]*)?		# One optional newline + spaces
			\[ (.*?) \]		# id = $3
		)
	  }xs

	### Turn image markup into image tags.
	def transform_images( str, rs )
		@log.debug " Transforming images" % str

		# Handle reference-style labeled images: ![alt text][id]
		str.
			gsub( ReferenceImageRegexp ) {|match|
				whole, alt, linkid = $1, $2, $3.downcase
				@log.debug "Matched %p" % match
				res = nil

				# for shortcut links like ![this][].
				linkid = alt.downcase if linkid.empty?

				if rs.urls.key?( linkid )
					url = escape_md( rs.urls[linkid] )
					@log.debug "Found url '%s' for linkid '%s' " %
						[ url, linkid ]

					# Build the tag
					result = %{<img src="%s" alt="%s"} % [ url, alt ]
					if rs.titles.key?( linkid )
						result += %{ title="%s"} % escape_md( rs.titles[linkid] )
					end
					result += EmptyElementSuffix

				else
					result = whole
				end

				@log.debug "Replacing %p with %p" %
					[ match, result ]
				result
			}.

			# Inline image style
			gsub( InlineImageRegexp ) {|match|
				@log.debug "Found inline image %p" % match
				whole, alt, title = $1, $2, $4
				url = escape_md( $3 )

				# Build the tag
				result = %{<img src="%s" alt="%s"} % [ url, alt ]
				unless title.nil?
					result += %{ title="%s"} % escape_md( title.gsub(/^"|"$/, '') )
				end
				result += EmptyElementSuffix

				@log.debug "Replacing %p with %p" %
					[ match, result ]
				result
			}
	end


	# Regexp to match special characters in a code block
	CodeEscapeRegexp = %r{( \* | _ | \{ | \} | \[ | \] )}x

	### Escape any characters special to HTML and encode any characters special
	### to Markdown in a copy of the given +str+ and return it.
	def encode_code( str, rs )
		str.gsub( %r{&}, '&amp;' ).
			gsub( %r{<}, '&lt;' ).
			gsub( %r{>}, '&gt;' ).
			gsub( CodeEscapeRegexp ) {|match| EscapeTable[match][:md5]}
	end
				


	#################################################################
	###	U T I L I T Y   F U N C T I O N S
	#################################################################

	### Escape any markdown characters in a copy of the given +str+ and return
	### it.
	def escape_md( str )
		str.
			gsub( /\*/, '&#42;' ).
			gsub( /_/,  '&#95;' )
	end


	# Matching constructs for tokenizing X/HTML
	HTMLCommentRegexp  = %r{ <! ( -- .*? -- \s* )+ > }mx
	XMLProcInstRegexp  = %r{ <\? .*? \?> }mx
	MetaTag = Regexp::union( HTMLCommentRegexp, XMLProcInstRegexp )

	HTMLTagOpenRegexp  = %r{ < [a-z/!$] [^<>]* }mx
	HTMLTagCloseRegexp = %r{ > }x
	HTMLTagPart = Regexp::union( HTMLTagOpenRegexp, HTMLTagCloseRegexp )

	### Break the HTML source in +str+ into a series of tokens and return
	### them. The tokens are just 2-element Array tuples with a type and the
	### actual content. If this function is called with a block, the type and
	### text parts of each token will be yielded to it one at a time as they are
	### extracted.
	def tokenize_html( str )
		depth = 0
		tokens = []
		@scanner.string = str.dup
		type, token = nil, nil

		until @scanner.empty?
			@log.debug "Scanning from %p" % @scanner.rest

			# Match comments and PIs without nesting
			if (( token = @scanner.scan(MetaTag) ))
				type = :tag

			# Do nested matching for HTML tags
			elsif (( token = @scanner.scan(HTMLTagOpenRegexp) ))
				tagstart = @scanner.pos
				@log.debug " Found the start of a plain tag at %d" % tagstart

				# Start the token with the opening angle
				depth = 1
				type = :tag

				# Scan the rest of the tag, allowing unlimited nested <>s. If
				# the scanner runs out of text before the tag is closed, raise
				# an error.
				while depth.nonzero?

					# Scan either an opener or a closer
					chunk = @scanner.scan( HTMLTagPart ) or
						raise "Malformed tag at character %d: %p" % 
							[ tagstart, token + @scanner.rest ]
						
					@log.debug "  Found another part of the tag at depth %d: %p" %
						[ depth, chunk ]

					token += chunk

					# If the last character of the token so far is a closing
					# angle bracket, decrement the depth. Otherwise increment
					# it for a nested tag.
					depth += ( token[-1, 1] == '>' ? -1 : 1 )
					@log.debug "  Depth is now #{depth}"
				end

			# Match text segments
			else
				@log.debug " Looking for a chunk of text"
				type = :text

				# Scan forward, always matching at least one character to move
				# the pointer beyond any non-tag '<'.
				token = @scanner.scan_until( /[^<]+/m )
			end

			@log.debug " type: %p, token: %p" % [ type, token ]

			# If a block is given, feed it one token at a time. Add the token to
			# the token list to be returned regardless.
			if block_given?
				yield( type, token )
			end
			tokens << [ type, token ]
		end

		return tokens
	end


	### Return a copy of +str+ with angle brackets and ampersands HTML-encoded.
	def encode_html( str )
		str.gsub( /&(?!#?[x]?(?:[0-9a-f]+|\w{1,8});)/i, "&amp;" ).
			gsub( %r{<(?![a-z/?\$!])}i, "&lt;" )
	end

	
	### Return one level of line-leading tabs or spaces from a copy of +str+ and
	### return it.
	def outdent( str )
		str.gsub( /^(\t|[ ]{1,#{TabWidth}})/, '')
	end
	
end # class BlueCloth


#                                vim:ts=4:sw=4:
# = RedCloth - Textile for Ruby
#
# Homepage::  http://whytheluckystiff.net/ruby/redcloth/
# Author::    why the lucky stiff (http://whytheluckystiff.net/)
# Copyright:: (c) 2004 why the lucky stiff (and his puppet organizations.)
# License::   BSD
#
# (see http://hobix.com/textile/ for a Textile Reference.)
#
# Based on (and also inspired by) both:
#
# PyTextile: http://diveintomark.org/projects/textile/textile.py.txt
# Textism for PHP: http://www.textism.com/tools/textile/
# 
#

class String
    #
    # Flexible HTML escaping
    #
    def htmlesc!( mode )
        gsub!( '&', '&amp;' )
        gsub!( '"', '&quot;' ) if mode != :NoQuotes
        gsub!( "'", '&#039;' ) if mode == :Quotes
        gsub!('<', '&lt;')
        gsub!('>', '&gt;')
    end
end

# = RedCloth
#
# RedCloth is a Ruby library for converting Textile
# into HTML.
#
# == What is Textile?
#
# Textile is a simple formatting style for text
# documents, loosely based on some HTML conventions.
#
# == Sample Textile Text
#
#  h2. This is a title
#
#  h3. This is a subhead
#
#  This is a bit of paragraph.
#
#  bq. This is a blockquote.
#
# = Writing Textile
#
# A Textile document consists of paragraphs.  Paragraphs
# can be specially formatted by adding a small instruction
# to the beginning of the paragraph.
#
#  h[n].   Header of size [n].
#  bq.     Blockquote.
#  #       Numeric list.
#  *       Bulleted list.
#
# == Quick Phrase Modifiers
#
# Quick phrase modifiers are also included, to allow formatting
# of small portions of text within a paragraph.
#
#  \_emphasis\_
#  \_\_italicized\_\_
#  \*strong\*
#  \*\*bold\*\*
#  ??citation??
#  -deleted text-
#  +inserted text+
#  ^superscript^
#  ~subscript~
#  @code@
#  %(classname)span%
#
#  ==notextile== (leave text alone)
#
# == Links
#
# To make a hypertext link, put the link text in "quotation 
# marks" followed immediately by a colon and the URL of the link.
# 
# Optional: text in (parentheses) following the link text, 
# but before the closing quotation mark, will become a Title 
# attribute for the link, visible as a tool tip when a cursor is above it.
# 
# Example:
#
#  "This is a link (This is a title) ":http://www.textism.com
# 
# Will become:
# 
#  <a href="http://www.textism.com" title="This is a title">This is a link</a>
#
# == Images
#
# To insert an image, put the URL for the image inside exclamation marks.
#
# Optional: text that immediately follows the URL in (parentheses) will 
# be used as the Alt text for the image. Images on the web should always 
# have descriptive Alt text for the benefit of readers using non-graphical 
# browsers.
#
# Optional: place a colon followed by a URL immediately after the 
# closing ! to make the image into a link.
# 
# Example:
#
#  !http://www.textism.com/common/textist.gif(Textist)!
#
# Will become:
#
#  <img src="http://www.textism.com/common/textist.gif" alt="Textist" />
#
# With a link:
#
#  !/common/textist.gif(Textist)!:http://textism.com
#
# Will become:
#
#  <a href="http://textism.com"><img src="/common/textist.gif" alt="Textist" /></a>
#
# == Defining Acronyms
#
# HTML allows authors to define acronyms via the tag. The definition appears as a 
# tool tip when a cursor hovers over the acronym. A crucial aid to clear writing, 
# this should be used at least once for each acronym in documents where they appear.
#
# To quickly define an acronym in Textile, place the full text in (parentheses) 
# immediately following the acronym.
# 
# Example:
#
#  ACLU(American Civil Liberties Union)
#
# Will become:
#
#  <acronym title="American Civil Liberties Union">ACLU</acronym>
#
# == Adding Tables
#
# In Textile, simple tables can be added by seperating each column by
# a pipe.
#
#     |a|simple|table|row|
#     |And|Another|table|row|
#
# Attributes are defined by style definitions in parentheses.
#
#     table(border:1px solid black).
#     (background:#ddd;color:red). |{}| | | |
#
# == Using RedCloth
# 
# RedCloth is simply an extension of the String class, which can handle
# Textile formatting.  Use it like a String and output HTML with its
# RedCloth#to_html method.
#
#  doc = RedCloth.new "
#
#  h2. Test document
#
#  Just a simple test."
#
#  puts doc.to_html

class RedCloth < String

    VERSION = '2.0.11'

    #
    # Two accessor for setting security restrictions.
    #
    # This is a nice thing if you're using RedCloth for
    # formatting in public places (e.g. Wikis) where you
    # don't want users to abuse HTML for bad things.
    #
    # If +:filter_html+ is set, HTML which wasn't
    # created by the Textile processor will be escaped.
    #
    # If +:filter_styles+ is set, it will also disable
    # the style markup specifier. ('{color: red}')
    #
    attr_accessor :filter_html, :filter_styles

    #
    # Accessor for toggling line folding.
    #
    # If +:fold_lines+ is set, single newlines will
    # not be converted to break tags.
    #
    attr_accessor :fold_lines

    #
    # Returns a new RedCloth object, based on _string_ and
    # enforcing all the included _restrictions_.
    #
    #   r = RedCloth.new( "h1. A <b>bold</b> man", [:filter_html] )
    #   r.to_html
    #     #=>"<h1>A &lt;b&gt;bold&lt;/b&gt; man</h1>"
    #
    def initialize( string, restrictions = [] )
        @lite = false
        restrictions.each { |r| method( "#{ r }=" ).call( true ) }
        super( string )
    end

    #
    # Generates HTML from the Textile contents.  The _lite_ flag
    # may be used to honor only inline markup, ignoring lists, tables,
    # and block formatting.
    #
    #   r = RedCloth.new( "And then? She *fell*!" )
    #   r.to_html( true )
    #     #=>"And then? She <strong>fell</strong>!"
    #
    def to_html( lite = nil )

        @lite = lite unless lite.nil?

        # make our working copy
        text = self.dup
        
        @urlrefs = {}
        @shelf = []

        incoming_entities text 
        ## encode_entities text 
        ## fix_entities text 
        clean_white_space text 

        get_refs text 

        no_textile text 

        inline text

        unless @lite
            fold text
            block text
        end

        retrieve text

        text.gsub!( /<\/?notextile>/, '' )
        text.gsub!( /x%x%/, '&#38;' )
        text.gsub!( /<br \/>/, "<br />\n" )
        text.strip!
        text

    end

    #######
    private
    #######
    #
    # Mapping of 8-bit ASCII codes to HTML numerical entity equivalents.
    # (from PyTextile)
    #
    TEXTILE_TAGS =

        [[128, 8364], [129, 0], [130, 8218], [131, 402], [132, 8222], [133, 8230], 
         [134, 8224], [135, 8225], [136, 710], [137, 8240], [138, 352], [139, 8249], 
         [140, 338], [141, 0], [142, 0], [143, 0], [144, 0], [145, 8216], [146, 8217], 
         [147, 8220], [148, 8221], [149, 8226], [150, 8211], [151, 8212], [152, 732], 
         [153, 8482], [154, 353], [155, 8250], [156, 339], [157, 0], [158, 0], [159, 376]].

        collect! do |a, b|
            [a.chr, ( b.zero? and "" or "&#{ b };" )]
        end

    #
    # Regular expressions to convert to HTML.
    #
    A_HLGN = /(?:(?:<>|<|>|\=|[()]+)+)/
    A_VLGN = /[\-^~]/
    C_CLAS = '(?:\([^)]+\))'
    C_LNGE = '(?:\[[^\]]+\])'
    C_STYL = '(?:\{[^}]+\})'
    S_CSPN = '(?:\\\\\d+)'
    S_RSPN = '(?:/\d+)'
    A = "(?:#{A_HLGN}?#{A_VLGN}?|#{A_VLGN}?#{A_HLGN}?)"
    S = "(?:#{S_CSPN}?#{S_RSPN}|#{S_RSPN}?#{S_CSPN}?)"
    C = "(?:#{C_CLAS}?#{C_STYL}?#{C_LNGE}?|#{C_STYL}?#{C_LNGE}?#{C_CLAS}?|#{C_LNGE}?#{C_STYL}?#{C_CLAS}?)"
    # PUNCT = Regexp::quote( '!"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~' )
    PUNCT = Regexp::quote( '!"#$%&\'*+,-./:;=?@\\^_`|~' )
    HYPERLINK = '(\S+?)([^\w\s/;=\?]*?)(\s|$)'

    GLYPHS = [
    #   [ /([^\s\[{(>])?\'([dmst]\b|ll\b|ve\b|\s|:|$)/, '\1&#8217;\2' ], # single closing
        [ /([^\s\[{(>])\'/, '\1&#8217;' ], # single closing
        [ /\'(?=\s|s\b|[#{PUNCT}])/, '&#8217;' ], # single closing
        [ /\'/, '&#8216;' ], # single opening
    #   [ /([^\s\[{(])?"(\s|:|$)/, '\1&#8221;\2' ], # double closing
        [ /([^\s\[{(>])"/, '\1&#8221;' ], # double closing
        [ /"(?=\s|[#{PUNCT}])/, '&#8221;' ], # double closing
        [ /"/, '&#8220;' ], # double opening
        [ /\b( )?\.{3}/, '\1&#8230;' ], # ellipsis
        [ /\b([A-Z][A-Z0-9]{2,})\b(?:[(]([^)]*)[)])/, '<acronym title="\2">\1</acronym>' ], # 3+ uppercase acronym
        [ /(^|[^"][>\s])([A-Z][A-Z0-9 ]{2,})([^<a-z0-9]|$)/, '\1<span class="caps">\2</span>\3' ], # 3+ uppercase caps
        [ /(\.\s)?\s?--\s?/, '\1&#8212;' ], # em dash
        [ /\s->\s/, ' &rarr; ' ], # en dash
        [ /\s-\s/, ' &#8211; ' ], # en dash
        [ /(\d+) ?x ?(\d+)/, '\1&#215;\2' ], # dimension sign
        [ /\b ?[(\[]TM[\])]/i, '&#8482;' ], # trademark
        [ /\b ?[(\[]R[\])]/i, '&#174;' ], # registered
        [ /\b ?[(\[]C[\])]/i, '&#169;' ] # copyright
    ]

    I_ALGN_VALS = {
        '<' => 'left',
        '=' => 'center',
        '>' => 'right'
    }

    H_ALGN_VALS = {
        '<' => 'left',
        '=' => 'center',
        '>' => 'right',
        '<>' => 'justify'
    }

    V_ALGN_VALS = {
        '^' => 'top',
        '-' => 'middle',
        '~' => 'bottom'
    }

    QTAGS = [
        ['**', 'b'],
        ['*', 'strong'],
        ['??', 'cite'],
        ['-', 'del'],
        ['__', 'i'],
        ['_', 'em'],
        ['%', 'span'],
        ['+', 'ins'],
        ['^', 'sup'],
        ['~', 'sub']
    ].collect do |rc, ht| 
        ttr = Regexp.quote(rc)
        punct = PUNCT.sub( Regexp::quote(rc), '' )
        re  = /(^|[\s\>#{punct}{(\[])
                #{ttr}
                (#{C})
                (?::(\S+?))?
                ([^\s#{ttr}]+?(?:[^\n]|\n(?!\n))*?)
                ([#{punct}]*?)
                #{ttr}
                (?=[\s\])}<#{punct}]|$)/xm 
        [re, ht]
    end 

    def pgl( text )
        GLYPHS.each do |re, resub|
            text.gsub! re, resub
        end
    end

    def pba( text_in, element = "" )
        
        return '' unless text_in

        style = []
        text = text_in.dup
        if element == 'td'
            colspan = $1 if text =~ /\\(\d+)/
            rowspan = $1 if text =~ /\/(\d+)/
            style << "vertical-align:#{ v_align( $& ) };" if text =~ A_VLGN
        end

        style << "#{ $1 };" if not @filter_styles and
            text.sub!( /\{([^}]*)\}/, '' )

        lang = $1 if
            text.sub!( /\[([^)]+?)\]/, '' )

        cls = $1 if
            text.sub!( /\(([^()]+?)\)/, '' )
                        
        style << "padding-left:#{ $1.length }em;" if
            text.sub!( /([(]+)/, '' )

        style << "padding-right:#{ $1.length }em;" if text.sub!( /([)]+)/, '' )

        style << "text-align:#{ h_align( $& ) };" if text =~ A_HLGN

        cls, id = $1, $2 if cls =~ /^(.*?)#(.*)$/
        
        atts = ''
        atts << " style=\"#{ style.join }\"" unless style.empty?
        atts << " class=\"#{ cls }\"" unless cls.to_s.empty?
        atts << " lang=\"#{ lang }\"" if lang
        atts << " id=\"#{ id }\"" if id
        atts << " colspan=\"#{ colspan }\"" if colspan
        atts << " rowspan=\"#{ rowspan }\"" if rowspan
        
        atts
    end

    TABLE_RE = /^(?:table(_?#{S}#{A}#{C})\. ?\n)?^(#{A}#{C}\.? ?\|.*?\|)(\n\n|\Z)/m
    
    def table( text ) 
        text.gsub!( TABLE_RE  ) do |matches|

            tatts, fullrow = $~[1..2]
            tatts = pba( tatts, 'table' )
            rows = []

            fullrow.
            split( /\|$/m ).
            delete_if { |x| x.empty? }.
            each do |row|

                ratts, row = pba( $1, 'tr' ), $2 if row =~ /^(#{A}#{C}\. )(.*)/m
                
                cells = []
                row.split( '|' ).each do |cell|
                    ctyp = 'd'
                    ctyp = 'h' if cell =~ /^_/

                    catts = ''
                    catts, cell = pba( $1, 'td' ), $2 if cell =~ /^(_?#{S}#{A}#{C}\. )(.*)/

                    unless cell.strip.empty?
                        cells << "\t\t\t<t#{ ctyp }#{ catts }>#{ cell }</t#{ ctyp }>" 
                    end
                end
                rows << "\t\t<tr#{ ratts }>\n#{ cells.join( "\n" ) }\n\t\t</tr>"
            end
            "\t<table#{ tatts }>\n#{ rows.join( "\n" ) }\n\t</table>\n\n"
        end
    end

    LISTS_RE = /^([#*]+?#{C} .*?)$(?![^#*])/m
    LISTS_CONTENT_RE = /^([#*]+)(#{A}#{C}) (.*)$/m

    def lists( text ) 
        text.gsub!( LISTS_RE ) do |match|
            lines = match.split( /\n/ )
            last_line = -1
            depth = []
            lines.each_with_index do |line, line_id|
                if line =~ LISTS_CONTENT_RE 
                    tl,atts,content = $~[1..3]
                    if depth.last
                        if depth.last.length > tl.length
                            (depth.length - 1).downto(0) do |i|
                                break if depth[i].length == tl.length
                                lines[line_id - 1] << "</li>\n\t</#{ lT( depth[i] ) }l>\n\t"
                                depth.pop
                            end
                        end
                        if depth.last.length == tl.length
                            lines[line_id - 1] << '</li>'
                        end
                    end
                    unless depth.last == tl
                        depth << tl
                        atts = pba( atts )
                        lines[line_id] = "\t<#{ lT(tl) }l#{ atts }>\n\t<li>#{ content }"
                    else
                        lines[line_id] = "\t\t<li>#{ content }"
                    end
                    last_line = line_id

                elsif line =~ /^\s+\S/
                    last_line = line_id
                elsif line_id - last_line < 2 and line =~ /^\S/
                    last_line = line_id
                end
                if line_id - last_line > 1 or line_id == lines.length - 1
                    depth.delete_if do |v|
                        lines[last_line] << "</li>\n\t</#{ lT( v ) }l>"
                    end
                end
            end
            lines.join( "\n" )
        end
    end

    def lT( text ) 
        text =~ /\#$/ ? 'o' : 'u'
    end

    def fold( text )
        text.gsub!( /(.+)\n(?![#*\s|])/, "\\1#{ @fold_lines ? ' ' : '<br />' }" )
    end

    BLOCK_RE = ['bq','h[1-6]','fn\d+','p'].collect!{|stag|
      [stag,
       /^(#{ stag })(#{A}#{C})\.(?::(\S+))? (.*)$/]
    }

    def block( text ) 
        pre = false
        find = ['bq','h[1-6]','fn\d+','p']
    
        lines = text.split( /\n/ ) + [' '] 
        new_text = 
        lines.collect do |line|
            pre = true if line =~ /<(pre|notextile)[^>]*>/i
            BLOCK_RE.each do |stag, ctag|
                line.gsub!( ctag ) do |m|
                    tag,atts,cite,content = $~[1..4]

                    atts = pba( atts )

                    if tag =~ /fn(\d+)/
                        tag = 'p';
                        atts << " id=\"fn#{ $1 }\""
                        content = "<sup>#{ $1 }</sup> #{ content }"
                    end

                    start = "\t<#{ tag }"
                    tend = "</#{ tag }>"
                    
                    if tag == "bq"
                        cite = check_refs( cite )
                        cite = " cite=\"#{ cite }\"" if cite
                        start = "\t<blockquote#{ cite }>\n\t\t<p";
                        tend = "</p>\n\t</blockquote>";
                    end

                    "#{ start }#{ atts }>#{ content }#{ tend }"
                end unless pre
            end
            
            line.gsub!( /^(?!\t|<\/?div|<\/?pre|<\/?notextile|<\/?code|$| )(.*)/, "\t<p>\\1</p>" ) unless pre
            
            line.gsub!( "<br />", "\n" ) if pre
            pre = false if line =~ /<\/(pre|notextile)>/i
            
            line
        end.join( "\n" )
        text.replace( new_text )
    end
    
    def span( text ) 
      QTAGS.each do |ttr, ht|
        text.gsub!(ttr) do |m|
         
                start,atts,cite,content,tend = $~[1..5]
                atts = pba( atts )
                atts << " cite=\"#{ cite }\"" if cite

                "#{ start }<#{ ht }#{ atts }>#{ content }#{ tend }</#{ ht }>"

            end
        end
    end

    LINK_RE = /
            ([\s\[{(]|[#{PUNCT}])?     # $pre
            "                          # start
            (#{C})                     # $atts
            ([^"]+?)                   # $text
            \s?
            (?:\(([^)]+?)\)(?="))?     # $title
            ":
            (\S+?)                     # $url
            (\/)?                      # $slash
            ([^\w\/;]*?)               # $post
            (?=<|\s|$)
        /x 

    def links( text ) 
        text.gsub!( LINK_RE ) do |m|
            pre,atts,text,title,url,slash,post = $~[1..7]

            url = check_refs( url )
            
            atts = pba( atts )
            atts << " title=\"#{ title }\"" if title
            atts = shelve( atts ) if atts
            
            "#{ pre }<a href=\"#{ url }#{ slash }\"#{ atts }>#{ text }</a>#{ post }"
        end
    end

    REFS_RE =  /(^|\s)\[(.+?)\]((?:http:\/\/|javascript:|ftp:\/\/|\/)\S+?)(?=\s|$)/

    def get_refs( text ) 
        text.gsub!( REFS_RE ) do |m|
            flag, url = $~[2..3]
            @urlrefs[flag] = url
            nil
        end
    end
    
    def check_refs( text ) 
        @urlrefs[text] || text
    end

    IMAGE_RE = /
            \!                   # opening
            (\<|\=|\>)?          # optional alignment atts
            (#{C})               # optional style,class atts
            (?:\. )?             # optional dot-space
            ([^\s(!]+?)          # presume this is the src
            \s?                  # optional space
            (?:\(((?:[^\(\)]|\([^\)]+\))+?)\))?   # optional title
            \!                   # closing
            (?::#{ HYPERLINK })? # optional href
        /x 

    def image( text ) 
        text.gsub!( IMAGE_RE )  do |m|
            algn,atts,url,title,href,href_a1,href_a2 = $~[1..7]
            atts = pba( atts )
            atts << " align=\"#{ i_align( algn ) }\"" if algn
            atts << " title=\"#{ title }\"" if title
            atts << " alt=\"#{ title }\"" 
            # size = @getimagesize($url);
            # if($size) $atts.= " $size[3]";

            href = check_refs( href ) if href
            url = check_refs( url )
            atts << " border=\"0\"" if href

            out = ''
            out << "<a href=\"#{ href }\">" if href
            out << "<img src=\"#{ url }\"#{ atts } />"
            out << "</a>#{ href_a1 }#{ href_a2 }" if href
            
            out
        end
    end

    CODE_RE = /
            (^|[\s>#{PUNCT}{(\[])            # 1 open bracket?
            @                                # opening
            (?:\|(\w+?)\|)?                  # 2 language
            (\S(?:[^\n]|\n(?!\n))*?)         # 3 code
            @                                # closing
            (?=[\s\]}\)<#{PUNCT}]|$)         # 4 closing bracket?
        /x 

    def code( text ) 
        text.gsub!( CODE_RE ) do |m|
            before,lang,code,after = $~[1..4]
            lang = " language=\"#{ lang }\"" if lang
            "#{ before }<code#{ lang }>#{ code }</code>#{ after }"
        end
    end

    def shelve( val ) 
        @shelf << val
        " <#{ @shelf.length }>"
    end
    
    def retrieve( text ) 
        @shelf.each_with_index do |r, i|
            text.gsub!( " <#{ i + 1 }>", r )
        end
    end

    def incoming_entities( text ) 
        ## turn any incoming ampersands into a dummy character for now.
        ## This uses a negative lookahead for alphanumerics followed by a semicolon,
        ## implying an incoming html entity, to be skipped

        text.gsub!( /&(?![#a-z0-9]+;)/i, "x%x%" )
    end

    def encode_entities( text ) 
        ## Convert high and low ascii to entities.
        #  if $-K == "UTF-8"  
        #      encode_high( text )
        #  else
            text.htmlesc!( :NoQuotes )
        #  end
    end

    def fix_entities( text )
        ## de-entify any remaining angle brackets or ampersands
        text.gsub!( "&gt;", ">" )
        text.gsub!( "&lt;", "<" )
        text.gsub!( "&amp;", "&" )
    end

    def clean_white_space( text ) 
        text.gsub!( /\r\n/, "\n" )
        text.gsub!( /\t/, '' )
        text.gsub!( /\n{3,}/, "\n\n" )
        text.gsub!( /\n *\n/, "\n\n" )
        text.gsub!( /"$/, "\" " )
    end

    def no_textile( text ) 
        text.gsub!( /(^|\s)==(.*?)==(\s|$)?/,
            '\1<notextile>\2</notextile>\3' )
    end

    def footnote_ref( text ) 
        text.gsub!( /\b\[([0-9]+?)\](\s)?/,
            '<sup><a href="#fn\1">\1</a></sup>\2' )
    end
    
    OFFTAGS = /(code|pre|kbd|notextile)/
    OFFTAG_MATCH = /(?:(<\/#{ OFFTAGS }>)|(<#{ OFFTAGS }[^>]*>))(.*?)(?=<\/?#{ OFFTAGS }>|\Z)/mi
    OFFTAG_OPEN = /<#{ OFFTAGS }/
    OFFTAG_CLOSE = /<\/?#{ OFFTAGS }/
    HASTAG_MATCH = /(<\/?\w[^\n]*?>)/m
    ALLTAG_MATCH = /(<\/?\w[^\n]*?>)|.*?(?=<\/?\w[^\n]*?>|$)/m

    def glyphs( text, level = 0 )
        if text !~ HASTAG_MATCH
            pgl text
            footnote_ref text
        else
            codepre = 0
            text.gsub!( ALLTAG_MATCH ) do |line|
                ## matches are off if we're between <code>, <pre> etc.
                if $1
                    if @filter_html
                        line.htmlesc!( :NoQuotes )
                    elsif line =~ OFFTAG_OPEN
                        codepre += 1
                    elsif line =~ OFFTAG_CLOSE
                        codepre -= 1
                        codepre = 0 if codepre < 0
                    end 
                ## do htmlspecial if between <code>
                elsif codepre.zero?
                    glyphs( line, level + 1 )
                else
                    line.htmlesc!( :NoQuotes )
                end
                ## p [level, codepre, orig_line, line]

                line
            end
        end
    end

    def rip_offtags( text )
        pre_list = []
        if text =~ /<.*>/
            ## strip and encode <pre> content
            codepre, used_offtags = 0, {}
            text.gsub!( OFFTAG_MATCH ) do |line|
                if $3
                    offtag, aftertag = $4, $5
                    codepre += 1
                    used_offtags[offtag] = true
                    if codepre - used_offtags.length > 0
                        line.htmlesc!( :NoQuotes ) 
                        pre_list.last << line
                        line = ""
                    else
                        aftertag.htmlesc!( :NoQuotes ) if aftertag
                        line = "<redpre##{ pre_list.length }>"
                        pre_list << "#{ $3 }#{ aftertag }"
                    end
                elsif $1 and codepre > 0
                    if codepre - used_offtags.length > 0
                        line.htmlesc!( :NoQuotes ) 
                        pre_list.last << line
                        line = ""
                    end
                    codepre -= 1 unless codepre.zero?
                    used_offtags = {} if codepre.zero?
                end 
                line
            end
        end
        pre_list
    end

    def smooth_offtags( text, pre_list )
        unless pre_list.empty?
            ## replace <pre> content
            text.gsub!( /<redpre#(\d+)>/ ) { pre_list[$1.to_i] }
        end
    end

    def inline( text ) 
        text.gsub!( /"\z/, "\" " )
        pre_list = rip_offtags text

        ## apply inline markup
        unless @lite
            lists text
            table text
        end

        image text 
        links text 
        code text 
        span text

        ## replace entities
        glyphs text
        smooth_offtags text, pre_list
    end

    def i_align( text )
        I_ALGN_VALS[text]
    end

    def h_align( text ) 
        H_ALGN_VALS[text]
    end

    def v_align( text ) 
        V_ALGN_VALS[text]
    end

    def encode_high( text )
        ## mb_encode_numericentity($text, $cmap, $charset);
    end

    def decode_high( text )
        ## mb_decode_numericentity($text, $cmap, $charset);
    end

    def textile_popup_help( name, helpvar, windowW, windowH )
        ' <a target="_blank" href="http://www.textpattern.com/help/?item=' + helpvar + '" onclick="window.open(this.href, \'popupwindow\', \'width=' + windowW + ',height=' + windowH + ',scrollbars,resizable\'); return false;">' + name + '</a><br />'
    end

    CMAP = [
         160,  255,  0, 0xffff,
         402,  402,  0, 0xffff,
         913,  929,  0, 0xffff,
         931,  937,  0, 0xffff,
         945,  969,  0, 0xffff,
         977,  978,  0, 0xffff, 
         982,  982,  0, 0xffff,
         8226, 8226, 0, 0xffff,
         8230, 8230, 0, 0xffff,
         8242, 8243, 0, 0xffff,
         8254, 8254, 0, 0xffff,
         8260, 8260, 0, 0xffff,
         8465, 8465, 0, 0xffff,
         8472, 8472, 0, 0xffff,
         8476, 8476, 0, 0xffff,
         8482, 8482, 0, 0xffff,
         8501, 8501, 0, 0xffff,
         8592, 8596, 0, 0xffff,
         8629, 8629, 0, 0xffff,
         8656, 8660, 0, 0xffff,
         8704, 8704, 0, 0xffff,
         8706, 8707, 0, 0xffff,
         8709, 8709, 0, 0xffff,
         8711, 8713, 0, 0xffff,
         8715, 8715, 0, 0xffff,
         8719, 8719, 0, 0xffff,
         8721, 8722, 0, 0xffff,
         8727, 8727, 0, 0xffff,
         8730, 8730, 0, 0xffff,
         8733, 8734, 0, 0xffff,
         8736, 8736, 0, 0xffff,
         8743, 8747, 0, 0xffff,
         8756, 8756, 0, 0xffff,
         8764, 8764, 0, 0xffff,
         8773, 8773, 0, 0xffff,
         8776, 8776, 0, 0xffff,
         8800, 8801, 0, 0xffff,
         8804, 8805, 0, 0xffff,
         8834, 8836, 0, 0xffff,
         8838, 8839, 0, 0xffff,
         8853, 8853, 0, 0xffff,
         8855, 8855, 0, 0xffff,
         8869, 8869, 0, 0xffff,
         8901, 8901, 0, 0xffff,
         8968, 8971, 0, 0xffff,
         9001, 9002, 0, 0xffff,
         9674, 9674, 0, 0xffff,
         9824, 9824, 0, 0xffff,
         9827, 9827, 0, 0xffff,
         9829, 9830, 0, 0xffff,
         338,  339,  0, 0xffff,
         352,  353,  0, 0xffff,
         376,  376,  0, 0xffff, 
         710,  710,  0, 0xffff,
         732,  732,  0, 0xffff,
         8194, 8195, 0, 0xffff,
         8201, 8201, 0, 0xffff,
         8204, 8207, 0, 0xffff,
         8211, 8212, 0, 0xffff,
         8216, 8218, 0, 0xffff,
         8218, 8218, 0, 0xffff,
         8220, 8222, 0, 0xffff,
         8224, 8225, 0, 0xffff,
         8240, 8240, 0, 0xffff,
         8249, 8250, 0, 0xffff,
         8364, 8364, 0, 0xffff
     ]
end


require 'wiki_words'
require 'chunks/chunk'
require 'chunks/wiki'
require 'cgi'

# Contains all the methods for finding and replacing wiki related links.
module WikiChunk
  include Chunk

  # A wiki link is the top-level class for anything that refers to
  # another wiki page.
  class WikiLink < Chunk::Abstract

    attr_reader :page_name, :link_text, :link_type

    def initialize(*args)
      super
      @link_type = 'show'
    end

    def self.apply_to(content)
      content.gsub!( self.pattern ) do |matched_text|
        chunk = self.new($~)
        if chunk.textile_url?
          # do not substitute
          matched_text
        else
          content.chunks << chunk
          chunk.mask(content)
        end
      end
    end

    def textile_url?
      not @textile_link_suffix.nil?
    end

    # By default, no escaped text
    def escaped_text() nil end

    # Replace link with a mask, but if the word is escaped, then don't replace it
    def mask(content) 
      escaped_text || super(content)
    end

    def revert(content) content.sub!(mask(content), text) end
    
    # Do not keep this chunk if it is escaped.
    # Otherwise, pass the link procedure a page_name and link_text and
    # get back a string of HTML to replace the mask with.
    def unmask(content)
      if escaped_text 
        return self
      else 
        chunk_found = content.sub!(mask(content)) do |match| 
          content.page_link(page_name, link_text, link_type)
        end
        if chunk_found
          return self
        else
          return nil
        end
      end
    end

  end

  # This chunk matches a WikiWord. WikiWords can be escaped
  # by prepending a '\'. When this is the case, the +escaped_text+
  # method will return the WikiWord instead of the usual +nil+.
  # The +page_name+ method returns the matched WikiWord.
  class Word < WikiLink
    unless defined? WIKI_WORD
      WIKI_WORD = Regexp.new('(":)?(\\\\)?(' + WikiWords::WIKI_WORD_PATTERN + ')\b', 0, "utf-8")
    end

    def self.pattern
      WIKI_WORD
    end

    def initialize(match_data)
      super(match_data)
      @textile_link_suffix, @escape, @page_name = match_data[1..3]
    end

    def escaped_text
      page_name unless @escape.nil?
    end
    def link_text() WikiWords.separate(page_name) end	
  end

  # This chunk handles [[bracketted wiki words]] and 
  # [[AliasedWords|aliased wiki words]]. The first part of an
  # aliased wiki word must be a WikiWord. If the WikiWord
  # is aliased, the +link_text+ field will contain the
  # alias, otherwise +link_text+ will contain the entire
  # contents within the double brackets.
  #
  # NOTE: This chunk must be tested before WikiWord since
  #       a WikiWords can be a substring of a WikiLink. 
  class Link < WikiLink
    
    unless defined? WIKI_LINK
      WIKI_LINK = /(":)?\[\[([^\]]+)\]\]/
      LINK_TYPE_SEPARATION = Regexp.new('^(.+):((file)|(pic))$', 0, 'utf-8')
      ALIAS_SEPARATION = Regexp.new('^(.+)\|(.+)$', 0, 'utf-8')
    end    
        
    def self.pattern() WIKI_LINK end

    def initialize(match_data)
      super(match_data)
      @textile_link_suffix, @page_name = match_data[1..2]
      @link_text = @page_name
      separate_link_type
      separate_alias
    end

    private

    # if link wihin the brackets has a form of [[filename:file]] or [[filename:pic]], 
    # this means a link to a picture or a file
    def separate_link_type
      link_type_match = LINK_TYPE_SEPARATION.match(@page_name)
      if link_type_match
        @link_text = @page_name = link_type_match[1]
        @link_type = link_type_match[2..3].compact[0]
      end
    end

    # link text may be different from page name. this will look like [[actual page|link text]]
    def separate_alias
      alias_match = ALIAS_SEPARATION.match(@page_name)
      if alias_match
        @page_name, @link_text = alias_match[1..2]
      end
      # note that [[filename|link text:file]] is also supported
    end  
  
  end
  
end

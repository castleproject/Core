require 'cgi'
require 'chunks/engines'
require 'chunks/category'
require 'chunks/include'
require 'chunks/wiki'
require 'chunks/literal'
require 'chunks/uri'
require 'chunks/nowiki'

# Wiki content is just a string that can process itself with a chain of
# actions. The actions can modify wiki content so that certain parts of
# it are protected from being rendered by later actions.
#
# When wiki content is rendered, it can be interrogated to find out 
# which chunks were rendered. This means things like categories, wiki 
# links, can be determined.
#
# Exactly how wiki content is rendered is determined by a number of 
# settings that are optionally passed in to a constructor. The current
# options are:
#  * :engine 
#    => The structural markup engine to use (Textile, Markdown, RDoc)
#  * :engine_opts
#    => A list of options to pass to the markup engines (safe modes, etc)
#  * :pre_engine_actions
#    => A list of render actions or chunks to be processed before the
#       markup engine is applied. By default this is:    
#       Category, Include, URIChunk, WikiChunk::Link, WikiChunk::Word        
#  * :post_engine_actions
#    => A list of render actions or chunks to apply after the markup 
#       engine. By default these are:    
#       Literal::Pre, Literal::Tags
#  * :mode
#    => How should the content be rendered? For normal display (:display), 
#       publishing (:publish) or export (:export)?
#
# AUTHOR: Mark Reid <mark @ threewordslong . com>
# CREATED: 15th May 2004
# UPDATED: 22nd May 2004
class WikiContent < String

  PRE_ENGINE_ACTIONS  = [ NoWiki, Category, Include, WikiChunk::Link, URIChunk, LocalURIChunk, 
                          WikiChunk::Word ] 
  POST_ENGINE_ACTIONS = [ Literal::Pre, Literal::Tags ]
  DEFAULT_OPTS = {
    :pre_engine_actions  => PRE_ENGINE_ACTIONS,
    :post_engine_actions => POST_ENGINE_ACTIONS,
    :engine              => Engines::Textile,
    :engine_opts         => [],
    :mode                => [:display]
  }

  attr_reader :web, :options, :rendered, :chunks

  # Create a new wiki content string from the given one.
  # The options are explained at the top of this file.
  def initialize(revision, options = {})
    @revision = revision
    @web = @revision.page.web

    # Deep copy of DEFAULT_OPTS to ensure that changes to PRE/POST_ENGINE_ACTIONS stay local
    @options = Marshal.load(Marshal.dump(DEFAULT_OPTS)).update(options)
    @options[:engine] = Engines::MAP[@web.markup] || Engines::Textile
    @options[:engine_opts] = (@web.safe_mode ? [:filter_html, :filter_styles] : [])

    @options[:pre_engine_actions].delete(WikiChunk::Word) if @web.brackets_only

    super(@revision.content)
    
    begin
      render!(@options[:pre_engine_actions] + [@options[:engine]] + @options[:post_engine_actions])
# FIXME this is where all the parsing problems were shoved under the carpet
#   rescue => e
#     @rendered = e.message
    end
  end

  # Call @web.page_link using current options.
  def page_link(name, text, link_type)
    @options[:link_type] = link_type || :show
    @web.make_link(name, text, @options)
  end

  # Find all the chunks of the given types
  def find_chunks(chunk_type)
    rendered.select { |chunk| chunk.kind_of?(chunk_type) }
  end

  # Render this content using the specified actions.
  def render!(chunk_types)
    @chunks = []
    chunk_types.each { |chunk_type| chunk_type.apply_to(self) }
    @rendered = @chunks.map { |chunk| chunk.unmask(self) }.compact
    (@chunks - @rendered).each { |chunk| chunk.revert(self) }
  end
  
end
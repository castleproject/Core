$: << File.dirname(__FILE__) + "../../libraries"

require 'redcloth'
require 'bluecloth'
require 'rdocsupport'
require 'chunks/chunk'

# The markup engines are Chunks that call the one of RedCloth, BlueCloth
# or RDoc to convert text. This markup occurs when the chunk is required
# to mask itself.
module Engines
  class Textile < Chunk::Abstract
    def self.pattern() /^(.*)$/m end
    def mask(content)
      RedCloth.new(text,content.options[:engine_opts]).to_html
    end
    def unmask(content) self end
  end

  class Markdown < Chunk::Abstract
    def self.pattern() /^(.*)$/m end
    def mask(content)
      BlueCloth.new(text,content.options[:engine_opts]).to_html
    end
    def unmask(content) self end
  end

  class RDoc < Chunk::Abstract
    def self.pattern() /^(.*)$/m end
    def mask(content)
      RDocSupport::RDocFormatter.new(text).to_html
    end
    def unmask(content) self end
  end

  MAP = { :textile => Textile, :markdown => Markdown, :rdoc => RDoc }
end


require 'chunks/chunk'

# These are basic chunks that have a pattern and can be protected.
# They are used by rendering process to prevent wiki rendering
# occuring within literal areas such as <code> and <pre> blocks
# and within HTML tags.
module Literal
  # A literal chunk that protects 'code' and 'pre' tags from wiki rendering.
  class Pre < Chunk::Abstract
    PRE_BLOCKS = "a|pre|code"
    def self.pattern() Regexp.new('<('+PRE_BLOCKS+')\b[^>]*?>.*?</\1>', Regexp::MULTILINE) end
  end 

  # A literal chunk that protects HTML tags from wiki rendering.
  class Tags < Chunk::Abstract
    TAGS = "a|img|em|strong|div|span|table|td|th|ul|ol|li|dl|dt|dd"
    def self.pattern() Regexp.new('<(?:'+TAGS+')[^>]*?>', Regexp::MULTILINE) end
  end
end

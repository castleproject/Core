require 'digest/md5'
require 'uri/common'

# A chunk is a pattern of text that can be protected
# and interrogated by a renderer. Each Chunk class has a 
# +pattern+ that states what sort of text it matches.
# Chunks are initalized by passing in the result of a
# match by its pattern. 
module Chunk
  class Abstract
	attr_reader :text

	def initialize(match_data) @text = match_data[0] end
	
    # Find all the chunks of the given type in content
    # Each time the pattern is matched, create a new
    # chunk for it, and replace the occurance of the chunk
    # in this content with its mask.
	def self.apply_to(content)
	  content.gsub!( self.pattern ) do |match|	
	    new_chunk = self.new($~)
        content.chunks << new_chunk
        new_chunk.mask(content)
      end
    end

	def mask(content) 
	  "chunk#{self.object_id}#{self.class.to_s.delete(':').downcase}chunk"
	end

	def revert(content) 
	  content.sub!( Regexp.new(mask(content)), text )
	end

	def unmask(content) 
	  self if revert(content) 
	end

  end
end

require 'chunks/chunk'

# This chunks allows certain parts of a wiki page to be hidden from the
# rest of the rendering pipeline. It should be run at the beginning
# of the pipeline in `wiki_content.rb`.
#
# An example use of this chunk is to markup double brackets or
# auto URI links:
#  <nowiki>Here are [[double brackets]] and a URI: www.uri.org</nowiki>
#
# The contents of the chunks will not be processed by any other chunk
# so the `www.uri.org` and the double brackets will appear verbatim.
#
# Author: Mark Reid <mark at threewordslong dot com>
# Created: 8th June 2004
class NoWiki < Chunk::Abstract

  def self.pattern() Regexp.new('<nowiki>(.*?)</nowiki>') end

  attr_reader :plain_text

  def initialize(match_data)
	super(match_data)
	@plain_text = match_data[1]
  end
  
  # The nowiki content is not unmasked. This means the chunk will be reverted
  # using the plain text.
  def unmask(content) nil end
  def revert(content) content.sub!(mask(content), plain_text) end
end

require 'chunks/chunk'

# The category chunk looks for "category: news" on a line by
# itself and parses the terms after the ':' as categories.
# Other classes can search for Category chunks within
# rendered content to find out what categories this page
# should be in.
#
# Category lines can be hidden using ':category: news', for example
class Category < Chunk::Abstract
  def self.pattern() return /^(:)?category\s*:(.*)$/i end

  attr_reader :hidden, :list

  def initialize(match_data)
    super(match_data)
	@hidden = match_data[1]
    @list = match_data[2].split(',').map { |c| c.strip }
  end

  # If the chunk is hidden, erase the mask and return this chunk
  # otherwise, surround it with a 'div' block.
  def unmask(content)
    return '' if hidden
    
    category_urls = @list.map{|category| url(category) }.join(', ')
	replacement = '<div class="property"> category: ' + category_urls + '</div>'
    self if content.sub!(mask(content), replacement)
  end
  
  # TODO move presentation of page metadata to controller/view
  def url(category)
    %{<a class="category_link" href="../list/?category=#{category}">#{category}</a>}
  end
end

require 'chunks/wiki'

# Includes the contents of another page for rendering.
# The include command looks like this: "[[!include PageName]]".
# It is a WikiLink since it refers to another page (PageName)
# and the wiki content using this command must be notified
# of changes to that page.
# If the included page could not be found, a warning is displayed.
class Include < WikiChunk::WikiLink
  def self.pattern() /^\[\[!include(.*)\]\]\s*$/i end

  attr_reader :page_name

  def initialize(match_data)
    super(match_data)
    @page_name = match_data[1].strip
  end

  # This replaces the [[!include PageName]] text with
  # the contents of PageName if it exists. Otherwise
  # a warning is displayed.
  def mask(content) 
    page = content.web.pages[page_name]
    (page ? page.content : "<em>Could not include #{page_name}</em>")
  end

  # Keep this chunk regardless of what happens.
  def unmask(content) self end
end

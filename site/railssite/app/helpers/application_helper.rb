
require 'projects'

# The methods added to this helper will be available to all templates in the application.
module ApplicationHelper

  # Accepts a container (hash, array, enumerable, your type) and returns a string of option tags. Given a container 
  # where the elements respond to first and last (such as a two-element array), the "lasts" serve as option values and
  # the "firsts" as option text. Hashes are turned into this form automatically, so the keys become "firsts" and values
  # become lasts. If +selected+ is specified, the matching "last" or element will get the selected option-tag.
  #
  # Examples (call, result):
  #   html_options([["Dollar", "$"], ["Kroner", "DKK"]])
  #     <option value="$">Dollar</option>\n<option value="DKK">Kroner</option>
  #
  #   html_options([ "VISA", "Mastercard" ], "Mastercard")
  #     <option>VISA</option>\n<option selected>Mastercard</option>
  #
  #   html_options({ "Basic" => "$20", "Plus" => "$40" }, "$40")
  #     <option value="$20">Basic</option>\n<option value="$40" selected>Plus</option>
  def html_options(container, selected = nil)
    container = container.to_a if Hash === container
  
    html_options = container.inject([]) do |options, element| 
      if element.respond_to?(:first) && element.respond_to?(:last)
        if element.last != selected
          options << "<option value=\"#{element.last}\">#{element.first}</option>"
        else
          options << "<option value=\"#{element.last}\" selected>#{element.first}</option>"
        end
      else
        options << ((element != selected) ? "<option>#{element}</option>" : "<option selected>#{element}</option>")
      end
    end
    
    html_options.join("\n")
  end

  def projects
    ProjectList.new()
  end

  def add_topmenu( label, link )
    @topmenuitems = [] if @topmenuitems.nil?
    @topmenuitems << Breadcumb.new( link, label, true )
  end

  def add_rightmenu( label, link, show_link = true )
    @rightmenuitems = [] if @rightmenuitems.nil?
    @rightmenuitems << Breadcumb.new( link, label, show_link )
  end

  def add_rightmenu_separator( )
    @rightmenuitems = [] if @rightmenuitems.nil?
    @rightmenuitems << Separator
  end

  def add_breadcumb( label, link , show_link = true )
    @breadcumbs = [] if @breadcumbs.nil?
    @breadcumbs << Breadcumb.new( link, label, show_link )
  end
  
  def breadcumbs
    @breadcumbs.nil? ? [] : @breadcumbs
  end
  
  def topmenuitems
    @topmenuitems.nil? ? [] : @topmenuitems
  end

  def rightmenuitems
    @rightmenuitems.nil? ? [] : @rightmenuitems
  end
  
  def show_breadcumbs
    content = ''
    for i in 0...breadcumbs.length
      item = breadcumbs[i]
      if (item.show_link)
        content += "<a href='#{item.link}'>#{item.label}</a>"
      else
        content += "#{item.label}"
      end
      content += ' >> ' unless i == (breadcumbs.length - 1)
    end
    content
  end

  def show_topmenu
    content = ''
    for i in 0...topmenuitems.length
      item = topmenuitems[i]
      content += "<a href='#{item.link}'>#{item.label}</a>"
      content += ' | ' unless i == (topmenuitems.length - 1)
    end
    content
  end
  
  def show_rightmenu
    content = ''
    for i in 0...rightmenuitems.length
      item = rightmenuitems[i]    
      if (item == Separator)
        content += "<hr noshade size=\"1\" color=\"#FFFFFF\">"
      else 
        if (item.show_link)
          content += "<div class=\"contentmenuitem\"><a href='#{item.link}'>#{item.label}</a></div>"
        else
          content += "#{item.label}"
        end
      end
    end
    content
  end
  
  def has_breadcumbs
    return true if breadcumbs.length != 0
    return false
  end
  
  def has_topmenu
    return true if topmenuitems.length != 0
    return false
  end
  
  def has_rightmenu
    return true if rightmenuitems.length != 0
    return false
  end
  
  def set_quick_access(val)
    @quick_access = val
  end

end

class Breadcumb

  attr_reader :link, :label
  attr_accessor :show_link
  
  def initialize( link, label, show_link = true )
    @link, @label = link, label
    @show_link = show_link
  end
  
end

Separator = Breadcumb.new( '-', '-', false ) 

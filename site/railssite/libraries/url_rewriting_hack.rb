# Below are some hacks to Rails internal classes that implement Instiki URLs scheme.
# It is no doubt a bad practice to override internal implementation of anything.
# When Rails implements some way to do it in the framework, this code should be replaced 
# with something more legitimate.

# In Instiki URLs are mapped to the ActionPack actions, possibly performed on a particular 
# web (sub-wiki) and page within that web.
#
# 1. Controller is determined by action name (default is 'wiki')
# 2. '/name1/' maps to action 'name1', unspecified web
#    Example: http://localhost/create_system/
# 3. Special case of above, URI '/wiki/' maps to action 'index', because Rails sets this address 
#    when default controller name is specified as 'wiki', and an application root 
#    (http://localhost:2500/)is requested.
# 4. '/name1/name2/' maps to web 'name1', action 'name2'
#    Example: http://localhost/mywiki/search/
# 5. '/name1/name2/name3/' maps to web 'name1', action 'name2', 
#    Example: http://localhost/mywiki/show/HomePage


require 'dispatcher'

# Overrides Rails DispatchServlet.parse_uri 
class DispatchServlet

  def self.parse_uri(path)
    result = parse_path(path)
    if result
      result[:controller] = ActionMapper.map_to_controller(result[:action])
      result
    else
      false
    end
  end

  def self.parse_path(path)
    ApplicationController.logger.debug "Parsing URI '#{path}'"
    component = '([-_a-zA-Z0-9]+)'
    page_name = '(.*)'
    case path.sub(%r{^/(?:fcgi|mruby|cgi)/}, "/")
      when '/wiki/'
        { :web => nil, :controller => 'wiki', :action => 'index' }
      when %r{^/#{component}/?$}
        { :web => nil, :controller => 'wiki', :action => $1 }
      when %r{^/#{component}/#{component}/?$}
        { :web => $1, :controller => 'wiki', :action => $2 }
      when %r{^/#{component}/#{component}/(.*)/?$}
        { :web => $1, :controller => 'wiki', :action => $2, :id => drop_trailing_slash($3) }
      else
        false
    end
  end

  def self.drop_trailing_slash(line) 
    if line[-1] == ?/
      line.chop
    else
      line
    end
  end
  
  class ActionMapper

    @@action_to_controller_map = {
      'create_system' => 'admin',
      'create_web' => 'admin',
      'edit_web' => 'admin',
      'file' => 'file',
      'import' => 'file',
      'pic' => 'file',
      'update_web' => 'admin'
    }
    
    def self.map_to_controller(action)
      @@action_to_controller_map[action] || 'wiki'
    end
  
  end

end


require 'action_controller/url_rewriter.rb'

# Overrides parts of AP UrlRewriter to achieve the Instiki's legacy URL scheme
module ActionController
  class UrlRewriter

    VALID_OPTIONS << :web unless VALID_OPTIONS.include? :web
  
    private  
    
    def resolve_aliases(options)
      options[:controller_prefix] = options[:web] unless options[:web].nil?
      options
    end
  
    def controller_name(options, controller_prefix)
      ensure_slash_suffix(options, :controller_prefix)
  
      controller_name = case options[:controller_prefix]
        when String:  options[:controller_prefix]
        when false : ""
        when nil   : controller_prefix || ""
      end
      # In Instiki we don't need the controller name (there is only one comtroller, anyway)
      # therefore the below line is commented out
      # controller_name << (options[:controller] + "/") if options[:controller] 
      return controller_name
    end
  end
end

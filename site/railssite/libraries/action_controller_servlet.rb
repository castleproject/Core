require 'erb'
require 'cgi'
require 'webrick'
include WEBrick

require 'view_helper'

class ActionControllerServlet < HTTPServlet::AbstractServlet
  @@template_root = "./views"
  def self.template_root() @@template_root end
  def self.template_root=(template_root) @@template_root = template_root end

  include ViewHelper
  
  def do_POST(req, res) do_GET(req, res) end 
  
  def do_GET(req, res)
    @req, @res = req, res

    @res['Content-Type']  = "text/html; charset=utf-8"
    @res['Pragma']        = "no-cache"
    @res['Cache-Control'] = "no-cache"

    @params = @req.query
    @assigns = {}
    @performed_render = @performed_redirect = false

    @logger.info "Performing #{action_name}"
    @logger.info "  Parameters: #{@params.inspect}"
    @logger.info "  Cookies: #{@req.cookies.collect { |c| "#{c.name} => #{c.value}" }.join(", ") }"

    perform_action
    @res
  end
  
  protected
    def template_root() self.class.template_root end

    # Issues a HTTP 302 (location) redirect to the specified <tt>path</tt>. Query string
    # parameters can be specified in the <tt>params</tt> hash and are automatically URL escaped.
    # An <tt>anchor</tt> can also be specified (as a string).
    def redirect_path(path, params = nil, anchor = nil)
      path << build_query_string(params) unless params.nil?
      path << "\##{anchor}" unless anchor.nil?
      @res.set_redirect WEBrick::HTTPStatus::MovedPermanently, path
      @performed_redirect = true
    end

    # Redirects the browser to another action within the current controller, so redirect_path "pages"
    # within a controller called WikiController would take the user to "http://www.example.com/wiki/pages"
    def redirect_action(action, params = nil, anchor = nil)
      redirect_path "/#{controller_name}/#{action}", params, anchor
    end

    
    # Compiles the template response and adds it to the response. If no template_name has been
    # supplied, the action parameter parsed to the controller is used. So invoking
    # render on a object initialized with myController.new("show_person") will compile the template
    # located at "../views/show_person.rhtml". Notice that the ".rhtml" extension is postfixed
    # to the template_name regardless of one was passed or that the action parameter was used.
    def render(template_name = "#{controller_name}/#{action_name}")
      @performed_render = true
      @logger.info "Rendering: #{template_name}"
      add_instance_variables_to_assigns
      @res.body = template_result "#{template_root}/#{template_name}.rhtml"
    end

    def render_text(text)
      @performed_render = true
      @logger.info "Rendering in text"
      @res.body = text
    end

    # Wrapper around render that presumes the current controller is used as a base for the action, 
    # so render_action("page") in WikiController will be equal to render("wiki/page")
    def render_action(action_name)
      render "#{controller_name}/#{action_name}"
    end

    def sub_template(template_name)
      template_result "#{template_root}/#{template_name}.rhtml"
    end

    # Returns the value of the cookie matching +name+ (or nil if none is found).
    def read_cookie(key)
      cookies = @req.cookies.select { |cookie| cookie.name == key }
      cookies.empty? ? nil : cookies.first.value
    end

    def write_cookie(key, value, permanent = false)
      @logger.info "Writing cookie: #{key} => #{value}"

      cookie = WEBrick::Cookie.new(key, value)
      cookie.path = "/"
      cookie.expires = Time.local(2030) if permanent
      @res.cookies << cookie
    end


    # Returns the last part of the uri path ("page" in "/wiki/page") by default, but
    # can be overwritten to implement mod_rewrite-like behaviour, such as "page" in "/wiki/page/home"
    def action_name
      @req.path.to_s.split(/\//).last
    end

    # Returns an array with each of the parts in the request as an element. So /something/cool/dude
    # returns ["something", "cool", "dude"]
    def request_path
      request_path_parts = @req.path.to_s.split(/\//)
      request_path_parts.length > 1 ? request_path_parts[1..-1] : []
    end

    # Can be overwritten by a controller class to implement shared behaviour for all the actions in
    # the class, such as security measures
    def before_action() end


  private
    # Wraps around all action calls to ensure the session is properly updated and closed.
    # Figures out whether an action, such as "list", is implemented as show_list or do_list.
    # The show_* type has precedence and automatically calls render after execution.
    def perform_action
      if before_action == false then return end
        
      if action_methods.include?(action_name)
        send(action_name)
        render unless @performed_render || @performed_redirect || !@res.body.empty?
      elsif template_exists_for_action
        render
      else
        raise "No action responded to #{action_name}", caller
      end
    end

    def add_instance_variables_to_assigns
      instance_variables.each { |var|
        next if protected_instance_variables.include?(var)
        @assigns[var[1..-1]] = instance_variable_get(var)
      }
    end

    def protected_instance_variables
      [ "@assigns", "@performed_redirect", "@performed_render" ]
    end

    def action_methods
      methods - Object.instance_methods
    end
  
    # Returns true if a template exists in the controller directory for the specified <tt>action_name</tt>,
    # which would mean true if called for WikiController#changes and "/views/wiki/changes.rhtml" existed.
    def template_exists_for_action
      File.exist? action_template_path
    end
  
    def action_template_path(action = action_name)
      "#{template_root}/#{controller_name}/#{action}.rhtml"
    end
  
    def template_result(template_path)
      @assigns.each { |key, value| instance_variable_set "@#{key}", value }
      ERB.new(IO.readlines(template_path).join).result(binding)
    end
  
    # Converts the class name from something like "OneModule::TwoModule::NeatController"
    # to "neat".
    def controller_name
      self.class.to_s.split("::").last.sub(/Controller/, "").downcase
    end

    # Returns a query string with escaped keys and values from the passed hash.
    def build_query_string(hash)
      elements = []
      hash.each { |key, value| elements << "#{CGI.escape(key)}=#{CGI.escape(value.to_s)}" }
      "?" + elements.join("&")
    end
end
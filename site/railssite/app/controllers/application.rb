require 'url_rewriting_hack'

# The filters added to this controller will be run for all controllers in the application.
# Likewise will all the methods added be available for all controllers.
class ApplicationController < ActionController::Base

  # implements Instiki's legacy URLs
  require 'url_rewriting_hack'

  before_filter :set_utf8_http_header, :connect_to_model
  after_filter :remember_location

  # For injecting a different wiki model implementation. Intended for use in tests
  def self.wiki=(the_wiki)
    # a global variable is used here because Rails reloads controller and model classes in the 
    # development environment; therefore, storing it as a class variable does not work
    # class variable is, anyway, not much different from a global variable
    $instiki_wiki_service = the_wiki
    logger.debug("Wiki service: #{the_wiki.to_s}")
  end

  def self.wiki
    $instiki_wiki_service
  end

  protected
  
  def authorized?
    @web.nil? ||
    @web.password.nil? || 
    cookies['web_address'] == @web.password || 
    password_check(@params['password'])
  end

  def check_authorization
    if in_a_web? and 
        not authorized? and 
        not %w( login authenticate published ).include?(@action_name)
      redirect_to :action => 'login'
      return false
    end
  end

  def connect_to_model
    @action_name = @params['action'] || 'index'
    @web_name = @params['web']
    @wiki = wiki
    @web = @wiki.webs[@web_name] unless @web_name.nil?
    @page_name = @file_name = @params['id']
    @page = @wiki.read_page(@web_name, @page_name) unless @page_name.nil?
    @author = cookies['author'] || 'AnonymousCoward'
    check_authorization
  end

  FILE_TYPES = {
    '.exe' => 'application/octet-stream',
    '.gif' => 'image/gif',
    '.jpg' => 'image/jpeg',
    '.pdf' => 'application/pdf',
    '.png' => 'image/png',
    '.txt' => 'text/plain',
    '.zip' => 'application/zip'
  }

  def send_file(file, options = {})
    options[:type] ||= (FILE_TYPES[File.extname(file)] || 'application/octet-stream')
    super(file, options)
  end

  def in_a_web?
    not @web_name.nil?
  end

  def redirect_show(page_name = @page_name, web = @web_name)
    redirect_to :web => web, :controller => 'wiki', :action => 'show', 
        :id => CGI.escape(page_name || 'HomePage')
  end

  @@REMEMBER_NOT = ['locked', 'save', 'back', 'file', 'pic', 'import']
  def remember_location
    if @response.headers['Status'] == '200 OK'
      unless @@REMEMBER_NOT.include? action_name or @request.method != :get
        @session[:return_to] = url_for 
        logger.debug("Session ##{session.object_id}: remembered URL '#{@session[:return_to]}'")
      end
    end
  end

  def return_to_last_remembered
    # Forget the redirect location
    redirect_target, @session[:return_to] = @session[:return_to], nil
    # then try to redirect to it
    if redirect_target.nil?
      logger.debug("Session ##{session.object_id}: no remembered redirect location, trying /")
      redirect_to_url '/'
    else
      logger.debug("Session ##{session.object_id}: " +
          "redirect to the last remembered URL #{redirect_target}")
      redirect_to_url(redirect_target)
    end
  end

  def set_utf8_http_header
    @response.headers['Content-Type'] = 'text/html; charset=UTF-8'
  end

  def wiki
    $instiki_wiki_service
  end

end

require 'application'

class AdminController < ApplicationController

  layout 'default'

  def create_system
    if @wiki.setup?
      flash[:error] = <<-EOL
          Wiki has already been created in '#{@wiki.storage_path}'. Shut down Instiki and delete 
          this directory if you want to recreate it from scratch.<br/><br/>
          (WARNING: this will destroy content of your current wiki).
      EOL
      redirect_show('HomePage', @wiki.webs.keys.first)
    elsif @params['web_name']
      # form submitted -> create a wiki
      @wiki.setup(@params['password'], @params['web_name'], @params['web_address']) 
      flash[:info] = <<-EOL
          Your new wiki '#{@params['web_name']}' is created!<br/>
          Please edit its home page and press Submit when finished.
      EOL
      redirect_to  :web => @params['web_address'], :controller => 'wiki', :action => 'new', 
          :id => 'HomePage'
    else
      # no form submitted -> go to template
    end
  end

  def create_web
    if @params['address']
      # form submitted
      if @wiki.authenticate(@params['system_password'])
        @wiki.create_web(@params['name'], @params['address'])
        redirect_show('HomePage', @params['address'])
      else 
        redirect_to :action => 'index'
      end
    else
      # no form submitted -> render template
      if @wiki.system[:password].nil?
        redirect_to :controller => 'wiki', :action => 'index'
      end
    end
  end

  def edit_web
    # to template
  end

  def update_web
    if wiki.authenticate(@params['system_password'])
      wiki.update_web(
        @web.address, @params['address'], @params['name'], 
        @params['markup'].intern, 
        @params['color'], @params['additional_style'], 
        @params['safe_mode'] ? true : false, 
        @params['password'].empty? ? nil : @params['password'],
        @params['published'] ? true : false, 
        @params['brackets_only'] ? true : false,
        @params['count_pages'] ? true : false,
        @params['allow_uploads'] ? true : false,
        @params['max_upload_size']
      )
      redirect_show('HomePage', @params['address'])
    else
      redirect_show('HomePage') 
    end
  end

end

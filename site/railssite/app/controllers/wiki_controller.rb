require 'application'
require 'fileutils'
require 'redcloth_for_tex'

class WikiController < ApplicationController

  layout 'default', :except => [:rss_feed, :rss_with_content, :rss_with_headlines, :tex,  :export_tex, :export_html]

  def index
    if @web_name
      redirect_show 'HomePage'
    elsif not @wiki.setup?
      redirect_to :controller => 'admin', :action => 'create_system'
    elsif @wiki.webs.length == 1
      redirect_show 'HomePage', @wiki.webs.values.first.address
    else
      redirect_to :action => 'web_list'
    end
  end

  # Outside a single web --------------------------------------------------------

  def authenticate
    if password_check(@params['password'])
      redirect_show('HomePage')
    else 
      redirect_to :action => 'login'
    end
  end

  def login
    # to template
  end
  
  def web_list
    @webs = wiki.webs.values.sort_by { |web| web.name }
  end


  # Within a single web ---------------------------------------------------------

  def authors
    @authors = @web.select.authors
  end
  
  def export_html
    export_pages_as_zip('html') do |page| 
      @page = page
      @link_mode = :export
      render_to_string 'wiki/print'
    end
  end

  def export_markup
    export_pages_as_zip(@web.markup) { |page| page.content }
  end

  def export_pdf
    file_name = "#{@web.address}-tex-#{@web.revised_on.strftime('%Y-%m-%d-%H-%M-%S')}"
    file_path = @wiki.storage_path + file_name

    export_web_to_tex "#{file_path}.tex"  unless FileTest.exists? "#{file_path}.tex"
    convert_tex_to_pdf "#{file_path}.tex"
    send_file "#{file_path}.pdf"
  end

  def export_tex
    file_name = "#{@web.address}-tex-#{@web.revised_on.strftime('%Y-%m-%d-%H-%M-%S')}.tex"
    file_path = @wiki.storage_path + file_name

    export_web_to_tex(file_path) unless FileTest.exists?(file_path)
    send_file file_path
  end

  def feeds
    # to template
  end

  def list
    parse_category
    @pages_by_name = @pages_in_category.by_name
    @page_names_that_are_wanted = @pages_in_category.wanted_pages
    @pages_that_are_orphaned = @pages_in_category.orphaned_pages
  end
  
  def recently_revised
    parse_category
    @pages_by_revision = @pages_in_category.by_revision
  end

  def remove_orphaned_pages
    if wiki.authenticate(@params['system_password'])
      wiki.remove_orphaned_pages(@web_name)
      redirect_to :action => 'list'
    else
      redirect_show 'HomePage'
    end
  end

  def rss_with_content
    render_rss
  end

  def rss_with_headlines
    render_rss(hide_description = true)
  end

  def search
    @query = @params['query']
    @title_results = @web.select { |page| page.name =~ /#{@query}/i }.sort
    @results = @web.select { |page| page.content =~ /#{@query}/i }.sort
    all_pages_found = (@results + @title_results).uniq
    if all_pages_found.size == 1
      redirect_show(all_pages_found.first.name)
    end
  end

  # Within a single page --------------------------------------------------------
  
  def cancel_edit
    @page.unlock
    redirect_show
  end
  
  def edit
    if @page.nil?
      redirect_to :action => 'index'
    elsif @page.locked?(Time.now) and not @params['break_lock']
      redirect_to :web => @web_name, :action => 'locked', :id => @page_name
    else
      @page.lock(Time.now, @author)
    end
  end
  
  def locked
    # to template
  end
  
  def new
    # to template
  end

  def pdf
    page = wiki.read_page(@web_name, @page_name)
    safe_page_name = @page.name.gsub(/\W/, '')
    file_name = "#{safe_page_name}-#{@web.address}-#{@page.created_at.strftime('%Y-%m-%d-%H-%M-%S')}"
    file_path = @wiki.storage_path + file_name

    export_page_to_tex("#{file_path}.tex") unless FileTest.exists?("#{file_path}.tex")
    # NB: this is _very_ slow
    convert_tex_to_pdf("#{file_path}.tex")
    send_file "#{file_path}.pdf"
  end

  def print
    @link_mode ||= :show
    # to template
  end

  def published
    if @web.published
      @page = wiki.read_page(@web_name, @page_name || 'HomePage') 
    else 
      redirect_show('HomePage') 
    end
  end
  
  def revision
    get_page_and_revision
  end

  def rollback
    get_page_and_revision
  end

  def save
    redirect_to :action => 'index' if @page_name.nil?
    cookies['author'] = @params['author']

    begin
      page = @web.pages[@page_name]
      if @web.pages[@page_name]
        wiki.revise_page(
            @web_name, @page_name, @params['content'], Time.now, 
            Author.new(@params['author'], remote_ip)
        )
        page.unlock
      else
        wiki.write_page(
            @web_name, @page_name, @params['content'], Time.now, 
            Author.new(@params['author'], remote_ip)
        )
      end
      redirect_show(@page_name)
    rescue Instiki::ValidationError => e
      page.unlock if defined? page
      flash[:error] = e
      return_to_last_remembered
    end
  end

  def show
    if @page
      begin
        render_action 'page'
      # TODO this rescue should differentiate between errors due to rendering and errors in 
      # the application itself (for application errors, it's better not to rescue the error at all)
      rescue => e
        logger.error e
        if in_a_web?
          redirect_to :web => @web_name, :action => 'edit',
              :action_suffix => "#{CGI.escape(@page_name)}?msg=#{CGI.escape(e.message)}"
        else
          raise e
        end
      end
    else
      redirect_to :web => @web_name, :action => 'new', :id => CGI.escape(@page_name)
    end
  end

  def tex
    @tex_content = RedClothForTex.new(@page.content).to_tex
  end


  private
    
  def convert_tex_to_pdf(tex_path)
    # TODO remove earlier PDF files with the same prefix
    # TODO handle gracefully situation where pdflatex is not available
    begin
      wd = Dir.getwd
      Dir.chdir(File.dirname(tex_path))
      logger.info `pdflatex --interaction=nonstopmode #{File.basename(tex_path)}`
    ensure
      Dir.chdir(wd)
    end
  end

  def export_page_to_tex(file_path)
    tex
    File.open(file_path, 'w') { |f| f.write(render_to_string('wiki/tex')) }
  end

  def export_pages_as_zip(file_type, &block)

    file_prefix = "#{@web.address}-#{file_type}-"
    timestamp = @web.revised_on.strftime('%Y-%m-%d-%H-%M-%S')
    file_path = @wiki.storage_path + file_prefix + timestamp + '.zip'
    tmp_path = "#{file_path}.tmp"

    Zip::ZipOutputStream.open(tmp_path) do |zip_out|
      @web.select.by_name.each do |page|
        zip_out.put_next_entry("#{page.name}.#{file_type}")
        zip_out.puts(block.call(page))
      end
      # add an index file, if exporting to HTML
      if file_type.to_s.downcase == 'html'
        zip_out.put_next_entry 'index.html'
        zip_out.puts <<-EOL
          <html>
            <head>
              <META HTTP-EQUIV="Refresh" CONTENT="0;URL=HomePage.#{file_type}">
            </head>
          </html>
        EOL
      end
    end
    FileUtils.rm_rf(Dir[@wiki.storage_path + file_prefix + '*.zip'])
    FileUtils.mv(tmp_path, file_path)
    send_file file_path
  end

  def export_web_to_tex(file_path)
    @tex_content = table_of_contents(@web.pages['HomePage'].content.dup, render_tex_web)
    File.open(file_path, 'w') { |f| f.write(render_to_string('wiki/tex_web')) }
  end

  def get_page_and_revision
    @revision = @page.revisions[@params['rev'].to_i]
  end

  def parse_category
    @categories = @web.categories
    @category = @params['category']
    if @categories.include?(@category)
      @pages_in_category = @web.select { |page| page.in_category?(@category) }
      @set_name = "category '#{@category}'"
    else 
      @pages_in_category = PageSet.new(@web).by_name
      @set_name = 'the web'
    end
    @category_links = @categories.map { |c| 
      if @category == c
        %{<span class="selected">#{c}</span>} 
      else
        %{<a href="?category=#{c}">#{c}</a>}
      end
    }
  end

  def password_check(password)
    if password == @web.password
      cookies['web_address'] = password
      true
    else
      false
    end
  end

  def remote_ip
    ip = @request.remote_ip
    logger.info(ip)
    ip
  end

  def render_rss(hide_description = false)
    @pages_by_revision = @web.select.by_revision.first(15)
    @hide_description = hide_description
    @response.headers['Content-Type'] = 'text/xml'
    render 'wiki/rss_feed'
  end

  def render_tex_web
    @web.select.by_name.inject({}) do |tex_web, page|
      tex_web[page.name] = RedClothForTex.new(page.content).to_tex
      tex_web
    end
  end

  def render_to_string(template_name)
    add_variables_to_assigns
    render template_name
    @performed_render = false
    @template.render_file(template_name)
  end
  
  def truncate(text, length = 30, truncate_string = '...')
    if text.length > length then text[0..(length - 3)] + truncate_string else text end
  end
  
end

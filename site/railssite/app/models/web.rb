require "cgi"
require "page"
require "page_set"
require "wiki_words"
require "zip/zip"

class Web
  attr_accessor :name, :address, :password, :markup, :color, :safe_mode, :pages
  attr_accessor :additional_style, :published, :brackets_only, :count_pages, :allow_uploads
  attr_accessor :max_upload_size

  def initialize(parent_wiki, name, address, password = nil)
    @wiki, @name, @address, @password = parent_wiki, name, address, password

    # default values
    @markup = :textile 
    @color = '008B26'
    @safe_mode = false
    @pages = {}
    @allow_uploads = true
    @additional_style = nil
    @published = false
    @brackets_only = false
    @count_pages = false
    @allow_uploads = true
    @max_upload_size = 100
  end

  def add_page(page)
    @pages[page.name] = page
  end

  def authors 
    select.authors 
  end

  def categories
    select.map { |page| page.categories }.flatten.uniq.sort
  end

  def has_page?(name)
    pages[name]
  end

  def has_file?(name)
    wiki.file_yard(self).has_file?(name)
  end

  def make_file_link(mode, name, text)
    link = CGI.escape(name)
    case mode
    when :export
      if has_file?(name) then "<a class=\"existingWikiWord\" href=\"#{link}.html\">#{text}</a>"
      else "<span class=\"newWikiWord\">#{text}</span>" end
    when :publish
      if has_file?(name) then "<a class=\"existingWikiWord\" href=\"../published/#{link}\">#{text}</a>"
      else "<span class=\"newWikiWord\">#{text}</span>" end
    else 
      if has_file?(name)
        "<a class=\"existingWikiWord\" href=\"../file/#{link}\">#{text}</a>"
      else 
        "<span class=\"newWikiWord\">#{text}<a href=\"../file/#{link}\">?</a></span>"
      end
    end
  end

  # Create a link for the given page name and link text based
  # on the render mode in options and whether the page exists
  # in the this web.
  def make_link(name, text = nil, options = {})
    text = CGI.escapeHTML(text || WikiWords.separate(name))
    mode = options[:mode]
    link_type = options[:link_type] || 'show'
    case link_type
    when 'show'
      make_page_link(mode, name, text)
    when 'file'
      make_file_link(mode, name, text)
    when 'pic'
      make_pic_link(mode, name, text)
    else
      raise "Unknown link type: #{link_type}"
    end
  end

  def make_page_link(mode, name, text)
    link = CGI.escape(name)
    case mode
    when :export
      if has_page?(name) then "<a class=\"existingWikiWord\" href=\"#{link}.html\">#{text}</a>"
      else "<span class=\"newWikiWord\">#{text}</span>" end
    when :publish
      if has_page?(name) then "<a class=\"existingWikiWord\" href=\"../published/#{link}\">#{text}</a>"
      else "<span class=\"newWikiWord\">#{text}</span>" end
    else 
      if has_page?(name)
        "<a class=\"existingWikiWord\" href=\"../show/#{link}\">#{text}</a>"
      else 
        "<span class=\"newWikiWord\">#{text}<a href=\"../show/#{link}\">?</a></span>"
      end
    end
  end

  def make_pic_link(mode, name, text)
    link = CGI.escape(name)
    case mode
    when :export
      if has_file?(name) then "<img alt=\"#{text}\" src=\"#{link}\" />"
      else "<img alt=\"#{text}\" src=\"no image\" />" end
    when :publish
      if has_file?(name) then "<img alt=\"#{text}\" src=\"#{link}\" />"
      else "<span class=\"newWikiWord\">#{text}</span>" end
    else 
      if has_file?(name) then "<img alt=\"#{text}\" src=\"../pic/#{link}\" />"
      else "<span class=\"newWikiWord\">#{text}<a href=\"../pic/#{link}\">?</a></span>" end
    end
  end

  def max_upload_size
    @max_upload_size || 100
  end

  # Clears the display cache for all the pages with references to 
  def refresh_pages_with_references(page_name)
    select.pages_that_reference(page_name).each { |page| 
      page.revisions.each { |revision| revision.clear_display_cache }
    }
  end
  
  def refresh_revisions
    select.each { |page| page.revisions.each { |revision| revision.clear_display_cache } }
  end

  def remove_pages(pages_to_be_removed)
    pages.delete_if { |page_name, page| pages_to_be_removed.include?(page) }
  end
  
  def revised_on
    select.most_recent_revision
  end
    
  def select(&condition)
    PageSet.new(self, @pages.values, condition)
  end
  
  private
    # Returns an array of all the wiki words in any current revision
    def wiki_words
      pages.values.inject([]) { |wiki_words, page| wiki_words << page.wiki_words }.flatten.uniq
    end
    
    # Returns an array of all the page names on this web
    def page_names
      pages.keys
    end
    
    # This ensures compatibility with 0.9 storages
    def wiki
      @wiki ||= WikiService.instance
    end

end
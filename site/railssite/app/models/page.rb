require 'date'
require 'page_lock'
require 'revision'
require 'wiki_words'
require 'chunks/wiki'

class Page
  include PageLock

  attr_reader :name, :web
  attr_accessor :revisions
  
  def initialize(web, name, content, created_at, author)
    @web, @name, @revisions = web, name, []
    revise(content, created_at, author)
  end

  def revise(content, created_at, author)

    if not @revisions.empty? and content == @revisions.last.content
      raise Instiki::ValidationError.new(
          "You have tried to save page '#{name}' without changing its content")
    end

    # A user may change a page, look at it and make some more changes - several times.
    # Not to record every such iteration as a new revision, if the previous revision was done 
    # by the same author, not more than 30 minutes ago, then update the last revision instead of
    # creating a new one
    if !@revisions.empty? && continous_revision?(created_at, author)
      @revisions.last.created_at = created_at
      @revisions.last.content = content
      @revisions.last.clear_display_cache
    else
      @revisions << Revision.new(self, @revisions.length, content, created_at, author)
    end

    self.revisions.last.force_rendering
    # at this point the page may not be inserted in the web yet, and therefore 
    # references to the page itself are rendered as "unresolved". Clearing the cache allows 
    # the page to re-render itself once again, hopefully _after_ it is inserted in the web
    self.revisions.last.clear_display_cache
    
    @web.refresh_pages_with_references(@name) if @revisions.length == 1
  end

  def rollback(revision_number, created_at, author_ip = nil)
    roll_back_revision = @revisions[revision_number].dup
    revise(roll_back_revision.content, created_at, Author.new(roll_back_revision.author, author_ip))
  end
  
  def revisions?
    revisions.length > 1
  end

  def revised_on
    created_on
  end

  def in_category?(cat)
    cat.nil? || cat.empty? || categories.include?(cat)
  end

  def categories
    display_content.find_chunks(Category).map { |cat| cat.list }.flatten
  end

  def authors
    revisions.collect { |rev| rev.author }
  end

  def references
    @web.select.pages_that_reference(name)
  end

  # Returns the original wiki-word name as separate words, so "MyPage" becomes "My Page".
  def plain_name
    @web.brackets_only ? name : WikiWords.separate(name)
  end

  def link(options = {})
    @web.make_link(name, nil, options)
  end

  def author_link(options = {})
    @web.make_link(author, nil, options)
  end


  private

  def continous_revision?(created_at, author)
    @revisions.last.author == author && @revisions.last.created_at + 30.minutes > created_at
  end

  # Forward method calls to the current revision, so the page responds to all revision calls
  def method_missing(method_symbol)
    revisions.last.send(method_symbol)
  end

end

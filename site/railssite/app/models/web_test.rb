require "test/unit"
require "wiki_service"

class WebTest < Test::Unit::TestCase
  def setup
    @web = Web.new "Instiki", "instiki"
  end

  def test_wiki_word_linking
    @web.add_page(Page.new(@web, "SecondPage", "Yo, yo. Have you EverBeenHated", Time.now, "DavidHeinemeierHansson"))
    assert_equal(
      "<p>Yo, yo. Have you <span class=\"newWikiWord\">Ever Been Hated<a href=\"../show/EverBeenHated\">?</a></span></p>",
      @web.pages["SecondPage"].display_content
    )
    
    @web.add_page(Page.new(@web, "EverBeenHated", "Yo, yo. Have you EverBeenHated", Time.now, "DavidHeinemeierHansson"))
    assert_equal(
      "<p>Yo, yo. Have you <a class=\"existingWikiWord\" href=\"../show/EverBeenHated\">Ever Been Hated</a></p>", 
      @web.pages["SecondPage"].display_content
    )
  end
  
  def test_pages_by_revision
    add_sample_pages
    assert_equal "EverBeenHated", @web.select.by_revision.first.name
  end
  
  def test_pages_by_match
    add_sample_pages
    assert_equal 2, @web.select { |page| page.content =~ /me/i }.length
    assert_equal 1, @web.select { |page| page.content =~ /Who/i }.length
    assert_equal 0, @web.select { |page| page.content =~ /none/i }.length
  end
  
  def test_references
    add_sample_pages
    assert_equal 1, @web.select.pages_that_reference("EverBeenHated").length
    assert_equal 0, @web.select.pages_that_reference("EverBeenInLove").length
  end
  
  def test_delete
    add_sample_pages
    assert_equal 2, @web.pages.length
    @web.remove_pages([ @web.pages["EverBeenInLove"] ])
    assert_equal 1, @web.pages.length
  end
  
  private
    def add_sample_pages
      @web.add_page(Page.new(@web, "EverBeenInLove", "Who am I me", Time.local(2004, 4, 4, 16, 50), "DavidHeinemeierHansson"))
      @web.add_page(Page.new(@web, "EverBeenHated", "I am me EverBeenHated", Time.local(2004, 4, 4, 16, 51), "DavidHeinemeierHansson"))
    end
end
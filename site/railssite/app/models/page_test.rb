require "test/unit"
require "web"
require "page"

class MockWeb < Web
  def initialize() super('test','test') end
  def [](wiki_word) %w( MyWay ThatWay SmartEngine ).include?(wiki_word) end
  def refresh_pages_with_references(name) end
end

class PageTest < Test::Unit::TestCase
  def setup
    @page = Page.new(
      MockWeb.new,
      "FirstPage", 
      "HisWay would be MyWay in kinda ThatWay in HisWay though MyWay \\OverThere -- see SmartEngine in that SmartEngineGUI", 
      Time.local(2004, 4, 4, 16, 50),
      "DavidHeinemeierHansson"
    )
  end

  def test_basics
    assert_equal "First Page", @page.plain_name
    assert_equal "April  4, 2004", @page.pretty_revised_on
  end

  def test_locking
    assert !@page.locked?(Time.local(2004, 4, 4, 16, 50))

    @page.lock(Time.local(2004, 4, 4, 16, 30), "DavidHeinemeierHansson")

    assert @page.locked?(Time.local(2004, 4, 4, 16, 50))
    assert !@page.locked?(Time.local(2004, 4, 4, 17, 1))

    @page.unlock

    assert !@page.locked?(Time.local(2004, 4, 4, 16, 50))
  end
  
  def test_locking_duration
    @page.lock(Time.local(2004, 4, 4, 16, 30), "DavidHeinemeierHansson")

    assert_equal 15, @page.lock_duration(Time.local(2004, 4, 4, 16, 45))
  end
  
  def test_revision
    @page.revise("HisWay would be MyWay in kinda lame", Time.local(2004, 4, 4, 16, 55), "MarianneSyhler")
    assert_equal 2, @page.revisions.length, "Should have two revisions"
    assert_equal "MarianneSyhler", @page.author, "Mary should be the author now"
    assert_equal "DavidHeinemeierHansson", @page.revisions.first.author, "David was the first author"
  end
  
  def test_rollback
    @page.revise("spot two", Time.now, "David")
    @page.revise("spot three", Time.now + 2000, "David")
    assert_equal 3, @page.revisions.length, "Should have three revisions"
    @page.rollback(1, Time.now)
    assert_equal "spot two", @page.content
  end
  
  def test_continous_revision
    @page.revise("HisWay would be MyWay in kinda lame", Time.local(2004, 4, 4, 16, 55), "MarianneSyhler")
    assert_equal 2, @page.revisions.length

    @page.revise("HisWay would be MyWay in kinda update", Time.local(2004, 4, 4, 16, 57), "MarianneSyhler")
    assert_equal 2, @page.revisions.length
    assert_equal "HisWay would be MyWay in kinda update", @page.revisions.last.content

    @page.revise("HisWay would be MyWay in the house", Time.local(2004, 4, 4, 16, 58), "DavidHeinemeierHansson")
    assert_equal 3, @page.revisions.length
    assert_equal "HisWay would be MyWay in the house", @page.revisions.last.content

    @page.revise("HisWay would be MyWay in my way", Time.local(2004, 4, 4, 17, 30), "DavidHeinemeierHansson")
    assert_equal 4, @page.revisions.length
  end
end
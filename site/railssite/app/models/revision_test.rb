require "web"
require "test/unit"
require "revision"

class MockWeb < Web; 
  attr_accessor :markup
  def pages() MockPages.new end
  def safe_mode() false end
end
class MockPages
  def [](wiki_word) %w( MyWay ThatWay SmartEngine ).include?(wiki_word) end
end
class MockPage
  attr_accessor :web, :revisions
  def name() "page" end
end

class RevisionTest < Test::Unit::TestCase

  def setup
    @mock_page = MockPage.new
    @mock_web  = MockWeb.new
    @mock_page.web = @mock_web

    @mock_web.markup = :textile

    @revision = Revision.new(
      @mock_page,
      1,
      "HisWay would be MyWay in kinda ThatWay in HisWay though MyWay \\OverThere -- see SmartEngine in that SmartEngineGUI", 
      Time.local(2004, 4, 4, 16, 50),
      "DavidHeinemeierHansson"
    )
    
    @revision_with_auto_links = Revision.new(
      @mock_page,
      1,
      "http://www.loudthinking.com/ points to ThatWay from david@loudthinking.com", 
      Time.local(2004, 4, 4, 16, 50),
      "DavidHeinemeierHansson"
    )

	@revision_with_aliased_links = Revision.new(
	  @mock_page,
	  1,
      "Would a [[SmartEngine|clever motor]] go by any other name?",
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"
	)

	@revision_with_wiki_word_in_em = Revision.new(
	  @mock_page,
	  1,
      "_should we go ThatWay or ThisWay _",
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"      
    )

	@revision_with_pre_blocks = Revision.new(
	  @mock_page,
	  1,
      "A <code>class SmartEngine end</code> would not mark up <pre>CodeBlocks</pre>",
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"
	)

	@revision_with_wikiword_in_tag = Revision.new(
	  @mock_page,
	  1,
      "That is some <em style=\"WikiWord\">Stylish Emphasis</em>",
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"
	)

	@revision_with_autolink_in_parentheses = Revision.new(
	  @mock_page,
      1,
      'The W3C body (http://www.w3c.org) sets web standards',
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"														 
	)

	@revision_with_link_in_parentheses = Revision.new(
	  @mock_page,
      1,
      'Instiki is a "Wiki Clone":http://www.c2.com/cgi/wiki?WikiWikiClones ("What is a wiki?":http://wiki.org/wiki.cgi?WhatIsWiki) that\'s so easy to setup',
	  Time.local(2004, 4, 4, 16, 50),
      "MarkReid"
    )

	@revision_with_image_link = Revision.new(
	  @mock_page,
      1,
      'This !http://hobix.com/sample.jpg! is a Textile image link.',
	  Time.local(2004, 4, 4, 16, 50),
      "MarkReid"
    )

	@revision_with_nowiki_text = Revision.new(
	  @mock_page,
	  1,
	  'Do not mark up <nowiki>[[this text]]</nowiki> or <nowiki>http://www.thislink.com</nowiki>.',
	  Time.local(2004, 4, 4, 16, 50),
	  "MarkReid"
	)

	@revision_with_bracketted_wiki_word = Revision.new(
	  @mock_page,
	  1,
	  'This is a WikiWord and a tricky name [[Sperberg-McQueen]].',
	  Time.local(2004, 4, 4, 16, 50),
	  "MarkReid"
    )

  end

  def test_wiki_words
    assert_equal %w( HisWay MyWay SmartEngine SmartEngineGUI ThatWay ), @revision.wiki_words.sort
  end
  
  def test_existing_pages
    assert_equal %w( MyWay SmartEngine ThatWay ), @revision.existing_pages.sort
  end
  
  def test_unexisting_pages
    assert_equal %w( HisWay SmartEngineGUI ), @revision.unexisting_pages.sort
  end
  
  def test_content_with_wiki_links
    assert_equal "<p><span class=\"newWikiWord\">His Way<a href=\"../show/HisWay\">?</a></span> would be <a class=\"existingWikiWord\" href=\"../show/MyWay\">My Way</a> in kinda <a class=\"existingWikiWord\" href=\"../show/ThatWay\">That Way</a> in <span class=\"newWikiWord\">His Way<a href=\"../show/HisWay\">?</a></span> though <a class=\"existingWikiWord\" href=\"../show/MyWay\">My Way</a> OverThere&#8212;see <a class=\"existingWikiWord\" href=\"../show/SmartEngine\">Smart Engine</a> in that <span class=\"newWikiWord\">Smart Engine <span class=\"caps\">GUI</span><a href=\"../show/SmartEngineGUI\">?</a></span></p>", @revision.display_content
  end

  def test_bluecloth
    @mock_web.markup = :markdown

    @revision = Revision.new(
      @mock_page,
      1,
      "My Headline\n===========\n\n that SmartEngineGUI", 
      Time.local(2004, 4, 4, 16, 50),
      "DavidHeinemeierHansson"
    )

	@revision_with_code_block = Revision.new(
      @mock_page,
      1,
      "This is a code block:\n    def a_method(arg)\n    return ThatWay\n\nNice!",
      Time.local(2004, 4, 4, 16, 50),
      "MarkReid"      
    )

    assert_equal "<h1>My Headline</h1>\n\n<p>that <span class=\"newWikiWord\">Smart Engine GUI<a href=\"../show/SmartEngineGUI\">?</a></span></p>", @revision.display_content

	assert_equal "<p>This is a code block:</p>\n\n<pre><code>def a_method(arg)\nreturn ThatWay\n</code></pre>\n\n<p>Nice!</p>", @revision_with_code_block.display_content
  end

  def test_rdoc
    @mock_web.markup = :rdoc

    @revision = Revision.new(
      @mock_page,
      1,
      "+hello+ that SmartEngineGUI", 
      Time.local(2004, 4, 4, 16, 50),
      "DavidHeinemeierHansson"
    )

    assert_equal "<tt>hello</tt> that <span class=\"newWikiWord\">Smart Engine GUI<a href=\"../show/SmartEngineGUI\">?</a></span>\n\n", @revision.display_content
  end
  
  def test_content_with_auto_links
    assert_equal "<p><a href=\"http://www.loudthinking.com/\">http://www.loudthinking.com/</a> points to <a class=\"existingWikiWord\" href=\"../show/ThatWay\">That Way</a> from <a href=\"mailto:david@loudthinking.com\">david@loudthinking.com</a></p>", @revision_with_auto_links.display_content
  end  

  def test_content_with_aliased_links
	assert_equal "<p>Would a <a class=\"existingWikiWord\" href=\"../show/SmartEngine\">clever motor</a> go by any other name?</p>", @revision_with_aliased_links.display_content
  end

  def test_content_with_wikiword_in_em
	assert_equal "<p><em>should we go <a class=\"existingWikiWord\" href=\"../show/ThatWay\">That Way</a> or <span class=\"newWikiWord\">This Way<a href=\"../show/ThisWay\">?</a></span> </em></p>", @revision_with_wiki_word_in_em.display_content
  end

  def test_content_with_wikiword_in_tag
    assert_equal "<p>That is some <em style=\"WikiWord\">Stylish Emphasis</em></p>", @revision_with_wikiword_in_tag.display_content
  end

  def test_content_with_pre_blocks
	assert_equal "A <code>class SmartEngine end</code> would not mark up <pre>CodeBlocks</pre>", @revision_with_pre_blocks.display_content
  end

  def test_content_with_autolink_in_parentheses
	assert_equal '<p>The <span class="caps">W3C</span> body (<a href="http://www.w3c.org">http://www.w3c.org</a>) sets web standards</p>', @revision_with_autolink_in_parentheses.display_content
  end

  def test_content_with_link_in_parentheses
	assert_equal '<p>Instiki is a <a href="http://www.c2.com/cgi/wiki?WikiWikiClones">Wiki Clone</a> (<a href="http://wiki.org/wiki.cgi?WhatIsWiki">What is a wiki?</a>) that&#8217;s so easy to setup</p>', @revision_with_link_in_parentheses.display_content
  end

  def test_content_with_image_link
	assert_equal '<p>This <img src="http://hobix.com/sample.jpg" alt="" /> is a Textile image link.</p>', @revision_with_image_link.display_content
  end

  def test_content_with_nowiki_text
	assert_equal '<p>Do not mark up [[this text]] or http://www.thislink.com.</p>', @revision_with_nowiki_text.display_content
  end

  def test_content_with_bracketted_wiki_word
	@mock_web.brackets_only = true
	assert_equal '<p>This is a WikiWord and a tricky name <span class="newWikiWord">Sperberg-McQueen<a href="../show/Sperberg-McQueen">?</a></span>.</p>', @revision_with_bracketted_wiki_word.display_content
  end

  def test_content_for_export
    assert_equal "<p><span class=\"newWikiWord\">His Way</span> would be <a class=\"existingWikiWord\" href=\"MyWay.html\">My Way</a> in kinda <a class=\"existingWikiWord\" href=\"ThatWay.html\">That Way</a> in <span class=\"newWikiWord\">His Way</span> though <a class=\"existingWikiWord\" href=\"MyWay.html\">My Way</a> OverThere&#8212;see <a class=\"existingWikiWord\" href=\"SmartEngine.html\">Smart Engine</a> in that <span class=\"newWikiWord\">Smart Engine <span class=\"caps\">GUI</span></span></p>", @revision.display_content_for_export
  end

  def test_double_replacing
    @revision.content = "VersionHistory\r\n\r\ncry VersionHistory"
    assert_equal(
      "<p><span class=\"newWikiWord\">Version History<a href=\"../show/VersionHistory\">?</a></span></p>\n\n\t<p>cry <span class=\"newWikiWord\">Version History<a href=\"../show/VersionHistory\">?</a></span></p>", 
      @revision.display_content
    )

    @revision.clear_display_cache

    @revision.content = "f\r\nVersionHistory\r\n\r\ncry VersionHistory"
    assert_equal(
      "<p>f<br />\n<span class=\"newWikiWord\">Version History<a href=\"../show/VersionHistory\">?</a></span></p>\n\n\t<p>cry <span class=\"newWikiWord\">Version History<a href=\"../show/VersionHistory\">?</a></span></p>", 
      @revision.display_content
    )
  end  

  def test_difficult_wiki_words
    @revision.content = "[[It's just awesome GUI!]]"
    assert_equal(
      "<p><span class=\"newWikiWord\">It&#8217;s just awesome <span class=\"caps\">GUI</span>!<a href=\"../show/It%27s+just+awesome+GUI%21\">?</a></span></p>", 
      @revision.display_content
    )
  end
  
  def test_revisions_diff
    page = MockPage.new
    web  = MockWeb.new
    web.markup = :textile
    page.web = web

    page.revisions = [ 0 ] 
    page.revisions << Revision.new(page, 1, "What a blue and lovely morning", Time.local(2004, 4, 4, 16, 50), "DavidHeinemeierHansson")
    page.revisions << Revision.new(page, 2, "What a red and lovely morning today", Time.local(2004, 4, 4, 16, 50), "DavidHeinemeierHansson")
    
    assert_equal "<p>What a <del class=\"diffmod\">blue </del><ins class=\"diffmod\">red </ins>and lovely <del class=\"diffmod\">morning</del><ins class=\"diffmod\">morning today</ins></p>", page.revisions.last.display_diff
  end
end
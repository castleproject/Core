require 'chunks/nowiki'
require 'chunks/match'
require 'test/unit'

class NoWikiTest < Test::Unit::TestCase
  include ChunkMatch

  def test_simple_nowiki
	match(NoWiki, 'This sentence contains <nowiki>[[raw text]]</nowiki>. Do not touch!',
		:plain_text => '[[raw text]]'
	)
  end

end

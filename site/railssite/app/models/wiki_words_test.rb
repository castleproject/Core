require "test/unit"
require "wiki_words"

class WikiWordsTest < Test::Unit::TestCase
  
  def test_utf8_characters_in_wiki_word
    assert_equal "Æåle Øen", WikiWords.separate("ÆåleØen")
    assert_equal "ÆÅØle Øen", WikiWords.separate("ÆÅØleØen")
    assert_equal "Æe ÅØle Øen", WikiWords.separate("ÆeÅØleØen")
    assert_equal "Legetøj", WikiWords.separate("Legetøj")
  end
end

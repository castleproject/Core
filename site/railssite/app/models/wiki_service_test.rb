require "test/unit"
require "wiki_service"

class WikiServiceTest < Test::Unit::TestCase
  def setup
    WikiService.restart
    @s = WikiService.instance
    @s.create_web "Instiki", "instiki"
  end

  def test_read_write_page
    @s.write_page "instiki", "FirstPage", "Electric shocks, I love 'em", Time.now, "DavidHeinemeierHansson"
    assert_equal "Electric shocks, I love 'em", @s.read_page("instiki", "FirstPage").content
  end
end

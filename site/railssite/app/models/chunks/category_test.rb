require 'chunks/category'
require 'chunks/match'
require 'test/unit'

class CategoryTest < Test::Unit::TestCase
  include ChunkMatch

  def test_single_category
	match(Category, 'category: test', :list => ['test'], :hidden => nil)
	match(Category, 'category :   chunk test   ', :list => ['chunk test'], :hidden => nil)
	match(Category, ':category: test', :list => ['test'], :hidden => ':')
  end

  def test_multiple_categories
	match(Category, 'category: test, multiple', :list => ['test', 'multiple'], :hidden => nil)
	match(Category, 'category : chunk test , multi category,regression test case  ', 
		:list => ['chunk test','multi category','regression test case'], :hidden => nil
	)
  end

end

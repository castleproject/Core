require 'test/unit'
require 'diff'

include Diff

class DiffTest < Test::Unit::TestCase
  def test_init
    assert(1 == 1, "tests working")
    assert_nothing_raised("object created") do
      s = SequenceMatcher.new "private Thread currentThread;",
        "private volatile Thread currentThread;",
        proc { |x| x == ' ' }
    end
  end
  
  def test_matching_blocks
    s = SequenceMatcher.new "abxcd", "abcd"
    assert(s.get_matching_blocks == [[0, 0, 2], [3, 2, 2], [5, 4, 0]],
           "get_matching_blocks works")
  end
  
  def test_ratio
    s = SequenceMatcher.new "abcd", "bcde"
    assert(s.ratio == 0.75, "ratio works")
    assert(s.quick_ratio == 0.75, "quick_ratio works")
    assert(s.real_quick_ratio == 1.0, "real_quick_ratio works")
  end
  
  def test_longest_match
    s = SequenceMatcher.new(" abcd", "abcd abcd")
    assert(s.find_longest_match(0, 5, 0, 9) == [0, 4, 5], 
           "find_longest_match works")
    s = SequenceMatcher.new()
  end
  
  def test_opcodes
    s = SequenceMatcher.new("qabxcd", "abycdf")
    assert(s.get_opcodes == [
             [:delete, 0, 1, 0, 0],
             [:equal, 1, 3, 0, 2],
             [:replace, 3, 4, 2, 3],
             [:equal, 4, 6, 3, 5],
             [:insert, 6, 6, 5, 6]], "get_opcodes works")
  end


  def test_count_leading
    assert(Diff.count_leading('   abc', ' ') == 3, 
           "count_leading works")
  end

  def test_html2list
    a = "here is the original text"
    #p HTMLDiff.html2list(a)
  end

  def test_html_diff
    a = "this was the original string"
    b = "this is the super string"
    assert_equal 'this <del class="diffmod">was </del>' + 
           '<ins class="diffmod">is </ins>the ' +
           '<del class="diffmod">original </del>' + 
           '<ins class="diffmod">super </ins>string',
           HTMLDiff.diff(a, b)
  end
  
  def test_html_diff_with_multiple_paragraphs
    a = "<p>this was the original string</p>"
    b = "<p>this is</p>\r\n<p>the super string</p>\r\n<p>around the world</p>"

    assert_equal(
      "<p>this <del class=\"diffmod\">was </del>" + 
      "<ins class=\"diffmod\">is</ins></p>\r\n<p>the " +
      "<del class=\"diffmod\">original </del>" + 
      "<ins class=\"diffmod\">super </ins>string</p>\r\n" +
      "<p><ins class=\"diffins\">around the world</ins></p>",
      HTMLDiff.diff(a, b)
    )
  end
end
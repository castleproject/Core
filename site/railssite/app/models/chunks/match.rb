# This module is to be included in unit tests that involve matching chunks.
# It provides a easy way to test whether a chunk matches a particular string
# and any the values of any fields that should be set after a match.
module ChunkMatch

  # Asserts a number of tests for the given type and text.
  def match(type, test_text, expected)
	pattern = type.pattern
    assert_match(pattern, test_text)
    pattern =~ test_text   # Previous assertion guarantees match
    chunk = type.new($~)
    
    # Test if requested parts are correct.
    for method_sym, value in expected do
      assert_respond_to(chunk, method_sym)
      assert_equal(value, chunk.method(method_sym).call, "Checking value of '#{method_sym}'")
    end
  end
end

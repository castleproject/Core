
require 'madeleine/zmarshal'

require 'stringio'
require 'yaml'

class ZMarshalTest < Test::Unit::TestCase

  def test_full_circle_marshal
    target = Madeleine::ZMarshal.new(Marshal)
    object = ["foo", "bar"]
    stream = StringIO.new

    target.dump(object, stream)
    stream.rewind
    result = target.load(stream)
    
    assert_equal(object, result)
  end

  def test_full_circle_yaml
    target = Madeleine::ZMarshal.new(YAML)
    object = ["foo", "bar"]
    stream = StringIO.new

    target.dump(object, stream)
    stream.rewind
    result = target.load(stream)
    
    assert_equal(object, result)
  end

  def test_compression
    target = Madeleine::ZMarshal.new(Marshal)
    object = "x" * 1000

    stream = StringIO.new
    Marshal.dump(object, stream)
    reference_size = stream.size

    stream = StringIO.new
    target.dump(object, stream)
    compressed_size = stream.size

    assert(compressed_size < reference_size)
  end
end


def add_zmarshal_tests(suite)
  suite << ZMarshalTest.suite
end

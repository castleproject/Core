#
# Author::    Anders Bengtsson <ndrsbngtssn@yahoo.se>
# Copyright:: Copyright (c) 2004
#

require 'zlib'

module Madeleine
  #
  # Snapshot marshaller for compressed snapshots.
  #
  # Compresses the snapshots created by another marshaller. Uses either
  # Marshal (the default) or another supplied marshaller.
  #
  # Uses <tt>zlib</tt> to do on-the-fly compression/decompression.
  #
  # ZMarshal works with Ruby's own Marshal and YAML, but not with SOAP
  # marshalling.
  #
  # Usage:
  #
  #  require 'madeleine'
  #  require 'madeleine/zmarshal'
  #
  #  marshaller = Madeleine::ZMarshal.new(YAML)
  #  madeleine = SnapshotMadeleine.new("my_example_storage", marshaller) {
  #    SomeExampleApplication.new()
  #  }
  #
  class ZMarshal

    def initialize(marshaller=Marshal)
      @marshaller = marshaller
    end

    def load(stream)
      zstream = Zlib::GzipReader.new(stream)
      begin
        # Buffer into a string first, since GzipReader can't handle
        # Marshal's 0-sized reads and SOAP can't handle streams at all.
        # In a bright future we can revert to reading directly from the
        # stream again.
        buffer = zstream.read
        return @marshaller.load(buffer)
      ensure
        zstream.finish
      end
    end

    def dump(system, stream)
      zstream = Zlib::GzipWriter.new(stream)
      begin
        @marshaller.dump(system, zstream)
      ensure
        zstream.finish
      end
      nil
    end
  end
end

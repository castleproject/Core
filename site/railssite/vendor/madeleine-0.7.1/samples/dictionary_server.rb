#
# A dictionary server using Distributed Ruby (DRb).
#
# All modifications to the dictionary are done as commands,
# while read-only queries (i.e 'lookup') are done directly.
#
# First launch this server in the background, then use
# dictionary_client.rb to look up and add items to the
# dictionary.
# You can kill the server at any time. The contents of the
# dictionary will still be there when you restart it.
#
# DRb is available at http://raa.ruby-lang.org/list.rhtml?name=druby
#

$LOAD_PATH.unshift(".." + File::SEPARATOR + "lib")
require 'madeleine'

require 'drb'


class Dictionary
  def initialize
    @data = {}
  end

  def add(key, value)
    @data[key] = value
  end

  def lookup(key)
    @data[key]
  end
end


class Addition
  def initialize(key, value)
    @key, @value = key, value
  end

  def execute(system)
    system.add(@key, @value)
  end
end


class Lookup
  def initialize(key)
    @key = key
  end

  def execute(system)
    system.lookup(@key)
  end
end


class DictionaryServer

  def initialize(madeleine)
    @madeleine = madeleine
    @dictionary = madeleine.system
  end

  def add(key, value)
    # When adding a new key-value pair we modify the system, so
    # this operation has to be done through a command.
    @madeleine.execute_command(Addition.new(key, value))
  end

  def lookup(key)
    # A lookup is a read-only operation, so we can do it as a non-logged
    # query. If we weren't worried about concurrency problems we could
    # have just called @dictionary.lookup(key) directly instead.
    @madeleine.execute_query(Lookup.new(key))
  end
end


madeleine = SnapshotMadeleine.new("dictionary-base") { Dictionary.new }

Thread.new(madeleine) {
  puts "Taking snapshot every 30 seconds."
  while true
    sleep(30)
    madeleine.take_snapshot
  end
}

DRb.start_service("druby://localhost:1234",
                  DictionaryServer.new(madeleine))
DRb.thread.join


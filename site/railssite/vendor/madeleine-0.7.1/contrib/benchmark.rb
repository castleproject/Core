#!/usr/local/bin/ruby -w

$LOAD_PATH.unshift("../lib")

require 'madeleine'
require 'batched'

class BenchmarkCommand
  def initialize(value)
    @value = value
  end

  def execute(system)
    # do nothing
  end
end

madeleine = BatchedSnapshotMadeleine.new("benchmark-base") { :the_system }

RUNS = 2000

GC.start
GC.disable

t0 = Time.now
RUNS.times {
  madeleine.execute_command(BenchmarkCommand.new(1234))
}
t1 = Time.now

GC.enable

tps = RUNS/(t1 - t0)

puts "#{tps.to_i} transactions/s"

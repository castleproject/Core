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

madeleine = BattchedSnapshotMadeleine.new("benchmark-base") { :the_system }

RUNS = 200
THREADS = 10

GC.start
GC.disable

t0 = Time.now

threads = []
THREADS.times {
  threads << Thread.new {
    RUNS.times {
      madeleine.execute_command(BenchmarkCommand.new(1234))
    }
  }
}
threads.each {|t| t.join }
t1 = Time.now

GC.enable

tps = (THREADS * RUNS)/(t1 - t0)

puts "#{tps.to_i} transactions/s"



require 'madeleine/clock'

class ClockedAddingSystem
  include Madeleine::Clock::ClockedSystem

  attr_reader :total

  def initialize
    @total = 0
  end

  def add(value)
    @total += value
    @total
  end
end

class TimeTest < Test::Unit::TestCase

  def test_clock
    target = Madeleine::Clock::Clock.new
    assert_equal(0, target.time.to_i)
    assert_equal(0, target.time.usec)

    t1 = Time.at(10000)
    target.forward_to(t1)
    assert_equal(t1, target.time)
    t2 = Time.at(20000)
    target.forward_to(t2)
    assert_equal(t2, target.time)

    assert_nothing_raised() {
      target.forward_to(t2)
    }
  end

  def test_time_actor
    @forward_calls = 0
    @last_time = Time.at(0)

    target = Madeleine::Clock::TimeActor.launch(self, 0.01)

    # When launch() has returned it should have sent
    # one synchronous clock-tick before it went to sleep
    assert_equal(1, @forward_calls)

    sleep(0.1)
    assert(@forward_calls > 1)
    target.destroy

    @forward_calls = 0
    sleep(0.1)
    assert_equal(0, @forward_calls)
  end

  # Self-shunt
  def execute_command(command)
    mock_system = self
    command.execute(mock_system)
  end

  # Self-shunt (system)
  def clock
    self
  end

  # Self-shunt (clock)
  def forward_to(time)
    if time < @last_time
      raise "non-monotonous time"
    end
    @last_time = time
    @forward_calls += 1
  end

  def test_clocked_system
    target = Object.new
    target.extend(Madeleine::Clock::ClockedSystem)
    t1 = Time.at(10000)
    target.clock.forward_to(t1)
    assert_equal(t1, target.clock.time)
    t2 = Time.at(20000)
    target.clock.forward_to(t2)
    assert_equal(t2, target.clock.time)
    reloaded_target = Marshal.load(Marshal.dump(target))
    assert_equal(t2, reloaded_target.clock.time)
  end
end


def add_clocked_tests(suite)
  suite << TimeTest.suite
end

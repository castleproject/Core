#
# Copyright(c) Anders Bengtsson 2003
#

require 'madeleine'

module Madeleine
  module Clock

    # Deprecated. Use SnapshotMadeleine instead.
    class ClockedSnapshotMadeleine < ::Madeleine::SnapshotMadeleine # :nodoc:
    end

    # Let your system extend this module if you need to access the
    # machine time. Used together with a TimeActor that keeps
    # the clock current.
    module ClockedSystem

      # Returns this system's Clock.
      def clock
        unless defined? @clock
          @clock = Clock.new
        end
        @clock
      end
    end

    # Sends clock ticks to update a ClockedSystem, so that time can be
    # dealt with in a deterministic way.
    class TimeActor

      # Create and launch a new TimeActor
      #
      # * <tt>madeleine</tt> - The SnapshotMadeleine instance to work on.
      # * <tt>delay</tt> - Delay between ticks in seconds (Optional).
      def self.launch(madeleine, delay=0.1)
        result = new(madeleine, delay)
        result
      end

      # Stops the TimeActor.
      def destroy
        @is_destroyed = true
        @thread.wakeup
        @thread.join
      end

      private_class_method :new

      private

      def initialize(madeleine, delay) #:nodoc:
        @madeleine = madeleine
        @is_destroyed = false
        send_tick
        @thread = Thread.new {
          until @is_destroyed
            sleep(delay)
            send_tick
          end
        }
      end

      def send_tick
        @madeleine.execute_command(Tick.new(Time.now))
      end
    end

    # Keeps track of time in a ClockedSystem.
    class Clock
      # Returns the system's time as a Ruby <tt>Time</tt>.
      attr_reader :time

      def initialize
        @time = Time.at(0)
      end

      def forward_to(newTime)
        @time = newTime
      end
    end

    #
    # Internal classes below
    #

    # Deprecated. Merged into default implementation.
    class TimeOptimizingLogger < ::Madeleine::Logger # :nodoc:
    end

  end
end

ClockedSnapshotMadeleine = Madeleine::Clock::ClockedSnapshotMadeleine

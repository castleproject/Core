#
# Simple example of using time with Madeleine.
#

$LOAD_PATH.unshift(".." + File::SEPARATOR + "lib")

require 'madeleine/clock'
require 'tk'

# The Clicker keeps track of when it was last clicked.
#
# To access the time it extends ClockedSystem, which provides
# it with the 'clock' attribute.
#
class Clicker
  include Madeleine::Clock::ClockedSystem

  def initialize
    @last_clicked = nil
  end

  def click
    @last_clicked = clock.time
  end

  def last_clicked
    return '-' if @last_clicked.nil?
    @last_clicked.to_s
  end
end

# A command to update the Clicker with.
#
class Click
  def execute(system)
    system.click
  end
end

# Launch a ClockedSnapshotMadeleine.
#
# ClockedSnapshotMadeleine works like the regular SnapshotMadeleine, but
# optimizes away redundant commands from TimeActor.
#
madeleine = ClockedSnapshotMadeleine.new("clock-demo") { Clicker.new }

# Launch the TimeActor.
#
# This provides time commands, without which the system's time would stand still.
#
Madeleine::Clock::TimeActor.launch(madeleine)

clicker = madeleine.system

# The GUI

root = TkRoot.new() { title "Madeleine Clock Example" }
label = TkLabel.new(root) {
  text "Last clicked " + clicker.last_clicked
  width 40
  pack
}
button = TkButton.new(root) {
  text 'Click'
  command proc {
    madeleine.execute_command(Click.new)
    label.text("Last clicked " + clicker.last_clicked)
  }
  pack
}

Tk.mainloop


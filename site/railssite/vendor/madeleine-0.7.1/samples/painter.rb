#
# Simple drawing program to show Madeleine's logging feature.
#
# When you restart the program, your old artwork is still there.
#
# (Note: The GUI components used here aren't marshal-able,
# so in a real app you would have to do custom marshaling for
# the Painter class to get working snapshots. Then again, in a real
# app you wouldn't use the GUI components to hold the app's data,
# would you?)
#

$LOAD_PATH.unshift(".." + File::SEPARATOR + "lib")

require 'madeleine'

require 'tkclass'

class Painter

  def initialize(canvas)
    @canvas = canvas
  end

  def draw(x, y)
    line = Line.new(@canvas, x, y, x + 1, y + 1)
    line.fill('black')
  end
end

class PaintCommand

  def initialize(x, y)
    @x, @y = x, y
  end

  def execute(system)
    system.draw(@x, @y)
  end
end

root = TkRoot.new() { title "Madeleine Painter" }
canvas = Canvas.new(root)
canvas.pack

$madeleine = Madeleine::SnapshotMadeleine.new("painter-demo") { Painter.new(canvas) }

canvas.bind("1",
            proc {|x, y|
              $madeleine.execute_command(PaintCommand.new(x, y))
            },
            "%x %y")
canvas.bind("B1-Motion",
            proc {|x, y|
              $madeleine.execute_command(PaintCommand.new(x, y))
            },
            "%x %y")

Tk.mainloop


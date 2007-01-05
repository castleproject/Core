import System
import System.IO
import System.Threading
import System.Diagnostics

# The command to launch the web server
SERVER_CMD = argv[0]
# Determine if the web server has to be restarted
# afther a built
SERVER_RESTART = argv[1] == 'restart'
# Arguments to send to with the server command
SERVER_ARGUMENTS = join(argv[2:], ' ')

# Arguments to send to nant when building
# Typically the target that builds and some arguments
NANT_ARGUMENTS = 'build -nologo -q'

# Time to wait before watching for changes again
REBUILD_DELAY = 1000 #ms


######## Stop editing here unless you're a damn cool Boo hacker ########
watcher = FileSystemWatcher(Directory.GetCurrentDirectory(), "*.cs")
watcher.IncludeSubdirectories = true
print "Starting live server process, hit CTRL+C to stop"

serverProcess as Process
rebuild = true

while (true):
  print "nant ${NANT_ARGUMENTS}"
  buildProcess = shellp('nant', NANT_ARGUMENTS)
  buildProcess.WaitForExit()
  rebuild = false

  if (buildProcess.ExitCode == 0):
    if SERVER_RESTART or serverProcess == null:
      # Try to stop the server
      if (serverProcess != null):
        try:
          serverProcess.Kill()
        except e:
          pass
        serverProcess.WaitForExit()
      # Starts the server
      print "${SERVER_CMD} ${SERVER_ARGUMENTS}"
      serverProcess = shellp(SERVER_CMD, SERVER_ARGUMENTS)
  else:
    print "Build failed! Errors:"
    print '=' * 80
    print buildProcess.StandardOutput.ReadToEnd()
    print '=' * 80
  
  Thread.CurrentThread.Sleep(REBUILD_DELAY)
  change = watcher.WaitForChanged(WatcherChangeTypes.All)
  print "File change detected (${change.Name})"

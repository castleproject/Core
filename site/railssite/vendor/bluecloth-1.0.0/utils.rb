#
#	Install/distribution utility functions
#	$Id: utils.rb,v 1.1 2005/01/07 23:01:51 alexeyv Exp $
#
#	Copyright (c) 2001-2004, The FaerieMUD Consortium.
#
#	This is free software. You may use, modify, and/or redistribute this
#	software under the terms of the Perl Artistic License. (See
#	http://language.perl.com/misc/Artistic.html)
#


BEGIN {
	require 'find'

	begin
		require 'readline'
		include Readline
	rescue LoadError => e
		$stderr.puts "Faking readline..."
		def readline( prompt )
			$stderr.print prompt.chomp
			return $stdin.gets.chomp
		end
	end
}

class File
    Win32Exts = %w{.exe .com .bat}

    def self::which( prog, path=ENV['PATH'] )
        path.split(File::PATH_SEPARATOR).each {|dir|
            # If running under Windows, look for prog + extensions
            if File::ALT_SEPARATOR
                ext = Win32Exts.find_all {|ext|
                    f = File::join(dir, prog+ext)
                    File::executable?(f) && !File::directory?(f)
                }
                ext.each {|f|
                    f = File::join( dir, prog + f ).gsub(%r:/:,'\\')
                    if block_given? then yield( f ) else return f end
                }
            else
                f = File::join( dir, prog )
                if File::executable?( f ) && ! File::directory?( f )
                    if block_given? then yield(f) else return f end
                end
            end
        }
    end

end


module UtilityFunctions

	# The list of regexen that eliminate files from the MANIFEST
	ANTIMANIFEST = [
		/makedist\.rb/,
		/\bCVS\b/,
		/~$/,
		/^#/,
		%r{docs/html},
		%r{docs/man},
		/\bTEMPLATE\.\w+\.tpl\b/,
		/\.cvsignore/,
		/\.s?o$/,
	]
	AMRegexp = Regexp::union( *ANTIMANIFEST )

	# Set some ANSI escape code constants (Shamelessly stolen from Perl's
	# Term::ANSIColor by Russ Allbery <rra@stanford.edu> and Zenin <zenin@best.com>
	AnsiAttributes = {
		'clear'      => 0,
		'reset'      => 0,
		'bold'       => 1,
		'dark'       => 2,
		'underline'  => 4,
		'underscore' => 4,
		'blink'      => 5,
		'reverse'    => 7,
		'concealed'  => 8,

		'black'      => 30,   'on_black'   => 40, 
		'red'        => 31,   'on_red'     => 41, 
		'green'      => 32,   'on_green'   => 42, 
		'yellow'     => 33,   'on_yellow'  => 43, 
		'blue'       => 34,   'on_blue'    => 44, 
		'magenta'    => 35,   'on_magenta' => 45, 
		'cyan'       => 36,   'on_cyan'    => 46, 
		'white'      => 37,   'on_white'   => 47
	}

	ErasePreviousLine = "\033[A\033[K"


	###############
	module_function
	###############

	# Create a string that contains the ANSI codes specified and return it
	def ansiCode( *attributes )
		return '' unless /(?:vt10[03]|xterm(?:-color)?|linux)/i =~ ENV['TERM']
		attr = attributes.collect {|a| AnsiAttributes[a] ? AnsiAttributes[a] : nil}.compact.join(';')
		if attr.empty? 
			return ''
		else
			return "\e[%sm" % attr
		end
	end


	# Test for the presence of the specified <tt>library</tt>, and output a
	# message describing the test using <tt>nicename</tt>. If <tt>nicename</tt>
	# is <tt>nil</tt>, the value in <tt>library</tt> is used to build a default.
	def testForLibrary( library, nicename=nil )
		nicename ||= library
		message( "Testing for the #{nicename} library..." )
		found = false

		begin
			require library
		rescue LoadError => err
			message "no found (%s)\n" % err.message
		else
			message "found\n"
			found = true
		end

		return found
	end


	# Test for the presence of the specified <tt>library</tt>, and output a
	# message describing the problem using <tt>nicename</tt>. If
	# <tt>nicename</tt> is <tt>nil</tt>, the value in <tt>library</tt> is used
	# to build a default. If <tt>raaUrl</tt> and/or <tt>downloadUrl</tt> are
	# specified, they are also use to build a message describing how to find the
	# required library. If <tt>fatal</tt> is <tt>true</tt>, a missing library
	# will cause the program to abort.
	def testForRequiredLibrary( library, nicename=nil, raaUrl=nil, downloadUrl=nil, fatal=true )
		nicename ||= library
		unless testForLibrary( library, nicename )
			msgs = [ "You are missing the required #{nicename} library.\n" ]
			msgs << "RAA: #{raaUrl}\n" if raaUrl
			msgs << "Download: #{downloadUrl}\n" if downloadUrl
			if fatal
				abort msgs.join('')
			else
				errorMessage msgs.join('')
			end
		end
		return true
	end

	### Output <tt>msg</tt> as a ANSI-colored program/section header (white on
	### blue).
	def header( msg )
		msg.chomp!
		$stderr.puts ansiCode( 'bold', 'white', 'on_blue' ) + msg + ansiCode( 'reset' )
		$stderr.flush
	end

	### Output <tt>msg</tt> to STDERR and flush it.
	def message( msg )
		$stderr.print ansiCode( 'cyan' ) + msg + ansiCode( 'reset' )
		$stderr.flush
	end

	### Output the specified <tt>msg</tt> as an ANSI-colored error message
	### (white on red).
	def errorMessage( msg )
		message ansiCode( 'bold', 'white', 'on_red' ) + msg + ansiCode( 'reset' )
	end

	### Output the specified <tt>msg</tt> as an ANSI-colored debugging message
	### (yellow on blue).
	def debugMsg( msg )
		return unless $DEBUG
		msg.chomp!
		$stderr.puts ansiCode( 'bold', 'yellow', 'on_blue' ) + ">>> #{msg}" + ansiCode( 'reset' )
		$stderr.flush
	end

	### Erase the previous line (if supported by your terminal) and output the
	### specified <tt>msg</tt> instead.
	def replaceMessage( msg )
		print ErasePreviousLine
		message( msg )
	end

	### Output a divider made up of <tt>length</tt> hyphen characters.
	def divider( length=75 )
		puts "\r" + ("-" * length )
	end
	alias :writeLine :divider

	### Output the specified <tt>msg</tt> colored in ANSI red and exit with a
	### status of 1.
	def abort( msg )
		print ansiCode( 'bold', 'red' ) + "Aborted: " + msg.chomp + ansiCode( 'reset' ) + "\n\n"
		Kernel.exit!( 1 )
	end

	### Output the specified <tt>promptString</tt> as a prompt (in green) and
	### return the user's input with leading and trailing spaces removed.
	def prompt( promptString )
		promptString.chomp!
		return readline( ansiCode('bold', 'green') + "#{promptString}: " + ansiCode('reset') ).strip
	end

	### Prompt the user with the given <tt>promptString</tt> via #prompt,
	### substituting the given <tt>default</tt> if the user doesn't input
	### anything.
	def promptWithDefault( promptString, default )
		response = prompt( "%s [%s]" % [ promptString, default ] )
		if response.empty?
			return default
		else
			return response
		end
	end

	### Search for the program specified by the given <tt>progname</tt> in the
	### user's <tt>PATH</tt>, and return the full path to it, or <tt>nil</tt> if
	### no such program is in the path.
	def findProgram( progname )
		ENV['PATH'].split(File::PATH_SEPARATOR).each {|d|
			file = File.join( d, progname )
			return file if File.executable?( file )
		}
		return nil
	end

	### Using the CVS log for the given <tt>file</tt> attempt to guess what the
	### next release version might be. This only works if releases are tagged
	### with tags like 'RELEASE_x_y'.
	def extractNextVersionFromTags( file )
		message "Attempting to extract next release version from CVS tags for #{file}...\n"
		raise RuntimeError, "No such file '#{file}'" unless File.exists?( file )
		cvsPath = findProgram( 'cvs' ) or
			raise RuntimeError, "Cannot find the 'cvs' program. Aborting."

		output = %x{#{cvsPath} log #{file}}
		release = [ 0, 0 ]
		output.scan( /RELEASE_(\d+)_(\d+)/ ) {|match|
			if $1.to_i > release[0] || $2.to_i > release[1]
				release = [ $1.to_i, $2.to_i ]
				replaceMessage( "Found %d.%02d...\n" % release )
			end
		}

		if release[1] >= 99
			release[0] += 1
			release[1] = 1
		else
			release[1] += 1
		end

		return "%d.%02d" % release
	end


	### Write a new manifest file with the given +named+, moving any current one
	### aside with an ".old" suffix if +backup+ is true.
	def makeManifest( name="MANIFEST", backup=true )
		message "Making manifest file '#{name}'"

		# Move an old one aside if a backup is desired
		if backup and File::exists?( name )
			File::rename( name, name + ".old" )
		end

		File::open( name, File::WRONLY|File::TRUNC|File::CREAT ) {|ofh|
			Find::find( "." ) do |file|
				Find.prune if AMRegexp =~ file
				Find.prune if %r{/\.} =~ file
				Find.prune if /TEMPLATE/ =~ file
				next if File::directory?( file )

				ofh.puts file
			end
		}
	end


	### Read the specified <tt>manifestFile</tt>, which is a text file
	### describing which files to package up for a distribution. The manifest
	### should consist of one or more lines, each containing one filename or
	### shell glob pattern.
	def readManifest( manifestFile="MANIFEST" )
		message "Reading manifest..."
		raise "Missing #{manifestFile}, please remake it" unless File.exists? manifestFile

		manifest = IO::readlines( manifestFile ).collect {|line|
			line.chomp
		}.select {|line|
			line !~ /^(\s*(#.*)?)?$/
		}

		filelist = []
		for pat in manifest
			$stderr.puts "Adding files that match '#{pat}' to the file list" if $VERBOSE
			filelist |= Dir.glob( pat ).find_all {|f| FileTest.file?(f)}
		end

		message "found #{filelist.length} files.\n"
		return filelist
	end


	### Given a <tt>filelist</tt> like that returned by #readManifest, remove
	### the entries therein which match the Regexp objects in the given
	### <tt>antimanifest</tt> and return the resultant Array.
	def vetManifest( filelist, antimanifest=ANITMANIFEST )
		origLength = filelist.length
		message "Vetting manifest..."

		for regex in antimanifest
			if $VERBOSE
				message "\n\tPattern /#{regex.source}/ removed: " +
					filelist.find_all {|file| regex.match(file)}.join(', ')
			end
			filelist.delete_if {|file| regex.match(file)}
		end

		message "removed #{origLength - filelist.length} files from the list.\n"
		return filelist
	end

	### Combine a call to #readManifest with one to #vetManifest.
	def getVettedManifest( manifestFile="MANIFEST", antimanifest=ANTIMANIFEST )
		vetManifest( readManifest(manifestFile), antimanifest )
	end

	### Given a documentation <tt>catalogFile</tt>, extract the title, if
	### available, and return it. Otherwise generate a title from the name of
	### the CVS module.
	def findRdocTitle( catalogFile="docs/CATALOG" )

		# Try extracting it from the CATALOG file from a line that looks like:
		# Title: Foo Bar Module
		title = findCatalogKeyword( 'title', catalogFile )

		# If that doesn't work for some reason, try grabbing the name of the CVS
		# repository the directory belongs to.
		if title.nil? && File::directory?( "CVS" ) &&
				File::exists?( "CVS/Repository" )
			title = File::read( "CVS/Repository" ).chomp
		end

		# As a last resort, use the name of the project directory
		if title.nil?
			distdir = File::dirname( __FILE__ )
			distdir = File::dirname( distdir ) if /docs$/ =~ distdir
			title = File::basename( distdir )
		end

		return title
	end

	### Given a documentation <tt>catalogFile</tt>, extract the name of the file
	### to use as the initally displayed page. If extraction fails, the
	### +default+ will be used if it exists. Returns +nil+ if there is no main
	### file to be found.
	def findRdocMain( catalogFile="docs/CATALOG", default="README" )

		# Try extracting it from the CATALOG file from a line that looks like:
		# Main: Foo Bar Module
		main = findCatalogKeyword( 'main', catalogFile )

		# Try to make some educated guesses if that doesn't work
		if main.nil?
			basedir = File::dirname( __FILE__ )
			basedir = File::dirname( basedir ) if /docs$/ =~ basedir
			
			if File::exists?( File::join(basedir, default) )
				main = default
			end
		end

		return main
	end


	### Given a documentation <tt>catalogFile</tt>, extract an upload URL for
	### RDoc.
	def findRdocUpload( catalogFile="docs/CATALOG" )
		findCatalogKeyword( 'upload', catalogFile )
	end


	### Given a documentation <tt>catalogFile</tt>, extract a CVS web frontend
	### URL for RDoc.
	def findRdocCvsURL( catalogFile="docs/CATALOG" )
		findCatalogKeyword( 'webcvs', catalogFile )
	end


	### Given a documentation <tt>catalogFile</tt>, try extracting the given
	### +keyword+'s value from it. Keywords are lines that look like:
	###   # <keyword>: <value>
	### Returns +nil+ if the catalog file was unreadable or didn't contain the
	### specified +keyword+.
	def findCatalogKeyword( keyword, catalogFile="docs/CATALOG" )
		val = nil

		if File::exists? catalogFile
			message "Extracting '#{keyword}' from CATALOG file (%s).\n" % catalogFile
			File::foreach( catalogFile ) {|line|
				debugMsg( "Examining line #{line.inspect}..." )
				val = $1.strip and break if /^#\s*#{keyword}:\s*(.*)$/i =~ line
			}
		end

		return val
	end


	### Given a documentation <tt>catalogFile</tt>, which is in the same format
	### as that described by #readManifest, read and expand it, and then return
	### a list of those files which appear to have RDoc documentation in
	### them. If <tt>catalogFile</tt> is nil or does not exist, the MANIFEST
	### file is used instead.
	def findRdocableFiles( catalogFile="docs/CATALOG" )
		startlist = []
		if File.exists? catalogFile
			message "Using CATALOG file (%s).\n" % catalogFile
			startlist = getVettedManifest( catalogFile )
		else
			message "Using default MANIFEST\n"
			startlist = getVettedManifest()
		end

		message "Looking for RDoc comments in:\n" if $VERBOSE
		startlist.select {|fn|
			message "  #{fn}: " if $VERBOSE
			found = false
			File::open( fn, "r" ) {|fh|
				fh.each {|line|
					if line =~ /^(\s*#)?\s*=/ || line =~ /:\w+:/ || line =~ %r{/\*}
						found = true
						break
					end
				}
			}

			message( (found ? "yes" : "no") + "\n" ) if $VERBOSE
			found
		}
	end

	### Open a file and filter each of its lines through the given block a
	### <tt>line</tt> at a time. The return value of the block is used as the
	### new line, or omitted if the block returns <tt>nil</tt> or
	### <tt>false</tt>.
	def editInPlace( file ) # :yields: line
		raise "No block specified for editing operation" unless block_given?

		tempName = "#{file}.#{$$}"
		File::open( tempName, File::RDWR|File::CREAT, 0600 ) {|tempfile|
			File::unlink( tempName )
			File::open( file, File::RDONLY ) {|fh|
				fh.each {|line|
					newline = yield( line ) or next
					tempfile.print( newline )
				}
			}

			tempfile.seek(0)

			File::open( file, File::TRUNC|File::WRONLY, 0644 ) {|newfile|
				newfile.print( tempfile.read )
			}
		}
	end

	### Execute the specified shell <tt>command</tt>, read the results, and
	### return them. Like a %x{} that returns an Array instead of a String.
	def shellCommand( *command )
		raise "Empty command" if command.empty?

		cmdpipe = IO::popen( command.join(' '), 'r' )
		return cmdpipe.readlines
	end

	### Execute a block with $VERBOSE set to +false+, restoring it to its
	### previous value before returning.
	def verboseOff
		raise LocalJumpError, "No block given" unless block_given?

		thrcrit = Thread.critical
		oldverbose = $VERBOSE
		begin
			Thread.critical = true
			$VERBOSE = false
			yield
		ensure
			$VERBOSE = oldverbose
			Thread.critical = false
		end
	end


	### Try the specified code block, printing the given 
	def try( msg, bind=nil )
		result = nil
		if msg =~ /^to\s/
			message = "Trying #{msg}..."
		else
			message = msg
		end
			
		begin
			rval = nil
			if block_given?
				rval = yield
			else
				file, line = caller(1)[0].split(/:/,2)
				rval = eval( msg, bind, file, line.to_i )
			end

			result = rval.inspect
		rescue Exception => err
			if err.backtrace
				nicetrace = err.backtrace.delete_if {|frame|
					/in `(try|eval)'/ =~ frame
				}.join("\n\t")
			else
				nicetrace = "Exception had no backtrace"
			end

			result = err.message + "\n\t" + nicetrace
		ensure
			puts result
		end
	end

	def time
		start = Time::now
		stimes = Process::times
		rval = yield
		etimes = Process::times
		$stderr.puts "Time elapsed: %0.5f user, %0.5f system (%0.5f wall clock seconds)" % [
			etimes.utime - stimes.utime,
			etimes.stime - stimes.stime,
			Time::now.to_f - start.to_f,
		]

		return rval
	end

end

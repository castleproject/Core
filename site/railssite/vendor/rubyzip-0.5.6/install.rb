#!/usr/bin/env ruby

$VERBOSE = true

require 'rbconfig'
require 'find'
require 'ftools'

include Config

files = %w{ stdrubyext.rb ioextras.rb zip.rb zipfilesystem.rb ziprequire.rb tempfile_bugfixed.rb }

INSTALL_DIR = File.join(CONFIG["sitelibdir"], "zip")
File.makedirs(INSTALL_DIR)
Dir.chdir "zip"
files.each { 
  |filename|
  installPath = File.join(INSTALL_DIR, filename)
  File::install(filename, installPath, 0644, true)
}
Dir.chdir ".."

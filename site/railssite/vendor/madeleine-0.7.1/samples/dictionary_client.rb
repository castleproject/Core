#
# Dictionary client
#
# See dictionary_server.rb for details
#

require 'drb'

DRb.start_service
dictionary = DRbObject.new(nil, "druby://localhost:1234")

if ARGV.length == 1
  puts dictionary.lookup(ARGV[0])
elsif ARGV.length == 2
  dictionary.add(ARGV[0], ARGV[1])
  puts "Stored"
else
  puts "Usage: dictionary_client <key> [<value>]"
end


  



require 'rubygems'

spec = Gem::Specification.new do |s|
  s.name = 'madeleine'
  s.version = '0.7.1'
  s.platform = Gem::Platform::RUBY
  s.required_ruby_version = ">= 1.8.1"
  s.summary = "Madeleine is a Ruby implementation of Object Prevalence"
  s.require_path = 'lib'
  s.autorequire = 'madeleine'
  s.author = "Anders Bengtsson"
  s.email = "ndrsbngtssn@yahoo.se"
  s.homepage = "http://madeleine.sourceforge.net"
  s.files = Dir.glob("lib/**/*.rb")
  s.files += Dir.glob("samples/**/*.rb")
  s.files += Dir.glob("contrib/**/*.rb")
  s.files += ['README', 'NEWS', 'COPYING']
end

if $0 == __FILE__
  Gem::Builder.new(spec).build
end

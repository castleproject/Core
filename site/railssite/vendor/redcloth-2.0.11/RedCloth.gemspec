require 'rubygems'
spec = Gem::Specification.new do |s|

  ## Basic Information

  s.name = 'RedCloth'
  s.version = "2.0.11"
  s.platform = Gem::Platform::RUBY
  s.summary = <<-TXT
    RedCloth is a module for using Textile in Ruby. Textile is a text format. 
    A very simple text format. Another stab at making readable text that can be converted to HTML.
  TXT

  ## Include tests, libs, docs

  s.files = ['tests/**/*', 'lib/**/*', 'docs/**/*', 'run-tests.rb'].collect do |dirglob|
                Dir.glob(dirglob)
            end.flatten.delete_if {|item| item.include?("CVS")}

  ## Load-time details

  s.require_path = 'lib'
  s.autorequire = 'redcloth'

  ## Author and project details

  s.author = "Why the Lucky Stiff"
  s.email = "why@ruby-lang.org"
  s.rubyforge_project = "redcloth"
  s.homepage = "http://www.whytheluckystiff.net/ruby/redcloth/"
end
if $0==__FILE__
  Gem::Builder.new(spec).build
end

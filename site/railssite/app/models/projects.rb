
class Project
  attr_reader :name, :link, :description

  def initialize( name, link, description = '' )
    @name, @link, @description = name, link, description
  end
end

class ProjectList
  include Enumerable

  def initialize()
    @list = []
    @list << Project.new( 'Castle Container', '/container/', 'The inversion of control container' )
    @list << Project.new( 'DynamicProxy', '/dynamicproxy/', 'Tool for generating dynamic proxies for interfaces and classes' )
    @list << Project.new( 'ManagementExtensions', '/management/', 'Expose managed components that can be access from anywhere' )
    @list << Project.new( 'Castle on Rails', '/castleonrails/', 'A rails-like web framework' )
    @list << Project.new( 'YAML', '/yaml/', 'A formatter for YAML' )
    @list << Project.new( 'Facilities', '/facilities/', 'Others useful facilities' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end

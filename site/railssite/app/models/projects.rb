
class Project
  attr_reader :name, :link, :description, :docs

  def initialize( name, link, description = '', docs = nil )
    @name, @link, @description, @docs = name, link, description, docs
  end
end

class ProjectList
  include Enumerable

  def initialize()
    @list = []
    @list << Project.new( 'Castle Container', '/container/', 'The inversion of control container', '/containerdocs/' )
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

class FacilitiesList 
  include Enumerable

  def initialize()
    @list = []
    @list << Project.new( 'Prevalence', '/prevalence/', '' )
    @list << Project.new( 'Management Extensions', '/management/facility', '' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end
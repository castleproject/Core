
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
    @list << Project.new( 'Castle Container', '/castle/show/container', 'The inversion of control container', '/containerdocs/' )
    @list << Project.new( 'DynamicProxy', '/castle/show/dynamicproxy', 'Tool for generating dynamic proxies for interfaces and classes' )
    @list << Project.new( 'ManagementExtensions', '/castle/show/management', 'Expose managed components that can be access from anywhere' )
    @list << Project.new( 'Castle on Rails', '/castle/show/castleonrails', 'A rails-like web framework' )
    @list << Project.new( 'YAML', '/castle/show/yaml', 'A formatter for YAML' )
    @list << Project.new( 'Facilities', '/castle/show/facilities', 'Extensions for the MicroKernel' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end

class Facility
  attr_reader :name, :link, :description, :docs

  def initialize( name, link, description = '', docs = nil )
    @name, @link, @description, @docs = name, link, description, docs
  end
end

class FacilitiesList 
  include Enumerable

  def initialize()
    @list = []
    @list << Facility.new( 'Aspect#', '/castle/show/Facility+AspectSharp', 'Enable AOP capabilities for components' )
    @list << Facility.new( 'ActiveRecord', '/castle/show/Facility+ActiveRecord', 'Simulates the Rail\'s ActiveRecord. Still on early stages' )
    @list << Facility.new( 'Prevalence', '/castle/show/Facility+Prevalence', 'Manages the prevalence of an object model' )
    @list << Facility.new( 'Automatic Transaction Management', '/castle/show/Facility+AutoTransaction', 'Manages transactions for methods' )
    @list << Facility.new( 'NHibernate', '/castle/show/Facility+NHibernate', 'Enables the usage of NHibernate O/R framework for your components' )
    @list << Facility.new( 'iBatisNet', '/castle/show/Facility+iBatis', 'Enables the usage of iBatis O/R framework for your components' )
    @list << Facility.new( 'Batch Registration', '/castle/show/Facility+BatchRegistration', 'Registers components based on configuration instructions' )
    @list << Facility.new( 'TypedFactory', '/castle/show/Facility+TypedFactory', 'Implements factories for components' )
    @list << Facility.new( 'RemoteComponent', '/castle/show/Facility+Remoting', 'Exposes and access remote components in a transparent fashion. Still on early stages' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end



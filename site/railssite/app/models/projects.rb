
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
    @list << Facility.new( 'Aspect#', '/facility/aspectsharp/', 'Enable AOP capabilities for components' )
    @list << Facility.new( 'ActiveRecord', '/facility/activerecord/', 'Simulates the Rail\'s ActiveRecord. Still on early stages' )
    @list << Facility.new( 'Prevalence', '/facility/prevalence/', 'Manages the prevalence of an object model' )
    @list << Facility.new( 'Automatic Transaction Management', '/facility/autotransaction/', 'Manages transactions for methods' )
    @list << Facility.new( 'NHibernate', '/facility/nhibernate/', 'Enables the usage of NHibernate O/R framework for your components' )
    @list << Facility.new( 'iBatisNet', '/facility/ibatis/', 'Enables the usage of iBatis O/R framework for your components' )
    @list << Facility.new( 'Batch Registration', '/facility/batchreg/', 'Registers components based on configuration instructions' )
    @list << Facility.new( 'TypedFactory', '/facility/typedfactory/', 'Implements factories for components' )
    @list << Facility.new( 'RemoteComponent', '/facility/remcomponents/', 'Exposes and access remote components in a transparent fashion. Still on early stages' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end



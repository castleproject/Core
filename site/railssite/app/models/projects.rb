
class Project
  attr_reader :name, :link

  def initialize( name, link )
    @name, @link = name, link
  end
end

class ProjectList
  include Enumerable

  def initialize()
    @list = []
    @list << Project.new( 'DynamicProxy', '/dynamicproxy/' )
    @list << Project.new( 'ManagementExtensions', '/management/' )
    @list << Project.new( 'Castle on Rails', '/castleonrails/' )
    @list << Project.new( 'Facilities', '/facilities/' )
  end
  
  def each
    @list.each { |item| yield(item) }
  end

end

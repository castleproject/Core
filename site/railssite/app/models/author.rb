class Author < String
  attr_accessor :ip
  def initialize(name, ip) @ip = ip; super(name) end
end
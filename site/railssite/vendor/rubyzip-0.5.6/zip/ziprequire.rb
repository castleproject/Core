require 'zip/zip'

class ZipList
  def initialize(zipFileList)
      @zipFileList = zipFileList
  end

  def get_input_stream(entry, &aProc)
    @zipFileList.each {
      |zfName|
      Zip::ZipFile.open(zfName) {
	|zf|
	begin
	  return zf.get_input_stream(entry, &aProc) 
	rescue Errno::ENOENT
	end
      }
    }
    raise Errno::ENOENT,
      "No matching entry found in zip files '#{@zipFileList.join(', ')}' "+
      " for '#{entry}'"
  end
end


module Kernel
  alias :oldRequire :require

  def require(moduleName)
    zip_require(moduleName) || oldRequire(moduleName)
  end

  def zip_require(moduleName)
    return false if already_loaded?(moduleName)
    get_resource(ensure_rb_extension(moduleName)) { 
      |zis| 
      eval(zis.read); $" << moduleName 
    }
    return true
  rescue Errno::ENOENT => ex
    return false
  end

  def get_resource(resourceName, &aProc)
    zl = ZipList.new($:.grep(/\.zip$/))
    zl.get_input_stream(resourceName, &aProc)
  end

  def already_loaded?(moduleName)
    moduleRE = Regexp.new("^"+moduleName+"(\.rb|\.so|\.dll|\.o)?$")
    $".detect { |e| e =~ moduleRE } != nil
  end

  def ensure_rb_extension(aString)
    aString.sub(/(\.rb)?$/i, ".rb")
  end
end

# Copyright (C) 2002 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.

# Model methods that want to rollback transactions gracefully 
# (i.e, returning the user back to the form from which the request was posted)
# should raise Instiki::ValidationError.
# 
# E.g. if a model object does
#   raise "Foo: '#{foo}' is not equal to Bar: '#{bar}'" if (foo != bar) 
# 
# then the operation is not committed; Rails returns the user to the page 
# where s/he was entering foo and bar, and the error message will be displayed 
# on the page

module Instiki
  class ValidationError < StandardError
  end
end
$!
	HtmlHelper/MapToVirtual(target)
	
	@param target		target's relative path
	
	@notes	Fails if 'ApplicationPath' contains a trailing '/' 
			or if 'target'contains a leading '/'
			
			Undefined behaviour if '\' is the path separator
			in 'target' or 'ApplicationPath'
!$
$controller.Context.ApplicationPath$/$target$
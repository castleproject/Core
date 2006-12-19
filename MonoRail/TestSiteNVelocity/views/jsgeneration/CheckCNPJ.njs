#if(!$isValid)
	## not valid CNPJ 
	
	$page.Call("alert", '"start"');
    $page.Call("showError", '"invalid data"');
    $page.el('lblcnpj').style.color.set('"#FF0000"');
	$page.Call("alert", '"end"');

#else
	## valid CNPJ

	#if($exists)
		##page.ReplaceHtml("divOperation", "%{partial='shared/cnpjDiv.vm'}")
		$page.ReplaceHtml("divOperation", "TODDDDDDDDDDDDDDDDDDDDDDDDDD AAAAAAAAAAAAAAAAAAAAA DDDDDDDDDDDDDDDDDDD AAAAAA")
		$page.Show("divOperation")
		$page.el('txtSenhaErro').focus()
    #end

    $page.el('lblcnpj').style.color.set('"#6784A0"');
    $page.Hide("errormsg"); 
#end
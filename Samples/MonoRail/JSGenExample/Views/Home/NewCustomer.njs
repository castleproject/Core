$page.Hide('addnewcustomerlink')

$page.replacehtml('container', "%{partial='home/_addcustomer'}")

$page.visualeffect('toggleappear', 'container')

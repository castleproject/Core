$page.InsertHtml('bottom', 'maintbody', "%{partial='home/_customerrow'}")
$page.VisualEffect('Highlight', "row${customer.id}")
$page.visualeffect('toggleappear', 'container')
$page.Show('addnewcustomerlink')

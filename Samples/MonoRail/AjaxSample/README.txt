The easiest way to run this sample is by using cassini. 

First, register Cassini.dll in the gac if you don't have it already.

> gacutil /i Cassini.dll (or run the registercassini.bat)

Run the CassiniWebServer.exe, put the full path for the AutoCompletionSample, then the port and then the virtual directory

In my case:

CassiniWebServer.exe E:\dev\projects\AjaxSample\AutoCompletionSample 81 /

Finally start your browser and open the url

http://localhost:81/account/index.rails



Enjoy!

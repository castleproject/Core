Installation instructions:

1. Create a new virtual directory named SimpleHttp in IIS Manager that points to the folder that this file is in.  Other than specifying the name and the path to the folder, you can leave the rest of the setting on the default.  If you use another name, you will have to manually edit the SimpleHttp.csproj.webinfo file to reflect your location.
2. Right-click, and select properties on your new virtual folder.  Virtual Directory tab, configuration.  Click add to create a new extention mapping.  Executable should be: 'C:\WINNT\Microsoft.NET\Framework\v1.0.3705\aspnet_isapi.dll' (or whereever aspnet_isapi.dll is located), extension should be: '.vm' (don't include the quotes).  Uncheck the "Check that file exists" checkbox.  Click OK.
3. You will need to give read and write permissions to the web directory to the 'aspnet' user.  This is so that the templates can be read and so that the log file can be created.
4. Load the solution, build and run.

This is just a simple example to show what has to be done to extend the base NVelocityHandler class.  A person could certainly imagine extending this so that a lookup was done on each request for a business logic class to execute and a template to display as a view.

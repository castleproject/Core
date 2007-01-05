The Generator project is a fully customizable code generator written in Boo and templates with ASP style syntax.

The scaffold and project generators are heavily inspired by RoR, it generates real code you can modify for the controller, the views, layouts, css and tests. The CRUD views will look similar to current MR scaffold with the difference that the code is live in you project so you can add a field, color, validation, change the workflow without copy-pasting (hurrr...) from the last controller you've done! Plus you already got some running tests!

This is a work in progress and considered in early alpha stage so use at your own risk but contributions or comments are welcome.

Consult the project home page for more information (http://wiki.castleproject.org/index.php/Generator).

To install
==========
To install the generate and monorail command line script, run:
    nant install
Under unix systems:
	sudo nant install
On Windows, monorail.bat and generate.bat will be copied to your c:\Windows directory. On Unix, the monorail and generate bash scripts will be copied to /usr/bin. You can change this directory by setting the 'to' property, like this:
    nant install -D:to=/opt/local/bin
Now the generate and monorail scripts are accessible from everywhere! Just like Jesus!

To compile
==========
To compile from source you need NAnt 0.85, then from the base directory, run:
    nant

To test
=======
To run automated tests, from the base directory, run:
    nant test

To run
======
Afther installation, simply run:
   generate
Or the MonoRail project generator shortcut:
   monorail MyMonoRailProject
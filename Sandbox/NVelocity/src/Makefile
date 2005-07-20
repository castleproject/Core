#
# Location of the source and destination folders.
#
SRCDIR :=	../src
LIBDIR :=	../lib
BINDIR :=	../bin
BUILDDIR :=     ../build
TMPDIR :=       ../tmp

default: build

help:
	@echo make build - build all binary files
	@echo make clean - removes all files that are created durring make processes
	@echo make docs - creates api and msdn docs
	@echo make dist - builds package for upload to SourceForge
	@echo make regress - run the unit tests

MODULES := $(shell /usr/bin/find -name module.mk)
#include $(MODULES)

.DELETE_ON_ERROR:

build:
	@rm -rf build.log
	@devenv NVelocity.sln /rebuild Debug /out build.log /nologo
	@tail -n 6 build.log
	# remove directories created by VS IDE
	@rm -rf ../build/commons
	@rm -rf ../build/http
	@rm -rf ../build/nant
	@rm -rf ../build/nvelocity
	@rm -rf build.log

changelog:
	@updatechangelog.sh

docs:
	@../bin/NAnt/nant
	../bin/NDoc/NDocConsole.exe -documenter=JavaDoc -project=NVelocity.ndoc

clean:
	-@rm -rf ../bin/NAnt/*.log*
	-@rm -rf ../build/temp
	-@rm -rf ../build/*dll
	-@rm -rf ../build/*pdb
	-@rm -rf ../build/*exe
	-@rm -rf ../build/*xml
	-@rm -rf ../build/*log
	-@rm -rf ../build/*properties
	-@rm -rf ../build/*.log*
	-@rm -rf ../www/api/
	-@rm -rf ../www/css/
	-@rm -rf ../www/images/
	-@rm -rf ../www/nant/
	-@rm -rf ../www/*.html
	-@rm -rf NVelocity.xml
	-@rm -rf NVelocity.NAnt.xml 
	-@rm -rf NVelocity.Test.xml 
	-@rm -rf Commons.xml 
	-@rm -rf build.log

all: build regress docs

regress:
	../bin/NUnit/nunit-console.exe ../build/NVelocity.Test.dll

## TODO: compile astyle with fix so that -T8 or --force-indent=tab=8 works
style:
	@cd ..; find -iname '*.cs' -print -exec bin/style.sh {} \;

dist: clean all
	./dist.sh
	cvs -q update -Pd

publishwww:
	scp -r ../www/* corts@nvelocity.sourceforge.net:/home/groups/n/nv/nvelocity/htdocs

publishapi:
	scp -r ../www/api/* corts@nvelocity.sourceforge.net:/home/groups/n/nv/nvelocity/htdocs/api


#
# Rule for displaying debug information.
#
.PHONY : debug

debug:
	@echo MODULES = $(MODULES)

convert2002:
	@cd ..; ./bin/VSConvert /2002 "**/*.sln"
	@cd ..; ./bin/VSConvert /2002 "**/*.csproj"

convert2003:
	@cd ..; ./bin/VSConvert /2003 "**/*.sln"
	@cd ..; ./bin/VSConvert /2003 "**/*.csproj"

version:
	sed -e "s/##_DATE_##/$(shell date)/" -e "s/##_BUILD_##/$(shell getversion.sh)/" -e "s/##_COMPUTER_##/$(COMPUTERNAME)/" -e "s/##_USER_##/$(USERNAME)/" webapp/version.thtml > webapp/version.aspx

package: clean build regress docs
	mkdir -p ../dist
	cd ..; jar -cMvf dist/NVelocity-$(shell getversion.sh).zip *.txt bin build examples lib src test www xdocs

tagversion:
	cd ..; cvs tag RELEASE_$(subst .,_,$(shell ./getversion.sh))
	${BINDIR}/UpdateVersion -b Increment -i AssemblyVersionInfo.cs -o AssemblyVersionInfo.cs

updateversion:
	${BINDIR}/UpdateVersion -b Increment -i AssemblyVersionInfo.cs -o AssemblyVersionInfo.cs

release: changelog updateversion package tagversion
	cvs commit -m "update to next version" AssemblyVersionInfo.cs


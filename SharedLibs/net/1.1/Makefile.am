boodir = $(prefix)/lib/boo
boo_DATA = booc.exe booi.exe booish.exe booc.rsp booc.exe.config booi.exe.config booish.exe.config

BOO_GAC_DLLS = Boo.Lang.dll \
               Boo.Lang.Useful.dll \
	       Boo.Lang.Compiler.dll \
	       Boo.Lang.Parser.dll \
	       Boo.Lang.Interpreter.dll \
	       Boo.Lang.CodeDom.dll

install-data-hook:
	for lib in $(BOO_GAC_DLLS); do					\
		echo "$(GACUTIL) /i $${lib} $(GACUTIL_FLAGS)" ;	\
		$(GACUTIL) /i $${lib} $(GACUTIL_FLAGS) || exit 1 ;	\
	done

EXTRA_DIST = $(BOO_GAC_DLLS) $(boo_DATA)

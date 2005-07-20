cd ..
./bin/cvs2cl.pl -f ChangeLog.txt -w -S
cvs commit -m "update change log" ChangeLog.txt
rm -f ChangeLog.txt.bak
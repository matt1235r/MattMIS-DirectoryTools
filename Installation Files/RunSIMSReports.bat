C:
CD "C:\Program Files (x86)\SIMS\SIMS .net"
CommandReporter.exe /user:example /password:example /report:"MattMIS Sync- Staff Extract" /output:"%~dp0Exports\Staff.xml"
CommandReporter.exe /user:example /password:example /report:"MattMIS Sync- Student Extract" /output:"%~dp0Exports\Students.xml"
pause



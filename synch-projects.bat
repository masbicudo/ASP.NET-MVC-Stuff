echo Synch all CS files in the MvcStuff projects
robocopy "MvcStuff_MVC3" "MvcStuff_MVC4" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC3" "MvcStuff_MVC5" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC3" "MvcStuff_MVC6" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it

robocopy "MvcStuff_MVC4" "MvcStuff_MVC3" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC4" "MvcStuff_MVC5" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC4" "MvcStuff_MVC6" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it

robocopy "MvcStuff_MVC5" "MvcStuff_MVC3" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC5" "MvcStuff_MVC4" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC5" "MvcStuff_MVC6" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it

robocopy "MvcStuff_MVC6" "MvcStuff_MVC3" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC6" "MvcStuff_MVC4" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it
robocopy "MvcStuff_MVC6" "MvcStuff_MVC5" "*.cs" /e /b /z /dcopy:T /COPY:DAT /xo /xx /xj /it

IF %ERRORLEVEL%==1 exit 0
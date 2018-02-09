@echo off
del bin\*.pdb /S /Q
rd obj /S /Q

@call 7z-flipdog MyAdb *.* -x!*.7z -x!bin\Debug\*
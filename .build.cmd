@echo off

echo Clean.
call rd-s-q bin
call rd-s-q obj

echo Build.
call msbuild MyAdb.sln /t:Rebuild /p:Configuration=Release

if %errorlevel% NEQ 0 (
	call check-error-level
	exit 1 /B
)

echo Copy to cmd.
set DIR=bin\Release
copy /Y /B %DIR%\Common.Utils.dll %MY_CMD%
copy /Y /B %DIR%\MyAdb.exe %MY_CMD%

echo Clean.
call rd-s-q bin
call rd-s-q obj
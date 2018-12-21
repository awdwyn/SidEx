@echo off

set /P MODVERSION=<"%~dp0SidEx.version"
set RELEASEFILE=%~dp0Releases\SidEx-%MODVERSION%.zip

set ZIP="c:\Program Files\7-zip\7z.exe"

if exist "%RELEASEFILE%" del /F "%RELEASEFILE%"
%ZIP% a -tzip "%RELEASEFILE%" "%~dp0Compiled Mods\GameData"
%ZIP% a -tzip "%RELEASEFILE%" "%~dp0Non-Compiled Mods\GameData"
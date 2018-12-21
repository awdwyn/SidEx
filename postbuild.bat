@echo off

rem post-build event: start /D "$(SolutionDir)" /WAIT postbuild.bat "$(SolutionDir)" "$(TargetDir)" "$(TargetFileName)" "$(Configuration)" "$(ProjectName)"

rem clean up quotes and spaces and set variables
set PROJECT=%~5
set PROJECT=%PROJECT: =%
set MODDIR=%~1Compiled Mods\GameData\SidEx\%PROJECT%\

rem prepare the working directories and copy vstudio output
rmdir /S /Q "%MODDIR%"
mkdir "%MODDIR%"
xcopy /E /Y "%~2*" "%MODDIR%"

rem clean up the output in release format
del /F /Q "%MODDIR%*.pdb"
mkdir "%MODDIR%Plugins"
move /Y "%MODDIR%*.dll" "%MODDIR%Plugins\"

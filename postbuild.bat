@echo off

rem post-build event: start /D "$(SolutionDir)" /WAIT postbuild.bat "$(SolutionDir)" "$(TargetDir)" "$(TargetFileName)" "$(Configuration)" "$(ProjectName)"

set PROJECT=%~5
set PROJECT=%PROJECT: =%
set MODDIR=%~1GameData (%~4)\SidEx\%PROJECT%\

rmdir /S /Q "%MODDIR%"
mkdir "%MODDIR%"

rem move "%~2" "%~1GameData (%~4)\SidEx\%PROJECT%"
xcopy /E /Y "%~2*" "%MODDIR%"

del /F /Q "%MODDIR%*.pdb"
mkdir "%MODDIR%Plugins"
move /Y "%MODDIR%*.dll" "%MODDIR%Plugins\"

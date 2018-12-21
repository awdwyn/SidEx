@echo off

rem post-build event: start /D "$(SolutionDir)" /WAIT postbuild.bat "$(SolutionDir)" "$(TargetDir)" "$(TargetFileName)" "$(Configuration)" "$(ProjectName)"

set PROJECT=%~5
set PROJECT=%PROJECT: =%

del /F /Q "%~2*.pdb"
mkdir "%~2Plugins"
move /Y "%~2*.dll" "%~2Plugins\"

rmdir /S /Q "%~1GameData (%~4)\SidEx\%PROJECT%"
mkdir "%~1GameData (%~4)\SidEx\%PROJECT%"

rem move "%~2" "%~1GameData (%~4)\SidEx\%PROJECT%"
xcopy /E /Y "%~2*" "%~1GameData (%~4)\SidEx\%PROJECT%\"

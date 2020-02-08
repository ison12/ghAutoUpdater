@echo off

rem Args check
if "%~1" == "" (
	exit /b 1
)

if "%~2" == "" (
	exit /b 2
)

rem Variable declare
set curPath=%~dp0
set applicationDirPath=%~1
set downloadDirPath=%~2
set srcDirPath=
set nowDate=%date:~0,4%%date:~5,2%%date:~8,2%
set nowTime=%time:~0,2%%time:~3,2%%time:~6,2%

rem Install files check
if exist "%downloadDirPath%\CliboDone\CliboDone\CliboDone.exe" (
	set srcDirPath=%downloadDirPath%\CliboDone\CliboDone
) else if exist "%downloadDirPath%\CliboDone\CliboDone.exe" (
	set srcDirPath=%downloadDirPath%\CliboDone
) else (
	exit /b 3
)

rem Copy module files
copy /b /v /y "%srcDirPath%\*.exe" "%applicationDirPath%"
copy /b /v /y "%srcDirPath%\*.exe.config" "%applicationDirPath%"

rem Copy dll files (If not exists dll then no check)
copy /b /v /y "%srcDirPath%\*.dll" "%applicationDirPath%"

rem Copy manual
rd /s /q "%applicationDirPath%\Manual"
move /y "%srcDirPath%\Manual" "%applicationDirPath%\Manual"

rem Backup convert scripts
robocopy /s /e "%applicationDirPath%\ConvertScripts" "%applicationDirPath%\ConvertScripts_bk_%date%%time%"

rem Copy convert scripts (Newer or Changed)
robocopy /s /e /xo "%srcDirPath%\ConvertScripts" "%applicationDirPath%\ConvertScripts"

rem Success
exit /b 0

@echo on
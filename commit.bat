@echo off
cd /d %~dp0
git add .
for /f "tokens=2 delims==" %%I in ('"wmic os get localdatetime /value"') do if not defined datetime set datetime=%%I
set commitMsg=%datetime:~0,4%-%datetime:~4,2%-%datetime:~6,2% %datetime:~8,2%:%datetime:~10,2%:%datetime:~12,2%
git commit -m "%commitMsg%"
git push origin main
pause

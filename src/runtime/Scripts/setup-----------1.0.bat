echo '正在安装......'

set appPath="C:\Spider\"
set appBinPath="C:\Spider\bin"
set deskTopPath= "%USERPROFILE%\Desktop"

REM ----install cai niao-----------
CaiNiaoSetUp.exe /q


if exist "C:\WINDOWS\system32\vcruntime140.dll" set vc2015='true'
REM echo %vc2015%
if not defined vc2015 vc.exe /q

REM --------kill cainiao  if it is excuting.........
tasklist /fi "imagename eq CNPrintMonitor.exe" |find ":" > nul
if errorlevel 1 taskkill /f /im "CNPrintMonitor.exe"

tasklist /fi "imagename eq CNPrintClient.exe" |find ":" > nul
if errorlevel 1 taskkill /f /im "CNPrintClient.exe"


if exist "C:\Spider\bin\hook.exe" C:\Spider\bin\hook.exe -remove


if exist "C:\Spider\" rd /s /q %appPath%


mkdir %appPath%
7z x MonoVM.7z -aoa -o%appPath% 



C:\Spider\bin\hook.exe -install


echo '安装完毕!'

REM pause
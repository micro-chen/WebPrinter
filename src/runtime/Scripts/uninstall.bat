echo '开始卸载......'

hook.exe -remove
tasklist /fi "imagename eq MonoVM.exe" |find ":" > nul
if errorlevel 1 taskkill /f /im "MonoVM.exe"

rd   /s /q app
del hook.exe
del mono-2.0.dll
del mono-sgen.exe
del MonoVM.exe

cd ../

rd  /s /q  etc
rd  /s /q  lib


echo '卸载完毕!'
pause
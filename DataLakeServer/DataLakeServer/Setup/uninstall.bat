net stop "DataLakeServerWatcher" 
"%cd%\InstallUtil.exe" "%cd%\DataLakeServerWatcher.exe"  -u
taskkill /f /im DataLakeServerWatcher.exe
pause
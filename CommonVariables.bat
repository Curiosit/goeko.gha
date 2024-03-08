@echo off

REM VsDevTools might need to be adapted according your installation of Visual Studio 
set "VsDevTools=E:\Programy\vscom2022\Common7\Tools\VsDevCmd.bat"

REM where to find yak
set "YakExecutable=C:\Program Files\Rhino 7\System\yak.exe"

REM name of the solution to build
set "Name=goeko"

REM where to copy resulting yak packages
set "YakTargetDir=bin\packages"


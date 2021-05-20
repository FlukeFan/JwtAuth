@CD /D "%~dp0"
@title AuthEx Command Prompt
@SET PATH=C:\Program Files\dotnet\;%PATH%
@SET PATH=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\;%PATH%
@REM type readme.md
@echo.
@echo Aliases:
@echo.
@doskey /MACROS
%comspec%

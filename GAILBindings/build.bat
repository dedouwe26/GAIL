@echo off
cd bin
cl.exe /Zi /EHsc /nologo /MT /Fe%cd%\..\bin\main.exe %cd%\..\main.cpp %cd%\..\lib\glfw\lib\glfw3_mt.lib %cd%\..\lib\glad\lib\glad.c User32.lib Gdi32.lib Shell32.lib
main.exe
cd ..
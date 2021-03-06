@echo off

REM ================================
REM ABSOFTWARE SCRIPT FOR CODE COVERAGE
REM SIMPLY CHANGE THE VARIABLES UNDER "CHANGE THESE:"
REM The "testingCode" is the namespace that actually does the testing (e.g. ABSoftware.ABSave.Tests.UnitTests)
REM The "mainCode" is the actual code we're interested in.
REM You can use stars as wildcards in "testingCode" and "mainCode".
REM ================================

REM ================================
REM CHANGE THESE:
REM ================================
SET vsVer=2019
SET openCoverPath=I:\Tools\OpenCover\opencover.console.exe
SET reportGeneratorPath=I:\Tools\ReportGenerator_4.2.9\net47\ReportGenerator.exe

SET testDLLPath=..\ABSoftware.ABSave.Tests.UnitTests\bin\Debug\ABSoftware.ABSave.Tests.UnitTests.dll
SET testingCode=ABSoftware.ABSave.Tests*
SET mainCode=ABSoftware.ABSave*

REM ================================
REM ACTUAL SCRIPT:
REM ================================
SET /P title="Give a title: "

REM Create the "title" directory (since OpenCover won't do that)
mkdir %title%

REM Run OpenCover
"%openCoverPath%" ^
-register:user ^
-target:"C:\Program Files (x86)\Microsoft Visual Studio\%vsVer%\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ^
-targetargs:"%testDLLPath%" ^
-filter:"+[%mainCode%]* -[%testingCode%]*" ^
-mergebyhash ^
-skipautoprops ^
-output:"%title%\coverage.xml" > "%title%\TestResults.txt"

REM Run the ReportGenerator
"%reportGeneratorPath%" -reports:"%title%\coverage.xml" -targetdir:"%title%\Coverage"

REM Remove the "TestResults" directory (generated by VSTest)
rmdir /s /q TestResults

REM Finally, open up the report!
START "REPORT: %title%" "%title%\Coverage\index.htm"
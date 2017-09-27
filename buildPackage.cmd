@call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat"

MSBuild Aliyun.LOGSDK.log4net\Aliyun.LOGSDK.log4net.csproj /t:Clean;Rebuild /p:Configuration=Release
.nuget\nuget.exe pack Aliyun.LOGSDK.log4net.nuspec /o releases

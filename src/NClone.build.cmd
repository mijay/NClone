setlocal

set MsBuild="%SystemRoot:"=%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
set NuGet="packages\NuGet.CommandLine.2.7.1\tools\NuGet.exe"

%MsBuild% NClone.sln /t:Clean,Build /p:Configuration=Release
%NuGet% pack NClone.nuspec
REM generateFromRelease.bat tag message username token
REM example: generateFromRelease.bat v1.0.10 "Library update" xxx-xx ghp_xxx

nuget pack ../RfemSoapWsClient/RFEMWebServiceLibrary.nuspec
nuget pack ../RstabSoapWsClient/RSTABWebServiceLibrary.nuspec
FOR %%I in (*.nupkg) DO nuget push %%I xxxx -Source https://api.nuget.org/v3/index.json
del *.nupkg

git clone https://github.com/Dlubal-Software/Dlubal_CSharp_Client.git
cd Dlubal_CSharp_Client
git pull
xcopy ..\..\RfemSoapWsClient\*.cs source_code\RfemSoapWsCoreClient\ /Y
xcopy ..\..\RstabSoapWsClient\*.cs source_code\RstabSoapWsCoreClient\ /Y

cd source_code
cd RfemRstabSoapWsCoreLib
dotnet build --configuration "Release RFEM" --framework net5.0
cd ../RfemSoapWsCoreClient
dotnet build --configuration Release --framework net5.0
dotnet build --configuration Release --framework net48
cd ../RstabSoapWsCoreClient
dotnet build --configuration Release --framework net5.0
dotnet build --configuration Release --framework net48
cd ..\..

git commit -a -m %2
git push https://%3:%4@github.com/Dlubal-Software/Dlubal_CSharp_Client.git

cd ..

mkdir .\Binaries
mkdir .\Binaries\RfemSoapWsCoreClient_net5.0
mkdir .\Binaries\RFEM
mkdir .\Binaries\RFEM\net5.0
mkdir .\Binaries\RFEM\net48
mkdir .\Binaries\RSTAB
mkdir .\Binaries\RSTAB\net5.0
mkdir .\Binaries\RSTAB\net48
xcopy "Dlubal_CSharp_Client\source_code\RfemRstabSoapWsCoreLib\bin\Release RFEM\net5.0\RfemRstabSoapWsCoreLib.dll" "Binaries\RfemSoapWsCoreClient_net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RfemRstabSoapWsCoreLib\bin\Release RFEM\net5.0\RfemRstabSoapWsCoreLib.pdb" "Binaries\RfemSoapWsCoreClient_net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RfemSoapWsCoreClient\bin\Release\net5.0\RfemSoapWsCoreClient.dll" "Binaries\RFEM\net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RfemSoapWsCoreClient\bin\Release\net5.0\RfemSoapWsCoreClient.pdb" "Binaries\RFEM\net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RfemSoapWsCoreClient\bin\Release\net48\RfemSoapWsCoreClient.dll" "Binaries\RFEM\net48" /Y
xcopy "Dlubal_CSharp_Client\source_code\RfemSoapWsCoreClient\bin\Release\net48\RfemSoapWsCoreClient.pdb" "Binaries\RFEM\net48" /Y
xcopy "Dlubal_CSharp_Client\source_code\RstabSoapWsCoreClient\bin\Release\net5.0\RstabSoapWsCoreClient.dll" "Binaries\RSTAB\net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RstabSoapWsCoreClient\bin\Release\net5.0\RstabSoapWsCoreClient.pdb" "Binaries\RSTAB\net5.0" /Y
xcopy "Dlubal_CSharp_Client\source_code\RstabSoapWsCoreClient\bin\Release\net48\RstabSoapWsCoreClient.dll" "Binaries\RSTAB\net48" /Y
xcopy "Dlubal_CSharp_Client\source_code\RstabSoapWsCoreClient\bin\Release\net48\RstabSoapWsCoreClient.pdb" "Binaries\RSTAB\net48" /Y
tar.exe -a -cf Binaries.zip Binaries
rmdir /s/q .\Binaries

cd Dlubal_CSharp_Client

echo %3 > ..\token.txt
powershell Get-Content ..\token.txt | ..\gh auth login --with-token
del ..\token.txt
..\gh release create %1 --notes %2 ..\Binaries.zip
del ..\Binaries.zip

cd ..

rmdir /s/q .\Dlubal_CSharp_Client
function Get-Application-Version {
    $Body = '<x:Envelope
xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:rfe="http://www.dlubal.com/rfem.xsd">
<x:Header/>
<x:Body>
    <rfe:get_information></rfe:get_information>
</x:Body>
</x:Envelope>'
    $Response = Invoke-WebRequest -Credential $Credential -Uri http://localhost:8081 -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $Body -ContentType application/xml
    # $Response.Content
    [xml]$bn = ([xml]$Response.Content)
    $ApplicationType = $bn.GetElementsByTagName("type").innertext;
    $ApplicationVersion = $bn.GetElementsByTagName("version").innertext;
    $ApplicationLanguage = $bn.GetElementsByTagName("language_name").innertext;
    Write-Host "Runnig" $ApplicationType $ApplicationVersion $ApplicationLanguage
}

function Reset-Model {
    param (
        $ModelUrl
    )
    $Reset = '<x:Envelope
xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:rfe="http://www.dlubal.com/rfem.xsd">
<x:Header/>
<x:Body>
<rfe:reset></rfe:reset>
</x:Body>
</x:Envelope>'
    $ResponseTests = Invoke-WebRequest -Credential $Credential -Uri $ModelUrl -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $Reset -ContentType application/xml
}

function Create-New-Model {
    param (
        $ModelName
    )
    $BodyNewModel = '<x:Envelope
    xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:rfe="http://www.dlubal.com/rfem.xsd">
    <x:Header/>
    <x:Body>
        <rfe:new_model>
            <rfe:model_name>'+ $ModelName + '</rfe:model_name>
        </rfe:new_model>
    </x:Body>
</x:Envelope>'


    $ResponseNewModel = Invoke-WebRequest -Credential $Credential -Uri http://localhost:8081 -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $BodyNewModel -ContentType application/xml
    # $StatusCode = $ResponseNewModel.StatusCode
    # $StatusCode
    # $ResponseNewModel.Content
    [xml]$bn = ([xml]$ResponseNewModel.Content)
    $modelURL = $bn.GetElementsByTagName("value").innertext;
    Write-Output $ModelURL
}

function Run-Script {
  
    param (
        [Parameter(Mandatory = $true, Position = 0)]    
        [System.Uri]$ModelURL,
        [Parameter(Mandatory = $true, Position = 1)]
        [string[]]$Script
    )


    $RunScript = '<x:Envelope
xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
xmlns:rfe="http://www.dlubal.com/rfem.xsd">
<x:Header/>
<x:Body>
    <rfe:run_script>
        <rfe:script_file_path>' + $Script + '</rfe:script_file_path>
    </rfe:run_script>
</x:Body>
</x:Envelope>'

    $ResponseTests = Invoke-WebRequest -Credential $Credential -Uri $ModelURL -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $RunScript -ContentType application/xml
    $StatusCode = $ResponseTests.StatusCode
    $StatusCode 
}

function Close-Model {
    param (
        [Parameter(Mandatory = $true, Position = 0)]    
        [System.Uri]$ModelId,
        [Parameter(Mandatory = $true, Position = 1)]
        [bool]$Save
    )
    $Body = '<x:Envelope
    xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:rfe="http://www.dlubal.com/rfem.xsd">
    <x:Header/>
    <x:Body>
        <rfe:close_model>
            <rfe:index>'+ $ModelId + '</rfe:index>
            <rfe:save_changes>'+ $Save.ToString() + '</rfe:save_changes>
        </rfe:close_model>
    </x:Body>
</x:Envelope>'
    $ResponseTests = Invoke-WebRequest -Credential $Credential -Uri http://localhost:8081 -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $Body -ContentType application/xml
    $StatusCode = $ResponseTests.StatusCode
    $StatusCode 
}

function Open-Model {
    param (
        [System.Uri]$URL,
        [string]$File
    )
    $Body = '<x:Envelope
    xmlns:x="http://schemas.xmlsoap.org/soap/envelope/"
    xmlns:rse="http://www.dlubal.com/rsection.xsd">
    <x:Header/>
    <x:Body>
        <rse:open_model>
            <rse:model_path>'+ $File + '</rse:model_path>
        </rse:open_model>
    </x:Body>
</x:Envelope>'
    $Response = Invoke-WebRequest -Credential $Credential -Uri $URL -Headers (@{SOAPAction = 'Read' }) -Method Post -Body $Body -ContentType application/xml
    [xml]$bn = ([xml]$Response.Content)
    $modelURL = $bn.GetElementsByTagName("value").innertext;
    Write-Output $ModelURL
}

function Download-WSDL-File {
    param (
        [System.Uri]$URL,
        [string]$OutputFile
    )
    Invoke-WebRequest -Uri $URL -OutFile $OutputFile
    if (-not(Test-Path $OutputFile -PathType Leaf)) {
        Write-Host "The file [$OutputFile] has not been downloaded."
    }
    else {
        Write-Host "The file [$OutputFile] has been downloaded."
    }
}

Get-Application-Version
Download-WSDL-File http://localhost:8081/wsdl  "D:/TEMP/RfemApplication.wsdl"
$modelURL = Open-Model "http://localhost:8081/" "D:\OneDrive - Dlubal Software\MODELS\RFEM\RFEM6\AllAddOns.rf6"
$modelURL = $modelURL + 'wsdl'
Download-WSDL-File $modelURL  "D:/TEMP/RfemApplicationClient.wsdl"

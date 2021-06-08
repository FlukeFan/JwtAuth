#Requires -RunAsAdministrator

param(
    [string] $siteName = "jwtauth.rgbco.uk"
)

$ErrorActionPreference = "Stop"

Import-Module WebAdministration
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force

function Unzip($folder) {
    $zipFile = "$folder\publish.zip"
    if (Test-Path $zipFile) {
        Write-Host "unziping $zipFile"
        Expand-Archive -Path $zipFile -DestinationPath $folder -Force
        if (Test-Path "$folder\publish_unzipped.zip") {
            Remove-Item "$folder\publish_unzipped.zip" -Force
        }
        Rename-Item -Path $zipFile -NewName "publish_unzipped.zip"
    }
}

function SetupSite($siteName, $path) {

    $sitePool = Get-IISAppPool -Name $siteName
    if ($sitePool -eq $null) {
        Write-Host "Creating IIS pool $siteName"
        Reset-IISServerManager -Confirm:$False
        New-WebAppPool -Name $siteName
    }

    $site = Get-IISSite -Name $siteName
    if ($site -eq $null) {
        Write-Host "Creating IIS site $siteName"
        Reset-IISServerManager -Confirm:$False
        New-IISSite -Name $siteName -PhysicalPath $path -BindingInformation "*:80:$siteName"
    }

    Set-ItemProperty "IIS:\Sites\$siteName" -Name "PhysicalPath" -Value $path
    Set-ItemProperty "IIS:\Sites\$siteName" -Name "applicationPool" -Value $siteName
    Set-ItemProperty "IIS:\AppPools\$siteName" -Name recycling.disallowOverlappingRotation -Value True
    Set-ItemProperty "IIS:\AppPools\$siteName" -Name "processModel.identityType" -Value 0
}

function SetupApp($siteName, $appName, $path) {

    $poolName = "$siteName.$appName"
    $pool = Get-IISAppPool -Name $poolName
    if ($pool -eq $null) {
        Write-Host "Creating IIS pool $poolName"
        Reset-IISServerManager -Confirm:$False
        New-WebAppPool -Name $poolName
    }

    $app = Get-WebApplication -Site $siteName -Name $appName
    if ($app -eq $null) {
        Write-Host "Creating app $appName"
        Reset-IISServerManager -Confirm:$False
        New-WebApplication -Site $siteName -Name $appName -PhysicalPath $path
    }

    Set-ItemProperty "IIS:\Sites\$siteName\$appName" -Name "applicationPool" -Value $poolName
    Set-ItemProperty "IIS:\AppPools\$poolName" -Name "recycling.disallowOverlappingRotation" -Value True
    Set-ItemProperty "IIS:\AppPools\$poolName" -Name "processModel.identityType" -Value 0
}

Unzip "home"
Unzip "security"
Unzip "mvc"

Reset-IISServerManager -Confirm:$False
SetupSite $siteName "$((Get-Location).Path)\home"
SetupApp $siteName "Security" "$((Get-Location).Path)\security"
SetupApp $siteName "Mvc" "$((Get-Location).Path)\mvc"

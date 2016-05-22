########################
# THE BUILD!
########################
Push-Location $PSScriptRoot

function Invoke-DotNetBuild
{
  [cmdletbinding()]
  param([string] $DirectoryName)
  & dotnet build ("""" + $DirectoryName + """") -c Release; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Invoke-Tests
{
  [cmdletbinding()]
  param([string] $DirectoryName)
  & dotnet test ("""" + $DirectoryName + """") -c Release; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Invoke-DotNetPack
{
  [cmdletbinding()]
  param([string] $DirectoryName)
  & dotnet pack ("""" + $DirectoryName + """") -c Release -o .\artifacts\packages; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Remove-PathVariable
{
  [cmdletbinding()]
  param([string] $VariableToRemove)
  $path = [Environment]::GetEnvironmentVariable("PATH", "User")
  $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
  [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
  $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
  $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
  [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
}

# Prepare the dotnet CLI folder
$env:DOTNET_INSTALL_DIR="$(Convert-Path "$PSScriptRoot")\.dotnet\win7-x64"
if (!(Test-Path $env:DOTNET_INSTALL_DIR))
{
  mkdir $env:DOTNET_INSTALL_DIR | Out-Null
}

# Download the dotnet CLI install script
if (!(Test-Path .\dotnet\install.ps1))
{
  Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" -OutFile ".\.dotnet\dotnet-install.ps1"
}

# Run the dotnet CLI install
& .\.dotnet\dotnet-install.ps1

# Add the dotnet folder path to the process. This gets skipped
# by Install-DotNetCli if it's already installed.
Remove-PathVariable $env:DOTNET_INSTALL_DIR
$env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"

# Set build number
$env:DOTNET_BUILD_VERSION = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1}[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
Write-Host "Build number:" $env:DOTNET_BUILD_VERSION

# Clean
if(Test-Path .\artifacts) 
{ 
	Remove-Item .\artifacts -Force -Recurse 
}

# Package restore
& dotnet restore

# Build/package
Get-ChildItem -Path .\src -Filter *.xproj -Recurse | ForEach-Object { Invoke-DotNetPack $_.DirectoryName }
Get-ChildItem -Path .\samples -Filter *.xproj -Recurse | ForEach-Object { Invoke-DotNetBuild $_.DirectoryName }

# Test
Get-ChildItem -Path .\tests -Filter *.xproj -Recurse | ForEach-Object { Invoke-Tests $_.DirectoryName }

Pop-Location

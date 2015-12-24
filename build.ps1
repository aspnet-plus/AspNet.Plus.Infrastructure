########################
# FUNCTIONS
########################
function Install-Dnvm
{
    & where.exe dnvm 2>&1 | Out-Null
    if(($LASTEXITCODE -ne 0) -Or ((Test-Path Env:\APPVEYOR) -eq $true))
    {
        Write-Host "DNVM not found"
        &{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

        # Normally this happens automatically during install but AppVeyor has
        # an issue where you may need to manually re-run setup from within this process.
        if($env:DNX_HOME -eq $NULL)
        {
            Write-Host "Initial DNVM environment setup failed; running manual setup"
            $tempDnvmPath = Join-Path $env:TEMP "dnvminstall"
            $dnvmSetupCmdPath = Join-Path $tempDnvmPath "dnvm.ps1"
            & $dnvmSetupCmdPath setup
        }
    }
}

Install-Dnvm

# Install DNX
dnvm update-self
dnvm upgrade

dnvm uninstall 1.0.0-beta8 -r clr -a x86
dnvm uninstall 1.0.0-beta8 -r clr -a x64
dnvm uninstall 1.0.0-beta8 -r coreclr -a x86
dnvm uninstall 1.0.0-beta8 -r coreclr -a x64

dnvm install 1.0.0-rc1-final -r clr -a x86
dnvm install 1.0.0-rc1-final -r clr -a x64
dnvm install 1.0.0-rc1-final -r coreclr -a x86
dnvm install 1.0.0-rc1-final -r coreclr -a x64
dnvm install 1.0.0-rc1-update1 -r clr -a x86
dnvm install 1.0.0-rc1-update1 -r clr -a x64
dnvm install 1.0.0-rc1-update1 -r coreclr -a x86
dnvm install 1.0.0-rc1-update1 -r coreclr -a x64

dnvm list 
npm cache clean
dnu restore


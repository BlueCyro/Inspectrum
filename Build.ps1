dotnet build

$dir = Split-Path -Path (Get-Location) -Leaf
# Edit $NeosDir to be the path to the neos directory on your own system
$NeosDir = "C:\Neos\app\"
$NeosExe = "$NeosDir\Neos.exe"
$AssemblyLocation = "$(Get-Location)\bin\Debug\net4.7.2\$dir.dll"
$nml_mods = "$NeosDir\nml_mods\"

Copy-Item -Force -Path $AssemblyLocation -Destination $nml_mods

$LogJob = Start-Job {Start-Sleep -Seconds 8
    Get-Content "C:\Neos\app\Logs\$(Get-ChildItem -Path C:\Neos\app\Logs | Sort-Object LastWriteTime | Select-Object -last 1)" -Wait
}

$NeosProc = Start-Process -FilePath $NeosExe -WorkingDirectory $NeosDir -ArgumentList "-DontAutoOpenCloudHome", "-SkipIntroTutorial", "-Screen", "-LoadAssembly `"C:\Neos\app\Libraries\NeosModLoader.dll`"" -passthru

while(!$NeosProc.HasExited) {
    Start-Sleep -Seconds 1
    Receive-Job $LogJob.Id
}

Stop-Job $LogJob.Id
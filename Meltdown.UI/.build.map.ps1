. $PSScriptRoot/.config-utils.ps1

$targets = @{
    "build"          = {
        param($ctx, [bool][switch]$noRestore)

        $a = @()
        if ($noRestore) {
            $a += "--no-restore"
        }
        dotnet build @a
    }

    "pack"           = {
        param($ctx, [ValidateSet("debug", "release")] $configuration = "debug")

        $versionSuffix = get-xmlconfig "$psscriptroot/src/Meltdown.UI.csproj" -Path "Project/PropertyGroup/VersionSuffix"
        if ($versionSuffix -match "(?<part1>.*?)(?<number>\d+)$") {
            $number = $matches["number"]
            $number = [int]$number + 1
            $versionSuffix = $matches["part1"] + [string]::Format("{0:D3}", $number)
        }
        set-xmlconfig "$psscriptroot/src/Meltdown.UI.csproj" -Path "Project/PropertyGroup/VersionSuffix" -Value $versionSuffix
        
        write-host "packing with version suffix: $versionSuffix" -ForegroundColor Green
        dotnet pack "$psscriptroot/src" --output "$PSScriptRoot/dist" `
            --configuration $configuration --include-symbols --include-source --verbosity detailed
    }

    "publish"        = {
        param([ValidateSet("nuget", "local")] $source = "local")

        pushd $PSScriptRoot
        try {
            qbuild pack
            $versionPrefix = get-xmlconfig "src/Meltdown.UI.csproj" -Path "Project/PropertyGroup/VersionPrefix"
            $versionSuffix = get-xmlconfig "src/Meltdown.UI.csproj" -Path "Project/PropertyGroup/VersionSuffix"

            $version = "$versionPrefix-$versionSuffix"
            $package = "dist/Meltdown.UI.$version.nupkg"

            switch ($source) {
                "nuget" { 
                    dotnet nuget push $package -s "https://api.nuget.org/v3/index.json"  -k "$env:NUGET_API_KEY"
                }
                "local" {
                    nuget add $package -s "dist/nuget"
                }
            }
        }
        finally {
            popd
        }
    }

    "update-samples" = {
        qbuild publish

        $samplePath = "$PSScriptRoot/samples"
        $sampleProjects = Get-ChildItem -Path $samplePath -Filter *.csproj -Recurse -Depth 1
        foreach ($project in $sampleProjects) {
            $projectPath = $project.FullName
            Write-Host "Updating project: $projectPath" -ForegroundColor Green
            dotnet add $projectPath package "Meltdown.UI" --prerelease -s "$PSScriptRoot/dist/nuget"
            Write-Host "Building project: $projectPath" -ForegroundColor Green
            dotnet build $projectPath --verbosity detailed
        }
    } 

    "build-samples"  = {
        $samplePath = "$PSScriptRoot/samples"
        $sampleProjects = Get-ChildItem -Path $samplePath -Filter *.csproj -Recurse -Depth 1
        foreach ($project in $sampleProjects) {
            $projectPath = $project.FullName
            Write-Host "Building project: $projectPath" -ForegroundColor Green
            dotnet build $projectPath --verbosity detailed
        }
    }

    "clean"          = {
        $binDirs = Get-ChildItem $PSScriptRoot -Filter bin -Recurse -Directory -Depth 3
        $binDirs | ForEach-Object {
            Write-Host "Removing: $($_.FullName)" -ForegroundColor Green
            Remove-Item $_.FullName -Recurse -Force -ErrorAction Continue
        }

        $objDirs = Get-ChildItem $PSScriptRoot -Filter obj -Recurse -Directory -Depth 3
        $objDirs | ForEach-Object {
            Write-Host "Removing: $($_.FullName)" -ForegroundColor Green
            Remove-Item $_.FullName -Recurse -Force -ErrorAction Continue
        }

        $distDirs = Get-ChildItem $PSScriptRoot -Filter dist
        $distDirs | ForEach-Object {
            Write-Host "Removing: $($_.FullName)" -ForegroundColor Green
            Remove-Item $_.FullName -Recurse -Force -ErrorAction Continue
        }
    }

}

return $targets

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

            if ($LASTEXITCODE -ne 0) {
                Write-Host "Error: Nuget push failed" -ForegroundColor Red
                write-host "Please check your NUGET_API_KEY environment variable" -ForegroundColor Red
                exit $LASTEXITCODE
            }
        }
        finally {
            popd
        }
    }

    "update-ink-components" = {
        param([switch][bool] $clean, [switch][bool]$withNodeModules = $false, [string]$linkTo = $null) 
        if ($clean) {
            if (Test-Path "$psscriptRoot/src/CLI/node_modules/ink-components") {
                rm -Recurse -Force $psscriptRoot/src/CLI/node_modules/ink-components
            }
            $withNodeModules = $true
        }
        $inkComponents = "$psscriptroot/../../ink-components"
        if ($withNodeModules) {
            cp $inkComponents "$psscriptRoot/src/CLI/node_modules/" -Force -Recurse
        } else {
            cp "$inkComponents/build" "$psscriptRoot/src/CLI/node_modules/ink-components/build" -Force -Recurse
            cp "$inkComponents/package.json" "$psscriptRoot/src/CLI/node_modules/ink-components/package.json" -Force -Recurse
        }
        if ($linkTo) {
            $buildDir = "$psscriptRoot/src/CLI/node_modules/ink-components/build"
            rm -r $buildDir -Verbose
            New-Junction -path $buildDir -target "$linkTo/build" -Verbose
        }
        cp "$inkComponents/samples/cli-sample/*" "$psscriptRoot/src/CLI" -Verbose -Force -Recurse
        

        $deps = Get-ChildItem "$PSScriptRoot/src/CLI/dependencies" -File
        $deps | %{
            $name = $_.BaseName
            "export * from `"$name`";" | Out-File "$PSScriptRoot/src/CLI/dependencies/$name.ts" -Force
        }
    }

    "update-samples" = {
        qbuild publish

        $samplePath = "$PSScriptRoot/samples"
        $sampleProjects = Get-ChildItem -Path $samplePath -Filter *.csproj -Recurse -Depth 1
        foreach ($project in $sampleProjects) {
            $projectPath = $project.FullName
            $csproj = (Get-Content $projectPath) | Out-String
            if ($csproj -notmatch "<PackageReference Include=""Meltdown.UI""") {
                write-host "Skipping project: $projectPath because it doesn't containt package reference to Meltdown.UI" -ForegroundColor Yellow
                continue
            } 
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

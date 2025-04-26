$targets = @{
    "build" = {
        param($ctx, [bool][switch]$noRestore)

        $a = @()
        if ($noRestore) {
            $a += "--no-restore"
        }
        dotnet build @a
    }
    "pack" = {
        param($ctx)

        dotnet pack --output "$PSScriptRoot/dist" --version-suffix "beta01"
    }

    "publish" = {
        param($ctx)
        qbuild "pack"
        dotnet nuget push "$PSScriptRoot/dist/" -s https://api.nuget.org/v3/index.json -k "$env:NUGET_API_KEY"--skip-duplicate
    }
}

return $targets

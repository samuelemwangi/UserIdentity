if ($args -contains "-useLocal") {

    docker compose -f docker-compose.yml up -d --scale useridentity=0

    # Check for env files with better error handling
    $fileName = ".env"
    $exampleEnvPath = "example.env"
    
    if (Test-Path $fileName) {
    } 
    elseif (Test-Path $exampleEnvPath) {
        Copy-Item $exampleEnvPath $fileName
    }
    else {
        Write-Error "No .env or example.env file found. Please create one."
        exit 1
    }

    Get-Content $fileName | ForEach-Object {
        if ($_ -match "=") {
            $envVar = $_ -split "=", 2  # Split on first = only
            $envVarName = $envVar[0]
            $envVarValue = $envVar[1]
            Set-Item -Path env:$envVarName -Value $envVarValue
        }
    }
        
    # wait for the database to be ready
    Start-Sleep -s 20    

    # dotnet run --urls=http://localhost:5000 --project .\UserIdentity\UserIdentity.csproj
}
elseif ($args -contains "-build") {

    docker compose -f docker-compose.yml up --build -d 
}
else {
    
    docker compose -f docker-compose.yml up -d 
}
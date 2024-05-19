if ($args -contains "-useLocal") {

    docker compose -f docker-compose.build.yml up -d --scale useridentity=0

    $fileName = (Test-Path .env) ? ".env" : "example.env"

    Get-Content $fileName | ForEach-Object {
        $envVar = $_ -split "="
        $envVarName = $envVar[0]
        $envVarValue = $envVar[1]
        Set-Item -Path env:$envVarName -Value $envVarValue            
    }
        
    # wait for the database to be ready
    Start-Sleep -s 20    

    dotnet run --urls=http://localhost:5000 --project .\UserIdentity\UserIdentity.csproj
}
elseif ($args -contains "-build") {

    docker compose -f docker-compose.build.yml up --build -d 
}
else {
    
    docker compose -f docker-compose.build.yml up -d 
}

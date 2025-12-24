if ($args -contains "-useLocal") {

    docker compose -f docker-compose.yml up -d --scale useridentity=0
  
    # wait for the database to be ready
    Start-Sleep -s 20    

    dotnet run --urls=http://localhost:5050 --project .\UserIdentity\UserIdentity.csproj
}
elseif ($args -contains "-build") {

    docker compose -f docker-compose.yml up --build -d 
}
else {
    
    docker compose -f docker-compose.yml up -d 
}
# Summary 
Useridentity is an easy-to-use RESTful API that simplifies user management by providing a set of endpoints for managing user identities, creating user accounts, and handling role management. 

## Running the App
### Prerequisites
- MYSQL Database 
- Dotnet 9.0 SDK
- Docker 
- Visual Studio (optional)
- An Ed25519 key pair for JWT signing and verification. You can generate a key pair using the following commands (replace the passphrase with a strong one of your choice):
```
    openssl genpkey -algorithm ED25519 -out ed25519-private.pem -pass pass:S3T@G00dPa22Phr@S3
    openssl pkey -pubout -in ed25519-private.pem -out ed25519-public.pem -passin pass:S3T@G00dPa22Phr@S3
 
```

Ensure to (at least) set two environment variables using the keys  `EDDSA_PRIVATE_KEY` and `EDDSA_PUBLIC_KEY` for the generated ed25519-private.pem and pued25519-public.pem keys' content.

 - Postman (optional)

### Instructions

#### Running the App Locally
1. Clone the repository
2. Set the required environment variables. This can be done in project properties (on Visual Studio) or launchSettings.json
   
``` 
    KeySetOptions__PrivateKeyPath=EDDSA_PRIVATE_KEY
    KeySetOptions__PublicKeyPath=EDDSA_PUBLIC_KEY
    KeySetOptions__PrivateKeyPassPhrase=<Your Good Passphrase>
```
3. Run the docker-compose file using `docker compose -f docker-compose.yml up -d --scale useridentity=0` to start the database and redpanda dependencies.
4. Run the application using Visual Studio or the command line with `dotnet run` from the project directory.
5. Use the included Postman collection and environment to test the endpoints.

#### Running the App Using Docker Compose
1. Clone the repository
2. `cd` into the cloned folder
3. Run the docker-compose file using `docker compose -f docker-compose.yml up --build -d`
4. Use the included Postman collection and environment to test the endpoints

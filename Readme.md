# Summary 
Useridentity is an easy-to-use RESTful API that simplifies user management by providing a set of endpoints for managing user identities, creating user accounts, and handling role management. You can easily create and update user accounts, and assign or revoke roles for each user. 

## Running the App
### Prerequisites
- MYSQL Database 
- Dotnet 8.0 SDK
- Visual Studio 2022 (optional)
- Docker (optional)

#### Running the App Locally
1. Clone the repository
2. Set the required environment variables. This can be done in project properties (on Visual Studio) or launchSettings.json
   
```
    ASPNETCORE_ENVIRONMENT=Development
    MysqlSettings__Host=db
    MysqlSettings__Port=3306
    MysqlSettings__Database=test_db
    MysqlSettings__UserName=test_user
    MysqlSettings__Password=tesT*_p@ss12
    JwtIssuerOptions__Issuer=appv1
    JwtIssuerOptions__ValidForSeconds=600
    JwtIssuerOptions__Audience=https://test.app.com
    KeySetOptions__KeyId=key1
    DefaultResetPasswordMessage=If we have an account matching your email address, you will receive an email with a link to reset your password.
    KeySetOptions__PrivateKeyPath=../certs/ed25519-private.pem
    KeySetOptions__PublicKeyPath=../certs/ed25519-public.pem
    KeySetOptions__PrivateKeyPassPhrase=S3T@G00dPa22Phr@S3
    ApiKeySettings__ApiKey=kzqGLgkdDGP34/Kxaw4urndoEumbIDxwHSMj/8zvbsw=
    CorsSettings__AllowedOrigins=*
```
3. Run the app
4. Use the included Postman collection and environment to test the endpoints

#### Running the App Using Docker Compose
1. Clone the repository
2. `cd` into the cloned folder
3. Rename the `example.env` file to `.env` file and update/customize the required variables
3. Run the docker-compose file using `docker compose -f docker-compose.yml up --build -d`
4. Use the included Postman collection and environment to test the endpoints

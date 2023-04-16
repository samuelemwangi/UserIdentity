# Summary 
Useridentity is an easy-to-use RESTful API that simplifies user management by providing a set of endpoints for managing user identities, creating user accounts, and handling role management. You can easily create and update user accounts, and assign or revoke roles for each user. 

## Running the App
### Prerequisites
- MYSQL Database 
- Dotnet 6.0 SDK
- Visual Studio 2022 (optional)
- Docker (optional)

#### Running the App Locally
1. Clone the repository
2. Set the required environment variables (On visual studio, you can set them in the project properties)
   
```
    DB_SERVER=localhost
    DB_PORT=3306
    DB_NAME=user_indentity_db
    DB_USER={Your DB User}
    DB_PASSWORD={Your DB Password}
    APP_ISSUER=appv1
    APP_VALID_FOR=10
    APP_AUDIENCE=https://test.app.com
    APP_KEY_ID=V1APP_KEY_IDV1
    APP_SECRET_KEY=c8pgjBdXIJtjbaEAvzEt
    APP_DEFAULT_ROLE=Default
    APP_DEFAULT_RESET_PASSWORD_MESSAGE=If we have an account matching your email address, you will receive an email with a link to reset your password.
```

3. Run the app
4. Use the included postman collection and environment to test the endpoints

#### Buidling and Running the App on Docker
1. Clone the repository
2. `cd` into the cloned folder
3. Build the docker image using `docker build -t useridentity -f .\UserIdentity\Dockerfile .`
4. Run the docker container using `docker run -p 5000:80 --name useridentity -e DB_SERVER=host.docker.internal -e DB_PORT=3306 -e DB_NAME=user_indentity_db -e DB_USER={Your DB User} -e DB_PASSWORD={Your DB Password} -e APP_ISSUER=app.test.issuer -e APP_VALID_FOR=1200 -e APP_AUDIENCE=app.issuer.com -e APP_KEY_ID=AppKeyID -e APP_SECRET_KEY=c8pgjBdXIJtjbaEAvzEt -e APP_DEFAULT_ROLE=Default -e APP_DEFAULT_RESET_PASSWORD_MESSAGE="This is a sample message" useridentity` (Replace the environment variables with the values you want to use)
5. Use the included postman collection and environment to test the endpoints

#### Running the App Using Docker Compose
1. Clone the repository
2. `cd` into the cloned folder
3. Run the docker-compose file using `docker-compose up -d` or `docker compose -f docker-compose.build.yml up --build -d` to build the image locally
4. Use the included postman collection and environment to test the endpoints



USE #REPLACE_WITH_YOUR_DATABASE_NAME#;

SET @username = '#REPLACE_WITH_USERNAME#';

SELECT @user_id := id
FROM users
WHERE UserName = @username;

DELETE 
FROM  refresh_tokens
WHERE UserId = @user_id;

DELETE 
FROM user_claims
WHERE UserId = @user_id;

DELETE 
FROM user_logins
WHERE UserId = @user_id;

DELETE 
FROM user_logins
WHERE UserId = @user_id;

DELETE 
FROM user_roles
WHERE UserId = @user_id;

DELETE 
FROM user_tokens
WHERE UserId = @user_id;

DELETE 
FROM user_details
WHERE Id = @user_id;

DELETE 
FROM users
WHERE Id = @user_id;




USE #REPLACE_WITH_YOUR_DATABASE_NAME#;

SET @username = '#REPLACE_WITH_YOUR_USERNAME#';
SET @role_name = '#REPLACE_WITH_YOUR_ADMIN_ROLE_NAME#';

SELECT @role_id := UUID();

SELECT @role_id;

SELECT @user_id := id
FROM users
WHERE UserName = @username;

INSERT IGNORE INTO roles
SET Id = @role_id,
	Name = @role_name,
    NormalizedName = UPPER(@role_name);
   
-- If Role existed - we need to select first
SELECT @role_id := Id
FROM roles
WHERE Name = @role_name;

INSERT IGNORE INTO user_roles
SET UserId = @user_id,
    RoleId = @role_id;




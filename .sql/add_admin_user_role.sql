USE test_db;

SET @username = 'UNIQUE_USERNAME';
SET @role_name = 'super-administrator';

SELECT @role_id := UUID();

SELECT @role_id;

SELECT @user_id := id
FROM users
WHERE User_Name = @username;

INSERT IGNORE INTO roles
SET Id = @role_id,
	Name = @role_name,
    Normalized_Name = UPPER(@role_name);
   
-- If Role existed - we need to select first
SELECT @role_id := Id
FROM roles
WHERE Name = @role_name;

INSERT IGNORE INTO user_roles
SET User_Id = @user_id,
    Role_Id = @role_id;

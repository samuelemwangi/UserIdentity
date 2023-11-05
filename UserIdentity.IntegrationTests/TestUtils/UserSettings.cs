using System;

namespace UserIdentity.IntegrationTests.TestUtils
{
  internal static class UserSettings
  {
    public static Guid UserId = Guid.NewGuid();
    public static String UserEmail = "test.user@domain.com";
    public static String UserName = "test.user";
    public static String FirstName = "Test";
    public static String LastName = "User";
    public static String PhoneNumber = "0123456789";
    public static String UserPassword = "Password123";
    public static String InvalidUserToken = "eyJhbGciOiJIUzI1NiIsImtpZCI6IlFWQlFYMHRGV1Y5SlJBIiwidHlwIjoiSldUIiwiY3R5IjoiSldUIn0.eyJzdWIiOiJ0ZXN0LnVzZXIiLCJqdGkiOiJjNmRhNjY4Yy0xMzEzLTRhOTItOTQzYy1hMzVlMjA3MTYwZWEiLCJpYXQiOjE2Nzg3MDg0NzcsImlkIjoiNWZhODk4MjAtZTViMS00NmY2LWEwZmItYmQwMTk2NGY2NzRmIiwicm9sZXMiOiJBZG1pbmlzdHJhdG9yIiwibmJmIjoxNjc4NzA4NDc3LCJleHAiOjE2Nzg3MDg0OTIsImlzcyI6IklOVkFMSURfSVNTVUVSIiwiYXVkIjoiQVBQX0FVRElFTkNFIn0.Bcyy1ty8Uqa6duR8NrVolF4DlCfkKptWYP_oVwDVC5k";
  }
}

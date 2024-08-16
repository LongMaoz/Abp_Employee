using Microsoft.AspNetCore.Identity;

namespace Config;

public class PasswordHelper
{
    private const int SALT_WORK_FACTOR = 10;
    public static string HashPassword(string password)
    {
        // 生成盐
        string salt = BCrypt.Net.BCrypt.GenerateSalt(SALT_WORK_FACTOR);
        // 使用生成的盐哈希密码
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    public static PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (hashedPassword == null)
        {
            throw new ArgumentNullException(nameof(hashedPassword));
        }
        if (providedPassword == null)
        {
            throw new ArgumentNullException(nameof(providedPassword));
        }
        var areEqual = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

        if (areEqual)
        {
            return PasswordVerificationResult.Success;
        }
        return PasswordVerificationResult.Failed;
    }
}
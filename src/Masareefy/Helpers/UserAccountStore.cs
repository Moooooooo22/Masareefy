using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace Masareefy.Helpers
{
    public static class UserAccountStore
    {
        private const int HashIterations = 600000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Masareefy", "users.db");

        public static bool Register(string userName, string password, out string recoveryCode, out string error)
        {
            recoveryCode = null;
            error = null;
            userName = (userName ?? string.Empty).Trim();
            if (userName.Length < 3 || userName.Length > 32)
            {
                error = "اسم المستخدم يجب أن يكون بين 3 و32 حرفًا.";
                return false;
            }
            if (password == null || password.Length < 8 || password.Length > 128)
            {
                error = "كلمة المرور يجب أن تكون بين 8 و128 حرفًا.";
                return false;
            }

            var passwordSalt = RandomNumberGenerator.GetBytes(SaltSize);
            var passwordHash = Hash(password, passwordSalt);
            recoveryCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var recoverySalt = RandomNumberGenerator.GetBytes(SaltSize);
            var recoveryHash = Hash(recoveryCode, recoverySalt);

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (UserName, PasswordSalt, PasswordHash, RecoverySalt, RecoveryHash) VALUES ($name, $passwordSalt, $passwordHash, $recoverySalt, $recoveryHash);";
                command.Parameters.AddWithValue("$name", userName);
                command.Parameters.AddWithValue("$passwordSalt", Convert.ToBase64String(passwordSalt));
                command.Parameters.AddWithValue("$passwordHash", Convert.ToBase64String(passwordHash));
                command.Parameters.AddWithValue("$recoverySalt", Convert.ToBase64String(recoverySalt));
                command.Parameters.AddWithValue("$recoveryHash", Convert.ToBase64String(recoveryHash));
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
                {
                    recoveryCode = null;
                    error = "اسم المستخدم مستخدم بالفعل.";
                    return false;
                }
            }
        }

        public static bool VerifyCredentials(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || password == null)
                return false;

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT PasswordSalt, PasswordHash FROM Users WHERE UserName = $name COLLATE NOCASE;";
                command.Parameters.AddWithValue("$name", userName.Trim());
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return false;
                    var salt = Convert.FromBase64String(reader.GetString(0));
                    var expectedHash = Convert.FromBase64String(reader.GetString(1));
                    return CryptographicOperations.FixedTimeEquals(Hash(password, salt), expectedHash);
                }
            }
        }

        public static bool ResetPassword(string userName, string recoveryCode, string newPassword, out string error)
        {
            error = null;
            if (newPassword == null || newPassword.Length < 8 || newPassword.Length > 128)
            {
                error = "كلمة المرور الجديدة يجب أن تكون بين 8 و128 حرفًا.";
                return false;
            }

            using (var connection = OpenConnection())
            using (var select = connection.CreateCommand())
            {
                select.CommandText = "SELECT RecoverySalt, RecoveryHash FROM Users WHERE UserName = $name COLLATE NOCASE;";
                select.Parameters.AddWithValue("$name", (userName ?? string.Empty).Trim());
                using (var reader = select.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        error = "اسم المستخدم أو كود الاستعادة غير صحيح.";
                        return false;
                    }
                    var salt = Convert.FromBase64String(reader.GetString(0));
                    var expectedHash = Convert.FromBase64String(reader.GetString(1));
                    if (!CryptographicOperations.FixedTimeEquals(Hash(NormalizeRecoveryCode(recoveryCode), salt), expectedHash))
                    {
                        error = "اسم المستخدم أو كود الاستعادة غير صحيح.";
                        return false;
                    }
                }
            }

            var newSalt = RandomNumberGenerator.GetBytes(SaltSize);
            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Users SET PasswordSalt = $salt, PasswordHash = $hash WHERE UserName = $name COLLATE NOCASE;";
                command.Parameters.AddWithValue("$salt", Convert.ToBase64String(newSalt));
                command.Parameters.AddWithValue("$hash", Convert.ToBase64String(Hash(newPassword, newSalt)));
                command.Parameters.AddWithValue("$name", (userName ?? string.Empty).Trim());
                return command.ExecuteNonQuery() == 1;
            }
        }

        private static SqliteConnection OpenConnection()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath));
            var connection = new SqliteConnection("Data Source=" + DatabasePath);
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users (UserName TEXT NOT NULL COLLATE NOCASE PRIMARY KEY, PasswordSalt TEXT NOT NULL, PasswordHash TEXT NOT NULL, RecoverySalt TEXT NOT NULL, RecoveryHash TEXT NOT NULL);";
                command.ExecuteNonQuery();
            }
            return connection;
        }

        private static byte[] Hash(string value, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(value, salt, HashIterations, HashAlgorithmName.SHA256, HashSize);
        }

        private static string NormalizeRecoveryCode(string code)
        {
            return (code ?? string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).Trim().ToUpperInvariant();
        }
    }
}

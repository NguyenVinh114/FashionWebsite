namespace FashionWebsite.Helpers
{
    public static class PasswordPolicy
    {
        public static bool IsValidPassword(string password, string oldPassword = null)
        {

            if (password.Length < 12)
                return false;

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[!@#$%^&*(),.?\\\":{}|<>]"))
                return false;

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]"))
                return false;

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]"))
                return false;

            if (!string.IsNullOrEmpty(oldPassword) && password == oldPassword)
                return false;


            return true;
        }
    }
}

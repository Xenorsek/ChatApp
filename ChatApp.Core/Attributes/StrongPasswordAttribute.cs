using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Wymagania dotyczące hasła
            bool hasMinimumLength = password.Length >= 8;
            bool hasUpperChar = password.Any(char.IsUpper);
            bool hasLowerChar = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasMinimumLength && hasUpperChar && hasLowerChar && hasDigit && hasSpecialChar;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Password must be at least 8 characters long and include at least one uppercase letter, " +
                   "one lowercase letter, one digit, and one special character.";
        }
    }
}

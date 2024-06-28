using System;
using System.Text.RegularExpressions;

namespace Task.Validators
{
    public static class TaskValidator
    {
        public static (bool isValid, string message) ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return (false, "\nError: Description cannot be empty.\n");
            }
            
           
            if (!Regex.IsMatch(description, @"^[A-z \W]{5,}$"))
            {
                return (false, "\nError: Description should only contain letters,spaces, or punctuation.At least 5 letter's\n");
            }

            return (true, "\nValid description.\n");
        }

        public static (bool isValid, string message) ValidateDueDate(string dueDateStr)
        {
            if (string.IsNullOrWhiteSpace(dueDateStr))
            {
                return (false, "\nError: Date cannot be empty.\n");
            }

            if (!Regex.IsMatch(dueDateStr, @"^\d{4}-\d{2}-\d{2}$"))
            {
                return (false, "\nError: Invalid date format.\n");
            }

            if (!DateTime.TryParseExact(dueDateStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dueDate))
            {
                return (false, "\nError: Invalid date. Please provide a valid date (e.g., yyyy-mm-dd).\n");
            }

            
            if (dueDate.Date < DateTime.Today)
            {
                return (false, "\nError: Past dates are not allowed. Please Enter a future date.\n");
            }

            return (true, "\nValid date.\n");
        }

    }
}

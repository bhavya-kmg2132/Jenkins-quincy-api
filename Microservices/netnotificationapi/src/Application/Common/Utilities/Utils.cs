using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Application.Common.Utilities
{
    public static class Utils
    {
        /// <summary>
        /// Checks if EIN format is valid or not
        /// </summary>
        /// <param name="einVal"></param>
        /// <returns>bool</returns>
        public static bool HasValidEINFormat(string einVal)
        {
            //Regex for valid EIN
            var EIN = new Regex(RegexConstant.ein);

            //If EIN is valid return true
            return EIN.IsMatch(einVal);
        }

        /// <summary>
        /// Checks Date Format
        /// </summary>
        /// <param name="_date"></param>
        /// <returns>bool</returns>
        public static bool HasValidDateFormat(DateTime? _date)
        {
            //If date is null , it is valid
            if (_date == null)
            {
                return true;
            }
            else
            {
                //Else check for the MM/dd/yyyy format
                string inputDate = _date.Value.ToString("MM/dd/yyyy");

                Regex regex1 = new Regex(RegexConstant.date_MMddYYYYWithSlash);

                Regex regex2 = new Regex(RegexConstant.date_MMddYYYYWithDash);

                if (regex1.IsMatch(inputDate) || regex2.IsMatch(inputDate))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                var match = Regex.Match(emailaddress, RegexConstant.email, RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    return false;
                }

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

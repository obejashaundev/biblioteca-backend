using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API
{
    public class Utilities
    {
        public static string FormatString(string? inputValue)
        {
            string formattedString = string.Empty;
            
            if (inputValue is null || inputValue.Length == 0) goto resolve;
            
            formattedString = inputValue;
            
            #region Remove limit spaces
            formattedString = formattedString.TrimEnd();
            formattedString = formattedString.TrimStart();
            #endregion
            
            #region Remove accents
            char[] lsAccents = ['á', 'é', 'í', 'ó', 'ú'];
            char[] lsNoAccents = ['a', 'e', 'i', 'o', 'u'];
            for (int i = 0; i < lsAccents.Length; i++)
            {
                string letterAccent = lsAccents[i].ToString();
                string letterAccentMayuscle = letterAccent.ToUpper();

                string letterNoAccent = lsNoAccents[i].ToString();
                string letterNoAccentMayuscle = letterNoAccent.ToUpper();
                formattedString = formattedString.Replace(letterAccent, letterNoAccent);
                formattedString = formattedString.Replace(letterAccentMayuscle, letterNoAccentMayuscle);
            }
            #endregion

        resolve:
            return formattedString;
        }
    }
}

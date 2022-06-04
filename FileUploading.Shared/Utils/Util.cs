using System.Text.RegularExpressions;

namespace FileUploading.Shared.Utils
{
    public static class Util
    {
        private static readonly Regex rWhiteSpace = new(@"\s+");
        public static string RemoveStringWhiteSpacesAndBrackets(string text)
        {
            var removeBrackets = text.Replace("[", string.Empty).Replace("]", string.Empty);
            var proceed = rWhiteSpace.Replace(text, string.Empty);

            return proceed;
        }

        //public static bool CheckLeadToolsLicenseValidity()
        //{
        //    RasterSupport.SetLicense(Constants.LEAD_TOOLS_LICENCE, Constants.KEY);
        //    if (RasterSupport.KernelExpired)
        //    {
        //        Console.WriteLine("Your license has expired");
        //        return true;
        //    }
        //    else
        //    {
        //        Console.WriteLine("License file set successfully");
        //        return false;
        //    }
        //}
    }
}

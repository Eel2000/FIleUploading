using System.Text.RegularExpressions;

namespace FileUploading.Shared.Utils
{
    public static class Util
    {
        private static readonly Regex rWhiteSpace = new(@"\s+");
        public static string RemoveStringWhiteSpaces(string text)
        {
            return rWhiteSpace.Replace(text, string.Empty);
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

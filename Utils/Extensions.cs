using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Owin_Auth.Utils
{
    public static class Extensions
    {
        private static MD5 md5 = System.Security.Cryptography.MD5.Create();

        public static string MD5(this string s)
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        //Common error reporter
        public static IActionResult ReportError(this ControllerBase controller, Exception e)
        {
            return controller.BadRequest(new {message = $"err.custom {e.GetType().ToString()}:{e.Message}"});
        }

        //NullCheck
        public static bool CheckNull(this string o)
        {
            return o == null || o.Trim().Length == 0;
        }
    }
}
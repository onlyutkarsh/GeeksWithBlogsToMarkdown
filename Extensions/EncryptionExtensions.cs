using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GeeksWithBlogsToMarkdown.Extensions
{
    public static class EncryptionExtensions
    {
        static byte[] entropy = Encoding.Unicode.GetBytes("GWB-F66DF9AA-1401-4068-8C77-27D0890F2F73");

        public static string EncryptString(this SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(this string encryptedData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(encryptedData))
                {
                    byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                                Convert.FromBase64String(encryptedData),
                                entropy,
                                System.Security.Cryptography.DataProtectionScope.CurrentUser);
                    return ToSecureString(Encoding.Unicode.GetString(decryptedData));
                }
                return new SecureString();

            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(this string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(this SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
}

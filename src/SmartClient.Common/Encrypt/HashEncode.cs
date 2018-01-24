using System;
using System.Text;
using System.Security.Cryptography;
namespace SmartClient.Common.Encrypt
{
	/// <summary>
	/// 得到随机安全码（哈希加密）。
	/// </summary>
	public class HashEncode
	{

        /// <summary>
        /// 默认为UTF-8
        /// </summary>
       public static Encoding DefaultCoding = Encoding.UTF8;

		public HashEncode()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
		/// 得到随机哈希加密字符串
		/// </summary>
		/// <returns></returns>
		public static string GetSecurity()
		{			
			string Security = HashEncoding(GetRandomValue());		
			return Security;
		}
		/// <summary>
		/// 得到一个随机数值
		/// </summary>
		/// <returns></returns>
		public static string GetRandomValue()
		{			
			Random Seed = new Random();
			string RandomVaule = Seed.Next(1, int.MaxValue).ToString();
			return RandomVaule;
		}
		/// <summary>
		/// 哈希加密一个字符串
		/// </summary>
		/// <param name="Security"></param>
		/// <returns></returns>
		public static string HashEncoding(string Security)
		{						
			byte[] Value;
			UnicodeEncoding Code = new UnicodeEncoding();
			byte[] Message = Code.GetBytes(Security);
			SHA512Managed Arithmetic = new SHA512Managed();
			Value = Arithmetic.ComputeHash(Message);
			Security = "";
			foreach(byte o in Value)
			{
				Security += (int) o + "O";
			}
			return Security;
		}

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <param name="isLower"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetMD5(string value, Encoding encoding, bool isLower = true, int len=32)
        {
            var result = string.Empty;
            if (null==encoding)
            {
                encoding = Encoding.UTF8;
            }
            var fullMd5Str = EncryptMD5(value, encoding, isLower);
            switch (len)
            {
                case 8:result = fullMd5Str.Substring(0, 8);break;
                case 16: result = fullMd5Str.Substring(8, 16); break;
                case 32: result = fullMd5Str; break;

            }

            return result;
        }

        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="value">待加密的字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isLower">是否转换为小写的十六进制，默认是小写</param>
        /// <returns></returns>
        private static string EncryptMD5(string value, Encoding encoding, bool isLower = true)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(encoding.GetBytes(value));

            // 将MD5输出的二进制结果转换为小写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = isLower ? bytes[i].ToString("x2") : bytes[i].ToString("X2");
                result.Append(hex);
            }
            return result.ToString();
        }

    }
}

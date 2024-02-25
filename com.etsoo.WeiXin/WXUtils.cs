using com.etsoo.Utils;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace com.etsoo.WeiXin
{
    /// <summary>
    /// Weixin Utils
    /// 微信工具
    /// </summary>
    public static class WXUtils
    {
        /// <summary>
        /// Create SHA1 signature
        /// 创建SHA1签名
        /// </summary>
        /// <param name="data">Source data</param>
        /// <returns>Result</returns>
        public static async Task<string> CreateSignatureAsync(SortedDictionary<string, string> data)
        {
            var source = data.JoinAsString().TrimEnd('&');
            return await CreateSignatureAsync(source);
        }

        /// <summary>
        /// Create SHA1 signature
        /// 创建SHA1签名
        /// </summary>
        /// <param name="data">Source data</param>
        /// <returns>Result</returns>
        public static async Task<string> CreateSignatureAsync(IEnumerable<string> data)
        {
            var sorted = data.OrderBy((item) => item, new DictionarySort());
            var source = string.Join(string.Empty, sorted);
            return await CreateSignatureAsync(source);
        }

        static async Task<string> CreateSignatureAsync(string source)
        {
            var signatureBytes = await CryptographyUtils.SHA1Async(source);
            return Convert.ToHexString(signatureBytes).ToLower();
        }

        /// <summary>
        /// 消息解密
        /// </summary>
        /// <param name="Input">密文</param>
        /// <param name="EncodingAESKey">签名密码</param>
        /// <returns>加密结果</returns>
        public static async Task<byte[]> MessageDecryptAsync(string Input, string EncodingAESKey)
        {
            var Key = Convert.FromBase64String(EncodingAESKey + "=");
            var Iv = new byte[16];
            Array.Copy(Key, Iv, 16);
            var btmpMsg = await AESDecryptAsync(Input, Iv, Key);

            var len = BitConverter.ToInt32(btmpMsg, 16);
            len = IPAddress.NetworkToHostOrder(len);

            var bMsg = new byte[len];
            var bAppid = new byte[btmpMsg.Length - 20 - len];
            Array.Copy(btmpMsg, 20, bMsg, 0, len);
            Array.Copy(btmpMsg, 20 + len, bAppid, 0, btmpMsg.Length - 20 - len);

            // appid = Encoding.UTF8.GetString(bAppid);

            return bMsg;
        }

        /// <summary>
        /// 整形排序
        /// </summary>
        /// <param name="inval"></param>
        /// <returns></returns>
        static int HostToNetworkOrder(int inval)
        {
            var outval = 0;
            for (var i = 0; i < 4; i++)
                outval = (outval << 8) + ((inval >> (i * 8)) & 255);
            return outval;
        }

        static byte[] decode2(byte[] decrypted)
        {
            int pad = decrypted[^1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }

        static async Task<byte[]> AESDecryptAsync(string Input, byte[] Iv, byte[] Key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            aes.Key = Key;
            aes.IV = Iv;

            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);

            await using var ms = SharedUtils.GetStream();
            await using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write, true))
            {
                var xXml = Convert.FromBase64String(Input);
                var msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                Array.Copy(xXml, msg, xXml.Length);
                await cs.WriteAsync(xXml);
            }
            return decode2(ms.ToArray());
        }

        static char chr(int a)
        {
            var target = (byte)(a & 0xFF);
            return (char)target;
        }

        static byte[] KCS7Encoder(int text_length)
        {
            var block_size = 32;

            // 计算需要填充的位数
            var amount_to_pad = block_size - (text_length % block_size);
            if (amount_to_pad == 0)
            {
                amount_to_pad = block_size;
            }

            // 获得补位所用的字符
            char pad_chr = chr(amount_to_pad);
            return Encoding.UTF8.GetBytes(string.Empty.PadRight(amount_to_pad, pad_chr));
        }

        static async Task<string> AESEncryptAsync(byte[] Input, byte[] Iv, byte[] Key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.CBC;
            aes.Key = Key;
            aes.IV = Iv;

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);

            #region 自己进行PKCS7补位，用系统自己带的不行
            var msg = new byte[Input.Length + 32 - Input.Length % 32];
            Array.Copy(Input, msg, Input.Length);
            var pad = KCS7Encoder(Input.Length);
            Array.Copy(pad, 0, msg, Input.Length, pad.Length);
            #endregion

            await using var ms = SharedUtils.GetStream();
            await using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write, true))
            {
                await cs.WriteAsync(msg);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 消息加密
        /// </summary>
        /// <param name="Input">明文</param>
        /// <param name="EncodingAESKey">签名密码</param>
        /// <param name="appid">程序编号</param>
        /// <returns>加密结果</returns>
        public static async Task<string> MessageEncryptAsync(string Input, string EncodingAESKey, string appid)
        {
            var Key = Convert.FromBase64String(EncodingAESKey + "=");
            var Iv = new byte[16];
            Array.Copy(Key, Iv, 16);

            var bRand = CryptographyUtils.CreateRandBytes(16).ToArray();
            var bAppid = Encoding.UTF8.GetBytes(appid);
            var btmpMsg = Encoding.UTF8.GetBytes(Input);
            var bMsgLen = BitConverter.GetBytes(HostToNetworkOrder(btmpMsg.Length));
            var bMsg = new byte[bRand.Length + bMsgLen.Length + bAppid.Length + btmpMsg.Length];

            Array.Copy(bRand, bMsg, bRand.Length);
            Array.Copy(bMsgLen, 0, bMsg, bRand.Length, bMsgLen.Length);
            Array.Copy(btmpMsg, 0, bMsg, bRand.Length + bMsgLen.Length, btmpMsg.Length);
            Array.Copy(bAppid, 0, bMsg, bRand.Length + bMsgLen.Length + btmpMsg.Length, bAppid.Length);

            return await AESEncryptAsync(bMsg, Iv, Key);
        }
    }
}

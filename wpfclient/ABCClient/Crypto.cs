using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ABCClient
{
    public static class Crypto
    {
        static byte[] Pad(byte[] data)
        {
            int pad_length = (data.Length + 15) / 16 * 16;
            byte[] pad_data = new byte[pad_length];
            data.CopyTo(pad_data, 0);
            return pad_data;
        }

        public static string Encode(byte[] data, out string skey)
        {
            var aes = Aes.Create();
            byte[] key = aes.Key.Take(16).ToArray();
            skey = Convert.ToBase64String(key);
            byte[] iv = aes.IV.Take(16).ToArray();
            byte[] pad_data = Pad(data);
            List<byte> cipher;
            using (RijndaelManaged Aes128 = new RijndaelManaged())
            {
                Aes128.BlockSize = 128;
                Aes128.KeySize = 128;
                Aes128.Mode = CipherMode.CFB;
                Aes128.FeedbackSize = 128;
                Aes128.Padding = PaddingMode.None;
                Aes128.Key = key;
                Aes128.IV = iv;
                using (var encryptor = Aes128.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(csEncrypt, Encoding.UTF8))
                {
                    bw.Write(pad_data);
                    bw.Close();
                    cipher = iv.ToList();
                    cipher.AddRange(msEncrypt.ToArray().Take(data.Length));
                    return Convert.ToBase64String(cipher.ToArray());
                }
            }
        }

        public static byte[] Decode(string bcipher, string bkey)
        {
            byte[] cipher = Convert.FromBase64String(bcipher);
            byte[] iv = cipher.Take(16).ToArray();
            cipher = cipher.Skip(16).ToArray();
            byte[] key = Convert.FromBase64String(bkey);
            using (RijndaelManaged Aes128 = new RijndaelManaged())
            {
                Aes128.BlockSize = 128;
                Aes128.KeySize = 128;
                Aes128.Mode = CipherMode.CFB;
                Aes128.FeedbackSize = 128;
                Aes128.Padding = PaddingMode.None;
                Aes128.Key = key;
                Aes128.IV = iv;
                using (var decryptor = Aes128.CreateDecryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(csEncrypt, Encoding.UTF8))
                {
                    bw.Write(Pad(cipher));
                    bw.Close();
                    return msEncrypt.ToArray().Take(cipher.Length).ToArray();
                }
            }
        }
    }
}

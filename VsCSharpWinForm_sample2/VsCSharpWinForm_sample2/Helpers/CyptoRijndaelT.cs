using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class CyptoRijndaelT
    {
        // https://www.codeproject.com/Articles/5719/Simple-encrypting-and-decrypting-data-in-C
        // Updated date: 2019-10-02
        // Sample encrypt/decrypt functions.

        public static Helpers.TLog Logger { get; set; }

        // Encrypt a byte array into a byte array using a key and an IV.
        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            byte[] byteReturn = null;
            System.IO.MemoryStream ms = null;
            //System.Security.Cryptography.CryptoStream cs = null;
            System.Security.Cryptography.Rijndael alg = null;
            try
            {
                // Create a MemoryStream to accept the encrypted bytes.
                ms = new System.IO.MemoryStream();

                // Create a symmetric algorithm.
                // We are going to use Rijndael because it is strong and
                // available on all platforms.
                // You can use other algorithms, to do so substitute the
                // next line with something like
                //      TripleDES alg = TripleDES.Create();
                alg = System.Security.Cryptography.Rijndael.Create();

                // Now set the key and the IV.
                // We need the IV (Initialization Vector) because
                // the algorithm is operating in its default
                // mode called CBC (Cipher Block Chaining).
                // The IV is XORed with the first block (8 byte)
                // of the data before it is encrypted, and then each
                // encrypted block is XORed with the
                // following block of plaintext.
                // This is done to make encryption more secure.

                // There is also a mode called ECB which does not need an IV,
                // but it is much less secure.
                alg.Key = Key;
                alg.IV = IV;

                // Create a CryptoStream through which we are going to be
                // pumping our data.
                // CryptoStreamMode.Write means that we are going to be
                // writing data to the stream and the output will be written
                // in the MemoryStream we have provided.
                //cs = new System.Security.Cryptography.CryptoStream(ms, alg.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                // Write the data and make it do the encryption.
                //cs.Write(clearData, 0, clearData.Length);

                // Close the crypto stream (or do FlushFinalBlock).
                // This will tell it that we have done our encryption and
                // there is no more data coming in,
                // and it is now a good time to apply the padding and
                // finalize the encryption process.
                //cs.Close();

                int iLength = 0;
                if (clearData != null) { iLength = clearData.Length; }
                using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, alg.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                { cs.Write(clearData, 0, iLength); } // cs will be closed automically under USING block.

                // Now get the encrypted data from the MemoryStream.
                // Some people make a mistake of using GetBuffer() here,
                // which is not the right way.
                if (ms != null) { byteReturn = ms.ToArray(); }

                return byteReturn;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        // Encrypt bytes into bytes using a password.
        //    Uses Encrypt(byte[], byte[], byte[])
        public static byte[] Encrypt(byte[] clearData, string sPassword)
        {
            //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            try
            {
                // We need to turn the password into Key and IV.
                // We are using salt to make it harder to guess our key
                // using a dictionary attack -
                // trying to guess a password by enumerating all possible words.
                //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                // Now get the key/IV and do the encryption using the function
                // that accepts byte arrays.
                // Using PasswordDeriveBytes object we are first getting
                // 32 bytes for the Key
                // (the default Rijndael key length is 256bit = 32bytes)
                // and then 16 bytes for the IV.
                // IV should always be the block size, which is by default
                // 16 bytes (128 bit) for Rijndael.
                // If you are using DES/TripleDES/RC2 the block size is 8
                // bytes and so should be the IV size.
                // You can also read KeySize/BlockSize properties off the
                // algorithm to find out the sizes.
                //return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
                return Encrypt(clearData, pdb2.GetBytes(32), pdb2.GetBytes(16));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        // Encrypt a string into a string using a password.
        //    Uses Encrypt(byte[], byte[], byte[])
        //    Uses Encrypt(byte[], string)
        public static string Encrypt(string sText, string sPassword)
        {
            // byte[] clearBytes = null;
            // //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            // System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            // byte[] byteData = null;
            // try
            // {
            // // First we need to turn the input string into a byte array.
            // clearBytes = System.Text.Encoding.Unicode.GetBytes(sText);

            // // Then, we need to turn the password into Key and IV
            // // We are using salt to make it harder to guess our key
            // // using a dictionary attack -
            // // trying to guess a password by enumerating all possible words.
            // //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            // pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // // Now get the key/IV and do the encryption using the
            // // function that accepts byte arrays.
            // // Using PasswordDeriveBytes object we are first getting
            // // 32 bytes for the Key
            // // (the default Rijndael key length is 256bit = 32bytes)
            // // and then 16 bytes for the IV.
            // // IV should always be the block size, which is by default
            // // 16 bytes (128 bit) for Rijndael.
            // // If you are using DES/TripleDES/RC2 the block size is
            // // 8 bytes and so should be the IV size.
            // // You can also read KeySize/BlockSize properties off
            // // the algorithm to find out the sizes.
            // //byteData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            // byteData = Encrypt(clearBytes, pdb2.GetBytes(32), pdb2.GetBytes(16));

            // // Now we need to turn the resulting byte array into a string.
            // // A common mistake would be to use an Encoding class for that.
            // // It does not work because not all byte values can be
            // // represented by characters.
            // // We are going to be using Base64 encoding that is designed
            // // exactly for what we are trying to do.
            // return Convert.ToBase64String(byteData);
            // }
            // catch (Exception ex)
            // {
            // LocalLogger("[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". " + ex.Message);
            // return null;
            // }
            byte[] clearBytes = null;
            byte[] byteData = null;
            try
            {
                // First we need to turn the input string into a byte array.
                clearBytes = System.Text.Encoding.Unicode.GetBytes(sText);
                byteData = Encrypt(clearBytes, sPassword);
                return Convert.ToBase64String(byteData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        // Encrypt a file into another file using a password.
        public static void Encrypt(string sInputFilepath, string sOutputFilepath, string sPassword)
        {
            System.IO.FileStream fsIn = null;
            System.IO.FileStream fsOut = null;
            //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            System.Security.Cryptography.Rijndael alg = null;
            System.Security.Cryptography.CryptoStream cs = null;
            try
            {
                // First we are going to open the file streams.
                fsIn = new System.IO.FileStream(sInputFilepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                fsOut = new System.IO.FileStream(sOutputFilepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);

                // Then we are going to derive a Key and an IV from the Password and create an algorithm.
                //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                alg = System.Security.Cryptography.Rijndael.Create();
                //alg.Key = pdb.GetBytes(32);
                alg.Key = pdb2.GetBytes(32);
                //alg.IV = pdb.GetBytes(16);
                alg.IV = pdb2.GetBytes(16);

                // Now create a crypto stream through which we are going to be pumping data.
                // Our fileOut is going to be receiving the encrypted bytes.
                cs = new System.Security.Cryptography.CryptoStream(fsOut, alg.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                // Now will will initialize a buffer and will be processing the input file in chunks.
                // This is done to avoid reading the whole file (which can be huge) into memory.
                int iBufferLen = 4096;
                byte[] buffer = new byte[iBufferLen];
                int iRead;
                do
                {
                    // read a chunk of data from the input file.
                    iRead = fsIn.Read(buffer, 0, iBufferLen);
                    // encrypt it.
                    cs.Write(buffer, 0, iRead);
                } while (iRead != 0);
            }
            catch (Exception ex) { Logger.Error(ex); }
            finally
            {
                // close everything.
                if (cs != null) { cs.Close(); cs = null; }
                if (fsIn != null) { fsIn.Close(); fsIn = null; }
            }
        }

        // Decrypt a byte array into a byte array using a key and an IV.
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            byte[] byteReturn = null;
            System.IO.MemoryStream ms = null;
            System.Security.Cryptography.Rijndael alg = null;
            //System.Security.Cryptography.CryptoStream cs = null;
            try
            {
                // Create a MemoryStream that is going to accept the decrypted bytes.
                ms = new System.IO.MemoryStream();

                // Create a symmetric algorithm.
                // We are going to use Rijndael because it is strong and available on all platforms.
                // You can use other algorithms, to do so substitute the next line with something like
                //     TripleDES alg = TripleDES.Create();
                alg = System.Security.Cryptography.Rijndael.Create();

                // Now set the key and the IV. 
                // We need the IV (Initialization Vector) because the algorithm
                // is operating in its default 
                // mode called CBC (Cipher Block Chaining). The IV is XORed with
                // the first block (8 byte) 
                // of the data after it is decrypted, and then each decrypted
                // block is XORed with the previous 
                // cipher block. This is done to make encryption more secure. 
                // There is also a mode called ECB which does not need an IV,
                // but it is much less secure.
                alg.Key = Key;
                alg.IV = IV;

                // Create a CryptoStream through which we are going to be pumping our data.
                // CryptoStreamMode.Write means that we are going to be writing data to the stream and the output will be written in the MemoryStream we have provided.
                //cs = new System.Security.Cryptography.CryptoStream(ms, alg.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                // Write the data and make it do the decryption.
                //cs.Write(cipherData, 0, cipherData.Length);

                // Close the crypto stream (or do FlushFinalBlock).
                // This will tell it that we have done our decryption and there is no more data coming in,
                // and it is now a good time to remove the padding and finalize the decryption process.
                //cs.Close();

                int iLength = 0;
                if (cipherData != null) { iLength = cipherData.Length; }
                using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, alg.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                { cs.Write(cipherData, 0, iLength); }

                // Now get the decrypted data from the MemoryStream.
                // Some people make a mistake of using GetBuffer() here, which is not the right way.
                if (ms != null) { byteReturn = ms.ToArray(); }

                return byteReturn;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }

        // Decrypt bytes into bytes using a password.
        //    Uses Decrypt(byte[], byte[], byte[])
        public static byte[] Decrypt(byte[] cipherData, string sPassword)
        {
            //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            try
            {
                // We need to turn the password into Key and IV.
                // We are using salt to make it harder to guess our key
                // using a dictionary attack -
                // trying to guess a password by enumerating all possible words.
                //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                // Now get the key/IV and do the Decryption using the function that accepts byte arrays.
                // Using PasswordDeriveBytes object we are first getting 32 bytes for the Key
                // (the default Rijndael key length is 256bit = 32bytes)
                // and then 16 bytes for the IV.
                // IV should always be the block size, which is by default 16 bytes (128 bit) for Rijndael.
                // If you are using DES/TripleDES/RC2 the block size is 8 bytes and so should be the IV size.

                // You can also read KeySize/BlockSize properties off the algorithm to find out the sizes.
                //return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
                return Decrypt(cipherData, pdb2.GetBytes(32), pdb2.GetBytes(16));
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }

        // Decrypt a string into a string using a password.
        //    Uses Decrypt(byte[], byte[], byte[])
        public static string Decrypt(string sText, string sPassword)
        {
            // //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            // System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            // try
            // {
            // // First we need to turn the input string into a byte array.
            // // We presume that Base64 encoding was used.
            // byte[] cipherBytes = Convert.FromBase64String(sText);

            // // Then, we need to turn the password into Key and IV.
            // // We are using salt to make it harder to guess our key.
            // // using a dictionary attack -
            // // trying to guess a password by enumerating all possible words.
            // //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            // pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            // // Now get the key/IV and do the decryption using the function that accepts byte arrays.
            // // Using PasswordDeriveBytes object we are first getting 32 bytes for the Key
            // // (the default Rijndael key length is 256bit = 32bytes)
            // // and then 16 bytes for the IV.
            // // IV should always be the block size, which is by default 16 bytes (128 bit) for Rijndael.
            // // If you are using DES/TripleDES/RC2 the block size is 8 bytes and so should be the IV size.
            // // You can also read KeySize/BlockSize properties off the algorithm to find out the sizes.
            // //byte[] byteData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            // byte[] byteData = Decrypt(cipherBytes, pdb2.GetBytes(32), pdb2.GetBytes(16));

            // // Now we need to turn the resulting byte array into a string.
            // // A common mistake would be to use an Encoding class for that.
            // // It does not work because not all byte values can be represented by characters.
            // // We are going to be using Base64 encoding that is designed exactly for what we are trying to do.
            // return System.Text.Encoding.Unicode.GetString(byteData);
            // }
            // catch (Exception ex)
            // {
            // LocalLogger("[error] " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ". " + ex.Message);
            // return null;
            // }
            byte[] cipherBytes = null;
            byte[] byteData = null;
            try
            {
                cipherBytes = Convert.FromBase64String(sText);
                byteData = Decrypt(cipherBytes, sPassword);
                return System.Text.Encoding.Unicode.GetString(byteData);
            }
            catch (Exception ex)
            {
                Logger?.Error(ex);
                return null;
            }
        }

        // Decrypt a file into another file using a password.
        public static void Decrypt(string sInputFilepath, string sOutputFilepath, string sPassword)
        {
            System.IO.FileStream fsIn = null;
            System.IO.FileStream fsOut = null;
            //System.Security.Cryptography.PasswordDeriveBytes pdb = null;
            System.Security.Cryptography.Rfc2898DeriveBytes pdb2 = null;
            System.Security.Cryptography.Rijndael alg = null;
            System.Security.Cryptography.CryptoStream cs = null;
            try
            {
                // First we are going to open the file streams.
                fsIn = new System.IO.FileStream(sInputFilepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                fsOut = new System.IO.FileStream(sOutputFilepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);

                // Then we are going to derive a Key and an IV from the Password and create an algorithm.
                //pdb = new System.Security.Cryptography.PasswordDeriveBytes(sPassword, new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                pdb2 = new System.Security.Cryptography.Rfc2898DeriveBytes(sPassword, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                alg = System.Security.Cryptography.Rijndael.Create();

                //alg.Key = pdb.GetBytes(32);
                alg.Key = pdb2.GetBytes(32);
                //alg.IV = pdb.GetBytes(16);
                alg.IV = pdb2.GetBytes(16);

                // Now create a crypto stream through which we are going to be pumping data.
                // Our fileOut is going to be receiving the Decrypted bytes.
                cs = new System.Security.Cryptography.CryptoStream(fsOut, alg.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                // Now will will initialize a buffer and will be processing the input file in chunks.
                // This is done to avoid reading the whole file (which can be huge) into memory.
                int iBufferLen = 4096;
                byte[] buffer = new byte[iBufferLen];
                int iRead;
                do
                {
                    // read a chunk of data from the input file.
                    iRead = fsIn.Read(buffer, 0, iBufferLen);
                    // Decrypt it.
                    cs.Write(buffer, 0, iRead);
                } while (iRead != 0);
            }
            catch (Exception ex) { Logger?.Error(ex); }
            finally
            {
                // close everything.
                if (cs != null) { cs.Close(); cs = null; } // this will also close the unrelying fsOut stream
                if (fsIn != null) { fsIn.Close(); fsIn = null; }
            }
        }
    }
}

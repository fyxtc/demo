using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using LitJson;

public class GlobalDataHelper
{
    private const string DATA_ENCRYPT_KEY = "a234857890654c3678d77234567890O2";
    private static RijndaelManaged _encryptAlgorithm = null;

    public static RijndaelManaged DataEncryptAlgorithm ()
    {
        _encryptAlgorithm = new RijndaelManaged ();
        _encryptAlgorithm.Key = Encoding.UTF8.GetBytes (DATA_ENCRYPT_KEY);
        _encryptAlgorithm.Mode = CipherMode.ECB;
        _encryptAlgorithm.Padding = PaddingMode.PKCS7;

        return _encryptAlgorithm;
    }
}

public class LocalUtil
{

    private string LOCAL_FILE = "Assets"+Path.DirectorySeparatorChar+Path.DirectorySeparatorChar + "playerinfo.data";
    private static LocalUtil _LocalUtil = null;

    public static LocalUtil SharedInstance {
        get {
            if (_LocalUtil == null)
                _LocalUtil = new LocalUtil ();

            return _LocalUtil;
        }
    }

    public string EncryptData (string dataToEncrypt)
    {
        //给明文加密用GetBytes
        byte[] dataToEncryptArray = Encoding.UTF8.GetBytes (dataToEncrypt);
        byte[] dataAfterEncryptArray = GlobalDataHelper.DataEncryptAlgorithm().CreateEncryptor ()
    .TransformFinalBlock (dataToEncryptArray, 0, dataToEncryptArray.Length);

        return Convert.ToBase64String (dataAfterEncryptArray, 0, dataAfterEncryptArray.Length);
    }

    public string DecryptData (string dataToDecrypt)
    {
        //给密文解密用FromBase64String
        byte[] dataToDecryptArray = Convert.FromBase64String (dataToDecrypt);
        byte[] dataAfterDecryptArray = GlobalDataHelper.DataEncryptAlgorithm().CreateDecryptor ().TransformFinalBlock (dataToDecryptArray, 0, dataToDecryptArray.Length);

        return Encoding.UTF8.GetString (dataAfterDecryptArray);
    }

    public void Save (Object tobject, bool isEncrypt=true)
    {
        string serializedString = JsonMapper.ToJson (tobject);

        using (StreamWriter sw = File.CreateText(LOCAL_FILE)) {
            if (isEncrypt)
                sw.Write (EncryptData (serializedString));
            else
                sw.Write (serializedString);
        }
    }

    public T Load<T> (bool isEncrypt=true)
    {
        if (File.Exists (LOCAL_FILE) == false)
            return default(T);

        using (StreamReader sr = File.OpenText(LOCAL_FILE)) {
            string stringEncrypt = sr.ReadToEnd ();

            if (string.IsNullOrEmpty (stringEncrypt))
                return default(T);

            if (isEncrypt)
                return  JsonMapper.ToObject<T> (DecryptData (stringEncrypt));
            else
                return JsonMapper.ToObject<T> (stringEncrypt);
        }
    }
}
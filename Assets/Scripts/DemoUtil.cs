using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class DemoUtil{
    public static string ReadConfigFile(string file){
        StreamReader sr = new StreamReader("Assets"+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + file,Encoding.Default);
        string line;
        string res = "";
        while ((line = sr.ReadLine()) != null) 
        {
            res += line;
        }
        return res;
    }
}

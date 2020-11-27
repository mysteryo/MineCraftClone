using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StartInitializer : MonoBehaviour
{
    public static int seed;
    public static float seedForPerlin;
    // Start is called before the first frame update
    void Awake()
    {
        if(File.Exists(Application.persistentDataPath + "//seed.txt"))
        {
            string read = File.ReadAllText(Application.persistentDataPath + "//seed.txt");
            seed = int.Parse(read);
        }
        else
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            //File.Create(Application.persistentDataPath + "//seed.txt");

            using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "//seed.txt"))
            {
                file.WriteLine(seed);
            }
        }
        seedForPerlin = (float)seed / Mathf.Pow(10,Mathf.Abs(seed).ToString().Length);
    }

    
}

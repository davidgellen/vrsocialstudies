using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UMA.CharacterSystem;
public class LoadFromFileUMA : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject umaobject;
    
    void Start()
    {
        string filename = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
        StreamReader reader = new StreamReader(filename);
        string x = reader.ReadToEnd();
        reader.Close();

        umaobject.GetComponent<DynamicCharacterAvatar>().LoadFromRecipeString(x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

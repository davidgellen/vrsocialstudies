using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UMA.CharacterSystem;

public class LoadFromFileUMAsr : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject umaobject;
    public Avatar animatorAvatarFemale;
    public Avatar animatorAvatarMale;
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        string filename = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
        StreamReader reader = new StreamReader(filename);
        string x = reader.ReadToEnd();
        reader.Close();

        umaobject.GetComponent<DynamicCharacterAvatar>().LoadFromRecipeString(x);
        
        if(umaobject.GetComponent<DynamicCharacterAvatar>().activeRace.name == "HumanFemaleHighPoly")
        {
            umaobject.GetComponent<Animator>().avatar = animatorAvatarFemale;
        }
        else
        {
            umaobject.GetComponent<Animator>().avatar = animatorAvatarMale;
        }
        Destroy(this);
    }
}

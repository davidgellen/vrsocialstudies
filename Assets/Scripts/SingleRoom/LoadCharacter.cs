using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UMA;
using UMA.CharacterSystem;

public class LoadCharacter : MonoBehaviour
{
	public DynamicCharacterAvatar avatar;
	public GameObject avatarToScale;
    // Start is called before the first frame update
    void Start()
    {
        //RpcLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	/*
	public void RpcLoad()
    {
        //Regenerate UMA using string
		string filePath = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
		StreamReader reader = new StreamReader(filePath);
		string recString = reader.ReadToEnd();
		reader.Close();
		
        UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
        recipe.recipeString = recString;
        //avatar.Load(recipe);
		ImportSettings(UMATextRecipe.PackedLoadDCS(context, recString), thisLoadFlags, true);
		
        Destroy(recipe);
    }
	*/

}

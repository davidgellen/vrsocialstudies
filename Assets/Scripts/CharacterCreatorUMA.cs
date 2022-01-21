using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.UI;

public class CharacterCreatorUMA : MonoBehaviour
{
	public DynamicCharacterAvatar avatar;
	public Slider heightSlider;
	public Slider bellySlider;
	
	
	public List<string> hairModels = new List<string>();
	private int currentHair;
	
	private Dictionary<string, DnaSetter> dna;
	
	void OnEnable()
	{
		avatar.CharacterUpdated.AddListener(Updated);
		heightSlider.onValueChanged.AddListener(HeightChange);
		bellySlider.onValueChanged.AddListener(BellyChange);
	}
	void OnDisable()
	{
		avatar.CharacterUpdated.RemoveListener(Updated);
		heightSlider.onValueChanged.RemoveListener(HeightChange);
		bellySlider.onValueChanged.RemoveListener(BellyChange);
	}
	
	void Updated(UMAData data)
	{
		dna = avatar.GetDNA();
		heightSlider.value = dna["height"].Get();
		bellySlider.value = dna["belly"].Get();
	}
	public void HeightChange(float val)
	{
		dna["height"].Set(val);
		avatar.BuildCharacter();
	}
	public void BellyChange(float val)
	{
		dna["belly"].Set(val);
		avatar.BuildCharacter();
	}
	
	
	
	public void ChangeHair(bool plus)
	{
		if(plus)
		{
			currentHair++;
		}
		else
		{
			currentHair--;
		}
		
		if(currentHair == hairModels.Count)
		{currentHair=0;}	
		
		currentHair = Mathf.Clamp(currentHair, 0, hairModels.Count - 1);
		
		if(hairModels[currentHair] == "None")
		{
			avatar.ClearSlot("Chest");
		}
		else
		{
			avatar.SetSlot("Chest", hairModels[currentHair]);
		}
		avatar.BuildCharacter();
	}
	
	
	
	public void SwitchGender(bool male)
	{
		if(male && avatar.activeRace.name != "HumanMaleDCS")
		{
			avatar.ChangeRace("HumanMaleDCS");
		}
		
		if(!male && avatar.activeRace.name != "HumanFemaleDCS")
		{
			avatar.ChangeRace("HumanFemaleDCS");
		}
	}
	
	
	public void Save()
	{
		UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
        recipe.Save(avatar.umaData.umaRecipe, avatar.context);
        string characterParameters = recipe.recipeString;
        Destroy(recipe);
         
        //string fileName = "StreamingAssets/character.txt";
		string fileName = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
		if (File.Exists(fileName))
        {
			File.Delete(fileName);
        }
		 
        StreamWriter stream = File.CreateText(fileName);
        stream.WriteLine(characterParameters);
        stream.Close();
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

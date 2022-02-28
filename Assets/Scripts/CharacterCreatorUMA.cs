using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.SceneManagement;
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
		if(male && avatar.activeRace.name != "HumanMaleHighPoly")
		{
			avatar.ChangeRace("HumanMaleHighPoly");
			
			// rovno aj oblecenie
			avatar.ClearSlot("Hair");
			avatar.SetSlot("Hair", "MaleHairSlick01_Recipe");
			
			avatar.ClearSlot("Chest");
			avatar.SetSlot("Chest", "MaleHoodie_Recipe");
			
			avatar.ClearSlot("Legs");
			avatar.SetSlot("Legs", "MaleSweatPants_Recipe");
			
			avatar.ClearSlot("Feet");
			avatar.SetSlot("Feet", "TallShoes_Black_Recipe");
			
			avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Shirt",  new Color(51/255f, 63/255f, 140/255f) );
			//avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Pants",  new Color(178/255f, 178/255f, 178/255f) );
			//avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Pants1",  new Color(178/255f, 178/255f, 178/255f) );
			avatar.GetComponent<DynamicCharacterAvatar>().UpdateColors(true);
			
			avatar.BuildCharacter();
		}
		
		if(!male && avatar.activeRace.name != "HumanFemaleHighPoly")
		{
			avatar.ChangeRace("HumanFemaleHighPoly");
			
			// rovno aj oblecenie
			avatar.ClearSlot("Hair");
			avatar.SetSlot("Hair", "FemaleHair1");
			
			avatar.ClearSlot("Chest");
			avatar.SetSlot("Chest", "FemaleShirt1");
			
			avatar.ClearSlot("Legs");
			avatar.SetSlot("Legs", "FemalePants1");
			
			avatar.ClearSlot("Feet");
			avatar.SetSlot("Feet", "FemaleTallShoes_Black");
			
			avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Shirt",  new Color(51/255f, 63/255f, 140/255f) );
			avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Pants",  new Color(178/255f, 178/255f, 178/255f) );
			avatar.GetComponent<DynamicCharacterAvatar>().SetColor("Pants1",  new Color(178/255f, 178/255f, 178/255f) );
			avatar.GetComponent<DynamicCharacterAvatar>().UpdateColors(true);
			
			avatar.BuildCharacter();
		}
	}
	
	
	public void Save()
	{
		/*
		UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
        recipe.Save(avatar.umaData.umaRecipe, avatar.context);
        string characterParameters = recipe.recipeString;
        Destroy(recipe);
        */
		string characterParameters = avatar.GetComponent<DynamicCharacterAvatar>().GetCurrentRecipe();
         
        //string fileName = "StreamingAssets/character.txt";
		string fileName = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
		if (File.Exists(fileName))
        {
			File.Delete(fileName);
        }
		 
        StreamWriter stream = File.CreateText(fileName);
        stream.WriteLine(characterParameters);
        stream.Close();
        
        // return to menu
        SceneManager.LoadScene("Menu");
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

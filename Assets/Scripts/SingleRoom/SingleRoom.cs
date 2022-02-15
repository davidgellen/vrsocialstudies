/*
	skriptove poznamky k sebasucit, sebaprotekcia
	
	#0
	SingleRoom.cs je pociatocny skript ktory rozhoduje o dalsom konani v experimentoch
	sebasucit, sebaprotekcia. Je to taky main() pre obe a na zaklade aky parameter 
	pride z menu, taky skript sa spusti, ak príde
		1 -> Sebasucit.cs
		2 -> Sebaprotekcia.cs
	
	#1
	motionscript nemodifikuje component Animator, len vytvori a prida novu animaciu 
	cize treba vypnut component Animator az nasledne prehrat zachytenu animaciu
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UMA;
using UMA.CharacterSystem;

public class SingleRoom : MonoBehaviour
{
	// voice recognition
	[SerializeField] private Text m_Hypotheses;
    [SerializeField] private Text m_Recognitions;
    private DictationRecognizer m_DictationRecognizer;
	public float timeAdded = 0; 						//kolko casu sa  pridalo kvoli rozpravaniu
	
	// misc
	protected const float DEFAULT_PHASE_LENGTH = 30; 	// v sekundach
	protected float phase_length = 120;
	
	public int phase;					  				// aktualna faza
	protected int wordcount = 0;
	protected float time;								// cas od zaciatku sceny v sekundach
	public GameObject avatar; 							// gameobject avatara (participanta)
	public GameObject agent;  							// gameobject agenta

	// animacie
	public RuntimeAnimatorController animatorControllerPhase1_1;
	public RuntimeAnimatorController animatorControllerPhase1_2;
	public RuntimeAnimatorController animatorControllerPhase1_3;
	public RuntimeAnimatorController animatorControllerPhase1_4;
	public RuntimeAnimatorController animatorControllerPhase3_1;
	public RuntimeAnimatorController animatorControllerPhase3_2;
	public RuntimeAnimatorController animatorControllerPhase3_3;
	public Avatar animatorAvatar1;

	// zaznam a reprodukcia pohybu a zvuku
	public ScManager scmanager;
	public GameObject player;
	public GameObject kid;

	// vymena stoliciek
	public Vector3 seatPositionA;
	public Vector3 seatPositionB;
	public Quaternion seatRotationA;
	public Quaternion seatRotationB;

	// riadenie
	public Sebasucit sebasucit;
	public Sebaprotekcia sebaprotekcia;
	
	// slzy
	public bool isCrying = false;
	public GameObject tear;
	public GameObject tear1_position;
	public GameObject tear2_position;
	private float timer1 = 0.0f;
	private float timer2 = 0.0f;
	private float waitTime1 = 5.0f;
	private float waitTime2 = 4.0f;
	
    void Start()
    {
        Debug.Log("TERAZ SA NACITALA SCENA S PARAMETROM: "+MainMenu.info);
		//loadCharacterParameters();
		isCrying = false;
		recordStuff();
		
		this.gameObject.AddComponent<Sebasucit>();
    }
	
	
    void Update()
    {
		// nie moc efektivne riesenie
		// vo faze kde sa nahrava skracuj cas, vo faze kde sa prehrava neskracuj ale nastav
		if(phase == 1 || phase == 3)
		{
			timeAdded = timeAdded + (0.2f * wordcount);
			wordcount = 0;
		}

		if(isCrying)
		cry();
    }
	
	
	/* ----------------------- pomocne funkcie oboch experimentov  ----------------------- */
	// voice recognition funkcia
	void recordStuff()
	{
		//wordcount = 0;
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
			int count = text.Split(' ').Length;
			wordcount = wordcount + count;
          //Debug.LogFormat("Dictation result: {0} => length = {1}, wordcount = {2}", text, count, wordcount);
            //m_Recognitions.text += text + "\n";
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            //Debug.LogFormat("Dictation hypothesis: {0}", text); //otravne vypisi
            //m_Hypotheses.text += text;
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
			
			recordStuff();
        };
		
        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };
		
        m_DictationRecognizer.Start();
    }
	
	protected void changeAvatarScale(GameObject avatar, bool makeBigger) // 0 zmensi, 1 zvacsi
	{
		if(makeBigger)
		{
			if(avatar.transform.localScale.x < 1.1f)
			{
				Vector3 scaleChange = new Vector3(+0.00001f, +0.00001f, +0.00001f);
				avatar.transform.localScale += scaleChange;
			}
		}
		else
		{
			if(avatar.transform.localScale.x > 0.9f)
			{
				Vector3 scaleChange = new Vector3(-0.00001f, -0.00001f, -0.00001f);
				avatar.transform.localScale += scaleChange;
			}
		}
	}
	protected void changeAvatarScaleFixed(GameObject avatar, bool makeBigger)
	{
		if(makeBigger)
		{
			Vector3 scaleChange = new Vector3(1.1f, 1.1f, 1.1f);
			avatar.transform.localScale = scaleChange;
		}
		else
		{
			Vector3 scaleChange = new Vector3(0.9f, 0.9f, 0.9f);
			avatar.transform.localScale = scaleChange;
		}
	}
	protected void resetAvatarScale(GameObject avatar)
	{
		Vector3 scaleChange = new Vector3(1.0f, 1.0f, 1.0f);
		avatar.transform.localScale = scaleChange;
	}
	protected void makeSeat(GameObject avatarToSitOnA, GameObject avatarToSitOnB)
	{
		avatarToSitOnA.transform.position = seatPositionA;
		avatarToSitOnA.transform.rotation = seatRotationA;
		
		avatarToSitOnB.transform.position = seatPositionB;
		avatarToSitOnB.transform.rotation = seatRotationB;
	}
	protected void loadCharacterParameters()
	{
		string filename = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
		StreamReader reader = new StreamReader(filename);
		string x = reader.ReadToEnd();
		reader.Close();

		avatar.GetComponent<DynamicCharacterAvatar>().LoadFromRecipeString(x);
		agent.GetComponent<DynamicCharacterAvatar>().LoadFromRecipeString(x);
	}
	protected void cry()
	{
		this.timer1 = this.timer1 + Time.deltaTime;
		this.timer2 = this.timer2 + Time.deltaTime;
		
		if(timer1 >= waitTime1)
		{
			waitTime1 = Random.Range(2.5f, 4.0f);
			timer1 = 0.0f;
			
			GameObject obj1 = Instantiate(tear, tear1_position.transform.position, tear1_position.transform.rotation);
			Destroy(obj1, 3);
		}
		if(timer2 >= waitTime2)
		{
			waitTime2 = Random.Range(2.5f, 4.0f);
			timer2 = 0.0f;
			
			GameObject obj2 = Instantiate(tear, tear2_position.transform.position, tear2_position.transform.rotation);
			Destroy(obj2, 3);
		}
	}
}
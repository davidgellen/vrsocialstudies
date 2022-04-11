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
using Unity.VisualScripting;

public class SingleRoom : MonoBehaviour
{
	// voice recognition
	[SerializeField] private Text m_Hypotheses;
    [SerializeField] private Text m_Recognitions;
    private DictationRecognizer m_DictationRecognizer;
	public float timeAdded = 0; 						//kolko casu sa  pridalo kvoli rozpravaniu
	
	// misc
	protected const float DEFAULT_PHASE_LENGTH = 2 * 20.0f; 	// v sekundach
	protected float phase_length = 10;
	
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
	public Avatar animatorAvatarFemale;
	public Avatar animatorAvatarMale;
	
	// fix
	public GameObject seatA;
	public GameObject seatB;
	
	//
	public GameObject seatA_playrecord;
	public GameObject seatB_playrecord;

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
	
	// gui timer
	public bool isGUITimerActive = false;
	public float timeLimit = 30.0f;
	public TextMesh timeText;
	public GameObject timeObject;

    [SerializeField] AlignToHeght alightToHeight;

    public float phase2heightOffset = 0f;
	
	
    void Start()
    {
        Debug.Log("TERAZ SA NACITALA SCENA S PARAMETROM: "+MainMenuParam.param);
		//loadCharacterParameters();
		isCrying = false;
		recordStuff();
		
		if (MainMenuParam.param == 3)						// sebakritika
		{
			this.gameObject.AddComponent<Sebaprotekcia>();
		}
		else 												// sebasucit
		{
			this.gameObject.AddComponent<Sebasucit>();
		}
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


		if (isGUITimerActive)
		{
			timeObject.SetActive(true);
			// show time
			timeLimit = timeLimit - Time.deltaTime;
			Debug.Log(timeLimit);
        
			float minutes = Mathf.FloorToInt(timeLimit / 60);
			float seconds = Mathf.FloorToInt(timeLimit % 60);
			timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

			if (timeLimit <= 1)
			{
				isGUITimerActive = false; 
				timeObject.SetActive(false);
			}
		}
		else
		{
			timeLimit = 30.0f;
			timeObject.SetActive(false);
		}
    }
	
	
	/* ----------------------- pomocne funkcie oboch experimentov  ----------------------- */
	// voice recognition funkcia
	
	/*protected void startGUItimer()
	{
		if (!isGUITimerActive)
		{
			timeLimit = 30.0f;
			isGUITimerActive = true;
			timeObject.SetActive(true);
		}
	}
	*/

	void recordStuff()
	{
		//wordcount = 0;
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
			int count = text.Split(' ').Length;
			wordcount = wordcount + count;
            Debug.LogFormat("Dictation result: {0} => length = {1}, wordcount = {2}", text, count, wordcount);
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
		/*
		avatarToSitOnA.transform.position = seatPositionA;
		avatarToSitOnA.transform.rotation = seatRotationA;
		
		avatarToSitOnB.transform.position = seatPositionB;
		avatarToSitOnB.transform.rotation = seatRotationB;
		*/
		avatarToSitOnA.transform.position = seatA.transform.position;
		avatarToSitOnA.transform.rotation = seatA.transform.rotation;
		
		avatarToSitOnB.transform.position = seatB.transform.position;
		avatarToSitOnB.transform.rotation = seatB.transform.rotation;
	}
	protected void makeSeatPlayRecord(GameObject avatarToSitOnA, GameObject avatarToSitOnB)
	{
		/*
		avatarToSitOnA.transform.position = seatPositionA;
		avatarToSitOnA.transform.rotation = seatRotationA;
		
		avatarToSitOnB.transform.position = seatPositionB;
		avatarToSitOnB.transform.rotation = seatRotationB;
		*/
		avatarToSitOnA.transform.position = seatA_playrecord.transform.position;
		avatarToSitOnA.transform.rotation = seatA_playrecord.transform.rotation;
		
		avatarToSitOnB.transform.position = seatB_playrecord.transform.position;
		avatarToSitOnB.transform.rotation = seatB_playrecord.transform.rotation;
	}
	protected void makeSeat(GameObject obj, bool positionA)
	{
		if(positionA)
		{
			obj.transform.position = seatPositionA;
			obj.transform.rotation = seatRotationA;
		}
		else
		{
			obj.transform.position = seatPositionB;
			obj.transform.rotation = seatRotationB;
		}
	}

	/*
	MODRA = 1
	CIERNA = 2
	ZLTA = 3
	CERVENA = 4
	*/
	protected void changeColor(GameObject uma, int colorcode)
	{
		Color uma_blue = new Color(51/255f, 63/255f, 140/255f);
		Color uma_black = new Color(0/255f, 0/255f, 0/255f);
		Color uma_yellow = new Color(255/255f, 255/255f, 0/255f);
		Color uma_red = new Color(240/255f, 15/255f, 15/255f);
		Color color_chosen = new Color(0,0,0);

		switch (colorcode)
		{
			case 1:
				color_chosen = uma_blue;
				break;
			case 2:
				color_chosen = uma_black;
				break;
			case 3: 
				color_chosen = uma_yellow;
				break;
			case 4:
				color_chosen = uma_red;
				break;
		}

		uma.GetComponent<DynamicCharacterAvatar>().SetColor("Shirt", color_chosen);
		uma.GetComponent<DynamicCharacterAvatar>().UpdateColors(true);
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
			Destroy(obj1, 0.5f);
		}
		if(timer2 >= waitTime2)
		{
			waitTime2 = Random.Range(2.5f, 4.0f);
			timer2 = 0.0f;
			
			GameObject obj2 = Instantiate(tear, tear2_position.transform.position, tear2_position.transform.rotation);
			Destroy(obj2, 0.5f);
		}
	}

    public void setPhase2heightOffset(float newOffset)
    {
        phase2heightOffset = newOffset;
    }
}
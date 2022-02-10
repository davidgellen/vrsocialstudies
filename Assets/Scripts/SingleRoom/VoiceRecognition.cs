using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows.Speech;
using System.Collections;
using System.IO;
using UMA;
using UMA.CharacterSystem;

public class VoiceRecognition : MonoBehaviour
{
	// voice recognition
	[SerializeField] private Text m_Hypotheses;
    [SerializeField] private Text m_Recognitions;
    private DictationRecognizer m_DictationRecognizer;
	
	// misc
	private const float PHASE_LENGTH = 120; // v sekundach
	private int phase;
    private int wordcount;
	private float time;
	public GameObject avatarA;
	public GameObject avatar;
	private bool isSaved = false;
	private bool isRecording = false;
	
	// animacie
	public RuntimeAnimatorController animatorControllerPhase1_1;
	public RuntimeAnimatorController animatorControllerPhase1_2;
	public RuntimeAnimatorController animatorControllerPhase1_3;
	public RuntimeAnimatorController animatorControllerPhase1_4;
	public RuntimeAnimatorController animatorControllerPhase3_1;
	public RuntimeAnimatorController animatorControllerPhase3_2;
	public RuntimeAnimatorController animatorControllerPhase3_3;
	
	
	
	public Avatar animatorAvatar1;
	public Avatar animatorAvatar2;
	public Avatar animatorAvatar3;
	public Avatar animatorAvatar4;
	
	// pohyb a zvuk
	public ScManager scmanager;
	public GameObject player;
	public GameObject kid;
	
	
	// vymena stoliciek
	Vector3 seatPositionA;
	Vector3 seatPositionB;
	Quaternion seatRotationA;
	Quaternion seatRotationB;
	

	/*
	public SoundManager soundManager;
	private MotionManager motionManager;
	BVHRecorder recorder;
	BVHAnimationLoader loader;
	public static AudioSource audioSource;
	public Avatar avafix;
	*/

	
    void recordStuff(){
        wordcount = 0;
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
            // Debug.LogFormat("Dictation hypothesis: {0}", text);
            //m_Hypotheses.text += text;
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete){
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            } if (completionCause == DictationCompletionCause.Canceled ||
                  completionCause == DictationCompletionCause.TimeoutExceeded){
                Debug.Log("reloading recorder");
                recordStuff();
            }
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };


        m_DictationRecognizer.Start();
    }

    void Start()
    {
		phase = 1;
		time = 0;
        //recordStuff();
	
		seatPositionA = avatarA.transform.position;
		seatPositionB = avatar.transform.position;
		seatRotationA = avatarA.transform.rotation;
		seatRotationB = avatar.transform.rotation;
		
		/* fix rigidbody */
		//avatar.GetComponent<Animator>().enabled = false;
		
		
		phase1_start();
    }
	
	void Update()
	{
		this.time = this.time + Time.deltaTime;
		//Debug.Log("cas: " + time);
		
		// nie moc efektivne riesenie
		time += wordcount;
		wordcount = 0;
		
		if(time > PHASE_LENGTH) // nadišiel čas zmeniť fazu
		{
			time = 0;
			phase++;
			Debug.Log("PREPINAM FAZU, NOVA FAZA JE: "+phase);
			
			// jednorazove zapnutie funckie dalsej fazy
			switch(phase)
			{
				case 2: phase2_start(); break;
				case 3: phase3_start(); break;
				case 4: phase4_start(); break;
				default: phaseEndOfExperiment_start(); break;
			}
		}
		
		// periodicke vykonavanie kodu ktory prislucha momentalnej faze
		switch(phase)
		{
			case 1: phase1(); break;
			case 2: phase2(); break;
			case 3: phase3(); break;
			case 4: phase4(); break;
		}
	}


/* ------------------------------- POMOCNE FUNKCIE ------------------------------- */
	/* zmensovanie agenta */
	void changeAvatarScale(GameObject avatar, bool makeBigger) // 0 zmensi, 1 zvacsi
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
	void changeAvatarScaleFixed(GameObject avatar, bool makeBigger)
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
	void resetAvatarScale(GameObject avatar)
	{
		Vector3 scaleChange = new Vector3(1.0f, 1.0f, 1.0f);
		avatar.transform.localScale = scaleChange;
	}
	
	
	void makeSeat(GameObject avatarToSitOnA, GameObject avatarToSitOnB)
	{
		avatarToSitOnA.transform.position = seatPositionA;
		avatarToSitOnA.transform.rotation = seatRotationA;
		
		avatarToSitOnB.transform.position = seatPositionB;
		avatarToSitOnB.transform.rotation = seatRotationB;
	}



/* ------------------------------- KONKRETNE FAZY ------------------------------- */
/*
	agent oproti strieda animacie {anstate 1, anstate 2, anstate 3, anstate 4} a zvuky
	(oba akteri sedia oproti sebe, participant kritizuje agenta pred sebou)
	
	- agent sa bude postupom casu zmensovat
*/
void phase1_start()
{
	string filename = Path.Combine(Application.streamingAssetsPath, "character" + ".txt");
	StreamReader reader = new StreamReader(filename);
	string x = reader.ReadToEnd();
	reader.Close();
	
	//UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
   // recipe.Load(x, avatar.GetComponent<DynamicCharacterAvatar>().context);
	
	
	avatar.GetComponent<DynamicCharacterAvatar>().LoadFromRecipeString(x);
	//ImportSettings(UMATextRecipe.PackedLoadDCS(avatar.context, x));
	//UMATextRecipe.PackedLoadDCS(avatar.GetComponent<DynamicCharacterAvatar>().context, x)
}

void phase1()
{
	/* test */
	//UMATextRecipe asset = ScriptableObject.CreateInstance();
	

	
	// zapnutie nahravania zvuku a pohybu
	if(isRecording == false)
	{
		isRecording = true;
		
		/* fix */
		
		scmanager.soundManager.Record();
		scmanager.motionManager.Record();
	}
	
	
	if(time > 1 && time < 3)
	{
		avatarA.GetComponent<Rigidbody>().isKinematic = true;
		avatar.GetComponent<Rigidbody>().isKinematic = true;
	}
	
	
	if(time < 30)
	{
		Debug.Log("anstate 1");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_1;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else if(time < 60)
	{
		Debug.Log("anstate 2");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_2;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else if(time < 90)
	{
		Debug.Log("anstate 3");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_3;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else
	{
		Debug.Log("anstate 4");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_4;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	
	changeAvatarScale(avatar, false);
}

/*
	dojde k presadeniu praticipanta a agenta
	agent replikuje pohyb a zvuk participanta z fazy 1
*/
void phase2_start()
{
	//makeSeat(avatar, avatarA);
	
	if(isSaved == false)
	{
		isSaved = true;
		Debug.Log("SAVING & PLAYING");
		
		scmanager.soundManager.Save("sound");
		scmanager.motionManager.Save("motion");
		
		/* presadenie */
		/*
		Vector3 agent_position_temp = scmanager.kid.transform.position;
		scmanager.kid.transform.position = scmanager.player.transform.position;
		scmanager.player.transform.position = agent_position_temp;
		scmanager.player.transform.Rotate(0, 180, 0);
		*/
		
		avatar.GetComponent<Animator>().enabled = false;
		
		scmanager.soundManager.Play("sound");
		scmanager.motionManager.Play("motion");
	}
}

void phase2()
{
	changeAvatarScale(avatar, true);
}

/*
	dojde k presadeniu participanta a agenta
	agent mení animácie {anstate 1, anstate 2, anstate 3}
	(participant utešuje agenta pred sebou)
*/
void phase3_start()
{
	//makeSeat(avatarA, avatar);
	
	scmanager.soundManager.Record();
	
	scmanager.motionManager.Reset();
	scmanager.motionManager.Record();
	avatar.GetComponent<Animator>().enabled = true;
	
	changeAvatarScaleFixed(avatar, false);
}

void phase3()
{
	if(time < 30)
	{
		Debug.Log("anstate 1");
		/*
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorController1;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
		*/
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_1;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else if(time < 60)
	{
		Debug.Log("anstate 2");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_2;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else if(time < 90)
	{
		Debug.Log("anstate 3");
		avatar.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_3;
		avatar.GetComponent<Animator>().avatar = animatorAvatar1;
	}
	else
	{
		Debug.Log("anstate 4");
	}
}

/*
	dojde k presadeniu participanta a agenta
	agent replikuje pohyb a zvuk participanta z fazy 3
*/
void phase4_start()
{
		//makeSeat(avatar, avatarA);
	
		Debug.Log("SAVING & PLAYING");
		scmanager.soundManager.Save("sound");
		scmanager.motionManager.Save("motion");
		
		/* presadenie */
		/*
		Vector3 agent_position_temp = scmanager.kid.transform.position;
		scmanager.kid.transform.position = scmanager.player.transform.position;
		scmanager.player.transform.position = agent_position_temp;
		scmanager.player.transform.Rotate(0, 180, 0);
		*/
		
		avatar.GetComponent<Animator>().enabled = false;
		
		scmanager.soundManager.Play("sound");
		scmanager.motionManager.Play("motion");
		
		resetAvatarScale(avatar);
}

void phase4()
{}

void phaseEndOfExperiment_start()
{}
/* ---------------------------------------------------------------------------------- */
}
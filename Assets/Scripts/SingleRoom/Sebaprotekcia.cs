using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using CrazyMinnow.SALSA;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.SceneManagement;

public class Sebaprotekcia : SingleRoom
{
	private bool isRecording;
	public SingleRoom srm;
	public bool isPhaseStopped = true;
	public bool phaseTicket = false;
	public bool savedAtEnd = false;
	
	private float timePause = 0;
	public const float DELAY_TIME = 20.0f;
	
	
	// ------------------------------- lipsync
	async Task<AudioClip> LoadAudioClip(string path){
		AudioClip clip = null;

		using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
		{
			uwr.SendWebRequest();
 
			// wrap tasks in try/catch, otherwise it'll fail silently
			try
			{
				while (!uwr.isDone) await Task.Delay(5);
 
				if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
				else
				{
					clip = DownloadHandlerAudioClip.GetContent(uwr);
				}
			}
			catch (Exception err)
			{
				Debug.Log($"{err.Message}, {err.StackTrace}");
			}
		}
 
		return clip;
	}
	// -------------------------------
	
	
    void Start()
    {
        Debug.Log("Sebaprotekcia.cs");
		
		srm = GameObject.Find("SingleRoomManager").GetComponent<SingleRoom>();
		avatar = srm.avatar;
		agent = srm.agent;
		animatorControllerPhase1_1 = srm.animatorControllerPhase1_1;
		animatorControllerPhase1_2 = srm.animatorControllerPhase1_2;
		animatorControllerPhase1_3 = srm.animatorControllerPhase1_3;
		animatorControllerPhase1_4 = srm.animatorControllerPhase1_4;
		animatorControllerPhase3_1 = srm.animatorControllerPhase3_1;
		animatorControllerPhase3_2 = srm.animatorControllerPhase3_2;
		animatorControllerPhase3_3 = srm.animatorControllerPhase3_3;
		animatorAvatar1 = srm.animatorAvatar1;
		animatorAvatarFemale = srm.animatorAvatarFemale;
		animatorAvatarMale = srm.animatorAvatarMale;
		
		scmanager = srm.scmanager;
		seatPositionA = srm.seatPositionA;
		seatPositionB = srm.seatPositionB;
		seatRotationA = srm.seatRotationA;
		seatRotationB = srm.seatRotationB;

		seatA = srm.seatA;
		seatB = srm.seatB;

		seatA_playrecord = srm.seatA_playrecord;
		seatB_playrecord = srm.seatB_playrecord;
		
		
		/* gui */
		/*
		isGUITimerActive = srm.isGUITimerActive;
		timeLimit = srm.timeLimit;
		timeText = srm.timeText;
		timeObject = srm.timeObject;
		*/
		
		
		phase = 0;
		time = 0;
		isRecording = false;
		
		
		/* -- animator podla pohlavia -- */
		/*
		if(agent.GetComponent<DynamicCharacterAvatar>().activeRace.name == "HumanFemaleHighPoly")
		{
			agent.GetComponent<Animator>().avatar = animatorAvatarFemale;
		}
		else
		{
			agent.GetComponent<Animator>().avatar = animatorAvatarMale;
		}
		*/
    }
	
	void resetAddTime()
	{
		srm.timeAdded = 0;
		timeAdded = 0;
	}

    void Update()
    {
	    if(!isPhaseStopped)
	    {
			this.time = this.time + Time.deltaTime;
		}

	    this.timeAdded = srm.timeAdded;	 // ziskaj z srm timeAdded
		srm.phase = this.phase;			 // informuj srm o aktualnej faze
		
		if(time + timeAdded > DEFAULT_PHASE_LENGTH || phase == 0) // nadišiel čas zmeniť fazu
		{
			Debug.Log("timePause = " + timePause);
			this.timePause = this.timePause + Time.deltaTime;
			isPhaseStopped = true;

			// ak skoncila 1. a 3. faza tak uloz motion a sound
			if(phase == 1 && savedAtEnd == false || phase == 3 && savedAtEnd == false)
			{
				savedAtEnd = true;
				scmanager.soundManager.Save("sound");
				scmanager.motionManager.Save("motion");
			}
			
			if (timePause > DELAY_TIME)
			{
				timePause = 0;
				isPhaseStopped = false;
				phaseTicket = true;
			}
			else
			{
				phasePause();
			}
		}
		
		if(!isPhaseStopped && phaseTicket)
		{
			phaseTicket = false;
			
			Debug.Log("Prepinam z fazy "+phase+" ktorej sa pridal cas: "+timeAdded+"; nova faza je: "+(phase+1) );
			phaseUnpause(); // faza sa odpauzla

			//prepla sa faza
			phase++;
			time = 0;
			savedAtEnd = false;
				
			// jednorazove zapnutie funckie dalsej fazy
			switch(phase)
			{
				case 1: phase1_start(); break;
				case 2: phase2_start(); break;
				case 3: phase3_start(); break;
				case 4: phase4_start(); break;
				default: phaseEndOfExperiment_start(); break;
			}
		}

		// periodicke vykonavanie kodu ktory prislucha momentalnej faze, ak nie je faza pozastavena
		if(!isPhaseStopped)
		{
			switch(phase)
			{
				case 1: phase1(); break;
				case 2: phase2(); break;
				case 3: phase3(); break;
				case 4: phase4(); break;
			}	
		}
    }
	
	
	/* ----------------------- funkcie jednotlivych faz ----------------------- */
	void phase1_start()
	{
		// zmen farby
		changeColor(avatar, 2);
		changeColor(agent, 1);
		
		// vypnutie fyziky
		//avatar.GetComponent<Rigidbody>().isKinematic = true;
		agent.GetComponent<Rigidbody>().isKinematic = true;
		
		//agent.GetComponent<CapsuleCollider>().enabled = false;
		//agent.GetComponent<Rigidbody>().enabled = false;
	}
	void phase1()
	{
		// zapnutie nahravania zvuku a pohybu
		if(isRecording == false)
		{
			isRecording = true;
			scmanager.soundManager.Record();
			scmanager.motionManager.Record();
		}
		
		if(time > 0.1 && time < 3)
		{
			//avatar.GetComponent<Rigidbody>().isKinematic = true;
			agent.GetComponent<Rigidbody>().isKinematic = true;	
			
			/* nadvihnutie do spravnej vysky */
			//avatar.transform.position = new Vector3(avatar.transform.position.x, 0.185f, avatar.transform.position.z);
			//agent.transform.position = new Vector3(0.3645f, 0.185f, 0.8694555f);
			
			/* fix placu odrazajuceho sa od tela */
			agent.GetComponent<CapsuleCollider>().enabled = false;
			
			/* az teraz */
			seatPositionA = avatar.transform.position;
			seatPositionB = agent.transform.position;
			seatRotationA = avatar.transform.rotation;
			seatRotationB = agent.transform.rotation;
			
			
			// fyzika fix
			agent.GetComponent<Rigidbody>().isKinematic = true;
			agent.GetComponent<CapsuleCollider>().enabled = false;
		}
		
		// anstate
		if(time + timeAdded < 1*DEFAULT_PHASE_LENGTH/4)
		{
			Debug.Log("anstate 1");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_1;
			agent.GetComponent<Animator>().SetInteger("anstate", 1);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time + timeAdded < 2*DEFAULT_PHASE_LENGTH/4)
		{
			Debug.Log("anstate 2");
			//agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_2;
			agent.GetComponent<Animator>().SetInteger("anstate", 2);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time + timeAdded < 3*DEFAULT_PHASE_LENGTH/4)
		{
			Debug.Log("anstate 3");
			//agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_3;
			agent.GetComponent<Animator>().SetInteger("anstate", 3);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time + timeAdded < 4*DEFAULT_PHASE_LENGTH/4)
		{
			Debug.Log("anstate 4");
			agent.GetComponent<Animator>().SetInteger("anstate", 4);
			//agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_4;
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		
		changeAvatarScale(agent, false);
	}
	
	/*
		dojde k presadeniu praticipanta a agenta
		agent replikuje pohyb a zvuk participanta z fazy 1
	*/
	async void phase2_start()
	{
		// zmen farby
		changeColor(avatar, 1);
		changeColor(agent, 2);
		
		//makeSeat(agent, avatar);
		makeSeat(agent, avatar);
		
		// uz sa to nemoze ukladat a prehravat na jednom mieste lebo je tam delay!
		// najprv sa to musi ulozit a po delay prehrat
		Debug.Log("PLAYING SAVED STUFF");

		agent.GetComponent<Animator>().enabled = false;
		
		// --------------------- lipsync (on file input)
		agent.GetComponent<Salsa>().audioSrc.Stop();
		Debug.Log(".WAV LOAD FROM: "+Application.persistentDataPath); 
		string filename = Path.Combine(Application.persistentDataPath, "sound.wav");
		AudioClip clip = await LoadAudioClip(filename);
		if (clip != null){
			agent.GetComponent<AudioSource>().clip = clip;
			Debug.Log(clip);
			agent.GetComponent<Salsa>().audioSrc.Play();
		}
		else{
			Debug.Log("je to null");
		}
		// -----------------------------------------------
		
		//scmanager.soundManager.Play("sound");
		scmanager.motionManager.Play("motion");
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
		// zmen farby
		changeColor(avatar, 4);
		changeColor(agent, 2);
		
		resetAddTime();
		wordcount = 0;
		
		makeSeat(agent, avatar);
		
		scmanager.soundManager.Record();
		
		scmanager.motionManager.Reset();
		scmanager.motionManager.Record();
		agent.GetComponent<Animator>().enabled = true;
		
		changeAvatarScaleFixed(agent, false);
	}

	void phase3()
	{
		if(time + timeAdded < 1*DEFAULT_PHASE_LENGTH/3)
		{
			Debug.Log("anstate 1");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_2;
			agent.GetComponent<Animator>().SetInteger("anstate", 1);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
			
			// zapnutie slz
			srm.isCrying = true;
		}
		else if(time + timeAdded < 2*DEFAULT_PHASE_LENGTH/3)
		{
			Debug.Log("anstate 2");
			//agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_2;
			agent.GetComponent<Animator>().SetInteger("anstate", 2);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
			
			// vypnutie slz
			srm.isCrying = false;
		}
		else if(time + timeAdded < 3*DEFAULT_PHASE_LENGTH/3)
		{
			Debug.Log("anstate 3");
			//agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_3;
			agent.GetComponent<Animator>().SetInteger("anstate", 3);
				//agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		/*
		else if(time + timeAdded < 4*DEFAULT_PHASE_LENGTH/4)
		{
			Debug.Log("anstate 4");
			//?
		}
		*/
	}
	
	/*
		dojde k presadeniu participanta a agenta
		agent replikuje pohyb a zvuk participanta z fazy 3
	*/
	async void phase4_start()
	{
		// zmen farby
		changeColor(avatar, 2);
		changeColor(agent, 4);
		
		makeSeat(avatar, agent);
	
		/*
		Debug.Log("SAVING & PLAYING");
		scmanager.soundManager.Save("sound");
		scmanager.motionManager.Save("motion");
		*/
		
		agent.GetComponent<Animator>().enabled = false;
		
		// --------------------- lipsync (on file input)
		agent.GetComponent<Salsa>().audioSrc.Stop();
		Debug.Log(".WAV LOAD FROM: "+Application.persistentDataPath); 
		string filename = Path.Combine(Application.persistentDataPath, "sound.wav");
		AudioClip clip = await LoadAudioClip(filename);
		if (clip != null){
			agent.GetComponent<AudioSource>().clip = clip;
			Debug.Log(clip);
			agent.GetComponent<Salsa>().audioSrc.Play();
		}
		else{
			Debug.Log("je to null");
		}
		// ---------------------------------------------
		
		//scmanager.soundManager.Play("sound");
		scmanager.motionManager.Play("motion");
		
		resetAvatarScale(agent);
	}
	void phase4()
	{}

	/*
		funkcia ktora sa zavola raz ked sa ukonci experiment
	*/
	void phaseEndOfExperiment_start()
	{
		Debug.Log("KONIEC EXPERIMENTU");
		SceneManager.LoadScene("Menu");
	}

	/* --------- tlacidlo --------- */
	void phasePause()
	{
		agent.SetActive(false);
		
		//startGUItimer();
		//timeLimit = 30.0f;
		srm.isGUITimerActive = true;
		//timeObject.SetActive(true);
	}
	void phaseUnpause()
	{
		agent.SetActive(true);
	}
}
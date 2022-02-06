using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sebaprotekcia : SingleRoom
{
	private bool isRecording;
	public SingleRoom srm;
	
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
		scmanager = srm.scmanager;
		seatPositionA = srm.seatPositionA;
		seatPositionB = srm.seatPositionB;
		seatRotationA = srm.seatRotationA;
		seatRotationB = srm.seatRotationB;
		
		
		phase = 1;
		time = 0;
		isRecording = false;
		
		seatPositionA = avatar.transform.position;
		seatPositionB = agent.transform.position;
		seatRotationA = avatar.transform.rotation;
		seatRotationB = agent.transform.rotation;
		
		phase1_start();
    }

    void Update()
    {
        this.time = this.time + Time.deltaTime;
		
		// nie moc efektivne riesenie
		time += wordcount;
		wordcount = 0;
		
		if(time > DEFAULT_PHASE_LENGTH) // nadišiel čas zmeniť fazu
		{
			time = 0;
			phase++;
			Debug.Log("Prepinam fazu, nova faza je: "+phase);
			
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
	
	/* ----------------------- funkcie jednotlivych faz ----------------------- */
	void phase1_start()
	{
	}
	void phase1()
	{
		// zapnutie nahravania zvuku a pohybu
		if(isRecording == false)
		{
			isRecording = true;
			scmanager.soundManager.Record();
			scmanager.motionManager.Record();
			
			// vypnutie fyziky
			avatar.GetComponent<Rigidbody>().isKinematic = true;
			agent.GetComponent<Rigidbody>().isKinematic = true;
		}
		
		// anstate
		if(time < 30)
		{
			Debug.Log("anstate 1");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_1;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time < 60)
		{
			Debug.Log("anstate 2");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_2;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time < 90)
		{
			Debug.Log("anstate 3");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_3;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else
		{
			Debug.Log("anstate 4");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase1_4;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		
		changeAvatarScale(agent, false);
	}
	
	/*
		dojde k presadeniu praticipanta a agenta
		agent replikuje pohyb a zvuk participanta z fazy 1
	*/
	void phase2_start()
	{
		//makeSeat(avatar, avatarA);
		
		Debug.Log("SAVING & PLAYING");
		
		scmanager.soundManager.Save("sound");
		scmanager.motionManager.Save("motion");
		
		agent.GetComponent<Animator>().enabled = false;
		
		scmanager.soundManager.Play("sound");
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
		//makeSeat(avatarA, avatar);
		
		scmanager.soundManager.Record();
		
		scmanager.motionManager.Reset();
		scmanager.motionManager.Record();
		agent.GetComponent<Animator>().enabled = true;
		
		changeAvatarScaleFixed(agent, false);
	}

	void phase3()
	{
		if(time < 30)
		{
			Debug.Log("anstate 1");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_1;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time < 60)
		{
			Debug.Log("anstate 2");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_2;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
		}
		else if(time < 90)
		{
			Debug.Log("anstate 3");
			agent.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPhase3_3;
			agent.GetComponent<Animator>().avatar = animatorAvatar1;
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
		
		agent.GetComponent<Animator>().enabled = false;
		
		scmanager.soundManager.Play("sound");
		scmanager.motionManager.Play("motion");
		
		resetAvatarScale(agent);
	}
	void phase4()
	{}

	/*
		funkcia ktora sa zavola raz ked sa ukonci experiment
	*/
	void phaseEndOfExperiment_start()
	{}
}
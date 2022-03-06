using System.Collections;
using System.Collections.Generic;
using Dp;
using UnityEngine;



public class ScManager : MonoBehaviour
{
	public float time;
	public bool isSaved = false;
	public SoundManager soundManager;
	public MotionManager motionManager;
	
	public GameObject player;
	public GameObject kid;
	public static AudioSource audioSource;


	public AvatarManager avatarManager; //Hajdinova Instancia
	public GameObject[] RigedAvatars;
	private GameObject activeAvatar = null;
	public GameObject[] RigedAvatarsChilds;

	// Start is called before the first frame update
	void Start()
    {

		avatarManager= GameObject.FindGameObjectsWithTag("AvatarManagerGameObject")[0].GetComponent<AvatarManager>();
		Debug.Log("Avatar cislo" + avatarManager.SelectedAvatar);

		player = GameObject.FindGameObjectsWithTag("Player")[0];

		if (activeAvatar != null)
			activeAvatar.SetActive(false);

		activeAvatar = RigedAvatars[avatarManager.SelectedAvatar];
		kid = RigedAvatarsChilds[avatarManager.SelectedAvatar];

		activeAvatar.SetActive(true);
		//StartCoroutine(Calibration.StartCalibration(activeAvatar.GetComponentInChildren<VRRig>(true), terminal, DefaultControllers));
	


		BVHRecorder recorder = player.GetComponent<BVHRecorder>();
		BVHAnimationLoader loader = kid.GetComponent<BVHAnimationLoader>();
		audioSource = this.GetComponent<AudioSource>();
		
		motionManager = new MotionManager(recorder, loader);
		soundManager = gameObject.AddComponent<SoundManager>();
	
		
		soundManager.Record();
		motionManager.Record();
	
        time = Time.time;
		kid.SetActive(false);


	}

    // Update is called once per frame
    void Update()
    {
		
        if(time > 60.0f && isSaved == false)
		{
			isSaved = true;
			kid.SetActive(true);
			player.SetActive(false);
			
			soundManager.Save("audio");
			soundManager.Play("audio");
			
			motionManager.Save("motion");
			motionManager.Play("motion");
		}
		time = time + Time.deltaTime;
		
    }
}
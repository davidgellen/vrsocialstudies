using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScManager : MonoBehaviour
{
	private float time;
	private bool isSaved = false;
	private SoundManager soundManager;
	private MotionManager motionManager;
	
	public GameObject player;
	public GameObject kid;
	public static AudioSource audioSource;
	
    // Start is called before the first frame update
    void Start()
    {
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
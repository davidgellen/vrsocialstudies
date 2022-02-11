using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
	public bool isClicked;
	public float time;
	public float timeClicked;
	
    // Start is called before the first frame update
    void Start()
    {
        isClicked = false;
		timeClicked = 0;
    }
	
    // Update is called once per frame
    void Update()
    {
		this.time = this.time + Time.deltaTime;
		
		if(isClicked && time - timeClicked > 5)
		{
			isClicked = false;
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.051f, this.transform.position.z);
		}
    }
	
	void OnTriggerEnter(Collider other)
    {
		if (other.tag == "button")
		return;
		
		Debug.Log("CLICK");
		
		if(!isClicked)
		{
			isClicked = true;
			timeClicked = time;
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.051f, this.transform.position.z);
		}
    }
}
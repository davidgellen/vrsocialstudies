using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager
{
	public BVHRecorder recorder;
	public BVHAnimationLoader loader;

	public MotionManager(BVHRecorder recorder, BVHAnimationLoader loader)
	{
		this.recorder = recorder;
		this.loader = loader;
	}
	
	public void Reset()
	{
		recorder.clearCapture();
		recorder.capturing = true;
	}
	
	public void Record()
	{
		this.recorder.capturing = true;
	}
	
	public void Save(string fileName)
	{
		recorder.capturing = false;
		//recorder.filename = "C:\\bvh\\"+fileName+".bvh";
		
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".bvh");
        // Delete the file if it exists.
        if (File.Exists(filePath))
        {
			File.Delete(filePath);
        }
		
		recorder.filename = filePath;
		recorder.saveBVH();
	}
	
	public void Play(string fileName)
	{
		loader.clipName = fileName;
		//loader.filename = "C:\\bvh\\"+fileName+".bvh";
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".bvh");
		loader.filename = filePath;
		loader.parseFile();
		loader.loadAnimation();
		loader.anim.Play(fileName);
	}
}
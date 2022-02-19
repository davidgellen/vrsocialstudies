using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using UnityEngine.Windows.Speech;
using CrazyMinnow.SALSA;
using CrazyMinnow.SALSA.OneClicks;

public class changeSoundSource : MonoBehaviour
{

    public GameObject avatar;
    public AudioClip ac;
    public float time = 0;
    // Start is called before the first frame update
    void Start()
    {



        /*Transform child = transform.GetChild(0);
        Debug.Log(child);
        /*var smr = child.GetComponent<SkinnedMeshRenderer>();

        if (smr == null
                || smr.sharedMesh == null
                || smr.sharedMesh.blendShapeCount == 0
                || smr.sharedMesh.GetBlendShapeIndex("saySml") == -1
                || smr.sharedMesh.GetBlendShapeIndex("sayMed") == -1
                || smr.sharedMesh.GetBlendShapeIndex("sayLrg") == -1)
            {
                Debug.Log("This object does not have the required components.");
                //return;
            }

        var audSrc = avatar.GetComponent<AudioSource>();
            if (audSrc == null)
                audSrc = avatar.AddComponent<AudioSource>();
            // Optional: set your audio source for your preferences...
            audSrc.playOnAwake = false;
            audSrc.loop = false;

        string filename = "file://" + Path.Combine(Application.streamingAssetsPath, "hlas1.wav");
			var www = new WWW(filename);
			var clip = www.GetAudioClip(false, false, AudioType.WAV);
        audSrc.clip = clip;
        // Salsa.audioSrc.Play();

        var qp = avatar.GetComponent<QueueProcessor>();
            if (qp == null)
                qp = avatar.AddComponent<QueueProcessor>();

            // Configure SALSA
            var salsa = avatar.GetComponent<Salsa>();
            if (salsa == null)
                salsa = avatar.AddComponent<Salsa>();

            var smr = salsa.skinnedMesh;
            salsa.audioSrc = audSrc;
            salsa.queueProcessor = qp;

            // adjust salsa settings
            //  - data analysis settings
            salsa.autoAdjustAnalysis = true;
            salsa.autoAdjustMicrophone = false;
            salsa.audioUpdateDelay = 0.0875f;
            //  - advanced dynamics
            salsa.loCutoff = 0.015f;
            salsa.hiCutoff = 0.75f;
            salsa.useAdvDyn = true;
            salsa.advDynPrimaryBias = 0.40f;
            salsa.useAdvDynJitter = true;
            salsa.advDynJitterAmount = 0.10f;
            salsa.advDynJitterProb = 0.20f;
            salsa.advDynSecondaryMix = 0.0f;

            // setup visemes
            salsa.visemes.Clear(); // start fresh

            // setup viseme 1 -- saySmall
            salsa.visemes.Add(new LipsyncExpression("saySmall", new InspectorControllerHelperData(), 0f));
            var saySmallViseme = salsa.visemes[0].expData; // cache the say small viseme...

            // Create an expression component for the say small viseme...
            // NOTE: when a viseme is created, the first component is automatically created 
            // and default values are used. The default control type is automatically set 
            // to shape (blendshape)...so we do not need to manually set it.
            saySmallViseme.components[0].name = "saySmall component";
            saySmallViseme.controllerVars[0].smr = smr;
            saySmallViseme.controllerVars[0].blendIndex = smr.sharedMesh.GetBlendShapeIndex("saySml");
            saySmallViseme.controllerVars[0].minShape = 0f;
            saySmallViseme.controllerVars[0].maxShape = 1f;

            // setup viseme 2 -- sayMedium
            salsa.visemes.Add(new LipsyncExpression("sayMedium", new InspectorControllerHelperData(), 0f));
            var sayMediumViseme = salsa.visemes[1].expData; // cache the say medium viseme...
            // create an expression component for the sayMedium viseme...
            sayMediumViseme.components[0].name = "sayMedium component";
            sayMediumViseme.controllerVars[0].smr = smr;
            sayMediumViseme.controllerVars[0].blendIndex = smr.sharedMesh.GetBlendShapeIndex("sayMed");
            sayMediumViseme.controllerVars[0].minShape = 0f;
            sayMediumViseme.controllerVars[0].maxShape = 1f;

            // ============================= Optional/theoretical example code:
            // To add extra components to a viseme,
            // add new .controllerVars and .components list elements
            // then adjust settings...
            // NOTE: the following adds the say large blendshape as a 
            // second component on the say medium viseme expression,
            // this is not practical and is only for demonstration purposes...
            // The first component's helper and component data entries are 
            // created automatically when a new viseme is created. When adding 
            // more components to a viseme, it is necessary to manually add 
            // the helper and component items to the respective lists.
            /*sayMediumViseme.controllerVars.Add(new InspectorControllerHelperData());
            sayMediumViseme.components.Add(new ExpressionComponent());
            // Now configure the second component...
            sayMediumViseme.components[1].name = "sayLarge component";
            sayMediumViseme.components[1].durationOn = .08f;
            sayMediumViseme.components[1].durationOff = .06f;
            // It is necessary to set the easing and control type on subsequently added components.
            sayMediumViseme.components[1].easing = LerpEasings.EasingType.CubicOut;
            sayMediumViseme.components[1].controlType = ExpressionComponent.ControlType.Shape;
            sayMediumViseme.controllerVars[1].smr = smr;
            sayMediumViseme.controllerVars[1].blendIndex = smr.sharedMesh.GetBlendShapeIndex("sayLrg");
            sayMediumViseme.controllerVars[1].minShape = 0f;
            sayMediumViseme.controllerVars[1].maxShape = 1f;
            // ====================================================== etc...

            // setup viseme 3 -- sayLarge
            salsa.visemes.Add(new LipsyncExpression("sayLarge", new InspectorControllerHelperData(), 0f));
            var sayLargeViseme = salsa.visemes[2].expData;
            sayLargeViseme.components[0].name = "sayLarge component";
            sayLargeViseme.controllerVars[0].smr = smr;
            sayLargeViseme.controllerVars[0].blendIndex = smr.sharedMesh.GetBlendShapeIndex("sayLrg");
            sayLargeViseme.controllerVars[0].minShape = 0f;
            sayLargeViseme.controllerVars[0].maxShape = 1f;

            // apply api trigger distribution...
            salsa.DistributeTriggers(LerpEasings.EasingType.SquaredIn);
            // at runtime: apply controller baking...
            salsa.UpdateExpressionControllers();
        

        salsa.audioSrc.Play();*/

        /*var salsa = avatar.GetComponent<Salsa>();
        salsa.waitForAudioSource = true;
        

        var audSrc = avatar.GetComponent<AudioSource>();
        string filename = "file://" + Path.Combine(Application.dataPath, "/Scripts/ADHD/hlas1.wav");
			var www = new WWW(filename);
			var clip = www.GetAudioClip(false, false, AudioType.WAV);
        audSrc.clip = clip;

        salsa.audioSrc = audSrc;*/

        

    }

    // Update is called once per frame
    void Update()
    {
        /*time = time + Time.deltaTime;

        if (time>1 && time<3){
            avatar.GetComponent<Salsa>().audioSrc.Stop();
            avatar.GetComponent<AudioSource>().clip = ac;
            Debug.Log("Audio changed");
            avatar.GetComponent<Salsa>().audioSrc.Play();
        }*/
    }
}
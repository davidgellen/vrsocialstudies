using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows.Speech;

public class mictest : MonoBehaviour
{
    [SerializeField]
    private Text m_Hypotheses;

    [SerializeField]
    private Text m_Recognitions;

    private DictationRecognizer m_DictationRecognizer;

    private int wordcount;

    void recordStuff()
    {
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
            if (completionCause != DictationCompletionCause.Complete)
            {
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }
            if (completionCause == DictationCompletionCause.Canceled ||
                completionCause == DictationCompletionCause.TimeoutExceeded)
            {
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
        recordStuff();
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_Credit : MonoBehaviour
{

    public GameObject texts;
    private float curTime;
    private Vector3 curPos;

    // Start is called before the first frame update
    void Start()
    {
        curTime = 0;
        curPos = texts.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        RollingTexts();
    }

    void RollingTexts()
    {
        texts.transform.Translate(0, 60 * Time.deltaTime, 0);
        curTime += Time.deltaTime;

        if (curTime >= 40f)
        {
            texts.transform.position = curPos;
            curTime = 0;
        }

    }
}

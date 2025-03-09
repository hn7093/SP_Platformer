using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeTestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI normal;
    [SerializeField] private TextMeshProUGUI test;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button stopBtn;
    bool walking = false;
    public float scale = 1.0f;
    float testNumber = 0f;
    float normalNumber = 0f;
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(OnPause);
        stopBtn.onClick.AddListener(OnReset);
    }

    // Update is called once per frame
    void Update()
    {
        if (walking)
        {
            walking = true;
            normalNumber += Time.deltaTime;
            testNumber += Time.deltaTime * scale;
            normal.text = normalNumber.ToString();
            test.text = testNumber.ToString();
        }
    }
    public void OnPause()
    {
        walking = !walking;
    }
    public void OnReset()
    {
        walking = true;
        testNumber = 0f;
        normalNumber = 0f;
        normal.text = normalNumber.ToString();
        test.text = testNumber.ToString();
    }
}

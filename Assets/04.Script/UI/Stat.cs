using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// 슬라이더에 사용될 스텟
public class Stat : MonoBehaviour
{
    public float curValue;
    public float passiveValue; // 자동 변화량
    [SerializeField] private float startValue = 100;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private Image fill;
    private Color originColor;

    //components
    private Slider imgBar;
    private void Awake()
    {
        imgBar = GetComponent<Slider>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        curValue = startValue;
        imgBar.value = GetPercent();
        originColor = fill.color;
    }
    private float GetPercent()
    {
        return curValue / maxValue;
    }
    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
        imgBar.value = GetPercent();
    }
    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
        imgBar.value = GetPercent();
    }
    public bool IsFull()
    {
        return curValue >= maxValue;
    }

    // 게이지를 회색으로 변경, 원복
    public void SetGray(bool isGray)
    {
        if (isGray)
        {
            //(0.299f * R) + (0.587f * G) + (0.114f * B)
            if (fill != null)
            {
                Color gray = new Color(1,1,1,1);
                gray.r = 0.299f * originColor.r;
                gray.g = 0.587f * originColor.g;
                gray.b = 0.114f * originColor.b;
                gray.a = originColor.a;
                fill.color = gray;
            }
        }
        else
        {
            if (fill != null)
            {
                fill.color = originColor;
            }
        }
    }
}

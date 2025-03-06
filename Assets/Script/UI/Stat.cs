using UnityEngine;
using UnityEngine.UI;
public class Stat : MonoBehaviour
{
    public float curValue;
    public float passiveValue; // 자동 변화량
    [SerializeField] private float startValue = 100;
    [SerializeField] private float maxValue = 100;

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
}

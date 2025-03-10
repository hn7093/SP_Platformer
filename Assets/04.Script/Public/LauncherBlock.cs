using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트에 시간 간격을 두고 힘을 가하는 발사대
[RequireComponent(typeof(Animator))]
public class LauncherBlock : MonoBehaviour
{
    
    public float interval = 1;
    public float launchForce = 1;
    List<Rigidbody> objs = new List<Rigidbody>();
    private int _animAction = Animator.StringToHash("Launch");
    private Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        InvokeRepeating("OnLaunch", 0, interval);
    }
    void OnLaunch()
    {
        for(int i = 0; i<objs.Count; i++)
        {
            objs[i].AddForce(transform.up * launchForce, ForceMode.Impulse);
        }
        _animator.SetTrigger(_animAction);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rigidbody))
        {
            objs.Add(rigidbody);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rigidbody))
        {
            objs.Remove(rigidbody);
        }
    }

}

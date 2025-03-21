using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Animator))]
public class InteractObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactName;
    [SerializeField] private string description;
    public UnityEvent<bool> interAction;
    private Animator _animator;
    private int _animAction = Animator.StringToHash("Action");
    private bool _nowAct = false;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void Interact()
    {
        _nowAct = !_nowAct;
        _animator.SetBool(_animAction, _nowAct);
        interAction?.Invoke(_nowAct);
    }

    public string GetInteractPrompt()
    {
        string str = $"{interactName}\n{description}";
        return str;
    }
}

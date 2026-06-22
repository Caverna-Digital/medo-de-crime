using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;
public class NPCAnimation : MonoBehaviour
{
    public List<RuntimeAnimatorController> controllers;
    public int controller = 0;
    public int value = 0;
    public string parameter = "Type";

    private Animator animator;
    private int c;
    private int a;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = controllers[0] as RuntimeAnimatorController;
        animator.SetInteger(parameter, value);
    }
    private void Update()
    {
        if (controller != c)
        {
            animator.runtimeAnimatorController = controllers[controller] as RuntimeAnimatorController;
            c = controller;
        }

        if (value != a)
        {
            animator.SetInteger(parameter, value);
            a = value;
        }
    }

}
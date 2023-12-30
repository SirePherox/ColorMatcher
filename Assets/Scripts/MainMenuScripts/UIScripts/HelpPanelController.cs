using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HelpPanelController : MonoBehaviour
{
    [Header("References")]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel()
    {
        animator.SetTrigger("Open");
    }

    public void ClosePanel()
    {
        animator.SetTrigger("Close");
    }
}

using System;
using UnityEngine;
public class ShipController : MonoBehaviour
{
    public static ShipController Instance;

    [SerializeField] public GameObject controllingObject;

    private void Awake()
    {
        if(Instance != null & Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Controll();
    }

    private void Controll()
    {
        try
        {
            controllingObject.GetComponent<IController>().Controll();
        }
        catch (Exception e)
        {
            Debug.Log(controllingObject.name + "Cannot be controlled." + e.Message);
        }
    }
}
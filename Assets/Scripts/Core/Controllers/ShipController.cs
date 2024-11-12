using System;
using UnityEngine;
public class ShipController : MonoBehaviour
{
    [SerializeField] public GameObject controllingObject;

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
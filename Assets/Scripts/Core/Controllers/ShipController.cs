using System;
using UnityEngine;
public class ShipController : MonoBehaviour
{
    public static ShipController Instance;

    public delegate void ExperienceChangeHandler(float xpAmount);
    public event ExperienceChangeHandler OnExperienceChange;

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

    public void AddExperience(float xpAmount)
    {
        OnExperienceChange?.Invoke(xpAmount);
    }
}
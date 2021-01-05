using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public GameObject handModelPrefab;

    public InputDeviceCharacteristics userCharacteristics;

    private InputDevice selectedDevice;
    private GameObject spawnedHandModel;

    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
    }

    void initialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(userCharacteristics,devices);

        selectedDevice = devices[0];
        spawnedHandModel = Instantiate(handModelPrefab, transform);
        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }

    void UpdateHandAnimation()
    {
        if (selectedDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (selectedDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnedHandModel)
        {
            spawnedHandModel.SetActive(true);
            UpdateHandAnimation();
        }
    }
}
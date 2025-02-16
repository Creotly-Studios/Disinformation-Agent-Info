using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt, LootAtInverted, CameraFwd, CameraFwdInverted
    }

    [SerializeField] private Mode mode;
    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break; 
            case Mode.LootAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraFwd:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraFwdInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}

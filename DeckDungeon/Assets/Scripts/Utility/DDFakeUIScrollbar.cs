using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDFakeUIScrollbar : MonoBehaviour
{
    [SerializeField]
    private DDShowDeckArea area;

    [SerializeField]
    private Transform top;

    [SerializeField]
    private Transform bottom;

    [SerializeField]
    private Transform scroller;

    private bool grabbed = false;

    private float value = 1;
    public float Value { get { return value; } }

    private void Update()
    {
        if(grabbed)
        {
            Vector3 pos = scroller.position;
            pos.z = Mathf.Clamp(area.CamRay.origin.z, bottom.position.z, top.position.z);
            value = (pos.z - bottom.position.z) / (top.position.z - bottom.position.z);
            // Value is 1 up top and 0 down bottom
            scroller.position = pos;
        }
    }

    public void AreaOpened()
    {
        grabbed = false;
        value = 1;
    }

    public void GrabbedBar(bool didGrab)
    {
        grabbed = didGrab;
    }
}

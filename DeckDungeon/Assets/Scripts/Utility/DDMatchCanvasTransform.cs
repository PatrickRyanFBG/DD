using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDMatchCanvasTransform : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform matchingObject;
    private Vector3 lastLocalPosition;
    
    private void Update()
    {
        CheckPosition();
    }

    private void CheckPosition()
    {
        //if (lastLocalPosition != matchingObject.localPosition)
        {
            lastLocalPosition = matchingObject.localPosition;
            
            Vector2 screenPos = new Vector2(matchingObject.anchorMin.x * Screen.width, matchingObject.anchorMin.y * Screen.height);
            screenPos.x += matchingObject.anchoredPosition.x;
            screenPos.y += matchingObject.anchoredPosition.y;
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane p = new Plane(Vector3.up, Vector3.up);
            if (p.Raycast(ray, out float dist))
            {
                Vector3 pos = ray.GetPoint(dist);
                transform.position = pos;
            }
            //pos.y = transform.position.y;
            //transform.position = pos;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 200), Input.mousePosition.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Vector3 previousPosition = Vector3.zero;
    Vector3 moveDelta = Vector3.zero;
    float speed = 10f;

    float cameraDistanceMax = -3f;
    float cameraDistanceMin = -6f;
    float cameraDistance = -3f;
    float scrollSpeed = 0.5f;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            moveDelta = Input.mousePosition - previousPosition;

            //Normally, if I rotate upward, 90 degrees, then rotating horizontally will be inverted
            //To fix it, use the following condition
            if (Vector3.Dot(transform.up , Vector3.up) >= 0)
            {

                transform.Rotate(transform.up, -Vector3.Dot(moveDelta, Camera.main.transform.right) * speed * Time.deltaTime, Space.World);
            } else
            {
                transform.Rotate(transform.up, Vector3.Dot(moveDelta, Camera.main.transform.right) * speed * Time.deltaTime, Space.World);
            }

            //rotate up/down
            transform.Rotate(Camera.main.transform.right, Vector3.Dot(moveDelta, Camera.main.transform.up) * speed * Time.deltaTime, Space.World);
        }

        previousPosition = Input.mousePosition;


        //Camera zoom in/out
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        Camera.main.transform.position = Vector3.forward * cameraDistance;
    }
}

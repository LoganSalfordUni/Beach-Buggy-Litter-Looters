using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[SerializeField] Transform playerBody;
    //[SerializeField] float followStrength;

    [SerializeField] Transform followTransform;
    private void Update()
    {
        /*Vector3 desiredPosition = new Vector3(playerBody.transform.position.x + 20f, this.transform.position.y, this.transform.position.z);

        this.transform.position = Vector3.Lerp(this.transform.position, desiredPosition, Time.deltaTime * followStrength);*/

        this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);
    }

    [ContextMenu("Show camera position")]
    void ShowCameraPos()
    {
        this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, followTransform.position.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWalls : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    private void Update()
    {
        this.transform.position = new Vector3(playerBody.transform.position.x, 0f, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCollisions : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        player = GetComponent<PlayerMovement>().gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            if(this.gameObject.tag == "UpperClassWin")
            {
                SceneManager.LoadScene("MiddleClassScene");
            }

            if(this.gameObject.tag == "MiddleClassWin")
            {
                SceneManager.LoadScene("EndScene");
            }
    }
}

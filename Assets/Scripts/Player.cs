using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(0, 0, moveSpeed*Time.deltaTime);
        else if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(0, 0, -moveSpeed*Time.deltaTime);
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Translate(moveSpeed*Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(-moveSpeed*Time.deltaTime, 0, 0);
        else
        {
                // Debug.Log("Please press arrow key to move.");
        }
    }
    private void OnCollisionStay(Collision other) {
        if (other.gameObject.tag == "infield")
        {
            Debug.Log("Inside");
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "outfield")
        {
            Debug.Log("Outside");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "deadline")
        {
            Debug.Log("Falling out");
        }
    }
    
}

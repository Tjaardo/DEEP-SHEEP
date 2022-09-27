using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    private bool isEaten;
    // Start is called before the first frame update
    void Start()
    {
        isEaten = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isEaten && other.tag == "Sheep")
        {
            isEaten = true;
            Debug.Log("Wird gegessen");
            other.gameObject.GetComponent<Member>().hasEaten = true;
            Destroy(this.gameObject);
        }
    }
}

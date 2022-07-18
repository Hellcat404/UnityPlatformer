using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private static System.Random rand = new System.Random();

    public GameObject parentPlat;

    private Rigidbody rb;

    private bool left = true;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.velocity = new Vector3(2f,rb.velocity.y,0f);
        StartCoroutine(timer());
    }

    // Update is called once per frame
    void Update () {
        if(transform.position.x < parentPlat.GetComponent<Renderer>().bounds.min.x + 0.3) {
            left = false;
        }else if(transform.position.x > parentPlat.GetComponent<Renderer>().bounds.max.x - 0.3){ 
            left = true;
        }
	}

    IEnumerator timer(){
        while(true){
            if(left) {
                rb.velocity = new Vector3(-1f,rb.velocity.y,0f);
            }else{ 
                rb.velocity = new Vector3(1f,rb.velocity.y,0f);
            }

            yield return new WaitForSeconds(0.2f);

            if(rand.NextDouble() > 0.98) { left = !left; }
            if(rand.NextDouble() > 0.98) { rb.velocity = new Vector3(rb.velocity.x, 5f, 0f); };
        }
    }
}

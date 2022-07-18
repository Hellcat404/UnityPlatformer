    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playercontroller : MonoBehaviour {

    public float maxSpeed = 5;

    public GameObject finishLine;
    public GameObject world;

    private Rigidbody rb;
    private bool canJump = false;

    private Vector3 respawnLoc = new Vector3(0,1,0);

    private List<GameObject> playerCubes = new List<GameObject>();

    private void Start(){
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            if(rb.velocity.x < maxSpeed)
            {
                rb.velocity += new Vector3(0.2f,0,0);
            }
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            if(rb.velocity.x > -maxSpeed)
            {
                rb.velocity += new Vector3(-0.2f,0,0);
            }
        }
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)){
            if(canJump){
                rb.velocity = new Vector3(rb.velocity.x,5,rb.velocity.z);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
        if(Input.GetKeyDown(KeyCode.X)) {
            world.GetComponent<LevelGen>().LoadLevel();
        }

        if(transform.position.y <= -10){
            transform.position = respawnLoc;
            rb.velocity = new Vector3(0,0,0);
            world.GetComponent<LevelGen>().resetDrops();
            world.GetComponent<LevelGen>().resetEnemies();
            world.GetComponent<LevelGen>().Attempts++;
            world.GetComponent<LevelGen>().RedrawAttempts();
        }
	}

    private void OnCollisionEnter(Collision collision){
        canJump = true;
        if(collision.gameObject == finishLine){
            StartCoroutine(finishLineReached());
        }
        if(world.GetComponent<LevelGen>().isDropPlat(collision.gameObject)) {
            StartCoroutine(Drop(collision.gameObject));
        }
        if(world.GetComponent<LevelGen>().isEnemy(collision.gameObject)) {
            StartCoroutine(enemyHit());
        }
        if(world.GetComponent<LevelGen>().isCheckpoint(collision.gameObject)) {
            collision.gameObject.GetComponent<Renderer>().material = world.GetComponent<LevelGen>().checkpointOnMat;
            respawnLoc = new Vector3(collision.gameObject.transform.position.x,collision.gameObject.transform.position.y + 1, collision.gameObject.transform.position.z);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        canJump = false;
    }

    IEnumerator enemyHit(){ 
        Camera.main.gameObject.GetComponent<CameraTracking>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.detectCollisions = false;
        for(int i = 0; i < 20; i++){
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float cubeScale = 0.8f;
            cube.transform.localScale -= new Vector3(cubeScale,cubeScale,cubeScale);
            cube.transform.position = transform.position;
            cube.GetComponent<Renderer>().material = world.GetComponent<LevelGen>().EnemyMat;
            cube.AddComponent<Rigidbody>().useGravity = false;
            GameObject.Find("World").GetComponent<LevelGen>().objects.Add(cube);
            playerCubes.Add(cube);
        }

        yield return new WaitForSeconds(3);

        rb.detectCollisions = true;
        world.GetComponent<LevelGen>().Attempts++;
        world.GetComponent<LevelGen>().RedrawAttempts();
        transform.position = respawnLoc;
        rb.velocity = new Vector3(0,0,0);
        world.GetComponent<LevelGen>().resetDrops();
        world.GetComponent<LevelGen>().resetEnemies();
        GetComponent<MeshRenderer>().enabled = true;
        Camera.main.gameObject.GetComponent<CameraTracking>().enabled = true;
        foreach(GameObject playerCube in playerCubes) {
            Object.Destroy(playerCube);
        }
        playerCubes.Clear();
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    IEnumerator finishLineReached(){ 
        Camera.main.gameObject.GetComponent<CameraTracking>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        rb.detectCollisions = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
                for(int i = 0; i < 20; i++){
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    float cubeScale = 0.8f;
                    cube.transform.localScale -= new Vector3(cubeScale,cubeScale,cubeScale);
                    cube.transform.position = transform.position;
                    cube.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
                    cube.AddComponent<Rigidbody>().useGravity = false;
                    GameObject.Find("World").GetComponent<LevelGen>().objects.Add(cube);
                }

            yield return new WaitForSeconds(3);

            world.GetComponent<LevelGen>().Level++;
            world.GetComponent<LevelGen>().LoadLevel();
    }

    IEnumerator Drop(GameObject gameObject){ 
        yield return new WaitForSeconds(0.2f);

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

}

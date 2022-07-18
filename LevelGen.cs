using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGen : MonoBehaviour {

    private static System.Random rand = new System.Random();

    public GameObject levelText;
    public GameObject attemptText;

    public Material StartPlatformMat;
    public Material checkpointOffMat;
    public Material checkpointOnMat;
    public Material PlatMat;
    public Material DropPlatMat;
    public Material FinishPlatMat;
    public Material PlayerMat;
    public Material EnemyMat;

    private bool randomPlats = false;

    public int PlatCount;
    public int Level = 1;
    public int Attempts = 0;

    public GameObject checkpointPlat;

    public List<GameObject> objects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> dropPlats = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> enemies = new Dictionary<GameObject, Vector3>();

	// Use this for initialization
	void Start () {

        Platform.StartPlatformMat = StartPlatformMat;
        Platform.checkpointOffMat = checkpointOffMat;
        Platform.checkpointOnMat = checkpointOnMat;
        Platform.PlatMat = PlatMat;
        Platform.DropPlatMat = DropPlatMat;
        Platform.FinishPlatMat = FinishPlatMat;

        LoadLevel();
	}

    public void LoadLevel(){
        
        PlatCount = rand.Next(2,4) * Level;

        if(objects.Count > 0){
            foreach(GameObject obj in objects){
                Object.Destroy(obj);
            }
        }

        if(enemies.Count > 0) {
            enemies.Clear();
        }

        if(dropPlats.Count > 0) {
            dropPlats.Clear();
        }

		GameObject startPlat = Platform.createPlatform(PlatformType.Start);
        objects.Add(startPlat);

        Attempts = 0;
        levelText.transform.position = new Vector3(startPlat.transform.position.x, startPlat.transform.position.y + 2, 1);
        attemptText.transform.position = new Vector3(levelText.transform.position.x, levelText.transform.position.y - 1, 1);
        levelText.GetComponent<Text>().text = "Level: " + Level.ToString();
        attemptText.GetComponent<Text>().text = "Attempts: " + Attempts.ToString();

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.transform.localScale -= new Vector3(0.5f,0.5f,0.5f);
        player.transform.position = new Vector3(0,1,0);
        player.GetComponent<Renderer>().material = PlayerMat;

        objects.Add(player);

        GameObject prevPlat = startPlat;
        Vector3 prevPlatEnd = startPlat.GetComponent<Renderer>().bounds.max;
        checkpointPlat = null;
        float yOffset = 0;
        for(int i = 0; i < PlatCount; i++){
            GameObject plat = null;

            yOffset = (float)rand.NextDouble();
            if(rand.NextDouble() > 0.5){ yOffset = -yOffset; }
            if(rand.NextDouble() > 0.8){
                if(isDropPlat(prevPlat)) {
                    plat = Platform.createPlatform(PlatformType.Normal);
                    yOffset = (float)(rand.Next(-1, 0) - rand.NextDouble());
                    plat.transform.position = new Vector3(prevPlatEnd.x + (float)rand.NextDouble() + plat.GetComponent<Renderer>().bounds.size.x,prevPlatEnd.y + yOffset,0f);
                }else{
                    plat = Platform.createPlatform(PlatformType.Drop);
                    plat.transform.position = new Vector3(prevPlatEnd.x + (float)rand.NextDouble() + plat.GetComponent<Renderer>().bounds.size.x,prevPlatEnd.y + yOffset - 0.5f,0f);
                    dropPlats.Add(plat, plat.transform.position);
                }
            }else{
                if(PlatCount >= 10 && i >= PlatCount/2 && checkpointPlat == null) {
                    plat = Platform.createPlatform(PlatformType.Checkpoint);
                    plat.transform.position = new Vector3(prevPlatEnd.x + (float)rand.NextDouble() + plat.GetComponent<Renderer>().bounds.size.x,prevPlatEnd.y + yOffset,0f);
                    checkpointPlat = plat;
                }else{
                    plat = Platform.createPlatform(PlatformType.Normal);
                    plat.transform.position = new Vector3(prevPlatEnd.x + (float)rand.NextDouble() + plat.GetComponent<Renderer>().bounds.size.x,prevPlatEnd.y + yOffset,0f);
                }
            }

            plat.GetComponent<Rigidbody>().useGravity = false;
            plat.GetComponent<Rigidbody>().isKinematic = false;
            prevPlat = plat;
            prevPlatEnd = plat.GetComponent<Renderer>().bounds.max;

            if(rand.NextDouble() > 0.9 && !isDropPlat(plat) && !isCheckpoint(plat)) {
                GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                enemy.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                enemy.transform.position = new Vector3(plat.transform.position.x, plat.transform.position.y + 1, plat.transform.position.z);
                enemy.AddComponent<Rigidbody>();
                enemy.GetComponent<Renderer>().material = EnemyMat;
                enemy.AddComponent<EnemyController>().parentPlat = plat;

                enemies.Add(enemy, enemy.transform.position);
                objects.Add(enemy);
            }
            
            objects.Add(plat);
        }

        GameObject finishPlat = Platform.createPlatform(PlatformType.Finish);
        yOffset = (float)rand.NextDouble();
        if(rand.NextDouble() > 0.5){ yOffset = -yOffset; }
        finishPlat.transform.position = new Vector3(prevPlatEnd.x + (float)rand.NextDouble() + 4,prevPlatEnd.y + yOffset,0f);

        objects.Add(finishPlat);

        GameObject mc = Camera.main.gameObject;
        if(!mc.GetComponent<CameraTracking>()){
            mc.AddComponent<CameraTracking>();
        }
        mc.GetComponent<CameraTracking>().player = player;
        mc.GetComponent<CameraTracking>().enabled = true;

        player.AddComponent<Playercontroller>();
        player.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
        player.GetComponent<Playercontroller>().finishLine = finishPlat;
        player.GetComponent<Playercontroller>().world = gameObject;
    }

    public void resetDrops(){
        foreach(KeyValuePair<GameObject, Vector3> dropPlat in dropPlats){
            dropPlat.Key.GetComponent<Rigidbody>().useGravity = false;
            dropPlat.Key.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
            dropPlat.Key.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            dropPlat.Key.transform.position = dropPlat.Value;
        }
    }

    public void resetEnemies(){
        foreach(KeyValuePair<GameObject, Vector3> enemy in enemies) {
            enemy.Key.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
            enemy.Key.transform.position = enemy.Value;
        }
    }

    public bool isDropPlat(GameObject go){ 
        foreach(KeyValuePair<GameObject, Vector3> dropPlat in dropPlats){
            if(dropPlat.Key == go) {
                return true;
            }
        }
        return false;
    }

    public bool isEnemy(GameObject go){
        foreach(KeyValuePair<GameObject, Vector3> enemy in enemies) {
            if(enemy.Key == go) {
                return true;
            }
        }
        return false;
    }

    public bool isCheckpoint(GameObject go){
        if(checkpointPlat == go) {
            return true;
        }
        return false;
    }

    public void RedrawAttempts(){ 
        attemptText.GetComponent<Text>().text = "Attempts: " + Attempts.ToString();
    }
}

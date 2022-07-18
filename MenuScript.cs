using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public int platCount = 0;

	public void Enter(){ 
        SceneManager.SetActiveScene(gameObject.scene);
        GameObject[] gos = SceneManager.GetSceneAt(1).GetRootGameObjects();
        for(int i = 0; i < gos.Length; i++){
            if(gos[i].name.Equals("World")){ 
                gos[i].GetComponent<LevelGen>().PlatCount = platCount;
                gos[i].GetComponent<LevelGen>().LoadLevel();
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                SceneManager.UnloadSceneAsync("__MENU");
            }
        }
    }

    public void Settings(){ 
        
    }
}

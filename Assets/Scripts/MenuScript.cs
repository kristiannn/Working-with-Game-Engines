using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Scene1()
    {
        Application.LoadLevel("Scene1");
    }

    public void Scene2()
    {
        Application.LoadLevel("Scene2");
    }

    public void Scene3()
    {
        Application.LoadLevel("Scene3");
    }

    public void MenuScene()
    {
        Application.LoadLevel("MainMenu");
    }
}

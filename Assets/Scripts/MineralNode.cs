using UnityEngine;
using System.Collections;

public class MineralNode : MonoBehaviour {

	public float minerals;
	public float yieldRate = 2;

	// Use this for initialization
	void Start () {
		minerals = Random.Range(50, 100);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

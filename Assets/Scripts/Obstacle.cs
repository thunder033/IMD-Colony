using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float radius = 1.414f;  //hard coded or set in inspector for now

	// We will follow a convention that setters and getters, which
	// are functions, will have the same name as the variable they
	// reference, but with an upper-case initial letter.

	public float Radius
	{
		get { return radius; }
	}


}

using UnityEngine;
using System.Collections;


public class GuardScript : EnemyScript {
	public GameObject copSpawner;
	private CopSpawnerScript copSpawnerScript;

	public void Reset(){
		doDefaultPatrol = false;
		doDefaultAlert = false;
	}

	new void Start () {
		base.Start ();
		copSpawnerScript = copSpawner.GetComponent<CopSpawnerScript> ();
	}


	public override void Patrol(){
		DefaultPatrol ();
	}
	
	public override void Investigate(){

	}
	
	public override void Alert(){
//		radio other guards

//		call 911 and summon cops in some minutes
		copSpawnerScript.call(gameObject);

//		do default alert behavior
		DefaultAlert ();
	}
}

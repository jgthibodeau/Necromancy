using UnityEngine;
using System.Collections;

public class CopScript : EnemyScript {

	public void Reset(){
		doDefaultAlert = false;
		doDefaultPatrol = false;
	}

	public override void Patrol(){
		//visit all cop nodes 1 at a time pseudo-randomly

		DefaultPatrol ();
	}
	
	public override void Investigate(){

	}
	
	public override void Alert(){
//		radio other guards
//		do default alert behavior
		DefaultAlert ();
	}
}

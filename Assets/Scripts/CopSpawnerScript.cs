using UnityEngine;
using System.Collections;

public class CopSpawnerScript : MonoBehaviour {
	public Transform copPath;
	public GameObject cop;
	private bool called = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void call(GameObject caller){
		if (!called) {
			called = true;
			for (int i = 0; i < 3; i++) {

				CopScript copScript = cop.GetComponentInChildren<CopScript> ();
				copScript.enemydata.currentState = EnemyScript.State.Patrol;
				copScript.agentController.SetWaypointParent(copPath);

				GameObject copClone = (GameObject)Instantiate (cop, transform.position, transform.rotation);
				copClone.transform.position += new Vector3 (Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
			}
		}
	}
}

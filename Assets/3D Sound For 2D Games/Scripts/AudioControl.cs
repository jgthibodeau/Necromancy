using UnityEngine;
using System.Collections;
using EcholocationLtd3DFor2DUnity4; //Add plugin namespace

public class AudioControl : MonoBehaviour {

    private GameObject go1, go2, go3;
	private _3DSoundFor2DGames sound1, sound2, sound3;
	private Rigidbody rb1, rb2, rb3;

    void Start ()
    {
        go1 = GameObject.Find("Ball 1"); //Get 'Ball 1' game object
		go2 = GameObject.Find("Ball 2"); //Get 'Ball 2' game object
		go3 = GameObject.Find("Ball 3"); //Get 'Ball 3' game object
		rb1 = (Rigidbody)go1.GetComponent(typeof(Rigidbody)); //Get rigid body components
		rb2 = (Rigidbody)go2.GetComponent(typeof(Rigidbody));
		rb3 = (Rigidbody)go3.GetComponent(typeof(Rigidbody));
		sound1 = (_3DSoundFor2DGames)go1.GetComponent(typeof(_3DSoundFor2DGames)); //Get individual 3D sound processing components
		sound2 = (_3DSoundFor2DGames)go2.GetComponent(typeof(_3DSoundFor2DGames));
		sound3 = (_3DSoundFor2DGames)go3.GetComponent(typeof(_3DSoundFor2DGames));
    }

    void Update ()
    {
        sound1.Play3D(go1); //Play clip assinged to 'Ball 1' using it's position as input to 3D sound engine
        sound2.Play3D(go2, 0.9f); //Demonstrates overloading function with selectable volume

		//Test for left mouse button click
		//Also allows the user to pick up a ball, move it and drop it
		//If the user clicks in the space outside a ball, the clip linked to Ball 3 will play in a 3D sound position that shadows this ball

		if (Input.GetMouseButton(0))
        {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Calculate ray from mouse cursor throuhg the screen
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Find position on the screen of mouse cursor

			//Test if the ray hit any Game Object, and the mouse cursor is within the boudary
			if (Physics.Raycast(ray, out hit) && pos.x < 3.6f && pos.x > -3.5f && pos.y < 6.0f && pos.y > -2.9f)
            {
				//If so, assign position of mouse cursor to position of selected game object
				//This will allow any ball to be grabbed and moved about the screen, then released and left to bounce around
                if(hit.collider.gameObject == go1)
				{
					go1.transform.position = new Vector3(pos.x, pos.y, 2.5f);
					rb1.velocity = new Vector3(0f, 0f, 0f);
				}
				if(hit.collider.gameObject == go2)
				{
					go2.transform.position = new Vector3(pos.x, pos.y, 2.5f);
					rb2.velocity = new Vector3(0f, 0f, 0f);
				}
				if(hit.collider.gameObject == go3)
				{
					go3.transform.position = new Vector3(pos.x, pos.y, 2.5f);
					rb3.velocity = new Vector3(0f, 0f, 0f);
				}
            }
			else
            {
				//If there is no ray hit, play the 3D sound assinged for 'Ball 3'
                sound3.Play3D(go3);
            }
        }

		if (Input.GetKey("escape"))	Application.Quit();
    }
}

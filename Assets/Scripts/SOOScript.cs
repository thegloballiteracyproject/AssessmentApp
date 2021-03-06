﻿using UnityEngine;
using System.Collections;
/* SOOScript : Observer

 * Inherits from Observer so it can listen for and respond to events
 */
/// <summary>
/// SOO script. Extends Observer.
/// Class that contains functionality for the Stimulus Organizational Object, the wrapper object for a question's four selectable objects.
/// </summary>
public class SOOScript : Observer {

	private GameObject[] stimArray = new GameObject[4];
	private Vector3 [] posArray = new Vector3[4];
	private Vector3[] destArray = new Vector3[4];
	private int questionNumber;
	bool isDraggable;
	Vector3 curDest;
	public float marginOfError;
	public float speed;
	bool moving;
	public Subject eventWrapper;
	bool movingAlreadyFalse;

	void Start () {
		isDraggable = false;
		movingAlreadyFalse = false;
	}

// Methods for accessing variables
	public bool getDrag ()
	{
		return isDraggable;
	}
	
	public int getQNumber ()
	{
		return questionNumber;
	}

	public GameObject[] returnStimArray() {
		return stimArray;
	}

	public Vector3[] returnPosArray() {
		return posArray;
	}

	public Vector3[] returnDestArray() {
		return destArray;
	}

// Methods for setting variables
	
	public void setQNumber (int num)
	{
		questionNumber = num;
	}

	public void setStimArray(GameObject[] input) {
		stimArray = input;
	}

	public void setPosArray(Vector3[] input) {
		posArray = input;
	}

	public void setDestArray(Vector3[] input) {
		destArray = input;
	}
	

	public void setPos (Vector3 pos)
	{
		transform.position = pos;
	}

    /// <summary>
    /// method for properly sizing box colliders during customization events
    /// </summary>

    public void setBoxColliders ()
	{
		for (int i = 0; i < stimArray.Length; i++)
		{
			Vector2 boxOrig = stimArray[i].GetComponent<BoxCollider2D>().size;
			Debug.Log("original size: (" + stimArray[i].GetComponent<BoxCollider2D>().size.x + "," +stimArray[i].GetComponent<BoxCollider2D>().size.y + ")");
			stimArray[i].GetComponent<BoxCollider2D>().size = new Vector2((boxOrig.x * 3.3333f), (boxOrig.y * 3.3333f));
			Debug.Log("new size: (" + stimArray[i].GetComponent<BoxCollider2D>().size.x + "," +stimArray[i].GetComponent<BoxCollider2D>().size.y + ")");
	/*		boxOrig.x = ((boxOrig.x * 100f)/30f);
			boxOrig.y = ((boxOrig.y * 100f)/30f);
			stimArray[i].GetComponent<BoxCollider2D>().size.Set(boxOrig.x,boxOrig.y); */
		}
	}	

//******************
// Other functions *
//******************

//inherited Observer method
	public override void onNotify (EventInstance<GameObject> e)
	{
		if(e.type== eType.Grab || e.type == eType.Drag)
		{
		}
	}
	
/// <summary>
/// Move the SOO to the next destination (center of screen or Garbage Collector)
/// </summary>
/// <param name="dest">The next Destination.</param>

	public void move(int dest)
	{
		moving = true;
		movingAlreadyFalse = false;
		setDrag("Suspended");
		curDest = destArray[dest];
		setWalk("set");
	}

    /// <summary>
    /// sets triggers instructing the main character on where and when to walk
    /// </summary>
    /// <param name="param">a string</param>

	void setWalk(string param)
	{
		if(stimArray == null)
			return;
		for (int i = 0; i < stimArray.Length; i++)
		{
			if(stimArray[i] != null)
			{
				Animator m = stimArray[i].GetComponent<Animator>();
				if (m != null)
				{
					if(param == "set")
						m.SetTrigger("Walk_Left");
					else if (param == "right")
						m.SetTrigger("Walk_Right");
					else if (param == "reset")
						m.SetTrigger("Landed");
				}
			}
		}
	}

    /// <summary>
    /// Tell the stimuli to update homePos to their current position, for snap-back functionality
    /// </summary>
    
	void updatePos()
	{
		for (int i = 0; i < stimArray.Length; i++)
		{
			if (stimArray[i] != null)
			{
				stimArray[i].GetComponent<StimulusScript>().setHomePos();
			}
		}
	}

	
/// <summary>
/// Sets the soo.
/// </summary>
/// <param name="array">Array of selectable objects (stimuli or customization options).</param>
/// <param name="qNum">Question number.</param>
	public void setSoo (GameObject[] array, int qNum) 
	{
		stimArray = array;
		questionNumber = qNum;
	}
	

	void setDrag (string draggable)
	{
		for (int i = 0; i < stimArray.Length; i++)
		{
			if(stimArray[i] == null)
			{
				continue;
			}
			else if (stimArray[i].GetComponent<StimulusScript>().token != null)
			{
				TokenScript token = stimArray[i].GetComponent<StimulusScript>().token; 
				token.gameObject.tag = draggable;
			}
			else
			{
				stimArray[i].GetComponent<StimulusScript>().tag = draggable;
			}
		}
	}
	
	void Update()
	{
		if(moving == false && !movingAlreadyFalse)
		{		
			setDrag("Stimulus");
			if(curDest == destArray[0])
			{
				eventWrapper.sendEvent(eType.Ready);
			}
			movingAlreadyFalse = true; // only do this once per move
		}
		if(moving == true)
		{
			transform.position = Vector3.Lerp(transform.position, curDest, Time.deltaTime * speed);
		}
		if(moving == true && ((transform.position.x < curDest.x + marginOfError) && (transform.position.x > curDest.x - marginOfError)))
		{
			transform.position = curDest;
			updatePos();
			setWalk ("reset");
			moving = false;
		}
		
	}
		
			

}

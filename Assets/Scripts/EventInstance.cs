﻿using UnityEngine;
using System.Collections;

/* EventInstance.cs
 * Contains event signaling data types (enum eType, EventInstance <T>)
 */
public enum eType {Tap, Trashed, Selected, Drag, Init, NewQuestion, EndGame, TimedOut, FingerDown, FingerUp, Grab, Ready, NewGame, Payoff, Transition}; //basic enum that contains all signalable events


/// <summary>
/// EventInstance <T>
/// Generic class that wraps an eType signal and the signaler object for
/// notify to pass to an observer list.
/// The Subject passing the EventInstance and the Observer receiving it must
/// have the same type of EventInstance<T>  (i.e: EventInstance<GameObject>)
/// </summary>
public class EventInstance <T> 
{
	public eType type;
	public T signaler;
}

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class RudderSlider : MonoBehaviour {

	EventTrigger trigger;
	UnityAction down, up;
	void Start () {
		down = new UnityAction (NavBoatControl.s_instance.RudderSliderValueWasChangedTrue);
		up = new UnityAction (NavBoatControl.s_instance.RudderSliderValueWasChangedFalse);

		trigger = GetComponent<EventTrigger> ();

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener((eventData) => down() );
		trigger.triggers.Add(entry);

		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerUp;
		entry2.callback.AddListener((eventData) => up() );
		trigger.triggers.Add(entry2);
	}
}

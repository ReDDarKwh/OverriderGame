using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {
	
	struct Event {
		public string evt;
		public EventData data;

		public Event(string evt, EventData data) {
			this.evt = evt;
			this.data = data;
		}
	}
}

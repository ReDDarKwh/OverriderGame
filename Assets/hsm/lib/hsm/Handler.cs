using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsm {

	public class Handler {

		public State targetState;
		public TransitionKind kind;
		public Action<EventData> action;
		public Func<EventData, bool> guard;
		
		public Handler(State targetState, TransitionKind kind, Action<EventData> action, Func<EventData, bool> guard) {
			this.targetState = targetState;
			this.kind = kind;
			this.action = action;
			this.guard = guard;
		}
	}
}
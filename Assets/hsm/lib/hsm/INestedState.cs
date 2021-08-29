using System.Collections.Generic;

namespace Hsm {

	interface INestedState {
		bool Handle(string evt, EventData data);
		List<string> getActiveStateConfiguration();
	}

}

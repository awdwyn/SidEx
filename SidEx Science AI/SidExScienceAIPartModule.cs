using System;
using UnityEngine;
using KSP.UI.Screens;

namespace SidEx.ScienceAI {
	[KSPModule("SidEx Science AI")]
	public class SidExScienceAIPartModule : PartModule {
		public const string DEBUGNAME = @"[SidEx Science AI]";

		[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Science AI"), UI_Toggle(scene = UI_Scene.All, enabledText = "Enabled", disabledText = "Disabled")]
		public bool enableScienceAIOnThisVessel = false;

		[KSPAction(guiName = "Enable Science AI")]
		public void enableScienceAIAction(KSPActionParam param) {
			this.enableScienceAIOnThisVessel = true;
		}

		[KSPAction(guiName = "Disable Science AI")]
		public void disableScienceAIAction(KSPActionParam param) {
			this.enableScienceAIOnThisVessel = false;
		}

		[KSPAction(guiName = "Toggle Science AI")]
		public void toggleScienceAIAction(KSPActionParam param) {
			this.enableScienceAIOnThisVessel = !this.enableScienceAIOnThisVessel;
		}

		public override string GetInfo() {
			return "Disabled by default. Allows automated ccience collection on this vessel when enabled.";
		}
	}
}

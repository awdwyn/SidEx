using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SidEx.ProjectManager {

	[KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[] { GameScenes.FLIGHT })]
	public class SidExProjectManagerScenario : ScenarioModule {
		public const string DEBUGNAME = @"[SidEx Project Manager]";

		// singleton
		public static SidExProjectManagerScenario Instance { get; private set; }

		private static ConfigNode _node; // where we're saving our project data in persistent.sfs
		public ConfigNode node { get {
				return _node;
			} }

		public override void OnAwake() {
			base.OnAwake();

			if (Instance == null)
				Instance = this;
		}

		public override void OnLoad(ConfigNode node) {
			Debug.LogFormat(@"{0} Loading data...", DEBUGNAME);
			base.OnLoad(node);

			if (Instance != this)
				return;

			if (node.HasNode("Projects"))
				_node = node.GetNode("Projects");
			else
				_node = new ConfigNode("Projects");
		}

		public override void OnSave(ConfigNode node) {
			Debug.LogFormat(@"{0} Saving data...", DEBUGNAME);
			base.OnSave(node);

			if (Instance != this)
				return;

			if (node.HasNode("Projects"))
				node.RemoveNode("Projects");

			node.AddNode(_node);
		}
	}
}

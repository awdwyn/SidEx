using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SidEx.ProjectManager {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class SidExProjectManager : MonoBehaviour {

		public void Start() {
			Log.Write($"Starting up...");

			GameEvents.onVesselChange.Add(this.VesselChangeHandler);
			GameEvents.onVesselCreate.Add(this.VesselCreateHandler);
			GameEvents.onGameStatePostLoad.Add(this.GameStatePostLoadHandler);

			// we just loaded the flight scene, so we are in a vessel now .. run it
			HandleVesselNaming(FlightGlobals.ActiveVessel);
		}

		public void OnDisable() {
			Log.Write($"Disabling...");
			GameEvents.onVesselChange.Remove(this.VesselChangeHandler);
			GameEvents.onVesselCreate.Remove(this.VesselCreateHandler);
			GameEvents.onGameStatePostLoad.Remove(this.GameStatePostLoadHandler);
		}

		public void VesselChangeHandler(Vessel vessel) {
//			Log.Write($"VesselChangeHandler()");
			HandleVesselNaming(vessel);
		}

		public void VesselCreateHandler(Vessel vessel) {
//			Log.Write($"VesselCreateHandler()");
			HandleVesselNaming(vessel);
		}

		public void GameStatePostLoadHandler(ConfigNode node) {
			if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
				HandleVesselNaming(FlightGlobals.ActiveVessel);
		}

		public void HandleVesselNaming(Vessel vessel) {
			// if this got run before the scenario was created, don't do anything
			if (SidExProjectManagerScenario.Instance == null)
				return;

			// if by some chance we don't have a vessel, just bug out
			if (vessel == null)
				return;

			Log.Write($"Handling naming on {vessel.vesselName}");

			// set up shared numbering
			Dictionary<string, int> sharedNumberingProjects = new Dictionary<string, int>();

			// this would be a ship created without vesselnaming configured
			//if (vessel.FindPartModulesImplementing<VesselNaming>().Count == 0) {
			if (vessel.parts.FindAll(x => x.vesselNaming != null).Count == 0) { 
				// handle unlikely events
				if (string.IsNullOrEmpty(vessel.vesselName))
					return;

				string tag, countRoman;
				if (!ConvertName(vessel.vesselName, ref sharedNumberingProjects, out tag, out countRoman))
					return;

				// apply the new name
				vessel.vesselName = string.Format("{0} {1}", tag, countRoman);
				Log.Write($"Renaming vessel to {vessel.vesselName}");

				// all done
				return;
			}

//			for (int i = 0; i < vessel.parts.Count(); i++)
//				Log.Write($"\tParts list: [{1}] {2} '{3}'", i, vessel.parts[i].partName, vessel.parts[i].vesselNaming == null ? "null" : vessel.parts[i].vesselNaming.vesselName);

			// we won't rename the vessel at the end if we don't modify it
			bool hasBeenModified = false;

			foreach (Part part in vessel.parts.FindAll(x => x.vesselNaming != null && x.vesselNaming.vesselName.Contains("["))) {
				string partName = part.vesselNaming.vesselName;

//				Log.Write($"Found part with vesselnaming {1} '{2}'", part.partName, part.vesselNaming.vesselName);

				// handle unlikely events
				if (string.IsNullOrEmpty(partName))
					continue;

				string tag, countRoman;
				if (!ConvertName(partName, ref sharedNumberingProjects, out tag, out countRoman))
					continue;

				// apply the new name to this part
				part.vesselNaming.vesselName = string.Format("{0} {1}", tag, countRoman);

				Log.Write($"Renaming tagged part {part.partName} to {part.vesselNaming.vesselName}");

				hasBeenModified = true;
			}

			// finally rename the whole vessel
			if (hasBeenModified) {
				vessel.vesselName = VesselNaming.FindPriorityNamePart(vessel).vesselNaming.vesselName;
				Log.Write($"Renaming vessel to {vessel.vesselName}");
			}
		}

		private bool ConvertName(string partName, ref Dictionary<string, int> sharedNumberingProjects, out string tag, out string countRoman) {
			tag = countRoman = string.Empty;

			// set up shared numbering
			bool sharedNumbering = partName.Contains("[[");

			// get the tag
			string tagRegex = @"\[([^\[]*?)\]"; // anything inside single or double square brackets
			Match tagMatch = Regex.Match(partName, tagRegex);
			if (string.IsNullOrEmpty(tagMatch.Value))
				return false;

			// clean up the tag
			tag = tagMatch.Value;
			tag = tag.Replace("[", "").Replace("]", "");

			// get the project from the tag
			string project = Regex.Replace(tag, @"\s+", "");
			project = Regex.Replace(project, @"[^0-9a-zA-Z]+", "").ToUpper();


			// look for an existing project and create if needed
			ConfigNode projectNode = SidExProjectManagerScenario.Instance.node.GetNodes().Any(x => x.name == project) ? SidExProjectManagerScenario.Instance.node.GetNode(project) : SidExProjectManagerScenario.Instance.node.AddNode(project);

			// get the count
			int count;
			if (!int.TryParse(projectNode.GetValue("count"), out count))
				count = 0;

			// check for shared numbering
			if (sharedNumbering) {
				if (!sharedNumberingProjects.ContainsKey(project)) { // the project is shared but not used yet, increment and save
					count++;
					sharedNumberingProjects.Add(project, count);
				} else
					count = sharedNumberingProjects[project];
			} else
				count++;

			// convert to roman numerals
			countRoman = ArabicToRoman(count);

			// save the count
			projectNode.SetValue("count", count.ToString(), true);

			return true;
		}

		private string ArabicToRoman(int number) {
			string[][] romanNumerals = new string[][] {
				new string[]{"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
				new string[]{"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
				new string[]{"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
				new string[]{"", "M", "MM", "MMM"} // thousands
			};

			// split integer string into array and reverse array
			var intArr = number.ToString().Reverse().ToArray();
			var len = intArr.Length;
			var romanNumeral = "";
			var i = len;

			// starting with the highest place (for 3046, it would be the thousands
			// place, or 3), get the roman numeral representation for that place
			// and add it to the final roman numeral string
			while (i-- > 0) {
				romanNumeral += romanNumerals[i][Int32.Parse(intArr[i].ToString())];
			}

			return romanNumeral;
		}
	}
}

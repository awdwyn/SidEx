using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SidEx.ScienceAI {

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class SidExScienceAIFlightAddon : MonoBehaviour {
		public const string DEBUGNAME = @"[SidEx Science AI]";
		public int timerCounter = 0;

		public enum Modes {
			None,
			RerunnableOnly,
			RerunnableAndResettableWithScientist,
			All
		}

		private Modes _lastMode = Modes.None;
		public Modes mode = Modes.None;

		private Vessel _lastVessel;
		private CelestialBody _lastCelestial;
		private ExperimentSituations _lastSituation;
		private string _lastBiome;

		private Vessel currentVessel;
		private CelestialBody currentCelestial;
		private ExperimentSituations currentSituation;
		private string currentBiome;

		/// <summary>
		/// Determines whether there is stuff left to do in the current vessel/body/biome/situation combination.
		/// </summary>
		private bool _dirty { get; set; }

		private enum DebugFormat {
			Normal,
			Warning,
			Error,
			Screen
		}

		private void Log(string s, DebugFormat debugFormat = DebugFormat.Normal, bool always = false) {
#if DEBUG
			_Log(s, debugFormat);
#else
			if (always)
				_Log(s, debugFormat);
#endif
		}

	private void _Log(string s, DebugFormat debugFormat = DebugFormat.Normal) {
			switch (debugFormat) {
				case DebugFormat.Normal:
					Debug.LogFormat($"{DEBUGNAME} {s}");
					break;
				case DebugFormat.Warning:
					Debug.LogWarningFormat($"{DEBUGNAME} {s}");
					break;
				case DebugFormat.Error:
					Debug.LogErrorFormat($"{DEBUGNAME} {s}");
					break;
				case DebugFormat.Screen:
					if (KerbalismAPI.KerbalismInstalled)
						KerbalismAPI.Message(s);
					else
						ScreenMessages.PostScreenMessage(s, 3.0f, ScreenMessageStyle.LOWER_CENTER, false);
					break;
			}
		}

		public void Start() {
			// only run in career or science modes
			if (!(HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)) {
				Log($"This mod is only available in Career or Science modes. Disabling...", DebugFormat.Normal, true);
				Destroy(this);
				return;
			}

			Log($"Kerbalism is{(KerbalismAPI.KerbalismInstalled ? "" : " not")} installed...");
//			if (KerbalismAPI.KerbalismInstalled)
//				Log($"<color=#BBBB00><b>Science AI</b></color>\n<b>Kerbalism</b> is installed\n<i>Enabling extra features</i>", DebugFormat.Screen, true);
		}


		/// <summary>
		/// Updates the mode based on R&D unlocks
		/// </summary>
		/// <returns>true if there was a change</returns>
		private Modes CheckMode() {
//			Log($"Checking tech tree for mode unlocks...");
			if (ResearchAndDevelopment.Instance == null) {
				Log($"ResearchAndDevelopment.Instance is null!", DebugFormat.Error);
				return Modes.None;
			} else {
				return ResearchAndDevelopment.GetTechnologyState("advUnmanned") == RDTech.State.Available ? Modes.All :
							ResearchAndDevelopment.GetTechnologyState("basicScience") == RDTech.State.Available ? Modes.RerunnableAndResettableWithScientist :
							ResearchAndDevelopment.GetTechnologyState("engineering101") == RDTech.State.Available ? Modes.RerunnableOnly : Modes.None;
			}

		}

		public void FixedUpdate() {
			// only run every 150 updates - prevent spamming experiments before they can even finish running
			timerCounter++;
			if (timerCounter < (60*2.5))
				return;
			timerCounter = 0;

			// TODO: cache vessel and situation and biome, when any of these change reset the dirty marker. When dirty, run every 150 (60*2.5) updates until no more is left to do (all science done, experiment data transferred / reviewed, experiments reset if needed) then mark not dirty. If not dirty, do nothing until dirty again.
			currentVessel = FlightGlobals.ActiveVessel;
			if (currentVessel == null)
				return;

			currentCelestial = currentVessel.mainBody;
			currentSituation = GetExperimentSituations(currentVessel);
			currentBiome = GetCurrentBiome(currentVessel);

			mode = CheckMode();
			if (mode != _lastMode) 
				Log($"Updated mode to {mode.ToString()}");

			if (mode == Modes.None)
				return;

			// get out of here if there's nothing to do (CheckMode is also necessary to be first to ensure it runs as we need to update the mode regardless of anything else)
			if (!_dirty) {
				if (mode != _lastMode) {
					Log($"Mode changed, marking dirty");
					_dirty = true;
				} else if (_lastVessel == null || _lastVessel != currentVessel) {
					Log($"Vessel changed, marking dirty");
					_dirty = true;
				} else if (_lastCelestial != currentCelestial) {
					Log($"Celestial changed, marking dirty");
					_dirty = true;
				} else if (_lastSituation != currentSituation) {
					Log($"Situation changed, marking dirty");
					_dirty = true;
				} else if (!_lastBiome.Equals(currentBiome)) {
					Log($"Biome changed, marking dirty");
					_dirty = true;
				}
			}
			if (!_dirty) {
				Log($"Nothing to do, waiting for next update...");
				return;
			}

			// don't run if we don't have control of the vessel
			if (!currentVessel.IsControllable)
				return;

				//if (currentVessel == _lastVessel && currentSituation == _lastSituation && currentBiome == _lastBiome)
				//	return;

				//_lastVessel = currentVessel;
				//_lastSituation = currentSituation;
				//_lastBiome = currentBiome;

				// use the priority naming part if possible (will be null if vessel isn't saved) otherwise first found part with our module
				Part controlPart = VesselNaming.FindPriorityNamePart(currentVessel) ?? currentVessel.parts.Find(x => x.Modules.Contains("SideliasScienceAIPartModule"));

			if (controlPart == null) // whelp, guess we're not needed here
				return;

			if (!((SidExScienceAIPartModule)controlPart.Modules["SideliasScienceAIPartModule"]).enableScienceAIOnThisVessel) // we're presently disabled.
				return;

			//// get a list of all science experiments
			//List<Part> moduleScienceExperimentParts = currentVessel.Parts.FindAll(x => x.Modules.Contains("ModuleScienceExperiment"));

			//foreach (Part moduleScienceExperimentPart in moduleScienceExperimentParts) {
			//	foreach (ModuleScienceExperiment moduleScienceExperiment in moduleScienceExperimentPart.Modules.GetModules<ModuleScienceExperiment>())
			//		RunExperiment(moduleScienceExperiment);
			//}

			//Log($"Attempting to find a science data transfer target...");
			//ModuleScienceContainer commandPartScienceContainer = null;
			//foreach (PartModule pm in controlPart.Modules) {
			//	if ((pm as ModuleScienceContainer) != null) {
			//		Log($"    Found  option in part: {controlPart.partName}/{pm.moduleName}...");
			//		commandPartScienceContainer = pm as ModuleScienceContainer;
			//	}
			//}

			Log($"Checking on-board science experiments");
			// attempt #2 to include dmagic
			_dirty = false;
			foreach (ModuleScienceExperiment moduleScienceExperiment in currentVessel.FindPartModulesImplementing<ModuleScienceExperiment>())
				if (RunExperiment(moduleScienceExperiment))
					_dirty = true;

			Log($"Marking us {(_dirty ? "dirty" : "not dirty")}");

			_lastVessel = currentVessel;
			_lastCelestial = currentCelestial;
			_lastSituation = currentSituation;
			_lastBiome = currentBiome;

			_lastMode = mode;
		}

		/// <summary>
		/// Performs actions on experiments. Run/Transfer/Review/Reset
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		/// <returns>false if nothing is left to do, true otherwise</returns>
		private bool RunExperiment(ModuleScienceExperiment moduleScienceExperiment) {
			Log($"  {moduleScienceExperiment.part.partInfo.title}/{moduleScienceExperiment.experiment.experimentTitle}");
			Log($"    rerunnable:{moduleScienceExperiment.rerunnable} resettable:{moduleScienceExperiment.resettable} deployed:{moduleScienceExperiment.Deployed} inpoerable:{moduleScienceExperiment.Inoperable}");

			//			// If the experiment is already deployed but not yet handled, don't try to re-run it
			//			if (moduleScienceExperiment.Deployed && !moduleScienceExperiment.Inoperable)
			//				return false;

			// Filter by mode
			if (!moduleScienceExperiment.rerunnable) {
				switch (mode) { 
					case Modes.RerunnableOnly:
						Log($"    We haven't unlocked the mode to run this experiment yet");
						return false;
					case Modes.RerunnableAndResettableWithScientist:
						if (currentVessel.GetVesselCrew().Find(x => x.trait == "Scientist") == null) {
							Log($"    We either need to unlock the next mode or bring a scientist along with us for this");
							return false;
						}
						break;
				}
			}

			//ScienceExperiment scienceExperiment = moduleScienceExperiment.experiment;
			ScienceExperiment scienceExperiment = ResearchAndDevelopment.GetExperiment(moduleScienceExperiment.experimentID); // lookup from R&D instead of pulling from ModuleScienceExperiment.experiment becuase DMagic is misbehaving.

//			Log($"Using science experiment:{1}[{5}] with biomeMask: {2} situationMask: {3} baseValue: {4}", scienceExperiment.id, scienceExperiment.biomeMask, scienceExperiment.situationMask, scienceExperiment.baseValue, scienceExperiment.experimentTitle);

			// Review data?
			if (moduleScienceExperiment.Deployed && !moduleScienceExperiment.Inoperable) {
				if (KerbalismAPI.KerbalismInstalled) {
					Log($"    Kerbalism installed, attempting to send science data straight to hard drive");

					// file or sample? (as per kerbalisms definition)
					bool sample = moduleScienceExperiment.xmitDataScalar < 0.666f;
					Log($"      {moduleScienceExperiment.experimentID} is a {(sample ? "sample" : "file")}");

					ScienceData[] data = moduleScienceExperiment.GetDataUsingReflection();
					for (int i = 0; i < data.Count(); i++) {
						
						Log($"      Storing {data[i].subjectID} in hard drive");
						if (sample)
							KerbalismAPI.StoreSample(currentVessel, data[i].subjectID, data[i].dataAmount);
						else
							KerbalismAPI.StoreFile(currentVessel, data[i].subjectID, data[i].dataAmount);

						// get rid of the data in the experiment now
						Log($"      Dumping {data[i].subjectID} from experiment");
						moduleScienceExperiment.DumpDataUsingReflection(data[i]);
					}
				} else {
					Log($"      Attempting to review data to force collection...");
					moduleScienceExperiment.ReviewDataUsingReflection();					
				}
				return true;
			}

			// Reset inoperable experiments
			if (moduleScienceExperiment.resettable && moduleScienceExperiment.Inoperable) {
				Log($"    Resetting experiment");
//				Log($"Science AI is cleaning {scienceExperiment.experimentTitle}", DebugFormat.Screen, true);
				moduleScienceExperiment.ResetExperimentUsingReflection();
				return true;
			}

			// check if we can even run this experiment now
			if (!scienceExperiment.IsAvailableWhile(currentSituation, currentVessel.mainBody)) {
				Log($"    Cannot run in {currentSituation} on {currentVessel.mainBody}.");
				return false;
			}

			string experimentSpecificBiome = string.Empty;
			if (scienceExperiment.BiomeIsRelevantWhile(currentSituation))
				experimentSpecificBiome = currentBiome;

			// check science value of experiment based on what is returned to KSC R&D and what is presently stored on the vessel (including science containers and kerbalism data drives)
			ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(scienceExperiment, currentSituation, currentVessel.mainBody, experimentSpecificBiome, null);
			List<ScienceData> storedData;
			int numberOfExperimentsOnBoard = GetNumberOfExperimentsOnBoard(subject, out storedData);

			// Check for duplicate experiments
			if (numberOfExperimentsOnBoard > 0) {
				Log($"    We already have this experiment on board");
				return false;
			}

			float scienceValue = GetScienceValue(scienceExperiment, subject, storedData);

			if (scienceValue < 0.01) {
				Log($"    Science value is too low ({scienceValue} science). Not running.");
				return false;
			}

			// run the experiment
			Log($"    Running experiment {moduleScienceExperiment.part.partInfo.title}/{moduleScienceExperiment.experiment.experimentTitle} for {scienceValue} science in biome {currentSituation.ToString()} {currentBiome}.");
			moduleScienceExperiment.DeployExperimentUsingReflection();

//			Log($"<color=#BBBB00><b>Science AI</b></color>\nRunning <b>{scienceExperiment.experimentTitle}</b>\n<i>Hold onto your knickers!</i>", DebugFormat.Screen, true);

			return true;
		}

		private string GetCurrentBiome(Vessel v) {
//			Debug.LogWarningFormat(@"{0} Biome Details = landedAt: {1} landedAtString:{2} biome:{3}", v.landedAt, Vessel.GetLandedAtString(v.landedAt), v.mainBody.BiomeMap.GetAtt(v.latitude * UtilMath.Deg2Rad, v.longitude * UtilMath.Deg2Rad).name);

			if (v.landedAt != string.Empty)
				return v.landedAt;

			if (v.mainBody.BiomeMap == null)
				return "N/A";

			return v.mainBody.BiomeMap.GetAtt(v.latitude * UtilMath.Deg2Rad, v.longitude * UtilMath.Deg2Rad).name;
		}

		private ExperimentSituations GetExperimentSituations(Vessel v) {
			switch (v.situation) {
				case Vessel.Situations.LANDED:
				case Vessel.Situations.PRELAUNCH:
					return ExperimentSituations.SrfLanded;
				case Vessel.Situations.SPLASHED:
					return ExperimentSituations.SrfSplashed;
				case Vessel.Situations.FLYING:
					if (v.altitude < v.mainBody.scienceValues.flyingAltitudeThreshold)
						return ExperimentSituations.FlyingLow;
					else
						return ExperimentSituations.FlyingHigh;
				default:
					if (v.altitude < v.mainBody.scienceValues.spaceAltitudeThreshold)
						return ExperimentSituations.InSpaceLow;
					else
						return ExperimentSituations.InSpaceHigh;
			}
		}

		private float GetScienceValue(ScienceExperiment experiment, ScienceSubject subject, List<ScienceData> scienceData) {
			;
			float currentScienceValue = GetCurrentScienceValue(experiment, subject, scienceData);

			if (scienceData.Count == 0)
				return ResearchAndDevelopment.GetScienceValue(experiment.baseValue * experiment.dataScale, subject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;

			float experimentValue = ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, subject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;

			if (scienceData.Count == 1)
				return experimentValue;

			return experimentValue / Mathf.Pow(4f, scienceData.Count - 1);
		}

		private float GetCurrentScienceValue(ScienceExperiment experiment, ScienceSubject subject, List<ScienceData> storedData) {
			// get total value of science in experiment containers and the kerbalism data drive
			int numberOfExperimentsOnBoard = storedData.Count;
			if (numberOfExperimentsOnBoard == 0)
				return subject.science;

			float potentialScience = subject.science +
						  ResearchAndDevelopment.GetScienceValue(storedData.First().dataAmount, subject) *
						  HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;

			if (numberOfExperimentsOnBoard > 1) {
				float secondReport =
						ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, subject) *
						HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
				potentialScience += secondReport;
				if (numberOfExperimentsOnBoard > 2)
					for (int i = 3; i < numberOfExperimentsOnBoard; ++i)
						potentialScience += secondReport / Mathf.Pow(4f, i - 2);
			}
			return potentialScience;
		}

		private int GetNumberOfExperimentsOnBoard(ScienceSubject subject, out List<ScienceData> storedData) {
			storedData = new List<ScienceData>();
			foreach (IScienceDataContainer container in currentVessel.FindPartModulesImplementing<IScienceDataContainer>()) {
				try {
					ScienceData[] data = container.GetData();
					for (int i = 0; i < data.Length; i++) {
//						Log($"\t[{1}]: {2}", i, data[i].subjectID);
						if (data[i].subjectID == subject.id)
							storedData.Add(data[i]);
					}
				} catch { }
			}

//			Log($"    Checking for duplicate experiments on board with subject {subject.id}.... found {storedData.Count}");
			return storedData.Count;
		}
	}
}

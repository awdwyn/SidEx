using System;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections.Generic;
using System.Linq;

namespace SidEx.SelfDestruct {

	[KSPModule("SidEx Self Destruct")]
    public class SidExSelfDestruct : PartModule {

		//[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Time Delay"), UI_FloatRange(minValue = 1.0f, maxValue = 60.0f, stepIncrement = 1.0f, scene = UI_Scene.All)]
		//public float timeDelay = 10.0f;

		//[KSPField(isPersistant = true)]
		//public bool canStage = true;

		//[KSPField(isPersistant = true)]
		//public int stagingMode = (int)StagingMode.SelfDestruct;

		//[KSPField]
		//public bool addPawIdent = false;

		//[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Countdown"), UI_Toggle(scene = UI_Scene.All, enabledText = "Visible", disabledText = "Hidden")]
		//public bool showCountdown = true;

		//private float countDownInitiated = 0.0f;
		//private bool staged = false;
		//private bool abortCountdown = false;
		//private bool countDownActive = false;

		/// <summary>
		/// Determines whether we will allow enabling/disabling the stage icon
		/// </summary>
		[KSPField(isPersistant = true)]
		public bool addStagingOption = false;

		/// <summary>
		/// Add a staging icon
		/// </summary>
//		[KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "Enable Staging")]
		public void EnableStaging() {
			Log.Write($"Enabling staging");
			part.deactivate();
			part.inverseStage = Math.Min(StageManager.LastStage, part.inverseStage);
			part.stackIcon.CreateIcon();
			StageManager.Instance.SortIcons(true);

//			Events["EnableStaging"].active = false;
//			Events["DisableStaging"].active = true;
		}

		/// <summary>
		/// Remove the staging icon
		/// </summary>
//		[KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "Disable Staging")]
		public void DisableStaging() {
			Log.Write($"Disabling staging");
			part.stackIcon.RemoveIcon();
			StageManager.Instance.SortIcons(true);

//			Events["EnableStaging"].active = true;
//			Events["DisableStaging"].active = false;
		}

		/// <summary>
		/// Determines whether the part will self destruct when it is staged
		/// </summary>
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Staging Self Destruct"), UI_Toggle(scene = UI_Scene.All, enabledText = "Enabled", disabledText = "Disabled")]
		public bool selfDestructOnStage = false;

		private void selfDestructOnStageChanged(object oldValueObject) {
			Log.Write($"selfDestructOnStageChanged()");
			//			Log.Write($"Self Destruct On Stage Changed to {selfDestructOnStage} from {(bool)oldValueObject}");
			Log.Write($"addStagingOption: {addStagingOption}, selfDestructOnStage: {selfDestructOnStage}");

			if (addStagingOption) {
				if (selfDestructOnStage)
					EnableStaging();
				else
					DisableStaging();
			}

			if (selfDestructOnStage)
				this.part.stackIcon.StageIcon.SetIconColor(Color.yellow);
			else
				if (this.part.stackIcon.StageIcon != null)
					this.part.stackIcon.StageIcon.SetIconColor(Color.white);
		}

		/// <summary>
		/// Start self destruct from an action group
		/// </summary>
		/// <param name="param"></param>
		[KSPAction(guiName = "Self Destruct")]
		public void selfDestructAction(KSPActionParam param) {
			Log.Write($"Triggering self destruct via action group");
			StartSelfDestruct();
		}

		/// <summary>
		/// Start self destruct from the part ui
		/// </summary>
		[KSPEvent(active = true, guiActive = true, guiActiveUnfocused = true, guiName = "Self Destruct", unfocusedRange = 8.0f)]
		public void selfDestructEvent() {
			Log.Write($"Triggering self destruct via part UI");
			StartSelfDestruct();
		}

		public override void OnAwake() {
			Log.Write($"OnAwake()");
			//base.OnAwake();

//			if (addStagingOption)
//				DisableStaging();

//			Events["EnableStaging"].active = addStagingOption;
//			if (selfDestructOnStage)
//				if (!selfDestructOnStage)
//					DisableStaging();
//				else
//					EnableStaging();
		}

//		bool _originalActivatesEventIfDisconnected;

		public override void OnStart(StartState state) {
			Log.Write($"OnStart({state.ToString()})");

			Log.Write($"addStagingOption: {addStagingOption}, selfDestructOnStage: {selfDestructOnStage}");

			Fields["selfDestructOnStage"].OnValueModified += selfDestructOnStageChanged;

//			Log.Write($"partModule.stagingEnabled is {stagingEnabled}, part.stagingOn = {part.stagingOn}, part.hasStagingIcon = {part.hasStagingIcon}");
//			Fields["selfDestructOnStage"].guiActiveEditor = stagingEnabled;
//			Fields["selfDestructOnStage"].guiActive = stagingEnabled;

//			_originalActivatesEventIfDisconnected = part.ActivatesEvenIfDisconnected;
			part.ActivatesEvenIfDisconnected = true;
			//			if (stagingEnabled) {
//			if (selfDestructOnStage) {
//					part.stackIcon.Highlight(true);
					//				part.stackIcon.SetIconColor(XKCDColors.FireEngineRed);
//				part.ActivatesEvenIfDisconnected = true;
//				if (addStagingOption)
//					EnableStaging();
//			} else {
//					part.stackIcon.Highlight(false);
//				part.ActivatesEvenIfDisconnected = _originalActivatesEventIfDisconnected;
//				if (addStagingOption)
//					DisableStaging();
	//		}
//			}
			//if (!canStage) {
			//	Invoke("DisableStaging", 0.5f);
			//} else {
			//	UpdateStagingEvents();
			//}

			//UpdateSelfDestructEvents();
			//GameEvents.onVesselChange.Add(OnVesselChange);
		}

		// OnStart isn't called in the Editor, so these need to be in Start // no longer true
				void Start() {
					Log.Write($"Start()");

			if (selfDestructOnStage) {
				//					part.stackIcon.Highlight(true);
				//				part.stackIcon.SetIconColor(XKCDColors.FireEngineRed);
//				part.ActivatesEvenIfDisconnected = true;
				if (addStagingOption)
					EnableStaging();

				this.part.stackIcon.StageIcon.SetIconColor(Color.yellow);
			} else {
				//					part.stackIcon.Highlight(false);
//				part.ActivatesEvenIfDisconnected = _originalActivatesEventIfDisconnected;
				if (addStagingOption)
					DisableStaging();
			}

			//			Fields["selfDestructOnStage"].guiActiveEditor = Fields["selfDestructOnStage"].guiActive = stagingEnabled;

			//if (addPawIdent) {
			//	Events["EnableStaging"].guiName = "Self Destruct: Enable Staging";
			//	Events["DisableStaging"].guiName = "Self Destruct: Disable Staging";
			//}
		}

		public override void OnInitialize() {
			Log.Write($"OnInitialize()");
		}

		public override void OnActive() {
			Log.Write($"OnActive()");
			if (stagingEnabled && selfDestructOnStage) {
				Log.Write($"Triggering self destruct via staging");
				StartSelfDestruct();
			}

//			Log.Write($"Activating!");
//			if (canStage && countDownInitiated == 0.0f) {
//				Log.Write($"OnActive");
//				countDownInitiated = Time.time;
//				StartCoroutine(DoSelfDestruct());
//				staged = true;
//				UpdateSelfDestructEvents();
//#if false
//                switch ((StagingMode)stagingMode)
//                {
//                    case StagingMode.SelfDestruct:
//                        ExplodeAllEvent();
//                        break;
//                    case StagingMode.DetonateParent:
//                        DetonateParentEvent();
//                        break;
//                }
//#endif
//			}
		}

		float countDownTime = 0.0f;
//		bool countingDown = false;

		private void StartSelfDestruct() {
			Log.Write($"Preparing self destruct coroutine");
//			countingDown = true;
			if (countDownTime == 0.0f) {
				countDownTime = Time.time;
				StartCoroutine(DoSelfDestruct());
			}
		}

		private IEnumerator<WaitForSeconds> DoSelfDestruct() {
			Log.Write($"Starting self destruct coroutine");
			ScreenMessage msg = ScreenMessages.PostScreenMessage("Starting self destruct sequence", 1.0f, ScreenMessageStyle.UPPER_CENTER);

			StageIconInfoBox ib = null;
			if (this.part.stackIcon.StageIcon != null) {
				this.part.stackIcon.StageIcon.SetIconColor(Color.red);
				ib = this.part.stackIcon.StageIcon.DisplayInfo();
				//			ib.SetCaption($"Self Destruct");
				ib.SetMsgBgColor(Color.white);
				ib.SetMsgTextColor(Color.black);
			}

			while ((Time.time - countDownTime) < 5.0f) {
				Log.Write($"Self destruct countdown: {(5.0f - (Time.time - countDownTime)).ToString("#0")}");
				msg.message = $"Self destruct in {(5.0f - (Time.time - countDownTime)).ToString("#0")}";
				ScreenMessages.PostScreenMessage(msg);
				if (ib != null) {
					ib.SetValue((Time.time - countDownTime), 0.0f, 4.0f);
					ib.SetMessage($"Self Destruct: {(5.0f - (Time.time - countDownTime)).ToString("#0")}s");
				}
				yield return new WaitForSeconds(1.0f);
			}

			while (vessel.parts.Count > 0) {
				Part part = vessel.parts.Find(x => x != vessel.rootPart && x != this.part && !x.children.Any() && x.partInfo.name != "pkLES.EscapeMk1v2" && x.partInfo.name != "pkLES.mk2" && x.partInfo.name != "pkLES.mk2.noBPC" && x.partInfo.name != "LaunchEscapeSystem");
				if (part != null) {
					Log.Write($"Exploding {part.partInfo.name} [{part.partInfo.title}]");
					part.explode();
					yield return null;
				} else {
					while (vessel.parts.Count > 0) {
						Log.Write($"Exploding {vessel.parts[0].partInfo.name} [{vessel.parts[0].partInfo.title}]");
						vessel.parts[0].explode();
						yield return null;
					}
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		//public void OnVesselChange(Vessel newVessel) {
			//if (newVessel == this.vessel && !canStage) {
			//	Invoke("DisableStaging", 0.5f);
			//}
		//}

		//public override string GetInfo() {
			//return "Default delay = " + timeDelay + " (tweakable)";
		//}

		//[KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "Enable Staging")]
		//public void EnableStaging() {
		//	part.deactivate();
		//	part.inverseStage = Math.Min(StageManager.LastStage, part.inverseStage);
		//	part.stackIcon.CreateIcon();
		//	StageManager.Instance.SortIcons(true);

		//	canStage = true;
		//	UpdateStagingEvents();
		//}

		//[KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "Disable Staging")]
		//public void DisableStaging() {
		//	part.stackIcon.RemoveIcon();
		//	StageManager.Instance.SortIcons(true);

		//	canStage = false;
		//	UpdateStagingEvents();
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveEditor = true, guiName = "Mode: Detonate Parent")]
		//public void DetonateParentOnStaging() {
		//	stagingMode = (int)StagingMode.SelfDestruct;
		//	UpdateStagingModeEvents();
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveEditor = true, guiName = "Mode: Self Destruct")]
		//public void SelfDestructOnStaging() {
		//	stagingMode = (int)StagingMode.DetonateParent;
		//	UpdateStagingModeEvents();
		//}

		//private void UpdateStagingEvents() {
		//	UpdateStagingModeEvents();
		//	if (HighLogic.LoadedSceneIsEditor) {
		//		if (canStage) {
		//			Events["EnableStaging"].active = false;
		//			Events["DisableStaging"].active = true;
		//		} else {
		//			Events["EnableStaging"].active = true;
		//			Events["DisableStaging"].active = false;
		//		}
		//	} else {
		//		Events["EnableStaging"].active = false;
		//		Events["DisableStaging"].active = false;
		//	}
		//}

		//private void UpdateStagingModeEvents() {
		//	Events["DetonateParentOnStaging"].active = false;
		//	Events["SelfDestructOnStaging"].active = false;

		//	if (canStage) {
		//		switch ((StagingMode)stagingMode) {
		//			case StagingMode.SelfDestruct:
		//				Events["SelfDestructOnStaging"].active = true;
		//				break;
		//			case StagingMode.DetonateParent:
		//				Events["DetonateParentOnStaging"].active = true;
		//				break;
		//		}
		//	}
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveUnfocused = true, guiName = "Self Destruct!", unfocusedRange = 8.0f)]
		//public void ExplodeAllEvent() {
		//	countDownInitiated = Time.time;
		//	Log.Write("ExplodeAllEvent");
		//	StartCoroutine(DoSelfDestruct());
		//	UpdateSelfDestructEvents();
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveUnfocused = true, guiName = "Explode!", unfocusedRange = 8.0f)]
		//public void ExplodeEvent() {
		//	part.explode();
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveUnfocused = true, guiName = "Detonate parent!", unfocusedRange = 8.0f)]
		//public void DetonateParentEvent() {
		//	part.parent.explode();
		//	part.explode();
		//}

		//[KSPEvent(active = false, guiActive = true, guiActiveUnfocused = true, guiName = "Abort Self Destruct", unfocusedRange = 8.0f)]
		//public void AbortSelfDestruct() {
		//	abortCountdown = true;
		//}

		//[KSPAction("Self Destruct!")]
		//public void ExplodeAllAction(KSPActionParam param) {
		//	countDownActive = true;
		//	if (countDownInitiated == 0.0f) {
		//		countDownInitiated = Time.time;
		//		StartCoroutine(DoSelfDestruct());
		//		UpdateSelfDestructEvents();
		//	}

		//}

		//[KSPAction("Detonate parent!")]
		//public void ExplodeParentAction(KSPActionParam param) {
		//	if (countDownInitiated == 0.0f) {
		//		DetonateParentEvent();
		//	}
		//}

		//[KSPAction("Explode!")]
		//public void ExplodeAction(KSPActionParam param) {
		//	if (countDownInitiated == 0.0f) {
		//		part.explode();
		//	}
		//}

		//private void UpdateSelfDestructEvents() {
		//	UpdateStagingModeEvents();
		//	if (countDownInitiated == 0.0f) {
		//		// countdown has not been started
		//		Events["ExplodeAllEvent"].active = true;
		//		Events["ExplodeEvent"].active = true;
		//		Events["DetonateParentEvent"].active = true;
		//		Events["AbortSelfDestruct"].active = false;
		//	} else {
		//		// countdown has been started
		//		Events["ExplodeAllEvent"].active = false;
		//		Events["ExplodeEvent"].active = false;
		//		Events["DetonateParentEvent"].active = false;
		//		Events["AbortSelfDestruct"].active = true;
		//	}
		//}

//		private IEnumerator<WaitForSeconds> DoSelfDestruct() {
//			ScreenMessage msg = null;
//			if (showCountdown) {
//				Log.Write($"DoSelfDestruct 1"); 
//				msg = ScreenMessages.PostScreenMessage("Self destruct sequence initiated.", 1.0f, ScreenMessageStyle.UPPER_CENTER);
//				Log.Write($"DoSelfDestruct"); 
//			}

//			while ((Time.time - countDownInitiated) < timeDelay && !abortCountdown) {
//				float remaining = timeDelay - (Time.time - countDownInitiated);
//				if (showCountdown) {
//					UpdateCountdownMessage(ref msg, remaining);
//					Log.Write($"DoSelfDestruct 2");
//					ScreenMessages.PostScreenMessage(msg.message, 1.0f, ScreenMessageStyle.UPPER_CENTER);
//				}
//				yield return new WaitForSeconds(1.0f);
//			}

//			if (!abortCountdown) {
//				if (staged) {
//					if ((StagingMode)stagingMode == StagingMode.DetonateParent) {
//						DetonateParentEvent();
//						yield break;
//					}
//				}

//				while (vessel.parts.Count > 0) {
//					// We do not want to blow up the root part nor the self destruct part until last.
//					// also, ignore the various LES parts, in case they are there and waiting to react to an explosion
//					Part part = vessel.parts.Find(p => p != vessel.rootPart && p != this.part && !p.children.Any() &&
//						p.partInfo.name != "pkLES.EscapeMk1v2" && p.partInfo.name != "pkLES.mk2" && p.partInfo.name != "pkLES.mk2.noBPC" &&
//						p.partInfo.name != "LaunchEscapeSystem"
//					);
//					if (part != null) {
//						Log.Write($"Blowing up {part.name}");
//						part.explode();
//						// Do a yield here in case something else (ie:  Bob's Panic Box) needs to react to the part exploding
//						yield return null;
//					} else {
//						// Explode the rest of the parts
//						Log.Write($"No more safe parts, doing the last ones...");
//						while (vessel.parts.Count > 0) { 
//							Log.Write($"Blowing up {vessel.parts[0].name}");
//							vessel.parts[0].explode();
//							// Do a yield here in case something else (ie:  Bob's Panic Box) needs to react to the part exploding
//							yield return null;
//						}
//					}
//					yield return new WaitForSeconds(0.1f);
//				}
//			} else {
//				// reset
//				if (countDownActive) {
//					msg.startTime = Time.time;
//					msg.duration = 5.0f;
//					msg.message = "Self destruct sequence stopped.";
//				}
//				countDownActive = false;
//				part.deactivate();
//				countDownInitiated = 0.0f;
//				abortCountdown = false;
//				UpdateSelfDestructEvents();
//			}
//		}

//		private void UpdateCountdownMessage(ref ScreenMessage msg, float remaining) {
//			Log.Write($"UpdatecountdownMessage, countdownActive: {countDownActive.ToString()}");
//			// If the countdown message is currently being displayed
//			//if (countDownActive)
//			{
//				// If it is still supposed to be displayed
//				if (showCountdown) {
//					// Update the countdown message
//					msg.message = "Self destruct sequence initiated: " + remaining.ToString("#0");
//					Log.Write($"Updating message 1");
//				} else {
//					// The UI must have been hidden or the user changed showCountdown?
//					msg.duration = 1.0f;
//					msg = null;
//				}
//			}
//#if false
//            else
//            {
//                // If it should be displayed now. Maybe the UI was unhidden or the user changed showCountdown?
//                if (showCountdown)
//                {
//                    // Show the countdown message
//                    msg = ScreenMessages.PostScreenMessage("Self destruct sequence initiated: " + remaining.ToString("#0"),
//                        remaining, ScreenMessageStyle.UPPER_CENTER);
//                    Debug.Log("Updating message 2");
//                }
//            }
//#endif
//		}

//		private enum StagingMode {
//			SelfDestruct = 0,
//			DetonateParent = 1
//		}
	}
}

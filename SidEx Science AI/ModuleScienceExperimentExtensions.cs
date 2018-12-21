using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SidEx {
	public static class ModuleScienceExperimentExtensions {

		/// <summary>
		/// Attempts to call GetData() on subclasses. Falls back to default on failure.
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		/// <returns></returns>
		public static ScienceData[] GetDataUsingReflection(this ModuleScienceExperiment moduleScienceExperiment) {
			try {
				return (ScienceData[])moduleScienceExperiment.GetType().InvokeMember("GetData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, moduleScienceExperiment, null);
			} catch {
				return moduleScienceExperiment.GetData();
			}
		}

		/// <summary>
		/// Attempts to call DumpData() on subclasses. Falls back to default on failure.
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		/// <param name="scienceData"></param>
		public static void DumpDataUsingReflection(this ModuleScienceExperiment moduleScienceExperiment, ScienceData scienceData) {
			try {
				moduleScienceExperiment.GetType().InvokeMember("DumpData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.InvokeMethod, null, moduleScienceExperiment, new object[] { scienceData });
			} catch {
				moduleScienceExperiment.DumpData(scienceData);
			}
		}

		/// <summary>
		/// Attempts to call ReviewData() on subclasses. Falls back to default on failure.
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		public static void ReviewDataUsingReflection(this ModuleScienceExperiment moduleScienceExperiment) {
			try {
				moduleScienceExperiment.GetType().InvokeMember("ReviewData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.InvokeMethod, null, moduleScienceExperiment, null);
			} catch {
				moduleScienceExperiment.ReviewData();
			}
		}

		/// <summary>
		/// Attempts to call ResetExperiment() on subclasses. Falls back to default on failure.
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		public static void ResetExperimentUsingReflection(this ModuleScienceExperiment moduleScienceExperiment) {
			try {
				moduleScienceExperiment.GetType().InvokeMember("ResetExperiment", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.InvokeMethod, null, moduleScienceExperiment, null);
			} catch {
				moduleScienceExperiment.ResetExperiment();
			}
		}

		/// <summary>
		/// Attempts to call DeployExperiment() on subclasses. Falls back to default on failure.
		/// </summary>
		/// <param name="moduleScienceExperiment"></param>
		public static void DeployExperimentUsingReflection(this ModuleScienceExperiment moduleScienceExperiment) {
			try {
				moduleScienceExperiment.GetType().InvokeMember("DeployExperiment", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.InvokeMethod, null, moduleScienceExperiment, null);
			} catch {
				moduleScienceExperiment.DeployExperiment();
			}
		}
	}
}

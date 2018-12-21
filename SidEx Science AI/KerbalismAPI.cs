using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace SidEx {
	public static class KerbalismAPI {

		static Type _kerbalismAPI;

		static KerbalismAPI() {
			foreach (var ass in AssemblyLoader.loadedAssemblies) {
				if (ass.name == $"Kerbalism{Versioning.version_major.ToString()}{Versioning.version_minor.ToString()}") {
					_kerbalismAPI = ass.assembly.GetType("KERBALISM.API");

					_message = _kerbalismAPI.GetMethod("Message");
					_kill = _kerbalismAPI.GetMethod("Kill");

					_radiation = _kerbalismAPI.GetMethod("Radiation");

					_storeFile = _kerbalismAPI.GetMethod("StoreFile");
					_storeSample = _kerbalismAPI.GetMethod("StoreSample");

					break;
				}
			}
		}

		/// <summary>
		/// returns true if kerbalism is detected
		/// </summary>
		public static bool KerbalismInstalled {
			get {
				return _kerbalismAPI != null;
			}
		}

		/// <summary>
		/// show a message in the ui
		/// </summary>
		/// <param name="msg"></param>
		public static void Message(string msg) {
			_message.Invoke(null, new object[] { msg });
		}
		static MethodInfo _message;

		/// <summary>
		/// kill a kerbal, even an EVA one
		/// </summary>
		/// <param name="v"></param>
		/// <param name="c"></param>
		public static void Kill(Vessel v, ProtoCrewMember c) {
			_kill.Invoke(null, new object[] { v, c });
		}
		static MethodInfo _kill;

		// trigger an undesiderable event for the kerbal specified
		public static void Breakdown(Vessel v, ProtoCrewMember c) { throw new NotImplementedException(); }
		// disable or re-enable all rules for the specified kerbal
		public static void DisableKerbal(string k_name, bool disabled) { throw new NotImplementedException(); }
		// inject instant radiation dose to the specified kerbal (can use negative amounts)
		public static void InjectRadiation(string k_name, double amount) { throw new NotImplementedException(); }

		// --- ENVIRONMENT ----------------------------------------------------------

		// return true if the vessel specified is in sunlight
		public static bool InSunlight(Vessel v) { throw new NotImplementedException(); }
		// return true if the vessel specified is inside a breathable atmosphere
		public static bool Breathable(Vessel v) { throw new NotImplementedException(); }

		// --- RADIATION ------------------------------------------------------------

		/// <summary>
		/// return amount of environment radiation at the position of the specified vessel
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static double Radiation(Vessel v) {
			return (double)_radiation.Invoke(null, new object[] { v });
		}
		static MethodInfo _radiation;

		// return true if the vessel is inside the magnetopause of some body (except the sun)
		public static bool Magnetosphere(Vessel v) { throw new NotImplementedException(); }
		// return true if the vessel is inside the radiation belt of some body
		public static bool InnerBelt(Vessel v) { throw new NotImplementedException(); }
		// return true if the vessel is inside the radiation belt of some body
		public static bool OuterBelt(Vessel v) { throw new NotImplementedException(); }

		// --- SPACE WEATHER --------------------------------------------------------

		// return true if a solar storm is incoming at the vessel position
		public static bool StormIncoming(Vessel v) { throw new NotImplementedException(); }
		// return true if a solar storm is in progress at the vessel position
		public static bool StormInProgress(Vessel v) { throw new NotImplementedException(); }
		// return true if the vessel is subject to a signal blackout
		public static bool Blackout(Vessel v) { throw new NotImplementedException(); }

		// --- RELIABILITY ----------------------------------------------------------

		// return true if at least a component has malfunctioned, or had a critical failure
		public static bool Malfunction(Vessel v) { throw new NotImplementedException(); }
		// return true if at least a componet had a critical failure
		public static bool Critical(Vessel v) { throw new NotImplementedException(); }
		// return true if the part specified has a malfunction or critical failure
		public static bool Broken(Part part) { throw new NotImplementedException(); }
		// repair a specified part
		public static void Repair(Part part) { throw new NotImplementedException(); }

		// --- HABITAT --------------------------------------------------------------

		// return volume of internal habitat in m^3
		public static double Volume(Vessel v) { throw new NotImplementedException(); }
		// return surface of internal habitat in m^2
		public static double Surface(Vessel v) { throw new NotImplementedException(); }
		// return normalized pressure of internal habitat
		public static double Pressure(Vessel v) { throw new NotImplementedException(); }
		// return level of co2 of internal habitat
		public static double Poisoning(Vessel v) { throw new NotImplementedException(); }
		// return level of co2 of internal habitat
		public static double Humidity(Vessel v) { throw new NotImplementedException(); }
		// return proportion of radiation blocked by shielding
		public static double Shielding(Vessel v) { throw new NotImplementedException(); }
		// return living space factor
		public static double LivingSpace(Vessel v) { throw new NotImplementedException(); }
		// return comfort factor
		public static double Comfort(Vessel v) { throw new NotImplementedException(); }

		// --- SCIENCE --------------------------------------------------------------

		// return size of a file in a vessel drive
		public static double FileSize(Vessel v, string subject_id) { throw new NotImplementedException(); }
		// return size of a sample in a vessel drive
		public static double SampleSize(Vessel v, string subject_id) { throw new NotImplementedException(); }

		/// <summary>
		/// store a file on a vessel
		/// </summary>
		/// <param name="v"></param>
		/// <param name="subject_id"></param>
		/// <param name="amount"></param>
		public static void StoreFile(Vessel v, string subject_id, double amount) {
			_storeFile.Invoke(null, new object[] { v, subject_id, amount });
		}
		static MethodInfo _storeFile;

		/// <summary>
		/// store a sample on a vessel
		/// </summary>
		/// <param name="v"></param>
		/// <param name="subject_id"></param>
		/// <param name="amount"></param>
		public static void StoreSample(Vessel v, string subject_id, double amount) {
			_storeSample.Invoke(null, new object[] { v, subject_id, amount });
		}
		static MethodInfo _storeSample;

		// remove a file from a vessel
		public static void RemoveFile(Vessel v, string subject_id, double amount) { throw new NotImplementedException(); }
		// remove a sample from a vessel
		public static void RemoveSample(Vessel v, string subject_id, double amount) { throw new NotImplementedException(); }
	}
}

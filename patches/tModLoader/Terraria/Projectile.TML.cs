using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terraria
{
	public partial class Projectile
	{
		public ModProjectile modProjectile { get; internal set; }

		internal GlobalProjectile[] globalProjectiles = new GlobalProjectile[0];

		/// <summary>
		/// The damage types that this Projectile is affected by.
		/// Vanilla classes use DamageClass.Melee/Ranged/Magic/Summon/Throwing. Use ModContent.GetInstance<T>() for custom damage types.
		/// These will only matter if you leave Projectile.allDamageTypes null.
		/// </summary>
		public Dictionary<DamageClass, bool> DamageTypes = new Dictionary<DamageClass, bool>();

		/// <summary>
		/// Should all damage types affect this Projectile? Should none of them? That's what you decide here.
		/// Set to true to make all damage types affect this Projectile.
		/// Set to false to make no damage types affect this Projectile.
		/// Leave this field null to let the individual DamageTypes entries decide.
		/// </summary>
		public bool? allDamageTypes;

		// Get

		/// <summary> Gets the instance of the specified GlobalProjectile type. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public T GetGlobalProjectile<T>() where T : GlobalProjectile
			=> GetGlobalProjectile(ModContent.GetInstance<T>());

		/// <summary> Gets the local instance of the type of the specified GlobalProjectile instance. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="NullReferenceException"/>
		public T GetGlobalProjectile<T>(T baseInstance) where T : GlobalProjectile
			=> baseInstance.Instance(this) as T ?? throw new KeyNotFoundException($"Instance of '{typeof(T).Name}' does not exist on the current projectile.");
		
		/*
		// TryGet

		/// <summary> Gets the instance of the specified GlobalProjectile type. </summary>
		public bool TryGetGlobalProjectile<T>(out T result) where T : GlobalProjectile
			=> TryGetGlobalProjectile(ModContent.GetInstance<T>(), out result);

		/// <summary> Safely attempts to get the local instance of the type of the specified GlobalProjectile instance. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public bool TryGetGlobalProjectile<T>(T baseInstance, out T result) where T : GlobalProjectile {
			if (baseInstance == null || baseInstance.index < 0 || baseInstance.index >= globalProjectiles.Length) {
				result = default;

				return false;
			}

			result = baseInstance.Instance(this) as T;

			return result != null;
		}
		*/
	}
}

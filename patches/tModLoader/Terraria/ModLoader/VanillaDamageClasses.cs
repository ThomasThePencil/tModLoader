﻿using Terraria.Localization;

namespace Terraria.ModLoader
{
	public abstract class VanillaDamageClass : DamageClass
	{
		internal protected override string DisplayNameInternal => Language.GetTextValue(LangKey).Substring(1);

		protected abstract string LangKey { get; }

		protected override float GetBenefitsFrom(DamageClass damageClass) => 0;

		public override bool CountsAs(DamageClass damageClass) => false;
	}

	public class Generic : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.55";
	}

	public class NoScaling : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.55";
	}

	public class Melee : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.2";

		protected override float GetBenefitsFrom(DamageClass damageClass) => damageClass == Generic ? 1f : 0f;
	}

	public class Ranged : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.3";

		protected override float GetBenefitsFrom(DamageClass damageClass) => damageClass == Generic ? 1f : 0f;
	}

	public class Magic : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.4";

		protected override float GetBenefitsFrom(DamageClass damageClass) => damageClass == Generic ? 1f : 0f;
	}

	public class Summon : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.53";

		protected override float GetBenefitsFrom(DamageClass damageClass) => damageClass == Generic ? 1f : 0f;
	}

	public class Throwing : VanillaDamageClass
	{
		protected override string LangKey => "LegacyTooltip.58";

		protected override float GetBenefitsFrom(DamageClass damageClass) => damageClass == Generic ? 1f : 0f;
	}
}

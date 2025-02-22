using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVSlothComboPlugin.Combos
{
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Benefic = 3594,
            Benefic2 = 3610,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            MinorArcana = 7443,
            SleeveDraw = 7448,
            Malefic4 = 16555,
            LucidDreaming = 7562,
            Ascend = 3603,
            Swiftcast = 7561,
            CrownPlay = 25869,
            Astrodyne = 25870,
            FallMalefic = 25871,
            Malefic1 = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Combust = 3599,
            Play = 17055,
            LordOfCrowns = 7444,
            LadyOfCrown = 7445,

            // aoes
            Gravity = 3615,
            Gravity2 = 25872,

            // dots
            Combust3 = 16554,
            Combust2 = 3608,
            Combust1 = 3599,


            // heals
            Helios = 3600,
            AspectedHelios = 3601;

        public static class Buffs
        {
            public const short
            Swiftcast = 167,
            LordOfCrownsDrawn = 2054,
            LadyOfCrownsDrawn = 2055,
            AspectedHelios = 836;
        }

        public static class Debuffs
        {
            public const short
            Combust1 = 838,
            Combust2 = 843,
            Combust3 = 1881;
        }

        public static class Levels
        {
            public const byte
                Benefic2 = 26,
                MinorArcana = 50,
                Draw = 30,
                CrownPlay = 70;
        }
    }

    internal class AstrologianCardsOnDrawFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianCardsOnDrawFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Play)
            {
                var gauge = GetJobGauge<ASTGauge>();
                if (level >= AST.Levels.Draw && gauge.DrawnCard == CardType.NONE)
                    return AST.Draw;
            }

            return actionID;
        }
    }

    internal class AstrologianCrownPlayFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianCrownPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.CrownPlay)
            {
                var gauge = GetJobGauge<ASTGauge>();
                var ladyofCrown = HasEffect(AST.Buffs.LadyOfCrownsDrawn);
                var lordofCrown = HasEffect(AST.Buffs.LordOfCrownsDrawn);
                var minorArcanaCD = GetCooldown(AST.MinorArcana);
                if (level >= AST.Levels.MinorArcana && gauge.DrawnCrownCard == CardType.NONE)
                    return AST.MinorArcana;
            }

            return actionID;
        }
    }

    internal class AstrologianBeneficFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianBeneficFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Benefic2)
            {
                if (level < AST.Levels.Benefic2)
                    return AST.Benefic;
            }

            return actionID;
        }
    }

    internal class AstrologianAscendFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianAscendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Swiftcast)
            {
                if (IsEnabled(CustomComboPreset.AstrologianAscendFeature))
                {
                    if (HasEffect(AST.Buffs.Swiftcast))
                        return AST.Ascend;
                }

                return OriginalHook(AST.Swiftcast);
            }

            return actionID;
        }
    }

    internal class AstrologianDpsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianDpsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.FallMalefic || actionID == AST.Malefic4 || actionID == AST.Malefic3 || actionID == AST.Malefic2 || actionID == AST.Malefic1)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var fallmalefic = GetCooldown(AST.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature))
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.4)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature))
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.4 && drawCD.CooldownRemaining < 30)
                        return AST.Draw;
                     
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature))
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.4)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2)
                        return AST.LucidDreaming;
                }

                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && level >= 72 && incombat)
                {
                    if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                        return AST.Combust3;
                }

                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && level >= 46 && level <= 71 && incombat)
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Combust2;
                }

                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && level >= 4 && level <= 45 && incombat)
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Combust1;
                }
            }

            return actionID;
        }
    }
   
    internal class AstrologianHeliosFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianHeliosFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.AspectedHelios)
            {
                if (HasEffect(AST.Buffs.AspectedHelios))
                    return AST.Helios;
            }

            return actionID;
        }
    }
    internal class AstrologianDpsAoEFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.AstrologianDpsAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Gravity || actionID == AST.Gravity2)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var gravityCD = GetCooldown(AST.Gravity);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature))
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.4)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature))
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.4 && drawCD.CooldownRemaining < 30)
                        return AST.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature))
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.4)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature))
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && actionIDCD.CooldownRemaining > 0.2)
                        return AST.LucidDreaming;
                }
            }

            return actionID;
        }
    }
}
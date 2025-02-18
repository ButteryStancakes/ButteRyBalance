using ButteRyBalance.Network;
using ButteRyBalance.Overrides.Moons;
using System.Collections.Generic;
using UnityEngine;

namespace ButteRyBalance.Overrides
{
    internal class OverrideCoordinator
    {
        internal static void ApplyOnServer()
        {
            if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
            {
                foreach (SelectableLevel level in StartOfRound.Instance.levels)
                {
                    switch (level.name)
                    {
                        case "ExperimentationLevel":
                            ExperimentationOverrides.Setup(level);
                            break;
                        case "AssuranceLevel":
                            AssuranceOverrides.Setup(level);
                            break;
                        case "VowLevel":
                            VowOverrides.Setup(level);
                            break;
                        case "OffenseLevel":
                            OffenseOverrides.Setup(level);
                            break;
                        case "MarchLevel":
                            MarchOverrides.Setup(level);
                            break;
                        case "AdamanceLevel":
                            AdamanceOverrides.Setup(level);
                            break;
                        case "RendLevel":
                            RendOverrides.Setup(level);
                            break;
                        case "DineLevel":
                            DineOverrides.Setup(level);
                            break;
                        case "TitanLevel":
                            TitanOverrides.Setup(level);
                            break;
                        case "ArtificeLevel":
                            ArtificeOverrides.Setup(level);
                            break;
                        case "EmbrionLevel":
                            EmbrionOverrides.Setup(level);
                            break;
                    }
                }
            }

            foreach (KeyValuePair<string, EnemyType> enemy in Common.enemies)
            {
                switch (enemy.Key)
                {
                    case "Butler":
                        if (Configuration.butlerManorChance.Value)
                        {
                            enemy.Value.increasedChanceInterior = 1;
                            Plugin.Logger.LogDebug("Butler: Increased chance in manors");
                        }
                        break;
                    case "CaveDweller":
                        if (Configuration.maneaterPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Maneater: Power level {enemy.Value.PowerLevel} -> 3");
                            enemy.Value.PowerLevel = 3f;
                        }
                        break;
                    case "RadMech":
                        if (Configuration.robotFog.Value)
                        {
                            enemy.Value.canSeeThroughFog = true;
                            Plugin.Logger.LogDebug("Old Bird: See through fog");
                        }
                        break;
                    case "SpringMan":
                        if (Configuration.coilheadCurves.Value)
                        {
                            // ~60% spawn rate at 7 AM, 100% spawn rate by noon
                            enemy.Value.probabilityCurve = AnimationCurve.EaseInOut(0.1f, 0.6142857f, 0.3333333f, 1f);
                            Plugin.Logger.LogDebug("Coil-head: Time of day curve");
                            enemy.Value.numberSpawnedFalloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            enemy.Value.useNumberSpawnedFalloff = true;
                            Plugin.Logger.LogDebug("Coil-head: Spawn count curve");
                        }
                        if (Configuration.coilheadPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Coil-head: Power level {enemy.Value.PowerLevel} -> 2");
                            enemy.Value.PowerLevel = 2f;
                        }
                        break;
                }
            }

            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.name)
                {
                    case "MetalSheet":
                        if (Configuration.metalSheetPrice.Value)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.minValue: {item.minValue} -> 35");
                            item.minValue = 35;
                            Plugin.Logger.LogDebug($"{item.name}.maxValue: {item.maxValue} -> 85");
                            item.maxValue = 85;
                        }
                        break;
                }
            }
        }

        internal static void ApplyOnAllClients()
        {
            Dictionary<string, float> adjustedWeights = new()
            {
                { "BottleBin",       1.15f },   // vanilla: 1.18
                { "Brush",           1.07f },   // vanilla: 1.1
                { "Candy",              1f },   // vanilla: 1.1
                { "ChemicalJug",      1.4f },   // vanilla: 1.3
                { "Clock",            1.2f },   // vanilla: 1.25
                { "Cog1",             1.2f },   // vanilla: 1.15
                { "EnginePart1",     1.18f },   // vanilla: 1.15
                { "FancyCup",         1.2f },   // vanilla: 1.15
                { "FancyLamp",       1.25f },   // vanilla: 1.2
                { "Flask",            1.1f },   // vanilla: 1.18
                { "GarbageLid",       1.1f },   // vanilla: 1
                { "Hairdryer",        1.1f },   // vanilla: 1.07
                { "MetalSheet",      1.23f },   // vanilla: 1.25
                { "Ring",            1.08f },   // vanilla: 1.15
                { "SoccerBall",      1.13f },   // vanilla: 1.18
                { "StopSign",         1.2f },   // vanilla: 1.23
                { "TeaKettle",       1.15f },   // vanilla: 1.2
                { "YieldSign",        1.3f },   // vanilla: 1.4
            };

            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.name)
                {
                    case "Jetpack":
                        if (BRBNetworker.Instance.JetpackReduceDiscount.Value && item.highestSalePercentage != 60)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.highestSalePercentage: {item.highestSalePercentage}% -> 60%");
                            item.highestSalePercentage = 60;
                        }
                        break;
                    case "RadarBooster":
                        if (BRBNetworker.Instance.RadarBoosterPrice.Value && item.creditsWorth != 50)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.creditsWorth: ${item.creditsWorth} -> $50");
                            item.creditsWorth = 50;
                        }
                        break;
                    case "StunGrenade":
                        if (BRBNetworker.Instance.StunGrenadePrice.Value && item.creditsWorth != 40)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.creditsWorth: ${item.creditsWorth} -> $40");
                            item.creditsWorth = 40;
                        }
                        break;
                    case "ZapGun":
                        if (BRBNetworker.Instance.ZapGunPrice.Value && item.creditsWorth != 200)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.creditsWorth: ${item.creditsWorth} -> $200");
                            item.creditsWorth = 200;
                        }
                        break;
                }

                if (BRBNetworker.Instance.ScrapAdjustWeights.Value && adjustedWeights.TryGetValue(item.name, out float weight) && item.weight != weight)
                {
                    Plugin.Logger.LogDebug($"{item.name}.weight: ${item.weight} -> {weight}");
                    item.weight = weight;
                }
            }

            //StartOfRound.Instance.SetPlanetsWeather();
        }
    }
}

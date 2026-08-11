using ButteRyBalance.Components;
using ButteRyBalance.Network;
using ButteRyBalance.Overrides.Moons;
using ButteRyBalance.Patches;
using ButteRyBalance.Patches.Items;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace ButteRyBalance.Overrides
{
    internal class OverrideCoordinator
    {
        const int VANILLA_CRUISER_PRICE = 370, VANILLA_JETPACK_PRICE = 900;

        internal static void ApplyOnServer()
        {
            List<IndoorMapHazardType> indoorMapHazardTypes = [];
            foreach (SelectableLevel level in StartOfRound.Instance.levels)
            {
                if (!BRBNetworker.Instance.MoonsKillSwitch.Value)
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

                foreach (IndoorMapHazard indoorMapHazard in level.indoorMapHazards)
                {
                    if (indoorMapHazard.hazardType != null && !indoorMapHazardTypes.Contains(indoorMapHazard.hazardType))
                        indoorMapHazardTypes.Add(indoorMapHazard.hazardType);
                }
            }

            foreach (KeyValuePair<string, EnemyType> enemy in Common.enemies)
            {
                switch (enemy.Key)
                {
                    case "Butler":
                        if (Configuration.butlerManorChance.Value)
                        {
                            enemy.Value.increasedChanceInterior = (int)Common.InteriorID.Manor;
                            Plugin.Logger.LogDebug("Butler: Increased chance in manors");
                        }
                        break;
                    case "CadaverGrowths":
                        if (Configuration.cadaversPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Cadavers: Power level {enemy.Value.PowerLevel} -> 4");
                            enemy.Value.PowerLevel = 4f;
                        }
                        break;
                    case "CaveDweller":
                        if (Configuration.maneaterPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Maneater: Power level {enemy.Value.PowerLevel} -> 3");
                            enemy.Value.PowerLevel = 3f;
                        }
                        break;
                    case "DressGirl":
                        enemy.Value.pushPlayerForce = 0f;
                        enemy.Value.pushPlayerDistance = 0f;
                        Plugin.Logger.LogDebug("Girl: Don't push");
                        break;
                    case "Puffer":
                        if (Configuration.pufferPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Spore lizard: Power level {enemy.Value.PowerLevel} -> 0.5");
                            enemy.Value.PowerLevel = 0.5f;
                        }
                        break;
                    case "RadMech":
                        enemy.Value.canSeeThroughFog = true;
                        Plugin.Logger.LogDebug("Old Bird: See through fog");
                        break;
                    case "SpringMan":
                        if (Configuration.coilheadPower.Value)
                        {
                            Plugin.Logger.LogDebug($"Coil-head: Power level {enemy.Value.PowerLevel} -> 2");
                            enemy.Value.PowerLevel = 2f;
                        }
                        break;
                }
            }
        }

        internal static void ApplyOnAllClients()
        {
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.name)
                {
                    case "Jetpack":
                        if (BRBNetworker.Instance.JetpackUtility.Value && !item.disallowUtilitySlot)
                        {
                            item.disallowUtilitySlot = true;
                            Plugin.Logger.LogDebug($"{item.name}.item.disallowUtilitySlot: False -> True");
                        }
                        if (BRBNetworker.Instance.JetpackPrice.Value != default && BRBNetworker.Instance.JetpackPrice.Value != VANILLA_JETPACK_PRICE)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.creditsWorth: ${item.creditsWorth} -> ${BRBNetworker.Instance.JetpackPrice.Value}");
                            item.creditsWorth = BRBNetworker.Instance.JetpackPrice.Value;
                        }
                        break;
                    case "ProFlashlight":
                        if (BRBNetworker.Instance.ProFlashlightPrice.Value && item.creditsWorth != 32)
                        {
                            Plugin.Logger.LogDebug($"{item.name}.creditsWorth: ${item.creditsWorth} -> $32");
                            item.creditsWorth = 32;
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
            }

            //StartOfRound.Instance.SetPlanetsWeather();

            if (BRBNetworker.Instance.NerfNightVision.Value)
            {
                foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                {
                    HDAdditionalLightData nightVisionHDRP = player.nightVision.GetComponent<HDAdditionalLightData>();
                    if (nightVisionHDRP != null)
                    {
                        nightVisionHDRP.intensity = 2488f;
                        nightVisionHDRP.range = 8.3f;
                    }
                    else
                    {
                        player.nightVision.intensity = 197.98874f;
                        player.nightVision.range = 8.3f;
                    }
                }
            }

            foreach (KeyValuePair<string, EnemyType> enemy in Common.enemies)
            {
                switch (enemy.Key)
                {
                    case "CadaverBloom":
                        if (BRBNetworker.Instance.CadaverTarget.Value && enemy.Value.enemyPrefab != null && enemy.Value.enemyPrefab.GetComponentInChildren<IVisibleThreat>() == null)
                        {
                            CadaverBloomAI cadaverBloomAI = enemy.Value.enemyPrefab.GetComponentInChildren<CadaverBloomAI>();
                            if (cadaverBloomAI != null)
                            {
                                cadaverBloomAI.gameObject.AddComponent<CadaverBloomThreat>().cadaverBloomAI = cadaverBloomAI;
                                Plugin.Logger.LogDebug($"Cadaver Bloom: Add IVisibleThreat");

                                if (!cadaverBloomAI.GetComponent<Collider>())
                                {
                                    BoxCollider boxCollider = cadaverBloomAI.gameObject.AddComponent<BoxCollider>();
                                    boxCollider.isTrigger = true;
                                    boxCollider.center = new(0f, 0.4428673f, 0f);
                                    boxCollider.size = new(0f, 0.4412155f, 0f);
                                    Plugin.Logger.LogDebug($"Cadaver Bloom: Add collider");
                                }
                            }
                        }
                        break;
                    case "CaveDweller":
                        if (BRBNetworker.Instance.ManeaterTarget.Value && enemy.Value.enemyPrefab != null && enemy.Value.enemyPrefab.GetComponentInChildren<IVisibleThreat>() == null)
                        {
                            CaveDwellerAI caveDwellerAI = enemy.Value.enemyPrefab.GetComponentInChildren<CaveDwellerAI>();
                            if (caveDwellerAI != null)
                            {
                                caveDwellerAI.gameObject.AddComponent<ManeaterThreat>().caveDwellerAI = caveDwellerAI;
                                Plugin.Logger.LogDebug($"Maneater: Add IVisibleThreat");
                            }
                        }
                        break;
                    case "Crawler":
                        if (BRBNetworker.Instance.StunLonger.Value)
                        {
                            Plugin.Logger.LogDebug($"Thumper: Stun multiplier {enemy.Value.stunTimeMultiplier} -> 1");
                            enemy.Value.stunTimeMultiplier = 1f;
                        }
                        break;
                    case "HoarderBug":
                        if (BRBNetworker.Instance.StunLonger.Value)
                        {
                            Plugin.Logger.LogDebug($"Hoarding bug: Stun multiplier {enemy.Value.stunTimeMultiplier} -> 0.5");
                            enemy.Value.stunTimeMultiplier = 0.5f;
                        }
                        break;
                }
            }

            if (!Common.INSTALLED_VERSION55_COMPANY_CRUISER)
            {
                if (BRBNetworker.Instance.CruiserPrice.Value != default && BRBNetworker.Instance.CruiserPrice.Value != VANILLA_CRUISER_PRICE && Common.Terminal?.buyableVehicles != null)
                {
                    BuyableVehicle cruiser = Common.Terminal.buyableVehicles.FirstOrDefault(buyableVehicle => buyableVehicle.vehicleDisplayName == "Cruiser");
                    if (cruiser != null)
                    {
                        int price = cruiser.creditsWorth;
                        cruiser.creditsWorth = BRBNetworker.Instance.CruiserPrice.Value;

                        if (Common.Terminal.terminalNodes?.allKeywords != null)
                        {
                            TerminalKeyword buyKeyword = Common.Terminal.terminalNodes.allKeywords.FirstOrDefault(keyword => keyword.word == "buy");
                            TerminalKeyword cruiserKeyword = Common.Terminal.terminalNodes.allKeywords.FirstOrDefault(keyword => keyword.word == "cruiser");
                            if (buyKeyword?.compatibleNouns != null && cruiserKeyword != null)
                            {
                                TerminalNode buyCruiser = buyKeyword.compatibleNouns.FirstOrDefault(compatibleNoun => compatibleNoun.noun == cruiserKeyword)?.result;
                                if (buyCruiser != null)
                                {
                                    buyCruiser.itemCost = BRBNetworker.Instance.CruiserPrice.Value;

                                    if (buyCruiser.terminalOptions != null)
                                    {
                                        TerminalKeyword confirmKeyword = Common.Terminal.terminalNodes.allKeywords.FirstOrDefault(keyword => keyword.word == "confirm");
                                        if (confirmKeyword != null)
                                        {
                                            TerminalNode buyCruiser2 = buyCruiser.terminalOptions.FirstOrDefault(compatibleNoun => compatibleNoun.noun == confirmKeyword)?.result;
                                            if (buyCruiser2 != null)
                                            {
                                                buyCruiser2.itemCost = BRBNetworker.Instance.CruiserPrice.Value;
                                                Plugin.Logger.LogDebug($"Cruiser: Price ${price} -> ${BRBNetworker.Instance.CruiserPrice.Value}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (BRBNetworker.Instance.CruiserRegen.Value)
                {
                    Plugin.Logger.LogDebug($"Cruiser: Regen ({VehicleControllerPatches.criticalDurability}, {VehicleControllerPatches.regenInterval}) => (7, 16)");
                    VehicleControllerPatches.criticalDurability = 7;
                    VehicleControllerPatches.regenInterval = 16f;
                }
                if (BRBNetworker.Instance.CruiserCrashDamage.Value)
                {
                    Plugin.Logger.LogDebug($"Cruiser: Crash ({VehicleControllerPatches.scrapingStress}, {VehicleControllerPatches.adjustableCrashSpeed}) => (0.35, 27)");
                    VehicleControllerPatches.scrapingStress = 0.35f;
                    VehicleControllerPatches.adjustableCrashSpeed = 27f;
                }
                Plugin.Logger.LogDebug($"Cruiser: Turbo {VehicleControllerPatches.turboBoosts} => {BRBNetworker.Instance.CruiserTurbos.Value}");
                VehicleControllerPatches.turboBoosts = BRBNetworker.Instance.CruiserTurbos.Value;

                if (BRBNetworker.Instance.CruiserPatchEnemies.Value && !Common.INSTALLED_SCANDALS_TWEAKS)
                    Plugin.Logger.LogWarning("\"Patch Enemy Protection\" is enabled in the Cruiser settings, but Scandal's Tweaks is not installed. Some patches will not apply to this session!");
            }

            if (BRBNetworker.Instance.WeedKillerDamage.Value != default)
            {
                Plugin.Logger.LogDebug($"Weed killer: Damage {SprayPaintPatches.damageNumber} => {BRBNetworker.Instance.WeedKillerDamage.Value}");
                SprayPaintPatches.damageNumber = BRBNetworker.Instance.WeedKillerDamage.Value;
            }
        }
    }
}

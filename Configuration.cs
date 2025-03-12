using BepInEx.Configuration;
//using ButteRyBalance.Network;

namespace ButteRyBalance
{
    internal class Configuration
    {
        static ConfigFile configFile;

        internal static ConfigEntry<bool> coilheadStunReset, jesterWalkThrough, butlerManorChance, butlerStealthStab, butlerLongCooldown, jesterLongCooldown, butlerKnifePrice, knifeShortCooldown, knifeAutoSwing, maneaterLimitGrowth, maneaterWideTurns, maneaterScrapGrowth, moonsKillSwitch, dineReduceButlers, barberDynamicSpawns, foggyLimit, experimentationNoEvents, experimentationNoGiants, experimentationNoEggs, experimentationNoNuts, experimentationBuffScrap, randomIndoorFog, assuranceNerfScrap, assuranceMasked, vowAdjustScrap, vowNoCoils, vowMineshafts, shrinkMineshafts, offenseBuffScrap, offenseMineshafts, offenseMasked, offenseNerfEclipse, vowNoTraps, marchShrink, marchBuffScrap, marchRainy, multiplayerWeather, butlerSquishy, adamanceBuffScrap, adamanceReduceChaos, coilheadCurves, rendMineshafts, rendShrink, rendAdjustIndoor, rendAdjustScrap, rendWorms, metalSheetPrice, coilheadPower, dineAdjustIndoor, dineBuffScrap, dineAdjustOutdoor, dineAdjustCurves, titanBuffScrap, titanAddGold, titanMineshafts, titanAdjustEnemies, titanWeeds, dineMasked, giantSnowSight, /*giantForgetTargets,*/ dineFloods, robotFog, nutcrackerGunPrice, nutcrackerKevlar, jetpackBattery, jetpackReduceDiscount, tzpExpandCapacity, jetpackInertia, artificeBuffScrap, artificeInteriors, artificeTurrets, zapGunPrice, radarBoosterPrice, stunGrenadePrice, scrapAdjustWeights, maneaterPower, embrionMineshafts, embrionBuffScrap, embrionWeeds, embrionAdjustEnemies, embrionMega, infestationRework, infestationButlers, infestationMasked, infestationBarbers, foxSquishy, zapGunBattery, offenseBees, apparatusPrice, robotRider/*, jetpackShortCircuit*/;

        internal static void Init(ConfigFile cfg)
        {
            configFile = cfg;

            MiscConfig();
            InfestationConfig();
            EnemyConfig();
            ItemConfig();
            MoonConfig();

            MigrateLegacyConfig();

            /*cfg.SettingChanged += delegate
            {
                BRBNetworker.ConfigUpdated();
            };
            cfg.ConfigReloaded += delegate
            {
                BRBNetworker.ConfigUpdated();
            };*/
        }

        static void ItemConfig()
        {
            scrapAdjustWeights = configFile.Bind(
                "Items",
                "Adjust Scrap Weights",
                false,
                "Makes some minor alterations to the weight of several scrap items.");
            // Apparatus
            apparatusPrice = configFile.Bind(
                "Item.Apparatus",
                "Randomize Price",
                true,
                "Randomizes the price of the apparatus once it has been unplugged.");
            // Jetpack
            jetpackBattery = configFile.Bind(
                "Item.Jetpack",
                "Reduce Battery",
                true,
                "Reduces jetpack battery back to 40s (from 50s), like the v50 beta.");
            jetpackReduceDiscount = configFile.Bind(
                "Item.Jetpack",
                "Reduce Max Discount",
                true,
                "Jetpack price will never go beyond 60% off ($360)");
            jetpackInertia = configFile.Bind(
                "Item.Jetpack",
                "v49 Controls",
                false,
                "Re-enables v49 jetpack controls, which had much greater inertia. This value can be enabled client-side, but will additionally be enforced for all players if enabled by the host.");
            // Kitchen knife
            knifeShortCooldown = configFile.Bind(
                "Item.KitchenKnife",
                "Short Cooldown",
                true,
                "The knife will deal damage faster.");
            knifeAutoSwing = configFile.Bind(
                "Item.KitchenKnife",
                "Auto-Swing",
                true,
                "(Client-side) Holding the attack button will automatically swing the knife. When damage is dealt, your swing speed will decrease to avoid wasting hits.");
            // Radar booster
            radarBoosterPrice = configFile.Bind(
                "Item.RadarBooster",
                "Buff Price",
                true,
                "Reduces the cost of the radar booster from $60 to $50, like in v40.");
            // Stun grenade
            stunGrenadePrice = configFile.Bind(
                "Item.StunGrenade",
                "Nerf Price",
                true,
                "Increases the cost of stun grenades from $30 to $40, like in v40.");
            // Tattered metal sheet
            metalSheetPrice = configFile.Bind(
                "Item.MetalSheet",
                "Increase Value",
                true,
                "Increases the average sell value of metal sheets.");
            // TZP inhalant
            tzpExpandCapacity = configFile.Bind(
                "Item.TZPInhalant",
                "Expand Capacity",
                true,
                "Increases capacity of TZP inhalant from 22s to 34s.");
            // Zap gun
            zapGunPrice = configFile.Bind(
                "Item.ZapGun",
                "Buff Price",
                true,
                "Reduces the cost of the zap gun from $400 to $200, like in v9.");
            zapGunBattery = configFile.Bind(
                "Item.ZapGun",
                "Increase Battery",
                true,
                "Increases zap gun battery back to 120s (from 22s), like v9.");
        }

        static void EnemyConfig()
        {
            // Barber
            barberDynamicSpawns = configFile.Bind(
                "Enemy.Barber",
                "Dynamic Spawn Settings",
                true,
                "Barbers will spawn in pairs, up to 8 total, in factories and manors. In mineshafts, they are limited to 1 total.");

            // Butler
            butlerManorChance = configFile.Bind(
                "Enemy.Butler",
                "Manor Increased Chance",
                true,
                "Butlers have an increased chance to spawn in manor interiors.");
            butlerStealthStab = configFile.Bind(
                "Enemy.Butler",
                "Stealth Stab",
                true,
                "When triggering a Butler to attack by bumping into it (rare chance), it will no longer \"berserk\", unless the offending player is alone.");
            butlerLongCooldown = configFile.Bind(
                "Enemy.Butler",
                "Slow Attacks",
                true,
                "Butlers will deal damage slower, as long as they haven't been attacked before. In singleplayer, this setting will also increase their HP from 2 to 3.");
            butlerSquishy = configFile.Bind(
                "Enemy.Butler",
                "Squishy",
                true,
                "Butlers take bonus damage from shotgun shots and explosions, allowing those to kill in one hit.");
            butlerKnifePrice = configFile.Bind(
                "Enemy.Butler",
                "Randomize Knife Price",
                true,
                "Restores the kitchen knife's price randomization. On average, it will be significantly more valuable than $35, the vanilla price.");

            // Coil-head
            coilheadStunReset = configFile.Bind(
                "Enemy.Coilhead",
                "Stuns Reset",
                true,
                "Coil-heads will begin \"recharging\" when they are stunned by stun grenades, radar boosters, or homemade flashbangs.");
            coilheadCurves = configFile.Bind(
                "Enemy.Coilhead",
                "Adjust Spawn Curves",
                true,
                "Adds two multipliers to Coil-head spawn rates; one reduces their chances earlier in the day, and the other reduces the chance of multiple spawning in one day.");
            coilheadPower = configFile.Bind(
                "Enemy.Coilhead",
                "Increase Power Level",
                true,
                "Increase Coil-heads' power level from 1 to 2, to reduce the potential for forming bad enemy combos.");

            // Forest Keeper
            giantSnowSight = configFile.Bind(
                "Enemy.ForestKeeper",
                "Treat Blizzard Like Fog",
                true,
                "On snowy moons, Forest Keepers will have their line-of-sight range reduced, the same way as in foggy weather.");

            // Kidnapper Fox
            foxSquishy = configFile.Bind(
                "Enemy.KidnapperFox",
                "Squishy",
                true,
                "Reduce kidnapper fox HP from 7 to 6, and shotgun deals bonus damage. This allows the fox to be killed easier with a single explosion (like easter eggs) or both barrels from the shotgun.");

            // Jester
            jesterWalkThrough = configFile.Bind(
                "Enemy.Jester",
                "Walk-Through",
                true,
                "Lets you walk through Jesters once they begin winding up.");
            jesterLongCooldown = configFile.Bind(
                "Enemy.Jester",
                "Long Cooldown",
                true,
                "Slightly increase the average time before a Jester winds, in multiplayer.");

            // Maneater
            maneaterPower = configFile.Bind(
                "Enemy.Maneater",
                "Increase Power Level",
                true,
                "Increase the Maneater's power level from 2 to 3, to reduce the potential for forming bad enemy combos.");
            maneaterLimitGrowth = configFile.Bind(
                "Enemy.Maneater",
                "Limit Growth",
                true,
                "Sharply limits (or completely prevents) the Maneater's ability to transform before it has been encountered by a player.");
            maneaterScrapGrowth = configFile.Bind(
                "Enemy.Maneater",
                "Metabolism",
                true,
                "Eating scrap will permanently reduce the speed at which a Maneater transforms from crying.");
            maneaterWideTurns = configFile.Bind(
                "Enemy.Maneater",
                "Limit Turn Speed",
                true,
                "Adult Maneaters will have far worse turning speed when lunging, as long as they are inside the building.");

            // Nutcracker
            nutcrackerGunPrice = configFile.Bind(
                "Enemy.Nutcracker",
                "Randomize Shotgun Price",
                true,
                "Restores randomization for the shotgun's price.");
            nutcrackerKevlar = configFile.Bind(
                "Enemy.Nutcracker",
                "Last Stand",
                true,
                "Nutcrackers will resist instant death from a shotgun blast if they are at full health, immediately entering \"berserk\" state.");

            // Old Bird
            robotFog = configFile.Bind(
                "Enemy.OldBird",
                "See Through Fog",
                true,
                "Old Birds can see through fog with their powerful searchlights.");
            robotRider = configFile.Bind(
                "Enemy.OldBird",
                "Restore Mech Riding",
                true,
                "Lets you ride and drop items on top of Old Birds, like in v64.");
        }

        static void MoonConfig()
        {
            moonsKillSwitch = configFile.Bind(
                "Moons",
                "Kill Switch",
                false,
                "If this setting is enabled, all other settings in the \"Moon\" category will be disabled. This is helpful if you want to use an external mod to configure them.");

            // Experimentation
            experimentationBuffScrap = configFile.Bind(
                "Moon.Experimentation",
                "Buff Scrap",
                true,
                "Increases the amount of scrap that spawns on Experimentation, like in v9. (8-11 -> 11-15)");
            experimentationNoEvents = configFile.Bind(
                "Moon.Experimentation",
                "No Random Events",
                true,
                "Disables all random events on Experimentation. (Meteor showers, infestations, spooky fog)");
            experimentationNoGiants = configFile.Bind(
                "Moon.Experimentation",
                "No Forest Keepers",
                true,
                "Forest Keepers no longer have a random chance to spawn on Experimentation.");
            experimentationNoNuts = configFile.Bind(
                "Moon.Experimentation",
                "No Nutcrackers",
                false,
                "Nutcrackers no longer have a random chance to spawn on Experimentation.");
            experimentationNoEggs = configFile.Bind(
                "Moon.Experimentation",
                "No Easter Eggs",
                true,
                "Easter eggs no longer appear in Experimentation's loot pool.");

            // Assurance
            assuranceNerfScrap = configFile.Bind(
                "Moon.Assurance",
                "Nerf Scrap",
                true,
                "Reduces scrap counts on Assurance from 13-15 to 11-15, like in v9. Swaps several items' rarities with the corresponding values from Offense's loot pool.");
            assuranceMasked = configFile.Bind(
                "Moon.Assurance",
                "Spawn Masked",
                true,
                "Allow \"Masked\" enemies to spawn very rarely on Assurance, since Comedy and Tragedy spawn there.");

            // Vow
            vowAdjustScrap = configFile.Bind(
                "Moon.Vow",
                "Adjust Scrap",
                true,
                "Slightly reduce the \"quality\" of scrap items on Vow, to keep it more in line with Experimentation and Assurance, and a little below March.");
            vowMineshafts = configFile.Bind(
                "Moon.Vow",
                "Mostly Mineshafts",
                true,
                "Significantly reduce the likelihood of factory interiors on Vow.");
            vowNoCoils = configFile.Bind(
                "Moon.Vow",
                "No Coil-heads",
                true,
                "Coil-heads no longer have a random chance to spawn on Vow.");
            vowNoTraps = configFile.Bind(
                "Moon.Vow",
                "No Traps",
                true,
                "Removes landmines and turrets from Vow, like in v9.");

            // Offense
            offenseBuffScrap = configFile.Bind(
                "Moon.Offense",
                "Buff Scrap",
                true,
                "Swaps several items' rarities with the corresponding values from Assurance's loot pool.");
            offenseBees = configFile.Bind(
                "Moon.Offense",
                "Circuit Bees",
                true,
                "Allow circuit bees to spawn on Offense, like Assurance.");
            offenseMineshafts = configFile.Bind(
                "Moon.Offense",
                "Mostly Mineshafts",
                true,
                "Significantly reduce the likelihood of factory interiors on Offense.");
            offenseMasked = configFile.Bind(
                "Moon.Offense",
                "Spawn Masked",
                true,
                "Allow \"Masked\" enemies to spawn rarely on Offense, since Comedy spawns there.");
            offenseNerfEclipse = configFile.Bind(
                "Moon.Offense",
                "Nerf Eclipse",
                true,
                "Reduces enemy spawns per \"wave\" during eclipses on Offense from 4 to 3.");

            // March
            marchBuffScrap = configFile.Bind(
                "Moon.March",
                "Buff Scrap",
                true,
                "Increases the amount of scrap that spawns on March, like in v9. (13-16 -> 16-20)");
            marchShrink = configFile.Bind(
                "Moon.March",
                "Shrink Interior",
                true,
                "Reduces March's interior size multiplier from 2x to 1.8x, like in v9.");
            marchRainy = configFile.Bind(
                "Moon.March",
                "Always Rainy",
                true,
                "Rainy weather becomes the new default weather on March, and mild weather becomes a rare alternate weather, like eclipses and storms.");

            // Adamance
            adamanceBuffScrap = configFile.Bind(
                "Moon.Adamance",
                "Buff Scrap",
                true,
                "Increases the amount of scrap that spawns on Adamance, like in the v50 betas. (16-18 -> 19-23)");
            adamanceReduceChaos = configFile.Bind(
                "Moon.Adamance",
                "Reduce Chaos",
                true,
                "Curbs the Artifice-level shenanigans by reducing outdoor power from 13 to 10. Eclipses spawn 2 enemies per wave instead of 3.");

            // Rend
            rendMineshafts = configFile.Bind(
                "Moon.Rend",
                "Rare Mineshafts",
                true,
                "Sharply reduce mineshaft chance on Rend, so it goes back to being almost always manors.");
            rendShrink = configFile.Bind(
                "Moon.Rend",
                "Shrink Interior",
                true,
                "Reduces Rend's interior size multiplier from 1.8x to 1.6x.");
            rendAdjustIndoor = configFile.Bind(
                "Moon.Rend",
                "Adjust Indoor Enemies",
                true,
                "Increase the variety of enemies that spawn inside the building on Rend, to reduce total nutcracker domination.");
            rendAdjustScrap = configFile.Bind(
                "Moon.Rend",
                "Adjust Scrap",
                true,
                "Slightly reduces the overall quality of items on Rend.");
            rendWorms = configFile.Bind(
                "Moon.Rend",
                "Restore Earth Leviathans",
                true,
                "Allow Earth Leviathans, which were removed from Rend in v56, to spawn again.");

            // Dine
            dineBuffScrap = configFile.Bind(
                "Moon.Dine",
                "Buff Scrap",
                true,
                "Significantly improves average quality of scrap on Dine. Also increases scrap counts from 22-25 to 22-27, like in v50 betas.");
            dineReduceButlers = configFile.Bind(
                "Moon.Dine",
                "Reduce Butler Chance",
                true,
                "Reduces the spawn weight of Butlers on Dine. Intended to be combined with the \"Manor Increased Chance\" setting for Butlers.");
            dineAdjustIndoor = configFile.Bind(
                "Moon.Dine",
                "Adjust Indoor Enemies",
                true,
                "Increase the variety of enemies that spawn inside the building on Dine.");
            dineAdjustOutdoor = configFile.Bind(
                "Moon.Dine",
                "Adjust Outdoor Enemies",
                true,
                "Decrease giants' and dogs' spawn chance, resulting in more robots and worms.");
            dineAdjustCurves = configFile.Bind(
                "Moon.Dine",
                "Adjust Spawn Curves",
                true,
                "Rolls back the outdoor spawn curve from v60, to match the one from v56. This will significantly reduce enemies spawning outside in the early morning hours.");
            dineFloods = configFile.Bind(
                "Moon.Dine",
                "Fix Floods",
                true,
                "Reduces the water level at the start of flooded weather days on Dine, so that the main entrance no longer starts out underwater.");
            dineMasked = configFile.Bind(
                "Moon.Dine",
                "Add Masked",
                true,
                "Allow \"Masked\" enemies to spawn on Dine, since Comedy and Tragedy spawn there.");

            // Titan
            titanBuffScrap = configFile.Bind(
                "Moon.Titan",
                "Buff Scrap",
                true,
                "Significantly improves average quality of scrap on Titan. Also increases scrap counts from 28-31 to 28-35, like in v50 betas.");
            titanAddGold = configFile.Bind(
                "Moon.Titan",
                "Gold Rush",
                true,
                "Adds gold bars to Titan's loot pool as a rare find.");
            titanMineshafts = configFile.Bind(
                "Moon.Titan",
                "Reduce Mineshafts",
                true,
                "Reduces mineshaft chance on Titan, since they are often far larger than any other moon's interiors.");
            titanAdjustEnemies = configFile.Bind(
                "Moon.Titan",
                "Adjust Enemies",
                true,
                "Adjusts enemy balance, mainly to reduce Jesters and giants. Also slightly increases Old Birds.");
            titanWeeds = configFile.Bind(
                "Moon.Titan",
                "No Vain Shrouds",
                true,
                "Disable vain shroud growth on Titan, to prevent the Kidnapper Fox from camping the ship.");

            // Artifice
            artificeBuffScrap = configFile.Bind(
                "Moon.Artifice",
                "Buff Scrap",
                true,
                "Restores Artifice's scrap counts from v56 (26-30 to 31-37), and also restores original gold bar spawn chance.");
            artificeInteriors = configFile.Bind(
                "Moon.Artifice",
                "Adjust Interiors",
                true,
                "Adjusts interior chances to make manor dominant again. (15%/35%/50% -> 15%/50%/35%) Also increases the size of manors and factories from 1.8x to 2.0x, but mineshafts are not affected.");
            artificeTurrets = configFile.Bind(
                "Moon.Artifice",
                "Increase Turrets",
                true,
                "Drastically increase the spawn rate of turrets, like in the v50 betas.");

            // Embrion
            embrionBuffScrap = configFile.Bind(
                "Moon.Embrion",
                "Buff Scrap",
                true,
                "Significantly improves the quality of scrap found on Embrion.");
            embrionMineshafts = configFile.Bind(
                "Moon.Embrion",
                "Increase Mineshafts",
                true,
                "Significantly increase the chance of mineshafts on Embrion.");
            embrionWeeds = configFile.Bind(
                "Moon.Embrion",
                "No Vain Shrouds",
                true,
                "Disable vain shroud growth on Embrion, since there is limited biological life on the surface.");
            embrionAdjustEnemies = configFile.Bind(
                "Moon.Embrion",
                "Adjust Indoor Enemies",
                true,
                "Increase the spawn rates of non-biological enemies in the interior.");
            embrionMega = configFile.Bind(
                "Moon.Embrion",
                "Bigger on the Inside",
                false,
                "Double the interior size, dramatically reduce the chance of factories, and greatly increase the number of scrap items.");
        }

        static void MiscConfig()
        {
            multiplayerWeather = configFile.Bind(
                "Misc",
                "Multiplayer Weather Multiplier",
                true,
                "Re-enable some unused code in vanilla to increase the frequency of random weather in multiplayer.");
            foggyLimit = configFile.Bind(
                "Misc",
                "Nerf Foggy Weather",
                true,
                "Reduce the maximum intensity of foggy weather.");
            randomIndoorFog = configFile.Bind(
                "Misc",
                "Random Indoor Fog",
                true,
                "When the indoor fog event occurs, its density will be randomized between the vanilla value and a much less extreme value.");
            shrinkMineshafts = configFile.Bind(
                "Misc",
                "Shrink Mineshafts",
                true,
                "Slightly reduce the overall size of mineshaft interiors.");
        }

        static void InfestationConfig()
        {
            infestationRework = configFile.Bind(
                "Infestations",
                "Rework Mechanics",
                true,
                "Infestations no longer override a moon's power level, and enemy spawn chances are no longer equalized. The \"infestation enemy\" takes up no power level during the event, and \"bonus spawns\" only occur until the infestation enemy hits their spawn cap.");

            infestationButlers = configFile.Bind(
                "Infestations",
                "Butler Infestations",
                true,
                "Allow butlers to be selected as the subject of an infestation.");

            infestationMasked = configFile.Bind(
                "Infestations",
                "Masked Infestations",
                true,
                "Allow \"masked\" to be selected as the subject of an infestation.");

            infestationBarbers = configFile.Bind(
                "Infestations",
                "Barber Infestations",
                true,
                "Allow Barbers to be selected as the subject of an infestation. Other enemy spawns will be disabled. Won't occur inside mineshafts.");
        }

        static void MigrateLegacyConfig()
        {
            if (foggyLimit.Value)
            {
                if (!configFile.Bind("Misc", "Rework Foggy Weather", true, "Legacy setting, doesn't work").Value)
                    foggyLimit.Value = false;

                configFile.Remove(configFile["Misc", "Rework Foggy Weather"].Definition);
            }
        }
    }
}

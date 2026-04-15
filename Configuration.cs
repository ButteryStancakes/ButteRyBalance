using BepInEx.Configuration;
//using ButteRyBalance.Network;

namespace ButteRyBalance
{
    internal class Configuration
    {
        internal enum DineScrap
        {
            DontChange,
            Consolidate,
            Rollback
        }

        internal enum SnowmanFrequency
        {
            None,
            Rare,
            Lots
        }

        static ConfigFile configFile;

        internal static ConfigEntry<bool> coilheadStunReset, jesterWalkThrough, butlerManorChance, butlerStealthStab, butlerLongCooldown, jesterLongCooldown, butlerKnifePrice, knifeShortCooldown, knifeAutoSwing, maneaterLimitGrowth, maneaterWideTurns, maneaterScrapGrowth, moonsKillSwitch, dineReduceButlers, barberDynamicSpawns, foggyLimit, experimentationNoEvents, experimentationNoGiants, experimentationNoEggs, /*experimentationNoNuts,*/ experimentationBuffScrap, randomIndoorFog, assuranceNerfScrap, assuranceMasked, /*vowAdjustScrap,*/ vowNoCoils, vowMineshafts, /*shrinkMineshafts,*/ offenseBuffScrap, /*offenseMineshafts,*/ offenseMasked, offenseNerfEclipse, vowNoTraps, /*marchShrink,*/ marchBuffScrap, /*marchRainy, multiplayerWeather,*/ butlerSquishy, adamanceBuffScrap, /*adamanceReduceChaos,*/ coilheadCurves, /*rendMineshafts, rendShrink, rendAdjustIndoor, rendAdjustScrap,*/ rendWorms, metalSheetPrice, coilheadPower, /*dineAdjustIndoor, dineBuffScrap,*/ dineAdjustOutdoor, /*dineAdjustCurves,*/ titanBuffScrap, titanAddGold, /*titanMineshafts,*/ titanAdjustEnemies, /*titanWeeds, dineMasked,*/ giantSnowSight, /*giantForgetTargets,*/ dineFloods, robotFog, nutcrackerGunPrice, nutcrackerKevlar, jetpackBattery, jetpackReduceDiscount, /*tzpExpandCapacity,*/ jetpackInertia, artificeBuffScrap, artificeInteriors, artificeTurrets, zapGunPrice, radarBoosterPrice, stunGrenadePrice, scrapAdjustWeights, maneaterPower, /*embrionMineshafts, embrionBuffScrap, embrionWeeds, embrionAdjustEnemies,*/ embrionMega, infestationRework, infestationButlers, infestationMasked, infestationBarbers, /*foxSquishy,*/ zapGunBattery, offenseBees, apparatusPrice, /*robotRider, jetpackShortCircuit,*/ spikeTrapDistance, infestationThumpers, coilheadPersistence, giantSquishy, hoarderAngerManagement, infestationSnareFlea, infestationCoilhead, foxSlender, adamanceNerfEclipse, /*adamanceReduceCadavers,*/ cadaversPower, pufferPower, spikeTrapMineshaft, jetpackUtility, infestationGunkfish, adamanceInteriors, adamanceNoMasks, offenseNerfTraps, assuranceGiants, dineMineshafts, proFlashlightPrice;
        internal static ConfigEntry<DineScrap> dineScrapPool;
        internal static ConfigEntry<SnowmanFrequency> rendSnowmen, dineSnowmen, titanSnowmen;

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
                false,
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
            jetpackUtility = configFile.Bind(
                "Item.Jetpack",
                "No Utility Belt",
                true,
                "Blacklists the jetpack from the utility belt, like shovels. This means you can only fly 3 scraps at a time like before.");
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
            // Pro-flashlight
            proFlashlightPrice = configFile.Bind(
                "Item.ProFlashlight",
                "Nerf Price",
                true,
                "Increases the cost of pro-flashlights from $28 to $32, like the first v80 beta.");
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
                false,
                "Increases the average sell value of metal sheets.");
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
                false,
                "Butlers will deal damage slower, as long as they haven't been attacked before. In singleplayer, this setting will also increase their HP from 2 to 3.");
            butlerSquishy = configFile.Bind(
                "Enemy.Butler",
                "Squishy",
                true,
                "Butlers take bonus damage from shotgun shots and explosions, allowing those to kill in one hit.");
            butlerKnifePrice = configFile.Bind(
                "Enemy.Butler",
                "Randomize Knife Price",
                false,
                "Restores the kitchen knife's price randomization. On average, it will be significantly more valuable than $35, the vanilla price.");

            // Cadaver Growths
            cadaversPower = configFile.Bind(
                "Enemy.CadaverGrowths",
                "Increase Power Level",
                true,
                "Increase Cadaver Growths' power level from 2 to 4, to keep the day's threats more solitarily focused.");

            // Coil-head
            coilheadStunReset = configFile.Bind(
                "Enemy.Coilhead",
                "Stuns Reset",
                true,
                "Coil-heads will begin \"recharging\" when they are stunned by stun grenades, radar boosters, or homemade flashbangs.");
            coilheadCurves = configFile.Bind(
                "Enemy.Coilhead",
                "Adjust Spawn Curves",
                false,
                "Adds two multipliers to Coil-head spawn rates; one reduces their chances earlier in the day, and the other reduces the chance of multiple spawning in one day.");
            coilheadPower = configFile.Bind(
                "Enemy.Coilhead",
                "Increase Power Level",
                true,
                "Increase Coil-heads' power level from 1 to 2, to reduce the potential for forming bad enemy combos.");
            coilheadPersistence = configFile.Bind(
                "Enemy.Coilhead",
                "Persistence",
                false,
                "Coil-heads no longer recharge from just chasing players, behaving akin to their pre-v60 iteration.");

            // Forest Keeper
            giantSnowSight = configFile.Bind(
                "Enemy.ForestKeeper",
                "Treat Blizzard Like Fog",
                true,
                "On snowy moons, Forest Keepers will have their line-of-sight range reduced, the same way as in foggy weather.");
            giantSquishy = configFile.Bind(
                "Enemy.ForestKeeper",
                "Squishy",
                true,
                "Forest Keepers are instantly killed if the Cruiser is rammed into them at high speeds, just like before v70.");

            // Hoarding bug
            hoarderAngerManagement = configFile.Bind(
                "Enemy.HoardingBug",
                "Anger Management",
                true,
                "Hoarding bug \"annoyance\" (from player proximity) will reset when they enter chase, which makes them less likely to get stuck in an aggro loop on players who don't steal from them.");

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
                "Slightly increase the average time before a Jester winds, in multiplayer. Only works in games with 4 players or less.");

            // Kidnapper fox
            foxSlender = configFile.Bind(
                "Enemy.KidnapperFox",
                "No Roadkill",
                true,
                "Make the kidnapper fox immune to the Company Cruiser. This is already the case in vanilla, but CruiserImproved fixes the bug causing it, which pseudo-nerfs the fox");

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
                false,
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
                false,
                "Restores randomization for the shotgun's price.");
            nutcrackerKevlar = configFile.Bind(
                "Enemy.Nutcracker",
                "Last Stand",
                false,
                "Nutcrackers will resist instant death from a shotgun blast if they are at full health, immediately entering \"berserk\" state.");

            // Old Bird
            robotFog = configFile.Bind(
                "Enemy.OldBird",
                "See Through Fog",
                true,
                "Old Birds can see through fog with their powerful searchlights.");

            // Spore lizard
            pufferPower = configFile.Bind(
                "Enemy.SporeLizard",
                "Decrease Power Level",
                true,
                "Decrease spore lizard's power level from 1 to 0.5, as like the Backwater Gunkfish, it is mostly passive to players and only incidentally dangerous.");
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
                "Swaps several items' rarities with the corresponding values from Offense's loot pool.");
            assuranceMasked = configFile.Bind(
                "Moon.Assurance",
                "Spawn Masked",
                true,
                "Allow \"Masked\" enemies to spawn very rarely on Assurance, since Comedy and Tragedy spawn there.");
            assuranceGiants = configFile.Bind(
                "Moon.Assurance",
                "More Forest Giants",
                true,
                "Swaps Assurance's forest giant spawn chance with Offense's chances. This will also reduce forest giants on Offense.");

            // Vow
            vowMineshafts = configFile.Bind(
                "Moon.Vow",
                "Mostly Mineshafts",
                true,
                "Significantly reduce the likelihood of factory interiors on Vow. Fewer \"mineshaft bonus items\" will spawn, and outside spawns will be slightly increased, to compensate for increased profits.");
            vowNoCoils = configFile.Bind(
                "Moon.Vow",
                "No Coil-heads",
                true,
                "Coil-heads no longer have a random chance to spawn on Vow.");
            vowNoTraps = configFile.Bind(
                "Moon.Vow",
                "No Traps",
                false,
                "Removes landmines and turrets from Vow, like in v9.");

            // Offense
            offenseBuffScrap = configFile.Bind(
                "Moon.Offense",
                "Buff Scrap",
                true,
                "Swaps several items' rarities with the corresponding values from Assurance's loot pool. Also adds gold bars as a very rare spawn.");
            offenseBees = configFile.Bind(
                "Moon.Offense",
                "Circuit Bees",
                false,
                "Allow circuit bees to spawn on Offense, like Assurance.");
            offenseMasked = configFile.Bind(
                "Moon.Offense",
                "Spawn Masked",
                true,
                "Allow \"Masked\" enemies to spawn rarely on Offense, since Comedy spawns there - these rare spawns will completely replace brackens.");
            offenseNerfEclipse = configFile.Bind(
                "Moon.Offense",
                "Nerf Eclipse",
                false,
                "Reduces enemy spawns per \"wave\" during eclipses on Offense from 4 to 3.");
            offenseNerfTraps = configFile.Bind(
                "Moon.Offense",
                "Reduce Traps",
                true,
                "Reduces trap spawns (landmines and turrets) to pre-v80 spawn rates.");

            // March
            marchBuffScrap = configFile.Bind(
                "Moon.March",
                "Buff Scrap",
                false,
                "Slightly increase the amount and quality of scrap that spawns on March, to bring it a little closer with Adamance.");

            // Adamance
            /*adamanceReduceCadavers = configFile.Bind(
                "Moon.Adamance",
                "Reduce Cadavers",
                true,
                "Cadavers are no longer guaranteed to appear when the ship lands.");*/
            adamanceBuffScrap = configFile.Bind(
                "Moon.Adamance",
                "Buff Scrap",
                false,
                "Increases the amount of scrap that spawns on Adamance, like before v80. (14-16 -> 16-18)");
            adamanceNerfEclipse = configFile.Bind(
                "Moon.Adamance",
                "Nerf Eclipse",
                false,
                "Reduces enemy spawns per \"wave\" during eclipses on Adamance from 3 to 2.");
            adamanceInteriors = configFile.Bind(
                "Moon.Adamance",
                "Adjust Interiors",
                false,
                "Increases mineshaft chance (like before v80) and also slightly increases manor chance (like v50 beta). Also adds butlers back to the spawn pool, as a bonus.");
            adamanceNoMasks = configFile.Bind(
                "Moon.Adamance",
                "No Masked",
                true,
                "\"Masked\" no longer have a random chance to spawn on Adamance.");

            // Rend
            rendWorms = configFile.Bind(
                "Moon.Rend",
                "Restore Earth Leviathans",
                false,
                "Allow Earth Leviathans, which were removed from Rend in v56, to spawn again.");
            rendSnowmen = configFile.Bind(
                "Moon.Rend",
                "Snowmen",
                SnowmanFrequency.Rare,
                "Allow snowmen, which were removed in v70, to spawn again.");

            // Dine
            dineScrapPool = configFile.Bind(
                "Moon.Dine",
                "Scrap Pool",
                DineScrap.Consolidate,
                "What sort of scrap should spawn on Dine?\n\"DontChange\" will avoid making any changes, letting vanilla or other mods take priority.\n\"Consolidate\" will use V73+'s spawn pool, but 40% as many items will spawn with 1.75x value each.\n\"Rollback\" will revert the scrap pool to what it was in ButteRyBalance before v73.");
            dineMineshafts = configFile.Bind(
                "Moon.Dine",
                "Dineshaft",
                true,
                "Increase the chance of mineshafts again, much like it was before v80.");
            dineReduceButlers = configFile.Bind(
                "Moon.Dine",
                "Reduce Butler Chance",
                true,
                "Reduces the spawn weight of Butlers on Dine. Intended to be combined with the \"Manor Increased Chance\" setting for Butlers.");
            dineAdjustOutdoor = configFile.Bind(
                "Moon.Dine",
                "Adjust Outdoor Enemies",
                true,
                "Decreases spawn chance for giants and increases spawn chance for Old Birds, leading to more varied outdoor gameplay.");
            dineFloods = configFile.Bind(
                "Moon.Dine",
                "Fix Floods",
                true,
                "Reduces the water level at the start of flooded weather days on Dine, so that the main entrance no longer starts out underwater.");
            dineSnowmen = configFile.Bind(
                "Moon.Dine",
                "Snowmen",
                SnowmanFrequency.Rare,
                "Allow snowmen, which were removed in v70, to spawn again.");

            // Titan
            titanBuffScrap = configFile.Bind(
                "Moon.Titan",
                "Buff Scrap",
                true,
                "Increases scrap counts on Titan from 28-31 to 28-35, like in v50 betas.");
            titanAddGold = configFile.Bind(
                "Moon.Titan",
                "Gold Rush",
                false,
                "Adds gold bars to Titan's loot pool as a rare find.");
            titanAdjustEnemies = configFile.Bind(
                "Moon.Titan",
                "Adjust Enemies",
                true,
                "Decreases giants and increases Old Birds. Also slightly adjusts interior spawns to reduce the frequency of the Jester.");
            titanSnowmen = configFile.Bind(
                "Moon.Titan",
                "Snowmen",
                SnowmanFrequency.Rare,
                "Allow snowmen, which were removed in v70, to spawn again.");

            // Artifice
            artificeBuffScrap = configFile.Bind(
                "Moon.Artifice",
                "Buff Scrap",
                true,
                "Restores Artifice's scrap counts from v56 (26-30 to 31-37), and also restores original gold bar spawn chance.");
            artificeInteriors = configFile.Bind(
                "Moon.Artifice",
                "Adjust Interiors",
                false,
                "Adjusts interior chances to make manor dominant again. (15%/35%/50% -> 15%/50%/35%) Also increases the size of manors and factories from 1.8x to 2.0x, but mineshafts are not affected.");
            artificeTurrets = configFile.Bind(
                "Moon.Artifice",
                "Increase Turrets",
                true,
                "Drastically increase the spawn rate of turrets, like in the v50 betas.");

            // Embrion
            embrionMega = configFile.Bind(
                "Moon.Embrion",
                "Bigger on the Inside",
                false,
                "Double the interior size, dramatically increase the chance of mineshafts, and greatly increase the number of scrap items.");
        }

        static void MiscConfig()
        {
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

            spikeTrapDistance = configFile.Bind(
                "Misc",
                "Safely Distance Spike Traps",
                true,
                "Spike traps are no longer allowed to spawn directly on top of building entrances.");
            spikeTrapMineshaft = configFile.Bind(
                "Misc",
                "Mineshaft Spike Traps",
                false,
                "Spike traps will be allowed in mineshaft interiors again, like before v80. It is *strongly recommended* to use \"Safely Distance Spike Traps\" in conjunction with this setting, or else spike traps will be allowed to spawn on top of the elevator!");
        }

        static void InfestationConfig()
        {
            infestationRework = configFile.Bind(
                "Infestations",
                "Rework Mechanics",
                true,
                "(REQUIRES SPAWN CYCLE FIXES!) Infestations no longer override a moon's power level, and enemy spawn chances are no longer altered. The \"infestation enemy\" takes up no power level during the event, and \"bonus spawns\" only occur until the infestation enemy hits their spawn cap.");

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
                false,
                "Allow Barbers to be selected as the subject of an infestation. Other enemy spawns will be disabled. Won't occur inside mineshafts.");

            infestationThumpers = configFile.Bind(
                "Infestations",
                "Thumper Infestations",
                false,
                "Allow thumpers to be selected as the subject of an infestation.");

            infestationSnareFlea = configFile.Bind(
                "Infestations",
                "Snare Flea Infestations",
                false,
                "Allow snare fleas to be selected as the subject of an infestation.");

            infestationCoilhead = configFile.Bind(
                "Infestations",
                "Coil-head Infestations",
                false,
                "Allow coil-heads to be selected as the subject of an infestation.");

            infestationGunkfish = configFile.Bind(
                "Infestations",
                "Backwater Gunkfish Infestations",
                false,
                "Allow Backwater Gunkfish to be selected as the subject of an infestation.");
        }

        static void MigrateLegacyConfig()
        {
            if (foggyLimit.Value)
            {
                if (!configFile.Bind("Misc", "Rework Foggy Weather", true, "Legacy setting, doesn't work").Value)
                    foggyLimit.Value = false;

                configFile.Remove(configFile["Misc", "Rework Foggy Weather"].Definition);
            }
            if (!adamanceNerfEclipse.Value)
            {
                if (configFile.Bind("Moon.Adamance", "Reduce Chaos", false, "Legacy setting, doesn't work").Value)
                    adamanceNerfEclipse.Value = true;

                configFile.Remove(configFile["Moon.Adamance", "Reduce Chaos"].Definition);
            }

            foreach ((string, string) oldKey in new (string, string)[]
            {
                ("Enemy.ForestKeeper", "Forget Out-of-Sight Players"),
                ("Item.TZPInhalant", "Expand Capacity"),
                ("Moon.Dine", "Buff Scrap"),
                ("Moon.Titan", "No Vain Shrouds"),
                ("Moon.Embrion", "No Vain Shrouds"),
                ("Enemy.OldBird", "Restore Mech Riding"),
                ("Moon.March", "Shrink Interior"),
                ("Moon.March", "Always Rainy"),
                ("Misc", "Multiplayer Weather Multiplier"),
                ("Moon.Offense", "Mostly Mineshafts"),
                ("Misc", "Shrink Mineshafts"),
                ("Moon.Vow", "Adjust Scrap"),
                ("Moon.Experimentation", "No Nutcrackers"),
                ("Moon.Rend", "Rare Mineshafts"),
                ("Moon.Rend", "Shrink Interior"),
                ("Moon.Rend", "Adjust Scrap"),
                ("Moon.Dine", "Adjust Indoor Enemies"),
                ("Moon.Dine", "Adjust Spawn Curves"),
                ("Moon.Dine", "Add Masked"),
                ("Moon.Embrion", "Buff Scrap"),
                ("Moon.Embrion", "Increase Enemies"),
                ("Moon.Embrion", "Adjust Indoor Enemies"),
                ("Moon.Rend", "Adjust Indoor Enemies"),
                ("Enemy.KidnapperFox", "Squishy"),
                ("Moon.Titan", "Reduce Mineshafts"),
            })
            {
                configFile.Bind(oldKey.Item1, oldKey.Item2, string.Empty, "Legacy setting, doesn't work");
                configFile.Remove(configFile[oldKey.Item1, oldKey.Item2].Definition);
            }

            configFile.Save();
        }
    }
}

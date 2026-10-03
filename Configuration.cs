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

        internal enum JetpackControls
        {
            Vanilla,
            V49,
            Dynamic
        }

        internal const int MIN_PRICE = 10, MAX_PRICE = 250000;

        static ConfigFile configFile;

        internal static ConfigEntry<bool> coilheadStunReset, jesterWalkThrough, butlerManorChance, butlerStealthStab, butlerLongCooldown, jesterLongCooldown, butlerKnifePrice, knifeShortCooldown, knifeAutoSwing, maneaterLimitGrowth, maneaterWideTurns, /*maneaterScrapGrowth,*/ moonsKillSwitch, /*dineReduceButlers,*/ barberDynamicSpawns, /*foggyLimit,*/ experimentationNoEvents, /*experimentationNoGiants, experimentationNoEggs, experimentationNoNuts,*/ experimentationBuffScrap, /*randomIndoorFog,*/ assuranceNerfScrap, assuranceMasked, /*vowAdjustScrap,*/ vowNoCoils, vowMineshafts, /*shrinkMineshafts,*/ offenseBuffScrap, /*offenseMineshafts,*/ offenseMasked, /*offenseNerfEclipse, vowNoTraps, marchShrink, marchBuffScrap, marchRainy, multiplayerWeather,*/ butlerSquishy, adamanceBuffScrap, /*adamanceReduceChaos, coilheadCurves, rendMineshafts,*/ rendShrink, /*rendAdjustIndoor, rendAdjustScrap, rendWorms, metalSheetPrice,*/ coilheadPower, /*dineAdjustIndoor, dineBuffScrap, dineAdjustOutdoor,*/ /*dineAdjustCurves,*/ titanBuffScrap, /*titanAddGold, titanMineshafts,*/ titanAdjustEnemies, titanWeeds, /*dineMasked,*/ giantSnowSight, /*giantForgetTargets,*/ dineFloods, /*robotFog,*/ nutcrackerGunPrice, nutcrackerKevlar, jetpackBattery, /*jetpackReduceDiscount, tzpExpandCapacity, jetpackInertia,*/ artificeBuffScrap, artificeInteriors, artificeTurrets, zapGunPrice, /*radarBoosterPrice, stunGrenadePrice, scrapAdjustWeights,*/ maneaterPower, /*embrionMineshafts, embrionBuffScrap,*/ embrionWeeds, /*embrionAdjustEnemies,*/ embrionMega, infestationRework, infestationButlers, infestationMasked, infestationBarbers, /*foxSquishy,*/ zapGunBattery, offenseBees, apparatusPrice, /*robotRider, jetpackShortCircuit,*/ spikeTrapDistance, infestationThumpers, coilheadPersistence, giantSquishy, hoarderAngerManagement, infestationSnareFlea, infestationCoilhead, /*foxSlender, adamanceNerfEclipse,*/ adamanceReduceCadavers, cadaversPower, pufferPower, spikeTrapMineshaft, jetpackUtility, infestationGunkfish, adamanceInteriors, adamanceNoMasks, offenseNerfTraps, assuranceGiants, dineMineshafts, proFlashlightPrice, vowMisty, nerfNightVision, /*marchAdjustEnemies, gunkfishSquishy,*/ shovelBuffer, stunLonger, maneaterTarget, cadaverTarget, cavernsNoKeys, jetpackWarmUp, cruiserItemSafety, cruiserExhaust, cruiserRegen, cruiserTrees, cruiserEnemyDamage, cruiserCrashDamage, /*weaponsAdjustWeights,*/ butlerNoSearch, offenseFireExits, dineFireExits, proportionalFireExits, cruiserPatchEnemies, cavernsNoTurrets, cadaversLimitGrowth, cruiserAutoHeal, cruiserStabilize, dineAdjustEnemies, girlBreakers, vainsShrink;
        internal static ConfigEntry<DineScrap> dineScrapPool;
        internal static ConfigEntry<SnowmanFrequency> rendSnowmen, dineSnowmen, titanSnowmen;
        internal static ConfigEntry<int> cruiserPrice, jetpackPrice, weedKillerDamage, cruiserTurbos, vainsIterations;
        internal static ConfigEntry<JetpackControls> jetpackControls;
        internal static ConfigEntry<float> vainsChanceSame, vainsChanceSameEarly, vainsChanceOther, vainsChanceRare;

        internal static void Init(ConfigFile cfg)
        {
            configFile = cfg;

            MiscConfig();
            InfestationConfig();
            EnemyConfig();
            ItemConfig();
            VehicleConfig();
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
            // Apparatus
            apparatusPrice = configFile.Bind(
                "Item.Apparatus",
                "Randomize Price",
                false,
                "Randomizes the price of the apparatus once it has been unplugged. ($80 -> $40-130)");
            // Jetpack
            jetpackBattery = configFile.Bind(
                "Item.Jetpack",
                "Reduce Battery",
                true,
                "Reduces jetpack battery back to 40s (from 50s), like the v50 beta.");
            jetpackPrice = configFile.Bind(
                "Item.Jetpack",
                "Price",
                900,
                new ConfigDescription(
                    "Alters the price of the Jetpack. $700 is the original price from launch. $900 is the current vanilla price.",
                    new AcceptableValueRange<int>(MIN_PRICE, MAX_PRICE)));
            jetpackControls = configFile.Bind(
                "Item.Jetpack",
                "Control Scheme",
                JetpackControls.V49,
                "Controls how the jetpacks' speed and handling are set.\n\"Vanilla\" makes no changes.\n\"V49\" restores the classic control scheme, with much stronger inertia, meaning you must pre-emptively adjust your momentum.\n\"Dynamic\" adjusts speed and handling based on your carried weight (light weight = high speed, poor handling; heavy weight = low speed, smooth handling)");
            jetpackWarmUp = configFile.Bind(
                "Item.Jetpack",
                "Warmup Period",
                false,
                "When first activated, jetpacks take a brief period of time to warm up to maximum thrust power, leaving you more vulnerable to enemies.");
            jetpackUtility = configFile.Bind(
                "Item.Jetpack",
                "No Utility Belt",
                true,
                "Blacklists the jetpack from the utility belt, like shovels. This means you can only fly 3 scraps at a time like before.");
            // Kitchen knife
            knifeShortCooldown = configFile.Bind(
                "Item.KitchenKnife",
                "Short Cooldown",
                false,
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
            // Shovel
            shovelBuffer = configFile.Bind(
                "Item.Shovel",
                "Input Buffer",
                true,
                "(Client-side) Clicking the attack button shortly before the shovel is readied will \"buffer\" your input, winding up as soon as the shovel finishes its swing.");
            // Stun grenade
            stunLonger = configFile.Bind(
                "Item.StunGrenade",
                "Stun Longer",
                true,
                "Increases the effectiveness of stun grenades against certain enemies. (Like hoarding bugs and thumpers)");
            // Weed killer
            weedKillerDamage = configFile.Bind(
                "Item.WeedKiller",
                "Player Damage",
                7,
                new ConfigDescription(
                    "How much damage is dealt to players that are sprayed with weed killer while infected by Cadavers. In vanilla, this was 7 in v80, and is 8 in v81+.",
                    new AcceptableValueRange<int>(3, 10)));
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

        static void VehicleConfig()
        {
            // Cruiser
            cruiserPrice = configFile.Bind(
                "Vehicle.Cruiser",
                "Price",
                700,
                new ConfigDescription(
                    "Alters the price of the Cruiser. $400 is the original price from v55 launch. $370 is the current vanilla price.",
                    new AcceptableValueRange<int>(MIN_PRICE, MAX_PRICE)));
            cruiserTurbos = configFile.Bind(
                "Vehicle.Cruiser",
                "Turbo Boosts",
                5,
                new ConfigDescription(
                    "How many boosts can the Cruiser use at maximum capacity? When set to 0, weed killer does nothing once the car is fully repaired.",
                    new AcceptableValueRange<int>(0, 5)));
            cruiserRegen = configFile.Bind(
                "Vehicle.Cruiser",
                "Reduced Regeneration",
                true,
                "Sharply decreases HP regen for the Cruiser, which makes weed killer more valuable as the only source of mid-round maintenance.");
            cruiserTrees = configFile.Bind(
                "Vehicle.Cruiser",
                "Nature Prevails",
                true,
                "The cruiser takes damage from destroying trees again. Does *not* affect snowmen.");
            cruiserEnemyDamage = configFile.Bind(
                "Vehicle.Cruiser",
                "Increase Enemy Damage",
                true,
                "The cruiser takes increased damage from collision with enemies. *Does not apply* when the Cruiser is left unattended, parked and with the engine off.");
            cruiserCrashDamage = configFile.Bind(
                "Vehicle.Cruiser",
                "Increase Crash Damage",
                true,
                "The cruiser takes increased damage from collision with terrain or walls.");
            cruiserPatchEnemies = configFile.Bind(
                "Vehicle.Cruiser",
                "Patch Enemy Protection",
                true,
                "Corrects several interactions where you are erroneously invincible to enemies while inside the Cruiser.\nFor example, this fixes the bug preventing giants from grabbing players off the roof of the car or out of the seats, any time the back door is closed.\nOld Birds and baboon hawks will also be able to see players sitting in the front seats.");
            cruiserItemSafety = configFile.Bind(
                "Vehicle.Cruiser",
                "No Space Insurance",
                true,
                "When returning to orbit, items on top of the Cruiser, or in the back while the door is open, are lost.");
            cruiserExhaust = configFile.Bind(
                "Vehicle.Cruiser",
                "Rocking Costs Stamina",
                true,
                "When \"rocking\" the car (with the jump button), stamina will be deducted, and you are unable to rock the car when out of stamina. The stamina cost gets steeper the more the car is facing upward.");
            cruiserAutoHeal = configFile.Bind(
                "Vehicle.Cruiser",
                "Heal in Orbit",
                true,
                "When returning to orbit, the Cruiser's durability is completely restored. This is vanilla behavior; when disabled, you must use weed killer to restore durability to maximum.");
            cruiserStabilize = configFile.Bind(
                "Vehicle.Cruiser",
                "Stabilize",
                true,
                "The Cruiser will automatically try to adjust itself to an upright position when starting to tip over. (This mechanic did not exist prior to v70.)");
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
                "When triggering a Butler to attack by bumping into them (rare chance), they will no longer \"berserk\", unless the offending player is alone.");
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
                false,
                "Restores the kitchen knife's price randomization. ($35 -> $28-84)");
            butlerNoSearch = configFile.Bind(
                "Enemy.Butler",
                "No Search in Solo",
                true,
                "Disables the Butler's \"mad search\" behavior in singleplayer, where they put away their broom and sprint around the interior looking for players. This should make it somewhat easier to stay out of their way without them forcing an interaction.");

            // Cadaver Bloom
            cadaverTarget = configFile.Bind(
                "Enemy.CadaverBloom",
                "Infighting",
                true,
                "Allows Cadaver Blooms to be targeted by other enemies after bursting outside the building. (Baboon hawks, Old Birds, giant sapsucker)");

            // Cadaver Growths
            cadaversPower = configFile.Bind(
                "Enemy.CadaverGrowths",
                "Increase Power Level",
                true,
                "Increase Cadaver Growths' power level from 2 to 4, to keep the day's threats more solitarily focused.");
            cadaversLimitGrowth = configFile.Bind(
                "Enemy.CadaverGrowths",
                "Limit Growth",
                true,
                "Cadaver Growths' rate of spread now scales based on how late in the day it spawns; immediately after spawning, there will always be a grace period where growth is extremely slow, but it will ramp faster when starting later in the day.");

            // Coil-head
            coilheadStunReset = configFile.Bind(
                "Enemy.Coilhead",
                "Stuns Reset",
                true,
                "Coil-heads will begin \"recharging\" when they are stunned by stun grenades, radar boosters, or homemade flashbangs.");
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

            // Girl
            girlBreakers = configFile.Bind(
                "Enemy.Girl",
                "Flip Breakers",
                true,
                "Fixes the wrong RPC being called when the little girl enters chases with a player. This restores her 65% chance to flip the lights off by messing up the breaker box - normally (in vanilla, or with a 35% chance) the lights just temporarily flicker.");

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
                "Sharply limits (or in some cases, completely prevents) the Maneater's ability to transform before it has been encountered by a player.");
            maneaterWideTurns = configFile.Bind(
                "Enemy.Maneater",
                "Limit Turn Speed",
                true,
                "Adult Maneaters will have far worse turning speed when lunging, as long as they are inside the building.");
            maneaterTarget = configFile.Bind(
                "Enemy.Maneater",
                "Infighting",
                true,
                "Allows Maneaters to be targeted by other enemies after transforming into an adult outside the building. (Baboon hawks, Old Birds, giant sapsucker)");

            // Nutcracker
            nutcrackerGunPrice = configFile.Bind(
                "Enemy.Nutcracker",
                "Randomize Shotgun Price",
                false,
                "Enables randomization for the shotgun's price. ($60 -> $25-90)");
            nutcrackerKevlar = configFile.Bind(
                "Enemy.Nutcracker",
                "Last Stand",
                false,
                "Nutcrackers will resist instant death from a shotgun blast if they are at full health, immediately entering \"berserk\" state.");

            // Vain Shrouds
            vainsChanceSame = configFile.Bind(
                "Enemy.VainShrouds",
                "Spawn Chance (Same Moon)",
                4.5f,
                new ConfigDescription(
                    "The chance for vain shrouds to start growing on a moon after leaving it. Only applies on quota 4 and later.",
                    new AcceptableValueRange<float>(0f, 100f)));
            vainsChanceSameEarly = configFile.Bind(
                "Enemy.VainShrouds",
                "Spawn Chance (Same Moon, Early)",
                3f,
                new ConfigDescription(
                    "The chance for vain shrouds to start growing on a moon after leaving it. Only applies on quotas 1-3.",
                    new AcceptableValueRange<float>(0f, 100f)));
            vainsChanceRare = configFile.Bind(
                "Enemy.VainShrouds",
                "Spawn Chance (Other Moons, Rare)",
                1.5f,
                new ConfigDescription(
                    "The chance for vain shrouds to start growing each day on purchasable moons (Rend, Titan, or before quota 4, Artifice) when leaving any other moon.",
                    new AcceptableValueRange<float>(0f, 100f)));
            vainsChanceOther = configFile.Bind(
                "Enemy.VainShrouds",
                "Spawn Chance (Other Moons)",
                2.2f,
                new ConfigDescription(
                    "The chance for vain shrouds to start growing on a moon each day if none of the other settings apply.",
                    new AcceptableValueRange<float>(0f, 100f)));
            vainsIterations = configFile.Bind(
                "Enemy.VainShrouds",
                "Maximum Iterations",
                35,
                new ConfigDescription(
                    "How many iterations of growth can occur for vain shrouds total? (2-3 per day on average)\nBefore v80, this value was set to 20.",
                    new AcceptableValueRange<int>(0, 35)));
            vainsShrink = configFile.Bind(
                "Enemy.VainShrouds",
                "Flimsy",
                false,
                "Weed killer is about 30% more efficient at shrinking Vain Shrouds.");

            // Spore lizard
            pufferPower = configFile.Bind(
                "Enemy.SporeLizard",
                "Decrease Power Level",
                false,
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
                false,
                "Increases the amount of scrap that spawns on Experimentation, like in v9. (8-11 -> 11-15) Easter eggs no longer appear in the loot pool.");
            experimentationNoEvents = configFile.Bind(
                "Moon.Experimentation",
                "No Random Events",
                true,
                "Disables all random events on Experimentation. (Meteor showers, infestations, spooky fog) Also prevents Forest Keepers from spawning.");

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
                "More Forest Keepers",
                true,
                "Swaps Assurance's Forest Keeper spawn chance with Offense's chances. This will also reduce Forest Keepers on Offense.");

            // Vow
            vowMineshafts = configFile.Bind(
                "Moon.Vow",
                "Mostly Mineshafts",
                true,
                "Factory interiors will not appear on Vow, like in v60. To compensate for mineshaft bonus, base item count will be slightly reduced and outdoor spawns slightly increased.");
            vowNoCoils = configFile.Bind(
                "Moon.Vow",
                "No Coil-heads",
                true,
                "Coil-heads no longer have a random chance to spawn on Vow.");
            vowMisty = configFile.Bind(
                "Moon.Vow",
                "Misty",
                true,
                "Makes Vow's natural fog much denser, like it was before v72.");

            // March

            // Adamance
            adamanceReduceCadavers = configFile.Bind(
                "Moon.Adamance",
                "Reduce Cadavers",
                true,
                "Cadavers are no longer guaranteed to appear when the ship lands. This greatly increases the spawn rate of other enemies to compensate.");
            adamanceBuffScrap = configFile.Bind(
                "Moon.Adamance",
                "Buff Scrap",
                true,
                "Increases the amount of scrap that spawns on Adamance, like before v80. (14-16 -> 16-18)");
            adamanceInteriors = configFile.Bind(
                "Moon.Adamance",
                "Adjust Interiors",
                true,
                "Increases mineshaft chance (like before v80) and also slightly increases manor chance (like v50 beta). Also adds butlers back to the spawn pool, as a bonus.");
            adamanceNoMasks = configFile.Bind(
                "Moon.Adamance",
                "No Masked",
                true,
                "\"Masked\" no longer have a random chance to spawn on Adamance.");

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
            offenseNerfTraps = configFile.Bind(
                "Moon.Offense",
                "Reduce Traps",
                true,
                "Reduces trap spawns (landmines and turrets) to pre-v80 spawn rates.");
            offenseFireExits = configFile.Bind(
                "Moon.Offense",
                "Extra Fire Exit",
                false,
                "Adds the fire exit from v9 as an additional entrance to the building.");

            // Rend
            rendShrink = configFile.Bind(
                "Moon.Rend",
                "Shrink Interior",
                false,
                "Reduces Rend's interior size multiplier from 1.8x to 1.6x.");
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
                "What sort of scrap should spawn on Dine?\n\"DontChange\" will avoid making any changes, letting vanilla or other mods take priority.\n\"Consolidate\" will use V73+'s spawn pool, but much fewer items will spawn, worth greater value.\n\"Rollback\" will revert the scrap pool to more closely resemble v50-v72.");
            dineMineshafts = configFile.Bind(
                "Moon.Dine",
                "Dineshaft",
                true,
                "Increase the chance of mineshafts again, much like it was before v80.");
            dineAdjustEnemies = configFile.Bind(
                "Moon.Dine",
                "Adjust Enemies",
                true,
                "Decreases spawn chance for giants and increases spawn chance for Old Birds, leading to more varied outdoor gameplay. Also increases indoor spawns.");
            dineFloods = configFile.Bind(
                "Moon.Dine",
                "Fix Floods",
                false,
                "Dine no longer \"reverse floods\" - all entrances are available at the start of the day, and rising water will gradually shrink the habitable area.");
            dineFireExits = configFile.Bind(
                "Moon.Dine",
                "Extra Fire Exits",
                false,
                "Adds the fire exits from v49 and v56 as additional entrances to the building.");
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
            titanAdjustEnemies = configFile.Bind(
                "Moon.Titan",
                "Adjust Enemies",
                true,
                "Slightly adjusts interior spawns to reduce the frequency of the Jester, and also increases Old Birds.");
            titanWeeds = configFile.Bind(
                "Moon.Titan",
                "No Vain Shrouds",
                true,
                "Disable vain shroud growth on Titan, to prevent the kidnapper fox from camping the ship.");
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
                true,
                "Adjusts interior chances to make manor dominant again. (15%/35%/50% -> 15%/50%/35%) Also increases the size of manors and factories from 1.8x to 2.0x, but mineshafts are not affected.");
            artificeTurrets = configFile.Bind(
                "Moon.Artifice",
                "Increase Turrets",
                false,
                "Drastically increase the spawn rate of turrets, like in the v50 betas.");

            // Embrion
            embrionMega = configFile.Bind(
                "Moon.Embrion",
                "Bigger on the Inside",
                false,
                "Double the interior size, dramatically increase the chance of mineshafts, and greatly increase the number of scrap items. Increase the spawn rates of non-biological enemies in the interior.");
            embrionWeeds = configFile.Bind(
                "Moon.Embrion",
                "No Vain Shrouds",
                false,
                "Disable vain shroud growth on Embrion, since there is limited biological life on the surface.");
        }

        static void MiscConfig()
        {
            nerfNightVision = configFile.Bind(
                "Misc",
                "Reduce Night Vision",
                false,
                "Lower the distance you can see clearly in the dark without a light source, as it was in the original v80 beta.");

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

            cavernsNoKeys = configFile.Bind(
                "Misc",
                "No Keys in Caverns",
                true,
                "Adjusts key spawns in mineshafts, so that they can no longer spawn in cavern tiles, *similar* to pre-v80 behavior.\nAlso removes the distance limit from main entrance, which will allow keys to spawn near deep fire exits, normally impossible in vanilla.");

            cavernsNoTurrets = configFile.Bind(
                "Misc",
                "No Turrets in Caverns",
                true,
                "Turrets are no longer allowed to spawn inside of cave tiles, where the radar is unable to see.");

            proportionalFireExits = configFile.Bind(
                "Misc",
                "Proportional Fire Exits",
                false,
                "Adjusts fire exit spawns so they are unable to spawn too close to main entrance, and less likely to spawn close to each other. WILL NOT WORK with Fairer Fire Exits installed.");

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
                false,
                "Allow butlers to be selected as the subject of an infestation.");

            infestationMasked = configFile.Bind(
                "Infestations",
                "Masked Infestations",
                false,
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
                "Allow Backwater Gunkfish to be selected as the subject of an infestation. Also reduces HP from 4 to 3.");
        }

        static void MigrateLegacyConfig()
        {
            if (dineAdjustEnemies.Value)
            {
                if (!configFile.Bind("Moon.Dine", "Adjust Outdoor Enemies", true, "Legacy setting, doesn't work").Value)
                    dineAdjustEnemies.Value = false;

                configFile.Remove(configFile["Moon.Dine", "Adjust Outdoor Enemies"].Definition);
            }
            if (assuranceGiants.Value)
            {
                if (!configFile.Bind("Moon.Assurance", "More Forest Giants", true, "Legacy setting, doesn't work").Value)
                    assuranceGiants.Value = false;

                configFile.Remove(configFile["Moon.Assurance", "More Forest Giants"].Definition);
            }

            foreach ((string, string) oldKey in new (string, string)[]
            {
                ("Enemy.ForestKeeper", "Forget Out-of-Sight Players"),
                ("Item.TZPInhalant", "Expand Capacity"),
                ("Moon.Dine", "Buff Scrap"),
                ("Enemy.OldBird", "Restore Mech Riding"),
                ("Moon.March", "Shrink Interior"),
                ("Moon.March", "Always Rainy"),
                ("Misc", "Multiplayer Weather Multiplier"),
                ("Moon.Offense", "Mostly Mineshafts"),
                ("Misc", "Shrink Mineshafts"),
                ("Moon.Vow", "Adjust Scrap"),
                ("Moon.Experimentation", "No Nutcrackers"),
                ("Moon.Rend", "Rare Mineshafts"),
                ("Moon.Rend", "Adjust Scrap"),
                ("Moon.Dine", "Adjust Indoor Enemies"),
                ("Moon.Dine", "Adjust Spawn Curves"),
                ("Moon.Dine", "Add Masked"),
                ("Moon.Embrion", "Buff Scrap"),
                ("Moon.Embrion", "Increase Mineshafts"),
                ("Moon.Rend", "Adjust Indoor Enemies"),
                ("Enemy.KidnapperFox", "Squishy"),
                ("Moon.Titan", "Reduce Mineshafts"),
                ("Item.Jetpack", "v49 Controls"),
                ("Enemy.KidnapperFox", "No Roadkill"),
                ("Moon.Dine", "Reduce Butler Chance"),
                ("Moon.Vow", "No Traps"),
                ("Enemy.Coilhead", "Adjust Spawn Curves"),
                ("Moon.March", "Buff Scrap"),
                ("Moon.Titan", "Gold Rush"),
                ("Enemy.OldBird", "See Through Fog"),
                ("Moon.Adamance", "Reduce Chaos"),
                ("Moon.Adamance", "Nerf Eclipse"),
                ("Enemy.Maneater", "Metabolism"),
                ("Item.RadarBooster", "Buff Price"),
                ("Item.StunGrenade", "Nerf Price"),
                ("Item.Jetpack", "Reduce Max Discount"),
                ("Misc", "Rework Foggy Weather"),
                ("Misc", "Nerf Foggy Weather"),
                ("Moon.March", "Adjust Outdoor Enemies"),
                ("Item.MetalSheet", "Increase Value"),
                ("Items", "Adjust Scrap Weights"),
                ("Item.Shovel", "Adjust Weapon Weights"),
                ("Enemy.BackwaterGunkfish", "Squishy"),
                ("Misc", "Random Indoor Fog"),
                ("Moon.Experimentation", "No Forest Keepers"),
                ("Moon.Offense", "Nerf Eclipse"),
                ("Moon.Experimentation", "No Easter Eggs"),
                ("Moon.Embrion", "Adjust Indoor Enemies"),
                ("Moon.Rend", "Restore Earth Leviathans"),
            })
            {
                try
                {
                    configFile.Bind(oldKey.Item1, oldKey.Item2, string.Empty, "Legacy setting, doesn't work");
                    configFile.Remove(configFile[oldKey.Item1, oldKey.Item2].Definition);
                }
                catch
                {
                    Plugin.Logger.LogWarning($"Can't delete \"{oldKey.Item1}\" - \"{oldKey.Item2}\" from config");
                }
            }

            configFile.Save();
        }
    }
}

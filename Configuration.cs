using BepInEx.Configuration;
using ButteRyBalance.Network;

namespace ButteRyBalance
{
    internal class Configuration
    {
        static ConfigFile configFile;

        internal static ConfigEntry<bool> coilheadStunReset, jesterWalkThrough, butlerManorChance, butlerStealthStab, butlerLongCooldown, jesterLongCooldown, butlerKnifePrice, knifeShortCooldown, knifeAutoSwing, maneaterLimitGrowth, maneaterWideTurns, maneaterScrapGrowth, moonsKillSwitch, dineReduceButlers, barberDynamicSpawns;

        internal static void Init(ConfigFile cfg)
        {
            configFile = cfg;

            ItemConfig();
            EnemyConfig();
            MoonConfig();

            cfg.SettingChanged += delegate
            {
                BRBNetworker.ConfigUpdated();
            };
            cfg.ConfigReloaded += delegate
            {
                BRBNetworker.ConfigUpdated();
            };
        }

        static void ItemConfig()
        {
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
                "Adult Maneaters will have far worse turning speed when ");
        }

        static void MoonConfig()
        {
            moonsKillSwitch = configFile.Bind(
                "Moons",
                "Kill Switch",
                false,
                "If this setting is enabled, all other settings in the \"Moon\" category will be disabled. This is helpful if you want to use an external mod to configure them.");

            // Dine
            dineReduceButlers = configFile.Bind(
                "Moon.Dine",
                "Reduce Butler Chance",
                true,
                "Reduces the spawn weight of Butlers on Dine. Intended to be combined with the \"Manor Increased Chance\" setting for Butlers.");
        }
    }
}

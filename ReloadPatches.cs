﻿using Aki.Reflection.Patching;
using BepInEx.Logging;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static EFT.Player;

namespace RealismMod
{
    public static class ReloadController
    {
        public static void ReloadStateCheck(Player player, EFT.Player.FirearmController fc, ManualLogSource logger)
        {
            PlayerProperties.IsInReloadOpertation = fc.IsInReloadOperation();

            if (PlayerProperties.IsInReloadOpertation == true)
            {
                StanceController.CancelShortStock = true;
                StanceController.CancelPistolStance = true;
                StanceController.CancelActiveAim = true;

                if (PlayerProperties.IsAttemptingToReloadInternalMag == true)
                {
                    StanceController.CancelHighReady = fc.Item.WeapClass != "shotgun" ? true : false;
                    StanceController.CancelLowReady = fc.Item.WeapClass == "shotgun" || fc.Item.WeapClass == "pistol" ? true : false;

                    float highReadyBonus = WeaponProperties._WeapClass == "shotgun" && StanceController.IsHighReady == true ? StanceController.HighReadyManipBuff : 1f;
                    float lowReadyBonus = WeaponProperties._WeapClass != "shotgun" && StanceController.IsLowReady == true ? StanceController.LowReadyManipBuff : 1f;

                    float IntenralMagReloadSpeed = Mathf.Clamp(WeaponProperties.CurrentMagReloadSpeed * Plugin.InternalMagReloadMulti.Value * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * highReadyBonus * lowReadyBonus * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.55f, 1.4f);
                    player.HandsAnimator.SetAnimationSpeed(IntenralMagReloadSpeed);

                    if (Plugin.EnableLogging.Value == true)
                    {
                        logger.LogWarning("IsAttemptingToReloadInternalMag = " + IntenralMagReloadSpeed);
                    }
                }
            }
            else
            {
                PlayerProperties.IsAttemptingToReloadInternalMag = false;
                PlayerProperties.IsAttemptingRevolverReload = false;
            }
        }

    }

    public class SetAnimatorAndProceduralValuesPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.Player.FirearmController).GetMethod("SetAnimatorAndProceduralValues", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(EFT.Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("SetAnimatorAndProceduralValues");
                }

                StanceController.ResetStances = true;
            }
        }
    }

    public class CheckAmmoFirearmControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.Player.FirearmController).GetMethod("CheckAmmo", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(EFT.Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("CheckAmmo");
                }

                StanceController.CancelLowReady = true;
                StanceController.CancelShortStock = true;
                StanceController.CancelPistolStance = true;
                StanceController.CancelActiveAim = true;
            }

        }
    }

/*    public class IsInLauncherModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.Player.FirearmController).GetMethod("IsInLauncherMode", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(EFT.Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("IsInLauncherMode");
                }



            }

        }
    }*/


    public class CheckChamberFirearmControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.Player.FirearmController).GetMethod("CheckChamber", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(EFT.Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("CheckChamber");
                }

                StanceController.CancelLowReady = true;
                StanceController.CancelHighReady = true;
                StanceController.CancelShortStock = true;
                StanceController.CancelActiveAim = true;
            }

        }
    }

    public class SetLauncherPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetLauncher", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(bool isLauncherEnabled)
        {
            Plugin.LauncherIsActive = isLauncherEnabled;
        }
    }

    public class SetWeaponLevelPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetWeaponLevel", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance, float weaponLevel)
        {
            if (WeaponProperties._WeapClass == "shotgun")
            {
                if (weaponLevel < 3)
                {
                    weaponLevel += 1;
                }
                WeaponAnimationSpeedControllerClass.SetWeaponLevel(__instance.Animator, weaponLevel);
            }

        }
    }


    public class SetHammerArmedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetHammerArmed", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {

            if (Plugin.IsFiring != true && PlayerProperties.IsInReloadOpertation)
            {
                float hammerSpeed = Mathf.Clamp(WeaponProperties.TotalChamberSpeed * Plugin.GlobalArmHammerSpeedMulti.Value * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.5f, 1.35f);
                __instance.SetAnimationSpeed(hammerSpeed);
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("SetHammerArmed, firing and reload op");
                    Logger.LogWarning("SetHammerArmed = " + hammerSpeed);
                }
            }

        }
    }

    public class CheckAmmoPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("CheckAmmo", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            float bonus = Plugin.GlobalCheckAmmoMulti.Value;
            if (WeaponProperties._WeapClass == "pistol")
            {
                bonus = Plugin.GlobalCheckAmmoPistolSpeedMulti.Value;
            }

            float totalChecmAmmoPatch = Mathf.Clamp(WeaponProperties.CurrentMagReloadSpeed * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * StanceController.HighReadyManipBuff * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)) * bonus, 0.6f, 1.3f);

            __instance.SetAnimationSpeed(totalChecmAmmoPatch);

            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===CheckAmmo===");
                Logger.LogWarning("Check Ammo =" + totalChecmAmmoPatch);
                Logger.LogWarning("=============");
            }

        }
    }

    public class CheckChamberPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("CheckChamber", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            float chamberSpeed = WeaponProperties.TotalChamberCheckSpeed;
            if (WeaponProperties._WeapClass == "pistol")
            {
                chamberSpeed *= Plugin.GlobalCheckChamberPistolSpeedMulti.Value;
            }
            else if (WeaponProperties._WeapClass == "shotgun")
            {
                chamberSpeed *= Plugin.GlobalCheckChamberShotgunSpeedMulti.Value;
            }
            else
            {
                chamberSpeed *= Plugin.GlobalCheckChamberSpeedMulti.Value;
            }

            float totalCheckChamberSpeed = Mathf.Clamp(chamberSpeed * PlayerProperties.FixSkillMulti * PlayerProperties.ReloadInjuryMulti * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.5f, 1.3f);
          
            __instance.SetAnimationSpeed(totalCheckChamberSpeed);

            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===CheckChamber===");
                Logger.LogWarning("Check Chamber = " + totalCheckChamberSpeed);
                Logger.LogWarning("=============");
            }

  
        }
    }

    public class SetBoltActionReloadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetBoltActionReload", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {

            if (WeaponProperties._IsManuallyOperated == true || Plugin.LauncherIsActive == true)
            {

                float chamberSpeed = WeaponProperties.TotalFiringChamberSpeed;
                if (WeaponProperties._WeapClass == "shotgun")
                {
                    chamberSpeed *= Plugin.GlobalShotgunRackSpeedFactor.Value;
                }
                if (Plugin.LauncherIsActive == true)
                {
                    chamberSpeed *= Plugin.GlobalUBGLReloadMulti.Value;
                }
                if (WeaponProperties._WeapClass == "sniperRifle")
                {
                    chamberSpeed *= Plugin.GlobalBoltSpeedMulti.Value;
                }

                float totalChamberSpeed = Mathf.Clamp(chamberSpeed * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.5f, 1.3f);

                __instance.SetAnimationSpeed(totalChamberSpeed);

                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===SetBoltActionReload===");
                    Logger.LogWarning("Set Bolt Action Reload = " + totalChamberSpeed);
                    Logger.LogWarning("=============");
                }
      
            }
        }
    }

    public class SetMalfRepairSpeedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetMalfRepairSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPrefix]
        private static void Prefix(FirearmsAnimator __instance, float fix)
        {

            float totalFixSpeed = Mathf.Clamp(fix * WeaponProperties.TotalFixSpeed * PlayerProperties.ReloadInjuryMulti * Plugin.GlobalFixSpeedMulti.Value * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.5f, 1.3f);
            WeaponAnimationSpeedControllerClass.SetSpeedFix(__instance.Animator, totalFixSpeed);
            __instance.SetAnimationSpeed(totalFixSpeed);
            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===SetMalfRepairSpeed===");
                Logger.LogWarning("SetMalfRepairSpeed = " + totalFixSpeed);
                Logger.LogWarning("=============");
            }

        }
    }

    public class RechamberSpeedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("Rechamber", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            float chamberSpeed = WeaponProperties.TotalFixSpeed;
            if (WeaponProperties._WeapClass == "pistol")
            {
                chamberSpeed *= Plugin.RechamberPistolSpeedMulti.Value;
            }
            else
            {
                chamberSpeed *= Plugin.GlobalRechamberSpeedMulti.Value;
            }
            
            float totalRechamberSpeed = Mathf.Clamp(chamberSpeed * PlayerProperties.FixSkillMulti * PlayerProperties.ReloadInjuryMulti * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.5f, 1.35f);

            __instance.SetAnimationSpeed(totalRechamberSpeed);
            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===Rechamber===");
                Logger.LogWarning("Rechamber = " + totalRechamberSpeed);
                Logger.LogWarning("=============");
            }

            StanceController.CancelShortStock = true;
            StanceController.CancelPistolStance = true;

        }
    }

    public class CanStartReloadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("CanStartReload", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, bool __result)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (__result == true)
                {
                    if (__instance.Item.GetCurrentMagazine() == null)
                    {
                        PlayerProperties.NoCurrentMagazineReload = true;
                    }
                    else
                    {
                        PlayerProperties.NoCurrentMagazineReload = false;
                    }
                }
            }
        }
    }

    public class ReloadMagPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("ReloadMag", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, MagazineClass magazine)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {

                }
                StatCalc.SetMagReloadSpeeds(__instance, magazine);
            }
        }
    }


    public class QuickReloadMagPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("QuickReloadMag", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, MagazineClass magazine)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===QuickReloadMag===");
                    Logger.LogWarning("=============");
                }

                StatCalc.SetMagReloadSpeeds(__instance, magazine, true);
            }
        }
    }


    public class ReloadRevolverDrumPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("ReloadRevolverDrum", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===ReloadRevolverDrum===");
                    Logger.LogWarning("=============");
                }

                PlayerProperties.IsAttemptingToReloadInternalMag = true;
                PlayerProperties.IsAttemptingRevolverReload = true;
            }
        }
    }

    public class ReloadWithAmmoPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("ReloadWithAmmo", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===ReloadWithAmmo===");
                    Logger.LogWarning("=============");
                }

                PlayerProperties.IsAttemptingToReloadInternalMag = true;
            }
        }
    }

    public class ReloadBarrelsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("ReloadBarrels", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance)
        {
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===ReloadBarrels===");
                    Logger.LogWarning("=============");
                }

                PlayerProperties.IsAttemptingToReloadInternalMag = true;
            }
        }
    }


    public class SetMagTypeNewPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetMagTypeNew", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {

            float totalReloadSpeed = Mathf.Clamp(WeaponProperties.CurrentMagReloadSpeed * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * StanceController.HighReadyManipBuff * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.45f, 1.3f);
            __instance.SetAnimationSpeed(totalReloadSpeed);
            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===SetMagTypeNew===");
                Logger.LogWarning("SetMagTypeNew = " + totalReloadSpeed);
                Logger.LogWarning("=============");
            }


        }
    }

    public class SetMagTypeCurrentPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetMagTypeCurrent", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            float totalReloadSpeed = Mathf.Clamp(WeaponProperties.CurrentMagReloadSpeed * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * StanceController.HighReadyManipBuff * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.45f, 1.3f);
            __instance.SetAnimationSpeed(totalReloadSpeed);
            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===SetMagTypeCurrent===");
                Logger.LogWarning("SetMagTypeCurrent = " + totalReloadSpeed);
                Logger.LogWarning("=============");
            }

        }
    }

    public class SetMagInWeaponPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetMagInWeapon", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            if (PlayerProperties.IsMagReloading == true)
            {
                float totalReloadSpeed = Mathf.Clamp(WeaponProperties.NewMagReloadSpeed * PlayerProperties.ReloadSkillMulti * PlayerProperties.ReloadInjuryMulti * PlayerProperties.GearReloadMulti * StanceController.HighReadyManipBuff * (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)), 0.45f, 1.3f);
                __instance.SetAnimationSpeed(totalReloadSpeed);
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===SetMagInWeapon===");
                    Logger.LogWarning("SetMagInWeapon = " + totalReloadSpeed);
                    Logger.LogWarning("ReloadSkillMulti = " + PlayerProperties.ReloadSkillMulti);
                    Logger.LogWarning("ReloadInjuryMulti = " + PlayerProperties.ReloadInjuryMulti);
                    Logger.LogWarning("GearReloadMulti = " + PlayerProperties.GearReloadMulti);
                    Logger.LogWarning("HighReadyManipBuff = " + StanceController.HighReadyManipBuff);
                    Logger.LogWarning("RemainingArmStamPercentage = " + (Mathf.Max(PlayerProperties.RemainingArmStamPercentage, 0.7f)));
                    Logger.LogWarning("NewMagReloadSpeed = " + WeaponProperties.NewMagReloadSpeed);
                    Logger.LogWarning("=============");
                }
     
            }
        }
    }

    public class SetSpeedParametersPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetSpeedParameters", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            if (Plugin.EnableLogging.Value == true)
            {
                Logger.LogWarning("===SetSpeedParameters===");
                Logger.LogWarning("=============");
            }
            __instance.SetAnimationSpeed(1);
        }
    }



    public class OnMagInsertedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("method_47", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(ref Player.FirearmController __instance)
        {
            //to find this again, look for private void method_47(){ this.CurrentOperation.OnMagInsertedToWeapon(); }
            Player player = (Player)AccessTools.Field(typeof(EFT.Player.FirearmController), "_player").GetValue(__instance);
            if (player.IsYourPlayer == true)
            {
                if (Plugin.EnableLogging.Value == true)
                {
                    Logger.LogWarning("===OnMagInsertedPatch/method_47===");
                    Logger.LogWarning("=============");
                }

                PlayerProperties.IsMagReloading = false;
                player.HandsAnimator.SetAnimationSpeed(1);
            }

        }

    }





    /*    public class SetBoltCatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetBoltCatch", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogWarning("SetBoltCatch");
            }
        }
    */

    /*    public class SetFireModePatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetFireMode", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetFireMode");
                __instance.SetAnimationSpeed(2);
            }
        }*/


    /*    public class SetAnimatorAndProceduralValuesPatch : ModulePatch
{
   protected override MethodBase GetTargetMethod()
   {
       return typeof(Player.FirearmController).GetMethod("SetAnimatorAndProceduralValues", BindingFlags.Instance | BindingFlags.Public);
   }

   [PatchPostfix]
   private static void PatchPostfix(FirearmsAnimator __instance)
   {
       Logger.LogInfo("SetSpeedReload");
   }
}
*/


    /*    public class SetAmmoCompatiblePatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetAmmoCompatible", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetAmmoCompatible");
            }
        }
    */
    /*    public class SetPatronInWeaponVisiblePatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetPatronInWeaponVisible", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetPatronInWeaponVisible");
            }
        }

    */

    /*    public class SetAmmoInChamberPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetAmmoInChamber", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetAmmoInChamber");
            }
        }
    */
    /*    public class SetChamberIndexWithShellPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetChamberIndexWithShell", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetChamberIndexWithShell");
            }
        }
    */
    /*    public class SetShellsInWeapon : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetShellsInWeapon", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("SetShellsInWeapon");
            }
        }*/

    /*    public class ReloadPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("Reload", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("Reload");

            }
        }

        public class SetAmmoOnMagPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("SetAmmoOnMag", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                if (Helper.IsAttemptingToReloadInternalMag == true && Helper.IsInReloadOpertation)
                {
                   *//* __instance.SetAnimationSpeed(5);*//*
                    Logger.LogInfo("SetAmmoOnMag");
                }
            }
        }*/

    /*    public class ResetReloadPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(FirearmsAnimator).GetMethod("ResetReload", BindingFlags.Instance | BindingFlags.Public);
            }

            [PatchPostfix]
            private static void PatchPostfix(FirearmsAnimator __instance)
            {
                Logger.LogInfo("ResetReload");
            }
        }*/

    /*    public class DischargePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("Discharge", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            Logger.LogInfo("Discharge");
        }
    }*/

    /*    public class SetSpeedReloadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(WeaponAnimationSpeedControllerClass).GetMethod("SetSpeedReload", BindingFlags.Static | BindingFlags.Public);

        }

        [PatchPostfix]
        private static void PatchPostfix()
        {
            Logger.LogError("=====================================SetSpeedReload===================================");

        }
    }

    public class PullOutMagInInventoryModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("PullOutMagInInventoryMode", BindingFlags.Instance | BindingFlags.Public);

        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            Logger.LogWarning("=====================================PullOutMagInInventoryMode===================================");

        }
    }

    public class ResetInsertMagInInventoryModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("ResetInsertMagInInventoryMode", BindingFlags.Instance | BindingFlags.Public);

        }

        [PatchPostfix]
        private static void PatchPostfix(FirearmsAnimator __instance)
        {
            Logger.LogWarning("=====================================ResetInsertMagInInventoryMode===================================");

        }
    }*/

}

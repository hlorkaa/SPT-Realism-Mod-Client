﻿using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
/*using Silencer = GClass2124;
using FlashHider = GClass2121;
using MuzzleCombo = GClass2116;
using Barrel = GClass2138;
using Mount = GClass2134;
using Receiver = GClass2141;
using Stock = GClass2136;
using Charge = GClass2129;
using CompactCollimator = GClass2122;
using Collimator = GClass2121;
using AssaultScope = GClass2120;
using Scope = GClass2124;
using IronSight = GClass2123;
using SpecialScope = GClass2125;
using AuxiliaryMod = GClass2104;
using Foregrip = GClass2108;
using PistolGrip = GClass2140;
using Gasblock = GClass2109;
using Handguard = GClass2139;
using Bipod = GClass2106;
using Flashlight = GClass2113;
using TacticalCombo = GClass2112;*/


namespace RealismMod
{
    public static class Utils
    {
        public static bool ProgramKEnabled = false;

        public static bool IsReady = false;

        public static bool WeaponReady = false;

        public static bool HasRunErgoWeightCalc = false;

        public static string Silencer = "550aa4cd4bdc2dd8348b456c";
        public static string FlashHider = "550aa4bf4bdc2dd6348b456b";
        public static string MuzzleCombo = "550aa4dd4bdc2dc9348b4569";
        public static string Barrel = "555ef6e44bdc2de9068b457e";
        public static string Mount = "55818b224bdc2dde698b456f";
        public static string Receiver = "55818a304bdc2db5418b457d";
        public static string Stock = "55818a594bdc2db9688b456a";
        public static string Charge = "55818a6f4bdc2db9688b456b";
        public static string CompactCollimator = "55818acf4bdc2dde698b456b";
        public static string Collimator = "55818ad54bdc2ddc698b4569";
        public static string AssaultScope = "55818add4bdc2d5b648b456f";
        public static string Scope = "55818ae44bdc2dde698b456c";
        public static string IronSight = "55818ac54bdc2d5b648b456e";
        public static string SpecialScope = "55818aeb4bdc2ddc698b456a";
        public static string AuxiliaryMod = "5a74651486f7744e73386dd1";
        public static string Foregrip = "55818af64bdc2d5b648b4570";
        public static string PistolGrip = "55818a684bdc2ddd698b456d";
        public static string Gasblock = "56ea9461d2720b67698b456f";
        public static string Handguard = "55818a104bdc2db9688b4569";
        public static string Bipod = "55818afb4bdc2dde698b456d";
        public static string Flashlight = "55818b084bdc2d5b648b4571";
        public static string TacticalCombo = "55818b164bdc2ddc698b456c";
        public static string UBGL = "55818b014bdc2ddc698b456b";

        public static LightComponent GetLightComponent(LightComponent x)
        {
            return x;
        }

        public static string GetLightId(LightComponent x)
        {
            return x.Item.Id;
        }

        public static Item GetContainedItem(Slot slot)
        {
            return slot.ContainedItem;
        }


        public static bool NullCheck(string[] confItemArray)
        {
            if (confItemArray != null && confItemArray.Length > 0)
            {
                if (confItemArray[0] == "SPTRM") // if the array has SPTRM, but is set up incorrectly, it will probably cause null errors
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckIsReady()
        {
            GameWorld gameWorld = Singleton<GameWorld>.Instance;
            SessionResultPanel sessionResultPanel = Singleton<SessionResultPanel>.Instance;

            if (gameWorld?.AllPlayers.Count > 0)
            {
                Player player = gameWorld.AllPlayers[0];
                if (player != null && player?.HandsController != null)
                {
                    if (player?.HandsController?.Item != null && player?.HandsController?.Item is Weapon)
                    {
                        Utils.WeaponReady = true;
                    }
                    else 
                    {
                        Utils.WeaponReady = false;
                    }
                }
            }

            if (gameWorld == null || gameWorld.AllPlayers == null || gameWorld.AllPlayers.Count <= 0 || sessionResultPanel != null)
            {
                Utils.IsReady = false;
                return false;
            }
            Utils.IsReady = true;

            return true;
        }


        public static void SafelyAddAttributeToList(ItemAttributeClass itemAttribute, Mod __instance)
        {
            if (itemAttribute.Base() != 0f)
            {
                __instance.Attributes.Add(itemAttribute);
            }
        }

        public static bool IsSight(Mod mod)
        {
            if (mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Scope] || mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[AssaultScope] || mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[SpecialScope] || mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[CompactCollimator] || mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Collimator] || mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[IronSight])
            {
                return true;
            }
            return false;
        }

        public static bool IsStock(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Stock] ? true : false;
        }
        public static bool IsSilencer(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Silencer] ? true: false;

        }
        public static bool IsMagazine(Mod mod)
        {
            return (mod is MagazineClass);
        }
        public static bool IsFlashHider(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[FlashHider] ? true : false;
        }
        public static bool IsMuzzleCombo(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[MuzzleCombo] ? true : false;
        }
        public static bool IsBarrel(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Barrel] ? true : false;
        }
        public static bool IsMount(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Mount] ? true : false;
        }
        public static bool IsReceiver(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Receiver] ? true : false;
        }
        public static bool IsCharge(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Charge] ? true : false;
        }
        public static bool IsCompactCollimator(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[CompactCollimator] ? true : false;
        }
        public static bool IsCollimator(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Collimator] ? true : false;
        }
        public static bool IsAssaultScope(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[AssaultScope] ? true : false;
        }
        public static bool IsScope(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Scope] ? true : false;
        }
        public static bool IsIronSight(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[IronSight] ? true : false;
        }
        public static bool IsSpecialScope(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[SpecialScope] ? true : false;
        }
        public static bool IsAuxiliaryMod(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[AuxiliaryMod] ? true : false;
        }
        public static bool IsForegrip(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Foregrip] ? true : false;
        }
        public static bool IsPistolGrip(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[PistolGrip] ? true : false;
        }
        public static bool IsGasblock(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Gasblock] ? true : false;
        }
        public static bool IsHandguard(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Handguard] ? true : false;
        }
        public static bool IsBipod(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Bipod] ? true : false;
        }
        public static bool IsFlashlight(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[Flashlight] ? true : false;
        }
        public static bool IsTacticalCombo(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[TacticalCombo] ? true : false;
        }
        public static bool IsUBGL(Mod mod)
        {
            return mod.GetType() == TemplateIdToObjectMappingsClass.TypeTable[UBGL] ? true : false;
        }
    }
}

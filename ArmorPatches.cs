﻿using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Reflection;
using System.Linq;
using EFT.InventoryLogic;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Comfort.Common;
using static RealismMod.Attributes;
using UnityEngine;

namespace RealismMod
{

    public class RigConstructorPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GClass2343).GetConstructor(new Type[] { typeof(string), typeof(GClass2250) });
        }


        [PatchPostfix]
        private static void PatchPostfix(GClass2343 __instance)
        {
            Item item = __instance as Item;

            float gearReloadSpeed = GearProperties.ReloadSpeedMulti(item);
            float reloadSpeedPercent = 0;

            reloadSpeedPercent = (float)Math.Round((gearReloadSpeed - 1f) * 100f);

            if (gearReloadSpeed != 1)
            {
                List<ItemAttributeClass> reloadAtt = item.Attributes;
                ItemAttributeClass reloadAttClass = new ItemAttributeClass(Attributes.ENewItemAttributeId.GearReloadSpeed);
                reloadAttClass.Name = ENewItemAttributeId.GearReloadSpeed.GetName();
                reloadAttClass.Base = () => reloadSpeedPercent;
                reloadAttClass.StringValue = () => reloadSpeedPercent.ToString() + " %";
                reloadAttClass.DisplayType = () => EItemAttributeDisplayType.Compact;
                reloadAttClass.LabelVariations = EItemAttributeLabelVariations.Colored;
                reloadAttClass.LessIsGood = false;
                reloadAtt.Add(reloadAttClass);
            }
        }
    }

    public class ArmorPatches
    {

        public class ArmorZoneBaseDisplayPatch : ModulePatch
        {

            private static Type _targetType;
            private static MethodInfo _method_0;

            public ArmorZoneBaseDisplayPatch()
            {
                _targetType = PatchConstants.EftTypes.Single(IsType);
                _method_0 = _targetType.GetMethod("method_0", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            protected override MethodBase GetTargetMethod()
            {
                return _method_0;
            }

            private static bool IsType(Type type)
            {
                return type.GetField("armorComponent_0") != null && type.GetField("item") != null;
            }

            private static string GetItemClass(CompositeArmorComponent x)
            {
                return x.Item.ShortName.Localized(null) + ": " + GearProperties.ArmorClass(x.Item);
            }

            [PatchPrefix]
            private static bool Prefix(ref float __result, ref EFT.InventoryLogic.ArmorComponent ___armorComponent_0)
            {
                float armorElementsToAdd = 0;
                if (GearProperties.HasNeckArmor(___armorComponent_0.Item) == true) 
                {
                    armorElementsToAdd += 1;
                }
                if (GearProperties.HasSideArmor(___armorComponent_0.Item) == true)
                {
                    armorElementsToAdd += 1;
                }
                if (GearProperties.HasStomachArmor(___armorComponent_0.Item) == true)
                {
                    armorElementsToAdd += 1;
                }
                __result = ___armorComponent_0.ArmorZone.Contains(EBodyPart.Stomach) ? (float)___armorComponent_0.ArmorZone.Length + armorElementsToAdd - 1f : (float)___armorComponent_0.ArmorZone.Length + armorElementsToAdd;
                return false;
            }
        }

        public class ArmorZoneSringValueDisplayPatch : ModulePatch
        {

            private static Type _targetType;
            private static MethodInfo _method_1;

            public ArmorZoneSringValueDisplayPatch()
            {
                _targetType = PatchConstants.EftTypes.Single(IsType);
                _method_1 = _targetType.GetMethod("method_1", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            protected override MethodBase GetTargetMethod()
            {
                return _method_1;
            }

            private static bool IsType(Type type)
            {
                return type.GetField("armorComponent_0") != null && type.GetField("item") != null;
            }

            private static string GetItemClass(CompositeArmorComponent x)
            {
                return x.Item.ShortName.Localized(null) + ": " + GearProperties.ArmorClass(x.Item);
            }

            [PatchPrefix]
            private static bool Prefix(ref string __result, ref EFT.InventoryLogic.ArmorComponent ___armorComponent_0)
            {
                if (___armorComponent_0.ArmorZone.Contains(EBodyPart.Head)) 
                {
                    return true;
                }

                List<string> parts = new List<string>();

                foreach (EBodyPart e in ___armorComponent_0.ArmorZone) 
                {
                    if (e != EBodyPart.Stomach) 
                    {
                        parts.Add(e.ToString());
                    }
    
                }
                if (GearProperties.HasNeckArmor(___armorComponent_0.Item) == true)
                {
                    parts.Add("NECK");
                }
                if (GearProperties.HasSideArmor(___armorComponent_0.Item) == true)
                {
                    parts.Add("SIDES");
                }
                if (GearProperties.HasStomachArmor(___armorComponent_0.Item) == true)
                {
                    parts.Add("STOMACH");
                }

                __result =  Enumerable.Cast<object>(parts).CastToStringValue("\n", true);
                return false;
            }
        }


        public class ArmorClassDisplayPatch : ModulePatch
        {

            private static Type _targetType;
            private static MethodInfo _method_4;

            public ArmorClassDisplayPatch()
            {
                _targetType = PatchConstants.EftTypes.Single(IsType);
                _method_4 = _targetType.GetMethod("method_4", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            protected override MethodBase GetTargetMethod()
            {
                return _method_4;
            }

            private static bool IsType(Type type)
            {
                return type.GetField("armorComponent_0") != null && type.GetField("item") != null;
            }

            private static string GetItemClass(CompositeArmorComponent x)
            {
                return x.Item.ShortName.Localized(null) + ": " + GearProperties.ArmorClass(x.Item);
            }

            [PatchPrefix]
            private static bool Prefix(ref string __result, ref EFT.InventoryLogic.ArmorComponent ___armorComponent_0)
            {
                CompositeArmorComponent[] array = ___armorComponent_0.Item.GetItemComponentsInChildren<CompositeArmorComponent>(true).ToArray<CompositeArmorComponent>();

                if (array.Length > 1)
                {
                    __result = array.Select(new Func<CompositeArmorComponent, string>(GetItemClass)).CastToStringValue("\n", true);
                    return false;
                }

                __result = GearProperties.ArmorClass(___armorComponent_0.Item);
                return false;
            }
        }


        public class ArmorComponentPatch : ModulePatch
        {

            protected override MethodBase GetTargetMethod()
            {
                return typeof(EFT.InventoryLogic.ArmorComponent).GetConstructor(new Type[] { typeof(Item), typeof(GInterface228), typeof(RepairableComponent), typeof(BuffComponent) });
            }


            [PatchPostfix]
            private static void PatchPostfix(ArmorComponent __instance)
            {

                bool showADS = false;
                EBodyPart[] zones = __instance.ArmorZone;

                foreach (EBodyPart part in zones)
                {
                    if (part == EBodyPart.Head)
                    {
                        showADS = true;
                    }
                }

                if (showADS == true)
                {
                    List<ItemAttributeClass> canADSAtt = __instance.Item.Attributes;
                    ItemAttributeClass canADSAttAttClass = new ItemAttributeClass(Attributes.ENewItemAttributeId.CanAds);
                    canADSAttAttClass.Name = ENewItemAttributeId.CanAds.GetName();
                    canADSAttAttClass.StringValue = () => GearProperties.AllowsADS(__instance.Item).ToString();
                    canADSAttAttClass.DisplayType = () => EItemAttributeDisplayType.Compact;
                    canADSAtt.Add(canADSAttAttClass);
                }

                if (Plugin.ModConfig.realistic_ballistics == true)
                {
                    bool canSpall = GearProperties.CanSpall(__instance.Item);

                    List<ItemAttributeClass> bluntAtt = __instance.Item.Attributes;
                    ItemAttributeClass bluntAttClass = new ItemAttributeClass(Attributes.ENewItemAttributeId.BluntThroughput);
                    bluntAttClass.Name = ENewItemAttributeId.BluntThroughput.GetName();
                    bluntAttClass.StringValue = () => ((1 - __instance.BluntThroughput) * 100).ToString() + " %";
                    bluntAttClass.DisplayType = () => EItemAttributeDisplayType.Compact;
                    bluntAtt.Add(bluntAttClass);

                    List<ItemAttributeClass> canSpallAtt = __instance.Item.Attributes;
                    ItemAttributeClass canSpallAttClass = new ItemAttributeClass(Attributes.ENewItemAttributeId.CanSpall);
                    canSpallAttClass.Name = ENewItemAttributeId.CanSpall.GetName();
                    canSpallAttClass.StringValue = () => canSpall.ToString();
                    canSpallAttClass.DisplayType = () => EItemAttributeDisplayType.Compact;
                    canSpallAtt.Add(canSpallAttClass);

                    if (canSpall == true)
                    {
                        List<ItemAttributeClass> spallReductAtt = __instance.Item.Attributes;
                        ItemAttributeClass spallReductAttClass = new ItemAttributeClass(Attributes.ENewItemAttributeId.SpallReduction);
                        spallReductAttClass.Name = ENewItemAttributeId.SpallReduction.GetName();
                        spallReductAttClass.StringValue = () => ((1 - GearProperties.SpallReduction(__instance.Item)) * 100).ToString() + " %";
                        spallReductAttClass.DisplayType = () => EItemAttributeDisplayType.Compact;
                        spallReductAtt.Add(spallReductAttClass);
                    }
                }
            }
        }
    }
}

using ColossalFramework.UI;
using CustomData.Localization;
using CustomData.Xml;
using ICities;
using Kwytto.Interfaces;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System.Globalization;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("0.0.0.*")]
namespace CustomData
{
    public class ModInstance : BasicIUserMod<ModInstance, CDController>
    {
        public override string SimpleName => "Custom Data Mod";

        public override string SafeName => "CustomData";

        public override string Acronym => "CD";

        public override Color ModColor => ColorExtensions.FromRGB("56ab41");

        public override string Description => Str.root_modDescription;

        public override BaseController GetController() => Controller;

        protected override void OnLevelLoadedInherit(LoadMode mode) { }

        protected override void SetLocaleCulture(CultureInfo culture) => Str.Culture = culture;
        
    }
}

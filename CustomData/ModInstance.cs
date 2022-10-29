using CustomData.Localization;
using CustomData.UI;
using Kwytto.Interfaces;
using Kwytto.Utils;
using System.Globalization;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.0.0.1")]
namespace CustomData
{
    public class ModInstance : BasicIUserMod<ModInstance, CDController>
    {
        public override string SimpleName => "Custom Data Mod";

        public override string SafeName => "CustomData";

        public override string Acronym => "CD";

        public override Color ModColor => ColorExtensions.FromRGB("56ab41");

        public override string Description => Str.root_modDescription;

        protected override void SetLocaleCulture(CultureInfo culture) => Str.Culture = culture;

        private IUUIButtonContainerPlaceholder[] cachedUUI;
        public override IUUIButtonContainerPlaceholder[] UUIButtons => cachedUUI ?? (cachedUUI = new IUUIButtonContainerPlaceholder[]
        {
            new UUIWindowButtonContainerPlaceholder(
             buttonName: GeneralName,
             tooltip: GeneralName,
             iconPath: IconName,
             windowGetter: ()=>CDMainWindow.Instance
             ),
        });
        protected override void DoOnLevelUnloading()
        {
            base.DoOnLevelUnloading();
            cachedUUI = null;
        }
    }
}

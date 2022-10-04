using CustomData.Localization;
using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.UI;
using UnityEngine;

namespace CustomData.UI
{
    public class CDAddressingSettingsTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_addressingSettingsTab_title;
        private readonly CDMainWindow root;
        public CDAddressingSettingsTab(CDMainWindow root)
        {
            this.root = root;

        }

        private OwnCitySettingsDW m_parsedData = null;
        private BuildingGeneralDW m_buildingData = null;

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (m_parsedData is null)
            {
                m_parsedData = CDStorage.Instance.GetOwnCitySettings();
            }
            if (m_buildingData is null)
            {
                m_buildingData = CDStorage.Instance.GetBuildingGeneralSettings();
            }
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Str.cd_addressingSettingsTab_postalCodeSettings);
                    GUIKwyttoCommons.AddIntField(tabAreaSize.x, Str.cd_addressingSettingsTab_citywidePostalCode, m_parsedData.PostalCodeDigits, (x) => m_parsedData.PostalCodeDigits = x.Value, true, 0, 999, "000");
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.TextWithLabel(tabAreaSize.x - 20, Str.cd_addressingSettingsTab_postalCodeFormat, m_parsedData.PostalCodeFormat, (x) => m_parsedData.PostalCodeFormat = x);
                        if (GUILayout.Button("?", root.m_inlineBtnStyle))
                        {
                            ShowHelpFormatPostalCode();
                        }
                    }
                    GUILayout.Space(8);
                    GUILayout.Label(Str.cd_addressingSettingsTab_addressesLines);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.TextWithLabel(tabAreaSize.x - 20, Str.cd_addressingSettingsTab_addressLine1, m_buildingData.AddressLine1, (x) => m_buildingData.AddressLine1 = x);
                        if (GUILayout.Button("?", root.m_inlineBtnStyle))
                        {
                            ShowHelpAddressLinesCode();
                        }
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.TextWithLabel(tabAreaSize.x - 20, Str.cd_addressingSettingsTab_addressLine2, m_buildingData.AddressLine2, (x) => m_buildingData.AddressLine2 = x);
                        if (GUILayout.Button("?", root.m_inlineBtnStyle))
                        {
                            ShowHelpAddressLinesCode();
                        }
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.TextWithLabel(tabAreaSize.x - 20, Str.cd_addressingSettingsTab_addressLine3, m_buildingData.AddressLine3, (x) => m_buildingData.AddressLine3 = x);
                        if (GUILayout.Button("?", root.m_inlineBtnStyle))
                        {
                            ShowHelpAddressLinesCode();
                        }
                    }
                }
            }
        }

        private void ShowHelpAddressLinesCode()
        {
            KwyttoDialog.ShowModal(new KwyttoDialog.BindProperties
            {
                buttons = KwyttoDialog.basicOkButtonBar,
                title = Str.cd_addressingSettingsTab_addressesLines,
                message = Str.cd_addressingSettingsTab_addressLineHelpHeaderText,
                scrollText =
$@"A - {Str.cd_addressingSettingsTab_addressLineDescription_A}
B - {Str.cd_addressingSettingsTab_addressLineDescription_B}
C - {Str.cd_addressingSettingsTab_addressLineDescription_C}
D - {Str.cd_addressingSettingsTab_addressLineDescription_D}
E - {Str.cd_addressingSettingsTab_addressLineDescription_E}
F - {Str.cd_addressingSettingsTab_addressLineDescription_F}
"
            });
        }

        private void ShowHelpFormatPostalCode()
        {
            KwyttoDialog.ShowModal(new KwyttoDialog.BindProperties
            {
                buttons = KwyttoDialog.basicOkButtonBar,
                title = Str.cd_addressingSettingsTab_postalCodeSettings,
                message = Str.cd_addressingSettingsTab_postalCodeHelpHeaderText,
                scrollText =
$@"A - {Str.cd_addressingSettingsTab_postalCodeDescription_A}
B - {Str.cd_addressingSettingsTab_postalCodeDescription_B}
C - {Str.cd_addressingSettingsTab_postalCodeDescription_C}
D - {Str.cd_addressingSettingsTab_postalCodeDescription_D}
E - {Str.cd_addressingSettingsTab_postalCodeDescription_E}

F - {Str.cd_addressingSettingsTab_postalCodeDescription_F}
G - {Str.cd_addressingSettingsTab_postalCodeDescription_G}
H - {Str.cd_addressingSettingsTab_postalCodeDescription_H}
I - {Str.cd_addressingSettingsTab_postalCodeDescription_I}
J - {Str.cd_addressingSettingsTab_postalCodeDescription_J}

K - {Str.cd_addressingSettingsTab_postalCodeDescription_K}
L - {Str.cd_addressingSettingsTab_postalCodeDescription_L}
M - {Str.cd_addressingSettingsTab_postalCodeDescription_M}

N - {Str.cd_addressingSettingsTab_postalCodeDescription_N}
O - {Str.cd_addressingSettingsTab_postalCodeDescription_O}
P - {Str.cd_addressingSettingsTab_postalCodeDescription_P}

X - {Str.cd_addressingSettingsTab_postalCodeDescription_X}
x - {Str.cd_addressingSettingsTab_postalCodeDescription_x_}
Y - {Str.cd_addressingSettingsTab_postalCodeDescription_Y}
y - {Str.cd_addressingSettingsTab_postalCodeDescription_y_}
"
            });
        }

        public void Reset()
        {
        }
    }
}

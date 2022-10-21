extern alias WE;

using CustomData;
using CustomData.Overrides;
using Kwytto.Interfaces;
using Kwytto.Utils;
using UnityEngine;
using WE::WriteEverywhere.Font.Utility;

namespace VariablesWE_CD
{
    internal class CDVarController : MonoBehaviour
    {
        static CDVarController instance;
        public static CDVarController Instance
        {
            get
            {
                if (instance is null && ModInstance.Controller is BaseController control)
                {
                    instance = control.gameObject.AddComponent<CDVarController>();
                }
                return instance;
            }
        }

        public SimpleNonSequentialList<BasicRenderInformation> CachedBuildingImages { get; private set; } = new SimpleNonSequentialList<BasicRenderInformation>();

        private void Start()
        {
            CDFacade.Instance.EventOnBuildingLogoChanged += (x) => CachedBuildingImages.Remove(x);
        }
    }
}

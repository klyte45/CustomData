using System;
using System.Collections;

namespace CustomData.Overrides
{
    public class CDFacade
    {
        public event Action EventOnBuildingNameGenStrategyChanged;
        internal void CallBuildingNameGenStrategyChangedEvent() => BuildingManager.instance.StartCoroutine(CallBuildRenamedEvent_impl());
        private IEnumerator CallBuildRenamedEvent_impl()
        {
            yield return 0;
            EventOnBuildingNameGenStrategyChanged?.Invoke();
        }
    }
}

using ICities;
using UnityEngine;

namespace DataCollection
{

    public class ModLoad : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {

        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            Log.Message("Initializing");
            base.OnLevelLoaded(mode);
            
        }
    }
    public class DataCollection : LoadingExtensionBase, IUserMod
    {
        public static GameObject gameObject;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }

            if (gameObject != null)
            {
                return;
            }
            gameObject = new GameObject("DataCollectionObject");
            gameObject.AddComponent<DataCollectionLogic>();
        }
        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (gameObject == null)
            {
                return;
            }
            UnityEngine.Object.Destroy(gameObject);
            gameObject = null;
        }

        public string Name
        {
            get { return "Data Collection"; }
        }

        public string Description
        {
            get { return "Allows recording and collection of data"; }
        }


    }
}

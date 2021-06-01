namespace UnityEssentials.SceneWorkflow
{
    /// <summary>
    ///     Singleton to retrieve dependencies
    /// </summary>
    public class DependencyInjectorSingleton : DependencyInjectorComponent
    {
        public static DependencyInjectorSingleton Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        protected void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

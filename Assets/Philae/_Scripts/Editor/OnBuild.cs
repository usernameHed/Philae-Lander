#if UNITY_EDITOR

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// handle build
/// </summary>
class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // Do the preprocessing here
    }
}

#endif
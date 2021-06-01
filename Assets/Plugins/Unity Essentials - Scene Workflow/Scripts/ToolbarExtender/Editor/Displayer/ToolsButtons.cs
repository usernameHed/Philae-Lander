using UnityEditor;

namespace UnityEssentials.SceneWorkflow.toolbarExtent
{
    [InitializeOnLoad]
    public class ToolsButton
    {
        private static SceneWorkflowButtons _sceneWorkflowButtons = new SceneWorkflowButtons();

        static ToolsButton()
        {
            _sceneWorkflowButtons.Init();
            ToolbarExtender.LeftToolbarGUI.Add(_sceneWorkflowButtons.DisplaySliderLeft);
            ToolbarExtender.RightToolbarGUI.Add(_sceneWorkflowButtons.DisplaySliderRight);
        }
    }
}
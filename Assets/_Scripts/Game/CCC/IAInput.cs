using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
[TypeInfoBox("Player input")]
public class IAInput : EntityAction
{
    [FoldoutGroup("Object"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    protected IAController iaController;
    public IAController IAController { get { return (iaController); } }

    /// <summary>
    /// tout les input du jeu, à chaque update
    /// </summary>
    private void GetInput()
    {
        //all axis
        moveInput = new Vector2(ExtRandom.GetRandomNumber(-1f, 1f), ExtRandom.GetRandomNumber(-1f, 1f));

        //all button
        Jump = ExtRandom.GetRandomBool();
        JumpUp = false;
    }

    private void Update()
    {
        GetInput();
    }
}

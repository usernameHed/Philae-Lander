using AiUnity.Common.Attributes;
using System.Collections.Generic;
using AiUnity.Common.Tags;

/// <Summary>
/// Provide strongly typed access to Unity tags.
/// <Summary>
[GeneratedType("17/03/2019 16:03:19")]
public class TagAccess : ITagAccess
{
	public const string Untagged = "Untagged";
	public const string Respawn = "Respawn";
	public const string Finish = "Finish";
	public const string EditorOnly = "EditorOnly";
	public const string MainCamera = "MainCamera";
	public const string Player = "Player";
	public const string GameController = "GameController";
	public const string Floor = "Floor";
	public const string Enemy = "Enemy";
	public const string GravityAttractor = "GravityAttractor";
	public const string Object = "Object";

	private static readonly List<string> tagPaths = new List<string>()
	{
		"Untagged",
		"Respawn",
		"Finish",
		"EditorOnly",
		"MainCamera",
		"Player",
		"GameController",
		"Floor",
		"Enemy",
		"GravityAttractor",
		"Object"
	};

	public IEnumerable<string> TagPaths { get { return tagPaths.AsReadOnly(); } }


	public enum TagAccessEnum
	{
	Untagged,
	Respawn,
	Finish,
	EditorOnly,
	MainCamera,
	Player,
	GameController,
	Floor,
	Enemy,
	GravityAttractor,
	Object,
	}
}


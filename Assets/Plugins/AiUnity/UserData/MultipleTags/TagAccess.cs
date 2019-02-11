using AiUnity.Common.Attributes;
using System.Collections.Generic;
using AiUnity.Common.Tags;

/// <Summary>
/// Provide strongly typed access to Unity tags.
/// <Summary>
[GeneratedType("10/02/2019 22:13:22")]
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
	public const string T1 = "T1";
	public const string T3 = "T3";
	public const string T2 = "T2";

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
		"T1",
		"T3/Color.Red",
		"T1/T2",
		"Color.Red/Color.Blue",
		"T2/T3"
	};

	public IEnumerable<string> TagPaths { get { return tagPaths.AsReadOnly(); } }

	public class Color
	{
		public const string Red = "Color.Red";
		public const string Blue = "Color.Blue";

		public static string Any()
		{
			return "Color";
		}
	}
}


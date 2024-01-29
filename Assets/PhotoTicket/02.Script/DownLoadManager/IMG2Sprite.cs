using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class IMG2Sprite : MonoBehaviour
{
	private static IMG2Sprite _instance;

	public static IMG2Sprite instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.

			if (_instance == null)
				_instance = GameObject.FindObjectOfType<IMG2Sprite>();
			return _instance;
		}
	}

	public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
	{
		// Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
		Texture2D SpriteTexture = LoadTexture(FilePath);

		if (SpriteTexture == null)
		{
			return null;
		}

		Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

		return NewSprite;
	}

	public Texture2D LoadTexture(string FilePath)
	{
		Texture2D Tex2D = null;
		byte[] FileData;

		try
		{
			if (File.Exists(FilePath))
			{
				FileData = File.ReadAllBytes(FilePath);
				Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
				if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
					return null;                 // If data = readable -> return texture
			} else
			{
				return null; // Return null if load failed
			}                  
		} catch (Exception e)
		{
			Debug.LogError("텍스처 로딩 중 예외발생");
			return null;
		}

		return Tex2D;
	}
}
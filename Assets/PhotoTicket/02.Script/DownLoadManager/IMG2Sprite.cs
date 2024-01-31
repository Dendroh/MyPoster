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
		try
		{
			if(File.Exists(FilePath))
			{
				Texture2D SpriteTexture = LoadTexture(FilePath);

				if (SpriteTexture == null)
				{
					print("포스터 이미지 파일로드 중 예외발생 filePath : " + FilePath);
					return null;
				}
				Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

				return NewSprite;
			} else
			{
				print("포스터 이미지 파일이 존재하지 않는 예외발생 filePath : " + FilePath);
				return null;
			}

		}
		catch
		{
			print("포스터 이미지 텍스처 로딩 중 예외발생 filePath : " + FilePath);
			return null;
		}
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
				Tex2D = new Texture2D(2, 2);
				if (!Tex2D.LoadImage(FileData))
				{
					print("포스터 이미지 파일로드 중 예외발생 filePath : " + FilePath);
					return null;
				}
			} else
			{
				print("포스터 이미지 파일이 존재하지 않는 예외발생 filePath : " + FilePath);
				return null;
			}
		} catch
		{
			print("포스터 이미지 텍스처 로딩 중 예외발생 filePath : " + FilePath);
			return null;
		}

		return Tex2D;
	}
}
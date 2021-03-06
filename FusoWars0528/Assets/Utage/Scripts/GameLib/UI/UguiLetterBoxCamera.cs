using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// カメラ制御。
	/// あらかじめ想定するゲームの画面サイズを設定し、
	/// 実行環境のデバイスの解像度あわせて画面全体を拡大・縮小するように設定する
	/// 設定した範囲内で表示するアスペクト比を変更し、余白部分はレターボックスで埋める。
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/LetterBoxCamera")]
	public class UguiLetterBoxCamera : MonoBehaviour
	{
		/// <summary>
		/// 2Dカメラの1単位あたりのピクセル数
		/// </summary>
		public int PixelsToUnits { get { return pixelsToUnits; } set{ hasChanged = true; pixelsToUnits = value; }}
		[SerializeField]
		int pixelsToUnits = 100;

		/// <summary>画面サイズ：幅(px)</summary>
		public int Width { get { return width; } set{ hasChanged = true; width = value; }}
		[SerializeField]
		int width = 800;

		/// <summary>画面サイズ：高さ(px)</summary>
		public int Height { get { return height; } set{ hasChanged = true; height = value; }}
		[SerializeField]
		int height = 600;

		/// <summary></summary>
		public bool IsFlexible { get { return isFlexible; } set{ hasChanged = true; isFlexible = value; }}
		[SerializeField]
		bool isFlexible = false;
	
		/// <summary>画面サイズ：幅(px)</summary>
		public int MaxWidth { get { return maxWidth; } set{ hasChanged = true; maxWidth = value; }}
		[SerializeField]
		int maxWidth = 800;
		
		/// <summary>画面サイズ：高さ(px)</summary>
		public int MaxHeight { get { return maxHeight; } set{ hasChanged = true; maxHeight = value; }}
		[SerializeField]
		int maxHeight = 600;

		public int FlexibleMinWidth { get { return IsFlexible ? Mathf.Min(Width, Width, MaxWidth) : Width; } }
		public int FlexibleMinHeight { get { return IsFlexible ? Mathf.Min(Height, Height, MaxHeight) : Height; } }

		public int FlexibleMaxWidth { get { return IsFlexible ? Mathf.Max(Width, Width, MaxWidth) : Width; } }
		public int FlexibleMaxHeight { get { return IsFlexible ? Mathf.Max(Height, Height, MaxHeight) : Height; } }

		public enum AnchorType
		{
			UpperLeft,
			UpperCenter,
			UpperRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			LowerLeft,
			LowerCenter,
			LowerRight
		};

		//レターボックスを使う際に、ゲーム画面を画面中央ではなく、下にくっつける形にする。広告表示などのレイアウトに合わせるために
		[SerializeField]
		AnchorType anchor = AnchorType.MiddleCenter;

		/// <summary>
		/// 現在の画面サイズ：高さ(px)
		/// </summary>
		public int CurrentHeight { get { return currentHeight; } }
		int currentHeight;

		/// <summary>
		/// 現在の画面サイズ：幅(px)
		/// </summary>
		public int CurrentWidth { get { return currentWidth; } }
		int currentWidth;

		float screenAspectRatio;		//デバイススクリーンのアスペクト比
		Vector2 padding;				//レターボックスのために使う、カメラのビューポート矩形の余白部分

		Rect currentRect;				//現在の表示領域矩形

		public Camera CachedCamera { get { return cachedCamera ?? (cachedCamera = this.GetComponent<Camera>()); } }
		Camera cachedCamera;
		bool hasChanged;

		void Start()
		{
			hasChanged = true;
		}

		void OnValidate()
		{
			hasChanged = true;
		}

		void Update()
		{
			if (hasChanged ||
			    (!Mathf.Approximately(screenAspectRatio, 1.0f * Screen.width / Screen.height))
			    )
			{
				Refresh();
			}
		}

		public void Refresh()
		{
			hasChanged = false;
			RefreshCurrentSize();
			RefreshCurrentRect();
			RefreshCamera();
		}

		void RefreshCurrentSize()
		{
			screenAspectRatio = 1.0f * Screen.width / Screen.height;

			float defaultAspectRatio = (float)Width/Height;
			float wideAspectRatio = (float)FlexibleMaxWidth / FlexibleMinHeight;
			float nallowAspectRatio = (float)FlexibleMinWidth / FlexibleMaxHeight;

			//スクリーンのアスペクト比から、ゲームの画面サイズを決める
			if (screenAspectRatio > wideAspectRatio)
			{
				//スクリーンのほうが限界よりも横長なので、左右にレターボックス

				padding.x = (1.0f - wideAspectRatio / screenAspectRatio) / 2;
				padding.y = 0;

				currentWidth = FlexibleMaxWidth;	//横は最大を
				currentHeight = FlexibleMinHeight;	//縦は最小を
			}
			else if (screenAspectRatio < nallowAspectRatio)
			{
				//スクリーンのほうが限界よりも縦長なので、上下にレターボックス
				padding.x = 0;
				padding.y = (1.0f - screenAspectRatio / nallowAspectRatio) / 2;

				currentWidth = FlexibleMinWidth;			//横は最小を
				currentHeight = FlexibleMaxHeight;	//縦は最大を
			}
			else
			{
				//アスペクト比が設定の範囲内ならレターボックスなし
				padding.x = 0;
				padding.y = 0;

				if (Mathf.Approximately(screenAspectRatio, defaultAspectRatio))
				{
					//基本的なアスペクト比と同じ
					currentWidth = Width;
					currentHeight = Height;
				}
				else
				{
					currentHeight = FlexibleMinHeight;
					currentWidth = Mathf.FloorToInt(screenAspectRatio * currentHeight);
					if (currentWidth < FlexibleMinWidth)
					{
						currentWidth = FlexibleMinWidth;
						currentHeight = Mathf.FloorToInt(currentWidth / screenAspectRatio);
					}
				}
			}
		}

		void RefreshCurrentRect()
		{
			float x = padding.x;
			float width = 1- padding.x*2;
			float y = padding.y;
			float height = 1-padding.y*2;

			switch (anchor)
			{
			case AnchorType.UpperLeft:
				x = 0;
				y = padding.y*2;
				break;
			case AnchorType.UpperCenter:
				y = padding.y*2;
				break;
			case AnchorType.UpperRight:
				x = padding.x*2;
				y = padding.y*2;
				break;
			case AnchorType.MiddleLeft:
				x = 0;
				break;
			case AnchorType.MiddleCenter:
				break;
			case AnchorType.MiddleRight:
				x = padding.x*2;
				break;
			case AnchorType.LowerLeft:
				x = 0;
				y = 0;
				break;
			case AnchorType.LowerCenter:
				y = 0;
				break;
			case AnchorType.LowerRight:
				x = padding.x*2;
				y = 0;
				break;
			}
			currentRect = new Rect(x, y, width, height);
		}

		void RefreshCamera()
		{
			CachedCamera.rect = currentRect;
			float camera2DOrthographicSize = (float)currentHeight / (2 * pixelsToUnits);
			CachedCamera.orthographicSize = camera2DOrthographicSize;
		}


		public Texture2D CaptureScreen()
		{
			int x = Mathf.CeilToInt(currentRect.x * Screen.width);
			int y = Mathf.CeilToInt(currentRect.y * Screen.height);
			int width = Mathf.FloorToInt(currentRect.width * Screen.width);
			int height = Mathf.FloorToInt(currentRect.height * Screen.height);
			return UtageToolKit.CaptureScreen (new Rect (x, y, width, height));
		}
	}
}
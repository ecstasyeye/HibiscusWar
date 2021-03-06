//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{


	/// <summary>
	///  子オブジェクトを縦に並べる
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/VerticalAlignGroupScaleEffect")]
	public class UguiVerticalAlignGroupScaleEffect : UguiVerticalAlignGroup
	{
		public float scaleRangeTop = -100f;
		public float scaleRangeHeight = 200f;
		public bool ignoreLocalPositionToScaleEffectRage = true;

		public float minScale = 0.5f;
		public float maxScale = 1f;

		protected override void CustomChild(RectTransform child, float offset )
		{
			tracker.Add(this, child,DrivenTransformProperties.Scale);
			
			float scale = minScale;
			float h = child.rect.height*scale;
			float top = ScaleEffectChildLocalPointTop;
			float bottom = ScaleEffectChildLocalPointBottom;
			if (direction == AlignDirection.BottomToTop)
			{
				bottom -= h;
				if (bottom < offset && offset < top)
				{
					float t = (offset -bottom)/(top-bottom);
					if(t>0.5f) t = 1.0f-t;
					scale =  Mathf.Lerp( minScale, maxScale, t );
				}
			}
			else
			{
				top += h;
				if (bottom < offset && offset < top)
				{
					float t = Mathf.Sin( Mathf.PI*(offset -bottom)/(top-bottom) );
					scale =  Mathf.Lerp( minScale, maxScale, t );
				}
			}
			child.localScale = Vector3.one*scale;
		}
		
		protected override void CustomLayoutRectTransform()
		{
			DrivenTransformProperties properties = DrivenTransformProperties.None;
			properties |= DrivenTransformProperties.AnchorMinY
				| DrivenTransformProperties.AnchorMaxY
					| DrivenTransformProperties.PivotY;
			tracker.Add(this, CachedRectTransform, properties);

			if (direction == AlignDirection.BottomToTop)
			{
				CachedRectTransform.anchorMin = new Vector2(CachedRectTransform.anchorMin.x, 0);
				CachedRectTransform.anchorMax = new Vector2(CachedRectTransform.anchorMax.x, 0);
				CachedRectTransform.pivot = new Vector2(CachedRectTransform.pivot.x, 0);
			}
			else
			{
				CachedRectTransform.anchorMin = new Vector2(CachedRectTransform.anchorMin.x, 1);
				CachedRectTransform.anchorMax = new Vector2(CachedRectTransform.anchorMax.x, 1);
				CachedRectTransform.pivot = new Vector2(CachedRectTransform.pivot.x, 1);
			}
		}

		void OnDrawGizmos ()
		{
			Vector3 top = ScaleEffectWolrdPointTop;
			Vector3 bottom = ScaleEffectWolrdPointBottom;
			Gizmos.DrawLine(top, bottom);
		}

		Vector3 ScaleEffectWolrdPointTop
		{
			get
			{
				Vector3 pos = new Vector3(0,scaleRangeTop,0);
				if( ignoreLocalPositionToScaleEffectRage )
				{
					pos -= CachedRectTransform.localPosition;
				}
				return CachedRectTransform.TransformPoint(pos);
			}
		}

		Vector3 ScaleEffectWolrdPointBottom
		{
			get
			{
				Vector3 pos = new Vector3(0,scaleRangeTop - scaleRangeHeight,0);
				if( ignoreLocalPositionToScaleEffectRage )
				{
					pos -= CachedRectTransform.localPosition;
				}
				return CachedRectTransform.TransformPoint(pos);
			}
		}

		float ScaleEffectChildLocalPointTop
		{
			get
			{
				Vector3 top = ScaleEffectWolrdPointTop;
				return CachedRectTransform.InverseTransformPoint(top).y;
			}
		}
		
		float ScaleEffectChildLocalPointBottom
		{
			get
			{
				Vector3 bottom = ScaleEffectWolrdPointBottom;
				return CachedRectTransform.InverseTransformPoint(bottom).y;
			}
		}

		///// <summary>
		///// 
		///// </summary>
		//public override void Reposition()
		//{
		//	if (CachedRectTransform.childCount <= 0) return;
			
		//	float totalSize = 0;
		//	foreach( RectTransform child in CachedRectTransform )
		//	{
		//		totalSize += child.rect.height * Mathf.Abs(child.localScale.y);
		//	}
		//	totalSize += (CachedRectTransform.childCount -1) * space;
		//	totalSize += paddingTop + paddingBottom;
			
		//	if (isAutoResize)
		//	{
		//		Vector2 pos = CachedRectTransform.anchoredPosition;
		//		tracker.Add(this, CachedRectTransform, DrivenTransformProperties.SizeDeltaY);
		//		CachedRectTransform.sizeDelta = new Vector2( CachedRectTransform.sizeDelta.x, totalSize );
		//	}

		//	float offset;
		//	float anchorY;
		//	float directionScale;
		//	if (direction == AlignDirection.BottomToTop)
		//	{
		//		offset = paddingBottom;
		//		anchorY = 0;
		//		directionScale = 1;
		//	}
		//	else
		//	{
		//		offset = -paddingTop;
		//		anchorY = 1;
		//		directionScale = -1;
		//	}

		//	foreach( RectTransform child in CachedRectTransform )
		//	{
		//		tracker.Add(this, child,
		//					DrivenTransformProperties.AnchorMinY
		//					| DrivenTransformProperties.AnchorMaxY
		//					| DrivenTransformProperties.AnchoredPositionY );
				
		//		child.anchorMin = new Vector2( child.anchorMin.x, anchorY);
		//		child.anchorMax = new Vector2( child.anchorMax.x, anchorY);

		//		float h = child.rect.height * Mathf.Abs(child.localScale.y);
		//		offset += directionScale*(h * child.pivot.y);
		//		child.anchoredPosition = new Vector2( child.anchoredPosition.x, offset );
		//		offset += directionScale*( h * (1.0f - child.pivot.y) + space );
		//	}
		//}
	}
}

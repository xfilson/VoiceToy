// Outline shader variant of "Spine/Special/Skeleton Grayscale"

Shader "Spine/Outline/Special/Skeleton Grayscale" {
	Properties {
		_GrayPhase ("Phase", Range(0, 1)) = 1
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector][MaterialToggle(_USE_SCREENSPACE_OUTLINE_WIDTH)] _UseScreenSpaceOutlineWidth("Width in Screen Space", Float) = 0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector][MaterialToggle(_OUTLINE_FILL_INSIDE)]_Fill("Fill", Float) = 0
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineOpaqueAlpha("Opaque Alpha", Range(0,1)) = 1.0
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
		Blend One OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		UsePass "Spine/Outline/Skeleton/OUTLINE"

		UsePass "Spine/Special/Skeleton Grayscale/NORMAL"

		UsePass "Spine/Special/Skeleton Grayscale/CASTER"
	}
	FallBack "Spine/Special/Skeleton Grayscale"
	CustomEditor "SpineShaderWithOutlineGUI"
}

Shader ".Perso/Always Visible Shadow Without Falloff"
{
    Properties
	{
        _ColorBehind("Always Visible Color", Color) = (0, 0, 0, 1)
        _ClipScale ("Near Clip Sharpness", Float) = 100
        _Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
        _Ambient ("Ambient", Range (0, 1)) = 0.3
        _Offset ("Offset", Range (-1, -10)) = -1.0
    }
    Subshader
	{
        Tags
		{
			"Queue" = "Transparent-1"
		}
        Pass
		{
			ZWrite Off
            ZTest Greater
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex DSPProjectorVertNoFalloff
            #pragma fragment fragShadowBehind
            #include "UnityCG.cginc"
            #include "DSPProjector.cginc"
            uniform fixed4 _ColorBehind;
            fixed4 fragShadowBehind(DSP_V2F_PROJECTOR i) : SV_Target
            {
                fixed4 col = _ColorBehind;
                col.a = DSPGetShadowAlpha(i) * tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow)).a;
                return col;
            }
            ENDCG
        }
        UsePass "DynamicShadowProjector/Projector/Shadow Without Falloff/PASS"
    }
}
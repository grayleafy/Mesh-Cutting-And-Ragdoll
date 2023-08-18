  // Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Grid"  
{  
    Properties  
    {  
          _gridColor("�������ɫ",Color) = (0.5,0.5,0.5)  
        _tickWidth("����ļ��",Range(0.01,1)) = 0.1  
        _gridWidth("����Ŀ��",Range(0.0001,0.01)) = 0.008      
    }  
    SubShader  
    {  
        //ѡȡAlpha��Ϸ�ʽ
        //Blend  SrcAlpha SrcAlpha
        //ȥ���ڵ�����Ȼ���  
        //  Cull Off
        // ZWrite Off  
        //������Ȳ���  
        // ZTest Always  
        CGINCLUDE  

    ENDCG  

    Pass  
    { 
        CGPROGRAM  
        //�ô����ʱ��Ҫע�⣺��CGPROGRAM���͡�#pragma...���е�ƴд��ͬ,�治֪����pragma����ʲô����  
        #pragma vertex vert  
        #pragma fragment frag  

        #include "UnityCG.cginc"  

        uniform float4 _backgroundColor;  
         uniform float4 _gridColor;  
        uniform float _tickWidth;  
        uniform float _gridWidth;  


        struct appdata  
        {  
            float4 vertex:POSITION;  
            float2 uv:TEXCOORD0;  
        };  
        struct v2f  
        {  
            float2 uv:TEXCOORD0;  
            float4 vertex:SV_POSITION;  
        };  
        v2f vert(appdata v)  
        {  
            v2f o;  
            o.vertex = UnityObjectToClipPos(v.vertex);  
            o.uv = v.uv;  
            return o;  
        }  

        float4 frag(v2f i) :COLOR
        {  
            //����������Ĵ����½��ƶ������������  
            float2 r = 2.0*(i.uv - 0.5);  
            float aspectRatio = _ScreenParams.x / _ScreenParams.y;  

            float4 backgroundColor = _backgroundColor;

            float4 gridColor = _gridColor;

            float4 pixel = backgroundColor;
            float a = 0;
            //��������ĵļ��  
            const float tickWidth = _tickWidth;  
            if (fmod(r.x, tickWidth) < _gridWidth)  
            {  
                pixel = gridColor;  

            }  

            if (fmod(r.y, tickWidth) < _gridWidth)  
            {  
                pixel = gridColor;  

            }  

            if (abs(pixel.x) == backgroundColor.x
                && abs(pixel.y) == backgroundColor.y
                && abs(pixel.z) == backgroundColor.z
                && abs(pixel.w) == backgroundColor.w
                )
            {
                discard;
            }

            return pixel;


        }  
        ENDCG  
    }  

}  

}  

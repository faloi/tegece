float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matWorldInverseTranspose;

float time = 0;
float textureOffset;

// Textura y sampler de textura
texture base_Tex;
sampler2D baseMap =
sampler_state
{
   Texture = (base_Tex);
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   MIPFILTER = LINEAR;
   AddressU = MIRROR;
   AddressV = MIRROR;
};

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float4 Color : COLOR0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position :        POSITION0;
   float2 Texcoord :        TEXCOORD0;
   float4 Color :			COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj);
   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   Output.Color = Input.Color;

   return( Output );
   
}

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	// Obtener el texel de textura
	// baseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D( baseMap, Texcoord );
	return 1*fvBaseColor + 0*Color;
}


//Vertex Shader (efecto de "agua")
//(mueve la coordenada u)
VS_OUTPUT vs_mainTextureOffset ( VS_INPUT Input )
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.Position = mul(Input.Position, matWorldViewProj);
	Out.Color = Input.Color;
	Out.Texcoord = float2(Input.Texcoord.x*1000,Input.Texcoord.y*100);
	Out.Texcoord[0] += time *2* textureOffset;
	
	return Out;
}

//Vertex Shader (efecto de "agua")
//(mueve la coordenada v)
VS_OUTPUT vs_mainTextureOffset2 ( VS_INPUT Input )
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.Position = mul(Input.Position, matWorldViewProj);
	Out.Color = Input.Color;
	Out.Texcoord = float2(Input.Texcoord.x*1000,Input.Texcoord.y*100);
	Out.Texcoord[1] += time *2* textureOffset;
	
	return Out;
}

// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
	  AlphaBlendEnable = false;
	  SrcBlend = One;
	  DestBlend = Zero;
	  VertexShader = compile vs_2_0 vs_mainTextureOffset();
	  PixelShader = compile ps_2_0 ps_main();
   }
   pass Pass_1
   {
	  AlphaBlendEnable = true;
	  SrcBlend = DestColor;
	  DestBlend = Zero;
	  VertexShader = compile vs_2_0 vs_mainTextureOffset2();
	  PixelShader = compile ps_2_0 ps_main();
   }
}

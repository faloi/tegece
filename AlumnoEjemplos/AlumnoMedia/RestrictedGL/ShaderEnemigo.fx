float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;
float4x4 matWorldInverseTranspose;

float time = 0;
bool isBlocked = false;

// Textura y sampler de textura
texture base_Tex;
sampler2D baseMap =
sampler_state
{
   Texture = (base_Tex);
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   MIPFILTER = LINEAR;
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
   float4 ModPos : TEXCOORD01;
   float4 Color :			COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   Output.ModPos = Input.Position;

   //Animar posicion
   Input.Position.x += sin(time*10) * sign(Input.Position.x) * 0.3;
   Input.Position.z += sin(time*10) * sign(Input.Position.z) * 0.3;
   
   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj);
   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   Output.Color = Input.Color;
   Output.Color.g = 0;
   Output.Color.b = 0;
   
   if (!isBlocked) {
	  Output.Color.r = 0;
   } else {
      Output.Color.r = 50;
   }      

   return( Output );
   
}

//Pixel Shader
float4 ps_mov( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0,float4 ModPos : TEXCOORD01) : COLOR0
{      
	// Obtener el texel de textura
	// baseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D( baseMap, Texcoord );
	if(fvBaseColor.a < 0.99)
	{
		if(Texcoord.y > 0.879)
		{
			if(ModPos.y > 17)
				Texcoord.x = (Texcoord.x + time*0.1)%0.4;
			else
				Texcoord.x = abs((Texcoord.x - time*0.1)%0.4);
		}
		else
		{
			if(ModPos.y > 17)
				Texcoord.y = ((Texcoord.y + time*0.1)%(0.84-0.425)) + 0.425;
			else
				Texcoord.y = abs(((Texcoord.y - time*0.1)%(0.84-0.425))) + 0.425;
		}
	}
	fvBaseColor = tex2D( baseMap, Texcoord );
	return 0.4*fvBaseColor + 0.6*Color;
}

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0,float4 ModPos : TEXCOORD01) : COLOR0
{      
	// Obtener el texel de textura
	// baseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D( baseMap, Texcoord );

	if(fvBaseColor.a < 0.99) {
		if(Texcoord.y > 0.879) {
			if(ModPos.y > 17) {
				Texcoord.x = (Texcoord.x + time*0.1)%0.4;
			} else {
				Texcoord.x = abs((Texcoord.x - time*0.1)%0.4);
			}
			
		} else {
			if(ModPos.y > 17) {
				Texcoord.y = ((Texcoord.y + time*0.1)%(0.84-0.425)) + 0.425;
			} else {
				Texcoord.y = abs(((Texcoord.y - time*0.1)%(0.84-0.425))) + 0.425;
			}
		}
	}

	fvBaseColor = tex2D( baseMap, Texcoord );
	return 0.4*fvBaseColor + 0.6*Color;
}

// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
	  AlphaBlendEnable = false;
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }
}

technique RenderSceneMovement
{
   pass Pass_0
   {
	  AlphaBlendEnable = false;
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_mov();
   }
}

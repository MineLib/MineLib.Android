float4x4	World;
float4x4	View;
float4x4	Projection;

float3		CameraPosition;

float		TimeOfDay;

float4		SunColor;          

Texture CubeTexture;
sampler CubeTextureSampler = sampler_state
{
	Texture		=	<CubeTexture>;
	
	MipFilter	=	POINT;
	MinFilter	=	POINT;
	MagFilter	=	POINT;
	
	AddressU	=	WRAP;
	AddressV	=	WRAP;
};

struct VertexShaderInput
{
	float4	Position	:	SV_POSITION;
	float2	TexCoords	:	TEXCOORD0;
	float	SunLight	:	COLOR0;
	float3	LocalLight	:	COLOR1;
};

struct PixelShaderInput
{
	float4	Position	:	SV_POSITION;
	float2	TexCoords	:	TEXCOORD0;
	float4	Color		:	COLOR0;
};


PixelShaderInput VertexShaderFunction(VertexShaderInput input)
{
	PixelShaderInput output;

	float4 worldPosition	=	mul(input.Position, World);
	float4 viewPosition		=	mul(worldPosition, View);
	output.Position			=	mul(viewPosition, Projection);

	output.TexCoords		=	input.TexCoords;

	float4 sColor			=	SunColor;

	if(TimeOfDay <= 12)
	{
		sColor				*=	TimeOfDay / 12;    
	}
	else
	{
		sColor				*=	(TimeOfDay - 24) / -12;    
	}

	output.Color.rgb		=	(sColor * input.SunLight);// + (input.LocalLight.rgb * 1.5);
	output.Color.a			= 	1.0f;

	return output;
}


float4 PixelShaderFunction(PixelShaderInput input) : SV_TARGET
{
	float4 texColor		=	tex2D(CubeTextureSampler, input.TexCoords);

	float4 color;
	color.rgb			=	texColor.rgb * input.Color.rgb;
	color.a				=	texColor.a;

	if(color.a == 0)
	{
		clip(-1);
	}

	return color;
}

technique BlockTechnique
{
	pass Pass1
	{
		VertexShader	=	compile vs_2_0	VertexShaderFunction();
		PixelShader		=	compile ps_2_0	PixelShaderFunction();
	}
}
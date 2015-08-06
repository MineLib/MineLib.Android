float4x4	World;
float4x4	View;
float4x4	Projection;

float3		CameraPosition;

float		TimeOfDay;

float4		SunColor;       


texture		Texture;
sampler diffuseSampler = sampler_state
{
    Texture		=	<Texture>;
	
	MipFilter	=	POINT;
	MinFilter	=	POINT;
	MagFilter	=	POINT;
	
	AddressU	=	WRAP;
	AddressV	=	WRAP;
};

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture		=	<SpecularMap>;
	
	MipFilter	=	POINT;
	MinFilter	=	POINT;
	MagFilter	=	POINT;
	
	AddressU	=	WRAP;
	AddressV	=	WRAP;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture		=	<NormalMap>;
	
	MipFilter	=	POINT;
	MinFilter	=	POINT;
	MagFilter	=	POINT;
	
	AddressU	=	WRAP;
	AddressV	=	WRAP;
};

struct VertexShaderInput
{
	float4	Position	:	POSITION0;
	float3	Normal		:	NORMAL0;
	float2	TexCoord	:	TEXCOORD0;
	float	SunLight	:	COLOR0;
	float3	LocalLight	:	COLOR1;
	float3	Binormal	:	BINORMAL0;
	float3	Tangent		:	TANGENT0;
};

struct VertexShaderOutput
{
	float4		Position		:	POSITION0;
	float4		Color			:	COLOR0;
	float2		TexCoord		:	TEXCOORD0;
	float2		Depth			:	TEXCOORD1;
	float3x3	tangentToWorld	:	TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	
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

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.TexCoord, World);

    return output;
}
struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
    float4 texColor = tex2D(diffuseSampler, input.TexCoord);
    
    float4 specularAttributes = tex2D(specularSampler, input.TexCoord);
    //specular Intensity
	output.Color.rgb			=	texColor.rgb * input.Color.rgb;
    //output.Color.a = specularAttributes.r;
	output.Color.a				=	texColor.a;
    
    // read the normal from the normal map
    float3 normalFromMap = tex2D(normalSampler, input.TexCoord);
    //tranform to [-1,1]
    normalFromMap = 2.0f * normalFromMap - 1.0f;
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);

    //specular Power
    output.Normal.a = specularAttributes.a;

    output.Depth = input.Depth.x / input.Depth.y;
    return output;
}
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

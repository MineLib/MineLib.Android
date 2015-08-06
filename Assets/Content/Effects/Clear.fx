//Vertex Shader
float4 VS(float3 Position : POSITION0) : POSITION0
{
return float4(Position, 1);
}
//Pixel Shader Out
struct PSO
{
float4 Albedo : COLOR0;
float4 Normals : COLOR1;
float4 Depth : COLOR2;
};
//Normal Encoding Function
half3 encode(half3 n)
{
n = normalize(n);
n.xyz = 0.5f * (n.xyz + 1.0f);
return n;
}
//Pixel Shader
PSO PS()
{
//Initialize Output
PSO output;
//Clear Albedo to Transperant Black
output.Albedo = 0.0f;
//Clear Normals to 0(encoded value is 0.5 but can't use normalize on 0, compile error)
output.Normals.xyz = 0.5f;
output.Normals.w = 0.0f;
//Clear Depth to 1.0f
output.Depth = 1.0f;
//Return
return output;
}
//Technique
technique Default
{
pass p0
{
VertexShader = compile vs_3_0 VS();
PixelShader = compile ps_3_0 PS();
}
}
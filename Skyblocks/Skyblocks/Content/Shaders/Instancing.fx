#define MAX_SHADER_MATRICES 60

// Camera
float4x4 View;
float4x4 Projection;

float3 LightDirection = normalize(float3(-1, -1, -1));
float3 DiffuseLight = 1.25;
float3 AmbientLight = 0.25;

texture Texture;

sampler Sampler = sampler_state
{
	Texture = (Texture);
	MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderCommon(VertexShaderInput input, float4x4 instanceTransform)
{
	VertexShaderOutput output;
	
	float4 worldPos = mul(input.Position, instanceTransform);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	
	// Simple lambert shading
	float3 worldNormal = mul(input.Normal, instanceTransform);
	
	float diffuseAmt = max(-dot(worldNormal, LightDirection), 0);
	
	float3 lightingRes = saturate(diffuseAmt * DiffuseLight + AmbientLight);
	
	output.Color = float4(lightingRes, 1);
	
	output.TextureCoordinate = input.TextureCoordinate;
	
	return output;
}

VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input, float4x4 instanceTransform : TEXCOORD1)
{
	return VertexShaderCommon(input, transpose(instanceTransform));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(Sampler, input.TextureCoordinate) * input.Color;
}

technique HardwareInstancing
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
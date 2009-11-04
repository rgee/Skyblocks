float4x4 matWorldViewProj;
float4x4 matInverseWorld;
float4   vLightDirection;

texture2D DiffuseTexture;

sampler2D DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	MagFilter = Linear;
	MinFilter = Anisotropic;
	MipFilter = Linear;
	MaxAnisotropy = 16;
};

struct VertexShaderOutput
{
    float4 Pos : POSITION;
    float3 L   : TEXCOORD0;
    float3 N   : TEXCOORD1;
    float2 TextureCoordinate : TEXCOORD2;
	
};

VertexShaderOutput VertexShaderFunction(float4 Pos: POSITION, float2 TextureCoordinate: TEXCOORD0, float3 N: NORMAL)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Pos = mul(Pos, matWorldViewProj);
    output.L = normalize(vLightDirection);
    output.N = normalize(mul(matInverseWorld, N));
    output.TextureCoordinate = TextureCoordinate;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput IN) : COLOR
{
	// Basic diffuse lighting
	
	float Ai = 0.8f;
	float4 Ac = float4(0.075, 0.075, 0.2, 1.0);
	float Di = 1.0f;
	float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
	
	float4 texel = tex2D(DiffuseSampler, IN.TextureCoordinate);
	
	return float4((Ai * Ac + Di * Dc * saturate(dot(IN.L, IN.N))*texel.xyz),texel.w);
}

technique DiffuseLight
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}

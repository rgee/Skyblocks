matrix matView : VIEW;
matrix matProj : PROJECTION;

texture Texture0 < string type = "CUBE"; string name = "skybox02.dds"; >;

sampler linear_sampler = sampler_state
{
	Texture = (Texture0);
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	ADDRESSU = Clamp;
	ADDRESSV = Clamp;
};


void VS(in float3 v0 : POSITION,
		out float4 oPos : POSITION,
		out float3 oT0 : TEXCOORD0)
{
	float4x4 matViewNoTrans=
	{
		matView[0],
		matView[1],
		matView[2],
		float4( 0.f, 0.f, 0.f, 1.f)
	};
	
	oPos = mul(float4(v0, 1.f), mul(matViewNoTrans, matProj));
	
	oT0 = v0;
} 

void PS(in float3 t0: TEXCOORD0,
		out float4 r0 : COLOR0)
{
	r0 = texCUBE(linear_sampler, t0);
}

technique Technique0
{
    pass Pass0
    {
        
        VertexShader = compile vs_1_1 VS();
        PixelShader = compile ps_1_4 PS();
        
        ZEnable = FALSE;
        ZWriteEnable = FALSE;
        AlphaBlendEnable = FALSE;
        CullMode = CCW;
        AlphaTestEnable = FALSE;
    }
}

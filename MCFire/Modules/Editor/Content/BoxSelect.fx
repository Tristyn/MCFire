// -----------------------------------------------------
// Vertex Projection
// -----------------------------------------------------
matrix TransformMatrix;
float2 MainShift;
float2 MainScale;

struct VSOut
{
	float4 pos : SV_Position;
	float2 texPos : TEXCOORD;
};

VSOut VSProjection(float4 position : SV_Position, float2 texPos : TEXCOORD)
{
	//project
	VSOut output = (VSOut)0;
	output.pos = mul(position, TransformMatrix);
	// shift it here so that the rasterizer can take care of wrapping
	output.texPos = (texPos + MainShift) * MainScale;
	return output;
}

// -----------------------------------------------------
// Light PS
// -----------------------------------------------------
Texture2D Main;
SamplerState Sampler;

float4 PSTexture(VSOut input) : SV_TARGET
{
	// sample texture, shift then scale the coordinate
	return Main.Sample(Sampler, input.texPos);
}

// -----------------------------------------------------
// Techniques
// -----------------------------------------------------
technique VertexLitTechnique
{
	pass
	{
		Profile = 9.1;
		VertexShader = VSProjection;
		PixelShader = PSTexture;
	}
}
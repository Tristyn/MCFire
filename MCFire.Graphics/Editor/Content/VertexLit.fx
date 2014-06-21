// -----------------------------------------------------
// Vertex Projection
// -----------------------------------------------------
matrix TransformMatrix;

struct ProjectionOut
{
	float4  position : SV_Position;
	float4  color : COLOR0;
};

ProjectionOut VSProjection(float4 position : SV_Position, float4 color : COLOR0)
{
	ProjectionOut output = (ProjectionOut)0;

	//project
	output.position = mul(position, TransformMatrix);

	output.color = color;

	return output;
}

// -----------------------------------------------------
// Light PS
// -----------------------------------------------------
//SamplerState PointSampler;
//float LightLevel;

float4 PSLightLevel(ProjectionOut input) : SV_TARGET
{
	// sample the texture
	//float4 color = Texture.Sample(PointSampler, input.position);

	// apply luminance
	/*float4 color;
	color.rgb = input.color.rgb;
	color.a = 1;*/

	return input.color;
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
		PixelShader = PSLightLevel;
	}
}
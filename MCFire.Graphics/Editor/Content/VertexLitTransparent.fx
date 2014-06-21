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
float Opacity;

float4 PSLightLevel(ProjectionOut input) : SV_TARGET
{
	input.color.a = Opacity;

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
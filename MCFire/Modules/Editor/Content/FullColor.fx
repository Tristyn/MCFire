// -----------------------------------------------------
// Vertex Projection
// -----------------------------------------------------
matrix TransformMatrix;

struct ProjectionOut
{
	float4  position : SV_Position;
	float4  color : COLOR0;
};

float4 VSProjection(float4 position : SV_Position) : SV_Position
{
	//project
	return mul(position, TransformMatrix);
}

// -----------------------------------------------------
// Light PS
// -----------------------------------------------------
float4 Color;

float4 PSLightLevel(ProjectionOut input) : SV_TARGET
{
	return Color;
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
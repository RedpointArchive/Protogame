// -----------------------------------------------------------------------------
// This is a template for a color-based shader.  It is written to provide the
// minimal amount of code such that it can be built upon (for custom lighting effects).
// -----------------------------------------------------------------------------

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION;
    float4 Color : PROTOGAME_TARGET(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float4 Color : PROTOGAME_TARGET(0);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Color = input.Color;

    return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	output.Color = input.Color;
    
    return output;
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DefaultPixelShader();
	}
}
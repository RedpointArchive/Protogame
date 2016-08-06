// -----------------------------------------------------------------------------
// This shader clears the geometry buffer in deferred rendering.
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#include "Macros.inc"
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float3 Position : PROTOGAME_POSITION(0);
};

struct VertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
	float4 Normal : PROTOGAME_TARGET(1);
	float4 Depth : PROTOGAME_TARGET(2);
	float4 Specular : PROTOGAME_TARGET(3);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position, 1);
	return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Black color.
	output.Color = 0.0f;

	// Alpha channel is unused.
	output.Color.a = 0.0f;

	// When transforming 0.5f into [-1, 1], we will get 0.0f.
	output.Normal.rgb = 0.5f;
	
	// Alpha channel is unused.
	output.Normal.a = 0.0f;

	// Maximum depth.
	output.Depth = 1.0f;
	
	// Specular channel to 0.
	output.Specular = float4(0, 0, 0, 0);

	return output;
}

technique Deferred
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DefaultPixelShader();
	}
}
// -----------------------------------------------------------------------------
// A very basic render for geometry buffer testing.  Real shaders should support
// deferred shading directly by declaring a technique with the name "Deferred".
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#include "Macros.inc"
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Color : PROTOGAME_TARGET(0);
};

struct VertexShaderInputBatched
{
	float4 Position : PROTOGAME_POSITION(0);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Color : PROTOGAME_TARGET(0);
	float4x4 InstanceWorld : PROTOGAME_TEXCOORD(1);
};

struct VertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float3 Color : PROTOGAME_TARGET(0);
	float3 Normal : PROTOGAME_TEXCOORD(0);
	float2 Depth : PROTOGAME_TEXCOORD(1);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
	float4 Normal : PROTOGAME_TARGET(1);
	float4 Depth : PROTOGAME_TARGET(2);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Color = input.Color;
	output.Normal = mul(float4(input.Normal.xyz, 0), World);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

VertexShaderOutput DefaultVertexShaderBatched(VertexShaderInputBatched input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, input.InstanceWorld);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Color = input.Color;
	output.Normal = mul(float4(input.Normal.xyz, 0), World);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Output the RGB color.
	output.Color.rgb = input.Color;

	// Alpha channel is unused.
	output.Color.a = 0.0f;

	// Transform normal.
	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);

	// Alpha channel is unused.
	output.Normal.a = 0.0f;

	// Calculate depth.
	output.Depth = input.Depth.x / input.Depth.y;

	return output;
}

technique RENDER_PASS_TYPE_DEFERRED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DefaultPixelShader();
	}
}

technique RENDER_PASS_TYPE_DEFERRED_BATCHED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DefaultVertexShaderBatched();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DefaultPixelShader();
	}
}
// -----------------------------------------------------------------------------
// A basic gaussian blur shader.
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#include "Macros.inc"
#endif

PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

// The pixel size in texels, that is, (1 divided by the texture width, 1 divided by the texture height).
float PixelWidth;
float PixelHeight;

static const float PixelKernel[9] =
{
	-4,
	-3,
	-2,
	-1,
	0,
	1,
	2,
	3,
	4,
};

static const float BlurWeights[81] =
{
	0,	0.000001,	0.000014,	0.000055,	0.000088,	0.000055,	0.000014,	0.000001,	0,
	0.000001,	0.000036,	0.000362,	0.001445,	0.002289,	0.001445,	0.000362,	0.000036,	0.000001,
	0.000014,	0.000362,	0.003672,	0.014648,	0.023205,	0.014648,	0.003672,	0.000362,	0.000014,
	0.000055,	0.001445,	0.014648,	0.058434,	0.092566,	0.058434,	0.014648,	0.001445,	0.000055,
	0.000088,	0.002289,	0.023205,	0.092566,	0.146634,	0.092566,	0.023205,	0.002289,	0.000088,
	0.000055,	0.001445,	0.014648,	0.058434,	0.092566,	0.058434,	0.014648,	0.001445,	0.000055,
	0.000014,	0.000362,	0.003672,	0.014648,	0.023205,	0.014648,	0.003672,	0.000362,	0.000014,
	0.000001,	0.000036,	0.000362,	0.001445,	0.002289,	0.001445,	0.000362,	0.000036,	0.000001,
	0,	0.000001,	0.000014,	0.000055,	0.000088,	0.000055,	0.000014,	0.000001,	0,
};

struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
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

	output.TexCoord = input.TexCoord;

	return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	float4 color = 0;
	
	// TODO: This is an incredibly naive guassian blur implementation, which is not
	// optimal on the GPU at all.
	for (int i = 0; i < 81; i++)
	{
		color += PROTOGAME_SAMPLE_TEXTURE(
			Texture,
			input.TexCoord + float2(PixelKernel[i % 9] * PixelWidth, PixelKernel[i / 9] * PixelHeight)) * BlurWeights[i];
	}

	output.Color = color;

	return output;
}

technique RENDER_PASS_TYPE_POSTPROCESS
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DefaultPixelShader();
	}
}
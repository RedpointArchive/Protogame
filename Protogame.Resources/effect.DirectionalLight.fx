// -----------------------------------------------------------------------------
// A shader which provides directional lighting.
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#include "Macros.inc"
#endif

PROTOGAME_DECLARE_TEXTURE(Color) = sampler_state
{
	Texture = (Color);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
};

PROTOGAME_DECLARE_TEXTURE(Depth) = sampler_state
{
	Texture = (Depth);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
};

PROTOGAME_DECLARE_TEXTURE(Normal) = sampler_state
{
	Texture = (Normal);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
};

float3 LightDirection;
float3 LightColor;
float2 HalfPixel;

float2 ScreenDimensions;

float4x4 World;
float4x4 View;
float4x4 Projection;

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

VertexShaderOutput ForwardVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;

	return output;
}

PixelShaderOutput ForwardPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Get the normal data from the normal map.
	float4 normalRaw = PROTOGAME_SAMPLE_TEXTURE(Normal, input.TexCoord);

	// Transform normal back into [-1, 1] range.
	float3 normal = 2.0f * normalRaw.xyz - 1.0f;

	// Calculate light vector.
	float3 lightVector = -normalize(LightDirection);

	// Compute diffuse light.
	float NdL = max(0, dot(normal, lightVector));
	float3 diffuseLight = NdL * LightColor.rgb;

	output.Color = float4(diffuseLight, 0);

	return output;
}

technique RENDER_PASS_TYPE_DEFERRED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER ForwardPixelShader();
	}
}
// -----------------------------------------------------------------------------
// A shader which combines the color map and light maps in deferred shading.
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

PROTOGAME_DECLARE_TEXTURE(DiffuseLight) = sampler_state
{
	Texture = (DiffuseLight);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
};

PROTOGAME_DECLARE_TEXTURE(SpecularLight) = sampler_state
{
	Texture = (SpecularLight);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
};

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

	// Sample diffuse color.
	float3 diffuseColor = PROTOGAME_SAMPLE_TEXTURE(Color, input.TexCoord).rgb;

	// Sample diffuse light color.
	float3 diffuseLight = PROTOGAME_SAMPLE_TEXTURE(DiffuseLight, input.TexCoord).rgb;

	// Sample specular light color.
	float3 specularLight = PROTOGAME_SAMPLE_TEXTURE(SpecularLight, input.TexCoord).rgb;

	// Set the calculated light.
	output.Color = float4((diffuseColor * diffuseLight + specularLight), 1);

	return output;
}

technique RENDER_PASS_TYPE_DEFERRED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DefaultPixelShader();
	}
}
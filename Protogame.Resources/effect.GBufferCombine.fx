// -----------------------------------------------------------------------------
// A shader which combines the color map and light maps in deferred shading.
// -----------------------------------------------------------------------------

PROTOGAME_DECLARE_TEXTURE(Color) = sampler_state
{
	Texture = (Color);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
};

PROTOGAME_DECLARE_TEXTURE(Light) = sampler_state
{
	Texture = (Light);
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
	float4 diffuseColor = PROTOGAME_SAMPLE_TEXTURE(Color, input.TexCoord);

	// Sample light color.
	float4 light = PROTOGAME_SAMPLE_TEXTURE(Light, input.TexCoord);

	// Get the color component of the light.
	float3 diffuseLight = light.rgb;

	// Alpha channel of light is unused.

	// Set the calculated light.
	output.Color = float4((diffuseColor.rgb * diffuseLight), 1);

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
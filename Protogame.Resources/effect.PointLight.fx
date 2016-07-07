// -----------------------------------------------------------------------------
// A shader which provides point lighting.
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

float3 LightPosition;
float3 LightColor;
float LightRadius;
float LightIntensity;
float4x4 LightInvertViewProjection;

float2 ScreenDimensions;

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
};

struct VertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float4 ScreenPosition : PROTOGAME_TEXCOORD(0);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput DeferredVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.ScreenPosition = output.Position;

	return output;
}

PixelShaderOutput DeferredPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	// Obtain screen position.
	input.ScreenPosition.xy /= input.ScreenPosition.w;

	// Obtain texture coordinates corresponding to the current pixel.
	// The screen coordinates are in [-1,1]*[1,-1].
	// The texture coordinates need to be in [0,1]*[0,1].
	float2 texCoord = 0.5f * (float2(input.ScreenPosition.x, -input.ScreenPosition.y) + 1);

	// Get the normal data from the normal map.
	float4 normalRaw = PROTOGAME_SAMPLE_TEXTURE(Normal, texCoord);

	// Transform normal back into [-1, 1] range.
	float3 normal = 2.0f * normalRaw.xyz - 1.0f;

	// Read depth.
	float depthVal = PROTOGAME_SAMPLE_TEXTURE(Depth, texCoord).r;

	// Compute screen-space position.
	float4 position;
	position.xy = input.ScreenPosition.xy;
	position.z = depthVal;
	position.w = 1.0f;

	// Transform to world space.
	position = mul(position, LightInvertViewProjection);
	position /= position.w;

	// Calculate surface to light vector.
	float3 lightVector = LightPosition - position;

	// Compute attenuation based on distance (linear attenuation).
	float attenuation = saturate(1.0f - length(lightVector) / LightRadius);

	// Normalize light vector.
	lightVector = normalize(lightVector);

	// Compute diffuse light.
	float NdL = max(0, dot(normal, lightVector));
	float3 diffuseLight = NdL * LightColor.rgb;

	output.Color = attenuation * LightIntensity * float4(diffuseLight.rgb, 0);

	return output;
}

technique RENDER_PASS_TYPE_DEFERRED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DeferredVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DeferredPixelShader();
	}
}
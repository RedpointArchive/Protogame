// -----------------------------------------------------------------------------
// This is a template for a color-based shader.  It is written to provide the
// minimal amount of code such that it can be built upon (for custom lighting effects).
// -----------------------------------------------------------------------------

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION_INPUT;
	float4 Normal : PROTOGAME_NORMAL(0);
    float4 Color : PROTOGAME_TARGET(0);
};

// ----------------- Forward Lighting -----------------

struct ForwardVertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION_STATE;
    float4 Color : PROTOGAME_TARGET(0);
};

struct ForwardPixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

ForwardVertexShaderOutput ForwardVertexShader(VertexShaderInput input)
{
	ForwardVertexShaderOutput output;
    
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Color = input.Color;

    return output;
}

ForwardPixelShaderOutput ForwardPixelShader(ForwardVertexShaderOutput input)
{
	ForwardPixelShaderOutput output;

	output.Color = input.Color;
    
    return output;
}

technique RENDER_PASS_TYPE_FORWARD
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER ForwardPixelShader();
	}
}

// ----------------- Deferred Lighting -----------------

struct DeferredVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_STATE;
	float3 Color : PROTOGAME_TARGET(0);
	float3 Normal : PROTOGAME_TEXCOORD(0);
	float2 Depth : PROTOGAME_TEXCOORD(1);
};

struct DeferredPixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
	float4 Normal : PROTOGAME_TARGET(1);
	float4 Depth : PROTOGAME_TARGET(2);
};

DeferredVertexShaderOutput DeferredVertexShader(VertexShaderInput input)
{
	DeferredVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Color = input.Color;
	output.Normal = mul(float4(input.Normal.rgb, 0), World);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredPixelShaderOutput DeferredPixelShader(DeferredVertexShaderOutput input)
{
	DeferredPixelShaderOutput output;

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
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DeferredVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DeferredPixelShader();
	}
}
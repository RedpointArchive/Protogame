PROTOGAME_DECLARE_TEXTURE(Texture);
PROTOGAME_DECLARE_TEXTURE(NormalMap);

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
};

struct VertexShaderInputBatched
{
	float4 Position : PROTOGAME_POSITION(0);
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
	float4x4 InstanceWorld : PROTOGAME_TEXCOORD(1);
};

// ----------------- Forward Lighting -----------------

struct ForwardVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
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
	output.Normal = mul(float4(input.Normal.rgb, 0), World);
	output.Tangent = mul(float4(input.Tangent.rgb, 0), World);
	output.Binormal = mul(float4(input.Binormal.rgb, 0), World);
	output.TexCoord = input.TexCoord;

	return output;
}

ForwardPixelShaderOutput ForwardPixelShader(ForwardVertexShaderOutput input)
{
	ForwardPixelShaderOutput output;

	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord);

	return output;
}

technique RENDER_PASS_TYPE_FORWARD
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER ForwardPixelShader();
	}
}

// ----------------- Deferred Lighting -----------------

struct DeferredVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float2 Depth : PROTOGAME_TEXCOORD(1);
	float4 Normal : PROTOGAME_NORMAL(0);
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
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

	output.TexCoord = input.TexCoord;
	output.Normal = mul(float4(input.Normal.rgb, 0), World);
	output.Tangent = mul(float4(input.Tangent.rgb, 0), World);
	output.Binormal = mul(float4(input.Binormal.rgb, 0), World);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredVertexShaderOutput DeferredVertexShaderBatched(VertexShaderInputBatched input)
{
	DeferredVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, input.InstanceWorld);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;
	output.Normal = mul(float4(input.Normal.rgb, 0), input.InstanceWorld);
	output.Tangent = mul(float4(input.Tangent.rgb, 0), input.InstanceWorld);
	output.Binormal = mul(float4(input.Binormal.rgb, 0), input.InstanceWorld);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredPixelShaderOutput DeferredPixelShader(DeferredVertexShaderOutput input)
{
	DeferredPixelShaderOutput output;

	// Output the RGB color.
	output.Color.rgb = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord).rgb;

	// Alpha channel is unused.
	output.Color.a = 0.0f;

	// Transform normal.
	float3 normalMap = PROTOGAME_SAMPLE_TEXTURE(NormalMap, input.TexCoord).rgb;
	normalMap = (normalMap * 2.0f) - 1.0f;
	output.Normal.rgb = normalize(
		(normalMap.x * input.Tangent) + 
		(normalMap.y * input.Binormal) +
		(normalMap.z * input.Normal));

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
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DeferredVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DeferredPixelShader();
	}
}

technique RENDER_PASS_TYPE_DEFERRED_BATCHED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER DeferredVertexShaderBatched();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER DeferredPixelShader();
	}
}

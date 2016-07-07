// -----------------------------------------------------------------------------
// This is a template for a skinning shader that would apply to models.  It is written
// to provide the minimal amount of code such that it can be built upon (for custom
// lighting effects).
//
// This shader supports rendering models with a single diffuse texture.
// -----------------------------------------------------------------------------

PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

static const int MAX_BONES = 48;

float4x4 Bones[MAX_BONES];

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION(0);
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
    float4 Normal : PROTOGAME_NORMAL(0);
    float4 BoneWeights : PROTOGAME_BLENDWEIGHT(0);
    uint4 BoneIndices : PROTOGAME_BLENDINDICES(0);
};

// ----------------- Forward Lighting -----------------

struct ForwardVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 Normal : PROTOGAME_TEXCOORD(1);
};

struct ForwardPixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

ForwardVertexShaderOutput ForwardVertexShader(VertexShaderInput input)
{
	ForwardVertexShaderOutput output;

	float4 correctedPosition = float4(input.Position.xyz, 1);
	float4 correctedNormal = float4(input.Normal.xyz, 0);

	float4x4 boneMatrix1 = Bones[input.BoneIndices.x];
	float4x4 boneMatrix2 = Bones[input.BoneIndices.y];
	float4x4 boneMatrix3 = Bones[input.BoneIndices.z];
	float4x4 boneMatrix4 = Bones[input.BoneIndices.w];

	float boneWeight1 = input.BoneWeights.x;
	float boneWeight2 = input.BoneWeights.y;
	float boneWeight3 = input.BoneWeights.z;
	float boneWeight4 = input.BoneWeights.w;

	float4 computedPosition = correctedPosition;
	float4 computedNormal = correctedNormal;

	// Don't perform bone weighting if all weights are 0 (because that means
	// there's no bones that impact this at all).
	if (boneWeight1 + boneWeight2 + boneWeight3 + boneWeight4 > 0.001)
	{
		computedPosition =
			(mul(correctedPosition, boneMatrix1) * boneWeight1) +
			(mul(correctedPosition, boneMatrix2) * boneWeight2) +
			(mul(correctedPosition, boneMatrix3) * boneWeight3) +
			(mul(correctedPosition, boneMatrix4) * boneWeight4);

		computedNormal =
			(mul(correctedNormal, boneMatrix1) * boneWeight1) +
			(mul(correctedNormal, boneMatrix2) * boneWeight2) +
			(mul(correctedNormal, boneMatrix3) * boneWeight3) +
			(mul(correctedNormal, boneMatrix4) * boneWeight4);
	}

	float4 worldPosition = mul(computedPosition, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Normal = computedNormal;

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
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER ForwardPixelShader();
	}
}

// ----------------- Deferred Lighting -----------------

struct DeferredVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float4 Normal : PROTOGAME_TEXCOORD(1);
	float2 Depth : PROTOGAME_TEXCOORD(2);
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

	float4 correctedPosition = float4(input.Position.xyz, 1);
	float4 correctedNormal = float4(input.Normal.xyz, 0);

	float4x4 boneMatrix1 = Bones[input.BoneIndices.x];
	float4x4 boneMatrix2 = Bones[input.BoneIndices.y];
	float4x4 boneMatrix3 = Bones[input.BoneIndices.z];
	float4x4 boneMatrix4 = Bones[input.BoneIndices.w];

	float boneWeight1 = input.BoneWeights.x;
	float boneWeight2 = input.BoneWeights.y;
	float boneWeight3 = input.BoneWeights.z;
	float boneWeight4 = input.BoneWeights.w;

	float4 computedPosition = correctedPosition;
	float4 computedNormal = correctedNormal;

	// Don't perform bone weighting if all weights are 0 (because that means
	// there's no bones that impact this at all).
	if (boneWeight1 + boneWeight2 + boneWeight3 + boneWeight4 > 0.001)
	{
		computedPosition =
			(mul(correctedPosition, boneMatrix1) * boneWeight1) +
			(mul(correctedPosition, boneMatrix2) * boneWeight2) +
			(mul(correctedPosition, boneMatrix3) * boneWeight3) +
			(mul(correctedPosition, boneMatrix4) * boneWeight4);

		computedNormal =
			(mul(correctedNormal, boneMatrix1) * boneWeight1) +
			(mul(correctedNormal, boneMatrix2) * boneWeight2) +
			(mul(correctedNormal, boneMatrix3) * boneWeight3) +
			(mul(correctedNormal, boneMatrix4) * boneWeight4);
	}

	float4 worldPosition = mul(computedPosition, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Normal = mul(float4(computedNormal.xyz, 0), World);

	output.TexCoord = input.TexCoord;

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
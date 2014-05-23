// -----------------------------------------------------------------------------
// This is a template for a skinning shader that would apply to models.  It is written
// to provide the minimal amount of code such that it can be built upon (for custom
// lighting effects).
//
// Note that this shader has no support for color-based vertexes; it expects rendering
// to be performed purely with textures and UVs.
// -----------------------------------------------------------------------------

PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

static const int MAX_BONES = 48;

float4x4 Bones[MAX_BONES];

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
    float4 Normal : PROTOGAME_NORMAL(0);
    float4 BoneWeights : PROTOGAME_BLENDWEIGHT(0);
    uint4 BoneIndices : PROTOGAME_BLENDINDICES(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
    float4 Normal : PROTOGAME_TEXCOORD(1);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
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

    float4 computedPosition = 
        (mul(correctedPosition, boneMatrix1) * boneWeight1) +
        (mul(correctedPosition, boneMatrix2) * boneWeight2) +
        (mul(correctedPosition, boneMatrix3) * boneWeight3) +
        (mul(correctedPosition, boneMatrix4) * boneWeight4);
    
    float4 computedNormal = 
        (mul(correctedNormal, boneMatrix1) * boneWeight1) +
        (mul(correctedNormal, boneMatrix2) * boneWeight2) +
        (mul(correctedNormal, boneMatrix3) * boneWeight3) +
        (mul(correctedNormal, boneMatrix4) * boneWeight4);

    float4 worldPosition = mul(computedPosition, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Normal = computedNormal;

    output.TexCoord = input.TexCoord;

    return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord);
    
    return output;
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DefaultPixelShader();
	}
}
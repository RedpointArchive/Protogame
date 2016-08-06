// uber Color										: HAS_VERTEX_COLOR
// uber ColorSkinned								: HAS_VERTEX_COLOR;HAS_BONES
// uber Diffuse										: HAS_GLOBAL_DIFFUSE
// uber DiffuseSkinned								: HAS_GLOBAL_DIFFUSE;HAS_BONES
// uber Image										: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS
// uber ImageTiled									: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS;HAS_TILED_TEXTURE_MAP
// uber Texture										: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP
// uber TextureSkinned								: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_BONES
// uber TextureNormal								: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP
// uber TextureNormalSkinned						: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_BONES
// uber TextureNormalSpecIntMapColDef				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntMapColDefSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecIntConColDef				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntConColDefSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecIntMapColCon				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntMapColConSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecIntConColCon				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntConColConSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecIntMapColMap				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntMapColMapSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_MAP;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecIntConColMap				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER
// uber TextureNormalSpecIntConColMapSkinned		: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_INTENSITY_CONSTANT;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER;HAS_BONES

#if defined(__INTELLISENSE__)
// NOTE: Place defines here while debugging uber variants.
#include "Macros.inc"
#endif

// Calculates whether the shader has normals on the input vertex based
// on the configuration.
#if defined(HAS_NORMAL_MAP)
#define HAS_VERTEX_NORMAL 1
#elif !defined(HAS_NO_NORMALS)
#define HAS_VERTEX_NORMAL 1
#endif

// Calculates whether the vertex shader passes a color or a texture coordinate
// on it's output.
#if defined(HAS_VERTEX_COLOR)
#define HAS_SAMPLED_COLOR 1
#elif defined(HAS_GLOBAL_DIFFUSE)
#define HAS_SAMPLED_COLOR 1
#endif


/**********************************************************************************/
/************************ PARAMETER DECLARATIONS AND MACROS ***********************/
/**********************************************************************************/


// Declares texture parameters, based on what maps the shader has enabled.
#if defined(HAS_TEXTURE_MAP)
#if defined(HAS_TILED_TEXTURE_MAP)
PROTOGAME_DECLARE_TEXTURE(Texture) = sampler_state
{
	Texture = (Texture);
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
};
#else
PROTOGAME_DECLARE_TEXTURE(Texture);
#endif
#endif
#if defined(HAS_NORMAL_MAP)
PROTOGAME_DECLARE_TEXTURE(NormalMap);
#endif
#if defined(HAS_SPECULAR_INTENSITY_MAP)
PROTOGAME_DECLARE_TEXTURE(SpecularIntensityMap);
#endif
#if defined(HAS_SPECULAR_COLOR_MAP)
PROTOGAME_DECLARE_TEXTURE(SpecularColorMap);
#endif

// Declares standard world-view-projection matrices.
float4x4 World;
float4x4 View;
float4x4 Projection;

// Declare relevant specular parameters.
#if defined(HAS_SPECULAR_INTENSITY_CONSTANT)
float4 SpecularIntensity;
#endif
#if defined(HAS_SPECULAR_COLOR_CONSTANT)
float4 SpecularColor;
#endif
#if defined(HAS_SPECULAR_POWER)
float4 SpecularPower;
#endif

// Declares the global diffuse parameters.
#if defined(HAS_GLOBAL_DIFFUSE)
float4 ColorDiffuse;
#endif

// Declares the bone parameters if the shader has bones turned on.
#if defined(HAS_BONES)
static const int MAX_BONES = 48;
float4x4 Bones[MAX_BONES];
#endif

/**********************************************************************************/
/****************************** VERTEX SHADER MACROS ******************************/
/**********************************************************************************/


// Declares macros for usage within COMPUTE_BONES_OR_ASSIGN, where we 
// need to optionally compute normals based on whether or not the
// shader accept normals on the input vertex.
#if defined(HAS_VERTEX_NORMAL)
#define DECLARE_CORRECTED_NORMAL \
	float4 correctedNormal = float4(input.Normal.xyz, 0);
#define DECLARE_CORRECTED_NORMAL_UNMODIFIED \
	float4 computedNormal = input.Normal;
#define ASSIGN_CORRECTED_NORMAL_INIT \
	float4 computedNormal = correctedNormal;
#define COMPUTE_CORRECTED_NORMAL \
	computedNormal = \
		(mul(correctedNormal, boneMatrix1) * boneWeight1) + \
		(mul(correctedNormal, boneMatrix2) * boneWeight2) + \
		(mul(correctedNormal, boneMatrix3) * boneWeight3) + \
		(mul(correctedNormal, boneMatrix4) * boneWeight4);
#else
#define DECLARE_CORRECTED_NORMAL 
#define DECLARE_CORRECTED_NORMAL_UNMODIFIED 
#define ASSIGN_CORRECTED_NORMAL_INIT 
#define COMPUTE_CORRECTED_NORMAL 
#endif

// Declares COMPUTE_BONES_OR_ASSIGN, which either does the computation
// for bone transformations, or assigns the values directly based on the
// input if the shader does not have bones enabled.
#if defined(HAS_BONES)
#define COMPUTE_BONES_OR_ASSIGN \
	float4 correctedPosition = float4(input.Position.xyz, 1); \
	DECLARE_CORRECTED_NORMAL \
	\
	float4x4 boneMatrix1 = Bones[input.BoneIndices.x]; \
	float4x4 boneMatrix2 = Bones[input.BoneIndices.y]; \
	float4x4 boneMatrix3 = Bones[input.BoneIndices.z]; \
	float4x4 boneMatrix4 = Bones[input.BoneIndices.w]; \
    \
	float boneWeight1 = input.BoneWeights.x; \
	float boneWeight2 = input.BoneWeights.y; \
	float boneWeight3 = input.BoneWeights.z; \
	float boneWeight4 = input.BoneWeights.w; \
    \
	float4 computedPosition = correctedPosition; \
	ASSIGN_CORRECTED_NORMAL_INIT \
	\
	if (boneWeight1 + boneWeight2 + boneWeight3 + boneWeight4 > 0.001) \
	{ \
		computedPosition = \
			(mul(correctedPosition, boneMatrix1) * boneWeight1) + \
			(mul(correctedPosition, boneMatrix2) * boneWeight2) + \
			(mul(correctedPosition, boneMatrix3) * boneWeight3) + \
			(mul(correctedPosition, boneMatrix4) * boneWeight4); \
		\
		COMPUTE_CORRECTED_NORMAL \
	}
#else
#define COMPUTE_BONES_OR_ASSIGN \
	float4 computedPosition = input.Position; \
	DECLARE_CORRECTED_NORMAL_UNMODIFIED
#endif

// Declares COMPUTE_ASSIGNMENT_COLOR, which emits a color assignment
// if the shader has vertex colors or global diffuse enabled.
#if defined(HAS_VERTEX_COLOR)
#define COMPUTE_ASSIGNMENT_COLOR output.Color = input.Color;
#elif defined(HAS_GLOBAL_DIFFUSE)
#define COMPUTE_ASSIGNMENT_COLOR output.Color = ColorDiffuse;
#else
#define COMPUTE_ASSIGNMENT_COLOR 
#endif

// Declares COMPUTE_ASSIGNMENT_PRIMARY_UV_COORDS, which emits a
// texture coordinate assignment if the shader has UV coordinates enabled.
#if defined(HAS_VERTEX_PRIMARY_UV_COORDS)
#define COMPUTE_ASSIGNMENT_PRIMARY_UV_COORDS output.TexCoord = input.TexCoord;
#else
#define COMPUTE_ASSIGNMENT_PRIMARY_UV_COORDS 
#endif

// Declares COMPUTE_ASSIGNMENT_VERTEX_NORMAL, which emits a
// normal assignment if the shader has UV coordinates enabled.
#if defined(HAS_VERTEX_NORMAL)
#define COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD) output.Normal = mul(float4(computedNormal.xyz, 0), _WORLD);
#else
#define COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD)
#endif

// Declares COMPUTE_ASSIGNMENT_TANGENT_BINORMAL, which emits a
// tangent and binormal assignment if the shader has a normal map enabled.
#if defined(HAS_NORMAL_MAP)
#define COMPUTE_ASSIGNMENT_TANGENT_BINORMAL(_WORLD) \
	output.Tangent = mul(float4(input.Tangent.xyz, 0), _WORLD); \
	output.Binormal = mul(float4(input.Binormal.xyz, 0), _WORLD);
#else
#define COMPUTE_ASSIGNMENT_TANGENT_BINORMAL(_WORLD) 
#endif

// Declares COMPUTE_VERTEX, which combines all previous computations
// for use in each of the vertex shaders.
#define COMPUTE_VERTEX(_WORLD) \
	COMPUTE_BONES_OR_ASSIGN \
	\
	float4 worldPosition = mul(computedPosition, _WORLD); \
	float4 viewPosition = mul(worldPosition, View); \
	output.Position = mul(viewPosition, Projection); \
	\
	COMPUTE_ASSIGNMENT_COLOR \
	COMPUTE_ASSIGNMENT_PRIMARY_UV_COORDS \
	COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD) \
	COMPUTE_ASSIGNMENT_TANGENT_BINORMAL(_WORLD)


/**********************************************************************************/
/********************** SHADER INPUT DECLARATIONS AND MACROS **********************/
/**********************************************************************************/


struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
#if defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
#endif
#if defined(HAS_VERTEX_COLOR)
	float4 Color : PROTOGAME_TARGET(0);
#endif
#if defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
#endif
#if defined(HAS_NORMAL_MAP)
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
#endif
#if defined(HAS_BONES)
	float4 BoneWeights : PROTOGAME_BLENDWEIGHT(0);
	uint4 BoneIndices : PROTOGAME_BLENDINDICES(0);
#endif
};

struct VertexShaderInputBatched
{
	float4 Position : PROTOGAME_POSITION(0);
#if defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
#endif
#if defined(HAS_VERTEX_COLOR)
	float4 Color : PROTOGAME_TARGET(0);
#endif
#if defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
#endif
#if defined(HAS_NORMAL_MAP)
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
#endif
#if defined(HAS_BONES)
	float4 BoneWeights : PROTOGAME_BLENDWEIGHT(0);
	uint4 BoneIndices : PROTOGAME_BLENDINDICES(0);
#endif
	float4x4 InstanceWorld : PROTOGAME_TEXCOORD(1);
};


/**********************************************************************************/
/**************************** FORWARD LIGHTING SHADERS ****************************/
/**********************************************************************************/


struct ForwardVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
#if defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
#endif
#if defined(HAS_SAMPLED_COLOR)
	float4 Color : PROTOGAME_TARGET(0);
#endif
#if defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
#endif
#if defined(HAS_NORMAL_MAP)
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
#endif
};

struct ForwardPixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

ForwardVertexShaderOutput ForwardVertexShader(VertexShaderInput input)
{
	ForwardVertexShaderOutput output;

	COMPUTE_VERTEX(World);

	return output;
}

ForwardVertexShaderOutput ForwardVertexShaderBatched(VertexShaderInputBatched input)
{
	ForwardVertexShaderOutput output;

	COMPUTE_VERTEX(input.InstanceWorld);

	return output;
}

ForwardPixelShaderOutput ForwardPixelShader(ForwardVertexShaderOutput input)
{
	ForwardPixelShaderOutput output;

#if defined(HAS_SAMPLED_COLOR)
	output.Color = input.Color;
#elif defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	output.Color = float4(PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord).rgb, 1);
#else
	output.Color = float4(1, 0, 0, 1);
#endif

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

technique RENDER_PASS_TYPE_FORWARD_BATCHED
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER ForwardVertexShaderBatched();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER ForwardPixelShader();
	}
}

technique RENDER_PASS_TYPE_CANVAS
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER ForwardPixelShader();
	}
}

technique RENDER_PASS_TYPE_BATCHED2D
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER ForwardPixelShader();
	}
}

technique RENDER_PASS_TYPE_DIRECT2D
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_HIGH_SHADER ForwardVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_HIGH_SHADER ForwardPixelShader();
	}
}


/**********************************************************************************/
/**************************** DEFERRED LIGHTING SHADERS ***************************/
/**********************************************************************************/


struct DeferredVertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
#if defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
	float2 Depth : PROTOGAME_TEXCOORD(1);
#else
	float2 Depth : PROTOGAME_TEXCOORD(0);
#endif
#if defined(HAS_SAMPLED_COLOR)
	float4 Color : PROTOGAME_TARGET(0);
#endif
#if defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
#endif
#if defined(HAS_NORMAL_MAP)
	float4 Binormal : PROTOGAME_BINORMAL(0);
	float4 Tangent : PROTOGAME_TANGENT(0);
#endif
};

struct DeferredPixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
	float4 Normal : PROTOGAME_TARGET(1);
	float4 Depth : PROTOGAME_TARGET(2);
	float4 Specular : PROTOGAME_TARGET(3);
};

DeferredVertexShaderOutput DeferredVertexShader(VertexShaderInput input)
{
	DeferredVertexShaderOutput output;

	COMPUTE_VERTEX(World);

	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredVertexShaderOutput DeferredVertexShaderBatched(VertexShaderInputBatched input)
{
	DeferredVertexShaderOutput output;

	COMPUTE_VERTEX(input.InstanceWorld);

	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredPixelShaderOutput DeferredPixelShader(DeferredVertexShaderOutput input)
{
	DeferredPixelShaderOutput output;

	// Output the RGB color.
#if defined(HAS_SAMPLED_COLOR)
	output.Color.rgb = input.Color.rgb;
#elif defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	output.Color.rgb = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord).rgb;
#endif

	// Alpha channel is unused.
	output.Color.a = 0.0f;

#if defined(HAS_NORMAL_MAP)
	// Transform normal.
	float3 normalMap = PROTOGAME_SAMPLE_TEXTURE(NormalMap, input.TexCoord).rgb;
	normalMap = (normalMap * 2.0f) - 1.0f;
	output.Normal.rgb = normalize(
		(normalMap.x * input.Tangent) + 
		(normalMap.y * input.Binormal) +
		(normalMap.z * input.Normal));
#elif !defined(HAS_NO_NORMALS)
	// Transform normal.
	output.Normal.rgb = (0.5f * (normalize(input.Normal) + 1.0f)).rgb;
#else
	output.Normal.rgb = float3(0, 0, 0);
#endif

#if defined(HAS_SPECULAR_POWER)
	// Alpha channel is used to store specular power.
	output.Normal.a = SpecularPower;
#else
	// Alpha channel is unused.
	output.Normal.a = 0.0f;
#endif

	// Calculate depth.
	output.Depth = input.Depth.x / input.Depth.y;

#if defined(HAS_SPECULAR_INTENSITY_CONSTANT)
	// Use the global specular intensity.
	float specularIntensity = SpecularIntensity;
#elif defined(HAS_SPECULAR_INTENSITY_MAP)
	// Sample specular intensity from the specular intensity map.
	float specularIntensity = PROTOGAME_SAMPLE_TEXTURE(SpecularIntensityMap, input.TexCoord).r;
#else
	// No specular intensity.
	float specularIntensity = 0;
#endif

#if defined(HAS_SPECULAR_COLOR_DEFAULT)
	// Use default specular color of white.
	output.Specular = float4(float3(1, 1, 1) * specularIntensity, 1);
#elif defined(HAS_SPECULAR_COLOR_CONSTANT)
	// Use the global specular color.
	output.Specular = float4(SpecularColor.rgb * specularIntensity, 1);
#elif defined(HAS_SPECULAR_COLOR_MAP)
	// Sample specular color from the specular color map.
	output.Specular = float4(PROTOGAME_SAMPLE_TEXTURE(SpecularColorMap, input.TexCoord).rgb * specularIntensity, 1);
#else
	// No specular color is set.
	output.Specular = float4(0, 0, 0, 0);
#endif

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

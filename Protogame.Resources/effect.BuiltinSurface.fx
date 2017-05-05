// uber Color										: HAS_VERTEX_COLOR
// uber ColorSkinned								: HAS_VERTEX_COLOR;HAS_BONES
// uber Diffuse										: HAS_GLOBAL_DIFFUSE
// uber DiffuseSkinned								: HAS_GLOBAL_DIFFUSE;HAS_BONES
// uber Image										: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS
// uber ImageTiled									: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS;HAS_TILED_TEXTURE_MAP
// uber ImageBlended								: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS;USE_TEXTURE_ALPHA
// uber ImageBlendedTiled							: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NO_NORMALS;HAS_TILED_TEXTURE_MAP;USE_TEXTURE_ALPHA
// uber Texture										: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP
// uber TextureSkinned								: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_BONES
// uber TextureNormal								: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP
// uber TextureNormalSkinned						: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_BONES
// uber TextureNormalSpecColDef						: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER
// uber TextureNormalSpecColDefSkinned				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_DEFAULT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecColCon						: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER
// uber TextureNormalSpecColConSkinned				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_CONSTANT;HAS_SPECULAR_POWER;HAS_BONES
// uber TextureNormalSpecColMap						: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER
// uber TextureNormalSpecColMapSkinned				: HAS_VERTEX_PRIMARY_UV_COORDS;HAS_TEXTURE_MAP;HAS_NORMAL_MAP;HAS_SPECULAR_COLOR_MAP;HAS_SPECULAR_POWER;HAS_BONES

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
#if defined(HAS_SPECULAR_COLOR_MAP)
PROTOGAME_DECLARE_TEXTURE(SpecularColorMap);
#endif

// Declares standard world-view-projection matrices.
float4x4 World;
float4x4 View;
float4x4 Projection;

// Declare relevant specular parameters.
#if defined(HAS_SPECULAR_COLOR_CONSTANT)
float4 SpecularColor;
#endif
#if defined(HAS_SPECULAR_POWER)
float SpecularPower;
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
#if defined(HAS_NORMAL_MAP)
#define COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD) \
	output.TangentToWorld = float3x3( \
		mul(float4(input.Tangent.xyz, 0), _WORLD).xyz, \
		mul(float4(input.Binormal.xyz, 0), _WORLD).xyz, \
		mul(float4(input.Normal.xyz, 0), _WORLD).xyz)
#elif defined(HAS_VERTEX_NORMAL)
#define COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD) output.Normal = mul(float4(computedNormal.xyz, 0), _WORLD);
#else
#define COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD)
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
	COMPUTE_ASSIGNMENT_VERTEX_NORMAL(_WORLD)


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
	float4 InstanceWorld1 : PROTOGAME_TEXCOORD(1);
	float4 InstanceWorld2 : PROTOGAME_TEXCOORD(2);
	float4 InstanceWorld3 : PROTOGAME_TEXCOORD(3);
	float4 InstanceWorld4 : PROTOGAME_TEXCOORD(4);
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
#if defined(HAS_NORMAL_MAP)
	float3x3 TangentToWorld : PROTOGAME_TEXCOORD(2);
#elif defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
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

	COMPUTE_VERTEX(float4x4(input.InstanceWorld1, input.InstanceWorld2, input.InstanceWorld3, input.InstanceWorld4));

	return output;
}

ForwardPixelShaderOutput ForwardPixelShader(ForwardVertexShaderOutput input)
{
	ForwardPixelShaderOutput output;

#if defined(HAS_SAMPLED_COLOR)
	output.Color = input.Color;
#elif defined(HAS_VERTEX_PRIMARY_UV_COORDS)
#if defined(USE_TEXTURE_ALPHA)
	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord).rgba;
#else
	output.Color = float4(PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord).rgb, 1);
#endif
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
#if defined(HAS_NORMAL_MAP)
	float3x3 TangentToWorld : PROTOGAME_TEXCOORD(2);
#elif defined(HAS_VERTEX_NORMAL)
	float4 Normal : PROTOGAME_NORMAL(0);
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

	// input.InstanceWorld
	// float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)
	COMPUTE_VERTEX(float4x4(input.InstanceWorld1, input.InstanceWorld2, input.InstanceWorld3, input.InstanceWorld4));

	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

DeferredPixelShaderOutput DeferredPixelShader(DeferredVertexShaderOutput input)
{
	DeferredPixelShaderOutput output;

	// Output the RGB color.
#if defined(HAS_SAMPLED_COLOR)
	output.Color = input.Color;
#elif defined(HAS_VERTEX_PRIMARY_UV_COORDS)
	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord);
#endif

#if defined(HAS_NORMAL_MAP)
	// Transform normal.
	float3 normalMap = PROTOGAME_SAMPLE_TEXTURE(NormalMap, input.TexCoord).xyz;
	normalMap = normalize((normalMap * 2.0f) - 1.0f);
	float3 normalTransformed = mul(normalMap, input.TangentToWorld);
	normalTransformed = normalize(normalTransformed);
	output.Normal.xyz = (0.5f * (normalTransformed + 1.0f));
#elif !defined(HAS_NO_NORMALS)
	// Transform normal.
	output.Normal.rgb = (0.5f * (normalize(input.Normal) + 1.0f)).xyz;
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

#if defined(HAS_SPECULAR_COLOR_DEFAULT)
	// Use default specular color of white.
	output.Specular = float4(1, 1, 1, 1);
#elif defined(HAS_SPECULAR_COLOR_CONSTANT)
	// Use the global specular color.
	output.Specular = float4(SpecularColor.rgb, 1);
#elif defined(HAS_SPECULAR_COLOR_MAP)
	// Sample specular color from the specular color map.
	output.Specular = float4(PROTOGAME_SAMPLE_TEXTURE(SpecularColorMap, input.TexCoord).rgb, 1);
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

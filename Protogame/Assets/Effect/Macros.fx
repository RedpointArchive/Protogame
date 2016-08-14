#line 1 "Macros.fx"
// -----------------------------------------------------------------------------
// These are utility macros that are provided to all effects compiled under Protogame.
// They are used to target different shader profiles depending on whether compilation
// is for DX11 or OpenGL.
//
// Some of these macros are from the MonoGame default effects.
//
// This file is included automatically as a literal prefix for all effect code.
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#define PLATFORM_WINDOWS 1
#endif

// Keep this in sync with RenderPipelineTechniqueName.
#define RENDER_PASS_TYPE_FORWARD Forward
#define RENDER_PASS_TYPE_FORWARD_BATCHED ForwardBatched
#define RENDER_PASS_TYPE_DEFERRED Deferred
#define RENDER_PASS_TYPE_DEFERRED_BATCHED DeferredBatched
#define RENDER_PASS_TYPE_BATCHED2D Batched2D
#define RENDER_PASS_TYPE_DIRECT2D Direct2D
#define RENDER_PASS_TYPE_CANVAS Canvas
#define RENDER_PASS_TYPE_POSTPROCESS PostProcess

// Declare macros to abstract shader models.
#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINDOWSPHONE8) || defined(PLATFORM_WINDOWSSTOREAPP)
#define PROTOGAME_VERTEX_HIGH_SHADER vs_4_0_level_9_3
#define PROTOGAME_PIXEL_HIGH_SHADER ps_4_0_level_9_3
#define PROTOGAME_VERTEX_LOW_SHADER vs_4_0_level_9_1
#define PROTOGAME_PIXEL_LOW_SHADER ps_4_0_level_9_1
#define PROTOGAME_POSITION(n) POSITION##n
#define PROTOGAME_POSITION_RASTERIZER SV_Position
#define PROTOGAME_DEPTH DEPTH0
#define PROTOGAME_TARGET(n) COLOR##n
#define PROTOGAME_NORMAL(n) NORMAL##n
#define PROTOGAME_TEXCOORD(n) TEXCOORD##n
#define PROTOGAME_BINORMAL(n) BINORMAL##n
#define PROTOGAME_TANGENT(n) TANGENT##n
#define PROTOGAME_BLENDWEIGHT(n) BLENDWEIGHT##n
#define PROTOGAME_BLENDINDICES(n) BLENDINDICES##n

#define PROTOGAME_DECLARE_TEXTURE(Name) \
    Texture2D<float4> Name; \
    sampler Name##Sampler

#define PROTOGAME_DECLARE_CUBEMAP(Name) \
    TextureCube<float4> Name; \
    sampler Name##Sampler

#define PROTOGAME_SAMPLER_NAME(Name) Name##Sampler
#define PROTOGAME_CUBEMAP_NAME(Name) Name##Sampler

#define PROTOGAME_SAMPLE_TEXTURE(Name, texCoord)  (Name.Sample(Name##Sampler, texCoord))
#define PROTOGAME_SAMPLE_CUBEMAP(Name, texCoord)  (Name.Sample(Name##Sampler, texCoord))
#define PROTOGAME_LOAD_TEXTURE(Name, texCoord)  (Name.Load(texCoord))

#else
#define PROTOGAME_VERTEX_HIGH_SHADER vs_3_0
#define PROTOGAME_PIXEL_HIGH_SHADER ps_3_0
#define PROTOGAME_VERTEX_LOW_SHADER vs_2_0
#define PROTOGAME_PIXEL_LOW_SHADER ps_2_0
#define PROTOGAME_POSITION(n) POSITION##n
#define PROTOGAME_POSITION_RASTERIZER POSITION0
#define PROTOGAME_DEPTH DEPTH0
#define PROTOGAME_TARGET(n) COLOR##n
#define PROTOGAME_NORMAL(n) NORMAL##n
#define PROTOGAME_TEXCOORD(n) TEXCOORD##n
#define PROTOGAME_BINORMAL(n) BINORMAL##n
#define PROTOGAME_TANGENT(n) TANGENT##n
#define PROTOGAME_BLENDWEIGHT(n) BLENDWEIGHT##n
#define PROTOGAME_BLENDINDICES(n) BLENDINDICES##n

#define PROTOGAME_DECLARE_TEXTURE(Name) \
    sampler2D Name

#define PROTOGAME_DECLARE_CUBEMAP(Name) \
    samplerCUBE Name

#define PROTOGAME_SAMPLER_NAME(Name) Name
#define PROTOGAME_CUBEMAP_NAME(Name) Name

#define PROTOGAME_SAMPLE_TEXTURE(Name, texCoord)  (tex2D(Name, texCoord))
#define PROTOGAME_SAMPLE_CUBEMAP(Name, texCoord)  (texCUBE(Name, texCoord))
#define PROTOGAME_LOAD_TEXTURE(Name, texCoord)  (tex2D(Name, texCoord))

#endif

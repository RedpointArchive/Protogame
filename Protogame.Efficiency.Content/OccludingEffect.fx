//-----------------------------------------------------------------------------
// OcclusionEffect.fx
//
// Filters rendered occludable sprites using the specified depth texture.
// Renders the provided texture as the content to be drawn in front of\
// occluded sprites.
//
// This file is derived from SpriteEffect.fx which is provided under
// the Microsoft Public License.  Thus this file is also licensed MS-PL.
//-----------------------------------------------------------------------------

#include "Macros.fxh"

// Declare the texture inputs.
DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(DepthTexture, 1);

// Define the matrix transformation input.
BEGIN_CONSTANTS
MATRIX_CONSTANTS
float4x4 MatrixTransform _vs(c0) _cb(c0);
END_CONSTANTS

//------------------------ VERTEX SHADER ---------------------------------------
// Performs simple matrix transformation so that the sprite
// is mapped correctly on screen.
void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float4 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, MatrixTransform);
}

//------------------------ PIXEL SHADER ----------------------------------------
// Reads the Red channel information from each pixel and sets it as the depth
// value at this point.  Occludable sprites will be compared with this
// depth value by the graphics card to determine whether each pixel is drawn.
float4 SpritePixelShader(float4 color : COLOR0,
                         float4 texCoord : TEXCOORD0,
                         out float depth : DEPTH0) : SV_Target0
{
    // Invert depth value read from the texture so that "less" colour in a field
    // is towards the back.
    depth = 1 - SAMPLE_TEXTURE(DepthTexture, float2(texCoord.x, texCoord.y)).r;
    
    return SAMPLE_TEXTURE(Texture, float2(texCoord.x, texCoord.y)) * color;
}

//------------------------ TECHNIQUE -------------------------------------------
// Compile the shaders.
technique OccludingEffect
{
    pass
    {
        VertexShader = compile vs_2_0 SpriteVertexShader();
        PixelShader  = compile ps_2_0 SpritePixelShader();
    }
}

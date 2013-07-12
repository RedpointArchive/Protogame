//-----------------------------------------------------------------------------
// OccludableSpriteEffect.fx
//
// Renders a sprite batch with the addition of Z depth information.
//
// This file is derived from SpriteEffect.fx which is provided under
// the Microsoft Public License.  Thus this file is also licensed MS-PL.
//-----------------------------------------------------------------------------

#include "Macros.fxh"

// Declare the texture inputs.
DECLARE_TEXTURE(Texture, 0);

// Define the matrix transformation input.
BEGIN_CONSTANTS
MATRIX_CONSTANTS
float4x4 MatrixTransform    _vs(c0) _cb(c0);
END_CONSTANTS

//------------------------ VERTEX SHADER ---------------------------------------
// Encodes the Z position information into the additional texture data area
// for retrieval in the pixel shader.  Also performs simple matrix
// transformation so that the sprite is mapped correctly on screen.
void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float4 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, MatrixTransform);

    // Encode depth information in 3rd texture coordinate field.
    texCoord.z = position.z;
}

//------------------------ PIXEL SHADER ----------------------------------------
// Exports the Z data from the additional texture data area to the depth
// buffer.
float4 SpritePixelShader(float4 color : COLOR0,
                         float4 texCoord : TEXCOORD0,
                         out float depth : DEPTH0) : SV_Target0
{
    depth = texCoord.z;

    return SAMPLE_TEXTURE(Texture, float2(texCoord.x, texCoord.y)) * color;
}

//------------------------ TECHNIQUE -------------------------------------------
// Compile the shaders.
technique OccludableSpriteEffect
{
    pass
    {
        VertexShader = compile vs_2_0 SpriteVertexShader();
        PixelShader  = compile ps_2_0 SpritePixelShader();
    }
}

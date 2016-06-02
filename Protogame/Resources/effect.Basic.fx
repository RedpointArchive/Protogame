// -----------------------------------------------------------------------------
// This is a template for a texture-based shader.  It is written to provide the
// minimal amount of code such that it can be built upon (for custom lighting effects).
//
// Note that this shader has no support for color-based vertexes; it expects rendering
// to be performed purely with textures and UVs.
// -----------------------------------------------------------------------------

PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
    float4 Position : PROTOGAME_POSITION;
    float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput ForwardVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;

    return output;
}

PixelShaderOutput ForwardPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

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
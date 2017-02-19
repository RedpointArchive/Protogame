// -----------------------------------------------------------------------------
// A shader which makes the screen more red.
// -----------------------------------------------------------------------------

PROTOGAME_DECLARE_TEXTURE(Texture);

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	float4 Position : PROTOGAME_POSITION(0);
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct VertexShaderOutput
{
	float4 Position : PROTOGAME_POSITION_RASTERIZER;
	float2 TexCoord : PROTOGAME_TEXCOORD(0);
};

struct PixelShaderOutput
{
	float4 Color : PROTOGAME_TARGET(0);
};

VertexShaderOutput DefaultVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;

	return output;
}

PixelShaderOutput DefaultPixelShader(VertexShaderOutput input)
{
	PixelShaderOutput output;

	output.Color = PROTOGAME_SAMPLE_TEXTURE(Texture, input.TexCoord);
	output.Color.r = 1;

	return output;
}

technique RENDER_PASS_TYPE_POSTPROCESS
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER DefaultVertexShader();
		PixelShader = compile PROTOGAME_PIXEL_LOW_SHADER DefaultPixelShader();
	}
}
// -----------------------------------------------------------------------------
// This shader writes depth information to the shadow map to produce
// shadow information for rendering.
// -----------------------------------------------------------------------------

#if defined(__INTELLISENSE__)
#include "Macros.inc"
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 ShadowMapVertexShader(float4 vertexPosition : POSITION) : SV_POSITION
{
	float4x4 modelViewProjectionMatrix = mul(World, mul(View, Projection));
	return mul(vertexPosition, modelViewProjectionMatrix);
}

technique
{
	pass
	{
		VertexShader = compile PROTOGAME_VERTEX_LOW_SHADER ShadowMapVertexShader();
	}
}
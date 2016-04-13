namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A texture packer implementation.
    /// </summary>
    /// <typeparam name="T">The type of texture.</typeparam>
    public class TexturePacker<T>
    {
        /// <summary>
        /// The list of textures to pack.
        /// </summary>
        private List<TextureToPack> m_TexturesToPack = new List<TextureToPack>();

        /// <summary>
        /// Adds a texture to the list of textures to pack.
        /// </summary>
        /// <param name="vector2">The size of the texture.</param>
        /// <param name="p">The texture to pack.</param>
        public void AddTexture(Vector2 vector2, T p)
        {
            this.m_TexturesToPack.Add(new TextureToPack { Size = vector2, Texture = p });
        }

        /// <summary>
        /// Pack the specified textures and return the placed textures, with the
        /// total resulting size set into the <see cref="size"/> parameter.
        /// </summary>
        /// <param name="size">The total size .</param>
        /// <returns>A list of packed textures.</returns>
        public List<PackedTexture<T>> Pack(out Vector2 size)
        {
            var results = new List<PackedTexture<T>>();

            // Create candidate list.
            var candidates = new List<CandidateRegion>();
            candidates.Add(new CandidateRegion
                {
                    Position = new Vector2(0, 0),
                    SizeH = null,
                    SizeW = null
                });

            // Sort pending textures by size.
            var sortedTextures = this.m_TexturesToPack.OrderByDescending(x => x.Size.LengthSquared()).ToList();

            // Pack textures
            while (sortedTextures.Count > 0)
            {
                var texture = sortedTextures[0];

                var maxX = results.Count == 0 ? 0 : results.Select(x => x.Position.X + x.Size.X).Max();
                var maxY = results.Count == 0 ? 0 : results.Select(x => x.Position.Y + x.Size.Y).Max();

                // Evaluate candidate locations
                CandidateRegion bestCandidate = null;
                var minimalIncrease = 100000f;
                foreach (var candidate in candidates)
                {
                    if (candidate.Fits(texture.Size))
                    {
                        var outerX = candidate.Position.X + texture.Size.X;
                        var outerY = candidate.Position.Y + texture.Size.Y;
                        var increaseX = Math.Max(outerX - maxX, 0);
                        var increaseY = Math.Max(outerY - maxY, 0);
                        var maximalIncrease = Math.Max(increaseX, increaseY);

                        var pick = false;

                        if (bestCandidate == null)
                        {
                            pick = true;
                        }
                        else if (maximalIncrease < minimalIncrease)
                        {
                            pick = true;
                        }
                        else if (maximalIncrease == minimalIncrease
                            && candidate.Position.LengthSquared() < bestCandidate.Position.LengthSquared())
                        {
                            pick = true;
                        }

                        if (pick)
                        {
                            bestCandidate = candidate;
                            minimalIncrease = maximalIncrease;
                        }
                    }
                }

                if (bestCandidate == null)
                {
                    throw new InvalidOperationException("Should not get into this state!");
                }

                // Detect candidate interruption
                var outerPosition = bestCandidate.Position + texture.Size;
                foreach (var candidate in candidates.ToList())
                {
                    if (candidate.Position.X < bestCandidate.Position.X && candidate.Position.Y <= outerPosition.Y
                        && (candidate.SizeW == null
                            || candidate.SizeW.Value + candidate.Position.X >= bestCandidate.Position.X))
                    {
                        // Outer position interrupts this candidate's area
                        candidates.Remove(candidate);

                        this.UpdateOrAddCandidate(
                            candidates,
                            new CandidateRegion
                            {
                                Position = candidate.Position,
                                SizeH = candidate.SizeH,
                                SizeW = bestCandidate.Position.X - candidate.Position.X
                            });
                        this.UpdateOrAddCandidate(
                            candidates,
                            new CandidateRegion
                            {
                                Position = new Vector2(bestCandidate.Position.X, outerPosition.Y),
                                SizeH =
                                    candidate.SizeH == null
                                    ? null
                                    : candidate.SizeH - (outerPosition.Y - candidate.Position.Y),
                                SizeW =
                                    candidate.SizeW == null
                                    ? null
                                    : candidate.SizeW - (bestCandidate.Position.X - candidate.Position.X)
                                });
                    }

                    if (candidate.Position.Y < bestCandidate.Position.Y && candidate.Position.X <= outerPosition.X
                        && (candidate.SizeH == null
                            || candidate.SizeH.Value + candidate.Position.Y >= bestCandidate.Position.Y))
                    {
                        // Outer position interrupts this candidate's area
                        candidates.Remove(candidate);

                        this.UpdateOrAddCandidate(
                            candidates,
                            new CandidateRegion
                            {
                                Position = candidate.Position,
                                SizeH = bestCandidate.Position.Y - candidate.Position.Y,
                                SizeW = candidate.SizeW
                            });
                        this.UpdateOrAddCandidate(
                            candidates,
                            new CandidateRegion
                            {
                                Position = new Vector2(bestCandidate.Position.X, outerPosition.Y),
                                SizeH =
                                    candidate.SizeH == null
                                    ? null
                                    : candidate.SizeH - (bestCandidate.Position.Y - candidate.Position.Y),
                                SizeW =
                                    candidate.SizeW == null
                                    ? null
                                    : candidate.SizeW - (outerPosition.X - candidate.Position.X),
                            });
                    }
                }

                // Replace the current candidate with two new candidates and yield the packed texture.
                candidates.Remove(bestCandidate);
                this.UpdateOrAddCandidate(
                    candidates,
                    new CandidateRegion
                    {
                        Position = new Vector2(bestCandidate.Position.X, outerPosition.Y),
                        SizeW = bestCandidate.SizeW,
                        SizeH = bestCandidate.SizeH == null ? null : bestCandidate.SizeH - texture.Size.Y
                    });
                this.UpdateOrAddCandidate(
                    candidates,
                    new CandidateRegion
                    {
                        Position = new Vector2(outerPosition.X, bestCandidate.Position.Y),
                        SizeW = bestCandidate.SizeW == null ? null : bestCandidate.SizeW - texture.Size.X,
                        SizeH = bestCandidate.SizeH
                    });

                results.Add(
                    new PackedTexture<T>
                    {
                        Position = bestCandidate.Position,
                        Size = texture.Size,
                        Texture = texture.Texture
                    });

                sortedTextures.RemoveAt(0);
            }

            // Calculate maximum bounds.
            var finalMaxX = results.Select(x => x.Position.X + x.Size.X).Max();
            var finalMaxY = results.Select(x => x.Position.Y + x.Size.Y).Max();
            size = new Vector2(finalMaxX, finalMaxY);

            return results;
        }

        /// <summary>
        /// Updates or adds a new candidate to the list of candidate region, based on whether
        /// the candidate already exists and whether the existing region is larger or smaller
        /// than the new region.
        /// </summary>
        /// <param name="regions">The existing set of candidate regions.</param>
        /// <param name="candidateRegion">The new candidate region to consider.</param>
        private void UpdateOrAddCandidate(List<CandidateRegion> regions, CandidateRegion candidateRegion)
        {
            var existing =
                regions.FirstOrDefault(
                    x => x.Position.X == candidateRegion.Position.X && x.Position.Y == candidateRegion.Position.Y);
            if (existing == null)
            {
                regions.Add(candidateRegion);
            }
            else
            {
                if (existing.SizeW == null && candidateRegion.SizeW != null)
                {
                    existing.SizeW = candidateRegion.SizeW;
                }
                else if (existing.SizeW > candidateRegion.SizeW)
                {
                    existing.SizeW = candidateRegion.SizeW;
                }

                if (existing.SizeH == null && candidateRegion.SizeH != null)
                {
                    existing.SizeH = candidateRegion.SizeH;
                }
                else if (existing.SizeH > candidateRegion.SizeH)
                {
                    existing.SizeH = candidateRegion.SizeH;
                }
            }
        }

        /// <summary>
        /// Represents a texture that is yet to be packed.
        /// </summary>
        private class TextureToPack
        {
            /// <summary>
            /// Gets or sets the texture size.
            /// </summary>
            /// <value>The texture size.</value>
            public Vector2 Size { get; set; }

            /// <summary>
            /// Gets or sets the texture.
            /// </summary>
            /// <value>The texture.</value>
            public T Texture { get; set; }
        }

        /// <summary>
        /// Represents a candidate region that a texture can be placed in.
        /// </summary>
        private class CandidateRegion
        {
            /// <summary>
            /// Gets or sets the position of a candidate region.
            /// </summary>
            /// <value>The position of a candidate region.</value>
            public Vector2 Position { get; set; }

            /// <summary>
            /// Gets or sets the width of the candidate region, null if infinite.
            /// </summary>
            /// <value>The width of the candidate region.</value>
            public float? SizeW { get; set; }

            /// <summary>
            /// Gets or sets the height of the candidate region, null if infinite.
            /// </summary>
            /// <value>The height of the candidate region.</value>
            public float? SizeH { get; set; }

            /// <summary>
            /// Returns whether the specified size will fit in the candidate region.
            /// </summary>
            /// <param name="size">The specified size to check.</param>
            /// <returns>True if the size will fit, false otherwise.</returns>
            public bool Fits(Vector2 size)
            {
                if ((this.SizeW == null || this.SizeW >= size.X)
                    && (this.SizeH == null || this.SizeH >= size.Y))
                {
                    return true;
                }

                return false;
            }
        }
    }
}

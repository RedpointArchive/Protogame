namespace Protogame
{
    using System;

    /// <summary>
    /// Given four points in 3D space, solves for a sphere such that all four points
    /// lie on the sphere's surface.
    /// </summary>
    /// <remarks>
    /// Translated from Javascript on http://www.convertalot.com/sphere_solver.html, originally
    /// linked to by http://stackoverflow.com/questions/13600739/calculate-centre-of-sphere-whose-surface-contains-4-points-c.
    /// </remarks>
    /// <module>Math</module>
    public class CircumcentreSolver
    {
        /// <summary>
        /// The zero.
        /// </summary>
        private const float ZERO = 0;

        /// <summary>
        /// The p.
        /// </summary>
        private readonly double[,] P =
        {
            { ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO }, 
            { ZERO, ZERO, ZERO }
        };

        /// <summary>
        /// The m_ radius.
        /// </summary>
        private double m_Radius;

        /// <summary>
        /// The m_ x 0.
        /// </summary>
        private double m_X0;

        /// <summary>
        /// The m_ y 0.
        /// </summary>
        private double m_Y0;

        /// <summary>
        /// The m_ z 0.
        /// </summary>
        private double m_Z0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircumcentreSolver"/> class. 
        /// Computes the centre of a sphere such that all four specified points in
        /// 3D space lie on the sphere's surface.
        /// </summary>
        /// <param name="a">
        /// The first point (array of 3 doubles for X, Y, Z).
        /// </param>
        /// <param name="b">
        /// The second point (array of 3 doubles for X, Y, Z).
        /// </param>
        /// <param name="c">
        /// The third point (array of 3 doubles for X, Y, Z).
        /// </param>
        /// <param name="d">
        /// The fourth point (array of 3 doubles for X, Y, Z).
        /// </param>
        public CircumcentreSolver(double[] a, double[] b, double[] c, double[] d)
        {
            this.Compute(a, b, c, d);
        }

        /// <summary>
        /// The centre of the resulting sphere.
        /// </summary>
        /// <value>
        /// The centre.
        /// </value>
        public double[] Centre
        {
            get
            {
                return new[] { this.m_X0, this.m_Y0, this.m_Z0 };
            }
        }

        /// <summary>
        /// The radius of the resulting sphere.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public double Radius
        {
            get
            {
                return this.m_Radius;
            }
        }

        /// <summary>
        /// Whether the result was a valid sphere.
        /// </summary>
        /// <value>
        /// The valid.
        /// </value>
        public bool Valid
        {
            get
            {
                return this.m_Radius != 0;
            }
        }

        /// <summary>
        /// Evaluate the determinant.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="d">
        /// The d.
        /// </param>
        private void Compute(double[] a, double[] b, double[] c, double[] d)
        {
            this.P[0, 0] = a[0];
            this.P[0, 1] = a[1];
            this.P[0, 2] = a[2];
            this.P[1, 0] = b[0];
            this.P[1, 1] = b[1];
            this.P[1, 2] = b[2];
            this.P[2, 0] = c[0];
            this.P[2, 1] = c[1];
            this.P[2, 2] = c[2];
            this.P[3, 0] = d[0];
            this.P[3, 1] = d[1];
            this.P[3, 2] = d[2];

            // Compute result sphere.
            this.Sphere();
        }

        /// <summary>
        /// Recursive definition of determinate using expansion by minors.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double Determinant(double[,] a, double n)
        {
            int i, j, j1, j2;
            double d = 0;
            double[,] m =
            {
                { ZERO, ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO, ZERO }, 
                { ZERO, ZERO, ZERO, ZERO }
            };

            if (n == 2)
            {
                // Terminate recursion.
                d = a[0, 0] * a[1, 1] - a[1, 0] * a[0, 1];
            }
            else
            {
                d = 0;
                for (j1 = 0; j1 < n; j1++)
                {
                    // Do each column.
                    for (i = 1; i < n; i++)
                    {
                        // Create minor.
                        j2 = 0;
                        for (j = 0; j < n; j++)
                        {
                            if (j == j1)
                            {
                                continue;
                            }

                            m[i - 1, j2] = a[i, j];
                            j2++;
                        }
                    }

                    // Sum (+/-)cofactor * minor.
                    d = d + Math.Pow(-1.0, j1) * a[0, j1] * this.Determinant(m, n - 1);
                }
            }

            return d;
        }

        /// <summary>
        /// The sphere.
        /// </summary>
        private void Sphere()
        {
            double m11, m12, m13, m14, m15;
            double[,] a =
            {
                { ZERO, ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO, ZERO }, { ZERO, ZERO, ZERO, ZERO }, 
                { ZERO, ZERO, ZERO, ZERO }
            };

            // Find minor 1, 1.
            for (int i = 0; i < 4; i++)
            {
                a[i, 0] = this.P[i, 0];
                a[i, 1] = this.P[i, 1];
                a[i, 2] = this.P[i, 2];
                a[i, 3] = 1;
            }

            m11 = this.Determinant(a, 4);

            // Find minor 1, 2.
            for (int i = 0; i < 4; i++)
            {
                a[i, 0] = this.P[i, 0] * this.P[i, 0] + this.P[i, 1] * this.P[i, 1] + this.P[i, 2] * this.P[i, 2];
                a[i, 1] = this.P[i, 1];
                a[i, 2] = this.P[i, 2];
                a[i, 3] = 1;
            }

            m12 = this.Determinant(a, 4);

            // Find minor 1, 3.
            for (int i = 0; i < 4; i++)
            {
                a[i, 0] = this.P[i, 0] * this.P[i, 0] + this.P[i, 1] * this.P[i, 1] + this.P[i, 2] * this.P[i, 2];
                a[i, 1] = this.P[i, 0];
                a[i, 2] = this.P[i, 2];
                a[i, 3] = 1;
            }

            m13 = this.Determinant(a, 4);

            // Find minor 1, 4.
            for (int i = 0; i < 4; i++)
            {
                a[i, 0] = this.P[i, 0] * this.P[i, 0] + this.P[i, 1] * this.P[i, 1] + this.P[i, 2] * this.P[i, 2];
                a[i, 1] = this.P[i, 0];
                a[i, 2] = this.P[i, 1];
                a[i, 3] = 1;
            }

            m14 = this.Determinant(a, 4);

            // Find minor 1, 5.
            for (int i = 0; i < 4; i++)
            {
                a[i, 0] = this.P[i, 0] * this.P[i, 0] + this.P[i, 1] * this.P[i, 1] + this.P[i, 2] * this.P[i, 2];
                a[i, 1] = this.P[i, 0];
                a[i, 2] = this.P[i, 1];
                a[i, 3] = this.P[i, 2];
            }

            m15 = this.Determinant(a, 4);

            // Calculate result.
            if (m11 == 0)
            {
                this.m_X0 = 0;
                this.m_Y0 = 0;
                this.m_Z0 = 0;
                this.m_Radius = 0;
            }
            else
            {
                this.m_X0 = 0.5 * m12 / m11;
                this.m_Y0 = -0.5 * m13 / m11;
                this.m_Z0 = 0.5 * m14 / m11;
                this.m_Radius =
                    Math.Sqrt(this.m_X0 * this.m_X0 + this.m_Y0 * this.m_Y0 + this.m_Z0 * this.m_Z0 - m15 / m11);
            }
        }
    }
}
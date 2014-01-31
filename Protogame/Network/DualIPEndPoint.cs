namespace Protogame
{
    using System;
    using System.Net;

    /// <summary>
    /// A representation of an IP address and two ports, a real time port and a reliable port.
    /// </summary>
    public class DualIPEndPoint : IEquatable<DualIPEndPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualIPEndPoint"/> class.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="realtimePort">
        /// The real time port.
        /// </param>
        /// <param name="reliablePort">
        /// The reliable port.
        /// </param>
        public DualIPEndPoint(IPAddress address, int realtimePort, int reliablePort)
            : this(new IPEndPoint(address, realtimePort), new IPEndPoint(address, reliablePort))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualIPEndPoint"/> class.
        /// </summary>
        /// <param name="realtimeEndpoint">
        /// The real time endpoint.
        /// </param>
        /// <param name="reliableEndPoint">
        /// The reliable end point.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the address components of the endpoints are not the same.
        /// </exception>
        public DualIPEndPoint(IPEndPoint realtimeEndpoint, IPEndPoint reliableEndPoint)
        {
            this.RealtimeEndPoint = realtimeEndpoint;
            this.ReliableEndPoint = reliableEndPoint;
        }

        /// <summary>
        /// Gets the real time end point.
        /// </summary>
        /// <value>
        /// The real time end point.
        /// </value>
        public IPEndPoint RealtimeEndPoint { get; set; }

        /// <summary>
        /// Gets the reliable end point.
        /// </summary>
        /// <value>
        /// The reliable end point.
        /// </value>
        public IPEndPoint ReliableEndPoint { get; set; }

        public override string ToString()
        {
            if (this.RealtimeEndPoint != null && this.ReliableEndPoint != null)
            {
                return this.RealtimeEndPoint.Address + ":U" + this.RealtimeEndPoint.Port + ":R" + this.ReliableEndPoint.Port;
            }
            else if (this.RealtimeEndPoint == null)
            {
                return this.ReliableEndPoint.Address + ":R" + this.ReliableEndPoint.Port;
            }
            else if (this.ReliableEndPoint == null)
            {
                return this.RealtimeEndPoint.Address + ":U" + this.RealtimeEndPoint.Port;
            }
            else
            {
                return "(unknown)";
            }
        }

        public bool Equals(DualIPEndPoint other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(this.RealtimeEndPoint, other.RealtimeEndPoint) && Equals(this.ReliableEndPoint, other.ReliableEndPoint);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((DualIPEndPoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.RealtimeEndPoint != null ? this.RealtimeEndPoint.GetHashCode() : 0) * 397) ^ (this.ReliableEndPoint != null ? this.ReliableEndPoint.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DualIPEndPoint left, DualIPEndPoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DualIPEndPoint left, DualIPEndPoint right)
        {
            return !Equals(left, right);
        }
    }
}
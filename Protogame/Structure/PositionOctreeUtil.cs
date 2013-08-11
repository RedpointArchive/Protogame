
namespace Protogame
{
    public static class PositionOctreeUtil
    {
        public static T GetFast64<T>(PositionOctree<T> octree, long x, long y, long z) where T : class
        {
            try
            {
                const long _mask0 = 0x1L << 63;
                const long _mask1 = 0x1L << 62;
                const long _mask2 = 0x1L << 61;
                const long _mask3 = 0x1L << 60;
                const long _mask4 = 0x1L << 59;
                const long _mask5 = 0x1L << 58;
                const long _mask6 = 0x1L << 57;
                const long _mask7 = 0x1L << 56;
                const long _mask8 = 0x1L << 55;
                const long _mask9 = 0x1L << 54;
                const long _mask10 = 0x1L << 53;
                const long _mask11 = 0x1L << 52;
                const long _mask12 = 0x1L << 51;
                const long _mask13 = 0x1L << 50;
                const long _mask14 = 0x1L << 49;
                const long _mask15 = 0x1L << 48;
                const long _mask16 = 0x1L << 47;
                const long _mask17 = 0x1L << 46;
                const long _mask18 = 0x1L << 45;
                const long _mask19 = 0x1L << 44;
                const long _mask20 = 0x1L << 43;
                const long _mask21 = 0x1L << 42;
                const long _mask22 = 0x1L << 41;
                const long _mask23 = 0x1L << 40;
                const long _mask24 = 0x1L << 39;
                const long _mask25 = 0x1L << 38;
                const long _mask26 = 0x1L << 37;
                const long _mask27 = 0x1L << 36;
                const long _mask28 = 0x1L << 35;
                const long _mask29 = 0x1L << 34;
                const long _mask30 = 0x1L << 33;
                const long _mask31 = 0x1L << 32;
                const long _mask32 = 0x1L << 31;
                const long _mask33 = 0x1L << 30;
                const long _mask34 = 0x1L << 29;
                const long _mask35 = 0x1L << 28;
                const long _mask36 = 0x1L << 27;
                const long _mask37 = 0x1L << 26;
                const long _mask38 = 0x1L << 25;
                const long _mask39 = 0x1L << 24;
                const long _mask40 = 0x1L << 23;
                const long _mask41 = 0x1L << 22;
                const long _mask42 = 0x1L << 21;
                const long _mask43 = 0x1L << 20;
                const long _mask44 = 0x1L << 19;
                const long _mask45 = 0x1L << 18;
                const long _mask46 = 0x1L << 17;
                const long _mask47 = 0x1L << 16;
                const long _mask48 = 0x1L << 15;
                const long _mask49 = 0x1L << 14;
                const long _mask50 = 0x1L << 13;
                const long _mask51 = 0x1L << 12;
                const long _mask52 = 0x1L << 11;
                const long _mask53 = 0x1L << 10;
                const long _mask54 = 0x1L << 9;
                const long _mask55 = 0x1L << 8;
                const long _mask56 = 0x1L << 7;
                const long _mask57 = 0x1L << 6;
                const long _mask58 = 0x1L << 5;
                const long _mask59 = 0x1L << 4;
                const long _mask60 = 0x1L << 3;
                const long _mask61 = 0x1L << 2;
                const long _mask62 = 0x1L << 1;
                const long _mask63 = 0x1L << 0;
                int _idx0 = System.Math.Abs((int)(((z & _mask0) >> 63) + (((y & _mask0) >> 63) << 1) + (((x & _mask0) >> 63) << 2)));
                int _idx1 = (int)(((z & _mask1) >> 62) + (((y & _mask1) >> 62) << 1) + (((x & _mask1) >> 62) << 2));
                int _idx2 = (int)(((z & _mask2) >> 61) + (((y & _mask2) >> 61) << 1) + (((x & _mask2) >> 61) << 2));
                int _idx3 = (int)(((z & _mask3) >> 60) + (((y & _mask3) >> 60) << 1) + (((x & _mask3) >> 60) << 2));
                int _idx4 = (int)(((z & _mask4) >> 59) + (((y & _mask4) >> 59) << 1) + (((x & _mask4) >> 59) << 2));
                int _idx5 = (int)(((z & _mask5) >> 58) + (((y & _mask5) >> 58) << 1) + (((x & _mask5) >> 58) << 2));
                int _idx6 = (int)(((z & _mask6) >> 57) + (((y & _mask6) >> 57) << 1) + (((x & _mask6) >> 57) << 2));
                int _idx7 = (int)(((z & _mask7) >> 56) + (((y & _mask7) >> 56) << 1) + (((x & _mask7) >> 56) << 2));
                int _idx8 = (int)(((z & _mask8) >> 55) + (((y & _mask8) >> 55) << 1) + (((x & _mask8) >> 55) << 2));
                int _idx9 = (int)(((z & _mask9) >> 54) + (((y & _mask9) >> 54) << 1) + (((x & _mask9) >> 54) << 2));
                int _idx10 = (int)(((z & _mask10) >> 53) + (((y & _mask10) >> 53) << 1) + (((x & _mask10) >> 53) << 2));
                int _idx11 = (int)(((z & _mask11) >> 52) + (((y & _mask11) >> 52) << 1) + (((x & _mask11) >> 52) << 2));
                int _idx12 = (int)(((z & _mask12) >> 51) + (((y & _mask12) >> 51) << 1) + (((x & _mask12) >> 51) << 2));
                int _idx13 = (int)(((z & _mask13) >> 50) + (((y & _mask13) >> 50) << 1) + (((x & _mask13) >> 50) << 2));
                int _idx14 = (int)(((z & _mask14) >> 49) + (((y & _mask14) >> 49) << 1) + (((x & _mask14) >> 49) << 2));
                int _idx15 = (int)(((z & _mask15) >> 48) + (((y & _mask15) >> 48) << 1) + (((x & _mask15) >> 48) << 2));
                int _idx16 = (int)(((z & _mask16) >> 47) + (((y & _mask16) >> 47) << 1) + (((x & _mask16) >> 47) << 2));
                int _idx17 = (int)(((z & _mask17) >> 46) + (((y & _mask17) >> 46) << 1) + (((x & _mask17) >> 46) << 2));
                int _idx18 = (int)(((z & _mask18) >> 45) + (((y & _mask18) >> 45) << 1) + (((x & _mask18) >> 45) << 2));
                int _idx19 = (int)(((z & _mask19) >> 44) + (((y & _mask19) >> 44) << 1) + (((x & _mask19) >> 44) << 2));
                int _idx20 = (int)(((z & _mask20) >> 43) + (((y & _mask20) >> 43) << 1) + (((x & _mask20) >> 43) << 2));
                int _idx21 = (int)(((z & _mask21) >> 42) + (((y & _mask21) >> 42) << 1) + (((x & _mask21) >> 42) << 2));
                int _idx22 = (int)(((z & _mask22) >> 41) + (((y & _mask22) >> 41) << 1) + (((x & _mask22) >> 41) << 2));
                int _idx23 = (int)(((z & _mask23) >> 40) + (((y & _mask23) >> 40) << 1) + (((x & _mask23) >> 40) << 2));
                int _idx24 = (int)(((z & _mask24) >> 39) + (((y & _mask24) >> 39) << 1) + (((x & _mask24) >> 39) << 2));
                int _idx25 = (int)(((z & _mask25) >> 38) + (((y & _mask25) >> 38) << 1) + (((x & _mask25) >> 38) << 2));
                int _idx26 = (int)(((z & _mask26) >> 37) + (((y & _mask26) >> 37) << 1) + (((x & _mask26) >> 37) << 2));
                int _idx27 = (int)(((z & _mask27) >> 36) + (((y & _mask27) >> 36) << 1) + (((x & _mask27) >> 36) << 2));
                int _idx28 = (int)(((z & _mask28) >> 35) + (((y & _mask28) >> 35) << 1) + (((x & _mask28) >> 35) << 2));
                int _idx29 = (int)(((z & _mask29) >> 34) + (((y & _mask29) >> 34) << 1) + (((x & _mask29) >> 34) << 2));
                int _idx30 = (int)(((z & _mask30) >> 33) + (((y & _mask30) >> 33) << 1) + (((x & _mask30) >> 33) << 2));
                int _idx31 = (int)(((z & _mask31) >> 32) + (((y & _mask31) >> 32) << 1) + (((x & _mask31) >> 32) << 2));
                int _idx32 = (int)(((z & _mask32) >> 31) + (((y & _mask32) >> 31) << 1) + (((x & _mask32) >> 31) << 2));
                int _idx33 = (int)(((z & _mask33) >> 30) + (((y & _mask33) >> 30) << 1) + (((x & _mask33) >> 30) << 2));
                int _idx34 = (int)(((z & _mask34) >> 29) + (((y & _mask34) >> 29) << 1) + (((x & _mask34) >> 29) << 2));
                int _idx35 = (int)(((z & _mask35) >> 28) + (((y & _mask35) >> 28) << 1) + (((x & _mask35) >> 28) << 2));
                int _idx36 = (int)(((z & _mask36) >> 27) + (((y & _mask36) >> 27) << 1) + (((x & _mask36) >> 27) << 2));
                int _idx37 = (int)(((z & _mask37) >> 26) + (((y & _mask37) >> 26) << 1) + (((x & _mask37) >> 26) << 2));
                int _idx38 = (int)(((z & _mask38) >> 25) + (((y & _mask38) >> 25) << 1) + (((x & _mask38) >> 25) << 2));
                int _idx39 = (int)(((z & _mask39) >> 24) + (((y & _mask39) >> 24) << 1) + (((x & _mask39) >> 24) << 2));
                int _idx40 = (int)(((z & _mask40) >> 23) + (((y & _mask40) >> 23) << 1) + (((x & _mask40) >> 23) << 2));
                int _idx41 = (int)(((z & _mask41) >> 22) + (((y & _mask41) >> 22) << 1) + (((x & _mask41) >> 22) << 2));
                int _idx42 = (int)(((z & _mask42) >> 21) + (((y & _mask42) >> 21) << 1) + (((x & _mask42) >> 21) << 2));
                int _idx43 = (int)(((z & _mask43) >> 20) + (((y & _mask43) >> 20) << 1) + (((x & _mask43) >> 20) << 2));
                int _idx44 = (int)(((z & _mask44) >> 19) + (((y & _mask44) >> 19) << 1) + (((x & _mask44) >> 19) << 2));
                int _idx45 = (int)(((z & _mask45) >> 18) + (((y & _mask45) >> 18) << 1) + (((x & _mask45) >> 18) << 2));
                int _idx46 = (int)(((z & _mask46) >> 17) + (((y & _mask46) >> 17) << 1) + (((x & _mask46) >> 17) << 2));
                int _idx47 = (int)(((z & _mask47) >> 16) + (((y & _mask47) >> 16) << 1) + (((x & _mask47) >> 16) << 2));
                int _idx48 = (int)(((z & _mask48) >> 15) + (((y & _mask48) >> 15) << 1) + (((x & _mask48) >> 15) << 2));
                int _idx49 = (int)(((z & _mask49) >> 14) + (((y & _mask49) >> 14) << 1) + (((x & _mask49) >> 14) << 2));
                int _idx50 = (int)(((z & _mask50) >> 13) + (((y & _mask50) >> 13) << 1) + (((x & _mask50) >> 13) << 2));
                int _idx51 = (int)(((z & _mask51) >> 12) + (((y & _mask51) >> 12) << 1) + (((x & _mask51) >> 12) << 2));
                int _idx52 = (int)(((z & _mask52) >> 11) + (((y & _mask52) >> 11) << 1) + (((x & _mask52) >> 11) << 2));
                int _idx53 = (int)(((z & _mask53) >> 10) + (((y & _mask53) >> 10) << 1) + (((x & _mask53) >> 10) << 2));
                int _idx54 = (int)(((z & _mask54) >> 9) + (((y & _mask54) >> 9) << 1) + (((x & _mask54) >> 9) << 2));
                int _idx55 = (int)(((z & _mask55) >> 8) + (((y & _mask55) >> 8) << 1) + (((x & _mask55) >> 8) << 2));
                int _idx56 = (int)(((z & _mask56) >> 7) + (((y & _mask56) >> 7) << 1) + (((x & _mask56) >> 7) << 2));
                int _idx57 = (int)(((z & _mask57) >> 6) + (((y & _mask57) >> 6) << 1) + (((x & _mask57) >> 6) << 2));
                int _idx58 = (int)(((z & _mask58) >> 5) + (((y & _mask58) >> 5) << 1) + (((x & _mask58) >> 5) << 2));
                int _idx59 = (int)(((z & _mask59) >> 4) + (((y & _mask59) >> 4) << 1) + (((x & _mask59) >> 4) << 2));
                int _idx60 = (int)(((z & _mask60) >> 3) + (((y & _mask60) >> 3) << 1) + (((x & _mask60) >> 3) << 2));
                int _idx61 = (int)(((z & _mask61) >> 2) + (((y & _mask61) >> 2) << 1) + (((x & _mask61) >> 2) << 2));
                int _idx62 = (int)(((z & _mask62) >> 1) + (((y & _mask62) >> 1) << 1) + (((x & _mask62) >> 1) << 2));
                int _idx63 = (int)(((z & _mask63) >> 0) + (((y & _mask63) >> 0) << 1) + (((x & _mask63) >> 0) << 2));
                return octree.RootNode
                    .m_Nodes[_idx0]
                    .m_Nodes[_idx1]
                    .m_Nodes[_idx2]
                    .m_Nodes[_idx3]
                    .m_Nodes[_idx4]
                    .m_Nodes[_idx5]
                    .m_Nodes[_idx6]
                    .m_Nodes[_idx7]
                    .m_Nodes[_idx8]
                    .m_Nodes[_idx9]
                    .m_Nodes[_idx10]
                    .m_Nodes[_idx11]
                    .m_Nodes[_idx12]
                    .m_Nodes[_idx13]
                    .m_Nodes[_idx14]
                    .m_Nodes[_idx15]
                    .m_Nodes[_idx16]
                    .m_Nodes[_idx17]
                    .m_Nodes[_idx18]
                    .m_Nodes[_idx19]
                    .m_Nodes[_idx20]
                    .m_Nodes[_idx21]
                    .m_Nodes[_idx22]
                    .m_Nodes[_idx23]
                    .m_Nodes[_idx24]
                    .m_Nodes[_idx25]
                    .m_Nodes[_idx26]
                    .m_Nodes[_idx27]
                    .m_Nodes[_idx28]
                    .m_Nodes[_idx29]
                    .m_Nodes[_idx30]
                    .m_Nodes[_idx31]
                    .m_Nodes[_idx32]
                    .m_Nodes[_idx33]
                    .m_Nodes[_idx34]
                    .m_Nodes[_idx35]
                    .m_Nodes[_idx36]
                    .m_Nodes[_idx37]
                    .m_Nodes[_idx38]
                    .m_Nodes[_idx39]
                    .m_Nodes[_idx40]
                    .m_Nodes[_idx41]
                    .m_Nodes[_idx42]
                    .m_Nodes[_idx43]
                    .m_Nodes[_idx44]
                    .m_Nodes[_idx45]
                    .m_Nodes[_idx46]
                    .m_Nodes[_idx47]
                    .m_Nodes[_idx48]
                    .m_Nodes[_idx49]
                    .m_Nodes[_idx50]
                    .m_Nodes[_idx51]
                    .m_Nodes[_idx52]
                    .m_Nodes[_idx53]
                    .m_Nodes[_idx54]
                    .m_Nodes[_idx55]
                    .m_Nodes[_idx56]
                    .m_Nodes[_idx57]
                    .m_Nodes[_idx58]
                    .m_Nodes[_idx59]
                    .m_Nodes[_idx60]
                    .m_Nodes[_idx61]
                    .m_Nodes[_idx62]
                    .m_Nodes[_idx63]
                    .m_Value;
            }
            catch (System.NullReferenceException)
            {
                return null;
            }
        }
    }
}

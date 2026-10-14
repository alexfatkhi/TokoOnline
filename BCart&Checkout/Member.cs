
using ProjectOOP_TokoOnline;
using System;

namespace ProjectOOP_TokoOnline
{
    public class Member : Customer
    {
        public int Point { get; private set; }

        public Member( string customerName, string address)
            : base( customerName, address)
        {
            Point = 0;
        }

        public int HitungPoin(decimal totalBelanja)
        {
            return (int)(totalBelanja / 10000m);
        }

        public void TambahPoin(decimal totalBelanja)
        {
            Point += HitungPoin(totalBelanja);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace g711audio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Random rnd = new Random();
            int s = 122, c = 0;
            double alaw = 0, mulaw = 0;
            Console.WriteLine("Orig\tµEnc\tµDec\tµ%Diff\tAEnc\tADec\tA%Diff");
            for (s = 122; s < MuLawEncoder.MAX; s = (int)(s * (1 + rnd.NextDouble() / 11.4)))
            {
                c++; // keep a tally of how many times we've run this, for averaging purposes
                byte Menc = MuLawEncoder.MuLawEncode(s);
                int Mdec = MuLawDecoder.MuLawDecode(Menc);
                double MpercentDiff = (100 * (Mdec - s)) / (double)s;
                mulaw += Math.Abs(MpercentDiff);
                byte Aenc = ALawEncoder.ALawEncode(s);
                int Adec = ALawDecoder.ALawDecode(Aenc);
                double ApercentDiff = (100 * (Adec - s)) / (double)s;
                alaw += Math.Abs(ApercentDiff);
                Console.WriteLine(s + " \t" + Menc + " \t" + Mdec + " \t" +
                    Math.Round(MpercentDiff, 2) + " \t" + Aenc + " \t" + Adec +
                    " \t" + Math.Round(ApercentDiff, 2));
            }
            Console.WriteLine();
            Console.WriteLine("Average percent error for each codec for this sample:");
            Console.WriteLine("µ-Law: " + (mulaw / (double)c));
            Console.WriteLine("A-Law: " + (alaw / (double)c));
            Console.WriteLine();

            //Calculate average error for the whole range
            double Alaw = 0, µlaw = 0, den = 100 / (double)(ALawEncoder.MAX);
            for (int i = 1; i <= ALawEncoder.MAX; i++)
            {
                Alaw += Math.Abs((ALawDecoder.ALawDecode(ALawEncoder.ALawEncode(i)) - i) / (double)i);
                µlaw += Math.Abs((MuLawDecoder.MuLawDecode(MuLawEncoder.MuLawEncode(i)) - i) / (double)i);
            }
            Console.WriteLine("Overall average percent error for each codec:");
            Console.WriteLine("µ-Law: " + (µlaw * den));
            Console.WriteLine("A-Law: " + (Alaw * den));
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}

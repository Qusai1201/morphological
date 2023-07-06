using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Morphological;

#pragma warning disable CA1416

class Program
{
    public static void Main(string[] args)
    {
        morphological m = new morphological();
        m.Run();
    }
}

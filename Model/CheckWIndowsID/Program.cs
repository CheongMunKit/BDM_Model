using Microsoft.Win32;
using Model.Protection;
using System;

namespace CheckWIndowsID
{
    class Program
    {
        static void Main(string[] args)
        {
            Model.Protection.LicenseManager.CheckLicense();
            Console.ReadKey();
        }
    }
}

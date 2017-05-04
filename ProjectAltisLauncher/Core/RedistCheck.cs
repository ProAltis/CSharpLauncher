﻿using System.Windows.Forms;
using Microsoft.Win32;

namespace ProjectAltis.Core
{
    public static class RedistCheck
    {
        public static bool RedistInstalled()
        {
            string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A";
            string valueName = "ProductName";
            return Registry.GetValue(keyName, valueName, null) != null;
        }

        public static void CheckRedistHandler()
        {
            if(!RedistInstalled())
            {
                DialogResult f = MessageBox.Show(
                    @"The Microsoft Visual C++ redistributable\n 2010 x86 was not found. It may be required to play Project Altis.\nGo to the download page?", @"Project Altis", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(f == DialogResult.Yes)
                {
                    Log.TryOpenUrl(
                        @"https://download.microsoft.com/download/1/6/5/165255E7-1014-4D0A-B094-B6A430A6BFFC/vcredist_x86.exe");
                }
            }
        }
    }
}
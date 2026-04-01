// ============================================================
//  SaveSecurityKeys.cs
//  IMPORTANT: Add this file to your .gitignore so keys are
//  never committed to version control.
//
//  Both values must stay the same across builds targeting the
//  same save files.  Changing them invalidates all existing
//  saves (existing files will fail to decrypt).
//
//  Key32 must be exactly 32 UTF-8 bytes.
//  IV16  must be exactly 16 UTF-8 bytes.
// ============================================================

using System.Text;

public static class SaveSecurityKeys
{
    // Replace with your own values before shipping.
    public static readonly byte[] Key32 = Encoding.UTF8.GetBytes("IkeNoBi_K32_RePlAcEmE_BeFoReShip");  // 32 bytes
    public static readonly byte[] IV16  = Encoding.UTF8.GetBytes("IkeNoBi_IV_16Byt");                  // 16 bytes
}

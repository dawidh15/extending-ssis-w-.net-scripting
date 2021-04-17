#region Help:  Introduction to the Script Component
/* The Script Component allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services data flow.
 *
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script component. */
#endregion

#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.IO;
using System.Security.Cryptography;
#endregion

/// <summary>
/// This is the class to which to add your code.  Do not change the name, attributes, or parent
/// of this class.
/// </summary>
[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{
    #region Help:  Using Integration Services variables and parameters
    /* To use a variable in this script, first ensure that the variable has been added to
     * either the list contained in the ReadOnlyVariables property or the list contained in
     * the ReadWriteVariables property of this script component, according to whether or not your
     * code needs to write into the variable.  To do so, save this script, close this instance of
     * Visual Studio, and update the ReadOnlyVariables and ReadWriteVariables properties in the
     * Script Transformation Editor window.
     * To use a parameter in this script, follow the same steps. Parameters are always read-only.
     *
     * Example of reading from a variable or parameter:
     *  DateTime startTime = Variables.MyStartTime;
     *
     * Example of writing to a variable:
     *  Variables.myStringVariable = "new value";
     */
    #endregion

    #region Help:  Using Integration Services Connnection Managers
    /* Some types of connection managers can be used in this script component.  See the help topic
     * "Working with Connection Managers Programatically" for details.
     *
     * To use a connection manager in this script, first ensure that the connection manager has
     * been added to either the list of connection managers on the Connection Managers page of the
     * script component editor.  To add the connection manager, save this script, close this instance of
     * Visual Studio, and add the Connection Manager to the list.
     *
     * If the component needs to hold a connection open while processing rows, override the
     * AcquireConnections and ReleaseConnections methods.
     * 
     * Example of using an ADO.Net connection manager to acquire a SqlConnection:
     *  object rawConnection = Connections.SalesDB.AcquireConnection(transaction);
     *  SqlConnection salesDBConn = (SqlConnection)rawConnection;
     *
     * Example of using a File connection manager to acquire a file path:
     *  object rawConnection = Connections.Prices_zip.AcquireConnection(transaction);
     *  string filePath = (string)rawConnection;
     *
     * Example of releasing a connection manager:
     *  Connections.SalesDB.ReleaseConnection(rawConnection);
     */
    #endregion

    #region Help:  Firing Integration Services Events
    /* This script component can fire events.
     *
     * Example of firing an error event:
     *  ComponentMetaData.FireError(10, "Process Values", "Bad value", "", 0, out cancel);
     *
     * Example of firing an information event:
     *  ComponentMetaData.FireInformation(10, "Process Values", "Processing has started", "", 0, fireAgain);
     *
     * Example of firing a warning event:
     *  ComponentMetaData.FireWarning(10, "Process Values", "No rows were received", "", 0);
     */
    #endregion


    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        if(!Row.YearlyIncome_IsNull)
        {
            string TextToEncrypt = Row.YearlyIncome.ToString();
            string EncryptedText = Encrypt(TextToEncrypt, Variables.EncryptKey);
            Row.EcryptIncome = EncryptedText;
            Row.DecryptIncome = Convert.ToInt32(Decrypt(EncryptedText, Variables.EncryptKey));
        }
    }

    // Encrypt text with Rijndael encryption
    public static string Encrypt(string clearText, string Password)
    {
        // Convert password string into byte array
        byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);

        // Create Key and IV from the password with salt technique
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

        // Create a symmetric algorithm with Rijndael
        Rijndael alg = Rijndael.Create();

        // Set Key and IV
        alg.Key = pdb.GetBytes(32);
        alg.IV = pdb.GetBytes(16);

        // Create a MemoryStream  
        MemoryStream ms = new MemoryStream();

        // Create a CryptoStream
        CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

        // Write the data and make it do the encryption 
        cs.Write(clearBytes, 0, clearBytes.Length);

        // Close CryptoStream
        cs.Close();

        // Get Encypted data from MemoryStream
        byte[] encryptedData = ms.ToArray();

        // return the Encypted data as a String
        return Convert.ToBase64String(encryptedData);
    }

    // Decrypt text with Rijndael encryption
    public static string Decrypt(string cipherText, string Password)
    {
        // Convert password string into byte array
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        // Create Key and IV from the password with salt technique
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

        // Create a symmetric algorithm with Rijndael
        Rijndael alg = Rijndael.Create();

        // Set Key and IV
        alg.Key = pdb.GetBytes(32);
        alg.IV = pdb.GetBytes(16);

        // Create a MemoryStream  
        MemoryStream ms = new MemoryStream();

        // Create a CryptoStream 
        CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);

        // Write the data and make it do the decryption 
        cs.Write(cipherBytes, 0, cipherBytes.Length);

        // Close CryptoStream
        cs.Close();

        // Get Decypted data from MemoryStream
        byte[] decryptedData = ms.ToArray();

        // return the Decypted data as a String
        return System.Text.Encoding.Unicode.GetString(decryptedData);
    }

}

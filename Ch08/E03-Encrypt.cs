#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
#endregion

namespace ST_11874bad94d54c67bdfb0d9587891450
{
    /// <summary>
    /// ScriptMain is the entry point class of the script.  Do not change the name, attributes,
    /// or parent of this class.
    /// </summary>
	[Microsoft.SqlServer.Dts.Tasks.ScriptTask.SSISScriptTaskEntryPointAttribute]
	public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase
	{
        #region Help:  Using Integration Services variables and parameters in a script
        /* To use a variable in this script, first ensure that the variable has been added to 
         * either the list contained in the ReadOnlyVariables property or the list contained in 
         * the ReadWriteVariables property of this script task, according to whether or not your
         * code needs to write to the variable.  To add the variable, save this script, close this instance of
         * Visual Studio, and update the ReadOnlyVariables and 
         * ReadWriteVariables properties in the Script Transformation Editor window.
         * To use a parameter in this script, follow the same steps. Parameters are always read-only.
         * 
         * Example of reading from a variable:
         *  DateTime startTime = (DateTime) Dts.Variables["System::StartTime"].Value;
         * 
         * Example of writing to a variable:
         *  Dts.Variables["User::myStringVariable"].Value = "new value";
         * 
         * Example of reading from a package parameter:
         *  int batchId = (int) Dts.Variables["$Package::batchId"].Value;
         *  
         * Example of reading from a project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].Value;
         * 
         * Example of reading from a sensitive project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].GetSensitiveValue();
         * */

        #endregion

        #region Help:  Firing Integration Services events from a script
        /* This script task can fire events for logging purposes.
         * 
         * Example of firing an error event:
         *  Dts.Events.FireError(18, "Process Values", "Bad value", "", 0);
         * 
         * Example of firing an information event:
         *  Dts.Events.FireInformation(3, "Process Values", "Processing has started", "", 0, ref fireAgain)
         * 
         * Example of firing a warning event:
         *  Dts.Events.FireWarning(14, "Process Values", "No values received for input", "", 0);
         * */
        #endregion

        #region Help:  Using Integration Services connection managers in a script
        /* Some types of connection managers can be used in this script task.  See the topic 
         * "Working with Connection Managers Programatically" for details.
         * 
         * Example of using an ADO.Net connection manager:
         *  object rawConnection = Dts.Connections["Sales DB"].AcquireConnection(Dts.Transaction);
         *  SqlConnection myADONETConnection = (SqlConnection)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Sales DB"].ReleaseConnection(rawConnection);
         *
         * Example of using a File connection manager
         *  object rawConnection = Dts.Connections["Prices.zip"].AcquireConnection(Dts.Transaction);
         *  string filePath = (string)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Prices.zip"].ReleaseConnection(rawConnection);
         * */
        #endregion


        /// <summary>
        /// This method is called when this script task executes in the control flow.
        /// Before returning from this method, set the value of Dts.TaskResult to indicate success or failure.
        /// To open Help, press F1.
        /// </summary>

        // Salt is some random data in addtion to a password
        // Protects against frequently used passwords.

        private static readonly byte[] SALT = new byte[] {0x26, 0xdc, 0x7a, 0xc5, 0xfe,
            0xad, 0xed, 0x7a, 0x64, 0xc5, 0xfe, 0x20, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        // Decrypt with Rijndael encryption
        public static void Decrypt(string fileIn, string fileOut, string Password)
        {
            // open file stream for encrypted source file
            using(FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read))
            {
                // open filestream for decrypted file
                using (FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // create Key from password and SALT
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Password, SALT);

                    // create a symmetric algorithm with Rijndael
                    Rijndael alg = Rijndael.Create();

                    // Set Key and IV
                    alg.Key = pdb.GetBytes(32);
                    alg.IV = pdb.GetBytes(16);

                    // create CryptoStream
                    using (CryptoStream cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // Initialize buffer and proces the input file in chunks
                        // This is done to avoid reading the whole file into memory
                        int bufferLength = 4096;
                        byte[] buffer = new byte[bufferLength];
                        int bytesRead;

                        do
                        {
                            // read chunk of data from the input file
                            bytesRead = fsIn.Read(buffer, 0, bufferLength);
                            // Decrypt
                            cs.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);

                        //close everything
                        cs.Close();
                        fsOut.Close();
                        fsIn.Close();
                }
                }
            }
        }

        // Encrypt textfile with Rijndael encryption
        public static void Encrypt(string fileIn, string fileOut, string Password)
        {
            // Open filestream for source file
            using (FileStream fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read))
            {
                // Open filestrem for encrypted file
                using (FileStream fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // Create Key and IV
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Password, SALT);
                    // Create symmetric algorithm with Rijndael
                    Rijndael alg = Rijndael.Create();
                    // Set key and IV
                    alg.Key = pdb.GetBytes(32);
                    alg.IV = pdb.GetBytes(16);

                    //Create a cryptostream
                    using (CryptoStream cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Initialize a buffer and process the input file in chunks
                        // File can be huge, this is important
                        int bufferLength = 4096;
                        byte[] buffer = new byte[bufferLength];
                        int bytesRead;

                        do
                        {
                            // read chunk of data from the input file
                            bytesRead = fsIn.Read(buffer, 0, bufferLength);
                            // encrypt it
                            cs.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                        //close all
                        cs.Close();
                        fsOut.Close();
                        fsIn.Close();
                    }
                }
            }
        }

        public void Main()
		{
            // Encrypt a file
            // Path of file to be encrypted
            string filepathSource = Dts.Connections["MyProducts"].AcquireConnection(null).ToString();
            // Get Path of encrypted file
            string filepathEncrypted = Dts.Connections["MyProducts_Encrypted"].AcquireConnection(null).ToString();
            // GetPassword
            string encryptionKey = Dts.Variables["$Package::Password"].GetSensitiveValue().ToString();
            //Create an encrypted copy of the file
            Encrypt(filepathSource, filepathEncrypted, encryptionKey);

            if (File.Exists(filepathEncrypted))
            {
                // Get path of decrypted file
                string filepathDecrypted = Dts.Connections["MyProducts_Decrypted"].AcquireConnection(null).ToString();
                Decrypt(filepathEncrypted, filepathDecrypted, encryptionKey);
                Dts.TaskResult = (int)ScriptResults.Success;

            } 
            else
            {
                // Fail component
                Dts.TaskResult = (int)ScriptResults.Failure;
                Dts.Events.FireError(0, "ERROR", "Encrypted file not found.", string.Empty, 0);
            }


			
		}

        #region ScriptResults declaration
        /// <summary>
        /// This enum provides a convenient shorthand within the scope of this class for setting the
        /// result of the script.
        /// 
        /// This code was generated automatically.
        /// </summary>
        enum ScriptResults
        {
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        };
        #endregion

	}
}
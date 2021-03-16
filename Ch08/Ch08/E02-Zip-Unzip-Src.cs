#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Collections;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
#endregion

namespace ST_6288655231b4426293080d0c108f193e
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
        public void Main()
        {
            bool fireAgain = true;
            // Obtener direccion de archivo
            string sourceFile = Dts.Connections["File Original"].ConnectionString;

            // Crear gestor de directorio para archivos fuente
            string originalPath = Dts.Variables["User::OriginalAddress"].Value.ToString();
            ManageDirectory originalDirectory = new ManageDirectory(originalPath);

            // Crear gestor de directorio para comprimidos
            string compressPath = Dts.Variables["User::CompressedAddress"].Value.ToString();
            ManageDirectory compressDirectory = new ManageDirectory(compressPath);

            // Crear gestor de directorio para descomprimidos
            string uncompressPath = Dts.Variables["User::UncompressedAddress"].Value.ToString();
            ManageDirectory uncompressDirectory = new ManageDirectory(uncompressPath);


            Dts.Events.FireInformation(0
               , "INFO"
               , $"Source Dir: {originalDirectory.Path}. Destination Dir: {compressDirectory.Path}"
               , string.Empty
               , 0
               , ref fireAgain);

            try
            {
                // Remove Dirs
                originalDirectory.Remove();
                compressDirectory.Remove();
                uncompressDirectory.Remove();

                // Crear Directorio para guardar original, otro para el Zip y para unzip
                originalDirectory.Create();
                compressDirectory.Create();
                uncompressDirectory.Create();

                // Copiar el archivo original al dir de respaldo
                string backupFile = Path.Combine(
                    originalDirectory.Path
                    , Path.GetFileName(sourceFile));

                File.Copy(sourceFile, backupFile, true);


            }
            catch (Exception ex)
            {
                Dts.Events.FireError(0
                    , "ERROR"
                    , $"Can't create directory: {ex.Message}"
                    , string.Empty
                    , 0);
            }



            // Comprimir Archivo

            //To use the ZipFile class, you must reference the System.IO.Compression.FileSystem assembly in your project.
            string zipFileDestination = Path.Combine(compressDirectory.Path, "backup.zip");
            ZipFile.CreateFromDirectory(originalDirectory.Path, zipFileDestination);

            // Descomprimir
            ZipFile.ExtractToDirectory(zipFileDestination, uncompressDirectory.Path);

            // Verificar que archivo se comprimio, y descomprimió
            var zipFiles = Directory.GetFiles(compressDirectory.Path, "*.zip");
            var unzipFiles = Directory.GetFiles(uncompressDirectory.Path, "*.csv");

            if ((zipFiles.Length != 0) && (unzipFiles.Length != 0))
            {
                Dts.TaskResult = (int)ScriptResults.Success;
            }
            else
            {
                Dts.TaskResult = (int)ScriptResults.Failure;
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

    public class ManageDirectory
    {
        public string Path { get; set; }

        public ManageDirectory(string inPath)
        {
            Path = inPath;

        }

        private bool Exists()
        {
            bool value = Directory.Exists(Path);
            return value;
        }

        // Create
        public void Create()
        {
            if (!Exists())
            {
                Directory.CreateDirectory(Path);
            }
            else
            {
                throw new Exception("Directory already exists");
            }
        }
        // Remove
        public void Remove()
        {
            if (Exists())
            {
                Directory.Delete(Path, true); //recursive delete
            }
        }
    }
}
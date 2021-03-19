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

    private int numberOfRows = 1000;
    private string randomNames = "Smith,Johnson,Williams,Brown,Jones," +
        "Miller,Davis,Garcia,Rodriguez,Wilson";
    private string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
           "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// This method is called once, before rows begin to be processed in the data flow.
    ///
    /// You can remove this method if you don't need to do anything here.
    /// </summary>
    public override void PreExecute()
    {
        base.PreExecute();

    }

    /// <summary>
    /// This method is called after all the rows have passed through this component.
    ///
    /// You can delete this method if you don't need to do anything here.
    /// </summary>
    public override void PostExecute()
    {
        base.PostExecute();
        /*
         * Add your code here
         */
    }

    public override void CreateNewOutputRows()
    {
        // Loop until Number of Rows is been reached
        for(int i = 0; i < numberOfRows; i++)
        {
            Output0Buffer.AddRow();

            Output0Buffer.Name = pickRandomString(randomNames, new Random(i));
            Output0Buffer.Street = createRndString(chars, 5, new Random(i)).ToUpper();
            Output0Buffer.HouseNumber = pickRndInt(0, 100, new Random(i));
            Output0Buffer.DateOfBirth = pickRndDate(new DateTime(1974, 01, 01), new DateTime(2000, 01, 01), new Random(i));
            Output0Buffer.Price = Convert.ToDecimal(
                pickRndNumber(100000d, 1000000d, new Random(i)));
            Output0Buffer.Percentaje = Convert.ToDecimal(pickRndNumber(0d, 100d, new Random(i)));
            Output0Buffer.Gender = pickRandomString("M,F", new Random(i));
        }
    }

    // Pick one string randomly
    private string pickRandomString(string stringlist, Random rndNumber)
    {
        string[] strings = stringlist.Split(',');
        return strings[rndNumber.Next(strings.Length)];
    }

    // Create string with random chars
    private string createRndString(string chars, int max, Random rndNumber)
    {
        max = rndNumber.Next(1, max);
        char[] stringChars = new char[max];
        for(int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[rndNumber.Next(chars.Length)];
        }

        return new string(stringChars); // convert char array to string
    }

    private int pickRndInt(int min, int max, Random rndNumber) => rndNumber.Next(min, max);

    private double pickRndNumber(double min, double max, Random rndNumber) => rndNumber.NextDouble() * (max - min) + min;

    private DateTime pickRndDate(DateTime from, DateTime to, Random rndNumber)
    {
        TimeSpan range = to - from;
        TimeSpan rndTimeSpan = new TimeSpan((long)(rndNumber.NextDouble()*range.Ticks));
        return (from + rndTimeSpan).Date;
    }

}

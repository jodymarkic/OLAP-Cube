/*
*   FILENAME: myForm
*   PROJECT: BI_A02
*   PROGRAMMER: Jody Markic
*   FIRST VERSION: 10/2/2017
*   DESCRIPTION: This files holds the main entry pointto  the BI_A02 project.
* 
*
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BI_A02
{
    //
    // Class: Program
    // Description: This class acts as the main entry point to the BI_A02
    //
    //
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new myForm());
        }
    }
}

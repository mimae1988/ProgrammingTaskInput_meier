using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Task_TransposeData
{
    class Program
    {

        static void Main(string[] args)
        {
            UserInteraction();          
            
        }

        /// <summary>
        /// Output on the console
        /// </summary>
        static void UserInteraction()
        {
            string ReceivedPath;
            string Outstream;
            Console.Clear();
            Console.WriteLine("Hello\nPlease insert full filepath here:\n");
            DataHandler Data = new DataHandler();           

            try
            {
                ReceivedPath = Console.ReadLine();
                int retval = Data.ImportData(ReceivedPath);
                Console.WriteLine("File read: {0}", Data.ErrorText);
                if(retval == 0)
                {
                    for (int line = 0; line < Data.OriginalData.Count; line++)
                    {
                        Outstream = String.Join("  ", Data.OriginalData[line]);
                        Console.WriteLine(Outstream);

                    }
                   
                }

                Console.WriteLine("Do you want to proceed? y/n");
                string decision1 = Console.ReadLine();
                if ((decision1 == "y")||(decision1=="Y"))
                {
                    // Visualize the transposed array and save the file
                    for (int line = 0; line < Data.TransposedData.Length; line++)
                    {
                        Outstream = String.Join("  ", Data.TransposedData[line]);
                        Console.WriteLine(Outstream);
                    }

                    Data.CreateNewFile();
                    Console.WriteLine("{0}",Data.ErrorText);
                    Console.WriteLine("Press any key to close the tool.");
                    Console.ReadKey();
                    
                }
                else if((decision1 == "n") || (decision1 == "N"))
                {
                    // close the console
                    Console.Clear();
                }

            }
            catch
            {
                Console.WriteLine("Error", Data.ErrorText);
            }


        }

    }


    class DataHandler
    {
        private string _Filename = "";
        public string Filename
        {
            get
            {
                return _Filename;
            }
        }
        

        private string _FilePath = "";
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
        }
       
        /// <summary>
        /// Data read from the .csv file
        /// </summary>
        private List<string[]> _OriginalData;
        public List<string[]> OriginalData
        {
            get
            {
                return _OriginalData;
            }
            set
            {
                _OriginalData = value;
            }
        }


        /// <summary>
        /// Data from the .csv file transposed 
        /// </summary>
        private string[][] _TransposedData;
        public string[][] TransposedData
        {
            get
            {
                return _TransposedData;
            }
            set
            {
                _TransposedData = value;
            }
        }

        private string _ErrorText;
        public string ErrorText
        {
            get
            {
                return _ErrorText;
            }

            set
            {
                switch(ErrorCode)
                {
                    case 0:
                        {
                            _ErrorText = "Ok";
                            break;
                        }
                    case 1:
                        {
                            _ErrorText = "Filepath empty";
                            break;
                        }
                    case 2:
                        {
                            _ErrorText = "File not found";
                            break;
                        }
                    case 3:
                        {
                            _ErrorText = "File not valid";
                            break;
                        }
                    case 4:
                        {
                            _ErrorText = "File empty";
                            break;
                        }
                    case 5:
                        {
                            _ErrorText = "File already exists";
                            break;
                        }
                    case 10:
                        {
                            _ErrorText = "New file successfully created";
                            break;
                        }
                    default:
                        {
                            _ErrorText = "Unknown";
                            break;
                        }
                }
            }
        }


        private void SetErrorText()
        {
            ErrorText = " ";
        }

        private int ErrorCode = 0;
        private char _Delimiter;
        public char Delimiter
        {
            get
            {
                return _Delimiter;
            }
            private set
            {
                _Delimiter = ';';
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns> 
        /// 0 = 0k
        /// 1 = filepath empty
        /// 2 = file not found
        /// 3 = file not valid (no .csv)
        /// 4 = file empty
        /// </returns>
        public int ImportData(string filepath)
        {
            OriginalData = new List<string[]>();
            ErrorCode = 0;
            int retval = 0;
            string[] cells;
            Delimiter = ' ';
            var tmppath = System.IO.Path.GetFullPath(filepath);
            //check if file exists
            if((tmppath == "")||(tmppath == null))
            {
                ErrorCode = 1;
                SetErrorText();
                return 1;
            }

            if(System.IO.File.Exists(tmppath))
            {
                _FilePath = Path.GetDirectoryName(tmppath);
                // check if file valid
                if(System.IO.Path.GetFileName(tmppath).Contains(".csv"))
                {
                    _Filename = Path.GetFileName(tmppath);
                    //Open file
                    string[] tmpfile = File.ReadAllLines(tmppath);
                    // check if file contains data
                    if(tmpfile.Length == 0)
                    {
                        retval = 4;
                    }
                    else
                    {
                        int count = 0;
                        // convert .csv to array
                        //load data
                        foreach (string s in tmpfile)
                        {
                            cells = s.Split(_Delimiter);

                            OriginalData.Add(cells);
                            count++;
                        }
                        TransposeList();
                    }

                }
                else
                {
                    retval = 3;
                }
                

            }
            else
            {
                retval = 2;
            }

            
            // return code
            ErrorCode = retval;
            SetErrorText();
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0 = should not be returned
        /// 5 = File already exists
        /// 10 = File successfully created
        /// </returns>
        public int CreateNewFile()
        {
            if(ErrorCode == 0)
            {
                string AddonNewFileName = "__transposed.csv";
                string NewFilename = String.Concat(System.IO.Path.GetFileNameWithoutExtension(_Filename), AddonNewFileName);

                // check if new filename already exists?
                if (File.Exists(System.IO.Path.Combine(_FilePath, NewFilename)))
                {
                    ErrorCode = 5;
                    SetErrorText();
                    return 5;
                }
                else
                {
                    // Create new transposed data .csv
                    // concat strings line by line
                    string[] newline = new string[TransposedData.Length];

                    for(int line = 0; line< TransposedData.Length; line++)
                    {
                        if (_TransposedData[line][0] != "")
                        {
                            newline[line] = String.Join(_Delimiter.ToString(), _TransposedData[line]);
                            newline[line] = String.Concat(newline[line], ';');
                        }
                    }
                    // Create and save file
                    File.AppendAllLines(System.IO.Path.Combine(_FilePath, NewFilename), newline);
                    ErrorCode = 10;
                    SetErrorText();
                    return 0;
                    
                }
            }
            else
            {
                SetErrorText();
                return ErrorCode;
            }
        }

        private void TransposeList()
        {
            int CSVColumnCount = 0;
            int CSVRowCount = 0;

            var testarray = OriginalData.ToArray();
            // identify size of the array
            CSVRowCount = testarray.Length;
            CSVColumnCount = testarray[0].Length;
            TransposedData = new string[CSVColumnCount][];
            for(int i = 0; i< CSVColumnCount; i++)
            {
                TransposedData[i] = new string[CSVRowCount];
            }

            //change xy-axis element to yx-axis element
            for (int row = 0; row< CSVRowCount; row++)
            {
                for(int column = 0; column<CSVColumnCount; column++)
                {
                    _TransposedData[column][row] = testarray[row][column];
                }
            }
        }
    }
}

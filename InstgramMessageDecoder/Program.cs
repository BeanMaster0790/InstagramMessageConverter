using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using static Decoder;

class Decoder
{
    private static List<Message> _messages = new List<Message>();
    private static InstagramMessageJson _chatJson = new InstagramMessageJson();

    private static Dictionary<string, string> _chatUsers = new Dictionary<string, string>();

    [STAThread]
    public static void Main(string[] args)
    {
        StartProgram();
    }

    public static void ConvertToTxt()
    {
        Console.Clear();
        Console.WriteLine("Where would you like to save the file?");

        SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };

        saveFileDialog.ShowDialog();

        Console.Clear();
        Console.WriteLine("Saving...");

        try
        {
            using (StreamWriter stream = new StreamWriter(saveFileDialog.FileName))
            {
                foreach (Message message in _messages)
                {
                    string line = "";

                    //Convert Instagrams timestamp_ms to a readable format
                    string messageDate = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).AddSeconds(message.timestamp_ms / 1000).ToLocalTime().ToShortDateString();

                    line += $"({messageDate}) ";

                    GetChatName(message);

                    line += _chatUsers[message.sender_name] + ": ";

                    //Check if the message contains any special media like photos
                    if (message.share != null)
                        line += message.share.link;
                    else if (message.photos != null)
                        line += message.photos[0].uri;
                    else if (message.content != null && message.content != "")
                        line += message.content;
                    else
                        line += "DECODER ERROR: Nothing was found that could be displayed. It may of been a view once photo or video";

                    //Convert the line to UTF8 to allow for all types of characters to be diplayed like ' and emojis
                    stream.WriteLine(UnescapeHex(line));
                }

                stream.Close();
            }
        }
        catch (Exception e)
        {
            Console.Clear();
            Console.WriteLine("Save Operaration Failed! Press Enter to try again or type quit to return to start.");

            string? input = Console.ReadLine();

            if(input != null && input.ToLower() == "quit")
            {
                StartProgram();
                return;
            }
            else
            {
                ConvertToTxt();
                return;
            }

        }

        Console.Clear();
        Console.WriteLine("Done! Press enter to return to menu.");
        Console.ReadKey();

        StartProgram();
    }

    public static void GetChatName(Message message)
    {
        if (!_chatUsers.ContainsKey(message.sender_name))
        {
            Console.Clear();

            string name = "";

            while (true)
            {
                //Converting to UEF8 here makes emojis slightly more understandable in the console
                Console.Clear();
                Console.WriteLine($"How would you like {UnescapeHex(message.sender_name)} to be shown in the file: ");

                name = Console.ReadLine();

                if (name != "" && name != null)
                    break;
            }

            _chatUsers.Add(message.sender_name, name);

            Console.Clear();
            Console.WriteLine("Saving...");
        }
    }

    public static void GetFileLocation()
    {
        int numberOfFiles = 1;

        OpenFileDialog fileDialog = new OpenFileDialog();

        fileDialog = new OpenFileDialog() { FileName = "Select A File", Title = "Open A File", Filter = "Json files (*.json)|*.json|All files (*.*)|*.*" };

        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            if (!fileDialog.FileName.Contains("message_") || !fileDialog.FileName.Contains(".json"))
            {
                MessageBox.Show("That was an invalid file. Make sure it is the 'message_1.json'.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetFileLocation();
                return;
            }
        }
        else
        {
            StartProgram();
            return;
        }


        fileDialog.FileName = fileDialog.FileName.Split("message_")[0];

        while (true)
        {
            string name = fileDialog.FileName + $"message_{numberOfFiles}.json";
            if (File.Exists(name))
            {
                numberOfFiles++;
            }
            else
                break;
        }

        numberOfFiles--;

        GetMessages(fileDialog.FileName, numberOfFiles);
    }

    private static void GetMessages(string filePath, int numberOfFiles)
    {
        filePath += $"message_{numberOfFiles}.json";

        Console.Clear();
        Console.WriteLine("Processing...");

        while (numberOfFiles >= 1)
        {
            StreamReader stream = new StreamReader(filePath);

            var json = stream.ReadToEnd();

            _chatJson = JsonConvert.DeserializeObject<InstagramMessageJson>(json);

            stream.Close();

            List<Message> messages = new List<Message>();

            foreach (Message message in _chatJson.messages)
            {
                messages.Add(message);
            }

            //Needed to make the order of messages make sence. Without it the order of the jsons is decending order but their contents are in assending order.
            messages.Reverse();

            foreach (Message message in messages)
            {
                _messages.Add(message);
            }

            numberOfFiles--;

            filePath = filePath.Replace($"message_{numberOfFiles + 1}", $"message_{numberOfFiles}");

        }
        ConvertToTxt();
    }

    public static void StartProgram()
    {
        Console.Clear();

        _messages.Clear();
        _chatJson = new InstagramMessageJson();
        _chatUsers = new Dictionary<string, string>();

        Console.WriteLine("Welcome to Instagram Message Converter! \nPress enter to continue or type 'Help'.");
        string? input = Console.ReadLine();

        if (input != null && input.ToLower() == "help")
        {
            Console.Clear();

            string helpText = "";

            try
            {
                StreamReader stream = new StreamReader(Application.StartupPath + @"\ReadMe.txt");
                
                helpText = stream.ReadToEnd();

                stream.Close();
                Console.WriteLine(helpText);
            }
            catch
            {
                Console.WriteLine("There was an error! The ReadMe.txt may have been deleted");
                Console.WriteLine("Press Enter to continue...");
            }


            Console.ReadKey();

            StartProgram();
        }
        else
            GetFileLocation();
    }

    public static string UnescapeHex(string data)
    {
        return Encoding.UTF8.GetString(Array.ConvertAll(Regex.Unescape(data).ToCharArray(), c => (byte)c));
    }



    #region Classes for Json
    public class InstagramMessageJson
    {
        public List<Participant> participants { get; set; }
        public List<Message> messages { get; set; }

        public string title { get; set; } = string.Empty;
        public bool is_still_participant { get; set; }
        public string thread_path { get; set; } = string.Empty;
    }

    public class Participant
    {
        public string Name = string.Empty;
    }

    public class Message
    {
        public string sender_name { get; set; } = string.Empty;
        public long timestamp_ms { get; set; }
        public string content { get; set; } = string.Empty;
        public bool is_geoblocked_for_viewer { get; set; }

        public List<Photo> photos { get; set; }

        public Share share { get; set; }
    }

    public class Photo
    {
        public string uri { get; set; } = string.Empty;
    }

    public class Share
    {
        public string link { get; set; } = string.Empty;
    }
    #endregion
}

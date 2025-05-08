using ClosedXML.Excel;
using DeepSeek.Core;
using DeepSeek.Core.Models;
using OpenAI.Chat;
using System.Text.RegularExpressions;

namespace AI_Test_Verification_Tool
{
    public partial class frmMain : Form
    {
        ChatClient GPTclient = new(model: "gpt-4o", apiKey: "redacted");
        DeepSeekClient DEEPclient = new DeepSeekClient("redacted");

        string path = "";
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            var response = ofd.ShowDialog();

            if (response == DialogResult.OK)
            {
                txtPath.Text = ofd.FileName;
                path = txtPath.Text;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            pnlBot.BringToFront();
            await StartAskingQuestions(rtbOutput);
        }

        private async Task StartAskingQuestions(RichTextBox rtb)
        {
            List<Q> Qs = await LoadQuestions();
            await AskingQuestions(Qs);
            await PrintAndPackage(Qs);
        }

        private async Task PrintAndPackage(List<Q> qs)
        {
            XLWorkbook wb = new();
            var stats = wb.AddWorksheet("Stats");
            var wk = wb.AddWorksheet("Validation Outputs");
            int row = 1;

            //row 1 Test Case ID #
            //1: Input Feature 2: Topic 3: Era 4: Question Type
            //                 2:       3:     4:
            //1: Context Feature 2: History Knowledge 3: Literacy 4: Role
            //                   2:                   3:          4:
            //1 - 2: Expected Output                  3: Invalid  4: Valid
            //                                        3: Irrelevant 5: Relevant
            //1: Text Input 2: Question
            //1:Expected answer 2: expected outputs
            //1 - 2: Actual Output          3: ChatGPT 4: Deepseek
            //                              3: gpt ans 4: deepseek ans
            //1 - 2: Result                 3: q.valid 4: q.valid
            stats.Row(1).Cell(1).Value = "#";
            stats.Row(1).Cell(2).Value = "DeepSeek %";
            stats.Row(1).Cell(3).Value = "DeepSeek Valid";
            stats.Row(1).Cell(4).Value = "ChatGPT %";
            stats.Row(1).Cell(5).Value = "ChatGPT Valid";


            int ind = 1;
            foreach (var q in qs)
            {
                string[] datarow = [ind.ToString(), ((float)q.DeepCorrect / q.ExpectedOutputs.Count * 100).ToString("N2"), q.DeepSeekValid.ToString(), ((float)q.ChatCorrect / q.ExpectedOutputs.Count * 100).ToString("N2"), q.ChatGPTValid.ToString()];
                stats.Row(ind + 1).Cell(1).InsertData(data: datarow, transpose: true);

                int firstrow = row;
                wk.Row(row).Cell(1).Value = $"Test Case ID {ind++}";
                wk.Row(row).Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                wk.Range($"A{row}:D{row}").Merge();

                wk.Row(++row).Cell(1).Value = "Input Feature";
                wk.Row(row).Cell(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                wk.Row(row).Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                wk.Row(row).Cell(2).Value = "Topic";
                wk.Row(row).Cell(3).Value = "Era";
                wk.Row(row).Cell(4).Value = "Question Type";

                wk.Range($"A{row}:A{row + 1}").Merge();
                
                wk.Row(++row).Cell(2).Value = q.Category;
                wk.Row(row).Cell(3).Value = q.Era;
                wk.Row(row).Cell(4).Value = q.QuestionType;

                wk.Row(++row).Cell(1).Value = "Context Feature";
                wk.Row(row).Cell(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                wk.Row(row).Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                wk.Row(row).Cell(2).Value = "History Knowledge";
                wk.Row(row).Cell(3).Value = "Literacy";
                wk.Row(row).Cell(4).Value = "Role";

                wk.Range($"A{row}:A{row + 1}").Merge();

                wk.Row(++row).Cell(2).Value = q.Knowledge;
                wk.Row(row).Cell(3).Value = q.Literacy;
                wk.Row(row).Cell(4).Value = q.Role;

                wk.Row(++row).Cell(1).Value = "Expected Output";

                wk.Range($"A{row}:B{row + 1}").Merge();
                wk.Row(row).Cell(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                wk.Row(row).Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                wk.Row(row).Cell(3).Value = "Invalid";
                wk.Row(row).Cell(4).Value = "Valid";

                wk.Range($"A{row}:A{row + 1}").Merge();

                wk.Row(++row).Cell(3).Value = "Irrelevant";
                wk.Row(row).Cell(4).Value = "Relevant";

                wk.Row(++row).Cell(1).Value = "Text Input";
                wk.Row(row).Cell(2).Value = q.Question;

                wk.Range($"B{row}:D{row}").Merge();

                wk.Row(++row).Cell(1).Value = "Expected Outputs";

                string s = "";

                foreach (var eo in q.ExpectedOutputs)
                {
                    s += eo + ", ";
                }
                s = s.Trim(',').Trim();

                wk.Row(row).Cell(2).Value = s;
                wk.Range($"B{row}:D{row}").Merge();

                wk.Row(++row).Cell(1).Value = "Actual Output";
                wk.Row(row).Cell(3).Value = "ChatGPT";
                wk.Row(row).Cell(4).Value = "DeepSeek";

                wk.Range($"A{row}:B{row + 9}").Merge();

                wk.Row(row).Cell(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                wk.Row(row).Cell(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                string d = "";
                string c = "";

                if (q.DeepSeek.Length > 300)
                    d = q.DeepSeek.Substring(0,300) + "...";
                else
                    d = q.DeepSeek;

                if (q.ChatGPT.Length > 300)
                    c = q.ChatGPT.Substring(0,300) + "...";
                else
                    c = q.ChatGPT;

                wk.Row(++row).Cell(3).Value = c;
                wk.Row(row).Cell(4).Value = d;

                wk.Range($"C{row}:C{row + 8}").Merge();
                wk.Range($"D{row}:D{row + 8}").Merge();

                wk.Cell($"C{row}").Style.Alignment.WrapText = true;
                wk.Cell($"D{row}").Style.Alignment.WrapText = true;

                row += 8;

                wk.Row(++row).Cell(1).Value = "Result";

                wk.Range($"A{row}:B{row}").Merge();

                string deepval = "";
                if (q.DeepSeekValid)
                    deepval = "Valid";
                else
                    deepval = "Invalid";


                string chatval = "";
                if (q.ChatGPTValid)
                    chatval = "Valid";
                else
                    chatval = "Invalid";

                wk.Row(row).Cell(3).Value = $"{chatval}";
                wk.Row(row).Cell(4).Value = $"{deepval}";
                int lastrow = row;

                wk.Range($"A{firstrow}:D{lastrow}").Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                wk.Range($"A{firstrow}:D{lastrow}").Cells().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                wk.Range($"A{firstrow}:D{lastrow}").Cells().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                wk.Range($"A{firstrow}:D{lastrow}").Cells().Style.Border.RightBorder = XLBorderStyleValues.Thin;

                row += 2;
            }

            wk.Column(1).Width = 40;
            wk.Column(2).Width = 40;
            wk.Column(3).Width = 40;
            wk.Column(4).Width = 40;

            //Ratio correct per each bot
            int dvalid = 0; // how many questions in deepseek are valid
            int cvalid = 0; // how many questions in chatgpt are valid
            for (int i = 2; i < qs.Count + 2; i++) //for every question, using index to place into xlsx
            {
                float dval = float.Parse(stats.Row(i).Cell(2).Value.ToString());
                float cval = float.Parse(stats.Row(i).Cell(4).Value.ToString());
                
                if (dval >= 50) //if over 50%, inc
                    dvalid++;
                if (cval >= 50)
                    cvalid++;
            }

            stats.Row(53).Cell(1).Value = "Totals: ";
            stats.Row(53).Cell(2).Value = $"{dvalid} / {qs.Count}";
            stats.Row(53).Cell(4).Value = $"{cvalid} / {qs.Count}";

            FileInfo fi = new FileInfo(path);

            string fname = fi.Name;
            string ext = fi.Name.Split('.')[1];

            File.WriteAllText(fi.FullName.Replace(ext, "console dump.txt"), rtbOutput.Text);
            stats.Columns().AdjustToContents();
            wb.SaveAs(fi.FullName.Replace(ext, $"xlsx"));
            Application.Exit();
        }

        private async Task CalculateAccuracy(Q q)
        {
            for (int i = 0; i < q.ExpectedOutputs.Count; i++)
            {
                Match d = Regex.Match(q.DeepSeek, q.ExpectedOutputs[i], RegexOptions.IgnoreCase);
                Match c = Regex.Match(q.ChatGPT, q.ExpectedOutputs[i], RegexOptions.IgnoreCase);
                if (d.Success)
                {
                    q.DeepCorrect += 1;
                    q.OutputDeepCorrect.Add(true);
                }
                else
                    q.OutputDeepCorrect.Add(false);


                if (c.Success)
                {
                    q.ChatCorrect += 1;
                    q.OutputGPTCorrect.Add(true);
                }
                else
                    q.OutputGPTCorrect.Add(false);
            }
            float DeepSeekCorrect = (float)q.DeepCorrect / q.ExpectedOutputs.Count * 100;
            float ChatGPTCorrect = (float)q.ChatCorrect / q.ExpectedOutputs.Count * 100;
            if (DeepSeekCorrect >= 50)
            {
                q.DeepSeekValid = true;
                q.DeepSeekValidReason = "Response had relevant info with a correct rate of >= 50%";
            }
            else
            {
                //because default state for boolean is false, we don't need to specify
                if (q.DeepSeek.Length == 0)
                {
                    q.DeepSeekValidReason = "No Response";
                }
                else
                    q.DeepSeekValidReason = "Low Accuracy";
                //if Correct% is below set amount, see if the response was invalid due to region blocking (like deepseek chinese questions)
                //if not, not relevant response.
            }

            if (ChatGPTCorrect >= 50)
            {
                q.ChatGPTValid = true;
                q.ChatGPTValidReason = "Response had relevant info with a correct rate of >= 50%";
            }
            else
            {
                //because default state for boolean is false, we don't need to specify
                if (q.ChatGPT.Length == 0)
                    q.ChatGPTValidReason = "No Response";
                else
                    q.ChatGPTValidReason = "Low Accuracy";
                //if Correct% is below set amount, see if the response was invalid due to region blocking (like deepseek chinese questions)
                //if not, not relevant response.
            }

            rtbOutput.Text += q.AnalysisString();
        }

        private async Task AskingQuestions(List<Q> qs)
        {
            foreach (var q in qs)
            {
                rtbOutput.AppendText($"Asking question: {q.Question}\n");
                q.DeepSeek = await AskDeepSeek(q.Question);
                q.ChatGPT = await AskChatGPT(q.Question);
                await CalculateAccuracy(q);
            }
        }

        private async Task<string> AskChatGPT(string question)
        {
            ChatCompletion response = await GPTclient.CompleteChatAsync(question);
            string str = "";
            foreach (var r in response.Content)
            {
                str += r.Text.ToString();
            }
            return str;
        }

        private async Task<string> AskDeepSeek(string question)
        {
            var request = new ChatRequest
            {
                Messages = [
                DeepSeek.Core.Models.Message.NewUserMessage(question)
                ],
                // Specify the model
                Model = DeepSeekModels.ChatModel
            };

            var chatResponse = await DEEPclient.ChatAsync(request, new CancellationToken());

            if (chatResponse is not null)
            {
                return chatResponse?.Choices.First().Message?.Content;
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task<List<Q>> LoadQuestions()
        {
            StreamReader sr = new(path);
            List<Q> qs = new();

            string temp = "";

            while ((temp = sr.ReadLine()) != null)
            {
                Q q = new Q();
                //(Question)|(Category)|(Era)|(QuestionType)|(Knowledge)|(Literacy)|(Role)
                string[] l1 = temp.Split('|');
                //(Expected Output 1)|(Expected Output 2)|....(etc)
                q.Question = l1[0];
                q.Category = l1[1];
                q.Era = l1[2];
                q.QuestionType = l1[3];
                q.Knowledge = l1[4];
                q.Literacy = l1[5];
                q.Role = l1[6];

                List<string> ExpectedOutputs = new();
                for (int j = 7; j < l1.Length; j++)
                {
                    ExpectedOutputs.Add(l1[j].Trim());
                }

                q.ExpectedOutputs = ExpectedOutputs;
                qs.Add(q);
            }


            return qs;
        }
    }

    public class Q
    {
        public string Question { get; set; }

        public string ChatGPT { get; set; }
        public string DeepSeek { get; set; }

        public List<string> ExpectedOutputs { get; set; } = new();
        public List<bool> OutputDeepCorrect { get; set; } = new();
        public List<bool> OutputGPTCorrect { get; set; } = new();

        public int ChatCorrect { get; set; } = 0;
        public int DeepCorrect { get; set; } = 0;

        public string Category { get; set; }
        public string Era { get; set; }
        public string QuestionType { get; set; }
        public string Knowledge { get; set; }
        public string Literacy { get; set; }
        public string Role { get; set; }

        public bool DeepSeekValid { get; set; } = false;
        public string DeepSeekValidReason { get; set; }
        public bool ChatGPTValid { get; set; } = false;
        public string ChatGPTValidReason { get; set; }

        public string AnalysisString()
        {
            int dPreview = 0; //if string is too long, shorten it
            if (DeepSeek.Length > 300)
                dPreview = 300;
            else
                dPreview = DeepSeek.Length;

            int cPreview = 0; //if string is too long, shorten it
            if (ChatGPT.Length > 300)
                cPreview = 300;
            else
                cPreview = ChatGPT.Length;
            //deepseek response = this.
            string x = string.Concat("DeepSeek Response: ", DeepSeek.AsSpan(0, dPreview), "...\n");
            x += $"DeepSeek Expected outputs:\n";
            for (int i = 0; i < ExpectedOutputs.Count; i++)
            {
                x += $"\t[{ExpectedOutputs[i]} : {OutputDeepCorrect[i]}]\n";
            }
            x += $"Total Correct: {DeepCorrect} - {(float)DeepCorrect / ExpectedOutputs.Count * 100}%.\n";
            x += $"Validity of response: {DeepSeekValid}\nReason: {DeepSeekValidReason}.\n";
            x += "\n";
            x += string.Concat("ChatGPT Response: ", ChatGPT.AsSpan(0, cPreview), "...\n");
            x += "ChatGPT expected outputs:\n";
            for (int i = 0; i < ExpectedOutputs.Count; i++)
            {
                x += $"\t[{ExpectedOutputs[i]} : {OutputGPTCorrect[i]}]\n";
            }
            x += $"Total Correct: {ChatCorrect} - {(float)ChatCorrect / ExpectedOutputs.Count * 100}%.\n";
            x += $"Validity of response: {ChatGPTValid}\nReason: {ChatGPTValidReason}.\n";
            x += "\n\n";

            return x;
        }
    }
}

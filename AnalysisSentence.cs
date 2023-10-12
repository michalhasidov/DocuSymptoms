
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using Range = Microsoft.Office.Interop.Excel.Range;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace BL
{
    public class AnalisisAndSendSentence
    {


        public static List<State> listStates { get; set; } = new List<State>();
        //משתנים גלובליים  
        public static Workbook wb { get; set; }
        //לסגירת הקובץ

        //רשימה של סופיים
        //public static Dictionary<int, string> dictFinal { get; set; } = new Dictionary<int, string> { };

        //תסמינים לשליחה לdfa
        public static List<string> symptomsList { get; set; } = new List<string>();

        //hashTable לפי האותיות 
        public static HashSymptoms[] symptomsArray { get; set; } = new HashSymptoms[27];


        public static List<string> goodSymptoms { get; set; } = new List<string>();


        //קריאה מאקסל
        public static Range ReadFromFile(string path, int numWS)
        {
            _Application excel = new _Excel.Application();
            wb = excel.Workbooks.Open(path);
            Worksheet wsFull = (Worksheet)wb.Worksheets[numWS];
            return wsFull.UsedRange;
        }



        public static void GeneriDfa()
        {
            string path = @"C:\Users\user nc\Desktop\DocuSymptoms\Data\wordsSymptoms.xlsx";
            Range ws = ReadFromFile(path, 1);
            int currentState = 0;
            int stateLocation;
            string finalState;
            int finalCount;
            listStates = new List<State>();
            int rowCount = ws.Rows.Count;
            int colCount = ws.Columns.Count;
            string word = ws.Cells[1][1].value2;
            char[] wordArr = word.ToCharArray();
            State state = new State();
            state.Id = currentState;
            state.Transition = new Dictionary<char, int>();
            listStates.Add(state);
            currentState++;
            for (int i = 1; i < rowCount + 1; i++)
            {

                finalState = ws.Cells[1][i].value2;
                finalCount = currentState;

                for (int j = 1; j < colCount + 1; j++)
                {
                    if (ws.Cells[i, j].value2 == null) break;
                    word = ws.Cells[i, j].value2;
                    wordArr = word.ToCharArray();
                    stateLocation = 0;
                    for (int k = 0; k < wordArr.Length; k++)
                    {
                        if (listStates[stateLocation].Transition.ContainsKey(wordArr[k]))
                        {
                            stateLocation = listStates[stateLocation].Transition[wordArr[k]];
                        }
                        else
                        {

                            if (j != 1 && k == wordArr.Length - 1)
                            {

                                listStates[stateLocation].Transition.Add(wordArr[k], finalCount - 1);

                            }
                            else
                            {
                                state = new State();
                                state.Transition = new Dictionary<char, int>();
                                listStates.Add(state);
                                listStates[currentState].Id = currentState;
                                listStates[stateLocation].Transition.Add(wordArr[k], currentState);

                                stateLocation = currentState;
                                currentState++;
                            }
                        }


                        if (k == wordArr.Length - 1)
                        {

                            if (j == 1)
                            {

                                listStates[currentState - 1].Final = finalState;
                                listStates[currentState - 2].Id = currentState;
                                finalCount = currentState;

                            }


                        }
                    }
                }

                //הכנסתם לרשימת המצבים הסופיים
                //dictFinal.Add(i, finalState);
            }

            wb.Close();
        }


        public static void ActionDfa(string str)
        {
            char[] letterArr = str.ToCharArray();

            Array.Resize(ref letterArr, letterArr.Length - 2);

            int current = 0;
            int nextState;
            for (int i = 0; i < letterArr.Length; i++)
            {
                if (listStates[current].Transition.TryGetValue(letterArr[i], out nextState))
                {
                    current = nextState;
                    if (listStates[current].Final != null)
                    {
                        symptomsList.Add(listStates[current].Final);
                    }
                }
                else
                    break;
            }

        }

        public static void initHashSymptoms()
        {
            string path = @"C:\Users\user nc\Desktop\DocuSymptoms\Data\hashTable.xlsx";
            Range ws = ReadFromFile(path, 1);
            int i = 1;
            char firstletter;
            int index;
            string sym;
            string symReal;
            HashSymptoms symptom;
            HashSymptoms temp;
            while (ws.Cells[i, 1].value2 != null)

            {
                sym = ws.Cells[i, 1].value2;
                symReal = ws.Cells[i, 2].value2;
                if (symReal == null)
                {
                    symReal = sym;
                }
                symptom = new HashSymptoms();
                symptom.Symptom = sym;
                symptom.RealSymtom = symReal;
                firstletter = sym.Substring(0, 1)[0];
                index = firstletter - 'א';
                if (symptomsArray[index] == null)
                {
                    symptomsArray[index] = symptom;
                }
                else
                {
                    //לעבור עד לסוף הרשימה
                    temp = symptomsArray[index];
                    while (symptomsArray[index].Next != null)
                    {
                        symptomsArray[index] = symptomsArray[index].Next;
                    }
                    symptomsArray[index].Next = symptom;
                    symptomsArray[index] = temp;
                }
                i++;

            }
            wb.Close();

        }
        public static string searchHashSymptoms(int index, string symptom)
        {
            string symptomTemp;
            if (symptomsArray[index].Symptom == symptom)
                return symptomsArray[index].RealSymtom;
            HashSymptoms temp = symptomsArray[index];
            while (symptomsArray[index].Next != null)
            {
                if (symptomsArray[index].Next.Symptom == symptom)
                {
                    symptomTemp = symptomsArray[index].Next.RealSymtom;
                    symptomsArray[index] = temp;

                    return symptomTemp;
                }
                symptomsArray[index] = symptomsArray[index].Next;
            }
            symptomsArray[index] = temp;
            return "no found";
        }

        public static string Analysis(string sentence)
        {

            string[] sentenceArr = sentence.Split(' ');
            for (int i = 0; i < sentenceArr.Length; i++)
            {
                if (sentenceArr[i].StartsWith(",") || sentenceArr[i].StartsWith("ו") || sentenceArr[i].StartsWith("."))
                {
                    sentenceArr[i] = sentenceArr[i].Substring(1);
                }
                if (sentenceArr[i].EndsWith(",") || sentenceArr[i].EndsWith("."))
                {
                    sentenceArr[i] = sentenceArr[i].Substring(0, sentenceArr[i].Length - 1);
                }
            }
            List<string> wordList = new List<string>();
            //שליחה לסקריפט בפייתון להוצאת שורש או ש"ע/תואר טהור 
            var psiRoot = new ProcessStartInfo();
            psiRoot.FileName = @"C:\Users\user nc\AppData\Local\Programs\Python\Python310\python.exe";
            var scriptRoot = @"C:\Users\user nc\Desktop\DocuSymptoms\python\myVisualDoctor\rootWebScraping.py";

            var er = "";
            psiRoot.UseShellExecute = false;
            psiRoot.CreateNoWindow = true;
            psiRoot.RedirectStandardOutput = true;
            psiRoot.RedirectStandardError = true;
            var resultsRoot = "";
            var psiNoun = new ProcessStartInfo();
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("windows-1255");
            psiNoun.StandardOutputEncoding = encoding;
            psiNoun.FileName = @"C:\Users\user nc\AppData\Local\Programs\Python\Python310\python.exe";
            var scriptNoun = @"C:\Users\user nc\Desktop\DocuSymptoms\python\myVisualDoctor\nouns.py";
            psiNoun.UseShellExecute = false;
            psiNoun.CreateNoWindow = true;
            psiNoun.RedirectStandardOutput = true;
            psiNoun.RedirectStandardError = true;
            var resultsNoun = "";


            bool shouldContinue = false;
            List<Thread> threads = new List<Thread>();
            string[] threadsArr = new string[sentenceArr.Length];
            for (int i = 0; i < sentenceArr.Length; i++)
            {
                int inde = i;
                Thread thread = new Thread(() =>
                {


                    psiRoot.StandardOutputEncoding = encoding;
                    psiRoot.RedirectStandardOutput = true;
                    psiRoot.Arguments = $"\"{scriptRoot}\" \"{sentenceArr[inde]}\"";


                    using (Process processRoot = Process.Start(psiRoot))
                    {
                        er = processRoot.StandardError.ReadToEnd();

                        StreamReader reader = processRoot.StandardOutput;
                        Console.WriteLine(reader.CurrentEncoding);
                        Encoding textEncoding = reader.CurrentEncoding;

                        resultsRoot = processRoot.StandardOutput.ReadToEnd();

                      
                    }



                    if (!resultsRoot.Contains("אין תוצאות"))
                    {
                        threadsArr[inde] = resultsRoot;
                        shouldContinue = true;
                    }
                    else
                    {
                        psiNoun.Arguments = $"\"{scriptNoun}\" \"{sentenceArr[inde]}\"";

                        using (Process processNoun = Process.Start(psiNoun))
                        {
                            resultsNoun = processNoun.StandardOutput.ReadToEnd();
                        }


                        if (!resultsNoun.Contains("Noun"))
                        {
                            threadsArr[inde] = resultsNoun;
                            shouldContinue = true;
                        }
                    }
                    if (shouldContinue)
                    {
                        return;
                    }

                });
                threads.Add(thread);
                thread.Start();

            }

            GeneriDfa();
            initHashSymptoms();

            foreach (Thread thread in threads)
            {
                thread.Join(); // Wait for the thread to finish
            }

            for (int i = 0; i < threadsArr.Length; i++)
            {
                if (threadsArr[i] != null)
                {
                    wordList.Add(threadsArr[i]);
                }
            }
            for (int i = 0; i < wordList.Count; i++)
            {
                ActionDfa(wordList[i]);
            }

            //לעבור בhashTable
            char firstletter;
            int index;
            string sym2 = "";



            for (int i = 0; i < symptomsList.Count; i++)
            {
                firstletter = symptomsList[i].Substring(0, 1)[0];
                index = firstletter - 'א';
                if (searchHashSymptoms(index, symptomsList[i]) != "no found")
                {
                    goodSymptoms.Add(searchHashSymptoms(index, symptomsList[i]));
                }
                else
                {
                    if (i + 1 < symptomsList.Count)
                    {
                        sym2 = symptomsList[i] + " " + symptomsList[i + 1];
                    }
                    if (searchHashSymptoms(index, sym2) != "no found")
                    {
                        goodSymptoms.Add(searchHashSymptoms(index, sym2));
                        i++;
                    }
                    else
                    {
                        symptomsList.Remove(symptomsList[i]);
                        i--;
                    }
                }
            }













            //שליחה לפייתון
            var file = @"C:\Users\user nc\Desktop\DocuSymptoms\python\myVisualDoctor\symptoms.txt";
            File.WriteAllLines(file, goodSymptoms);
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\user nc\AppData\Local\Programs\Python\Python310\python.exe";
            var script = @"C:\Users\user nc\Desktop\DocuSymptoms\python\myVisualDoctor\main.py";
            psi.Arguments = $"\"{script}\" \"{file}\"";
            psi.StandardOutputEncoding = encoding;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            var results = "";
            using (Process process = Process.Start(psi))
            {
                results = process.StandardOutput.ReadToEnd();
            }
            if (string.IsNullOrEmpty(results))
            {
                return "תסמינך אינם מצביעים על מחלה מסויימת, אם הינך ממשיך להרגיש תסמינים אלה, פנה לעזרה ויעוץ רפואי.";
            }
            return results;
        }

        public static void Main()
        {

        }


    }


}


































//אתחול DFA
//public void init()
//{
//    dfa.CurrentState = 0;
//    dfa.NumState = 0;
//    dfa.ListStates = new List<State>();
//    string path = "C:\\Users\\user nc\\Desktop\\קודים ניסיונים לפרויקט\\מעברים שורשים.xlsx";

//    Range ws = ReadFromFile(path, 1);
//    int i = 1;


//    State s;
//    for (int l = 0; l < 40; l++)
//    {
//        s = new State();
//        dfa.ListStates.Add(s);
//        dfa.ListStates[l].Id = l;
//        dfa.ListStates[l].Transition = new Dictionary<char, int>();
//    }
//    //כל עוד אין שורה ריקה
//    //תביא לי מכל שורה שלוש משבצות

//    while (ws.Cells[i, 1].value != null)

//    {

//        int a = (int)(ws.Cells[i, 1] as _Excel.Range).Value2;//משבצת ראשונה;
//        char b = (ws.Cells[i, 2] as _Excel.Range).Value2[0];//משבצת שניה;
//        int c = (int)(ws.Cells[i, 3] as _Excel.Range).Value2;// משבצת שלישית

//        dfa.ListStates[a].Transition.Add(b, c);

//        i++;


//    }
//    wb.Close();
//    //מעבר באקסל על מצבים סופיים
//    //הכנסתם לרשימת המצבים הסופיים
//    //לעשות אמת ב-אם סופי ברשימה של כל המצבים
//    i = 1;
//    path = "C:\\Users\\user nc\\Desktop\\קודים ניסיונים לפרויקט\\שורשים סופיים.xlsx";
//    ws = ReadFromFile(path, 1);


//    int final = 0;
//    //כל עוד אין שורה ריקה


//    while (ws.Cells[i, 1].value2 != null)
//    {

//        //תעבור על שורה שורה ותכניס להאם-סופי=אמת
//        final = (int)ws.Cells[i, 1].value2;//התוכן של השורה

//        dfa.ListStates[final].IfFainal = true;

//        //הכנסתם לרשימת המצבים הסופיים
//        int finalState = (int)ws.Cells[i, 1].value2;//מצב 
//        string finalWord = ws.Cells[i, 2].value2;//המילה


//        dictFinal.Add(finalState, finalWord);
//        i++;


//    }
//    wb.Close();


//}

// See https://aka.ms/new-console-template for more information
using System.IO;
namespace JJSPC
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Приветствую. Выберите функцию: \n [O] Открыть файл с расширением .jjs \n [S]Создать тестовый файл .jjs \n [C] Открыть консоль ");
            string key = Console.ReadKey().KeyChar.ToString().ToLower();
            if(key=="o")
            {
                Console.WriteLine("Скопируйте путь к файлу сюда:");
                string path = Console.ReadLine();
                try
                {
                    StreamReader fs = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                    String line;
                    if(path.Substring(path.Length-4)==".jjs")
                    { 
                    while ((line = fs.ReadLine())!=null)
                    {
                        Transcriptor.TranscriptCode(line,false);
                    }
                    Console.WriteLine("Скрипт выполнен.");
                    Console.Read();
                        Main(args);
                    }
                    else
                    {
                        Console.WriteLine("Файл не является скриптом .jjs!");
                        Main(args);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка. \nСообщение: " + ex.Message);
                    Main(args);
                }
            }
            else if(key=="s")
            {
                Console.WriteLine("Создаем тестовый файл...");
                string[] text = { "var a = 1" , "out a" };
                foreach(string line in text)
                {
                    using(StreamWriter sr = new StreamWriter(File.Create("test.jjs"),System.Text.Encoding.Default,1024*1024*32))
                    {
                        sr.WriteLine(line);
                    }
                }
                Console.WriteLine("Успешно!");
                Main(args);
            }
            else if(key=="c")
            {
                JJSConsole.CallConsole();
            }
            else
            {
                Main(args);
            }
        }
    }
    public class Transcriptor
    {
        private static Dictionary<string,string> values = new System.Collections.Generic.Dictionary<string,string>();
        static bool outmode = true;
        public static void TranscriptCode(string line, bool mode = true)
        {
            if (line.StartsWith("var"))
            {
                try
                {
                    if (line.Substring(2).StartsWith("=") || line.Substring(2).StartsWith(";") || line.Substring(2).StartsWith(":") || line.Substring(2).StartsWith("\"") || line.Substring(2).StartsWith("'") || line.Substring(2).StartsWith("\\") || line.Substring(2).StartsWith("/") || line.Substring(2).StartsWith("[") || line.Substring(2).StartsWith("]") || line.Substring(2).StartsWith("{") || line.Substring(2).StartsWith("}") || line.Substring(2).StartsWith("+") || line.Substring(2).StartsWith("-"))
                    {
                        Console.WriteLine("Неверное выражение переменной!\nИсточник: начало обьявления переменной.");
                    }
                    else
                    {
                        int index = line.IndexOf("=");
                        int varindex = line.IndexOf("var") + 3;
                        if (index == -1)
                        {
                            Console.WriteLine("Неверное выражение переменной!\nИсточник: после обьявления названия переменной(вы упустили =)");
                        }
                        else
                        {
                            string name = line.Substring(varindex, index - varindex).Trim();
                            string value = line.Substring(index + 1, line.Length - index - 1);
                            if (value.Contains(" "))
                            {
                                value = line.Substring(index + 2);
                            }
                            values.Add(name, value);
                            Console.WriteLine("Переменной " + name + " присвоено значение " + value);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Переменная уже существует!");
                }
            }
            else if (line.StartsWith("out"))
            {
                string name = line.Substring(4).Trim();
                if (name.StartsWith("off"))
                {
                    outmode = false;
                }
                else
                {
                    if (values.ContainsKey(name))
                    {
                        string val = values[name];
                        Console.WriteLine(val);
                    }
                    else
                    {
                        Console.WriteLine(name);
                        if (outmode)
                        {
                            Console.WriteLine("Если вы искали переменную - она не обьявлена. Используйте: var " + name + "=знач");
                        }
                    }
                }

            }
            else if (line.StartsWith("add"))
            {
                
                string add = "";
                if (line.IndexOf(";") == -1 || line.Length<4)
                {
                    Console.WriteLine("Вы неправильно ввели функцию!(Правильно: add <переменная>;<что добавить>");
                }
                else
                {
                    add = line.Substring(4);
                string[] vars = add.Split(';');
                vars[0] = vars[0].Trim();
                vars[1] = vars[1].Trim();
                if (vars.Length >= 3)
                {
                    Console.WriteLine("Ошибка в написании! Источник: переменные(Можно добавить одну переменную ко второй!)");
                }
                else
                {
                    if (values.ContainsKey(vars[0]))
                    {
                        int sum = 0;
                        int testvar = 0;
                            if (int.TryParse(values[vars[0]], out testvar))
                            {
                                sum += testvar;
                                int additional = 0;
                                if (values.ContainsKey(vars[1]))
                                {
                                    if (int.TryParse(values[vars[1]], out additional))
                                    {
                                        sum += additional;
                                        values[vars[0]] = sum.ToString();
                                        Console.WriteLine("К переменной " + vars[0] + " добавлено " + additional);
                                    }
                                    else
                                    {
                                        values[vars[0]] = values[vars[0]] + values[vars[1]];
                                        Console.WriteLine("К переменной " + vars[0] + " добавлен текст из переменной " + vars[1]);
                                    }
                                }
                                else
                                {
                                    if (int.TryParse(vars[1], out additional))
                                    {
                                        sum += additional;
                                        values[vars[0]] = sum.ToString();
                                        Console.WriteLine("К переменной " + vars[0] + " добавлено " + additional);
                                    }
                                    else
                                    {
                                        values[vars[0]] =values[vars[0]] + vars[1];
                                        Console.WriteLine("К переменной " + vars[0] + " добавлен текст " + vars[1]);
                                    }
                                }
                            }
                            else
                            {
                                if (values.ContainsKey(vars[1]))
                                {
                                    values[vars[0]] += values[vars[1]];
                                    Console.WriteLine("К переменной " + vars[0] + " добавлен текст из переменной " + vars[1]);
                                }
                                else
                                {
                                    values[vars[0]] += vars[1];
                                    Console.WriteLine("К переменной " + vars[0] + " добавлен текст " + vars[1]);
                                }
                            }
                        }
                    else
                    {
                        Console.WriteLine("Переменная " + vars[0] + " не найдена!");
                    }
                }
            }
        }
            else if(line.StartsWith("sub"))
            {
                if(line.Length <4 || line.IndexOf(";")==-1)
                {
                    Console.WriteLine("Вы неверно ввели команду! Правильно: sub <переменная>;<Число/переменная>");
                }
                else
                {
                    string[] vars = line.Substring(4).Split(";");
                    if (vars.Length > 2)
                    {
                        Console.WriteLine("Слишком много аргументов!(Максимум 2)");
                    }
                    else
                    {
                        vars[0] = vars[0].Trim();
                        vars[1] = vars[1].Trim();
                        if (values.ContainsKey(vars[0]))
                        {
                            int sum = 0; 
                            if(int.TryParse(values[vars[0]],out sum))
                            {
                                if(values.ContainsKey(vars[1]))
                                {
                                    int test = 0;
                                    if (int.TryParse(values[vars[1]], out test))
                                    {
                                        sum -= test;
                                        values[vars[0]] = sum.ToString();
                                        Console.WriteLine("Из переменной " + vars[0] + " вычтено число " + test);
                                    }
                                    else
                                    {
                                        Console.WriteLine(vars[1] + " не является числом.");
                                    }
                                }
                                else
                                {
                                    int test = 0;
                                    if (int.TryParse(vars[1], out test))
                                    {
                                        sum -= test;
                                        values[vars[0]] = sum.ToString();
                                        Console.WriteLine("Из переменной " + vars[0] + " вычтено число " + test);
                                    }
                                    else
                                    {
                                        Console.WriteLine(vars[1] + " не является числом.");
                                    }
                                }
                                }
                            else
                            {
                                Console.WriteLine(vars[0] + " не является числом.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Необьявленная переменная " + vars[0] + "!");
                        }
                    }    
                }
            }
            else if(line.StartsWith("unset"))
            {
                if(line.Length<6)
                {
                    Console.WriteLine("Неверно использование выражение! Использование: unset <Переменная>");
                }
                else
                {
                    string variable = line.Substring(5).Trim();
                    if (values.ContainsKey(variable))
                    {
                        values.Remove(variable);
                        Console.WriteLine("Переменная " + variable + " была удалена.");
                    }
                    else Console.WriteLine("Переменная не найдена!");
                }
            }
            else if(line.StartsWith("set"))
            {
                if(line.Length<4)
                {
                   Console.WriteLine("Неверно использовано выражение! Правильное использование: set <Переменная>;<Значение>");
                }
                else
                {
                    if(line.IndexOf(";")!=-1)
                    {
                        string main = line.Substring(4);
                        int index = main.IndexOf(";");
                        string var = main.Substring(0, index);
                        string value = main.Substring(index + 1, main.Length - index-1);
                        if(values.ContainsKey(var))
                        {
                            if(values.ContainsKey(value))
                            {
                                values[var] = values[value];
                                Console.WriteLine("Вы установили переменной " + var + " значение от переменной " + value);
                            }
                            else
                            {
                                values[var] = value;
                                Console.WriteLine("Вы установили переменной " + var + " значение " + value);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Переменной " + var + " не существует!");
                        }
                    }
                }
            }
            else if(line.StartsWith("exit"))
            {
                Console.WriteLine("Выход из оболочки...");
                foreach(string key in values.Keys)
                {
                    values.Remove(key);
                }
                Thread.Sleep(800);
                Program.Main(new string[] { });
            }
            else
            {
                Console.WriteLine("Неверная команда! Список команд: var name=value;out value");
            }
            if(mode)
            {
                JJSConsole.Continue();
            }
        }

    }
    public class JJSConsole
    {
        public static void CallConsole()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Это оболочка JJS. Здесь, как и в Python, вы можете выполнять команды.\n Введите команду:");
            string line = Console.ReadLine();
            Transcriptor.TranscriptCode(line);
        }
        public static void Continue()
        {
            string line = Console.ReadLine();
            Transcriptor.TranscriptCode(line);
        }
    }
}
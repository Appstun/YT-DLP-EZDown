using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using static GUI.Interface.Option;
using static GUI.Interface.Option.TableOption;
using static GUI.Interface.ScreenType.TableScreen;

internal class GUI
{
    private readonly static int maxShownItems = 10;
    public enum InterfaceReturns
    {
        Yes,
        No,
        Continued
    }

    public enum InterfaceYesno
    {
        Yes,
        No
    }

    /// <summary>Ist fürs Erstellen eines GUIs gedacht.</summary>
    public class Interface
    {
        public readonly long id = createID();
        public List<ScreenType> interfaceScreens { get; private set; } = new();
        /// <summary>Eine Liste mit allen Screens, bei den erfolgreich was ausgewählt ist.</summary>
        public List<ScreenType> outputResults { get; private set; } = new();

        /// <summary>Ist fürs Erstellen eines GUIs gedacht.</summary>
        /// <param name="interfaceScreens">Screens angeben, was das GUI nach einander abarbeitet.</param>
        public Interface(params ScreenType[] interfaceScreens) { this.interfaceScreens.AddRange(interfaceScreens); }

        private ScreenType? currentScreen = null;
        private int currentOptionSelected = 0;
        private bool isShown = false;

        /// <summary>Zeigt das GUI an.</summary>
        /// <param name="screenId">[OPTIONAL] Um anzugeben, mit welchen Screen das GUI startet.</param>
        public void showGUI(long? screenId = null)
        {
            if (!isShown)
            {
                outputResults = new();
                currentOptionSelected = 0;
                currentScreen = null;

                show(screenId);
            }
        }
        private void show(long? screenId = null)
        {
            hide();

            if (currentScreen != null)
            {
                foreach (var screen in interfaceScreens)
                {
                    if (screen.id == screenId)
                    {
                        currentScreen = screen;
                        break;
                    }
                }
            }
            else currentScreen = interfaceScreens[0];
            currentOptionSelected = 0;

            if (currentScreen is ScreenType.OptionsScreen)
            {
                ScreenType.OptionsScreen currentOptionscreen = (ScreenType.OptionsScreen)currentScreen;
                currentOptionSelected = currentOptionscreen.interfaceOptions.IndexOf(currentOptionscreen.defaultOption);
            }
            else if (currentScreen is ScreenType.YesNoScreen)
            {
                ScreenType.YesNoScreen currentYesnoscreen = (ScreenType.YesNoScreen)currentScreen;
                if (currentYesnoscreen.defaultOption == InterfaceYesno.Yes) currentOptionSelected = 0;
                if (currentYesnoscreen.defaultOption == InterfaceYesno.No) currentOptionSelected = 1;
            }

            Console.Clear();
            Console.ResetColor();

            isShown = true;
            updateGUI();
            inputLoop();
        }

        /// <summary>Beende das GUI.</summary>
        public void hideGUI()
        {
            if (isShown) hide();
        }
        private void hide()
        {
            isShown = false;
        }

        private void updateGUI()
        {
            if (currentScreen == null) return;
            if (currentScreen is ScreenType.OptionsScreen)
            {
                ScreenType.OptionsScreen currentOptionscreen = (ScreenType.OptionsScreen)currentScreen;
                int optionCount = currentOptionscreen.interfaceOptions.Count;

                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentOptionscreen.titleText, true);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Steuerung: F1");
                Console.WriteLine();

                if (currentOptionscreen.infoText != null)
                {
                    writeColoredLine(infoTextFormat(currentOptionscreen.infoText), true);
                    Console.WriteLine();
                }

                int maxOptionlenght = 0;
                foreach (var option in currentOptionscreen.interfaceOptions)
                {
                    int textLength = option.displayText.Length;
                    if (textLength > maxOptionlenght) maxOptionlenght = textLength;
                }

                int genStart = currentOptionSelected;
                if (genStart + maxShownItems >= optionCount) genStart = optionCount - maxShownItems;
                int genEnd = currentOptionSelected + maxShownItems;
                int shown = 0;

                string moreOnBegin = "";
                if (currentOptionSelected >= 1 && optionCount >= maxShownItems) moreOnBegin = "...";
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"    {moreOnBegin}");
                for (int i = 0; i < optionCount; i++)
                {
                    if ((i >= genStart && i < genEnd || optionCount <= maxShownItems))
                    {
                        Option.SelectOption option = currentOptionscreen.interfaceOptions[i];

                        Console.Write($"   ");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        if (i == currentOptionSelected)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.WriteLine($" {FillStringWithSpaces(option.displayText, maxOptionlenght)} ");
                        Console.ResetColor();

                        shown++;
                    }
                }

                string moreOnEnd = "";
                if (currentOptionSelected <= optionCount - maxShownItems && shown == maxShownItems) moreOnEnd = "...";
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"    {moreOnEnd}");

                Console.ResetColor();
            }
            else if (currentScreen is ScreenType.InfoScreen)
            {
                ScreenType.InfoScreen currentInfoscreen = (ScreenType.InfoScreen)currentScreen;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentInfoscreen.titleText, true);

                Console.WriteLine();
                Console.ResetColor();

                writeColoredLine(infoTextFormat(currentInfoscreen.infoText), true);

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Drücke '{currentInfoscreen.continueKey}', um fortzufahren.");
            }
            else if (currentScreen is ScreenType.TextinputScreen)
            {
                ScreenType.TextinputScreen currentTextinputscreen = (ScreenType.TextinputScreen)currentScreen;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentTextinputscreen.titleText, true);

                Console.WriteLine();
                Console.ResetColor();

                if (currentTextinputscreen.infoText != null)
                {
                    writeColoredLine(infoTextFormat(currentTextinputscreen.infoText), true);
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Bestätige die Eingabe mit '{ConsoleKey.Enter}'. {(!currentTextinputscreen.canEmpty ? "(Darf nicht leer sein)" : "")}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Eingabe: ");
                Console.ForegroundColor = ConsoleColor.Yellow;

            }
            else if (currentScreen is ScreenType.YesNoScreen)
            {
                ScreenType.YesNoScreen currentYesnoscreen = (ScreenType.YesNoScreen)currentScreen;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentYesnoscreen.titleText, true);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Steuerung: F1");

                Console.WriteLine();
                Console.ResetColor();

                if (currentYesnoscreen.infoText != null)
                {
                    writeColoredLine(infoTextFormat(currentYesnoscreen.infoText), true);
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                if (currentOptionSelected == 0)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(" [ JA ] ");

                Console.ResetColor();
                Console.Write("                    ");

                Console.ForegroundColor = ConsoleColor.Blue;
                if (currentOptionSelected == 1)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(" [ NEIN ] ");
                Console.WriteLine();
                Console.ResetColor();
            }
            else if (currentScreen is ScreenType.DateTimeScreen)
            {
                ScreenType.DateTimeScreen currentDatetimescreen = (ScreenType.DateTimeScreen)currentScreen;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentDatetimescreen.titleText, true);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Steuerung: F1");

                Console.WriteLine();
                Console.ResetColor();

                if (currentDatetimescreen.infoText != null)
                {
                    writeColoredLine(infoTextFormat(currentDatetimescreen.infoText), true);
                    Console.WriteLine();
                }

                Console.Write("            ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                if (currentDatetimescreen.selectedNumber == 0) Console.Write("__"); else Console.Write("  ");
                Console.Write(" ");
                if (currentDatetimescreen.selectedNumber == 1) Console.Write("__"); else Console.Write("  ");
                Console.Write(" ");
                if (currentDatetimescreen.selectedNumber == 2) Console.Write("____"); else Console.Write("    ");
                if (currentDatetimescreen.withTime)
                {
                    Console.Write("   ");
                    if (currentDatetimescreen.selectedNumber == 3) Console.Write("__"); else Console.Write("  ");
                    Console.Write(" ");
                    if (currentDatetimescreen.selectedNumber == 4) Console.Write("__"); else Console.Write("  ");
                    Console.Write(" ");
                    if (currentDatetimescreen.selectedNumber == 5 && currentDatetimescreen.timeWithSeconds) Console.Write("__"); else Console.Write("  ");
                }
                Console.ResetColor();

                Console.WriteLine();
                Console.Write("            ");
                for (int i = 0; i <= 5; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if (currentDatetimescreen.selectedNumber == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    if (i == 0) Console.Write(currentDatetimescreen.selectedDateTime.Day.ToString("00"));
                    if (i == 1) Console.Write(currentDatetimescreen.selectedDateTime.Month.ToString("00"));
                    if (i == 2) Console.Write(currentDatetimescreen.selectedDateTime.Year.ToString("0000"));
                    if (currentDatetimescreen.withTime)
                    {
                        if (i == 3) Console.Write(currentDatetimescreen.selectedDateTime.Hour.ToString("00"));
                        if (i == 4) Console.Write(currentDatetimescreen.selectedDateTime.Minute.ToString("00"));
                        if (i == 5 && currentDatetimescreen.withTime && currentDatetimescreen.timeWithSeconds) Console.Write(currentDatetimescreen.selectedDateTime.Second.ToString("00"));
                    }
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    if (i == 0 || i == 1) Console.Write(".");
                    if (i == 2) Console.Write("   ");
                    if (i == 3 && currentDatetimescreen.withTime) Console.Write(":");
                    if (i == 4 && currentDatetimescreen.withTime && currentDatetimescreen.timeWithSeconds) Console.Write(":");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
            else if (currentScreen is ScreenType.TableScreen)
            {
                ScreenType.TableScreen currentTablescreen = (ScreenType.TableScreen)currentScreen;

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writeColoredLine(currentTablescreen.titleText, true);
                Console.ResetColor();
                Console.WriteLine();

                if (currentTablescreen.infoText != null)
                {
                    writeColoredLine(infoTextFormat(currentTablescreen.infoText), true);
                    Console.WriteLine();
                }

                List<Option.TableOption> columns = currentTablescreen.interfaceOptions;
                List<TableContent> contents = currentTablescreen.getContentlist();

                Dictionary<int, int> columnWidths = new Dictionary<int, int>();
                foreach (var column in columns) columnWidths[columns.IndexOf(column)] = column.title.Length;

                foreach (var content in contents)
                {
                    int columnN = content.columnNumber;
                    string contentS = content.fieldContent.ToString();

                    if (columnN < columns.Count)
                    {
                        if (columns[columnN] is Column && columnWidths.ContainsKey(columnN) && columnWidths[columnN] < contentS.Length)
                        {
                            if (columnWidths.ContainsKey(columnN) && columnWidths[columnN] < contentS.Length) columnWidths[columnN] = contentS.Length;
                        }
                        else if (columns[columnN] is OptionalColumn)
                        {
                            int columnIndex = columns.IndexOf(columns[columnN]);

                            if (!columnWidths.ContainsKey(columnIndex))
                            {
                                columnWidths[columnIndex] = columns[columnN].title.Length;
                            }
                            else
                            {
                                int titleWidth = columns[columnN].title.Length;
                                if (titleWidth > columnWidths[columnIndex])
                                {
                                    columnWidths[columnIndex] = titleWidth;
                                }
                            }

                            if (contentS.Length > columnWidths[columnIndex]) columnWidths[columnIndex] = contentS.Length;
                        }

                    }
                }

                string divider = "+" + string.Join("+", columnWidths.Values.Select(width => new string('-', width + 2))) + "+";

                int consoleWidth = Console.WindowWidth;
                if (divider.Length > consoleWidth)
                {
                    for (int i = columns.Count - 1; i >= 0; i--)
                    {
                        var column = columns[i];
                        divider = "+" + string.Join("+", columnWidths.Values.Select(width => new string('-', width + 2))) + "+";

                        if (column is OptionalColumn && divider.Length > consoleWidth)
                        {
                            int columnIndex = columns.IndexOf(column);
                            if (columnWidths.ContainsKey(columnIndex))
                            {
                                columnWidths.Remove(columnIndex);
                            }
                        }
                    }

                    divider = "+" + string.Join("+", columnWidths.Values.Select(width => new string('-', width + 2))) + "+";
                }


                Console.WriteLine(divider);
                Console.Write("|");
                foreach (var column in columns)
                {
                    int columnIndex = columns.IndexOf(column);
                    if (columnWidths.ContainsKey(columnIndex))
                    {
                        Console.Write(" ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(column.title.PadRight(columnWidths[columnIndex]), false);
                        Console.ResetColor();
                        Console.Write(" |");
                    }
                }
                Console.WriteLine("\n" + divider);

                if (contents.Count == 0)
                {
                    string emptyMessage = "Keine Daten";
                    int totalWidth = divider.Length - 2;
                    int padding = (totalWidth - emptyMessage.Length) / 2;
                    Console.Write("|" + new string(' ', padding) + emptyMessage + new string(' ', totalWidth - padding - emptyMessage.Length) + "|\n");
                }
                else
                {
                    Dictionary<int, Dictionary<int, string>> contentByRowAndColumn = new Dictionary<int, Dictionary<int, string>>();

                    foreach (var content in contents)
                    {
                        int rowN = content.rowNumber;
                        int columnN = content.columnNumber;
                        string contentS = content.fieldContent.ToString();

                        if (!contentByRowAndColumn.ContainsKey(rowN)) contentByRowAndColumn[rowN] = new Dictionary<int, string>();

                        if (columnWidths.ContainsKey(columnN)) contentByRowAndColumn[rowN][columnN] = contentS;
                    }

                    bool isGray = false;
                    foreach (var row in contentByRowAndColumn)
                    {
                        Console.Write("|");
                        foreach (var column in columns)
                        {
                            int columnIndex = columns.IndexOf(column);
                            if (columnWidths.ContainsKey(columnIndex))
                            {
                                string contentS = row.Value.ContainsKey(columnIndex) ? row.Value[columnIndex] : "";

                                Console.Write(" ");
                                if (isGray) Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(contentS.PadRight(columnWidths[columnIndex]), false);
                                Console.ResetColor();
                                Console.Write(" |");
                            }
                        }
                        Console.WriteLine();
                        isGray = !isGray;
                    }
                }
                Console.WriteLine(divider);

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Drücke '{ConsoleKey.Spacebar}' oder '{ConsoleKey.Enter}', um fortzufahren.");

            }
        }

        private void inputLoop()
        {
            while (isShown)
            {
                ConsoleKeyInfo keyInfo = new ConsoleKeyInfo(Char.Parse("A"), ConsoleKey.A, false, false, false);
                if (currentScreen is not ScreenType.TextinputScreen) keyInfo = Console.ReadKey(true);
                bool disableFinishKeys = false;
                bool overrideFinishKeys = false;

                if (currentScreen is ScreenType.OptionsScreen)
                {
                    ScreenType.OptionsScreen currentOptionscreen = (ScreenType.OptionsScreen)currentScreen;

                    if (keyInfo.Key == ConsoleKey.UpArrow) currentOptionSelected--;
                    if (keyInfo.Key == ConsoleKey.DownArrow) currentOptionSelected++;

                    if (currentOptionSelected < 0) currentOptionSelected = currentOptionscreen.interfaceOptions.Count - 1;
                    if (currentOptionSelected >= currentOptionscreen.interfaceOptions.Count) currentOptionSelected = 0;

                    currentScreen.outputResult = currentOptionscreen.interfaceOptions[currentOptionSelected];

                    if (keyInfo.Key == ConsoleKey.F1)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("-- Steuerung für diesen Screen --");
                        Console.WriteLine("");

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Taste                                     Befehl");
                        Console.ResetColor();
                        Console.WriteLine("Pfeiltaste nach oben                      eine Option nach oben");
                        Console.WriteLine("Pfeiltaste nach unten                     eine Option nach unten");
                        Console.WriteLine("Entertaste oder Leertaste                 Option bestätigen");

                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Drücke eine beliebige Taste, um zum Screen zurück zu kehren.");
                        Console.ReadKey();
                    }
                }
                else if (currentScreen is ScreenType.InfoScreen)
                {
                    ScreenType.InfoScreen currentInfoscreen = (ScreenType.InfoScreen)currentScreen;
                    disableFinishKeys = true;
                    if (keyInfo.Key == currentInfoscreen.continueKey)
                    {
                        disableFinishKeys = false;
                        overrideFinishKeys = true;

                        currentScreen.outputResult = InterfaceReturns.Continued;
                    }
                }
                else if (currentScreen is ScreenType.TextinputScreen)
                {
                    ScreenType.TextinputScreen currentTextinputscreen = (ScreenType.TextinputScreen)currentScreen;
                    string? input = Console.ReadLine();
                    if ((string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) && !currentTextinputscreen.canEmpty) disableFinishKeys = true;
                    else
                    {
                        overrideFinishKeys = true;
                        currentScreen.outputResult = input;
                    }
                }
                else if (currentScreen is ScreenType.YesNoScreen)
                {
                    ScreenType.YesNoScreen currentYesnoscreen = (ScreenType.YesNoScreen)currentScreen;

                    if (keyInfo.Key == ConsoleKey.RightArrow) currentOptionSelected++;
                    if (keyInfo.Key == ConsoleKey.LeftArrow) currentOptionSelected--;

                    if (currentOptionSelected > 1) currentOptionSelected = 0;
                    if (currentOptionSelected < 0) currentOptionSelected = 1;

                    if (currentOptionSelected == 0) currentScreen.outputResult = InterfaceReturns.Yes;
                    if (currentOptionSelected == 1) currentScreen.outputResult = InterfaceReturns.No;

                    if (keyInfo.Key == ConsoleKey.F1)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("-- Steuerung für diesen Screen --");
                        Console.WriteLine("");

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Taste                                     Befehl");
                        Console.ResetColor();
                        Console.WriteLine("Pfeiltaste nach link oder rechts          Option wechseln");
                        Console.WriteLine("Entertaste oder Leertaste                 Option bestätigen");

                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Drücke eine beliebige Taste, um zum Screen zurück zu kehren.");
                        Console.ReadKey();
                    }
                }
                else if (currentScreen is ScreenType.DateTimeScreen)
                {
                    ScreenType.DateTimeScreen currentDatetimescreen = (ScreenType.DateTimeScreen)currentScreen;

                    if (keyInfo.Key == ConsoleKey.RightArrow) currentDatetimescreen.selectedNumber++;
                    if (keyInfo.Key == ConsoleKey.LeftArrow) currentDatetimescreen.selectedNumber--;

                    int num = 0;
                    if (keyInfo.Key == ConsoleKey.UpArrow) num = 1;
                    if (keyInfo.Key == ConsoleKey.DownArrow) num = -1;

                    if (currentDatetimescreen.withTime)
                    {
                        if (currentDatetimescreen.timeWithSeconds)
                        {
                            if (currentDatetimescreen.selectedNumber > 5) currentDatetimescreen.selectedNumber = 0;
                            if (currentDatetimescreen.selectedNumber < 0) currentDatetimescreen.selectedNumber = 5;
                        }
                        else
                        {
                            if (currentDatetimescreen.selectedNumber > 4) currentDatetimescreen.selectedNumber = 0;
                            if (currentDatetimescreen.selectedNumber < 0) currentDatetimescreen.selectedNumber = 4;
                        }
                    }
                    else
                    {
                        if (currentDatetimescreen.selectedNumber > 2) currentDatetimescreen.selectedNumber = 0;
                        if (currentDatetimescreen.selectedNumber < 0) currentDatetimescreen.selectedNumber = 2;
                    }

                    if (currentDatetimescreen.selectedNumber == 0) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddDays(num);
                    if (currentDatetimescreen.selectedNumber == 1) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddMonths(num);
                    if (currentDatetimescreen.selectedNumber == 2) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddYears(num);
                    if (currentDatetimescreen.selectedNumber == 3) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddHours(num);
                    if (currentDatetimescreen.selectedNumber == 4) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddMinutes(num);
                    if (currentDatetimescreen.selectedNumber == 5) currentDatetimescreen.selectedDateTime = currentDatetimescreen.selectedDateTime.AddSeconds(num);

                    currentScreen = currentDatetimescreen;
                    currentScreen.outputResult = currentDatetimescreen.selectedDateTime;

                    if (keyInfo.Key == ConsoleKey.F1)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("-- Steuerung für diesen Screen --");
                        Console.WriteLine("");

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Taste                                     Befehl");
                        Console.ResetColor();
                        Console.WriteLine("Pfeiltaste nach oben                      +1 bei ausgewählter Zahl");
                        Console.WriteLine("Pfeiltaste nach unten                     -1 bei ausgewählter Zahl");
                        Console.WriteLine("Pfeiltaste nach links                     eine Option nach links");
                        Console.WriteLine("Pfeiltaste nach rechts                    eine Option nach rechts");
                        Console.WriteLine("Entertaste oder Leertaste                 Option bestätigen");

                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Drücke eine beliebige Taste, um zum Screen zurück zu kehren.");
                        Console.ReadKey();
                    }
                }
                else if (currentScreen is ScreenType.TableScreen)
                {
                    ScreenType.TableScreen currentTablescreen = (ScreenType.TableScreen)currentScreen;

                    currentScreen.outputResult = InterfaceReturns.Continued;
                }


                if (!disableFinishKeys && (overrideFinishKeys || keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Spacebar))
                {
                    this.outputResults.Add(currentScreen);

                    int index = interfaceScreens.IndexOf(currentScreen);
                    if (index == interfaceScreens.Count - 1)
                    {
                        hide();
                        break;
                    }
                    else
                    {
                        show(interfaceScreens[index + 1].id);
                        break;
                    }
                }
                else updateGUI();
            }
        }

        /// <summary>Die Klasse "Option" ist nur ein Seperator und hat kein Nutzen.</summary>
        public class ScreenType
        {
            public readonly long id = createID();
            public string identifier { get; private set; }
            public object? outputResult { get; set; } = null;

            public string titleText { get; private set; }
            public string? infoText { get; private set; } = null;

            /// <summary>Ist unrelevant und bringt dir nicht wirklich was. :)</summary>
            private ScreenType(string identifier, string title, string? infoText = null)
            {
                this.identifier = identifier;
                this.titleText = title;
                this.infoText = infoText;
            }

            public class InfoScreen : ScreenType
            {
                public ConsoleKey continueKey { get; private set; }

                /// <summary>Zum Erstellen eines Screens verwenden, um Informationen anzuzeigen.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Wird oben als Titel angezeigt.</param>
                /// <param name="infoText">Kleiner Text für Informationen.</param>
                /// <param name="consoleKey">[OPTIONAL] Gebe an, welchen Tastatur-Knopf man drpcken muss um fortzufahren. (Standard: 'Spacebar')</param>
                public InfoScreen(string identifier, string title, string infoText, ConsoleKey consoleKey = ConsoleKey.Spacebar) : base(identifier, title, infoText)
                {
                    this.continueKey = consoleKey;
                    this.titleText = title;
                    this.infoText = infoText;
                }
            }

            public class TextinputScreen : ScreenType
            {
                public bool canEmpty { get; private set; }
                /// <summary>Zum Erstellen eines Screens verwenden, für Nutzer-Texteingaben.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Wird oben als Titel angezeigt.</param>
                /// <param name="canEmpty">Angeben, ob das Textfeld leer sein darf.</param>
                /// <param name="infoText">[OPTIONAL] Kleiner Text für Informationen.</param>
                public TextinputScreen(string identifier, string title, bool canEmpty, string? infoText = null) : base(identifier, title, infoText)
                {
                    this.canEmpty = canEmpty;
                    this.titleText = title;
                    this.infoText = infoText;
                }
            }

            public class YesNoScreen : ScreenType
            {
                public InterfaceYesno defaultOption { get; private set; }

                /// <summary>Zum Erstellen eines Screens verwenden, für eine Ja-Nein-Abfrage vom Nutzer.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Wird oben als Titel angezeigt.</param>
                /// <param name="infoText">[OPTIONAL] Kleiner Text für Informationen.</param>
                public YesNoScreen(string identifier, string title, string? infoText = null) : base(identifier, title, infoText)
                {
                    this.titleText = title;
                    this.infoText = infoText;
                    this.defaultOption = InterfaceYesno.Yes;
                }

                /// <summary>Setzt die standartmäßige Auswahl.</summary>
                public void setDefault(InterfaceYesno newInterfaceYesno)
                {
                    this.defaultOption = newInterfaceYesno;
                }

            }

            public class DateTimeScreen : ScreenType
            {
                public int selectedNumber { get; set; } = 0;
                public DateTime selectedDateTime { get; set; }
                public bool withTime { get; private set; }
                public bool timeWithSeconds { get; private set; }
                public DateTime defaultDatetime { get; private set; }

                /// <summary>Zum Erstellen eines Screens verwenden, für einer Datum-Zeit-Eingabe vom Nutzer.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Wird oben als Titel angezeigt.</param>
                /// <param name="withTime">Setzen, ob man nur das Datum einstellt oder auch mit Zeit.</param>
                /// <param name="timeWithSeconds">[OPTIONAL] Setzen, ob die Zeit (wenn aktiv) auch Sekunden anzeigen soll.</param>
                /// <param name="infoText">[OPTIONAL] Kleiner Text für Informationen.</param>
                /// <param name="defaultDatetime">[OPTIONAL] Das Start-Datum setzen (sonst ist standartgemäß das aktuelle Datum).</param>
                public DateTimeScreen(string identifier, string title, bool withTime, bool timeWithSeconds = false, string? infoText = null, DateTime? defaultDatetime = null) : base(identifier, title, infoText)
                {
                    this.titleText = title;
                    this.infoText = infoText;
                    this.withTime = withTime;
                    this.timeWithSeconds = timeWithSeconds;

                    this.defaultDatetime = defaultDatetime == null ? DateTime.Now : (DateTime)defaultDatetime;
                    if (!this.withTime)
                    {
                        this.defaultDatetime = this.defaultDatetime.AddHours(-this.defaultDatetime.Hour);
                        this.defaultDatetime = this.defaultDatetime.AddMinutes(-this.defaultDatetime.Minute);
                    }
                    if (!this.timeWithSeconds) this.defaultDatetime = this.defaultDatetime.AddSeconds(-this.defaultDatetime.Second);

                    this.selectedDateTime = this.defaultDatetime;
                }
            }

            public class OptionsScreen : ScreenType
            {
                public List<Option.SelectOption> interfaceOptions { get; private set; } = new();

                public Option.SelectOption defaultOption { get; private set; }

                /// <summary>Zum Erstellen eines Screens verwenden, damit der Nutzer sich zwischen mehreren Optionen entscheiden soll.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Kann man als Titel sehen und sag dem Nutzer, für was er eine Option wählen soll.</param>
                /// <param name="interfaceOptions">Angabe aller Optionen, die im Screen zu sehen sein sollen</param>
                public OptionsScreen(string identifier, string title, params Option.SelectOption[] interfaceOptions) : base(identifier, title)
                {
                    this.titleText = title;
                    this.interfaceOptions.AddRange(interfaceOptions);
                    this.defaultOption = this.interfaceOptions[0];
                }

                /// <summary>Setzt die standartmäßige Auswahl.</summary>
                /// <param name="optionId">ID von einer Option von der "interfaceOptions"-Liste.</param>
                public void setDefault(long optionId)
                {
                    foreach (var option in interfaceOptions)
                    {
                        if (option.id == optionId)
                        {
                            this.defaultOption = option;
                            break;
                        }
                    }
                }

                /// <summary>Setzt die standartmäßige Auswahl.</summary>
                /// <param name="optionPosition">Position von einer Option in der "interfaceOptions"-Liste.</param>
                public void setDefault(int optionPosition)
                {
                    if (optionPosition > 0 && optionPosition < interfaceOptions.Count)
                    {
                        this.defaultOption = interfaceOptions[optionPosition];
                    }
                }

                /// <summary>Kleiner Text für Informationen.</summary>
                public void setInfotext(string infotext)
                {
                    this.infoText = infotext;
                }
            }

            public class TableScreen : ScreenType
            {
                public List<Option.TableOption> interfaceOptions { get; private set; } = new();
                private List<TableContent> tableContents = new();

                /// <summary>Zum Erstellen eines Screens verwenden, um eine Tabelle mit Kontent dem Nutzer anzuzeigen.</summary>
                /// <param name="identifier">Wird nicht vom Code verwendet, aber damit kann man es für Strings besser zuordnen.</param>
                /// <param name="title">Kann man als Titel sehen und sag dem Nutzer, für was er eine Option wählen soll.</param>
                /// <param name="interfaceOptions">Angabe aller Optionen, die im Screen zu sehen sein sollen</param>
                public TableScreen(string identifier, string title, params Option.TableOption[] interfaceOptions) : base(identifier, title)
                {
                    this.interfaceOptions.AddRange(interfaceOptions);
                }

                /// <summary>Fügt ein Feld zur Tabelle hinzu.</summary>
                /// <param name="rowNumber">Position X (Zeile)</param>
                /// <param name="columnNumber">Position Y (Spalte)</param>
                /// <param name="fieldContent">Was im Feld als String angezeigt werden soll.</param>
                /// <returns>ID des Feldes.</returns>
                public long? addContent(int rowNumber, int columnNumber, object fieldContent)
                {
                    if (columnNumber < this.interfaceOptions.Count)
                    {
                        TableContent content = new TableContent(rowNumber, columnNumber, fieldContent);
                        tableContents.Add(content);
                        return content.id;
                    }
                    else return null;
                }

                /// <summary>Entfernt ein Feld aus der Tabelle anhand deren ID.</summary>
                /// <param name="id">ID des zu entfernenden Feldes. (Return von "addContent" oder steht auch in der Liste)</param>
                public void removeContent(long id)
                {
                    foreach (var item in tableContents)
                    {
                        if (item.id == id)
                        {
                            tableContents.Remove(item);
                            break;
                        }
                    }
                }

                /// <summary>Gibt die Liste der Felder zurück</summary>
                /// <returns>Felderliste. (Jedes Feld hat seine eigene ID)</returns>
                public List<TableContent> getContentlist() { return tableContents; }

                /// <summary>Kleiner Text für Informationen.</summary>
                public void setInfotext(string infotext)
                {
                    this.infoText = infotext;
                }

                /// <summary>Dies wird für die Felder verwendet ... unbedeutend.</summary>
                public class TableContent
                {
                    public readonly long id = createID();
                    public int rowNumber { get; private set; }
                    public int columnNumber { get; private set; }
                    public object fieldContent { get; private set; }
                    public TableContent(int rowNumber, int columnNumber, object fieldContent)
                    {
                        this.rowNumber = rowNumber;
                        this.columnNumber = columnNumber;
                        this.fieldContent = fieldContent;
                    }
                }
            }
        }

        /// <summary>Die Klasse "Option" ist nur ein Seperator und hat kein Nutzen.</summary>
        public class Option
        {
            public class SelectOption
            {
                public readonly long id = createID();
                public string displayText { get; private set; }
                public object? value { get; private set; }

                /// <summary>Zum Erstellen von Optionen für InterfaceOptionsscreen verwenden.</summary>
                /// <param name="displayText">Was sichtbar für den Nutzer angezeigt wird.</param>
                /// <param name="value">Wird zwar für nichts verwendet, hilft dir aber vielleicht.</param>
                public SelectOption(string displayText, object? value = null)
                {
                    this.displayText = displayText;
                    this.value = value;
                }
            }

            /// <summary>Die Klasse "TableOption" ist nur ein Seperator und hat kein Nutzen.</summary>
            public class TableOption
            {
                public readonly long id = createID();
                public string identifier { get; private set; }
                public string title { get; set; }

                private TableOption(string identifier, string title)
                {
                    this.identifier = identifier;
                    this.title = title;
                }

                public class OptionalColumn : TableOption
                {
                    public OptionalColumn(string identifier, string title) : base(identifier, title) { }
                }

                public class Column : TableOption
                {
                    public Column(string identifier, string title) : base(identifier, title) { }
                }
            }
        }
    }

    private static long createID()
    {
        long timestamp = (long)Math.Round(((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds) / 1000);
        return timestamp + new Random().Next(10000, 99999);
    }

    private static string FillStringWithSpaces(string input, int maxLength)
    {
        if (input.Length >= maxLength) return input;
        return input + new string(' ', maxLength - input.Length);
    }

    private static string infoTextFormat(string text)
    {
        string spacer = "  ";
        text = $"{spacer}{text.Replace("\n", $"\n{spacer}")}";
        return text;
    }

    /// <summary>Kannst du in den Interface-Screens bei Titel und Info-Texten in Strings angeben, um Farben zu verwenden</summary>
    public enum ColorCode
    {
        Black,
        DarkBlue,
        DarkGreen,
        DarkCyan,
        DarkRed,
        DarkMagenta,
        DarkYellow,
        Gray,
        DarkGray,
        Blue,
        Green,
        Cyan,
        Red,
        Magenta,
        Yellow,
        White,
        Reset
    }

    /// <summary>Farbige Konsolen-Zeile anzeigen.</summary>
    /// <remarks>
    /// §0 = ColorCode.Black = Schwarz;
    /// §1 = ColorCode.DarkBlue = Dunkelblau;
    /// §2 = ColorCode.DarkGreen = Dunkelgrün;
    /// §3 = ColorCode.DarkCyan = Dunkelcyan;
    /// §4 = ColorCode.DarkRed = Dunkelrot;
    /// §5 = ColorCode.DarkMagenta = Dunkelmagenta;
    /// §6 = ColorCode.DarkYellow = Dunkelgelb;
    /// §7 = ColorCode.Gray = Grau;
    /// §8 = ColorCode.DarkGray = Dunkelgrau;
    /// §9 = ColorCode.Blue = Blau;
    /// §a = ColorCode.Green = Grün;
    /// §b = ColorCode.Cyan = Cyan;
    /// §c = ColorCode.Red = Red;
    /// §d = ColorCode.Magenta = Magenta;
    /// §e = ColorCode.Yellow = Yellow;
    /// §f = ColorCode.White = Weiß;
    /// §r = ColorCode.Reset = ~Farben zurücksetzen~
    /// </remarks>
    /// <param name="input">String mit Farbcodes</param>
    public static void writeColoredLine(string input, bool newLine = true)
    {
        input = input.Replace($"{ColorCode.Black}", "§0");
        input = input.Replace($"{ColorCode.DarkBlue}", "§1");
        input = input.Replace($"{ColorCode.DarkGreen}", "§2");
        input = input.Replace($"{ColorCode.DarkCyan}", "§3");
        input = input.Replace($"{ColorCode.DarkRed}", "§4");
        input = input.Replace($"{ColorCode.DarkMagenta}", "§5");
        input = input.Replace($"{ColorCode.DarkYellow}", "§6");
        input = input.Replace($"{ColorCode.Gray}", "§7");
        input = input.Replace($"{ColorCode.DarkGray}", "§8");
        input = input.Replace($"{ColorCode.Blue}", "§9");
        input = input.Replace($"{ColorCode.Green}", "§a");
        input = input.Replace($"{ColorCode.Cyan}", "§b");
        input = input.Replace($"{ColorCode.Red}", "§c");
        input = input.Replace($"{ColorCode.Magenta}", "§d");
        input = input.Replace($"{ColorCode.Yellow}", "§e");
        input = input.Replace($"{ColorCode.White}", "§f");
        input = input.Replace($"{ColorCode.Reset}", "§r");

        bool isFirts = true;
        string[] splitInput = input.Split("§");
        if (splitInput.Length < 2)
        {
            Console.Write(input);
        }
        else
        {
            foreach (string str in splitInput)
            {
                if (str.Length < 1) continue;
                string s = str;
                char colorChar = s[0];
                if (!"0123456789abcdefr".Contains(s[0]) && !isFirts) s = $"§{s}";
                string text = s.Substring(1);
                ConsoleColor? color = null;
                switch (colorChar)
                {
                    case '0':
                        color = ConsoleColor.Black;
                        break;
                    case '1':
                        color = ConsoleColor.DarkBlue;
                        break;
                    case '2':
                        color = ConsoleColor.DarkGreen;
                        break;
                    case '3':
                        color = ConsoleColor.DarkCyan;
                        break;
                    case '4':
                        color = ConsoleColor.DarkRed;
                        break;
                    case '5':
                        color = ConsoleColor.DarkMagenta;
                        break;
                    case '6':
                        color = ConsoleColor.DarkYellow;
                        break;
                    case '7':
                        color = ConsoleColor.Gray;
                        break;
                    case '8':
                        color = ConsoleColor.DarkGray;
                        break;
                    case '9':
                        color = ConsoleColor.Blue;
                        break;
                    case 'a':
                        color = ConsoleColor.Green;
                        break;
                    case 'b':
                        color = ConsoleColor.Cyan;
                        break;
                    case 'c':
                        color = ConsoleColor.Red;
                        break;
                    case 'd':
                        color = ConsoleColor.Magenta;
                        break;
                    case 'e':
                        color = ConsoleColor.Yellow;
                        break;
                    case 'f':
                        color = ConsoleColor.White;
                        break;
                    case 'r':
                        Console.ResetColor();
                        break;
                    default:
                        Console.Write(s);
                        continue;
                }
                if (color != null) Console.ForegroundColor = (ConsoleColor)color;
                Console.Write(text);
                Console.ResetColor();

                isFirts = false;
            }
        }
        if (newLine) Console.WriteLine();
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCHLPositions
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PositionsManager positionsManager = new PositionsManager();

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Obtention des données du site de Hockey News");

                Progresser webData = new Progresser(30);
                foreach (NHLTeam team in Enum.GetValues(typeof(NHLTeam)))
                {
                    positionsManager.GetWebData(team);
                    webData.Update();
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Vérification avec la base de données des joueurs de la NCHL...");

                positionsManager.LoadNCHLDB();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Terminé! Appuyer Entrée pour voir les positions du Hockey News.");
                Console.Read();

                FileStream filestream = new FileStream("Positions.txt", FileMode.Create);
                var streamwriter = new StreamWriter(filestream);
                streamwriter.AutoFlush = true;
                Console.SetOut(streamwriter);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                var mismatchPlayers = positionsManager.Players.Where(p => p.NCHLTeam != NCHLTeam.AGL && p.NHLPos != p.NCHLPos);
                Console.WriteLine(string.Format("{0} joueurs jouent dans une position différente selon le Hockey News:", mismatchPlayers.Count()));
                string currentTeam = null;
                foreach (Player p in mismatchPlayers.OrderBy(pp => pp.NHLTeam))
                {
                    if (currentTeam != p.NHLTeam.ToString())
                    {
                        Console.WriteLine();
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(string.Format("***{0}***", p.NHLTeam));
                        currentTeam = p.NHLTeam.ToString();
                    }

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(string.Format("[{0}] {1} ", p.NCHLTeam, p.Name));

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(string.Format("NCHL: {0} ", p.NCHLPos));

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(string.Format("Hockey News: {0}", p.NHLPos));
                }

                Process.Start("notepad", Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Positions.txt"));
                //Console.WriteLine();
                //Console.ForegroundColor = ConsoleColor.White;
                //Console.WriteLine("Appuyer Entrer pour quitter");
                //Console.Read();
            }
            catch(Exception ex) { System.Diagnostics.Debug.Fail(ex.Message + "\n\n***START STACK TRACE***\n" + ex.StackTrace + "\n***END STACK TRACE***"); }
        }
    }
}

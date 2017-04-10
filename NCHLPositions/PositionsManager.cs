using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NCHLPositions
{
    class PositionsManager
    {
        internal List<Player> Players;
            
        public PositionsManager()
        {
            Players = new List<Player>();
        }

        internal void LoadNCHLDB()
        {
            using (StreamReader sr = new StreamReader("DB NCHL.csv"))
            {
                do
                {
                    string[] line = sr.ReadLine().Split(',');

                    if (line[2] == PlayerPosition.G.ToString())
                        continue;

                    List<Player> nhlPlayersFoundFromNCHLList = Players.Where(p => p.Name.ToLower() == line[0].ToLower()).ToList();
                    Player player = null;

                    if (nhlPlayersFoundFromNCHLList.Count() > 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine();
                        Console.WriteLine(string.Format(@"Il y a plus d'un {0} dans le Hockey News!
Le {0} qui joue dans la NCHL appartient aux {1} a la position {2}. Il s'agit de quel joueur dans la NHL?", line[0], line[1], line[2]));
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Dictionary<string, Player> samePlayer = new Dictionary<string, Player>();
                        int choice = 1;
                        foreach (Player p in nhlPlayersFoundFromNCHLList)
                        {
                            samePlayer.Add(choice.ToString(), p);
                            Console.WriteLine(string.Format("{0}- Équipe NHL: {1} Position Hockey News: {2}", choice, p.NHLTeam, p.NHLPos));
                            choice++;
                        }
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine();
                        Console.Write("Entrer le chiffre et appuyer Entrée:");
                        string entered = Console.ReadLine();
                        player = samePlayer[entered];
                    }
                    else if (nhlPlayersFoundFromNCHLList.Count() == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("Dans la base de donnée NCHL mais pas dans le Hockey News: " + line[0]);
                    }
                    else if (nhlPlayersFoundFromNCHLList.Count() == 1)
                    {
                        player = nhlPlayersFoundFromNCHLList[0];
                    }

                    if (player != null)
                    {
                        player.NCHLTeam = Utilities.GetNCHLTeamFromString(line[1]);
                        player.NCHLPos = Utilities.GetPlayerPositionFromString(line[2]);
                    }
                    
                } while (sr.Peek() >= 0);
            }
        }

        internal void Save()
        {
        }

        internal void GetWebData(NHLTeam nhlTeam)
        {
            string htmlCode;
            using (WebClient client = new WebClient())
            {
                string teamUrl = string.Format("http://forecaster.thehockeynews.com/depthchart/{0}", nhlTeam);
                htmlCode = client.DownloadString(teamUrl);
            }

            ParseFullHTML(htmlCode, nhlTeam);
        }

        private void ParseFullHTML(string htmlCode, NHLTeam team)
        {
            foreach (PlayerPosition pos in Enum.GetValues(typeof(PlayerPosition)))
            {
                if (pos == PlayerPosition.G)
                    continue;

                ParsePositionHTML(htmlCode, team, pos);
            }
        }

        private void ParsePositionHTML(string htmlCode, NHLTeam team, PlayerPosition pos)
        {
            int startPosition = 0;
            int endPosition = 0;
            int startPositionToDocumentEndLenght = 0;
            string positionHTMLString = string.Empty;
            
            switch (pos)
            {
                case PlayerPosition.G:
                    startPosition = htmlCode.IndexOf("<td>Goalie");
                    break;
                case PlayerPosition.D:
                    startPosition = htmlCode.IndexOf("<td>Defenceman");
                    break;
                case PlayerPosition.C:
                    startPosition = htmlCode.IndexOf("<td>Center");
                    break;
                case PlayerPosition.R:
                    startPosition = htmlCode.IndexOf("<td>Right Wing");
                    break;
                case PlayerPosition.L:
                    startPosition = htmlCode.IndexOf("<td>Left Wing");
                    break;
            }

            startPositionToDocumentEndLenght = htmlCode.Length - startPosition;
            endPosition = htmlCode.Substring(startPosition, startPositionToDocumentEndLenght).IndexOf("</tr>");
            positionHTMLString = htmlCode.Substring(startPosition, endPosition);

            string[] playersRaw = positionHTMLString.Split(new string[] { "</a" }, StringSplitOptions.None);


            foreach (string player in playersRaw)
            {
                if (player.Contains("<a"))
                {
                    for(int i = player.Length - 1; i > 0; i--)
                    {
                        if (player[i] == '>')
                        {
                            Players.Add(new Player() { Name = player.Substring(i + 1, player.Length - (i + 1)), NHLPos = pos, NHLTeam = team });
                            break;
                        }
                            
                    }
                }
            }

        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
namespace Pacman
{
	class Pacman
	{
		static Player p;
		static Player[] ghosts;
		static Player[] cokie;
		static string[] block;
		static int direction=1;
		static double ghostspeed=0.19;
		private static int interval=0,powerfultime=100,deathtime=20;
		private static bool islarge=false,isclear=false;
		public static bool ispowerful=false;
		public static uint Score=0;
		//0: left
		//1: right
		//2: up
		//3: down
		static void Main()
		{
			Console.Title="PACMAN";
			Title();
			Run();
		}
		static void Title()
		{
				Console.CursorVisible=false;
				Console.Clear();
				Console.ForegroundColor=ConsoleColor.Yellow;
				Console.Write("    +----+  +----+   +----+  +       +   +----+  +    +\n");
				Console.Write("    |    |  |    |   |    |  |+     +|   |    |  |+   |\n");
				Console.Write("    |    |  |    |   |       | +   + |   |    |  | +  |\n");
				Console.Write("    +----+  +----+   |       |  + +  |   +----+  |  + |\n");
				Console.Write("    |       |    |   |    |  |   +   |   |    |  |   +|\n");
				Console.Write("    |       |    |   +----+  |       |   |    |  |    +\n");
				Console.ForegroundColor=ConsoleColor.White;
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while(true)
				{
					if(sw.Elapsed.TotalSeconds>=5)
					{
						break;
					}
				}
				Console.Clear();
		}
		static Data[] ReadScore(ref Score[] data)
		{
			Data[] datas=new Data[0];
			try{
				var dexmlSerializer = new XmlSerializer(typeof(Data[]));
				var xmlSettings = new System.Xml.XmlReaderSettings()
				{
					CheckCharacters = false,
				};
				using (var streamReader = new StreamReader(".score", Encoding.UTF8))
					using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings))
					{
						datas = (Data[])dexmlSerializer.Deserialize(xmlReader);
					}
				for(int i=0;i<datas.Length;i++)
				{
					if(datas[i].Name=="Pacman")
					{
						data=datas[i].data;
						break;
					}
				}
			}catch{
			}
			return datas;
		}
		public static void SaveScore()
		{
			Score[] data=new Score[0];
			Data[] datas=ReadScore(ref data);
			Array.Resize(ref data,data.Length+1);
			data[data.Length-1]=new Score();
			data[data.Length-1].score=Score;
			Array.Sort(data);
			Array.Reverse(data);
			Score[] tmpdata=new Score[0];
			for(int i=0;i<10;i++)
			{
				if(i<=data.Length-1)
				{
					Array.Resize(ref tmpdata,tmpdata.Length+1);
					tmpdata[tmpdata.Length-1]=new Score();
					tmpdata[tmpdata.Length-1]=data[i];
				}
			}
			data=tmpdata;
			if(datas.Length>0)
				for(int i=0;i<datas.Length;i++)
				{
					if(datas[i].Name=="Pacman")
					{
						datas[i].data=data;
						break;
					}
					if(i>=datas.Length-1)
					{
						Array.Resize(ref datas,datas.Length+1);
						datas[datas.Length-1]=new Data();
						datas[datas.Length-1].data=data;
						datas[datas.Length-1].Name="Pacman";
						break;
					}
				}else
				{
					Array.Resize(ref datas,datas.Length+1);
					datas[datas.Length-1]=new Data();
					datas[datas.Length-1].data=data;
					datas[datas.Length-1].Name="Pacman";
				}
				try{
					using (var streamWriter = new StreamWriter(".score", false, Encoding.UTF8))
					{
						var xmlSerializer1 = new XmlSerializer(typeof(Data[]));
						xmlSerializer1.Serialize(streamWriter, datas);
					}
				}catch{}
		}
		static void Init()
		{
			isclear=false;
			islarge=false;
			ispowerful=false;
			interval=0;
			p.x=0;
			p.y=5;
			ghosts=new Player[0];
			cokie=new Player[0];
			block=new string[]
			{
				"#####################",
					"#. . . . . . . . . .#",
					"# ### ######### ### #",
					"#   #*# m # m #*#   #",
					"### ### # # # ### ###",
					"                     ",
					"### ### # # # ### ###",
					"#   #*# m # m #*#   #",
					"# ### ######### ### #",
					"#. . . . . . . . . .#",
					"#####################",
			};
			for(int i=0;i<block.Length;i++)
			{
				for(int j=0;j<block[i].Length;j++)
				{
					switch(block[i][j])
					{
						case '*':
							Array.Resize(ref cokie,cokie.Length+1);
							cokie[cokie.Length-1].y=i;
							cokie[cokie.Length-1].x=j;
							cokie[cokie.Length-1].status=1;
							break;
						case '.':
							Array.Resize(ref cokie,cokie.Length+1);
							cokie[cokie.Length-1].y=i;
							cokie[cokie.Length-1].x=j;
							break;
						case 'm':
							Array.Resize(ref ghosts,ghosts.Length+1);
							ghosts[ghosts.Length-1].y=i;
							ghosts[ghosts.Length-1].x=j;
							ghosts[ghosts.Length-1].originx=j;
							ghosts[ghosts.Length-1].originy=i;
							break;
					}
				}
				block[i].Replace("m"," ");
			}
		}
		static void Run()
		{
			Console.CursorVisible=false;
			int stage=1;
			while(p.status==0)
			{
				Init();
				Console.Clear();
				Stopwatch sw = new Stopwatch();
				Stopwatch swforg = new Stopwatch();
				sw.Start();
				swforg.Start();
				Console.SetCursorPosition(Console.WindowWidth/2-3,Console.WindowHeight/2);
				Console.Write("level {0}",stage);
				int counttimer=4;
				while(counttimer>=0)
				{
					if(sw.Elapsed.TotalSeconds>=1)
					{
						Console.Clear();
						Console.SetCursorPosition(Console.WindowWidth/2-1,Console.WindowHeight/2);
						Console.Write("{0}",counttimer);
						counttimer--;
						sw.Reset();
						sw.Start();
					}
				}
				Console.SetCursorPosition(Console.WindowWidth/2-1,Console.WindowHeight/2);
				Console.Write("Go");
				Console.SetCursorPosition(0,0);
				for(int i=0;i<block.Length;i++)
				{
					Console.WriteLine(block[i]);
				}
				while(!isclear&&p.status==0)
				{
					if(Console.KeyAvailable)
					{
						switch(Console.ReadKey(true).Key)
						{
							case ConsoleKey.UpArrow:
								direction=2;
								break;
							case ConsoleKey.DownArrow:
								direction=3;
								break;
							case ConsoleKey.LeftArrow:
								direction=0;
								break;
							case ConsoleKey.RightArrow:
								direction=1;
								break;
							case ConsoleKey.Escape:
								Console.CursorVisible=true;
								Environment.Exit(0);
								break;
							case ConsoleKey.Enter:
								sw.Stop();
								swforg.Stop();
								Console.SetCursorPosition(Console.WindowWidth/2-3,Console.WindowHeight/2);
								Console.Write("PAUSE");
								Console.ReadKey(true);
								sw.Start();
								swforg.Start();
								Console.SetCursorPosition(Console.WindowWidth/2-3,Console.WindowHeight/2);
								Console.Write("     ");
								break;
						}
					}
					if(sw.Elapsed.TotalSeconds>=0.2)
					{
						Update();
						sw.Reset();
						sw.Start();
					}
					if(swforg.Elapsed.TotalSeconds>=ghostspeed)
					{
						for(int i=0;i<ghosts.Length;i++)
						{
							Console.SetCursorPosition(ghosts[i].originx,ghosts[i].originy);
							Console.Write("0");
							Console.SetCursorPosition(ghosts[i].x,ghosts[i].y);
							Console.Write(" ");
							if(ghosts[i].status==0)
							{
								MoveGhost(ref ghosts[i]);
								Console.SetCursorPosition(ghosts[i].x,ghosts[i].y);
								if(ispowerful)
									Console.ForegroundColor=ConsoleColor.Blue;
								Console.Write("m");
								Console.ForegroundColor=ConsoleColor.White;
							}else
							{
								ghosts[i].variant[0]--;
								if(ghosts[i].variant[0]<0)
								{
									ghosts[i].status=0;
								}else
								{
									Console.SetCursorPosition(ghosts[i].originx,ghosts[i].originy);
									Console.Write("M");
								}
							}
						}
						Check();
						swforg.Reset();
						swforg.Start();
					}
				}
				if(isclear)
				{
					Console.Clear();
					Console.SetCursorPosition(Console.WindowWidth/2-3,Console.WindowHeight/2);
					Console.Write("CLEAR!!");
					Console.ReadKey();
					Score+=50;
					if(powerfultime>25)
					{
						powerfultime-=10;
					}
					if(deathtime>15)
					{
						deathtime--;
					}
					stage++;
				}
			}
			Console.Clear();
			Console.SetCursorPosition(Console.WindowWidth/2-3,Console.WindowHeight/2);
			Console.Write("GAME OVER");
			Console.ReadKey();
			SaveScore();
			Score[] score=null;
			ReadScore(ref score);
			Console.SetCursorPosition(1,0);
			Console.Write("Your Score: {0}",Score);
			for(int i=0;i<score.Length;i++)
			{
				if(i>Console.WindowHeight-3)
					break;
				Console.SetCursorPosition(1,i+3);
				if(score[i].score==Score)
					Console.ForegroundColor=ConsoleColor.Yellow;
				Console.Write("{0}: {1}",i+1,score[i].score);
				Console.ForegroundColor=ConsoleColor.White;
			}
			Console.ReadKey();
			Console.CursorVisible=true;
		}
		static void Update()
		{
			Console.SetCursorPosition(p.x,p.y);
			Console.Write(" ");
			Move();
			Console.SetCursorPosition(p.x,p.y);
			Console.ForegroundColor=ConsoleColor.Yellow;
			if(islarge)
			{
				Console.Write("C");
			}else
			{
				Console.Write("c");
			}
			Console.ForegroundColor=ConsoleColor.White;
			Check();
			if(interval>0)
			{
				interval--;
			}else if(interval==0)
			{
				ispowerful=false;
				ghostspeed=0.19;
			}
			islarge=!islarge;
		}
		static void Move()
		{
			switch(direction)
			{
				case 0://left
					if(p.x>0)
					{
						if(block[p.y][p.x-1]!='#' && block[p.y][p.x-1]!='-')
						{
							p.x--;
						}
					}else
					{
						if(block[p.y][block[p.y].Length-1]!='#'&&block[p.y][block[p.y].Length-1]!='-')
						{
							p.x=block[p.y].Length-1;
						}
					}
					break;
				case 1://right
					if(p.x<block[p.y].Length-1)
					{
						if(block[p.y][p.x+1]!='#'&&block[p.y][p.x+1]!='-')
						{
							p.x++;
						}
					}else
					{
						if(block[p.y][0]!='#'&&block[p.y][0]!='-')
						{
							p.x=0;
						}
					}
					break;
				case 2://up
					if(p.y>0)
					{
						if(block[p.y-1][p.x]!='#'&&block[p.y-1][p.x]!='-')
						{
							p.y--;
						}
					}else
					{
						if(block[block.Length-1][p.x]!='#'&&block[block.Length-1][p.x]!='-')
						{
							p.y=block.Length-1;
						}
					}
					break;
				case 3://down
					if(p.y<block.Length-1)
					{
						if(block[p.y+1][p.x]!='#'&&block[p.y+1][p.x]!='-')
						{
							p.y++;
						}
					}else
					{
						if(block[0][p.x]!='#'&&block[0][p.x]!='-')
						{
							p.y=0;
						}
					}
					break;
			}
		}
		static void Check()
		{
			for(int i=0;i<ghosts.Length;i++)
			{
				if(ghosts[i].x==p.x&&ghosts[i].y==p.y&&ghosts[i].status==0)
				{
					if(ispowerful)
					{
						ghosts[i].status=1;
						ghosts[i].x=ghosts[i].originx;
						ghosts[i].y=ghosts[i].originy;
						ghosts[i].variant=new int[1];
						ghosts[i].variant[0]=deathtime;
						Score+=10;
					}else
					{
						p.status=1;
					}
				}else
				{
				}
			}
			int eatencokie=cokie.Length;
			for(int i=0;i<cokie.Length;i++)
			{
				if(cokie[i].x==p.x&&cokie[i].y==p.y&&cokie[i].status!=2)
				{
					char[] tmp=block[p.y].ToCharArray();
					if(tmp[p.x]=='*'||tmp[p.x]=='.')
						tmp[p.x]=' ';
					block[p.y]=new string(tmp);
					if(cokie[i].status==1)
					{
						ispowerful=true;
						ghostspeed=0.4;
						interval+=powerfultime;
					}
					Score++;
					cokie[i].status=2;
				}
				if(cokie[i].status==2)
				{
					eatencokie--;
				}
			}
			if(eatencokie==0)
			{
				isclear=true;
			}
		}
		private static bool moveGohst(ref Player g,int h)
		{
			int samec=0;
			if(g.variant==null)
			{
				g.variant=new int[1];
			}
			switch(h)
			{
				case 0:
					if(g.x>0&&block[g.y][g.x-1]!='#'&&block[g.y][g.x-1]!='-'&&block[g.y][g.x-1]!='.'&&block[g.y][g.x-1]!='*'&&h!=g.variant[0])
					{
						g.x--;
						for(int i=0;i<ghosts.Length;i++)
						{
							if(ghosts[i].x==g.x&&ghosts[i].y==g.x)
							{
								samec++;
							}
						}
						if(samec>1)
						{
							g.x++;
							return false;
						}
						else
						{
							g.variant[0]=1;
							return true;
						}
					}else
					{
						return false;
					}
				case 1:
					if(g.x<block[g.y].Length-1&&block[g.y][g.x+1]!='#'&&block[g.y][g.x+1]!='-'&&block[g.y][g.x+1]!='.'&&block[g.y][g.x+1]!='*'&&h!=g.variant[0])
					{
						g.x++;
						for(int i=0;i<ghosts.Length;i++)
						{
							if(ghosts[i].x==g.x&&ghosts[i].y==g.x)
							{
								samec++;
							}
						}
						if(samec>1)
						{
							g.x--;
							return false;
						}
						else
						{
							g.variant[0]=0;
							return true;
						}
					}else
					{
						return false;
					}
				case 2:
					if(g.y>0&&block[g.y-1][g.x]==' '&&h!=g.variant[0])
					{
						g.y--;
						for(int i=0;i<ghosts.Length;i++)
						{
							if(ghosts[i].x==g.x&&ghosts[i].y==g.x)
							{
								samec++;
							}
						}
						if(samec>1)
						{
							g.y++;
							return false;
						}
						else
						{
							g.variant[0]=3;
							return true;
						}
					}else
					{
						return false;
					}
				case 3:
					if(g.y<block.Length-1&&block[g.y+1][g.x]==' '&&h!=g.variant[0])
					{
						g.y++;
						for(int i=0;i<ghosts.Length;i++)
						{
							if(ghosts[i].x==g.x&&ghosts[i].y==g.x)
							{
								samec++;
							}
						}
						if(samec>1)
						{
							g.y++;
							return false;
						}
						else
						{
							g.variant[0]=2;
							return true;
						}
					}else
					{
						return false;
					}
				default:
					return false;
			}
		}
		static void Ghostthink(ref Player g)
		{
			if(!ispowerful)
			{
				switch(g.type)
				{
					case 0:
						ghostthink1(ref g);
						break;
					case 1:
						break;
					case 2:
						break;
					case 3:
						break;
				}
			}else
			{
				if(g.x>p.x)
				{
					if(g.y>p.y)
					{
						if(!moveGohst(ref g,1))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,0);
					}else if(g.y<p.y)
					{
						if(!moveGohst(ref g,1))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,0);
					}else
					{
						g.variant[0]=4;
						if(!moveGohst(ref g,1))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,0);
					}
				}else if(g.x<p.x)
				{
					if(g.y>p.y)
					{
						if(!moveGohst(ref g,0))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,1);
					}else if(g.y<p.y)
					{
						if(!moveGohst(ref g,0))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,1);
					}else
					{
						g.variant[0]=4;
						if(!moveGohst(ref g,0))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,1);
					}
				}else
				{
					g.variant[0]=4;
					if(g.y>p.y)
					{
						if(!moveGohst(ref g,3))if(!moveGohst(ref g,1))if(!moveGohst(ref g,0))moveGohst(ref g,2);
					}else if(g.y<p.y)
					{
						if(!moveGohst(ref g,2))if(!moveGohst(ref g,1))if(!moveGohst(ref g,0))moveGohst(ref g,3);
					}
				}
			}
		}
		private static void ghostthink1(ref Player g)
		{
			if(g.x>p.x)
			{
				if(g.y>p.y)
				{
					if(!moveGohst(ref g,0))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,1);
				}else if(g.y<p.y)
				{
					if(!moveGohst(ref g,0))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,1);
				}else
				{
					g.variant[0]=4;
					if(!moveGohst(ref g,0))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,1);
				}
			}else if(g.x<p.x)
			{
				if(g.y>p.y)
				{
					if(!moveGohst(ref g,1))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,0);
				}else if(g.y<p.y)
				{
					if(!moveGohst(ref g,1))if(!moveGohst(ref g,3))if(!moveGohst(ref g,2))moveGohst(ref g,0);
				}else
				{
					g.variant[0]=4;
					if(!moveGohst(ref g,1))if(!moveGohst(ref g,2))if(!moveGohst(ref g,3))moveGohst(ref g,0);
				}
			}else
			{
				g.variant[0]=4;
				if(g.y>p.y)
				{
					if(!moveGohst(ref g,2))if(!moveGohst(ref g,0))if(!moveGohst(ref g,1))moveGohst(ref g,3);
				}else if(g.y<p.y)
				{
					if(!moveGohst(ref g,3))if(!moveGohst(ref g,0))if(!moveGohst(ref g,1))moveGohst(ref g,2);
				}
			}
		}
		private static void ghostthink2(ref Player g)
		{
			int seed=Environment.TickCount;
			while(moveGohst(ref g,new Random(seed++).Next(4))){}
		}
		static void MoveGhost(ref Player g)
		{
			Ghostthink(ref g);
		}
	}
	struct Player
	{
		public int x,y,status,type,originx,originy;
		public int[] variant;
	}
	public class Score : System.IComparable
	{
		public uint score;
		public string name;
		public int CompareTo(object obj)
		{
			return this.score.CompareTo(((Score)obj).score);
		}
	}
	public class Data
	{
		public Score[] data;
		public string Name="";
	}
}

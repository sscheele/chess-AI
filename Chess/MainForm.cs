/*
 * User: samsc
 * Date: 8/13/2015
 * Time: 1:55 PM
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
        AI[] gameAIs = new AI[2];
        static int defaultSearchDepth = 5;

        static string imagePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString(), "Images");
		PictureBox[] allTiles;
        bool[] baseTiles = new bool[64];
		int[] overlays = new int[64];
		BitboardLayer validMoveOverlays = new BitboardLayer(); //layer containing info as to whether a square should be marked as valid
        int tileXSize, tileYSize;
		ChessBoardDisplay c;
		bool isWhiteMove;
		int selectedPiece = -1;
		int gameMode;

        Image[] backgrounds = {Image.FromFile(Path.Combine(imagePath, "dark.png")), Image.FromFile(Path.Combine(imagePath, "light.png"))};
        Image[] pieceOverlays = new Image[13];
        

        public MainForm(int gameMode)
		{
			this.gameMode = gameMode;
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
			InitializeComponent();
			allTiles = new PictureBox[]{pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24, pictureBox25, pictureBox26, pictureBox27, pictureBox28, pictureBox29, pictureBox30, pictureBox31, pictureBox32, pictureBox33, pictureBox34, pictureBox35, pictureBox36, pictureBox37, pictureBox38, pictureBox39, pictureBox40, pictureBox41, pictureBox42, pictureBox43, pictureBox44, pictureBox45, pictureBox46, pictureBox47, pictureBox48, pictureBox49, pictureBox50, pictureBox51, pictureBox52, pictureBox53, pictureBox54, pictureBox55, pictureBox56, pictureBox57, pictureBox58, pictureBox59, pictureBox60, pictureBox61, pictureBox62, pictureBox63, pictureBox64};
			for(int i = 0; i < allTiles.Length; i++){
				this.allTiles[i].Click += new System.EventHandler(this.PictureBoxClick);
			}
            //set tiles to be light or dark
			for (int i = 0; i < baseTiles.Length; i++){
                baseTiles[i] = (i / 8) % 2 == 0 ^ i % 2 == 1;
			}
            //populate piece arrays
            int divide = pieceOverlays.Length / 2;
            for (int i = 0; i < divide; i++)
            {
                pieceOverlays[i] = Image.FromFile(Path.Combine(imagePath, i.ToString() + ".png"));
                pieceOverlays[i + divide] = Image.FromFile(Path.Combine(imagePath, "B" + i + ".png"));
            }
            pieceOverlays[pieceOverlays.Length - 1] = Image.FromFile(Path.Combine(imagePath, "validMove.png"));
            c = new ChessBoardDisplay(this);

            if ((gameMode & 1) > 0) gameAIs[1] = new AI(c.getBoard(), true, defaultSearchDepth, this);
            if ((gameMode & 2) > 0) gameAIs[0] = new AI(c.getBoard(), false, defaultSearchDepth, this);
            isWhiteMove = true;
            paintTiles();
        }
		
		void MainFormResizeEnd(object sender, EventArgs e)
		{
            resizeTiles();
			paintTiles();
		}
		
		public static Image resizeImage(Image imgToResize, int xSize, int ySize){
       		return new Bitmap(imgToResize, xSize, ySize);
    	}
		
		void paintTiles(){
			resizeTiles();
			c.genOverlay();
            for (int i = 0; i < pieceOverlays.Length; i++)
            {
                pieceOverlays[i] = resizeImage(pieceOverlays[i], tileXSize, tileYSize);
            }
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i] = resizeImage(backgrounds[i], tileXSize, tileYSize);
            }
            for (int i = 0; i < baseTiles.Length; i++) {
                Image imgOverlay = null;
                bool isValid = false;
                if (validMoveOverlays.trueAtIndex(i))
                {
                    imgOverlay = pieceOverlays[pieceOverlays.Length - 1];
                    isValid = true;
                }
                int backgroundIndex = baseTiles[i] ? 1 : 0;
                Image imgBackground = backgrounds[backgroundIndex];
                if (overlays[i] < pieceOverlays.Length || isValid) {
                    if (overlays[i] < pieceOverlays.Length ^ isValid)
                    {
                        if (overlays[i] < pieceOverlays.Length) imgOverlay = pieceOverlays[overlays[i]];
                        Image img = new Bitmap(imgBackground.Width, imgBackground.Height);
                        using (Graphics gr = Graphics.FromImage(img))
                        {
                            gr.DrawImage(imgBackground, new Point(0, 0));
                            gr.DrawImage(imgOverlay, new Point(0, 0));
                        }
                        allTiles[i].Image = img;
                    } else
                    {
                        Image imgPieceOverlay = pieceOverlays[overlays[i]];
                        Image finalImg = new Bitmap(imgBackground.Width, imgBackground.Height);

                        using (Graphics gr = Graphics.FromImage(finalImg))
                        {
                            gr.DrawImage(imgBackground, new Point(0, 0));
                            gr.DrawImage(imgPieceOverlay, new Point(0, 0));
                            gr.DrawImage(imgOverlay, new Point(0, 0));
                        }
                        allTiles[i].Image = finalImg;
                    }
                } else
                {
                    allTiles[i].Image = imgBackground;
                }
            }
		}

        public void addText(String s)
        {
            textDisplay.AppendText(s);
        }
		
		void resizeTiles(){
            //do actual resizing and positioning of tiles
            const double xScreenRatio = .7;
            const double yScreenRatio = .8;

            tileXSize = (int)((this.Width * xScreenRatio) / 8);
            tileYSize = (int)((this.Height * yScreenRatio) / 8);
			int min = yScreenRatio * this.Height > xScreenRatio * this.Width ? (int)(xScreenRatio * this.Width) : (int)(yScreenRatio * this.Height);
			for (int i = 0; i < allTiles.Length; i++){
                allTiles[i].Height = tileYSize;
				allTiles[i].Width = tileXSize;
                
                allTiles[i].Location = new Point(tileXSize * (i % 8), tileYSize * (i/8));
            }

            //do resizing and positioning of text display
            textDisplay.Height = this.Height - (8 * tileYSize);
            textDisplay.Width = (int)(.97 * this.Width);
            textDisplay.Location = new Point(0, 8 * tileYSize);

            //do resizing and positioning of buttons
            int controlSpaceHeight = this.Height - textDisplay.Height;
            const double buttonHeightRatio = .2;

            undoButton.Height = (int)(controlSpaceHeight * buttonHeightRatio); // 1/5
            undoButton.Width = this.Width - (8 * tileXSize);
            undoButton.Location = new Point(8 * tileXSize, 0);


            //do resizing and positioning of progress bar
            AIProgBar.Height = (int)(controlSpaceHeight / 16);
            AIProgBar.Width = this.Width - (8 * tileXSize);
            AIProgBar.Location = new Point(8 * tileXSize, (8 * tileYSize) - AIProgBar.Height);
        }

        public void stepProgBarBy(int x)
        {
            AIProgBar.Increment(x);
        }
		
        public void resetProgBar()
        {
            AIProgBar.Value = 0;
        }

		public void setOverlay(int index, int pieceNum){
			overlays[index] = pieceNum;
		}
		
		void PictureBoxClick(object sender, EventArgs e)
		{
            ChessBoard cb = c.getBoard();

            int isPlayerIndex = isWhiteMove ? 1 : 2;
			Point mousePosition = pictureBox1.PointToClient(Cursor.Position);
			int mouseX = mousePosition.X;
			int mouseY = mousePosition.Y;
			int indexClicked = (mouseX / tileXSize) + (8 * (mouseY / tileYSize));
			BitboardLayer[] dict = cb.getDict(isWhiteMove);
            if ((gameMode & isPlayerIndex) == 0) //is not an AI move
            {
                if (dict[pieceIndex.ALL_LOCATIONS].trueAtIndex(indexClicked))
                {
                    BitboardLayer vMoves = cb.getValidMoves(isWhiteMove, indexClicked);
                    validMoveOverlays = vMoves;
                    selectedPiece = indexClicked;
                }
                else if (validMoveOverlays.trueAtIndex(indexClicked) && selectedPiece != -1)
                {
                    cb.movePiece(isWhiteMove, selectedPiece, indexClicked);
                    cb.getAllLocations(isWhiteMove);
                    cb.getAllLocations(!isWhiteMove);
                    if (cb.checkForMate(isWhiteMove))
                    {
                        validMoveOverlays = new BitboardLayer();
                        overlays = new int[64];
                        c.genOverlay();
                        paintTiles();
                        String colorStr = isWhiteMove ? "White" : "Black";
                        MessageBox.Show("Checkmate! " + colorStr + " wins!");
                    }
                    selectedPiece = -1;
                    validMoveOverlays = new BitboardLayer();
                    isWhiteMove = !isWhiteMove;
                    overlays = new int[64];
                    c.genOverlay();
                    

                    paintTiles();
                    GC.Collect();

                    isPlayerIndex = isWhiteMove ? 1 : 2;
                    if ((gameMode & isPlayerIndex) > 0) //is an AI move
                    {
                        int[] aiMove;
                        if (isWhiteMove) aiMove = gameAIs[1].getAIMove(cb, isWhiteMove, defaultSearchDepth)[0];
                        else aiMove = gameAIs[0].getAIMove(cb, isWhiteMove, defaultSearchDepth)[0];
                        cb.movePiece(isWhiteMove, aiMove[0], aiMove[1]);
                        isWhiteMove = !isWhiteMove;
                    }
                }
                paintTiles();
                GC.Collect();
            }
		}
		
		
		public static ulong setAtIndex(ulong state, int index, bool isTrue){
			if (isTrue) return state | (ulong)(1uL << (63 - index));
			return state & ~((ulong)(1uL << 63 - index));
		}
		
		public static bool trueAtIndex(ulong t, int i){ //easier to think of the other way
			return (t & (ulong)(1uL << (63 - i))) > 0;
			//invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
		}

        private void undoButton_Click(object sender, EventArgs e)
        {
            c.getBoard().undoMove(!isWhiteMove);
            paintTiles();
            isWhiteMove = !isWhiteMove;
        }
    }
}

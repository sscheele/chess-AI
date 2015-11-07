/*
 * User: samsc
 * Date: 8/13/2015
 * Time: 1:55 PM
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace Chess
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		PictureBox[] allTiles;
		Image[] baseSquares = new Image[64];
		Image[] overlays = new Image[64];
		ulong validMoveOverlays = 0; //layer containing info as to whether a square should be marked as valid
		int tileSize;
		ChessBoard c;
		bool isWhiteMove;
		int selectedPiece = -1;
		int gameMode;
		
		public MainForm(int gameMode)
		{
			this.gameMode = gameMode;
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			allTiles = new PictureBox[]{pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24, pictureBox25, pictureBox26, pictureBox27, pictureBox28, pictureBox29, pictureBox30, pictureBox31, pictureBox32, pictureBox33, pictureBox34, pictureBox35, pictureBox36, pictureBox37, pictureBox38, pictureBox39, pictureBox40, pictureBox41, pictureBox42, pictureBox43, pictureBox44, pictureBox45, pictureBox46, pictureBox47, pictureBox48, pictureBox49, pictureBox50, pictureBox51, pictureBox52, pictureBox53, pictureBox54, pictureBox55, pictureBox56, pictureBox57, pictureBox58, pictureBox59, pictureBox60, pictureBox61, pictureBox62, pictureBox63, pictureBox64};
			for(int i = 0; i < allTiles.Length; i++){
				this.allTiles[i].Click += new System.EventHandler(this.PictureBoxClick);
			}
			for (int i = 0; i < baseSquares.Length; i++){
				Image tile;
				if ((i / 8) % 2 == 0) {
					tile = i % 2 == 0 ? Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\light.png") : Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\dark.png");
				} else {
					tile = i % 2 == 0 ? Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\dark.png") : Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\light.png");
				}
				baseSquares[i] = tile;
			}
			c = new ChessBoard(this, gameMode);
			isWhiteMove = true;
			paintTiles();
		}
		
		void MainFormResizeEnd(object sender, EventArgs e)
		{
			resizeTiles();
			paintTiles();
		}
		
		public static Image resizeImage(Image imgToResize, int size){
       		return (Image)(new Bitmap(imgToResize, size, size));
    	}
		
		void paintTiles(){
			resizeTiles();
			c.genOverlay();
			for (int i = 0; i < baseSquares.Length; i++){
				Image empty = trueAtIndex(validMoveOverlays, i) ? Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\validMove.png") : Image.FromFile(@"D:\Programs and Programming\Programming\C#\Images\empty.png");
				Image imgBackground = resizeImage(baseSquares[i], tileSize);
				Image imgOverlay = overlays[i] != null ? resizeImage(overlays[i], tileSize) : resizeImage(empty, tileSize);
				Image img = new Bitmap(imgBackground.Width, imgBackground.Height);
				using (Graphics gr = Graphics.FromImage(img)){
    				gr.DrawImage(imgBackground, new Point(0, 0));
    				gr.DrawImage(imgOverlay, new Point(0, 0));
				}
				allTiles[i].Image = img;
			}
		}
		
		void resizeTiles(){
			int min = this.Height > this.Width ? this.Width : this.Height;
			tileSize = (int)(min / 8.2);
			for (int i = 0; i < allTiles.Length; i++){
				allTiles[i].Height = tileSize;
				allTiles[i].Width = tileSize;
				allTiles[i].Location = new Point(((this.Width - (8 * tileSize)) / 2) + tileSize * (int)(i % 8), tileSize * (int)(i / 8));
			}
		}
		
		public void setOverlay(int index, Image img){
			overlays[index] = img;
		}
		
		void PictureBoxClick(object sender, EventArgs e)
		{
			Point mousePosition = pictureBox1.PointToClient(Cursor.Position);
			int mouseX = mousePosition.X;
			int mouseY = mousePosition.Y;
			int indexClicked = (mouseX / tileSize) + (8 * (mouseY / tileSize));
			ulong[] dict = c.getDict(isWhiteMove);
			if (trueAtIndex(dict[pieceIndex.ALL_LOCATIONS], indexClicked)){
				ulong vMoves = c.getValidMoves(isWhiteMove, indexClicked);
				validMoveOverlays = vMoves;
				selectedPiece = indexClicked;
			} else if (trueAtIndex(validMoveOverlays, indexClicked) && selectedPiece != -1){
				c.movePiece(isWhiteMove, selectedPiece, indexClicked);
				c.getAllLocations(isWhiteMove);
				if (c.checkForMate(isWhiteMove)){
					validMoveOverlays = 0;
					overlays = new Image[64];
					c.genOverlay();
					paintTiles();
					String colorStr = isWhiteMove ? "White" : "Black";
					MessageBox.Show("Checkmate! " + colorStr + " wins!");
				}
				selectedPiece = -1;
				validMoveOverlays = 0;
				isWhiteMove = !isWhiteMove;
				overlays = new Image[64];
				c.genOverlay();
			}
			paintTiles();
		}
		
		
		
		public static ulong setAtIndex(ulong state, int index, bool isTrue){
			if (isTrue) return state | (ulong)(1uL << (63 - index));
			return state & ~((ulong)(1uL << 63 - index));
		}
		
		public static bool trueAtIndex(ulong t, int i){ //easier to think of the other way
			return (t & (ulong)(1uL << (63 - i))) > 0;
			//invert normal digit order (ie, index of 0 gives LBS of 63, which is the leftmost bit
		}
	}
}

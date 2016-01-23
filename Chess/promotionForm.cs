using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class promotionForm : Form
    {
        public int result = -1;
        static string imagePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString(), "Images");
        public promotionForm(bool isWhite)
        {
            InitializeComponent();
            string prefix = isWhite ? "" : "B";
            string a = Path.Combine(imagePath, prefix + pieceIndex.KNIGHT.ToString());
            knightButton.Image = resizeImage(Image.FromFile(Path.Combine(imagePath, prefix + pieceIndex.KNIGHT.ToString() + ".png")), knightButton.Height);
            bishopButton.Image = resizeImage(Image.FromFile(Path.Combine(imagePath, prefix + pieceIndex.BISHOP.ToString() + ".png")), bishopButton.Height);
            rookButton.Image = resizeImage(Image.FromFile(Path.Combine(imagePath, prefix + pieceIndex.ROOK.ToString() + ".png")), rookButton.Height);
            queenButton.Image = resizeImage(Image.FromFile(Path.Combine(imagePath, prefix + pieceIndex.QUEEN.ToString() + ".png")), queenButton.Height);
        }

        private void knightButton_Click(object sender, EventArgs e)
        {
            result = pieceIndex.KNIGHT;
            this.Close();
        }

        private void bishopButton_Click(object sender, EventArgs e)
        {
            result = pieceIndex.BISHOP;
            this.Close();
        }

        private void rookButton_Click(object sender, EventArgs e)
        {
            result = pieceIndex.ROOK;
            this.Close();
        }

        private void queenButton_Click(object sender, EventArgs e)
        {
            result = pieceIndex.QUEEN;
            this.Close();
        }

        public static Image resizeImage(Image imgToResize, int size)
        {
            return new Bitmap(imgToResize, size, size);
        }
    }
}

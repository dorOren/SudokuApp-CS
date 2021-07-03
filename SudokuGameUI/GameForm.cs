﻿using System;
using System.Windows.Forms;
using SudokuLogic;

namespace SudokuUI
{
    public partial class GameForm : Form
    {
        public GameLogic Game { get; }
        public Board GameBoard { get; }
        public int BoardSideSize { get; }

        private bool m_BackPressed;
        private int m_DeletedValue;

        private TextBox[,] m_TextBoxMatrix;

        public GameForm()
        {
            Game = new GameLogic();
            GameBoard = Game.GameBoard;
            BoardSideSize = 9;
            int height = BoardSideSize * 42 + 30;
            int width = BoardSideSize * 42 + 100;
            InitializeComponent(BoardSideSize, BoardSideSize, height, width);
        }

        private void textBox_Click(object sender, EventArgs args)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectionStart = textBox.Text.Length;
        }
      

        private void board_Load(object sender, EventArgs e)
        {
            GameBoard.SetBoardToStartPosition();
            for(int i=0;i< BoardSideSize; i++)
            {
                for(int j=0;j< BoardSideSize; j++)
                {
                    if (GameBoard.GameBoard[i, j] != 0)
                    {
                        m_TextBoxMatrix[i, j].Text = $"{GameBoard.GameBoard[i, j]}";
                        m_TextBoxMatrix[i, j].ReadOnly = true;
                    }
                }
            }
        }

        private void textBox_TextChanged(object sender, EventArgs args)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length <= 1) //if only 1 digit entered
            {
                int colNum;
                int rowNum;
                (rowNum, colNum) = tagToIndexesHashFunction((int)(textBox.Tag));
                bool isNumber = int.TryParse(textBox.Text, out int res);
                if (isNumber && res!=0)
                {
                    GameBoard.MarkCell(rowNum, colNum, res);
                    Game.AddCollisions(rowNum, colNum, res);
                    textBox.Text = textBox.Text.Insert(0, " ");
                }
                else
                {
                    if (m_BackPressed)
                    {
                        Game.DeleteCollisions(rowNum, colNum, m_DeletedValue);
                        GameBoard.CollisionBoard[rowNum, colNum] = 0;
                        m_BackPressed = false;
                    }
                    textBox.Clear();
                }
                foreach (TextBox box in m_TextBoxMatrix)
                {
                    (rowNum, colNum) = tagToIndexesHashFunction((int)(box.Tag));
                    if (GameBoard.CollisionBoard[rowNum, colNum] > 0)
                    {
                        box.BackColor = System.Drawing.Color.IndianRed;
                    }
                    else
                    {
                        box.BackColor = System.Drawing.Color.White;

                    }
                }
            }

        }

        private void textBox_KeyPressed(object sender, EventArgs args)
        {
            TextBox textBox = (TextBox)sender;
            KeyEventArgs keyPressed = (KeyEventArgs)args;
            if (keyPressed.KeyData == Keys.Back)
                textBox_DeleteText(textBox, keyPressed);
        }

        private void textBox_DeleteText(TextBox i_TextBox, KeyEventArgs i_KeyPressed)
        {//TODO: FINISH THIS
            m_BackPressed = true;
            if (i_KeyPressed.KeyData == Keys.Back)
            {
                m_DeletedValue = int.Parse(i_TextBox.Text.Remove(0,1));
            }
        }

        private (int rowNum, int colNum) tagToIndexesHashFunction(int i_Tag)
        {
            int colNum = i_Tag % GameBoard.BoardSideSize;
            int rowNum = i_Tag / GameBoard.BoardSideSize;
            return (rowNum, colNum);
        }

        private void handleCollisions()
        {

        }
    }
}

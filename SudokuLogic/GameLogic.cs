﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLogic
{
    public class GameLogic
    {
        public Board GameBoard { get; }

        public GameLogic(int i_BoardSideSize)
        {
            GameBoard = new Board(i_BoardSideSize);
        }

        public void MarkCell(int i_RowNum, int i_ColNum, int i_Value)
        {
            GameBoard.MarkCell(i_RowNum, i_ColNum, i_Value);
            AddCollisions(i_RowNum, i_ColNum, i_Value);
        }

        public void ClearCell(int i_RowNum, int i_ColNum, int i_DeletedValue)
        {
            GameBoard.ClearCell(i_RowNum, i_ColNum);
            DeleteCollisions(i_RowNum, i_ColNum, i_DeletedValue);
        }

        public bool HasWon()
        {
            if (GameBoard.EmptyCells == 0 && GameBoard.CollisionCases == 0)
                return true;
            else return false;
        }

        // Collision System Handling

        public bool IsThereCollisions(int i_RowNum, int i_ColNum, int i_Value)
        {
            // Check collision of given cell in board with its neighbors from the same row/column/block
            // Function returs true if any collision was found, else returns false
            int blockSideSize = GameBoard.BlockSideSize;
            return  RowCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.None) ||
                    ColCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.None) ||
                    BlockCollisions(i_RowNum - i_RowNum % blockSideSize, i_ColNum - i_ColNum % blockSideSize, i_RowNum, i_ColNum, i_Value, eCollisionAction.None);
        }

        public bool AddCollisions(int i_RowNum, int i_ColNum, int i_Value)
        {
            // Count collision of given cell in board with its neighbors from the same row/column/block
            // Number of collisions is stored in all cells affected from the new cell in GameBoard.CollisionBoard
            // Function returs true if any collision was found, else returns false
            int blockSideSize = GameBoard.BlockSideSize;
            bool a = RowCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.Add);
            bool b = ColCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.Add);
            bool c = BlockCollisions(i_RowNum - i_RowNum % blockSideSize, i_ColNum - i_ColNum % blockSideSize, i_RowNum, i_ColNum, i_Value, eCollisionAction.Add);
            return a || b || c;
        }

        public bool DeleteCollisions(int i_RowNum, int i_ColNum, int i_Value)
        {
            // Decrease collision count of all cells that were affected from the cell to be removed
            // The count decreases from each affected cell in GameBoard.CollisionBoard
            // Function returs true if a decrement action was done, else returns false
            int blockSideSize = GameBoard.BlockSideSize;
            bool a = RowCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.Delete);
            bool b = ColCollisions(i_RowNum, i_ColNum, i_Value, eCollisionAction.Delete);
            bool c = BlockCollisions(i_RowNum - i_RowNum % blockSideSize, i_ColNum - i_ColNum % blockSideSize, i_RowNum, i_ColNum, i_Value, eCollisionAction.Delete);
            return a || b || c;
        }

        private bool RowCollisions(int i_RowNum, int i_ColNum, int i_Value, eCollisionAction i_Action)
        {
            bool collisions = false;
            int boardSideSize = GameBoard.BoardSideSize;
            for (int i = 0; i < boardSideSize; i++)
            {
                if (GameBoard.GameBoard[i_RowNum, i] == i_Value &&i!=i_ColNum)
                {
                    if (i_Action.Equals(eCollisionAction.Add))
                    {
                        GameBoard.CollisionBoard[i_RowNum, i] += 1;
                        GameBoard.CollisionBoard[i_RowNum, i_ColNum] += 1;
                        GameBoard.CollisionCases += 2;
                    }
                    else if (i_Action.Equals(eCollisionAction.Delete))
                    {
                        GameBoard.CollisionBoard[i_RowNum, i] -= 1;
                        GameBoard.CollisionCases -= 2;
                    }
                    collisions = true;
                }
            }
            return collisions;
        }

        private bool ColCollisions(int i_RowNum, int i_ColNum, int i_Value, eCollisionAction i_Action)
        {
            bool collisions = false;
            int boardSideSize = GameBoard.BoardSideSize;
            for (int i = 0; i < boardSideSize; i++)
            {
                if (GameBoard.GameBoard[i, i_ColNum] == i_Value && i!=i_RowNum)
                {
                    if (i_Action.Equals(eCollisionAction.Add))
                    {
                        GameBoard.CollisionBoard[i, i_ColNum] += 1;
                        GameBoard.CollisionBoard[i_RowNum, i_ColNum] += 1;
                        GameBoard.CollisionCases += 2;
                    }
                    else if( i_Action.Equals(eCollisionAction.Delete))
                    {
                        GameBoard.CollisionBoard[i, i_ColNum] -= 1;
                        GameBoard.CollisionCases -= 2;
                    }
                    collisions = true;
                }
            }
            return collisions;
        }

        private bool BlockCollisions(int i_RowStart, int i_ColStart, int i_RowNum, int i_ColNum, int i_Value, eCollisionAction i_Action)
        {
            bool collisions = false;
            int blockSideSize = GameBoard.BlockSideSize;
            for (int i = 0; i < blockSideSize; i++)
            {
                for (int j = 0; j < blockSideSize; j++)
                {
                    if (GameBoard.GameBoard[i_RowStart + i, i_ColStart + j] == i_Value &&
                        (i_RowStart + i) != i_RowNum && 
                        (i_ColStart + j) != i_ColNum)
                    {
                        if (i_Action.Equals(eCollisionAction.Add))
                        {
                            GameBoard.CollisionBoard[i_RowStart + i, i_ColStart + j] += 1;
                            GameBoard.CollisionBoard[i_RowNum, i_ColNum] += 1;
                            GameBoard.CollisionCases += 2;
                        }
                        else if (i_Action.Equals(eCollisionAction.Delete))
                        {
                            GameBoard.CollisionBoard[i_RowStart + i, i_ColStart + j] -= 1;
                            GameBoard.CollisionCases -= 2;
                        }
                        collisions = true;
                    }
                }
            }
                return collisions;
        }

        private enum eCollisionAction
        {
            None,
            Add,
            Delete
        }
    }
}

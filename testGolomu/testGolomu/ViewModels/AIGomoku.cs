using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace testGolomu.ViewModels
{
    class AIGomoku
    {
        public int[] _IsBlack;
        public bool isWin;
        public int PointWin;
        public int turn;
        public AIGomoku()
        {
            _IsBlack = new int[144];
            isWin = false;
            
        }
        public void InitiIsBlack()
        {
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                    _IsBlack[i * 12 + j] = ((i + j) % 2) + 1;
        }

        public void CheckWin(int _Player, int[] IsBlack)
        {

            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                {
                    if (isWin == true)
                        break;
                    if (IsBlack[i * 12 + j] > 2 && IsBlack[i * 12 + j] % 10 == _Player)
                    {
                        if (isWin == true)
                            break;
                        int PointChess = i * 12 + j;
                        int Value = IsBlack[PointChess];
                        if (j < 12 - 4)
                            if ((IsBlack[PointChess + 1] % 10 == Value % 10) && (IsBlack[PointChess + 2] % 10 == Value % 10) && (IsBlack[PointChess + 3] % 10 == Value % 10) && (IsBlack[PointChess + 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8)
                            if ((IsBlack[PointChess + 12] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8 && j < 8)
                            if ((IsBlack[PointChess + 12 + 1] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3 + 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4 + 4] % 10 == Value % 10))
                                isWin = true;
                        if (i < 8 && j > 3)
                            if ((IsBlack[PointChess + 12 - 1] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 3 - 3] % 10 == Value % 10) && (IsBlack[PointChess + 12 * 4 - 4] % 10 == Value % 10))
                                isWin = true;
                    }
                }

        }

        public void CopyChessBoard(int[] VirtualChess)
        {
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                    VirtualChess[i * 12 + j] = _IsBlack[i * 12 + j];
        }
        public void CheckUpcomingWin(int z)
        {
            if (isWin == true)
                return;
            // checUpconmingWin
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && isWin == false; i++)
                for (int j = 0; j < 12 && isWin == false; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        CheckWin(z, VirtualChess);
                        if (isWin == true)
                        {
                            // MessageBox.Show(" sap win");
                            PointWin = i * 12 + j;
                            isWin = false;
                            return;
                        }
                    }
                }
            isWin = false;
        }
        // kiem tra  3 quan khong bi chan
        public void Checkthreeopen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {

                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && (VirtualChess[PointChess + 4] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4 + 4] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue = Value;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 4 - 4] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue = Value;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            PointWin = i * 12 + j;
                            //MessageBox.Show("Co 3 quan k bi chan");
                            return;
                        }
                    }
                }
        }
        // kiem tra 2* quan lien tiep 
        public void CheckDoubleTwoOpen(int z)
        {

            int PointTrue = 0;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == 0; i++)
                for (int j = 0; j < 12 && PointTrue == 0; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12; k++)
                            for (int l = 0; l < 12; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {

                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 9 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue++;
                                    if (k < 9 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue++;
                                    if (k < 9 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue++;
                                    if (k < 9 && l > 2 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue++;

                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 4] < 3) || (VirtualChess[PointChess - 1] < 3)))
                                            PointTrue++;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4] < 3) || (VirtualChess[PointChess - 12] < 3)))
                                            PointTrue++;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 + 4] < 3) || (VirtualChess[PointChess - 12 - 1] < 3)))
                                            PointTrue++;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 - 4] < 3) || (VirtualChess[PointChess - 12 + 1] < 3)))
                                            PointTrue++;
                                }
                            }
                        if (PointTrue > 1)
                        {
                            // MessageBox.Show("Co 2 * 2 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                        else
                            PointTrue = 0;
                    }
                }
        }


        public void CheckOneChessOpen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {

                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 10 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l < 10 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l > 1 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue = z;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            // MessageBox.Show("Co 1 duong 2 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                    }
                }
        }
        // 1 duong 3 bi chan 1 dau 
        public void CheckthreeChessClose(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {

                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 12 - 4 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 4] < 3) || (VirtualChess[PointChess - 1] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4] < 3) || (VirtualChess[PointChess - 12] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && l < 8 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 + 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 + 4] < 3) || (VirtualChess[PointChess - 12 - 1] < 3)))
                                            PointTrue = Value;
                                    if (k < 8 && l > 3 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 3 - 3] % 10 == Value % 10) && ((VirtualChess[PointChess + 12 * 4 - 4] < 3) || (VirtualChess[PointChess - 12 + 1] < 3)))
                                            PointTrue = Value;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            PointWin = i * 12 + j;
                            // MessageBox.Show("Co 3 quan  bi chan");
                            return;
                        }
                    }
                }
        }
        public void CheckChessOpen(int z)
        {
            int PointTrue = -1;
            int[] VirtualChess = new int[144];
            CopyChessBoard(VirtualChess);
            for (int i = 0; i < 12 && PointTrue == -1; i++)
                for (int j = 0; j < 12 && PointTrue == -1; j++)
                {
                    CopyChessBoard(VirtualChess);
                    if (VirtualChess[i * 12 + j] < 3)
                    {
                        VirtualChess[i * 12 + j] = VirtualChess[i * 12 + j] * 10 + z;
                        for (int k = 0; k < 12 && PointTrue == -1; k++)
                            for (int l = 0; l < 12 && PointTrue == -1; l++)
                            {
                                if (VirtualChess[k * 12 + l] % 10 == z)
                                {

                                    int PointChess = k * 12 + l;
                                    int Value = VirtualChess[k * 12 + l];
                                    if (l < 10 && l > 0)
                                        if ((VirtualChess[PointChess + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 2] < 3) && (VirtualChess[PointChess - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && k > 0)
                                        if ((VirtualChess[PointChess + 12] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2] < 3) && (VirtualChess[PointChess - 12] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l < 10 && l > 0 && k > 0)
                                        if ((VirtualChess[PointChess + 12 + 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 + 2] < 3) && (VirtualChess[PointChess - 12 - 1] < 3))
                                            PointTrue = z;
                                    if (k < 10 && l > 1 && k > 0 && l < 11)
                                        if ((VirtualChess[PointChess + 12 - 1] % 10 == Value % 10) && (VirtualChess[PointChess + 12 * 2 - 2] < 3) && (VirtualChess[PointChess - 12 + 1] < 3))
                                            PointTrue = z;
                                }
                            }
                        if (PointTrue > 0)
                        {
                            //  MessageBox.Show("Co 1 duong 1 quan k bi chan");
                            PointWin = i * 12 + j;
                            return;
                        }
                    }
                }
        }


        public void Computer()
        {
            // if (isWin == true)
            //     return;
            PointWin = -1;
            if (turn == 0)
            {
                _IsBlack[6 * 12 + 6] = _IsBlack[6 * 12 + 6] * 10 + 8;
                PointWin = 6 * 12 + 6;
                return;
            }
            if (turn == 1)
            {
                for (int i = 0; i < 12; i++)
                    for (int j = 0; j < 12; j++)
                    {
                        if (_IsBlack[i * 12 + j] % 10 == 4)
                        {
                            _IsBlack[i * 12 + 12 + j + 1] = _IsBlack[i * 12 + 12 + j + 1] * 10 + 8;
                            PointWin = i * 12 + 12 + j + 1;
                            return;
                        }
                    }
            }
            if (turn == 2)
            {
                if (_IsBlack[7 * 12 + 5] < 3)
                {
                    _IsBlack[7 * 12 + 5] = _IsBlack[7 * 12 + 5] * 10 + 8;
                    PointWin = 7 * 12 + 5;
                    return;
                }
                if (_IsBlack[5 * 12 + 7] < 3)
                {
                    _IsBlack[5 * 12 + 7] = _IsBlack[5 * 12 + 7] * 10 + 8;
                    PointWin = 5 * 12 + 7;
                    return;
                }

            }
            if (turn == 3)
            {
                for (int i = 0; i < 12; i++)
                    for (int j = 0; j < 12; j++)
                    {
                        if (_IsBlack[i * 12 + j] % 10 == 8)
                        {
                            if (_IsBlack[(i + 1) * 12 + j + 1] < 3)
                            {
                                _IsBlack[(i + 1) * 12 + j + 1] = _IsBlack[(i + 1) * 12 + j + 1] * 10 + 8;
                                PointWin = ((i + 1) * 12 + j + 1);

                                return;
                            }
                            if (_IsBlack[(i - 1) * 12 + j + 1] < 3)
                            {
                                _IsBlack[(i - 1) * 12 + j + 1] = _IsBlack[(i - 1) * 12 + j + 1] * 10 + 8;
                                PointWin = (i - 1) * 12 + j + 1;
                                return;
                            }
                        }
                    }
            }
            if (turn == 4)
            {
                if (_IsBlack[6 * 12 + 5] < 3)
                {
                    _IsBlack[6 * 12 + 5] = _IsBlack[6 * 12 + 5] * 10 + 8;
                    PointWin = 6 * 12 + 5;
                   
                    return;
                }
                if (_IsBlack[5 * 12 + 6] < 3)
                {
                    _IsBlack[5 * 12 + 6] = _IsBlack[5 * 12 + 6] * 10 + 8;
                    PointWin = 5 * 12 + 6;
                    return;
                }

            }


            // checUpconmingWin

            CheckUpcomingWin(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;

                return;
            }
            CheckUpcomingWin(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;

                return;
            }



            // chex 3 k bi chan
            Checkthreeopen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            Checkthreeopen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            // 2 duong 3
            CheckDoubleTwoOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            CheckDoubleTwoOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            // 3 bi chan //2 quan k bi chan
            CheckthreeChessClose(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }

            CheckOneChessOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }


            CheckthreeChessClose(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            CheckOneChessOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }





            // 1 quan k bi chan
            CheckChessOpen(8);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            CheckChessOpen(4);
            if (PointWin > 0)
            {
                _IsBlack[PointWin] = _IsBlack[PointWin] * 10 + 8;
                return;
            }
            // 1 quan bi chan
            for (int i = 1; i < 11; i++)
                for (int j = 1; j < 11; j++)
                {
                    bool Check = false;
                    if (_IsBlack[i * 12 + j] % 10 == 8)
                    {
                        for (int k = j; k < 11; k++)
                        {
                            if (_IsBlack[i * 12 + k] > 2)
                            {
                                Check = false;
                                break;
                            }
                            Check = true;

                        }
                        if (Check == true)
                        {
                            _IsBlack[i * 12 + j + 1] = _IsBlack[i * 12 + j + 1] * 10 + 8;
                            return;
                        }



                        for (int k = i; k < 11; k++)
                        {
                            if (_IsBlack[k * 12 + j] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + j] = _IsBlack[(i + 1) * 12 + j] * 10 + 8;
                            return;
                        }




                        for (int k = i, l = j; k < 11 && l < 11; k++, l++)
                        {
                            if (_IsBlack[k * 12 + l] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + (j + 1)] = _IsBlack[(i + 1) * 12 + j + 1] * 10 + 8;
                            return;
                        }



                        for (int k = i, l = j; k < 11 && j < 11; k++, l++)
                        {
                            if (_IsBlack[k * 12 + l] > 2)
                            {
                                Check = false;
                                break;

                            }
                            Check = true;
                        }
                        if (Check == true)
                        {
                            _IsBlack[(i + 1) * 12 + j - 1] = _IsBlack[(i + 1) * 12 + j - 1] * 10 + 8;
                            return;
                        }

                    }
                }


            while (true)
            {
                int n, m;
                Random random = new Random();
                n = random.Next(0, 11);
                m = random.Next(0, 11);
                if (_IsBlack[n * 12 + m] < 3)
                {
                    _IsBlack[n * 12 + m] = _IsBlack[n * 12 + m] + 8;
                    // MessageBox.Show("random");
                    return;
                }
            }
        
        }
    }
}

using System;
using System.Collections.Generic;

static class RandomExtensions
{
    public static void Shuffle<T> (this Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

public class Sudoku
{
    int[,] mat;
    int N; // number of columns/rows.
    int SRN; // square root of N
    int K; // No. Of missing digits

    // dictionary for converting 9-0, 8-1, 7-2, etc.
    Dictionary<int, int> convertRight = new Dictionary<int, int>();
 
    public int[] diagrandom = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
    public int[] diagrandom2 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};

    // Constructor
    public Sudoku(int N, int K)
    {
        this.N = N;
        this.K = K;

        // adding cRight digits
        convertRight.Add(8, 0);
        convertRight.Add(7, 1);
        convertRight.Add(6, 2);
        convertRight.Add(5, 3);
        convertRight.Add(4, 4);
        convertRight.Add(3, 5);
        convertRight.Add(2, 6);
        convertRight.Add(1, 7);
        convertRight.Add(0, 8);
 
        // Compute square root of N
        double SRNd = Math.Sqrt(N);
        SRN = (int)SRNd;
 
        mat = new int[N,N];
    }
 
    // Sudoku Generator
    public void fillValues()
    {
        var rng = new Random();
        rng.Shuffle(diagrandom);
        rng.Shuffle(diagrandom);
        rng.Shuffle(diagrandom2);
        rng.Shuffle(diagrandom2);
        fillDiag(-1);
        // Fill for diag
        //fillRemainingDiag();
        Console.WriteLine("done diag");
        // Fill the diagonal of SRN x SRN matrices
        fillDiagonal();
        // Fill remaining blocks
        fillRemaining(0, SRN);
 
        // Remove Randomly K digits to make game
        removeKDigits();
    }

    // Fill the diagonal SRN number of SRN x SRN matrices
    void fillDiagonal()
    {
 
        for (int i = 0; i<N; i=i+SRN)
 
            // for diagonal box, start coordinates->i==j
            fillBox(i, i);
    }
 
    // Returns false if given 3 x 3 block contains num.
    bool unUsedInBox(int rowStart, int colStart, int num)
    {
        for (int i = 0; i<SRN; i++)
            for (int j = 0; j<SRN; j++)
                if (mat[rowStart+i,colStart+j]==num)
                    return false;
 
        return true;
    }
 
    // Fill a 3 x 3 matrix.
    void fillBox(int row,int col)
    {
        int num;
        for (int i=0; i<SRN; i++)
        {
            for (int j=0; j<SRN; j++)
            {
                if (i != j)
                {
                    do
                    {
                        num = randomGenerator(N);
                    }
                    while (!unUsedInBox(row, col, num));
    
                    mat[row+i,col+j] = num;
                }
            }
        }
    }
 
    // Random generator
    int randomGenerator(int num)
    {
        Random rand = new Random();
        return (int) Math.Floor((double)(rand.NextDouble()*num+1));
    }
 
    // Check if safe to put in cell
    bool CheckIfSafe(int i,int j,int num)
    {
        return (unUsedInRow(i, num) &&
            unUsedInCol(j, num) &&
            unUsedInBox(i-i%SRN, j-j%SRN, num));
    }
 
    bool checkifsafediagleft(int num, int l)
    {
        return (unUsedInDiagLeft(num) &&
        unUsedInCol(l, num) &&
        unUsedInRow(l, num));
    }

    bool checkifsafediagright(int num, int l, int l2)
    {
        return (unUsedInDiagRight(num) &&
        unUsedInCol(l2, num) &&
        unUsedInRow(l, num));
    }

    // check in the row for existence
    bool unUsedInRow(int i,int num)
    {
        for (int j = 0; j<N; j++)
            if (mat[i,j] == num)
                return false;
        return true;
    }
 
    // check in the column for existence
    bool unUsedInCol(int j,int num)
    {
        for (int i = 0; i<N; i++)
            if (mat[i,j] == num)
                return false;
        return true;
    }

    // trying to get diagonal check
    bool unUsedInDiagLeft(int num)
    {
        for (int l = 0; l < N; l++)
        {
            if (mat[l, l] == num)
            {
                return false;
            }
        }
        return true;
    }

    bool unUsedInDiagRight(int num)
    {
        for (int l = N-1; l <= 0; l--)
        {
            int lOther = convertRight[l];
            if (mat[l, lOther] == num)
            {
                return false;
            }
        }
        return true;
    }
/*
    void fillRemainingDiag()
    {
        for (int r = 1; r <= N; r++)
        {
            if (unUsedInDiag(r))
            {
                mat[r-1, r-1] = r;
                /*
                if (fillRemaining(i, j+1))
                    return true;
                mat[i,j] = 0;
                end comment here                
            }
        }
        for (int r = 9; r >= N; r--)
        {
            if (unUsedInDiag(r))
            {
                mat[r-1, r-1] = r;
                /*
                if (fillRemaining(i, j+1))
                    return true;
                mat[i,j] = 0;
                end comment here
            }
        }
    }
 */

    bool fillDiag(int l)
    {
        if (l >= N-1)
        {
            return false;
        }
        else
        {
            l++;
        }

        Console.WriteLine("here");

        foreach (int i in diagrandom)
        {
            if (checkifsafediagleft(i, l))
            {
                mat[l,l] = i;
            }
        }
        foreach(int i in diagrandom2)
        {
            int findL = convertRight[l];
            if (checkifsafediagright(i, l, findL))
            {
                int changeL = convertRight[i-1];
                mat[l, changeL] = i;
            }
        }
        if (fillDiag(l))
            {
                return true;
            }
            //mat[l,l] = 0;
        return false;
    }
    // A recursive function to fill remaining
    // matrix
    bool fillRemaining(int i, int j)
    {
        if (j>=N && i<N-1)
        {
            i = i + 1;
            j = 0;
        }
        if (i>=N && j>=N)
            return true;
 
        if (i < SRN)
        {
            if (j < SRN)
                j = SRN;
        }
        else if (i < N-SRN)
        {
            if (j==(int)(i/SRN)*SRN)
                j =  j + SRN;
        }
        else
        {
            if (j == N-SRN)
            {
                i = i + 1;
                j = 0;
                if (i>=N)
                    return true;
            }
        }
 
        for (int num = 1; num<=N; num++)
        {
            if (i != j)
            {
                if (CheckIfSafe(i, j, num))
                {
                    mat[i,j] = num;
                    if (fillRemaining(i, j+1))
                        return true;
                    mat[i,j] = 0;
                }
            }
            //else {return false}
        }
        return false;
    }
 
    // Remove the K no. of digits to
    // complete game
    public void removeKDigits()
    {
        int count = K;
        while (count != 0)
        {
            int cellId = randomGenerator(N*N)-1;
 
            // System.out.println(cellId);
            // extract coordinates i  and j
            int i = (cellId/N);
            int j = cellId%9;
            if (j != 0)
                j = j - 1;
 
            // System.out.println(i+" "+j);
            if (mat[i,j] != 0)
            {
                count--;
                mat[i,j] = 0;
            }
        }
    }
 
    // Print sudoku
    public void printSudoku()
    {
        for (int i = 0; i<N; i++)
        {
            for (int j = 0; j<N; j++)
                Console.Write(mat[i,j] + " ");
            Console.WriteLine();
        }
        Console.WriteLine();
    }
 
    // Driver code
    public static void Main(string[] args)
    {
        int N = 9, K = 0;
        Sudoku sudoku = new Sudoku(N, K);
        sudoku.fillValues();
        sudoku.printSudoku();
    }
}
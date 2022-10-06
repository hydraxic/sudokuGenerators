/* C# program for Sudoku generator */
using System;
using System.Collections.Generic;
using System.IO;

public class Sudoku
{
	int[,] mat;
	int N; // number of columns/rows.
	int SRN; // square root of N
	int K; // No. Of missing digits

	// Constructor
	public Sudoku(int N, int K)
	{
		this.N = N;
		this.K = K;

		// Compute square root of N
		double SRNd = Math.Sqrt(N);
		SRN = (int)SRNd;

		mat = new int[N,N];
	}

	// Sudoku Generator
	public void fillValues(int actualIter, string diff)
	{

		(int, int)[] asteriskValues = new (int, int)[]
		{
			(1, 4),
			(2, 2),
			(2, 6),
			(4, 1),
			(4, 4),
			(4, 7),
			(6, 2),
			(6, 6),
			(7, 4)
		}

		// example

		// 0 0 0 0 0 0 0 0 0
		// 0 0 0 0 1 0 0 0 0
		// 0 0 2 0 0 0 3 0 0
		// 0 0 0 0 0 0 0 0 0
		// 0 4 0 0 9 0 0 5 0
		// 0 0 0 0 0 0 0 0 0
		// 0 0 6 0 0 0 7 0 0
		// 0 0 0 0 8 0 0 0 0
		// 0 0 0 0 0 0 0 0 0

		// Fill the diagonal of SRN x SRN matrices
		fillDiagonal();

		// Fill remaining blocks
		fillRemaining(0, SRN);

        /*string path = string.Format(@"D:\OUTPUTSUDOKU\ClassicSudoku\{0}\Solved\{1}.txt", diff, actualIter.ToString());
        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i<N; i++)
            {
                for (int j = 0; j<N; j++)
                {
                    if (j == N-1)
                    {
                        sw.Write(mat[i, j]);
                    }
                    else
                    {
                        sw.Write(mat[i,j] + " ");
                    }
                }
                if (i != N-1)
                {
                    sw.Write("\n");
                }
            }
        sw.Flush();
        sw.Close();
        }
*/
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
				do
				{
					num = randomGenerator(N);
				}
				while (!unUsedInBox(row, col, num));

				mat[row+i,col+j] = num;
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

	// check in the row for existence
	bool unUsedInRow(int i,int num)
	{
		for (int j = 0; j<N; j++)
		if (mat[i,j] == num)
				return false;
		return true;
	}

	// check in the row for existence
	bool unUsedInCol(int j,int num)
	{
		for (int i = 0; i<N; i++)
			if (mat[i,j] == num)
				return false;
		return true;
	}

	// A recursive function to fill remaining
	// matrix
	bool fillRemaining(int i, int j)
	{
		// System.out.println(i+" "+j);
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
				j = j + SRN;
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
			if (CheckIfSafe(i, j, num))
			{
				mat[i,j] = num;
				if (fillRemaining(i, j+1))
					return true;

				mat[i,j] = 0;
			}
		}
		return false;
	}

	// Remove the K no. of digits to
	// complete game
	public void removeKDigits()
    {
        int countK = K;
        while (countK != 0)
        {
            Random r = new Random(); 
			int cr = r.Next(mat.GetLength(0));
			int rr = r.Next(mat.GetLength(1));
            if (mat[cr, rr] != 0)
			{
				mat[cr, rr] = 0;
				countK--;
			}
            
        }
    }

	// Print sudoku
	public void printSudoku(int actualIter, string diff)
	{
		string path = string.Format(@"D:\OUTPUTSUDOKU\ClassicSudoku\{0}\Unsolved\{1}.txt", diff, actualIter.ToString());
        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i<N; i++)
            {
                for (int j = 0; j<N; j++)
                {
                    if (j == N-1)
                    {
                        sw.Write(mat[i, j]);
                    }
                    else
                    {
                        sw.Write(mat[i,j] + " ");
                    }
                }
                if (i != N-1)
                {
                    sw.Write("\n");
                }
            }
        sw.Flush();
        sw.Close();
        }
	}

	// Driver code
	public static void Main(string[] args)
	{
		Random rnd = new Random();
		string dif = "Death";
        for (int i = 0; i < 500; i++)
        {
            int N = 9, K = 70;
            Sudoku sudoku = new Sudoku(N, K);
            sudoku.fillValues(i + 1, dif);
            sudoku.printSudoku(i + 1, dif);
        }
	}
}

// This code is contributed by rrrtnx.

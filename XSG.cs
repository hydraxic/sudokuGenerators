using System;
using System.Collections.Generic;
using System.IO;

static class Extensions
{
    private static Random rng = new Random();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}

public class Sudoku
{
    int[,] mat;
    int[,] sMat;
    int N; // number of columns/rows.
    int SRN; // square root of N
    int K; // No. Of missing digits

    // dictionary for converting 9-0, 8-1, 7-2, etc.
    Dictionary<int, int> convertRight = new Dictionary<int, int>();
 
    public List<int> diagrandom = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
    public List<int> diagrandom2 = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};

    /*public (int, int)[] doNotOverwriteGrids = new[] {
        (0, 0),
        (1, 1),
        (2, 2),
        (3, 3),
        (4, 4),
        (5, 5),
        (6, 6),
        (7, 7),
        (8, 8),
        (8, 0),
        (7, 1),
        (6, 2),
        (5, 3),
        (4, 4),
        (3, 5),
        (2, 6),
        (1, 7),
        (0, 8)
    };*/

    static int num = 8;
    static int [] buf = new int [num];
    static bool [] used = new bool [num];

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
        sMat = new int[N, N];
    }
 
    // Sudoku Generator
    public bool fillValues(int actualIter, string diff)
    {
        mat = new int[N,N];
        sMat = new int[N, N];
        diagrandom = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
        diagrandom2 = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};

        int toRemove = randomGenerator(N);
        diagrandom.RemoveAt(toRemove-1);
        diagrandom2.RemoveAt(toRemove-1);
        //  Console.WriteLine(toRemove);
        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom));
        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom2));
        //Console.WriteLine("[{0}]", string.Join(", ", diagrandom));
        //Console.WriteLine("[{0}]", string.Join(", ", diagrandom2));
        diagrandom.Shuffle();
        bool refresh = true;
        while (refresh == true)
        {
            diagrandom2.Shuffle();

            for (int index = 0; index <= N-2; index++)
            {
                List<int> reverseDiagrandom2 = new List<int>(diagrandom2);
                reverseDiagrandom2.Reverse();
                if (diagrandom[index] == diagrandom2[index])
                {
                    refresh = true;
                    break;
                }
                else if (diagrandom[index] == reverseDiagrandom2[index])
                {
                    refresh = true;
                    break;
                }
                else
                {
                    refresh = false;
                }
            }
            if (refresh == false)
            {
                break;
            }
        }
        
        diagrandom.Insert(4, toRemove);
        diagrandom2.Insert(4, toRemove);
        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom));
        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom2));



        int[] need = new int[] {-1, toRemove};
        //Console.WriteLine("[{0}]", string.Join(", ", derange()));
        fillDiag(need);

        //  for (int i = 0; i<N; i++)
        //  {
            //  for (int j = 0; j<N; j++)
                //  Console.Write(mat[i,j] + " ");
            //  Console.WriteLine();
        //  }
        //  Console.WriteLine();
        // Fill for diag
        //fillRemainingDiag();
        //  Console.WriteLine("done diag");
        // Fill the diagonal of SRN x SRN matrices
        fillDiagonal();
        //{
         //   fillValues();
          //  return;
           // Console.WriteLine("box recursion");
        //}

        //  for (int i = 0; i<N; i++)
        //  {
            //  for (int j = 0; j<N; j++)
                //  Console.Write(mat[i,j] + " ");
            //  Console.WriteLine();
        //  }

        //  Console.WriteLine("done box");
        // Fill remaining blocks
        
        fillRemaining(0, SRN, false);
        //{
         //   Console.WriteLine("row and col recursion");
          //  fillValues();
           // return;
        //}
        
        

        // Remove Randomly K digits to make game

        sMat = mat;

        if (CheckIfComplete() == true)
        {
            
            /*string path = string.Format(@"D:\OUTPUTSUDOKU\XSudoku\{0}\Solved\{1}.txt", diff, actualIter.ToString());
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i<N; i++)
                {
                    for (int j = 0; j<N; j++)
                    {
                        if (j == N-1)
                        {
                            sw.Write(sMat[i, j]);
                        }
                        else
                        {
                            sw.Write(sMat[i,j] + " ");
                        }
                    }
                    if (i != N-1)
                    {
                        sw.Write("\n");
                    }
                }
            sw.Flush();
            sw.Close();
            }*/
            RemoveKDigitsV2();
            return true;
        }
        else
        {return false;}


        
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
            {
                if (mat[rowStart+i,colStart+j]==num)
                    return false;
            }
        return true;
    }
 
    // Fill a 3 x 3 matrix.
    void fillBox(int row,int col)
    {
        int num;
        int counter = 0;
        for (int i=0; i<SRN; i++)
        {
            for (int j=0; j<SRN; j++)
            {
                //(double, int) gridLocation = (col+j, row+i);
                //if (!(Array.Exists(doNotOverwriteGrids, element => element == gridLocation)))
                if (mat[row+i, col+j] == 0)
                {
                    do
                    {
                        num = randomGenerator(N);
                        counter++;
                        //Console.WriteLine(counter);
                        if (counter >= 7500)
                        {
                            //  Console.WriteLine("is over 7500 box");
                            return;
                        }
                        //Console.WriteLine(counter);
                    }
                    while (!CheckIfSafe(row+i, col+j, num));
                    mat[row+i,col+j] = num;
                }
            }
        }
        //return 0;
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

    bool fillDiag(int[] listofneeded)
    {
        int l = listofneeded[0];
        int toRemove = listofneeded[1];

        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom));
        //  Console.WriteLine("[{0}]", string.Join(", ", diagrandom2));

        l++;

        //  Console.WriteLine("here");
        //  Console.WriteLine(l);
        foreach (int i in diagrandom)
        {
            //Console.WriteLine(i);
            mat[l,l] = i;
            l++;
        }
        l = 0;
        foreach (int i in diagrandom2)
        {
            //Console.WriteLine(i);
            int changeL = convertRight[l];
            //  Console.WriteLine(changeL);
            mat[changeL, l] = i;
            l++;
        }
        return false;
    }
    // A recursive function to fill remaining
    // matrix
    int counter2 = 0;
    //bool exit = false;
    int fillRemaining(int i, int j, bool exit)
    {
        /*if (exit)
        {
            return 0;
        }*/
        if (j>=N && i<N-1)
        {
            i = i + 1;
            j = 0;
        }
        if (i>=N && j>=N)
            return 1;
 
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
                    return 1;
            }
        }
 
        counter2++;
        //  Console.WriteLine(counter2);
        if (counter2 >= 7500)
        {
            //  Console.WriteLine("is over 7500");
            return 0;
        }

        //(double, int) gridLocation2 = (i, j);
        //if (!(Array.Exists(doNotOverwriteGrids, element => element == gridLocation2)))
        if (mat[i, j] == 0)
        {
            for (int num = 1; num<=N; num++)
            {
                if (CheckIfSafe(i, j, num))
                {
                    mat[i,j] = num;
                    if (fillRemaining(i, j+1, false) == 1)
                        return 1;
                    mat[i, j] = 0;
                    //exit = true;
                    //return 2;
                }
                //else {return false}
            }
            //return 0;
        }
        else
        {
            if (fillRemaining(i, j+1, false) == 1)
            {
                return 1;
            }
        }
        return 0;
    }
 
    // Remove the K no. of digits to
    // complete game
    public void removeKDigits()
    {
        Console.WriteLine("here1");
        int countk = K;
        while (countk != 0)
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
                countk--;
                mat[i,j] = 0;
            }
            
        }
        Console.WriteLine("here");
    }
 
    public void RemoveKDigitsV2()
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

    public bool CheckIfComplete()
    {
        for (int i = 0; i<N; i++)
        {
            for (int j = 0; j<N; j++)
            {
                if (mat[i, j] == 0) {return false;}
            }
            //Console.WriteLine();
        }
        return true;
    }

    // Print sudoku
    public bool printSudoku(int actualIter)
    {
        /*for (int i = 0; i<N; i++)
        {
            for (int j = 0; j<N; j++)
                Console.Write(mat[i,j] + " ");
            Console.WriteLine();
        }*/

        //int countExit = 0;
        Console.WriteLine(actualIter);
        string path = @"Documents\OUTPUTSUDOKU\" + actualIter.ToString() + ".txt";
        using (StreamWriter sw = File.CreateText(path))
        {
             for (int i = 0; i<N; i++)
            {
                for (int j = 0; j<N; j++)
                {
                    if (sMat[i, j] == 0) {goto Exit;}
                    else {sw.Write(sMat[i,j] + " ");}
                }
                sw.Write("\n");
            }
            
            sw.Flush();
            sw.Close();
            //sw.WriteLine();
            return true;   
        }
    Exit:
        Console.WriteLine("unsolvable");
        return false;
    }

    public void unsolvedPrintSudoku()
    {
        int countExit = 0;
        for (int i = 0; i<N; i++)
        {
            for (int j = 0; j<N; j++)
            {
                if (mat[i, j] == 0)
                {
                    countExit++;
                    if (countExit > K) {goto Exit;}
                }
                else {Console.Write(mat[i,j] + " ");}
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    Exit:
        Console.WriteLine("unsolvable");
    }

    public void unsolvedPrintSudoku2(int actualIter, string diff)
    {
        string path = string.Format(@"D:\OUTPUTSUDOKU\XSudoku\{0}\Unsolved\{1}.txt", diff, actualIter.ToString());
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
        int actualIter = 1;
		string dif = "Very Easy";
		Random rnd = new Random();
        for (int gen = 0; gen < 900; gen++)
        {
            int N = 9, K = rnd.Next(40, 45); //50-55, 55-60, 65, 70, 73
            Sudoku sudoku = new Sudoku(N, K);
            if (sudoku.fillValues(actualIter, dif) == true)
            {
                actualIter++;
                //sudoku.printSudoku(actualIter);
                sudoku.unsolvedPrintSudoku2(actualIter-1, dif);
            }
        }
    }
}
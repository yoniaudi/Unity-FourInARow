using UnityEngine;

public class ComputerPlayerAIModel : MonoBehaviour
{
    [SerializeField] private Color m_WhiteGamePieceColor = Color.white;
    [SerializeField] private Color m_RedGamePieceColor = Color.red;
    [SerializeField] private Color m_GreenGamePieceColor = Color.green;
    [SerializeField] private GameObject m_WhiteGamePiece;
    [SerializeField] private GameObject m_RedGamePiece;
    [SerializeField] private GameObject m_GreenGamePiece;
    public const string k_Name = "Computer AI";
    public int RowMoveInput { get; set; } = 0;
    public int ColumnMoveInput { get; set; } = 0;
    public GameObject[,] Board { get; set; } = null;

    private void Start()
    {
        Board = FindObjectOfType<BoardGameModel>().GetBoardGame();
    }

    public int GetBestMove()
    {
        int bestMove = -1;
        int bestScore = int.MinValue;
        int row = 0;
        int score = 0;

        for (int column = 0; column < Board.GetLength(1); column++)
        {
            if (isColumnFull(column))
            {
                continue;
            }

            row = getEmptyRow(column);
            Board[row, column].GetComponent<SpriteRenderer>().color = m_RedGamePieceColor;
            score = evaluateBoard(m_RedGamePieceColor);
            Board[row, column].GetComponent<SpriteRenderer>().color = m_WhiteGamePieceColor;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = column;
                RowMoveInput = row;
                ColumnMoveInput = bestMove;
            }
        }

        return bestMove;
    }

    private bool isColumnFull(int i_Column)
    {
        return Board[0, i_Column].GetComponent<SpriteRenderer>().color != m_WhiteGamePieceColor;
    }

    private int getEmptyRow(int i_Column)
    {
        int emptyRowIndex = -1;

        for (int i = Board.GetLength(0) - 1; i >= 0; i--)
        {
            if (Board[i, i_Column].GetComponent<SpriteRenderer>().color == m_WhiteGamePieceColor)
            {
                emptyRowIndex = i;
                break;
            }
        }

        return emptyRowIndex;
    }

    private int evaluateBoard(Color i_ComputerPlayerGamePiece)
    {
        Color opponentGamePiece = m_GreenGamePieceColor;
        int score = 0;

        score += evaluatePotentialWins(i_ComputerPlayerGamePiece, false);
        score -= evaluatePotentialWins(opponentGamePiece, true);

        return score;
    }

    private int evaluatePotentialWins(Color i_Player, bool i_IsOpponent)
    {
        int score = 0;

        score += evaluateDirection(i_Player, 1, 0, i_IsOpponent);
        score += evaluateDirection(i_Player, 0, 1, i_IsOpponent);
        score += evaluateDirection(i_Player, 1, 1, i_IsOpponent);
        score += evaluateDirection(i_Player, 1, -1, i_IsOpponent);

        return score;
    }

    private int evaluateDirection(Color i_Player, int i_RowDirection, int i_ColDirection, bool i_IsOpponent)
    {
        int score = 0;

        for (int row = 0; row < Board.GetLength(0); row++)
        {
            for (int col = 0; col < Board.GetLength(1); col++)
            {
                int count = 0;
                int emptyCount = 0;

                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + i * i_RowDirection;
                    int newCol = col + i * i_ColDirection;

                    if (newRow >= 0 && newRow < Board.GetLength(0) && newCol >= 0 && newCol < Board.GetLength(1))
                    {
                        if (Board[newRow, newCol].GetComponent<SpriteRenderer>().color == i_Player)
                        {
                            count++;
                        }
                        else if (Board[newRow, newCol].GetComponent<SpriteRenderer>().color == m_WhiteGamePieceColor)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            count = 0;
                            emptyCount = 0;
                            break;
                        }
                    }
                    else
                    {
                        count = 0;
                        emptyCount = 0;
                        break;
                    }
                }

                score += i_IsOpponent ? countToScoreForOpponent(count, emptyCount) : countToScore(count, emptyCount);
            }
        }

        return score;
    }

    private int countToScore(int i_Count, int i_EmptyCount)
    {
        switch (i_Count)
        {
            case 1:
                return i_EmptyCount == 3 ? 1 : 0;
            case 2:
                return i_EmptyCount == 2 ? 10 : 0;
            case 3:
                return i_EmptyCount == 1 ? 100 : 0;
            case 4:
                return 500;
            default:
                return 0;
        }
    }

    private int countToScoreForOpponent(int i_Count, int i_EmptyCount)
    {
        switch (i_Count)
        {
            case 1:
                return i_EmptyCount == 3 ? 1 : 0;
            case 2:
                return i_EmptyCount == 2 ? 10 : 0;
            case 3:
                return i_EmptyCount == 1 ? 500 : 0;
            case 4:
                return 1000;
            default:
                return 0;
        }
    }
}
